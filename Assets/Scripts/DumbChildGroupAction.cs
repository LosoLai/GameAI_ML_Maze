using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Action/DumbChildGroup")]
public class DumbChildGroupAction : Action {
	private static float ROTSPEED = 1.0f;
	private static float SPEED = 0.8f;

	GameObject[] leaders;
	int currentLeader = 0;

	void Awake()
	{
		leaders = GameObject.FindGameObjectsWithTag("SmartKid");
	}

	public override void Act(StateController controller) {
		WanderAndFollow (controller);
	}

	private void WanderAndFollow(StateController controller) {
		if (leaders.Length == 0)
			return;

		Vector3 randomRange = new Vector3 (Random.Range (10.0f, -10.0f), Random.Range (0.3f, -0.3f), 0);
		Vector3 direction = leaders [currentLeader].transform.position - controller.transform.position;
		direction += randomRange;

		controller.transform.rotation = Quaternion.Slerp (controller.transform.rotation,
														  Quaternion.LookRotation (direction),
														  ROTSPEED * Time.deltaTime);
		controller.transform.Translate (0, 0, Time.deltaTime * SPEED);
	}

}
