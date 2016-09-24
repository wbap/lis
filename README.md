# LIS (Life in Silico) ver2
=============
LIS (Life In Silico) is a framework that makes intelligent agents _live_ in a virtual environment.  
LIS version 2 uses [Unity Game Engine](https://unity3d.com) for the virtual environment and [OpenAI Gym](https://gym.openai.com) for the learning agent framework.


![screenshot](https://cloud.githubusercontent.com/assets/1708549/14311902/c6ce61ec-fc24-11e5-8018-5e3aaf98b6d3.png)

## Algorithm
<img width="470" alt="Lisv2algorithm" src="https://cloud.githubusercontent.com/assets/21034484/18500782/accc85a0-7a85-11e6-8957-cfc9cd87fdc6.png">

### Algorithm Reference 
+ Mnih, V. et al. Human-level control through deep reinforcement learning. Nature 518, 529–533 (2015)
 + http://www.nature.com/nature/journal/v518/n7540/abs/nature14236.html
 + [DQN-chainer](https://github.com/ugo-nama-kun/DQN-chainer)
 
+ A. Krizhevsky, I. Sutskever, and G. Hinton. ImageNet classification with deep convolutional neural networks. In NIPS, 2012.
 + [Caffe Model Zoo](https://github.com/BVLC/caffe/wiki/Model-Zoo)
  
## Requirements
- python 2.7

## Install
### Ubuntu 
Install [Unity experimental-build version](http://forum.unity3d.com/threads/unity-on-linux-release-notes-and-known-issues.350256/):

```
wget http://download.unity3d.com/download_unity/linux/unity-editor-installer-5.3.4f1+20160317.sh
sudo sh unity-editor-installer-5.3.4f1+20160317.sh

# run Unity
./unity-editor-5.3.4f1/Editor/Unity

# if background is pink, install:
sudo apt-get install lib32stdc++6 -y
```

install python modules:
```
pip install -r python-agent/requirements.txt
```

### Mac
Install Unity. 

install python modules:
```
pip install -r python-agent/requirements.txt
```

### Windows

[Building simulator on Windows10](http://qiita.com/autani/items/4daa5587773631245d86) (Japanese)

## Quick Start
download data:

```
./fetch.sh
```

Open unity-sample-environment with Unity and load Scenes/Sample.

![2016-09-13 10 27 53](https://cloud.githubusercontent.com/assets/21034484/18458591/0d40c912-799d-11e6-88da-5af8018fc784.png)

Press Start Button. 

![2016-09-13 10 28 14](https://cloud.githubusercontent.com/assets/21034484/18458604/342c6eaa-799d-11e6-987b-cbc06b00f497.png)

Next, run python module as a client.This will take a few minutes for loading caffe model.

```
cd gym_client/examples/agents
PYTHONPATH=../../ python Lis_dqn.py
```

You can watch reward history:

```
cd gym_client/examples/agents
python plot_reward_log.py
```

<img width="400" alt="screenshot" src="https://cloud.githubusercontent.com/assets/1708549/14384486/46ace0b6-fdd6-11e5-86be-3eda63712ebe.png">

This graph is a "sample" scene result. It takes about 6 hours on GPU Machine. 


[Sample scene result movie](https://www.youtube.com/watch?v=7Ein1hRUQ_U)

[SampleLikesAndDislikes scene result movie](https://www.youtube.com/watch?v=IERCgdG1_fw)

## Examples

See the examples directory
- Run examples/agents/Lis_random.py to run an simple random agent
- Run examples/agents/Lis_dqn.py to run an Deep Q-Network agent

## System Configuration

- Client: python module(gym)
- Server: Unity
- Communication: Socket (WebSocket over TCP) using MessagePack

<img width="300" alt="2016-04-09 4 14 49" src="https://cloud.githubusercontent.com/assets/21034484/18440301/9265bbf2-7943-11e6-93a5-93d49d98f8d6.png">

## Tips
### Simulate faster
Select "SceneController" in Hierarchy tab and change "Time Scale". 

<img width="292" alt="2016-04-23 15 52 03" src="https://cloud.githubusercontent.com/assets/1708549/14759823/631807d0-096b-11e6-9dc9-d2cc4280aee7.png">

This will make simulation more faster, but it will be slow gui response.


## Module Reference

+ MessagePack for Unity 
 + Copyright (C) 2011-2012 Kazuki Oikawa, Kazunari Kida
 + Apache License, Version 2.0
 + Assets/Packages/msgpack-unity 

+ websocket-sharp
 + Copyright (c) 2010-2016 sta.blockhead
 + The MIT License (MIT)
 + Assets/Packages/websocket-sharp

+ websocket-client
 + Copyright (C) 2010 Hiroki Ohtani(liris)
 + LGPL License 

+ gym
 + Copyright (c) 2016 OpenAI (http://openai.com)
 + The MIT License (MIT) 
 + LIS-ver2/gym_client/gym 

## License
+ Apache License, Version 2.0
+ Original Developer: ([DWANGO ARTIFICIAL INTELLIGENCE LABORATORY](http://ailab.dwango.co.jp/en/))

## Notice
If you created intelligent agents, please let me know about it to "masayoshi_nakamura@dwango.co.jp". We will make showcase varied intelligent agents.
