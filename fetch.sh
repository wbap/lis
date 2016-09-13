
#!/bin/bash

echo "download caffemodel..."
curl -o gym_client/examples/agents/bvlc_alexnet.caffemodel http://dl.caffe.berkeleyvision.org/bvlc_alexnet.caffemodel

curl -f -L -o gym_client/examples/agents/ilsvrc_2012_mean.npy https://github.com/BVLC/caffe/raw/master/python/caffe/imagenet/ilsvrc_2012_mean.npy

