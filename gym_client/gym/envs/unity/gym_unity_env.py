# -*- coding: utf-8 -*-

import websocket
import msgpack
import gym
import io
from PIL import Image
from PIL import ImageOps
from gym import spaces
import numpy as np

class GymUnityEnv(gym.Env):

    def __init__(self): #環境が作られたとき
        websocket.enableTrace(True)
    	self.ws = websocket.create_connection("ws://localhost:4649/CommunicationGym")
        self.action_space = spaces.Discrete(3)  # 3つのアクションをセット
        self.depth_image_dim = 32 * 32
        self.depth_image_count = 1
        self.observation, _, _ = self.receive()


    def reset(self):
        return self.observation



    def step(self, action):  # ステップ処理 、actionを外から受け取る

        actiondata = msgpack.packb({"command": str(action)})  # アクションをpack
        self.ws.send(actiondata)  # 送信

        # Unity Process

        observation, reward, end_episode = self.receive()

        return observation, reward, end_episode, {}

    def receive(self):
        statedata = self.ws.recv()  # 状態の受信
        state = msgpack.unpackb(statedata)  # 受け取ったデータをunpack

        image = []
        for i in xrange(self.depth_image_count):
            image.append(Image.open(io.BytesIO(bytearray(state['image'][i]))))
        depth = []
        for i in xrange(self.depth_image_count):
            d = (Image.open(io.BytesIO(bytearray(state['depth'][i]))))

            #d.save('stephoge.png')

            depth.append(np.array(ImageOps.grayscale(d)).reshape(self.depth_image_dim))

        observation = {"image": image, "depth": depth}
        reward = state['reward']
        end_episode = state['endEpisode']

        return observation, reward, end_episode


    def close(self):  # コネクション終了処理
        self.ws.close()  # コネクション終了
