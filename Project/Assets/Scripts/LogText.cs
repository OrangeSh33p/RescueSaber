using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogText : MonoBehaviour {
	public float timeToDestruction;
	public float speed;
	public bool permanent;

	void Update () {
		timeToDestruction -= Time.deltaTime;

		transform.position += new Vector3(0, Time.deltaTime * speed, 0);

		if (timeToDestruction < 0 && !permanent) Destroy(gameObject);
	}
}
