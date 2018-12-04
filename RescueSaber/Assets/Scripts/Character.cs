using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	[Header("Balancing : General")]
	//Use "name" property of the gameobject for the character's name
	public float walkSpeed;
	public float contactDistance; //distance past which you are considered in contact with an object

	[Header("References")]
	public Transform visuals;
	public CharacterIcon icon;

	[Header("State : General")]
	public State state;

	[Header("State : Stats")] //all values between 0 and 1
	public float hp;
	public float big;
	public float chill;
	public float sharp;
	public float smooth;

	[Header("State : Hunger")]
	public Hunger hunger;
	float timeSinceLastMeal;

	[Header("State : Bus Position")]
	public BusSide side;
	public float rank;
	
	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private Bus bus { get { return gm.bus; } }
			private Vector3 busPos { get { return bus.transform.position; } }
		private FoodManager food { get { return gm.foodManager; } }
		private Stopover stopover { get { return gm.stopover; } }
		private TimeManager timeManager { get { return gm.timeManager; } }
		private UIManager ui { get { return gm.uIManager; } }

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
		GetInBus();
		SetHunger(Hunger.SATED);
		InitStats();
		InitIcon();
	}

	void Update () {
		if (state == State.WALKING_TO_BUS) WalkToBus (); //keep walking if you were
		if (state == State.WALKING_TO_STOPOVER) WalkToStopover (); //keep walking if you were

		if (Vector3.Distance(transform.position, bus.transform.position) > bus.InfluenceZone) 
			Death("You have left "+name+" to die on the road..."); //kill character if too far

		IncreaseHunger();
	}

	void OnDestroy () {
		characters.Remove(this);
		if (stopover) stopover.charactersInvolved.Remove(this);
		if (characters.Count == 0) gm.Defeat();
	}


	//UI
	void InitIcon() {
		SetHP(hp);
		icon.SetName(gameObject.name);
		icon.SetStats(big, chill, sharp, smooth);
		icon.ChangeFace(hunger);
	}


	//HP
	public void SetHP (float value) {
		hp = Mathf.Clamp01(value);
		icon.SetHP(hp);

		if (hp == 0) Death(name+" died of their many injuries...");
	}

	public void AddHP (float value) {
		SetHP(hp + value);
	}

	public void Damage (float value) {
		SetHP(hp - value);
	}

	void Death (string message) {
		ui.Log(message);

		icon.SetHP(0);
		icon.Death();
		Destroy(gameObject);
	}


	//STATS
	void InitStats() {
		hp = 1;
		big = (Random.value + Random.value + Random.value ) / 3; //A little bit random but not too much
		chill = (Random.value + Random.value + Random.value ) / 3;
		sharp = (Random.value + Random.value + Random.value ) / 3;
		smooth = (Random.value + Random.value + Random.value ) / 3;
	}


	//HUNGER
	void SetHunger (Hunger hunger) {
		this.hunger = hunger;
		icon.ChangeFace(hunger);
	}

	public void Eat() {
		if (hunger == Hunger.FULL) ui.Log(name+" is full !"); //Character is not hungry
		else if (!food.EnoughFood(1)) ui.Log("You're out of food !"); //No food left
		else if (state != State.BUS) ui.Log(name+" can't eat outside the bus"); //Cant eat outside the bus

		else { //Eat
			food.RemoveFood(1); //Subtract food from stocks

			if (hunger == Hunger.HUNGRY || hunger == Hunger.STARVING) SetHunger(Hunger.SATED); //Change hunger status
			else if (hunger == Hunger.SATED) SetHunger(Hunger.FULL);
			timeSinceLastMeal = 0;

			float regenAmount = Random.Range(food.minHpRegen, food.maxHpRegen); //Add hp
			AddHP(regenAmount);

			ui.Log(name+" ate and regained "+(int)(regenAmount*100)+" hp !");
		} 
	}

	void IncreaseHunger() {
		timeSinceLastMeal += Time.deltaTime;

		if (hunger!= Hunger.STARVING 
				&& hunger!= Hunger.HUNGRY 
				&& timeSinceLastMeal > timeManager.dayDuration * timeManager.daysBeforeHunger) { //character gets hungry after a while 
			SetHunger(Hunger.HUNGRY);
			ui.Log(name+" is getting hungry...");
		}

		else if (hunger!= Hunger.STARVING 
				&& timeSinceLastMeal > timeManager.dayDuration * timeManager.daysBeforeStarving) { //character gets really hungry after a while 
			SetHunger(Hunger.STARVING);
			ui.Log(name+" is getting really hungry...");
		}

		else if (timeSinceLastMeal > timeManager.dayDuration * timeManager.daysBeforeDeath) //character dies after a while
			Death(name+" has starved to death !");
	}


	//BUS
	void WalkToBus () {
		transform.LookAt(busPos);
		transform.position += transform.forward * Time.deltaTime * walkSpeed; //Walk towards target
		
		if (Vector3.Distance(busPos, transform.position) < contactDistance) GetInBus(); //If arrived, get in target
	}

	void GetInBus() {
		state = State.BUS;
		transform.parent = bus.characterHolder; //Become child of the bus

		Vector3 newPos = new Vector3(); //Calculate right position inside bus
		if (side == BusSide.LEFT) newPos += Vector3.right * bus.leftSeatX;
		else if (side == BusSide.RIGHT) newPos += Vector3.right * bus.rightSeatX;
		newPos += Vector3.forward * (bus.rank0Z - rank * bus.distanceBetweenRanks);

		transform.localPosition = newPos; //Move to your assigned seat
		transform.rotation = Quaternion.identity; //Look forward
	}

	public void ExitBus () {
		transform.position = bus.busExit.position;
		transform.parent = stopover.characterHolder; //Become a child of the stopover
		state = State.WALKING_TO_STOPOVER;
	}


	//STOPOVER
	void WalkToStopover () {
		Vector3 target = stopover.entrance.position;
		
		transform.LookAt(target);
		transform.position += transform.forward * Time.deltaTime * walkSpeed; //Walk towards target
		
		if (Vector3.Distance(target, transform.position) < contactDistance) GetInStopover(); //If arrived, get in target
	}

	void GetInStopover () {
			state = State.STOPOVER;
			visuals.gameObject.SetActive(false); //become invisible
			stopover.Enter(this);
	}

	public void ExitStopover () {
			state = State.WALKING_TO_BUS;
			visuals.gameObject.SetActive(true); //become visible
			stopover.Exit(this);
	}
}
