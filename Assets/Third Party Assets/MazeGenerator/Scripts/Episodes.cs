using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionChoices{
	MOVE_UP,
	MOVE_RIGHT,
	MOVE_DOWN,
	MOVE_LEFT
};

public class Episodes {
	public MazeCell start = null;
	public ActionChoices action;
	public MazeCell end = null;
	public float actionResult = 0.0f;
}
