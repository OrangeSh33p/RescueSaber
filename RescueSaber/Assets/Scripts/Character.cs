using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	[Header("Balancing")]
	//Use "name" property of the gameobject for the character's name
	public float walkSpeed;

	[Header("State")]
	public float hp; //value is between 0 and 1
	public State state;

	[Header("Bus Position")]
	public BusSide side;
	public float rank;

	[Header("Hunger")]
	public Hunger hunger;
	float timeSinceLastMeal;

	[Header("References")]
	public Transform visuals;
	public CharacterIcon icon;
	
	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
	private Bus bus { get { return gm.bus; } }
	private Vector3 busPos { get { return bus.transform.position; } }
	private TimeManager timeManager { get { return gm.timeManager; } }

	//Enums
	public enum State {BUS, STOPOVER, WALKING_TO_BUS, WALKING_TO_STOPOVER}
	public enum BusSide {LEFT, RIGHT}
	public enum Hunger {DEAD, STARVING, HUNGRY, SATED, FULL}

	//Characters list
	static List<Character> _characters;
	public static List<Character> characters { 
		get { if (_characters == null) _characters = new List<Character>(); 
			return _characters; } }


	//BASIC METHODS
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

		if (Vector3.Distance(transform.position, bus.transform.position) > bus.distanceToAbandonment) Death(); //kill character if too far

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
		hp = Mathf.Clamp01(value);
		icon.SetHP(hp);

		if (hp == 0) Death();
	}

	public void AddHP (float value) {
		SetHP(hp + value);
	}

	void Death () {
		gm.uIManager.Log(name+" has died !");

		SetHP(0);
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
				&& timeSinceLastMeal > timeManager.dayDuration * timeManager.daysBeforeHunger) { //character gets hungry after a while 
			SetHunger(Hunger.HUNGRY);
			gm.uIManager.Log(name+" is getting hungry...");
		}

		else if (hunger!= Hunger.STARVING 
				&& timeSinceLastMeal > timeManager.dayDuration * timeManager.daysBeforeStarving) { //character gets really hungry after a while 
			SetHunger(Hunger.STARVING);
			gm.uIManager.Log(name+" is getting really hungry...");
		}

		else if (timeSinceLastMeal > timeManager.dayDuration * timeManager.daysBeforeDeath) //character dies after a while
			Death();
	}


	//BUS
	void WalkToBus () {
		transform.LookAt(busPos);
		transform.position += transform.forward * Time.deltaTime * walkSpeed; //Walk towards target
		
		if (Vector3.Distance(busPos, transform.position) < 2) GetInBus(); //If arrived, get in target
	}

	void GetInBus() {
		state = State.BUS;
		transform.parent = bus.characterHolder; //Become child of the bus (stop following the stopover and follow the bus instead)

		Vector3 newPos = new Vector3();
		if (side == BusSide.LEFT) newPos += Vector3.left * 0.25f;
		else if (side == BusSide.RIGHT) newPos += Vector3.right * 0.25f;
		newPos += Vector3.forward * ((3-rank)*0.5f - 0.75f);

		transform.localPosition = newPos; //Move to your assigned seat
		transform.rotation = Quaternion.identity; //Look forward
	}

	public void ExitBus () {
		transform.position = bus.busExit.position;
		transform.parent = gm.stopover.characterHolder; //Become a child of the stopover
		state = State.WALKING_TO_STOPOVER;
	}


	//STOPOVER
	void WalkToStopover () {
		Vector3 target = gm.stopover.entrance.position;
		
		transform.LookAt(target);
		transform.position += transform.forward * Time.deltaTime * walkSpeed; //Walk towards target
		
		if (Vector3.Distance(target, transform.position) < 2) GetInStopover(); //If arrived, get in target
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
