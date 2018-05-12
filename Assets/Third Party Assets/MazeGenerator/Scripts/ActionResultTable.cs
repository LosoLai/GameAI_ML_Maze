using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionResultTable {
	//public Dictionary<MazeCell, float> dictionary = new Dictionary<MazeCell, float>();
	public Dictionary<MazeCell, ActionResult> Q_Table = new Dictionary<MazeCell, ActionResult>();
}
