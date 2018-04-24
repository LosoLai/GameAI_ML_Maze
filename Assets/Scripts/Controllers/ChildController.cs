using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildController : MonoBehaviour {

	private GameController gameController;
	private Animator animator;

	// Use this for initialization
	void Awake () {
		gameController = GameObject.Find ("GameController").GetComponent<GameController> ();
		if(gameController != null)
			gameController.IncreaseLivingChildrenCount ();
		animator = GetComponent<Animator> ();
		animator.SetBool ("run", true); // Always running
	}
	
	void OnCollisionEnter(Collision col) {
		if (col.gameObject.CompareTag ("Zombie")) {
			gameController.DecreaseLivingChildrenCount ();
			gameController.IncreaseDeadChildrenCount ();
			Destroy (this.gameObject);
		}
	}
}
