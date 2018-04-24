using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// States have Actions and Transitions, and are managed by the StateController
/// </summary>

[CreateAssetMenu (menuName = "AI/State")]
public class State : ScriptableObject {

	public Action[] actions;
	public Transition[] transitions;
	public Color sceneGizmoColor = Color.red;
	public Vector3 velocity;

	public void UpdateState(StateController controller) {
		DoActions (controller);
		CheckTransitions (controller);
	}

	private void DoActions(StateController controller) {
		for (int i = 0; i < actions.Length; i++) {
			actions [i].Act (controller);
		}
	}

	private void CheckTransitions(StateController controller) {
		for (int i = 0; i < transitions.Length; i++) {
			if (transitions [i].decision.Decide (controller)) {
				controller.TransitionToState (transitions [i].trueState);
			} else {
				controller.TransitionToState (transitions [i].falseState);
			}
		}
	}

	public void SetVelocity(Vector3 velocity) {
		this.velocity = velocity;
	}
}
