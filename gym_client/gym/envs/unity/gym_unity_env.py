# -*- coding: utf-8 -*-

import websocket
import msgpack
import gym
import io
from PIL import Image
from PIL import ImageOps
from gym import spaces
import numpy as np
import time


class GymUnityEnv(gym.Env):

    def __init__(self):
        websocket.enableTrace(True)
    	self.ws = websocket.create_connection("ws://localhost:4649/CommunicationGym")
        self.action_space = spaces.Discrete(3)
        self.depth_image_dim = 32 * 32
        self.depth_image_count = 1
        self.observation, _, _ = self.receive()


    def reset(self):
        return self.observation



    def step(self, action):

        actiondata = msgpack.packb({"command": str(action)})
        self.ws.send(actiondata)

        # Unity Process

        observation, reward, end_episode = self.receive()

        return observation, reward, end_episode, {}

    def receive(self):

        while True:

            statedata = self.ws.recv()

            if not statedata:
                continue

            state = msgpack.unpackb(statedata)

            image = []
            for i in xrange(self.depth_image_count):
                image.append(Image.open(io.BytesIO(bytearray(state['image'][i]))))
            depth = []
            for i in xrange(self.depth_image_count):
                d = (Image.open(io.BytesIO(bytearray(state['depth'][i]))))
                depth.append(np.array(ImageOps.grayscale(d)).reshape(self.depth_image_dim))

            observation = {"image": image, "depth": depth}
            reward = state['reward']
            end_episode = state['endEpisode']

            return observation, reward, end_episode
            break

    def close(self):
        self.ws.close()
