import matplotlib.pyplot as plt
import pandas as pd
import argparse

parser = argparse.ArgumentParser()
parser.add_argument('--log-file', '-l', default='reward.log', type=str,
                    help='reward log file name')
args = parser.parse_args()

df = pd.read_csv(args.log_file)
x = df.columns[0]
y = df.columns[1]
ax = df.plot(kind='scatter', x=x, y=y)
df[y] = pd.rolling_mean(df[y], window=20)
df.plot(kind='line', x=x, y=y, ax=ax)
plt.show()
