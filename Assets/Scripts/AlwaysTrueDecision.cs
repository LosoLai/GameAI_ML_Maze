using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This decision always returns true. It is used to trigger a transition unconditionally
/// </summary>

[CreateAssetMenu (menuName = "AI/Decision/AlwaysTrue")]
public class AlwaysTrueDecision : Decision {
	
	public override bool Decide (StateController controller) {
		return true;
	}
}
