using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeZoneController : MonoBehaviour {
	public GameController gameController;

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Player")) {
			gameController.EndGame (false);
		} else if (other.CompareTag ("Child")) {
			Destroy (other.gameObject);
			gameController.IncreaseGameScore ();
		}
	}
}
