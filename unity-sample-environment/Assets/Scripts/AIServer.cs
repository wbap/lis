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
						
		//更新(8/27)
		public class CommunicationGym : WebSocketBehavior
		{
			public Action action { set; get; }
			public State CheckMSG = new State ();  //メッセージパックで送信できるかの確認（中身は空）
 			MsgPack.BoxingPacker packer = new MsgPack.BoxingPacker ();

		
			protected override void OnMessage (MessageEventArgs e)
			{
				action = new Action ();

				//receive message from GYM
				action.Set((Dictionary<System.Object,System.Object>)packer.Unpack (e.RawData));
				Debug.Log ("Rotate="+action.rotate+" Forword="+action.forward+" Jump="+action.jump);

				//CommunicationGymではStateデータを読み込めない...
				//ここにStateデータを持ってきたい....
				//他クラスのデータ参照ができれば良いが...

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

		void Awake ()
		{
			wssv = new WebSocketServer ("ws://localhost:" + 4649);
			//wssv.WaitTime = TimeSpan.FromSeconds (2);
			wssv.AddWebSocketService<CommunicationGym> ("/CommunicationGym");
			wssv.Start ();
		
	
			if (wssv.IsListening) {
				Debug.Log ("Listening on port " + wssv.Port + ", and providing WebSocket services:");
				foreach (var path in wssv.WebSocketServices.Paths)
					Debug.Log ("- " + path);
			}
		}

		void OnApplicationQuit ()
		{
			wssv.Stop ();
			Debug.Log ("websocket server exiteed");
		}
	}
}
