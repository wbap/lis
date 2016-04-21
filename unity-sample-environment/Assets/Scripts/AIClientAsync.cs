using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using MsgPack;

namespace MLPlayer {

	// for async communication
	public class AIClientAsync : IAIClient {
		
		private Queue<byte[]> agentMessageQueue;
		private Queue<byte[]> aiMessageQueue;
		private string url;
		private Thread th;
		private Mutex mutAgent;
		private Mutex mutAi;
		private MsgPack.CompiledPacker packer;
		public delegate void OnMessageFunc();
		OnMessageFunc onMessageFunc;
		
		public AIClientAsync (string _url) {
			url = _url;
			mutAgent = new Mutex();
			mutAi = new Mutex();
			packer = new MsgPack.CompiledPacker();
			agentMessageQueue = new Queue<byte[]>();
			aiMessageQueue = new Queue<byte[]>();
			th = new Thread(new ThreadStart(ExecuteInForeground));
			th.Start(this);
		}
		
		private void ExecuteInForeground() {
			
			WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket (url);
			Debug.Log("connecting... " + url);
			
			ws.OnMessage += (sender, e) => OnMassage(e.RawData);
			
			while (true) {
				ws.Connect ();
				
				while (!ws.IsConnected) {
					Thread.Sleep(1000);
				}
				
				while (ws.IsConnected) {
					byte[] data = PopAgentState();
					if(data != null) {
						ws.Send(data);
					}
					//Thread.Sleep(8);
				}
			}
		}
		
		private void OnMassage(byte[] msg) {
			PushAIMessage(msg);
			if (onMessageFunc != null) {
				onMessageFunc();
			}
		}
		
		public void PushAgentState(State s) {
			byte[] msg = packer.Pack(s);
			mutAgent.WaitOne();
			agentMessageQueue.Enqueue(msg);
			mutAgent.ReleaseMutex();
		}
		
		public void PushAIMessage(byte[] msg) {
			mutAi.WaitOne();
			aiMessageQueue.Enqueue(msg);
			mutAi.ReleaseMutex();
		}
		
		public byte[] PopAIMessage() {
			byte[] received = null;
			
			mutAi.WaitOne();
			if( aiMessageQueue.Count > 0 ) {
				received = aiMessageQueue.Dequeue();
			}
			mutAi.ReleaseMutex();
			
			return received;
		}
		
		public byte[] PopAgentState() {
			byte[] received = null;
			
			mutAgent.WaitOne();
			if( agentMessageQueue.Count > 0 ) {
				received = agentMessageQueue.Dequeue();
			}
			mutAgent.ReleaseMutex();
			
			return received;
		}
	}
}