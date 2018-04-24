using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Controller holds the state of the game, which we need to stay the same across scene transitions.
/// </summary>

public class GameController : MonoBehaviour {

	private int livingChildren = 0;
	private int deadChildren = 0;
	private int gameScore = 0;
	public Text livingChildrenText;
	public Text deadChildrenText;
	public Text gameScoreText;


	// DontDestroy this so we it survives scene transitions
	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}


	void Start() {
		gameScore = 0;
		deadChildren = 0;
		SetGameScoreText ();
		SetDeadChildrenText ();
	}

	private void SetLivingChildrenText() {
		livingChildrenText.text = "Living Children : " + livingChildren;
	}

	private void SetDeadChildrenText() {
		deadChildrenText.text = "Dead Children : " + deadChildren;
	}

	private void SetGameScoreText() {
		gameScoreText.text = "Game Score : " + gameScore;
	}

	public int GetScore() {
		return gameScore;
	}

	public void EndGame(bool lose) {
		// You lose if you get touched by a zombie
		if (lose) {
			SceneManager.LoadScene ("LoseScreen");
		} else {
			SceneManager.LoadScene ("WinScreen");
		}
	}

	private void CheckIfGameOver() {
		// If there are no living children
		if (livingChildren == 0) {
			// Did you save at least one?
			// if so, you win.
			if (GetScore () == 0) {
				EndGame (true);
			} else {
				EndGame (false);
			}
		}
	}
		
	public int getLivingChildren(){
		return livingChildren;
	}
		

	public void IncreaseLivingChildrenCount() {
		livingChildren++;
		SetLivingChildrenText ();
	}

	public void DecreaseLivingChildrenCount() {
		livingChildren--;
		SetLivingChildrenText ();
	}

	public void IncreaseDeadChildrenCount() {
		deadChildren++;
		SetDeadChildrenText ();
		CheckIfGameOver ();
	}

	public void IncreaseGameScore() {
		gameScore++;
		SetGameScoreText ();
		CheckIfGameOver ();
	}
}
