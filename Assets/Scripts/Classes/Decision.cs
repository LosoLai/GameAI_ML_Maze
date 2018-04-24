using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decisions test a condition and return a boolean if it is fulfilled.
/// </summary>

public abstract class Decision : ScriptableObject {
	public abstract bool Decide(StateController controller);
}
