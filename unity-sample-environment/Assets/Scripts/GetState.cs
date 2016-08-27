using UnityEngine;
using System.Collections.Generic;

namespace MLPlayer
{
	public class GetState : MonoBehaviour
	{
		[SerializeField] public Agent agent=new Agent();
		bool flag = false;

		//[SerializeField] Environment environment;
	
		// Use this for initialization
		void Start ()
		{
			StartNewEpisode ();
		}

		public void StartNewEpisode ()
		{
			agent.StartEpisode ();
		}


		// Update is called once per frame
		public void Update ()
		{
			agent.UpdateState ();
		}
	}
}
