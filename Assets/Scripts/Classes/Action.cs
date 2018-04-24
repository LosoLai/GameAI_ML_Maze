using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actions are called on Update and have a State to which they belong
/// </summary>

public abstract class Action : ScriptableObject {
	public abstract void Act (StateController controller);
}
