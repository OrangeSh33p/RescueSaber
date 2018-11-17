using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	//Position
	public enum Side {LEFT, RIGHT}
	public Side side;
	public float rank;
	public bool driver;
	public enum State {BUS, STOPOVER, WALKING_TO_STOPOVER, WALKING_TO_BUS}
	public State state;
	public float walkSpeed;
	public Transform visuals;

	static List<Character> _characters;
	public static List<Character> characters { 
		get { if (_characters == null) _characters = new List<Character>(); 
			return _characters; } }

	//Stats
	//Use "name" property of the gameobject for the character's name
	public CharacterIcon icon;
	public float hp; //value is between 0 and 1


	private GameManager gm { get { return GameManager.Instance; } }

	void Start () {
		characters.Add(this);
		hp = Random.value; // ***** start : random HP *****
		GetInBus();
		InitIcon();
	}

	void Update () {
		//if (Input.GetKeyDown(KeyCode.A))SetHP(Random.value);
		if (state == State.WALKING_TO_BUS) WalkToBus ();
		if (state == State.WALKING_TO_STOPOVER) WalkToStopover ();

		if (Input.GetKeyDown(KeyCode.W) && transform.GetSiblingIndex() == 2) Death(); // ***** W : kill first character *****

		if (state != State.BUS && Vector3.Distance(transform.position, gm.bus.transform.position) > gm.bus.distanceToAbandonment)
			Death();
	}

	void OnDestroy () {
		characters.Remove(this);
	}


	//UI
	void InitIcon() {
		SetHP(hp);
		icon.SetName(gameObject.name);
	}


	//STATS
	public void SetHP (float value) {
		hp = Mathf.Clamp(value, 0, 1);
		icon.SetHP(hp);

		if (hp == 0) Death();
	}

	public void Damage (float value) {
		SetHP(hp - value);
	}

	void Death () {
		Debug.Log(name+" has died :/");

		Destroy(icon.gameObject);
		Destroy(gameObject);
	}


	//BUS
	void WalkToBus () {
		Vector3 target = gm.bus.transform.position;

		//Walk towards target
		transform.LookAt(target);
		transform.position += transform.forward * Time.deltaTime * walkSpeed;

		//If arrived, get in target
		if (Vector3.Distance(target, transform.position) < 2) GetInBus();
	}

	void GetInBus() {
		transform.parent = gm.bus.characterHolder;

		Vector3 newPos = new Vector3();

		if (side == Side.LEFT) newPos += Vector3.left * 0.25f;
		if (side == Side.RIGHT) newPos += Vector3.right * 0.25f;
		newPos += Vector3.forward * ((3-rank)*0.5f - 0.75f);

		transform.localPosition = newPos;
		transform.rotation = Quaternion.identity;
	}

	public void ExitBus () {
		transform.position = gm.bus.busExit.position;
		transform.parent = gm.stopover.characterHolder;
		state = State.WALKING_TO_STOPOVER;
	}


	//STOPOVER
	void WalkToStopover () {
		Vector3 target = gm.stopover.entrance.position;

		//Walk towards target
		transform.LookAt(target);
		transform.position += transform.forward * Time.deltaTime * walkSpeed;

		//If arrived, get in target
		if (Vector3.Distance(target, transform.position) < 2) GetInStopover();
	}

	void GetInStopover () {
			state = State.STOPOVER;
			visuals.gameObject.SetActive(false);
			gm.stopover.charactersInvolved.Add(this);
			gm.stopover.CheckEvent();
	}

	public void ExitStopover () {
			state = State.WALKING_TO_BUS;
			visuals.gameObject.SetActive(true);
			gm.stopover.charactersInvolved.Remove(this);
	}
}
