using UnityEngine;
using System.Collections.Generic;

namespace MLPlayer {
	public class Action {
		public float rotate;
		public float forward;
		public bool jump;
		public void Clear() {
			rotate = 0;
			forward = 0;
			jump = false;
		}

		public void Set(Dictionary<System.Object, System.Object> action) {


			// make hash table (because the 2 data arrays with equal content do not provide the same hash)
			var originalKey = new Dictionary<string, byte[]>();
			foreach (byte[] key in action.Keys) {
				originalKey.Add (System.Text.Encoding.UTF8.GetString(key), key);
				//Debug.Log ("key:" + System.Text.Encoding.UTF8.GetString(key) + " value:" + action[key]);
			}

			// string:
			string command = System.Text.Encoding.UTF8.GetString((byte[])action [originalKey ["command"]]);
			// int:
			//int i = (int)action [originalKey ["command"]];
			// float:
			//float f = float.Parse (System.Text.Encoding.UTF8.GetString((byte[])action [originalKey ["value"]]));

			Clear ();
			switch (command) {
			case "0":
				rotate = 1;
				break;
			case "1":
				rotate = -1;
				break;
			case "2":
				forward = 1;
				break;
			case "3":
				jump = true;
				break;
			}
		}


	}
}