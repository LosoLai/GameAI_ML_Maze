using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controller sets variables on every game object that are outside the scope of the State Controller
/// </summary>

public class ZombieController : MonoBehaviour {

	private Animator animator;

	void Awake () {
		animator = GetComponent<Animator> ();
		animator.SetBool ("run", true); // Always running
	}
	
}
