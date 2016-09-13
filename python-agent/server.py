# -*- coding: utf-8 -*-

import cherrypy
import argparse
from ws4py.server.cherrypyserver import WebSocketPlugin, WebSocketTool
from ws4py.websocket import WebSocket
from cnn_dqn_agent import CnnDqnAgent
import msgpack
import io
from PIL import Image
from PIL import ImageOps
import threading
import numpy as np

parser = argparse.ArgumentParser(description='ml-agent-for-unity')
parser.add_argument('--port', '-p', default='8765', type=int,
                    help='websocket port')
parser.add_argument('--ip', '-i', default='127.0.0.1',
                    help='server ip')
parser.add_argument('--gpu', '-g', default=-1, type=int,
                    help='GPU ID (negative value indicates CPU)')
parser.add_argument('--log-file', '-l', default='reward.log', type=str,
                    help='reward log file name')
args = parser.parse_args()


class Root(object):
    @cherrypy.expose
    def index(self):
        return 'some HTML with a websocket javascript connection'

    @cherrypy.expose
    def ws(self):
        # you can access the class instance through
        handler = cherrypy.request.ws_handler


class AgentServer(WebSocket):
    agent = CnnDqnAgent()
    agent_initialized = False
    cycle_counter = 0
    thread_event = threading.Event()
    log_file = args.log_file
    reward_sum = 0
    depth_image_dim = 32 * 32
    depth_image_count = 1

    def send_action(self, action):
        dat = msgpack.packb({"command": str(action)})
        self.send(dat, binary=True)

    def received_message(self, m):
        payload = m.data
        dat = msgpack.unpackb(payload)

        image = []
        for i in xrange(self.depth_image_count):
            image.append(Image.open(io.BytesIO(bytearray(dat['image'][i]))))
        depth = []
        for i in xrange(self.depth_image_count):
            d = (Image.open(io.BytesIO(bytearray(dat['depth'][i]))))
            depth.append(np.array(ImageOps.grayscale(d)).reshape(self.depth_image_dim))

        observation = {"image": image, "depth": depth}
        reward = dat['reward']
        end_episode = dat['endEpisode']

        if not self.agent_initialized:
            self.agent_initialized = True
            print ("initializing agent...")
            self.agent.agent_init(
                use_gpu=args.gpu,
                depth_image_dim=self.depth_image_dim * self.depth_image_count)

            action = self.agent.agent_start(observation)
            self.send_action(action)
            with open(self.log_file, 'w') as the_file:
                the_file.write('cycle, episode_reward_sum \n')
        else:
            self.thread_event.wait()
            self.cycle_counter += 1
            self.reward_sum += reward

            if end_episode:
                self.agent.agent_end(reward)
                action = self.agent.agent_start(observation)  # TODO
                self.send_action(action)
                with open(self.log_file, 'a') as the_file:
                    the_file.write(str(self.cycle_counter) +
                                   ',' + str(self.reward_sum) + '\n')
                self.reward_sum = 0
            else:
                action, eps, q_now, obs_array = self.agent.agent_step(reward, observation)
                self.send_action(action)
                self.agent.agent_step_update(reward, action, eps, q_now, obs_array)

        self.thread_event.set()

cherrypy.config.update({'server.socket_host': args.ip,
                        'server.socket_port': args.port})
WebSocketPlugin(cherrypy.engine).subscribe()
cherrypy.tools.websocket = WebSocketTool()
cherrypy.config.update({'engine.autoreload.on': False})
config = {'/ws': {'tools.websocket.on': True,
                  'tools.websocket.handler_cls': AgentServer}}
cherrypy.quickstart(Root(), '/', config)

