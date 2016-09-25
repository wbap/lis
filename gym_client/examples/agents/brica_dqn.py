#!/usr/bin/env python

import argparse
from cnn_dqn_agent import CnnDqnAgent
import gym
import logging
import numpy as np

import brica1

print brica1.__file__

import brica1.gym

class CNNDQNComponent(brica1.Component):
    def __init__(self, cnn_dqn_agent):
        super(CNNDQNComponent, self).__init__()

        self.agent = cnn_dqn_agent

        self.make_in_port('observation', 1)
        self.make_in_port('reward', 1)
        self.make_in_port('done', 1)
        self.make_in_port('info', 1)
        self.make_out_port('action', 1)

        self.get_out_port('action').buffer = np.array([0])
        self.results['action'] = np.array([0])

    def fire(self):
        observation = self.inputs['observation']
        reward = self.inputs['reward']
        done = self.inputs['done']
        info = self.inputs['info']

        action = 0

        if done:
            self.agent.end(reward)
            action = self.agent.agent_start(observation)
        else:
            action, eps, q_now, obs_array = self.agent.agent_step(reward, observation)
            self.agent.agent_step_update(reward, action, eps, q_now, obs_array)

        self.results['action'] = np.array([action])

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Process some integers.')
    parser.add_argument('--gpu', '-g', default=-1, type=int,
                        help='GPU ID (negative value indicates CPU)')
    parser.add_argument('--log-file', '-l', default='reward.log', type=str,
                        help='reward log file name')
    args = parser.parse_args()

    agent = CnnDqnAgent()
    cycle_counter = 0
    log_file = args.log_file
    reward_sum = 0
    depth_image_dim = 32 * 32
    depth_image_count = 1
    total_episode = 10000
    episode_count = 0

    agent.agent_init(use_gpu=args.gpu, depth_image_dim=depth_image_dim * depth_image_count)

    env = gym.make('Lis-v2')

    observation = env.reset()

    agent.agent_start(observation)

    cnn_dqn = CNNDQNComponent(agent)

    agent = brica1.gym.GymAgent(cnn_dqn, env)
    scheduler = brica1.VirtualTimeSyncScheduler(agent)

    for _ in range(10000):
        scheduler.step()

