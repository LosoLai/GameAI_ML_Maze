using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple decision which tests if the target stored in the StateController still exists.
/// </summary>

[CreateAssetMenu (menuName = "AI/Decision/TargetNoLongerExists")]
public class TargetNoLongerExistsDecision : Decision {

	public override bool Decide (StateController controller) {
		// If the target has Destroy() called on it, then the reference
		//  should return null, and thus this works.
		return controller.target == null;
	}
}
