using UnityEngine;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net;
using MsgPack;

namespace MLPlayer
{
	public class AIServer : MonoBehaviour
	{
		private WebSocketServer wssv;
		                                           //*******************************************************
		private Queue<byte[]> agentMessageQueue;  //ここのagentMessageQueueのデータを下にあるクラスのCommuncationGymに持っていきたい
		private Queue<byte[]> aiMessageQueue;     //*******************************************************
		public Agent agent;
		private MsgPack.CompiledPacker packer;

		public AIServer (Agent _agent) {
			packer = new MsgPack.CompiledPacker();
			agentMessageQueue = new Queue<byte[]>();
			aiMessageQueue = new Queue<byte[]>();
			agent = _agent;
		}
			
		public class CommunicationGym : WebSocketBehavior
		{
			public Agent agent { set; get; }
			public State CheckMSG = new State ();  
 			MsgPack.BoxingPacker packer = new MsgPack.BoxingPacker ();

			//*******************************************
			//private Queue<byte[]> agentMessageQueue; こうすると当然別のキューが作成されてしまう...
			//*******************************************


			protected override void OnMessage (MessageEventArgs e)
			{
				//receive message from GYM
				agent.action.Set((Dictionary<System.Object,System.Object>)packer.Unpack (e.RawData));
				Debug.Log ("Rotate="+agent.action.rotate+" Forword="+agent.action.forward+" Jump="+agent.action.jump);


				//byte[] data = PopAgentState();
				//Send (data);


				SendMessage (CheckMSG);
			}
				
			public void SendMessage(State s){
				//Send state datas to GYM
				var packer2 = new MsgPack.CompiledPacker ();
				byte[] msg = packer2.Pack (s);
				Send (msg);
			}
				
			protected override void OnOpen ()
			{
				Debug.Log ("Socket Open");
			}

		
		}

		CommunicationGym instantiate() {
			CommunicationGym service = new CommunicationGym();
			service.agent = agent;
			return service;
		}

		void Awake ()
		{
			wssv = new WebSocketServer ("ws://localhost:" + 4649);
			wssv.AddWebSocketService<CommunicationGym> ("/CommunicationGym", instantiate);
			wssv.Start ();

			if (wssv.IsListening) {
				Debug.Log ("Listening on port " + wssv.Port + ", and providing WebSocket services:");
				foreach (var path in wssv.WebSocketServices.Paths)
					Debug.Log ("- " + path);
			}
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
			byte[] msg = packer.Pack(s);     //*********************************
			agentMessageQueue.Enqueue(msg);  //キューには画像データが入っていることを確認済み
		}                          

		public byte[] PopAgentState() {
			byte[] received = null;
			if( agentMessageQueue.Count > 0 ) {
				received = agentMessageQueue.Dequeue();
			}

			return received;
		}
			
		void OnApplicationQuit ()
		{
			wssv.Stop ();
			Debug.Log ("websocket server exiteed");
		}
	}
}
