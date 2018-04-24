using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Action/SmartZombieWander")]
public class SmartZombieWanderAction : Action {

	public Vector3 circleCenter;
	public float circleDistance = 10.0f;


	public Vector3 displaceForce; 
	public float circleRadius = 20.0f;
	private float wanderAngle = 60.0f;
	public float rotateChange = 60.0f;

	// replaced with controller.speed
	//public float maxspeed = 2.0f;

	//variables used for character facing
	public float rotateSpeed = 10.0f;
	public float rotateMag = 0.2f;

	public override void Act(StateController controller) {
		if (controller.RecalculationTimerElapsed () || Vector3.Distance (controller.transform.position, controller.wanderForce) < 0.5f) {
			WanderForce (controller);
		} else {
			Wander (controller);
		}
	}

	private void Wander(StateController controller) {

		//moves the character to the new position
		controller.transform.position = Vector3.MoveTowards (controller.transform.position, controller.wanderForce, controller.speed * Time.deltaTime);

		//draws a line to show the target point in the Camera scene. Can delete this eventually
		Debug.DrawLine (controller.transform.position, controller.wanderForce, Color.yellow, 1);


		//character slowly faces towards new target direction 
		Vector3 targetDir = controller.wanderForce - controller.transform.position;
		Vector3 newDir = Vector3.RotateTowards (controller.transform.forward, targetDir, rotateSpeed * Time.deltaTime, rotateMag);
		controller.transform.rotation = Quaternion.LookRotation (newDir);
	}

	void WanderForce (StateController controller) {
		circleCenter = controller.transform.position + (controller.transform.forward * circleDistance);
		//Debug.DrawLine (controller.transform.position, circleCenter, Color.red, 100.0f);
		//print ("Circle Center: " + circleCenter);

		displaceForce = new Vector3 (0.0f, 0.0f, 0.1f);
		displaceForce = displaceForce * circleRadius;

		float len = displaceForce.magnitude;
		displaceForce.x = Mathf.Cos (wanderAngle * (Mathf.PI / 180)) * len;
		displaceForce.z = Mathf.Sin (wanderAngle * (Mathf.PI / 180)) * len;

		//this is supposed to make sure that the new target point is never anywhere above/below ground
		//need to ask someone if there is a better solution for this one

		//randomly calculates the wander angle for the next wander point
		wanderAngle += ((Random.Range (0.0f, rotateChange) - (rotateChange * 0.5f))) % 360;
		//Debug.Log (wanderAngle);

		//adding up the circle position and displacement force for the wander point
		controller.wanderForce = circleCenter + displaceForce;

	}
}
