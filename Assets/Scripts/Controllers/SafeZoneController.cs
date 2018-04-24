using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeZoneController : MonoBehaviour {
	public GameController gameController;

	/*
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") { //tag name Object to Pick Up
			Debug.Log ("enter the tent");
			if (gameController != null) {
				Debug.Log ("Living child: " + gameController.getLivingChildren());
				if (gameController.getLivingChildren () > 0) {
					//gameController.SetGameStatus ("Player Wins : enter the safezone and still have living child");
					SceneManager.LoadScene ("WinScreen");
				} else {
					//gameController.SetGameStatus ("Game Over : enter the safezone and have no living child");
					SceneManager.LoadScene ("LoseScreen");
				}
			}
		}
	}*/

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Player")) {
			gameController.EndGame (false);
		} else if (other.CompareTag ("Child")) {
			Destroy (other.gameObject);
			gameController.IncreaseGameScore ();
		}
	}
}
