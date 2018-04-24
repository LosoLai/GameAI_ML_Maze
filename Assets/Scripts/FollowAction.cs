using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

[CreateAssetMenu (menuName = "AI/Action/Follow")]
public class FollowAction : Action {

	public override void Act(StateController controller) {
		if (Vector3.Distance (controller.transform.position, controller.target.transform.position) < 1f) {
			controller.rigidbody.velocity = Vector3.zero;
			return;
		}
		if (controller.RecalculationTimerElapsed ()) {
			RecalculatePath (controller);
		} else {
			Follow (controller);
		}
	}

	private void Follow (StateController controller) {
		if (controller.path.Count == 0) {
			Debug.Log ("No path");
			return;
		}

		if (controller.targetNode == null) {
			Debug.LogError ("No target node");
			return;
		}
				
		if (Vector3.Distance (controller.transform.position, controller.targetNode) < 0.5f) {
			controller.path.RemoveAt (0);
			if (controller.path.Count > 0) {
				controller.targetNode = controller.path [0];
			}
		} else {
			Debug.DrawLine (controller.transform.position, controller.targetNode);
			controller.transform.position = Vector3.MoveTowards (controller.transform.position, controller.targetNode, controller.speed * Time.deltaTime);
			controller.transform.LookAt (controller.targetNode);
		}

	}


	// Recalc the List of Vector3 to follow to target
	void RecalculatePath(StateController controller) {
		// First find grid and sanity check
		GameObject grid = GameObject.Find ("Grid");
		if (grid == null) {
			Debug.LogError ("Couldn't find Grid in SeekAction");
			return;
		}

		// Second find the controller and sanity check
		GridController gridController = grid.GetComponent<GridController> ();
		if (gridController == null) {
			Debug.LogError ("Couldn't find GridController in SeekAction");
			return;
		}

		gridController.ResetColors ();

		// Then calculate the path
		controller.path = gridController.AStar (controller.gameObject, controller.target);
		if (controller.path == null) {
			Debug.LogError ("No path found");
		} else {
			controller.targetNode = controller.path [0];
		}
	}
}
