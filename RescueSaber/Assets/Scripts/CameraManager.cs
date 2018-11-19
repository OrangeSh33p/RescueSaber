using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager> {
	public float accelMagnitude;
	public float decelMagnitude;
	public float returnMagnitude;

	Vector3 origin;

	void Start () {
		origin = transform.position;
	}

	void Update () {
		transform.position += (origin - transform.position)*Time.deltaTime;
	}

	public void Faster () {
		transform.position -= Vector3.forward*Time.deltaTime*accelMagnitude;
	}

	public void Slower () {
		transform.position += Vector3.forward*Time.deltaTime*decelMagnitude;
	}
}
