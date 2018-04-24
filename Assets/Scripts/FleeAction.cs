using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Flee Action is used to run away from a target.
/// It calculates a vector in the opposite direction to the target, and then
///  move towards that opposite direction vector.
/// </summary>

[CreateAssetMenu (menuName = "AI/Action/Flee")]
public class FleeAction : Action {

	public override void Act(StateController controller) {
		Flee (controller);
	}

	private void Flee(StateController controller) {

		// Math from this website:
		// https://answers.unity.com/questions/132592/lookat-in-opposite-direction.html
		Vector3 target = new Vector3(
			2 * controller.transform.position.x - controller.target.transform.position.x,
			controller.transform.position.y,
			2 * controller.transform.position.z - controller.target.transform.position.z
		);
		/*
		controller.transform.LookAt (new Vector3(
			2 * controller.transform.position.x - controller.target.transform.position.x,
			controller.transform.position.y,
			2 * controller.transform.position.z - controller.target.transform.position.z
		));*/
		controller.gameObject.transform.LookAt (target);
		controller.gameObject.transform.position = Vector3.MoveTowards (
			controller.gameObject.transform.position, target, controller.speed * Time.deltaTime
		);
	}
}
