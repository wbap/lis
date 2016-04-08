# LIS (Life in Silico)
=============

![screenshot](https://cloud.githubusercontent.com/assets/1708549/14311902/c6ce61ec-fc24-11e5-8018-5e3aaf98b6d3.png)

## Algorithm
<img width="371" alt="2016-04-08 19 00 14" src="https://cloud.githubusercontent.com/assets/1708549/14380751/2fec7856-fdbc-11e5-9ac0-17650fc74f34.png">

## Requirements:
- python 2.7

## Install
### Ubuntu 
Install Unity (experimental-build version):

```
wget http://download.unity3d.com/download_unity/unity-editor-installer-5.1.0f3+2015082501.sh
sudo sh unity-editor-installer-5.1.0f3+2015082501.sh

# run Unity
./unity-editor-5.1.0f3/Editor/Unity

# if background is pink, install:
sudo apt-get install lib32stdc++6 -y
```

install python modules:
```
pip install chainer
pip install ws4py
pip install cherrypy
pip install msgpack-python
```

### Mac
Install Unity. (if you are going to use Ubuntu for GPU pwer, I reccomend install Unity 5.1.0 linux experimental-build version)

install python modules:
```
pip install chainer
pip install ws4py
pip install cherrypy
pip install msgpack-python
```

### Windows

[超人工生命ハッカソンに向けてWindows10マシンにpython環境を整えた](http://qiita.com/autani/items/4daa5587773631245d86)

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

Open unity-sample-environment in Unity and load Scenes/sample.

![screenshot from 2016-04-06 18 08 31](https://cloud.githubusercontent.com/assets/1708549/14311462/990e607e-fc22-11e5-84cf-26c049482afc.png)

Press Start Buttn. This will take a few minuts for loading caffe model.

![screenshot from 2016-04-06 18 09 36](https://cloud.githubusercontent.com/assets/1708549/14311518/c309f8f2-fc22-11e5-937c-abd0d227d307.png)

You can watch reward history:

```
cd python-agent
python plot_reward_log.py
```

<img width="400" alt="screenshot" src="https://cloud.githubusercontent.com/assets/1708549/14384486/46ace0b6-fdd6-11e5-86be-3eda63712ebe.png">

This graph is a "sample" scene result. It takes about 6 hours on GPU Machine. 


[Sample scene result movie](https://www.youtube.com/watch?v=7Ein1hRUQ_U)

[SampleLikesAndDislikes scene result movie](https://www.youtube.com/watch?v=IERCgdG1_fw)


## System Configuration

- Client: Unity
- Server: python module
- Communication: Socket (WebSocket over TCP) using MessagePack

<img width="300" alt="2016-04-09 4 14 49" src="https://cloud.githubusercontent.com/assets/1708549/14394932/bbd77756-fe09-11e5-89ba-da7834c2a39e.png">

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
+ Apache License, Version 2.0
+ This framework was made by [DWANGO ARTIFICIAL INTELLIGENCE LABORATORY](http://ailab.dwango.co.jp/en/) and lab member [Masayoshi Nakamura](http://masayosshi.com/)

## Notice
If you created intelligent agents, please let me know about it to "masayoshi_nakamura@dwango.co.jp". We will make showcase varied intelligent agents.
