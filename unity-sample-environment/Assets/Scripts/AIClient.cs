using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using MsgPack;

namespace MLPlayer
{
	// for sync communication
	public class AIClient : IAIClient {
		
		private Queue<byte[]> agentMessageQueue;
		private Queue<byte[]> aiMessageQueue;
		private string url;
		private Thread th;
		private Mutex mutAgent;
		private Mutex mutAi;
		private MsgPack.CompiledPacker packer;
		
		public delegate void MassageCB(byte[] msg, Agent agent);
		MassageCB messageCallBack;
		Agent agent;
		
		public AIClient (string _url, MassageCB cb, Agent _agent) {
			url = _url;
			messageCallBack = cb;
			agent = _agent;
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

			ws.OnMessage += (sender, e) => MassageCallBack(e.RawData);
			
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
					//Thread.Sleep(0);
				}
			}
		}
		
		private void MassageCallBack(byte[] msg) {
			messageCallBack(msg, agent);
		}

		public void PushAIMessage (byte[] msg)
		{
			throw new System.NotImplementedException ();
		}

		public byte[] PopAIMessage ()
		{
			throw new System.NotImplementedException ();
		}

		public void PushAgentState(State s) {
			byte[] msg = packer.Pack(s);
			mutAgent.WaitOne();
			agentMessageQueue.Enqueue(msg);
			mutAgent.ReleaseMutex();
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