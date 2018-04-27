using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Action/SmartChildFollow")]
public class SmartChildFollowAction : Action {

	public override void Act(StateController controller) {
		Follow (controller);
	}

	private void Follow(StateController controller) {

		if(Vector3.Distance(controller.target.transform.position, controller.transform.position) < 1.0f)
			return;

		var direction = controller.target.transform.position - controller.transform.position;
		controller.transform.rotation = Quaternion.Slerp (
			controller.transform.rotation,
			Quaternion.LookRotation(direction),
			controller.rotationSpeed * Time.deltaTime);
		controller.transform.Translate (0, 0, Time.deltaTime * controller.speed);
	}
}
