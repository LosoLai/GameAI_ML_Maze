using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// See Decision takes a tag, and raycasts out, testing for collisions with objects who are tagged as such.
/// This is used to trigger a transition to a new state when a unit "sees" another unit, such as when
/// zombies see a child, or a child sees a zombie.
/// </summary>

[CreateAssetMenu (menuName = "AI/Decision/See")]
public class SeeDecision : Decision {
	
	public string tag;

	public override bool Decide (StateController controller) {
		return See (controller);
	}

	private bool See(StateController controller) {

		RaycastHit hit;
		//Debug.DrawRay (controller.transform.position, controller.transform.forward.normalized * controller.lookRange, Color.gray);

		// Using the example from the unity official tutorials
		// https://youtu.be/RTlsZPvzYrk?list=PLX2vGYjWbI0ROSj_B0_eir_VkHrEkd4pi&t=180
		if (Physics.SphereCast (controller.transform.position, controller.lookRadius, controller.transform.forward, out hit, controller.lookRange)
			&& hit.collider.CompareTag (tag)) {
			controller.target = hit.collider.gameObject;
			return true;
		} else {
			return false;
		}
	}

}
