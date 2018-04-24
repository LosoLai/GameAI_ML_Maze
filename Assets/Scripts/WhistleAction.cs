using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Whistle Action
/// </summary>

[CreateAssetMenu (menuName = "AI/Action/Whistle")]
public class WhistleAction : Action {

	public override void Act (StateController controller) {
		Whistle (controller);
	}

	void Whistle (StateController controller) {
		if (controller.path.Count == 0) {
			return;
		}

		Vector3 node = controller.path [0];
		if (Vector3.Distance (controller.gameObject.transform.position, node) < 0.5f) {
			controller.path.RemoveAt (0);
		} else {
			//Debug.DrawLine (controller.gameObject.transform.position, node);
			controller.gameObject.transform.position = Vector3.MoveTowards (
				controller.gameObject.transform.position, node, controller.speed * Time.deltaTime
			);
			controller.transform.LookAt (node);
		}

	}
}
