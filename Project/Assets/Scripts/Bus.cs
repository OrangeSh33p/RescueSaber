using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour {
	[Header("Balancing")]
	public float maxSpeed;
	public float acceleration;
	public float deceleration;
	public float distanceToTriggerStopovers;
	public float distanceToAbandonment;

	[Header("State")]
	public bool inStopover;

	private float currentSpeed;

	[Header("Boring")]
	public GameObject LD;
	public Transform characterHolder;
	public Transform busExit;

	private GameManager gm { get { return GameManager.Instance; } }

	//Storage
	private Ray ray;
	private RaycastHit hit;
	private int characterLayerMask = 1 << 9;
	


	void Update () {
		if (Input.GetKey(KeyCode.Z)) Faster();
		if (Input.GetKey(KeyCode.S)) Slower();
		Move();

		if (inStopover) ContinueStopover();

		if (Input.GetKeyDown(KeyCode.K)) Klaxon();
	}

	void Move() {
		LD.transform.position += new Vector3 (0, 0, -currentSpeed*Time.deltaTime);
	}

	void Faster() {
		if (currentSpeed < maxSpeed) {
			currentSpeed = Mathf.Min (currentSpeed + acceleration*Time.deltaTime, maxSpeed);
			gm.cameraManager.Faster();
		}
	}

	void Slower () {
		if (currentSpeed > 0) {
			currentSpeed = Mathf.Max (currentSpeed - deceleration*Time.deltaTime, 0);
			gm.cameraManager.Slower();
		}

		//if speed is null and stopover is close, start the stopover event
		if (currentSpeed == 0 
				&& gm.stopover 
				&& Vector3.Distance(gm.stopover.transform.position, transform.position) < distanceToTriggerStopovers)
			inStopover = true;
	}

	void Klaxon () {
		Debug.Log("POUET");

		//Every character in the stopover or walking towards it evacuates
		foreach(Character character in Character.characters)
			if (character.state == Character.State.STOPOVER || character.state == Character.State.WALKING_TO_STOPOVER)
				character.ExitStopover();
	}

	void ContinueStopover() {
		//Detect a player click
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Input.GetMouseButtonUp(0) && Physics.Raycast (ray, out hit, Mathf.Infinity, characterLayerMask)) {
			Character character = hit.collider.GetComponent<Character>();

			//If target is a character (not the driver), add it to the stopover
			if (character != null && !character.driver) {
				character.ExitBus();
			}
		}

	}
}