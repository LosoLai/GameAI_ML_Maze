using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Controller is used for any unit which needs to maintain its state.
/// </summary>

public class StateController : MonoBehaviour {

	// Look Range and Radius for See Decisions
	public float lookRange = 10f;
	public float lookRadius = 1f;

	// Chase Range is for Target Out Of Range decision
	public float chaseRange = 15f;

	// Unit speed
	public float speed = 5f;
	public float rotationSpeed = 5f;

	// Variables for state management (remain is the ignore state)
	public State currentState;
	public State remainState;

	// Parent and Child used for Smart and Dumb relationship
	[HideInInspector] public StateController parent;
	[HideInInspector] public List<StateController> children;

	// Recalculation timer for actions that use A*
	[HideInInspector] public float recalculationTimer = 1f;

	// Target variables for actions with a target unit or location (and path to it)
	[HideInInspector] public GameObject target;
	[HideInInspector] public Vector3 targetNode;
	[HideInInspector] public List<Vector3> path;

	// Rigidbody is used for velocity
	[HideInInspector] public Rigidbody rigidbody;

	// WanderForce for SmartZombie wander action
	[HideInInspector] public Vector3 wanderForce;

	[HideInInspector] public float whistleTimer;


	void Awake() {
		rigidbody = GetComponent<Rigidbody> ();
		wanderForce = transform.position;
		path = new List<Vector3> ();
	}


	void Update() {
		recalculationTimer -= Time.deltaTime;
		currentState.UpdateState (this);
	}

	// Draw the sphere and ray for each unit, to debug states.
	void OnDrawGizmos() {
		Gizmos.color = currentState.sceneGizmoColor;
		Gizmos.DrawWireSphere (transform.position, 1f);
		Gizmos.DrawRay (transform.position, transform.forward * lookRange);
	}

	// When a decision returns, it has true and false states
	public void TransitionToState(State nextState) {
		// If the state is the remain state, ignore it.
		// Otherwise we need to transition.
		if (nextState != remainState) {
			currentState = nextState;
			recalculationTimer = 0f;
		}
	}
		
	// Used by Decisions and States to not call functions on every frame.
	// Has a built in reset because if it is true, you might as well reset it here rather than
	//  in every single function that is calling it.
	public bool RecalculationTimerElapsed() {
		if (recalculationTimer <= 0f) {
			recalculationTimer = 10f;
			return true;
		}
		return false;
	}
		
	// When the player pushes the whistle, this function is called on all of the Smart Children.
	public void HearWhistle(GameObject player) {
		whistleTimer = 10f;
		this.target = player;
		path = GameObject.Find ("Grid").GetComponent<GridController> ().AStar (this.gameObject, player);
	}
		
	// Used in Whistle Action to check if the unit can give up.
	public bool GetWhistleStatus() {
		if (whistleTimer > 0.0f) {
			whistleTimer -= Time.deltaTime;
			return true;
		}
		return false;
	}
}
