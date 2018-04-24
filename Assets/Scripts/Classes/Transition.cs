using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transitions is the pairing of a Decision with relevant States for true and false
/// </summary>

[System.Serializable]
public class Transition {
	public Decision decision;
	public State trueState;
	public State falseState;
}
