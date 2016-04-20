using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using MsgPack;

namespace MLPlayer {
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

		enum CommunicationMode {ASYNC, SYNC}
		[SerializeField] CommunicationMode communicationMode;

		[SerializeField] string domain;
		[SerializeField] string path;
		[SerializeField] int port;
		[SerializeField] float cycleTimeStepSize;
		[SerializeField] float episodeTimeLength;
		[Range(0.1f, 10.0f)]
		[SerializeField] float timeScale = 1.0f;

		[SerializeField] List<Agent> agents;
		private List<AIClient> clients;
		private List<Vector3> firstLocation;
		[SerializeField] Environment environment;
		private float lastSendTime;
		private float episodeStartTime = 0f;
		private int actionReceiveCounter;

		public static ManualResetEvent received = new ManualResetEvent(false);

		void Start () {
			AIClient.OnMessageFunc onMsg;
			if (communicationMode == CommunicationMode.SYNC) {
				onMsg = OnMessage;
			} else {
				Application.targetFrameRate = (int)Mathf.Max(60.0f, 60.0f * timeScale);
				onMsg = null;
			}

			clients = new List<AIClient> ();
			firstLocation = new List<Vector3> ();
			int port = this.port;
			foreach (var agent in agents) {
				clients.Add (new AIClient (
					"ws://" + domain + ":" + port.ToString() + "/" + path,
					onMsg));
				firstLocation.Add (agent.transform.position);
				port++;
			}

			StartNewEpisode ();
			lastSendTime = -cycleTimeStepSize;
		}

		void OnMessage() {
			actionReceiveCounter++;
			if (actionReceiveCounter == agents.Count) {
				received.Set();
			}
		}

		void OnCycleUpdateAfterReceiveAction() {
			foreach (var agent in agents) {
				agent.ResetState ();
			}
		}

		public void TimeOver() {
			foreach (var agent in agents) {
				agent.EndEpisode ();
			}
		}

		void StartNewEpisode() {
			episodeStartTime = Time.time;

			environment.OnReset ();
			for (int i=0; i<agents.Count; i++) {
				agents[i].transform.position = firstLocation[i];
				agents[i].StartEpisode ();
			}
		}


		void FixedUpdate() {
			if (communicationMode != CommunicationMode.SYNC) {
				return;
			}

			for (int i=0; i<agents.Count; i++) {
				byte[] msg = clients[i].PopAIMessage ();
				if (msg != null) {
					var packer = new MsgPack.BoxingPacker ();
					agents[i].action.Set ((Dictionary<System.Object, System.Object>)packer.Unpack (msg));
					OnCycleUpdateAfterReceiveAction ();
				}
			}

			if (lastSendTime + cycleTimeStepSize <= Time.time) {
				lastSendTime = Time.time;

				if (Time.time - episodeStartTime > episodeTimeLength) {
					TimeOver();
				}

				// TODO all agents have same value
				if (agents[0].state.endEpisode) {
					StartNewEpisode ();
				}

				actionReceiveCounter = 0;
				for (int i=0; i<agents.Count; i++) {
					agents[i].UpdateState ();
					clients[i].PushAgentState (agents[i].state);
				}
				received.Reset();
				received.WaitOne ();
			}
		}
	

		void Update() {
			if (communicationMode != CommunicationMode.ASYNC) {
				return;
			}
			Application.targetFrameRate = (int)Mathf.Max(60.0f, 60.0f * timeScale);

			for (int i=0; i<agents.Count; i++) {
				byte[] msg = clients[i].PopAIMessage ();
				if (msg != null) {
					var packer = new MsgPack.BoxingPacker ();
					agents[i].action.Set ((Dictionary<System.Object, System.Object>)packer.Unpack (msg));
					OnCycleUpdateAfterReceiveAction ();
					Time.timeScale = timeScale;
				}
			}

			if (lastSendTime + cycleTimeStepSize <= Time.time) {
				lastSendTime = Time.time;

				if (Time.time - episodeStartTime > episodeTimeLength) {
					TimeOver();
				}

				// TODO all agents have same value
				if (agents[0].state.endEpisode) {
					StartNewEpisode ();
				}

				for (int i=0; i<agents.Count; i++) {
					agents[i].UpdateState ();
					clients[i].PushAgentState (agents[i].state);
				};
				Time.timeScale = 0.0f;
			}
		}

	}
}
