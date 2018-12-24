using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	[Header("Balancing : General")]
	//Use "name" property of the gameobject for the character's name
	public float walkSpeed;
	public float contactDistance; //distance past which you are considered in contact with an object

	[Header("References")]
	public CharacterIcon icon;
	public Transform visuals; //Object to hide in order to make the character invisible
	public List<MeshRenderer> colors; //Objects to change in order to change the character's colors

	[Header("State : General")]
	public State state;

	[Header("State : Stats")] //all values between 0 and 1
	public float hp;
	public Stat big;
	public Stat chill;
	public Stat sharp;
	public Stat smooth;

	[Header("State : Hunger")]
	public Hunger hunger;
	float timeSinceLastMeal;

	[Header("State : Bus Position")]
	public Seat seat;
	
	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private Bus bus { get { return gm.bus; } }
			private Vector3 busPos { get { return bus.transform.position; } }
		private CharacterManager characterManager { get { return gm.characterManager; } }
		private FoodManager food { get { return gm.foodManager; } }
		private StatsManager sm { get { return gm.statsManager; } }
		private Stopover stopover { get { return gm.stopover; } }
		private TimeManager timeManager { get { return gm.timeManager; } }
		private UIManager ui { get { return gm.uIManager; } }

	//Enums
	public enum State {BUS, STOPOVER, WALKING_TO_BUS, WALKING_TO_STOPOVER}
	public enum BusSide {LEFT, RIGHT}
	public enum Hunger {DEAD, STARVING, HUNGRY, SATED, FULL}
	public enum StatType {BIG, CHILL, SHARP, SMOOTH}

	//Bus seat struct
	[System.Serializable]
	public struct Seat {
		public BusSide side;
		public float rank;

		public Seat (BusSide side, float rank) {
			this.side = side;
			this.rank = rank;
		}
	}

	//Stat struct
	[System.Serializable]
	public struct Stat {
		[HideInInspector]
		public Character owner;
		public StatType type;
		public float value; //values between 0 and 1

		public Stat (Character owner, StatType type, float value) {
			this.owner = owner;
			this.type = type;
			this.value = value;
		}
	}

	//Characters list
	private static List<Character> _characters;
	public static List<Character> characters { 
		get { if (_characters == null) _characters = new List<Character>(); 
			return _characters; } }


	//--------------------
	// BASIC METHODS
	//--------------------

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

		if (Input.GetKeyDown(KeyCode.T)) StatTest();
		if (Input.GetKeyDown(KeyCode.A) && transform.GetSiblingIndex() == 0) characterManager.Add();
		if (Input.GetKeyDown(KeyCode.R)) characters[0].Remove();
	}

	void OnDestroy () {
		characters.Remove(this);
		if (stopover) stopover.charactersInvolved.Remove(this);
		if (characters.Count == 0) gm.Defeat();
	}


	//--------------------
	// UI
	//--------------------

	void InitIcon() {
		icon.character = this;
		SetHP(hp);
		icon.SetName(gameObject.name);
		icon.SetStats(big.value, chill.value, sharp.value, smooth.value);
		icon.SetFace(hunger);
		icon.name = this.name + " icon";
	}


	//--------------------
	// HP
	//--------------------

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
		Remove();
	}

	public void Remove () {
		Destroy(icon.gameObject);
		Destroy(gameObject);		
	}


	//--------------------
	// STATS
	//--------------------

	void InitStats() {
		hp = 1;

		big = new Stat (this, StatType.BIG, Mathf.Pow(Random.value, 2));
		chill = new Stat (this, StatType.CHILL, Mathf.Pow(Random.value, 2));
		sharp = new Stat (this, StatType.SHARP, Mathf.Pow(Random.value, 2));
		smooth = new Stat (this, StatType.SMOOTH, Mathf.Pow(Random.value, 2));
	}

	void StatTest () {
		float bonus = 0;
		StatType result = sm.test(new List<Stat> {big, chill, sharp, smooth}, out bonus).type;
		Debug.Log(name + " decided to act "+ result + " with an extra efficiency of " + sm.Intify(bonus));
	}


	//--------------------
	// HUNGER
	//--------------------

	void SetHunger (Hunger hunger) {
		this.hunger = hunger;
		icon.SetFace(hunger);
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

			ui.Log(name+" ate and regained "+sm.Intify(regenAmount)+" hp !");
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


	//--------------------
	// BUS
	//--------------------
	
	void WalkToBus () {
		transform.LookAt(busPos);
		transform.position += transform.forward * Time.deltaTime * walkSpeed; //Walk towards target
		
		if (Vector3.Distance(busPos, transform.position) < contactDistance) GetInBus(); //If arrived, get in target
	}

	void GetInBus() {
		state = State.BUS;
		transform.parent = bus.characterHolder; //Become child of the bus
		Sit();
	}

	public void ExitBus () {
		transform.position = bus.busExit.position;
		transform.parent = stopover.characterHolder; //Become a child of the stopover
		state = State.WALKING_TO_STOPOVER;
	}

	public void FindSeat () {
		List<Seat> freeSeats = new List<Seat>(); //Make a list of seats
		for (int i=0; i<bus.amountOfRanks; i++) { //Add all existing seats
			freeSeats.Add(new Seat (BusSide.LEFT, i));
			freeSeats.Add(new Seat (BusSide.RIGHT, i));
		}

		freeSeats.RemoveAt(0); //Nobody can sit at the driver's seat
		foreach(Character c in characters) freeSeats.Remove(c.seat); //Or at somebody else's seat

		seat = freeSeats[Random.Range(0,freeSeats.Count)]; //But the other ones are free :D
	}

	public void Sit () { //Get character asset to its assigned position in the bus
		if(seat.rank == 0 && seat.side == BusSide.LEFT) FindSeat(); //If no seat assigned, find one

		Vector3 newPos = new Vector3();
		if (seat.side == BusSide.LEFT) newPos += Vector3.right * bus.leftSeatX; //X position
		else if (seat.side == BusSide.RIGHT) newPos += Vector3.right * bus.rightSeatX;
		newPos += Vector3.forward * (bus.rank0Z - seat.rank * bus.distanceBetweenRanks); //Z position

		transform.localPosition = newPos; //Move to your assigned seat
		transform.rotation = Quaternion.identity; //Look forward
	}


	//--------------------
	// STOPOVER
	//--------------------

	void WalkToStopover () {
		Vector3 target = stopover.entrance.position;
		
		transform.LookAt(target);
		transform.position += transform.forward * Time.deltaTime * walkSpeed; //Walk towards target
		
		if (Vector3.Distance(target, transform.position) < contactDistance) GetInStopover(); //If arrived, get in target
	}

	void GetInStopover () {
			state = State.STOPOVER;
			SetVisible(false);
			stopover.Enter(this);
	}

	public void ExitStopover () {
			state = State.WALKING_TO_BUS;
			SetVisible(true);
			stopover.Exit(this);
	}


	//--------------------
	// APPEARANCE
	//--------------------

	void SetVisible (bool value) { //true = make visible, false = make invisible
		visuals.gameObject.SetActive(value);
	}

	public void SetColor (Material color) {
		foreach (MeshRenderer mr in colors)
			mr.material = color;
	}
}