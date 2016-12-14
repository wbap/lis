using UnityEngine;
using System.Collections;

namespace MLPlayer {
	public class RewardTriggerEvent : MonoBehaviour {
		[SerializeField] float rewardOnTriggerEnter;
		[SerializeField] float rewardOnTriggerExit;
		[SerializeField] float rewardOnTriggerStayPerFixedDeltaTime;

		bool IsPlayer(GameObject obj) {
			return obj.tag == Defs.PLAYER_TAG;
		}

		void OnTriggerEnter(Collider other) {
			if (IsPlayer(other.gameObject)) {
				other.gameObject.GetComponent<Agent> ().AddReward (rewardOnTriggerEnter);
				//Debug.Log ("OnTriggerEnter");
			}
		}

		void OnTriggerExit(Collider other) {
			if (IsPlayer(other.gameObject)) {
				other.gameObject.GetComponent<Agent> ().AddReward (rewardOnTriggerExit);
				//Debug.Log ("OnTriggerExit");
			}
		}

		void OnTriggerStay(Collider other) {
			if (IsPlayer(other.gameObject)) {
				other.gameObject.GetComponent<Agent> ().AddReward (rewardOnTriggerStayPerFixedDeltaTime);
				//Debug.Log ("OnTriggerStay:");
			}
		}
	}
}
