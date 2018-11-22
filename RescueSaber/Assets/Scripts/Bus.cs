using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour {
	[Header("Balancing : Movement")]
	public float maxSpeed;
	public float acceleration;
	public float deceleration;
	public float InfluenceZone;

	[Header("Balancing : Seat positions")]
	public float leftSeatX;
	public float rightSeatX;
	public float rank0Z;
	public float distanceBetweenRanks;

	[Header("References")]
	public GameObject LD;
	public Transform characterHolder;
	public Transform busExit;

	[Header("State")]
	private float currentSpeed;

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
	private Ray ray;
	private RaycastHit hit;
	private int characterLayerMask = 1 << 9;

	void Update () {
		if (Input.GetKey(KeyCode.Z)) Faster();
		if (Input.GetKey(KeyCode.S)) Slower();
		if (currentSpeed > maxSpeed) Slower();
		Move();

		//If close to a stopover, be ready to get player input
		if (gm.stopover != null && Vector3.Distance(transform.position, gm.stopover.transform.position) < InfluenceZone) 
			ContinueStopover();

		if (Input.GetKeyDown(KeyCode.K)) Honk();

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
	}

	void Honk () {
		gm.uIManager.Log("HONK");

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

			//If target is a character, add it to the stopover
			if (character != null) character.ExitBus();
		}
	}
}