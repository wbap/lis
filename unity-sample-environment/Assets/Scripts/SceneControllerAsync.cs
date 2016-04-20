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

		[SerializeField] string domain;
		[SerializeField] string path;
		[SerializeField] int port;
		[SerializeField] float cycleTimeStepSize;
		[SerializeField] float episodeTimeLength;
		[Range(0.1f, 10.0f)]
		[SerializeField] float timeScale = 1.0f;

		[SerializeField] List<Agent> agents;
		[SerializeField] Environment environment;

		private List<AIClient> clients;
		private float lastSendTime;
		private float episodeStartTime = 0f;
		private List<Vector3> firstLocation;

		void Start () {
			Application.targetFrameRate = (int)Mathf.Max(60.0f, 60.0f * timeScale);

			clients = new List<AIClient> ();
			firstLocation = new List<Vector3> ();
			int port = this.port;
			foreach (var agent in agents) {
				clients.Add (new AIClient ("ws://" + domain + ":" + port.ToString() + "/" + path));
				firstLocation.Add (agent.transform.position);
				port++;
			}

			StartNewEpisode ();
			lastSendTime = -cycleTimeStepSize;
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


		void Update() {
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
				}
				Time.timeScale = 0.0f;
			}
		}

	}
}
