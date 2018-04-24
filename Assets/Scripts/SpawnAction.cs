using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn Action takes a prefab, and spawns it around the unit as an action.
/// It is used mostly for a smart unit that spawns dumb units in its starting state before transitioning to it's main behaviour
/// </summary>

[CreateAssetMenu (menuName = "AI/Action/Spawn")]
public class SpawnAction : Action {

	// What to spawn and how many
	public GameObject prefab;
	public int spawnAmount;

	public override void Act (StateController controller) {
		Spawn (controller);
	}

	private void Spawn (StateController controller) {
		for (int i = 0; i < spawnAmount; i++) {
			GameObject child = Instantiate (
				prefab,
				SpawnInCircle (controller.transform.position, 3.0f, 360 / spawnAmount * i),
				Quaternion.identity
			);

			// Set the StateController links to parent and child on both sides.
			// So children know their parent, and vice versa
			StateController childStateController = child.GetComponent<StateController> ();
			childStateController.parent = controller;
			controller.children.Add (childStateController);
		}
	}

	// Math came from this discussion;
	// https://answers.unity.com/questions/714835/best-way-to-spawn-prefabs-in-a-circle.html
	private Vector3 SpawnInCircle(Vector3 center, float radius, int a) {
		return new Vector3 (
			center.x + radius * Mathf.Sin (a * Mathf.Deg2Rad),
			center.y,
			center.z + radius * Mathf.Cos (a * Mathf.Deg2Rad)
		);
	}
		
}
