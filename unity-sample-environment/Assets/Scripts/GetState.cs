using UnityEngine;
using System.Collections.Generic;

namespace MLPlayer
{
	public class GetState : MonoBehaviour
	{
		[SerializeField] public Agent agent;
		private AIServer server;

		//[SerializeField] Environment environment;
	
		// Use this for initialization
		void Start ()
		{
			server = new AIServer(agent);
			StartNewEpisode ();
		}

		public void StartNewEpisode ()
		{
			agent.StartEpisode ();
		}

		// Update is called once per frame
		public void Update ()
		{
			if(agent.state.endEpisode) {
				StartNewEpisode ();
			}
			agent.UpdateState ();
			server.PushAgentState (agent.state);

			agent.ResetState ();
		}
	}
}
