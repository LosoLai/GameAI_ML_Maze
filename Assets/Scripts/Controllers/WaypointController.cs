using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The WaypointController contains the functions that each Waypoint needs for the GridController to interact with it.
/// </summary>

public class WaypointController : MonoBehaviour {

	private List<GameObject> neighbours = new List<GameObject>();
	private int collisions = 0;
	private int _id;

	// On Obstacle colliding with it
	// then increment collisions count
	void OnTriggerEnter(Collider col) {
		if (col.CompareTag("Obstacle")) {
			// GetComponent<MeshRenderer> ().enabled = false;
			collisions++;
		} 
	}

	// On Obstacle leaving collision,
	// then decrememnt collisions count
	void OnTriggerExit(Collider col) {
		if (col.CompareTag("Obstacle")) {
			collisions--;

			/*
			if (collisions == 0) {
				GetComponent<MeshRenderer> ().enabled = true;
			} */
		}
	}

	public bool IsValidNeighbour(float height) {
		// If you have any collisions, then always no
		if (collisions > 0) {
			return false;
		}

		// If the distance between the heights is greater than a threshold, no.
		if (Mathf.Abs (transform.position.y - height) > 0.5f) {
			return false;
		}

		// Default return true
		return true;
	}

	// Getters and Setters of ID for Grid
	public void SetID(int id) {
		_id = id;
	}

	public int GetID() {
		return _id;
	}

	// Set Neighbouring Waypoints once after building.
	public void SetNeighbours(List<GameObject> n) {
		neighbours = n;
	}

	public List<GameObject> GetNeighbours() {
		return neighbours;
	}

	// Euclidean Distance for a*
	// replaced with Vector3.Distance
	/*public float GetDistance(Transform target) {
		return Mathf.Sqrt (
			Mathf.Pow ((this.transform.position.x - target.transform.position.x), 2) + 
			Mathf.Pow ((this.transform.position.z - target.transform.position.z), 2)
		);
	}*/

}
