using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogText : MonoBehaviour {
	[Header("Balancing")]
	public float timeToDestruction;
	public float speed;

	//State
	private bool permanent;

	void Update () {
		Move ();
		if (!permanent) CheckDestroy();
	}

	void Move() {
		transform.position += new Vector3(0, Time.deltaTime * speed, 0);
	}

	void CheckDestroy () {
		timeToDestruction -= Time.deltaTime;
		if (timeToDestruction < 0) Destroy(gameObject);
	}

	public void MakePermanent () {
		permanent = true;
	}
}
