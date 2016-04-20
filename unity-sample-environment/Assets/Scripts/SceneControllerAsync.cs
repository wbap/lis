using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using MsgPack;

namespace MLPlayer {

	public class AIClient {

		private Queue<byte[]> agentMessageQueue;
		private Queue<byte[]> aiMessageQueue;
		private string url;
		private Thread th;
		private Mutex mutAgent;
		private Mutex mutAi;
		private MsgPack.CompiledPacker packer;

		public AIClient (string _url) {
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

	public class SceneControllerAsync : MonoBehaviour {

		// singleton
		protected static SceneControllerAsync instance;
		public static SceneControllerAsync Instance {
			get {
				if(instance == null) {
					instance = (SceneControllerAsync) FindObjectOfType(typeof(SceneControllerAsync));
					if (instance == null) {
						Debug.LogError("An instance of " + typeof(SceneControllerAsync) + 
						               " is needed in the scene, but there is none.");
					}
				}
				return instance;
			}
		}
		
		[SerializeField] string url;
		[SerializeField] float cycleTimeStepSize;
		[SerializeField] float episodeTimeLength;
		[Range(0.1f, 10.0f)]
		[SerializeField] float timeScale = 1.0f;

		[SerializeField] Agent agent;
		[SerializeField] Environment environment;

		private AIClient client;
		private float lastSendTime;
		private float episodeStartTime = 0f;
		private Vector3 FirstLocation;

		void Start () {
			Application.targetFrameRate = (int)Mathf.Max(60.0f, 60.0f * timeScale);

			client = new AIClient(url);
			FirstLocation = agent.transform.position;
			StartNewEpisode ();
			lastSendTime = -cycleTimeStepSize;
		}

		void OnCycleUpdateAfterReceiveAction() {
			agent.ResetState ();
		}

		public void TimeOver() {
			agent.AddReward (0);
			agent.EndEpisode ();
		}

		void StartNewEpisode() {
			episodeStartTime = Time.time;

			environment.OnReset ();
			agent.transform.position = FirstLocation;
			agent.StartEpisode ();
		}


		void Update() {
			Application.targetFrameRate = (int)Mathf.Max(60.0f, 60.0f * timeScale);

			byte[] msg = client.PopAIMessage();
			if(msg != null) {
				var packer = new MsgPack.BoxingPacker();
				agent.action.Set ((Dictionary<System.Object, System.Object>)packer.Unpack(msg));
				OnCycleUpdateAfterReceiveAction();
				Time.timeScale = timeScale;
			}

			if (lastSendTime + cycleTimeStepSize <= Time.time) {
				lastSendTime = Time.time;

				if (Time.time - episodeStartTime > episodeTimeLength) {
					TimeOver();
				}
				if (agent.state.endEpisode) {
					StartNewEpisode ();
				}
				agent.UpdateState ();
				client.PushAgentState(agent.state);
				Time.timeScale = 0.0f;
			}
		}
	}
}
