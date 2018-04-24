using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Action/DumbZombieGroup")]
public class DumbZombieGroupAction : Action {

	public float speed = 0.02f;
	float rotationSpeed = 4.0f;
	//float neighbourDistance = 2.0f;
	float neighbourDistance = 10.0f;
	Vector3 averageHeading;
	Vector3 averagePosition;

	public override void Act (StateController controller)
	{
		neighbourDistance = 10.0f;
		Flock(controller);
	}
	
	// Update is called once per frame
	private void Flock (StateController controller) {
		if (Random.Range (0, 5) < 1) {
			ApplyRules (controller);
		}
		controller.transform.Translate (0, 0, Time.deltaTime * controller.speed);
	}

	void ApplyRules (StateController controller) {
		List<StateController> gos;
		//gos = GlobleFlock.kidFlock;
		gos = controller.parent.children;

		Vector3 vcentre = Vector3.zero;
		Vector3 vavoid = Vector3.zero;
		float gSpeed = 0.1f;

		//Vector3 goalPos = GlobleFlock.goalPos;
		Vector3 goalPos = controller.parent.transform.position;
		float dist;

		int groupSize = 0;
		foreach (StateController go in gos) {
			if (go != controller) {
				dist = Vector3.Distance (go.transform.position, controller.transform.position);
				if (dist <= neighbourDistance) {
					
					vcentre += go.transform.position;
					groupSize++;

					if (dist < 1.0f)
						vavoid = vavoid + (controller.transform.position - go.transform.position);

					//flock anotherFlock = go.GetComponent<flock> ();
					//gSpeed = gSpeed + anotherFlock.speed;
					gSpeed = gSpeed + go.speed;
				}
			}
				
			if (groupSize > 0) {
				vcentre = vcentre / groupSize + (goalPos - controller.transform.position);
				speed = gSpeed / groupSize;

				Vector3 direction = (vcentre + vavoid) - controller.transform.position;
				if (direction != Vector3.zero)
					controller.transform.rotation = Quaternion.Slerp (controller.transform.rotation,
						Quaternion.LookRotation(direction),
						rotationSpeed + Time.deltaTime);
			}
		}
	}
}
