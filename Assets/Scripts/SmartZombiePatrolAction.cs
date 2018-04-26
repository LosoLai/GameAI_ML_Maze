using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Action/Patrol")]
public class SmartZombiePatrolAction : Action {
	private GameObject[] waypoints;
	private int currentWPIndex;

	void Awake()
	{
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		currentWPIndex = 0;
	}

	public override void Act (StateController controller)
	{
		Patrol (controller);
	}

	private void Patrol(StateController controller) {
		if (waypoints.Length == 0)
			return;
		
		if(Vector3.Distance(waypoints[currentWPIndex].transform.position, 
			controller.transform.position) < 1.0f)
		{
			currentWPIndex++;
			if(currentWPIndex >= waypoints.Length)
			{
				currentWPIndex = 0;
			}
		}

		var direction = waypoints[currentWPIndex].transform.position - controller.transform.position;
		controller.transform.rotation = Quaternion.Slerp (
			controller.transform.rotation,
			Quaternion.LookRotation(direction),
			controller.rotationSpeed * Time.deltaTime);
		controller.transform.Translate (0, 0, Time.deltaTime * controller.speed);
	}
}
