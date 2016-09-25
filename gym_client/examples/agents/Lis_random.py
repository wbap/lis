import gym

reward_sum = 0
total_episode = 20
episode_count = 1

env = gym.make('Lis-v2')

while episode_count <= total_episode:
    observation = env.reset()
    for t in range(100):
        action = env.action_space.sample() #take a random action
        observation, reward, end_episode, info =env.step(action)
        reward_sum += reward
        print(" episode: "+str(episode_count)+" , step: "+str(t+1)+" , reward: "+str(reward))
        if end_episode:
            print("Episode finished after {} timesteps".format(t+1)+" , reward sum: "+str(reward_sum))
            episode_count += 1
            reward_sum = 0
            break

