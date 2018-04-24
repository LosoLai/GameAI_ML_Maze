using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player movement.
/// </summary>

public class PlayerMovement : MonoBehaviour {

	public Transform children;

	private CharacterController _characterController;
	private Animator animator;

	private float Gravity = 20.0f;

	private Vector3 _moveDirection = Vector3.zero;

	public float Speed = 5.0f;
	public float RotationSpeed = 120.0f;

	// Use this for initialization
	void Start() {
		_characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update()
	{
		// Get Input for axis
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		// Calculate the forward vector
		Vector3 camForward_Dir = Vector3.Scale (Camera.main.transform.forward, new Vector3 (1, 0, 1)).normalized;
		Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

		if (move.magnitude > 1f) {
			animator.SetBool ("run", true);
			move.Normalize ();
		} else {
			animator.SetBool ("run", false);
		}

		// Calculate the rotation for the player
		move = transform.InverseTransformDirection (move);

		// Get Euler angles
		float turnAmount = Mathf.Atan2 (move.x, move.z);

		transform.Rotate (0, turnAmount * RotationSpeed * Time.deltaTime, 0);

		if (_characterController.isGrounded) {
			_moveDirection = transform.forward * move.magnitude;
			_moveDirection *= Speed;
		}
		_moveDirection.y -= Gravity * Time.deltaTime;
		_characterController.Move (_moveDirection * Time.deltaTime);

		// Play Whistle
		if (Input.GetKeyDown (KeyCode.Space)) {
			Whistle ();
		}
	}

	/// WHISTLE
	void Whistle() {
		GameObject.Find ("Grid").GetComponent<GridController> ().ResetColors ();
		AudioSource audio = GetComponent<AudioSource> ();
		audio.Play ();
		for (int i = 0; i < children.childCount; i++) {
			children.GetChild (i).GetComponent<StateController> ().HearWhistle (this.gameObject);
		}
	}
}
