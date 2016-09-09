using UnityEngine;
using System.Collections;

namespace MLPlayer {
	public class ResetEvent : MonoBehaviour {
		[SerializeField] float reward;

		void OnEvent(GameObject other) {
			if (other.tag == Defs.PLAYER_TAG) {
				other.GetComponent<Agent> ().AddReward (reward);
				gameObject.SetActive (false);
				Debug.Log ("ResetEvent reward:" + reward.ToString ());

				SceneController.Instance.TimeOver ();
			}
		}
		
		void OnTriggerEnter(Collider other) {
			OnEvent (other.gameObject);
		}
		
		void OnCollisionEnter(Collision collision) {
			OnEvent (collision.gameObject);
		}
	}
}
