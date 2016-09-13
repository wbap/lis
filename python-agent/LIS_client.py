from __future__ import print_function
from random import Random
import struct
import websocket
import msgpack
import io

if __name__ == "__main__":
    websocket.enableTrace(True)
    ws = websocket.create_connection("ws://localhost:4649/CommunicationGym")
    print("Sending Data")
    r = Random()
    action = r.randint(0,3)
    dat1 = msgpack.packb({"command": str(action)})
    ws.send(dat1);
    print("Sent")
    print("Receiving...")
    result = ws.recv()
    dat=msgpack.unpackb(result)
    print("Received '%s'" % dat)
    ws.close()
