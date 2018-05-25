using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour {
	public enum MazeGenerationAlgorithm{
		PureRecursive,
		RecursiveTree,
		RandomTree,
		OldestTree,
		RecursiveDivision,
	}

	public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
	public bool FullRandom = false;
	public int RandomSeed = 12345;
	public GameObject Floor = null;
	public GameObject SafeZone = null;
	public GameObject Wall = null;
	public GameObject Pillar = null;
	public int Rows = 5;
	public int Columns = 5;
	public int Child_Start_Row = 0;
	public int Child_Start_Col = 0;
	public int Zombie_Start_Row = 9;
	public int Zombie_Start_Col = 0;
	public int SafeZone_Row = 9;
	public int SafeZone_Col = 9;
	public float CellWidth = 5;
	public float CellHeight = 5;
	public bool AddGaps = true;
	public GameObject ChildPrefab = null;
	public GameObject ZombiePrefab = null;
	[HideInInspector] GameObject agentA = null;
	[HideInInspector] GameObject agentB = null;

	private BasicMazeGenerator mMazeGenerator = null;

	//the variables are for reinforcement learning
	private readonly float freezeTime = 1.0f;
	private float freezeTimer = 0.0f;
	private readonly float alphaDecade = 0.5f;
	private int current_RowIndex = 0;
	private int current_ColIndex = 0;
	private bool isActionPerforming = false;
	private bool isEnterSafeZone = false;
	private MazeCell startStatus = null;
	private MazeCell endStatus = null;
	private ActionChoices actionChoice;
	private List<ActionChoices> actionChoiceArray = new List<ActionChoices>();
	private ActionResultTable resultTable = null;

	//records for evaluation 
	private readonly int FIXED_EPISODE_NUMBER = 10000;
	private int episode_count = 0;
	private double iteration_steps = 0;
	//private List<int> experienceList = new List<int>();

	void Start () {
		createMaze ();
		initialResultTable ();
	}

	void FixedUpdate () {
		if (!isEnterSafeZone) {
			//do iteration training
			if (!isActionPerforming)
				doAction ();
			else
				actionPerforming ();
		} else {
			//enter safezone
			freezeTimer += Time.deltaTime;
			Debug.Log("Timer " + freezeTimer);
			if (freezeTimer >= freezeTime) {
				if (episode_count < FIXED_EPISODE_NUMBER) {
					episode_count++;
					saveIterationPerformance ();
					iteration_steps = 0;
					Debug.Log("Steps Count: " + iteration_steps);
				}
				else
					return;
				
				//reset
				current_RowIndex = Child_Start_Row;
				current_ColIndex = Child_Start_Col;
				agentA.transform.position = mMazeGenerator.GetMazeCell (current_RowIndex, current_ColIndex).floor.transform.position;
				isEnterSafeZone = false;
				freezeTimer = 0.0f;
			}
		}
	}

	void saveIterationPerformance() {
		//update file
		string content = episode_count + "\t\t" + iteration_steps + Environment.NewLine;
		File.AppendAllText(@"C:\Users\Loso\Documents\GitHub\GameAI_ML_Maze\Performance.txt", content);
	}

	void createMaze() {
		if (!FullRandom) {
			//Random.seed = RandomSeed;
			UnityEngine.Random.InitState (RandomSeed);
		}
		switch (Algorithm) {
		case MazeGenerationAlgorithm.PureRecursive:
			mMazeGenerator = new RecursiveMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.RecursiveTree:
			mMazeGenerator = new RecursiveTreeMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.RandomTree:
			mMazeGenerator = new RandomTreeMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.OldestTree:
			mMazeGenerator = new OldestTreeMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.RecursiveDivision:
			mMazeGenerator = new DivisionMazeGenerator (Rows, Columns);
			break;
		}
		mMazeGenerator.GenerateMaze ();
		for (int row = 0; row < Rows; row++) {
			for(int column = 0; column < Columns; column++){
				float x = column*(CellWidth+(AddGaps?.2f:0));
				float z = row*(CellHeight+(AddGaps?.2f:0));
				MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
				GameObject tmp;
				if(row == SafeZone_Row && column == SafeZone_Col){
					cell.floor = Instantiate(SafeZone,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
					cell.IsGoal = true;
				}
				else
					cell.floor = Instantiate(Floor,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
				cell.floor.transform.parent = transform;
				if(cell.WallRight){
					tmp = Instantiate(Wall,new Vector3(x+CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
					tmp.transform.parent = transform;
				}
				if(cell.WallFront){
					tmp = Instantiate(Wall,new Vector3(x,0,z+CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
					tmp.transform.parent = transform;
				}
				if(cell.WallLeft){
					tmp = Instantiate(Wall,new Vector3(x-CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
					tmp.transform.parent = transform;
				}
				if(cell.WallBack){
					tmp = Instantiate(Wall,new Vector3(x,0,z-CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
					tmp.transform.parent = transform;
				}
				/*if(cell.IsGoal && GoalPrefab != null){
					tmp = Instantiate(GoalPrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
					tmp.transform.parent = transform;
				}*/

				if(row == Child_Start_Row && column == Child_Start_Col)
					agentA = Instantiate(ChildPrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
				//if(row == Zombie_Start_Row && column == Zombie_Start_Col)
				//	agentB = Instantiate(ZombiePrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
			}
		}

		if(Pillar != null){
			for (int row = 0; row < Rows+1; row++) {
				for (int column = 0; column < Columns+1; column++) {
					float x = column*(CellWidth+(AddGaps?.2f:0));
					float z = row*(CellHeight+(AddGaps?.2f:0));
					GameObject tmp = Instantiate(Pillar,new Vector3(x-CellWidth/2,0,z-CellHeight/2),Quaternion.identity) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}
	}

	void initialResultTable() {
		current_RowIndex = Child_Start_Row;
		current_ColIndex = Child_Start_Col;

		//writing file title
		using (var sw = new StreamWriter("QValueResult.txt"))
		{
			var no = "Line No.1";
			var position = "Status\t";
			var up = "Ac_U";
			var right = "Ac_R";
			var down = "Ac_D";
			var left = "Ac_L";
			var line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", no, position, up, right, down, left);
			sw.WriteLine(line);

			int mazeRow = Rows;
			int mazeCol = Columns;
			resultTable = new ActionResultTable ();

			for (int i=0 ; i<mazeRow ; i++) {
				for (int j = 0; j < mazeCol; j++) {
					//Q_Table
					ActionResult Q_Value = new ActionResult();
					resultTable.Q_Table.Add(mMazeGenerator.GetMazeCell(i, j), Q_Value);
					resultTable.writeInitialValue (sw, i, j, Q_Value, Columns);
				}
			}

			sw.Flush();
			sw.Close();
		}

		using (var sw = new StreamWriter("Performance.txt"))
		{
			var interationNo = "Iter.Index";
			var stepsNo = "Steps Count";
			var line = string.Format("{0}\t{1}\n", interationNo, stepsNo);
			sw.WriteLine(line);

			sw.Flush();
			sw.Close();
		}

		actionChoiceArray.Add (ActionChoices.MOVE_UP);
		actionChoiceArray.Add (ActionChoices.MOVE_RIGHT);
		actionChoiceArray.Add (ActionChoices.MOVE_DOWN);
		actionChoiceArray.Add (ActionChoices.MOVE_LEFT);
	}

	List<ActionChoices> getOptimalPolicy(MazeCell status) {
		List<ActionChoices> optimalPolicySet = null;
		ActionResult status_QValue;
		float optimalValue = -10.0f;
		if (resultTable.Q_Table.TryGetValue (status, out status_QValue)) {
			if (status_QValue.Q_Value.Count > 0) {
				optimalPolicySet = new List<ActionChoices> ();
				ActionChoices optimalIndex = ActionChoices.MOVE_UP;
				foreach (var result in status_QValue.Q_Value) {
					if (result.Value >= optimalValue) {
						optimalValue = result.Value;
						optimalIndex = result.Key;
						optimalPolicySet.Add (optimalIndex);
					}
				}
				//optimalPolicySet.Add (optimalIndex);
				return optimalPolicySet;
			} else {
				return optimalPolicySet;
			}
		} 

		return optimalPolicySet;
	}

	void doAction() {
		//count steps
		iteration_steps++;

		startStatus = mMazeGenerator.GetMazeCell (current_RowIndex, current_ColIndex);

		ActionChoices action;
		List<ActionChoices> actionSet = getOptimalPolicy(startStatus);
		if (actionSet == null) {
			Debug.Log("Random Run");
			int actionIndex = UnityEngine.Random.Range (1, 1000) % 4;
			action = actionChoiceArray [actionIndex];
		} else if(actionSet.Count == 1){
			Debug.Log("Choose Optimal Policy, count :" + actionSet.Count);
			action = (ActionChoices)actionSet [0];
		} 
		else {
			Debug.Log("Choose Optimal Policy, count :" + actionSet.Count);
			//need to check is any optimal policay here
			int randomIndex = UnityEngine.Random.Range (1, 1000) % actionSet.Count;
			Debug.Log("randomIndex :" + randomIndex);
			action = (ActionChoices)actionSet [randomIndex];
		}

		if (action == ActionChoices.MOVE_UP) {
			current_RowIndex++;
			if(current_RowIndex >= Rows)
				current_RowIndex = Rows - 1;
			actionChoice = ActionChoices.MOVE_UP;
			Debug.Log("move up " + current_RowIndex + current_ColIndex);
		} else if (action == ActionChoices.MOVE_RIGHT) {
			current_ColIndex++;
			if (current_ColIndex >= Columns)
				current_ColIndex = Columns - 1;
			actionChoice = ActionChoices.MOVE_RIGHT;
			Debug.Log("move right " + current_RowIndex + current_ColIndex);
		} else if (action == ActionChoices.MOVE_DOWN) {
			current_RowIndex--;
			if (current_RowIndex <= 0)
				current_RowIndex = 0;
			actionChoice = ActionChoices.MOVE_DOWN;
			Debug.Log("move back " + current_RowIndex + current_ColIndex);
		} else {
			current_ColIndex--;
			if (current_ColIndex <= 0)
				current_ColIndex = 0;
			actionChoice = ActionChoices.MOVE_LEFT;
			Debug.Log("move left " + current_RowIndex + current_ColIndex);
		}

		endStatus = mMazeGenerator.GetMazeCell (current_RowIndex, current_ColIndex);
		isActionPerforming = true;
	}

	void actionPerforming() {
		if (agentA != null) {
			float actionResult = actionResultCalculate ();

			if (actionResult >= 0) {
				Vector3 targetPos = endStatus.floor.transform.position;
				agentA.transform.position = targetPos;
			} else {
				Vector3 targetPos = startStatus.floor.transform.position;
				agentA.transform.position = targetPos;
				current_RowIndex = startStatus.row;
				current_ColIndex = startStatus.col;
			}

			//save and update
			saveResultAndUpdateTable(actionResult);

			Debug.Log("current pos :" + current_RowIndex + current_ColIndex);
			isActionPerforming = false;
		}
	}

	float actionResultCalculate() {
		float result = 0.0f;
		if (endStatus.IsGoal)
			return 10.0f;
		
		bool hasWall = false;
		switch (actionChoice) {
			case ActionChoices.MOVE_UP:
			{
				hasWall = startStatus.WallFront;
				break;
			}
			case ActionChoices.MOVE_RIGHT:
			{
				hasWall = startStatus.WallRight;
				break;
			}
			case ActionChoices.MOVE_DOWN:
			{
				hasWall = startStatus.WallBack;
				break;
			}
			case ActionChoices.MOVE_LEFT:
			{
				hasWall = startStatus.WallLeft;
				break;
			}
		}

		if (hasWall)
			result = -1.0f;
		else
			result = 1.0f;

		return result;
	}

	string updatedStatusString(int no, int i, int j, ActionResult result){
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
			
		Debug.Log("S_QV String:" + vText);
		return vText;
	}

	void saveResultAndUpdateTable(float actionResult){
		bool update_StartStatus = false;
		bool update_EndStatus = false;

		//update table
		if(resultTable != null){
			ActionResult startStatus_QValue;
			float startStatus_Value = 0.0f; 
			if (resultTable.Q_Table.TryGetValue (startStatus, out startStatus_QValue)) {
				if (startStatus_QValue.Q_Value.TryGetValue (actionChoice, out startStatus_Value)) {
					update_StartStatus = true;
					Debug.Log("table value: start" + startStatus.row + startStatus.col + "value : " + startStatus_Value);
				}
			}

			ActionResult endStatus_QValue;
			float endStatus_Value = 0.0f;
			if(resultTable.Q_Table.TryGetValue (endStatus, out endStatus_QValue)){
				if (endStatus_QValue.Q_Value.TryGetValue (actionChoice, out endStatus_Value)) {
					if (endStatus.IsGoal) {
						endStatus_Value = actionResult;

						endStatus_QValue.Q_Value[ActionChoices.MOVE_UP] = endStatus_Value;
						endStatus_QValue.Q_Value[ActionChoices.MOVE_RIGHT] = endStatus_Value;
						endStatus_QValue.Q_Value[ActionChoices.MOVE_DOWN] = endStatus_Value;
						endStatus_QValue.Q_Value[ActionChoices.MOVE_LEFT] = endStatus_Value;
						Debug.Log("Update endStatus");
						update_EndStatus = true;
						isEnterSafeZone = true;
					}
					if(isEnterSafeZone)
						Debug.Log("Enter SafeZone");
				} 

				Debug.Log ("table value: end" + endStatus.row + endStatus.col + "value : " + endStatus_Value);
				startStatus_Value = endStatus_Value + (actionResult * alphaDecade);
				Debug.Log ("updated start value :" + startStatus_Value);
				startStatus_QValue.Q_Value [actionChoice] = startStatus_Value;
			} 
			//else {
			//	startStatus_Value = actionResult * alphaDecade;
			//	startStatus_QValue.Q_Value [actionChoice] = startStatus_Value;
			//}

			//update file
			string path = @"C:\Users\Loso\Documents\GitHub\GameAI_ML_Maze\QValueResult.txt";
			string[] lines = System.IO.File.ReadAllLines(path);
			if (update_StartStatus) {
				int no = (startStatus.row * Columns) + startStatus.col + 2;
				lines[no] = updatedStatusString (no, startStatus.row, startStatus.col, startStatus_QValue);
			}
			if (update_EndStatus) {
				int no = (endStatus.row * Columns) + endStatus.col + 2;
				lines[no] = updatedStatusString (no, endStatus.row, endStatus.col, endStatus_QValue);
			}

			//write file
			if (System.IO.File.Exists(path))
				System.IO.File.WriteAllLines(path, lines);
			
		}
	} 
}
