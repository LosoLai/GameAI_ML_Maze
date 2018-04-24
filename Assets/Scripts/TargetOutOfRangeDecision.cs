using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple decision to check if the distance between the current unit and its target is greater than the chase range
///  that is stored in the state controller. Basically "are they far enough away to give up"
/// </summary>

[CreateAssetMenu (menuName = "AI/Decision/TargetOutOfRange")]
public class TargetOutOfRangeDecision : Decision {

	public override bool Decide(StateController controller) {
		return Vector3.Distance (controller.gameObject.transform.position, controller.target.transform.position) > controller.chaseRange;
	}
}
