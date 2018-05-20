using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ActionResultTable {
	//public Dictionary<MazeCell, float> dictionary = new Dictionary<MazeCell, float>();
	public Dictionary<MazeCell, ActionResult> Q_Table = new Dictionary<MazeCell, ActionResult>();

	private int colNum;

	public void writeInitialValue(StreamWriter sw, int i, int j, ActionResult result, int col) {
		colNum = col;

		int no = (i * col) + j + 2;
		string line = "Line No." + no + "\t";
		string pos = "POS(" + i + "," + j + ")";
		string vText = line + pos;

		float up = 0.0f;
		float right = 0.0f;
		float down = 0.0f;
		float left = 0.0f;

		if (result.Q_Value.TryGetValue (ActionChoices.MOVE_UP, out up)) {
			vText += "\tU:" + up + "\t";
		}
		if (result.Q_Value.TryGetValue (ActionChoices.MOVE_RIGHT, out right)) {
			vText += "R:" + right + "\t";
		}
		if (result.Q_Value.TryGetValue (ActionChoices.MOVE_DOWN, out down)) {
			vText += "D:" + down + "\t";
		}
		if (result.Q_Value.TryGetValue (ActionChoices.MOVE_LEFT, out left)) {
			vText += "L:" + left;
		}
		sw.WriteLine(vText);
	}
}
