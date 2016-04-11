# -*- coding: utf-8 -*-
from __future__ import print_function
import sys

import numpy as np
import chainer
from chainer import cuda
import chainer.functions as F
from chainer.links import caffe


class CnnFeatureExtractor:
    def __init__(self, gpu, model_file, in_size, mean_file, feature_name):
        self.gpu = gpu
        self.model_file = model_file
        self.mean_file = mean_file
        self.feature_name = feature_name
        self.in_size = in_size
        self.batchsize = 1

        if self.gpu >= 0:
            cuda.check_cuda_available()

        print('Loading Caffe model file %s...' % self.model_file, file = sys.stderr)
        self.func = caffe.CaffeFunction(self.model_file)
        print('Loaded', file=sys.stderr)
        if self.gpu >= 0:
            cuda.get_device(self.gpu).use()
            self.func.to_gpu()

        mean_image = np.load(self.mean_file)
        self.mean_image = self.crop(mean_image)

    def forward(self, x, t):
        y, = self.func(inputs={'data': x}, outputs=[self.feature_name], train=False)
        return F.softmax_cross_entropy(y, t), F.accuracy(y, t)

    def predict(self, x):
        y, = self.func(inputs={'data': x}, outputs=[self.feature_name], train=False)
        return F.softmax(y)

    def feature(self, camera_image):
        x_batch = np.ndarray((self.batchsize, 3, self.in_size, self.in_size), dtype=np.float32)
        image = np.asarray(camera_image).transpose(2, 0, 1)[::-1].astype(np.float32)
        image = self.crop(image)
        image -= self.mean_image

        x_batch[0] = image
        xp = cuda.cupy if self.gpu >= 0 else np
        x_data = xp.asarray(x_batch)

        if self.gpu >= 0:
            x_data=cuda.to_gpu(x_data)

        x = chainer.Variable(x_data, volatile=True)
        feature = self.predict(x)
        feature = feature.data

        if self.gpu >= 0:
            feature = cuda.to_cpu(feature)
        feature = self.vec(feature)

        return feature * 255.0

    def crop(self, image):
        #assume image is square
        cropwidth = image.shape[1] - self.in_size
        start = cropwidth // 2
        stop = start + self.in_size
        return image[:, start:stop, start:stop].copy()

    #vectrization, or mat[:] in MATLAB
    def vec(self, mat):
        return mat.reshape(mat.size)
