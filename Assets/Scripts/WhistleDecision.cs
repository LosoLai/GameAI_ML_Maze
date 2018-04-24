using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Whistle Decision checks if the unit has heard the whistle.
/// When the player pushes the button, all of the state controllers get their boolean value update.
/// This script calls a function which checks that boolean value.
/// </summary>

[CreateAssetMenu (menuName = "AI/Decision/Whistle")]
public class WhistleDecision : Decision {

	public override bool Decide (StateController controller) {
		return controller.GetWhistleStatus ();
	}
}
