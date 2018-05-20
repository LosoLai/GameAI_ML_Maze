using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionResult {
	public Dictionary<ActionChoices, float> Q_Value = new Dictionary<ActionChoices, float>();
	public ActionResult (){
		Q_Value.Add (ActionChoices.MOVE_UP, 0.0f);
		Q_Value.Add (ActionChoices.MOVE_RIGHT, 0.0f);
		Q_Value.Add (ActionChoices.MOVE_DOWN, 0.0f);
		Q_Value.Add (ActionChoices.MOVE_LEFT, 0.0f);
	}
}
