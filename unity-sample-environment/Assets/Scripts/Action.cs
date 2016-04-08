using UnityEngine;
using System.Collections;

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
		public void Set(string msg) {
			Clear ();
			switch (msg) {
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