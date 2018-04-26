using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Action/SmartChildWander")]
public class SmartChildWanderAction : Action {
	private Vector3 randomRange;
	private static float WANDERTIME = 50.0f;
	private static float ROTSPEED = 2.0f;
	private float timer = WANDERTIME;
	private bool isTimeOut = false;

	public override void Act(StateController controller) {
		//moves the character to the new position
		randomRange = new Vector3 (Random.Range(360.0f, -360.0f), Random.Range(0.3f, -0.3f), 0);
		Wander (controller);
	}

	private void Wander(StateController controller) {
		//check is it timeout or not
		if (timer <= 0)
			isTimeOut = true;

		if (!isTimeOut) {
			timer -= Time.deltaTime;
		} else {
			//if time out, default setting
			randomRange.Set(Random.Range(360.0f, -360.0f), Random.Range(0.3f, -0.3f), 0);
			timer = WANDERTIME;
			isTimeOut = false;
		}

		controller.transform.rotation = Quaternion.Slerp (controller.transform.rotation,
														  Quaternion.LookRotation(randomRange),
														  ROTSPEED * Time.deltaTime);
		controller.transform.Translate (Vector3.forward * Time.deltaTime);

		//controller.transform.position = Vector3.MoveTowards (controller.transform.position, controller.wanderForce, controller.speed * Time.deltaTime);
	}
}
