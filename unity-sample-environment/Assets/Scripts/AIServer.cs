using UnityEngine;
using System;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net;


public class AIServer : MonoBehaviour {

	private WebSocketServer wssv;

	//動作確認のためエコーサーバを構築
	public class Echo : WebSocketBehavior
	{
		protected override void OnMessage (MessageEventArgs e)
		{
			var name = Context.QueryString ["name"];
			Send (!name.IsNullOrEmpty () ? String.Format ("\"{0}\" to {1}", e.Data, name) : e.Data);
		}
		protected override void OnOpen(){
			Debug.Log ("Socket Open");
		}

	}

	void Awake()
	{
		wssv = new WebSocketServer ("ws://localhost:"+4649);
		//wssv.WaitTime = TimeSpan.FromSeconds (2);
		wssv.AddWebSocketService<Echo> ("/Echo");
		wssv.Start ();


		if (wssv.IsListening) {
			Debug.Log ("Listening on port "+wssv.Port+", and providing WebSocket services:");
			foreach (var path in wssv.WebSocketServices.Paths)
				Debug.Log ("- " + path);
		}
	}

	void OnApplicationQuit()
	{
		wssv.Stop ();
		Debug.Log ("websocket server exiteed");
	}
}
