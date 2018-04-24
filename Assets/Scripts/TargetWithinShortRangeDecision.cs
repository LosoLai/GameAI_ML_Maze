using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple decision to see if the unit has gotten arbritarily close to its target.
/// Its used for changing state but without using a collider.
/// </summary>

[CreateAssetMenu (menuName = "AI/Decision/TargetWithinShortRangeDecision")]
public class TargetWithinShortRangeDecision : Decision {

	public override bool Decide (StateController controller) {
		return TargetWithinShortRange (controller);
	}

	private bool TargetWithinShortRange(StateController controller) {
		if (Vector3.Distance (controller.gameObject.transform.position, controller.target.transform.position) < 3f) {

			// This is a hack, but we can do this in the transition because transitions TO the whistle state will
			//  not work therefore because whistleTimer needs to be > 0;
			controller.whistleTimer = 0f;
			return true;
		}
		return false;
	}
}
