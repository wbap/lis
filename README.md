under construction!
# Machine Learning Agent for Unity
=============

![screenshot](https://cloud.githubusercontent.com/assets/1708549/14311902/c6ce61ec-fc24-11e5-8018-5e3aaf98b6d3.png)

## Algorithm
<img width="359" alt="2016-04-07 11 15 34" src="https://cloud.githubusercontent.com/assets/1708549/14338417/1d18b66e-fcb2-11e5-8fd2-a86b092edfb2.png">

## Requirements:
- python 2.7

## Install
### Ubuntu 
Install Unity (experimental-build version):

```
wget http://download.unity3d.com/download_unity/unity-editor-installer-5.1.0f3+2015082501.sh
sudo sh unity-editor-installer-5.1.0f3+2015082501.sh

# run
./unity-editor-5.1.0f3/Editor/Unity

# if background is pink, install:
sudo apt-get install lib32stdc++6 -y
```

install python modules
```
pip install chainer
pip install ws4py
pip install cherrypy
pip install msgpack-python
```

### Mac
Install Unity. (if you are going to use Ubuntu for GPU pwer, I reccomend install Unity 5.1.0 linux experimental-build version)

install python modules
```
pip install chainer
pip install ws4py
pip install cherrypy
pip install msgpack-python
```

### Windows

not supported

## Quick Start
download data:

```
./fetch.sh
```

Next, run python module as a server.

```
cd python-agent
python server.py
```

Open unity-sample-environment in Unity and load Scenes/sample

![screenshot from 2016-04-06 18 08 31](https://cloud.githubusercontent.com/assets/1708549/14311462/990e607e-fc22-11e5-84cf-26c049482afc.png)

Press Start Buttn. In first time, this will take a few minuts time.

![screenshot from 2016-04-06 18 09 36](https://cloud.githubusercontent.com/assets/1708549/14311518/c309f8f2-fc22-11e5-937c-abd0d227d307.png)

You can watch reward history python_agent/reward.log

<img width="300" alt="screenshot" src="https://cloud.githubusercontent.com/assets/1708549/14312192/20292fdc-fc26-11e5-9b18-39c3c5ea113f.png">


## System Architecture


- Client: Unity
- Server: python module
- Communication: Socket (WebSocket over TCP) using MessagePack

<img width="200" alt="screenshot" src="https://cloud.githubusercontent.com/assets/1708549/14312101/b06f0310-fc25-11e5-9366-f41bfa414d90.png">

## Tips
### Simulate faster

<img width="400" alt="screenshot" src="https://cloud.githubusercontent.com/assets/1708549/14313279/9aa6b66c-fc2b-11e5-915e-796348fbcdec.png">

Set "Time Scale" to 100. This will make simulation more faster, but GUI response will be very slow.
<img width="250" alt="2016-04-06 19 06 50" src="https://cloud.githubusercontent.com/assets/1708549/14313288/aa9aab28-fc2b-11e5-8965-b69b0ecef151.png">
>


## Algorithm Reference 
+ Mnih, V. et al. Human-level control through deep reinforcement learning. Nature 518, 529–533 (2015)
 + http://www.nature.com/nature/journal/v518/n7540/abs/nature14236.html
 + [DQN-chainer](https://github.com/ugo-nama-kun/DQN-chainer)
 
+ A. Krizhevsky, I. Sutskever, and G. Hinton. ImageNet classification with deep convolutional neural networks. In NIPS, 2012.
 + [Caffe Model Zoo](https://github.com/BVLC/caffe/wiki/Model-Zoo)

## Module Reference

+ MessagePack for Unity 
 + Copyright (C) 2011-2012 Kazuki Oikawa, Kazunari Kida
 + Apache License, Version 2.0
 + Assets/Packages/msgpack-unity 

+ websocket-sharp
 + Copyright (c) 2010-2016 sta.blockhead
 + The MIT License (MIT)
 + Assets/Packages/websocket-sharp


## License
This program was made by [DWANGO ARTIFICIAL INTELLIGENCE LABORATORY](http://ailab.dwango.co.jp/en/) and lab member [Masayoshi Nakamura](http://masayosshi.com/)
