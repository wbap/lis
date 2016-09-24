# coding:utf-8
import argparse
from cnn_dqn_agent import CnnDqnAgent
import gym
from PIL import Image
import numpy as np

parser = argparse.ArgumentParser(description='Process some integers.')
parser.add_argument('--gpu', '-g', default=-1, type=int,
                    help='GPU ID (negative value indicates CPU)')
parser.add_argument('--log-file', '-l', default='reward.log', type=str,
                    help='reward log file name')
args = parser.parse_args()

agent = CnnDqnAgent()
agent_initialized = False
cycle_counter = 0
log_file = args.log_file
reward_sum = 0
depth_image_dim = 32 * 32
depth_image_count = 1
total_episode = 10000
episode_count = 1

while episode_count <= total_episode:
    if not agent_initialized:
        agent_initialized = True
        print ("initializing agent...")
        agent.agent_init(
            use_gpu=args.gpu,
            depth_image_dim=depth_image_dim * depth_image_count)

        env = gym.make('Lis-v2')

        observation = env.reset()
        action = agent.agent_start(observation)
        observation, reward, end_episode, _ = env.step(action)

        with open(log_file, 'w') as the_file:
            the_file.write('cycle, episode_reward_sum \n')
    else:
        cycle_counter += 1
        reward_sum += reward

        if end_episode:
            agent.agent_end(reward)

            action = agent.agent_start(observation)  # TODO
            observation, reward, end_episode, _ = env.step(action)

            with open(log_file, 'a') as the_file:
                the_file.write(str(cycle_counter) +
                               ',' + str(reward_sum) + '\n')
            reward_sum = 0
            episode_count += 1

        else:
            action, eps, q_now, obs_array = agent.agent_step(reward, observation)
            agent.agent_step_update(reward, action, eps, q_now, obs_array)
            observation, reward, end_episode, _ = env.step(action)

env.close()
