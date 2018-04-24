using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple controller for the Start Screen
/// </summary>

public class ScreenController : MonoBehaviour {

	// This is called on the Start Button press.
	public void StartGame() {
		SceneManager.LoadScene ("MainScene");
	}
}
