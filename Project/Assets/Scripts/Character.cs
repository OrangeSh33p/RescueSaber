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
	public enum Hunger {DEAD, STARVING, HUNGRY, SATED, FULL}
	public Hunger hunger;
	float timeSinceLastMeal;

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
		hp = 1;
		hunger = Hunger.SATED;
		GetInBus();
		InitIcon();
	}

	void Update () {
		if (state == State.WALKING_TO_BUS) WalkToBus (); //keep walking if you were
		if (state == State.WALKING_TO_STOPOVER) WalkToStopover (); //keep walking if you were

		if (Input.GetKeyDown(KeyCode.W) && transform.GetSiblingIndex() == 2) Death(); // ***** W : kill first character *****

		if (state != State.BUS && Vector3.Distance(transform.position, gm.bus.transform.position) > gm.bus.distanceToAbandonment)
			Death(); //kill character if too far

		IncreaseHunger();
	}

	void OnDestroy () {
		characters.Remove(this);
		if (characters.Count == 0) gm.Defeat();
	}


	//UI
	void InitIcon() {
		SetHP(hp);
		icon.SetName(gameObject.name);
		icon.ChangeFace(hunger);
	}


	//HP
	public void SetHP (float value) {
		hp = Mathf.Clamp(value, 0, 1);
		icon.SetHP(hp);

		if (hp == 0) Death();
	}

	public void AddHP (float value) {
		SetHP(hp + value);
	}

	void Death () {
		gm.uIManager.Log(name+" has died !");

		icon.Death();
		Destroy(gameObject);
	}


	//HUNGER
	void SetHunger (Hunger hunger) {
		this.hunger = hunger;
		icon.ChangeFace(hunger);
	}

	public void Eat() {
		if (hunger == Hunger.FULL) gm.uIManager.Log(name+" is full !"); //Character is not hungry
		else if (!gm.foodManager.EnoughFood(1)) gm.uIManager.Log("You're out of food !"); //No food left

		else { //Eat
			gm.foodManager.AddFood(-1);
			AddHP(0.2f);
			if (hunger == Hunger.HUNGRY || hunger == Hunger.STARVING) SetHunger(Hunger.SATED);
			else if (hunger == Hunger.SATED) SetHunger(Hunger.FULL);
			timeSinceLastMeal = 0;

			gm.uIManager.Log(name+" ate and feels better !");
		} 
	}

	void IncreaseHunger() {
		timeSinceLastMeal += Time.deltaTime;

		if (hunger!= Hunger.STARVING 
				&& hunger!= Hunger.HUNGRY 
				&& timeSinceLastMeal > gm.timeManager.dayDuration * gm.timeManager.daysBeforeHunger) { //character gets hungry after a while 
			SetHunger(Hunger.HUNGRY);
			gm.uIManager.Log(name+" is getting hungry...");
		}

		if (hunger!= Hunger.STARVING 
				&& timeSinceLastMeal > gm.timeManager.dayDuration * gm.timeManager.daysBeforeStarving) { //character gets really hungry after a while 
			SetHunger(Hunger.STARVING);
			gm.uIManager.Log(name+" is getting really hungry...");
		}

		else if (timeSinceLastMeal > gm.timeManager.dayDuration * gm.timeManager.daysBeforeDeath) //character dies after a while
			Death();
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
