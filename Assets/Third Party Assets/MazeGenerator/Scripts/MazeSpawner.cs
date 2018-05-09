using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private readonly float alphaDecade = 0.5f;
	private int current_RowIndex = 0;
	private int current_ColIndex = 0;
	private bool isActionPerforming = false;
	private bool isEnterSafeZone = false;
	private MazeCell startStatus = null;
	private MazeCell endStatus = null;
	private ActionChoices actionChoice;
	private ActionResultTable resultTable = null;
	private List<Episodes> experienceList = new List<Episodes>();

	void Start () {
		createMaze ();
		initialResultTable ();
	}

	void FixedUpdate () {
		if (!isEnterSafeZone) {
			//do iteration training
			if (!isActionPerforming)
				doAction ();
			else {
				actionPerforming ();
			}
		} else {
			//evaluate experience
			//reset
			current_RowIndex = Child_Start_Row;
			current_ColIndex = Child_Start_Col;
			agentA.transform.position = mMazeGenerator.GetMazeCell (current_RowIndex, current_ColIndex).floor.transform.position;
		}
	}

	void createMaze() {
		if (!FullRandom) {
			//Random.seed = RandomSeed;
			Random.InitState (RandomSeed);
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
				if(row == SafeZone_Row && column == SafeZone_Col)
					cell.floor = Instantiate(SafeZone,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
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

		int mazeRow = Rows;
		int mazeCol = Columns;
		resultTable = new ActionResultTable ();
		float initialValue = 0.0f;

		for (int i=0 ; i<mazeRow ; i++) {
			for (int j = 0; j < mazeCol; j++) {
				if(i == SafeZone_Row && j == SafeZone_Col)
					resultTable.dictionary.Add (mMazeGenerator.GetMazeCell(i, j), 10.0f);
				else
					resultTable.dictionary.Add (mMazeGenerator.GetMazeCell(i, j), initialValue);
			}
		}
	}

	void doAction() {
		startStatus = mMazeGenerator.GetMazeCell (current_RowIndex, current_ColIndex);

		int action = Random.Range (1, 1000) % 4;
		if (action == 0) {
			current_RowIndex++;
			if(current_RowIndex >= Rows)
				current_RowIndex = Rows - 1;
			actionChoice = ActionChoices.MOVE_UP;
			Debug.Log("move up " + current_RowIndex + current_ColIndex);
		} else if (action == 1) {
			current_ColIndex++;
			if (current_ColIndex >= Columns)
				current_ColIndex = Columns - 1;
			actionChoice = ActionChoices.MOVE_RIGHT;
			Debug.Log("move right " + current_RowIndex + current_ColIndex);
		} else if (action == 2) {
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
			Debug.Log("current pos :" + current_RowIndex + current_ColIndex);

			//save and update
			saveResultAndUpdateTable(actionResult);
			isActionPerforming = false;
		}
	}

	float actionResultCalculate() {
		float result = 0.0f;
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

		return result;
	}

	void saveResultAndUpdateTable(float actionResult){
		//save the record
		//Episodes record = new Episodes ();
		//record.start = startStatus;
		//record.end = endStatus;
		//record.action = actionChoice;
		//record.actionResult = actionResult;
		//experienceList.Add (record);

		//update table
		if(resultTable != null){
			float startStatus_Value; 
			if (resultTable.dictionary.TryGetValue (startStatus, out startStatus_Value)) {
				Debug.Log("table value: start" + startStatus.row + startStatus.col + "value : " + startStatus_Value);
				resultTable.dictionary.Remove(startStatus);
			}
				
			float endStatus_Value;
			if(resultTable.dictionary.TryGetValue (endStatus, out endStatus_Value)){
				Debug.Log("table value: end" + endStatus.row + endStatus.col + "value : " + endStatus_Value);
				startStatus_Value = (endStatus_Value + actionResult) * alphaDecade;
				Debug.Log("updated start value :" + startStatus_Value);
				resultTable.dictionary.Add (startStatus, startStatus_Value);
				if (endStatus_Value >= 10.0f)
					isEnterSafeZone = true;
				if(isEnterSafeZone)
					Debug.Log("Enter SafeZone");
			}
		}
	} 
}
