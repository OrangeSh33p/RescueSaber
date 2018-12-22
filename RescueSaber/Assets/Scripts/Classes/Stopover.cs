using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Stopover : MonoBehaviour {
	[Header ("General")]

	[Header("References")]
	public Transform characterHolder;
	public Transform entrance;

	[Header("State")]
	public List<Character> charactersInvolved;
	protected int round; //Index of current round. Set it to minus one to stop events
	protected FightState fightState;
	private bool signIsGone = false; //True if sign has been showed once. after that, IT IS GONE

	//Storage
	protected GameManager gm { get { return GameManager.Instance; } }
		protected UIManager ui { get { return gm.uIManager; } }
			protected GameObject stopoverSign { get { return ui.stopoverSign; } }
			protected Text stopoverText { get { return ui.stopoverText; } }
		protected StopoverManager som { get { return gm.stopoverManager; } }
		protected StatsManager sm { get { return gm.statsManager; } }

	//Fightstate struct (used to initialise a fight)
	[System.Serializable]
	public struct FightState {
		public int amountOfEnemies;
		public float enemyBig;
		public float minDamage;
		public float maxDamage;

		public FightState (bool useless) { //Parameter is useless but mandatory
			amountOfEnemies = GameManager.Instance.stopoverManager.defaultAmountOfEnemies; 	//A mouthful but we cant access other references. 
			enemyBig = GameManager.Instance.stopoverManager.defaultEnemyBig;				//Default values in stopover manager.
			minDamage = GameManager.Instance.stopoverManager.defaultMinDamage;
			maxDamage = GameManager.Instance.stopoverManager.defaultMaxDamage;
		}

		public FightState (int amountOfEnemies, float enemyBig, float minDamage, float maxDamage) {
			this.amountOfEnemies = amountOfEnemies;
			this.enemyBig = enemyBig;
			this.minDamage = minDamage;
			this.maxDamage = maxDamage;
		}
	}


	//--------------------
	// BASIC METHODS
	//--------------------

	private void Start () {
		gm.stopover = this;
		InitializeFightState ();
		OnStart ();
	}

	protected virtual void OnStart () {}

	private void Update () {
		DisplaySign();
		OnUpdate ();
	}

	protected virtual void OnUpdate () {}

	private void OnDestroy () {
		gm.stopover = null;
		OnOnDestroy ();
	}

	protected virtual void OnOnDestroy () {}


	//--------------------
	// SIGN
	//--------------------

	private void DisplaySign () {
		if (!signIsGone) {
			int distance = Mathf.FloorToInt(Vector3.Distance(gm.bus.transform.position, transform.position));

			if (distance < som.distanceToShowText && distance > som.distanceToShowSecondText) { //If close enough, show stopover sign
				stopoverSign.SetActive(true);
				stopoverText.text = "You can see something of interest "+ distance +" meters away...";
			}

			else if (distance < som.distanceToShowSecondText && distance > som.distanceToHideText) { //If closer, change stopover sign text
				stopoverText.text = "You can see something of interest very close...";
			}

			else if (distance < som.distanceToHideText && stopoverSign.activeInHierarchy) { //If very close, hide sign
				stopoverSign.SetActive(false);
				signIsGone = true;
			}
		}
	}


	//--------------------
	// ENTRIES AND EXITS
	//--------------------

	public void Enter(Character character) { //Called by characters
		charactersInvolved.Add(character);
		if (charactersInvolved.Count == 1) StartEvents(); //When character enters an empty stopover, start events			
		OnEnter();
	}

	protected virtual void OnEnter () {}
	
	public void Exit (Character character) { //Called by characters
		charactersInvolved.Remove(character);
		OnExit();
	}

	protected virtual void OnExit () {}	

	protected void Evacuate () {
		StartCoroutine (_Evacuate());
	}

	private IEnumerator _Evacuate () {
		StopEvents();
		while (charactersInvolved.Count > 0) {
			yield return new WaitForSeconds(som.roundDuration);
			charactersInvolved[charactersInvolved.Count-1].ExitStopover();
		}
	}


	//--------------------
	// EVENTS
	//--------------------

	private void StartEvents () {
		round = 1;
		StartCoroutine (Events());
	}

	private void NextEvent () {
		round ++;
	}

	private void StopEvents () {
		round = -1;
	}

	protected bool EventsHappening () {
		return round > 0;
	}

	private IEnumerator Events () {
		while (EventsHappening()) {
			yield return new WaitForSeconds(som.roundDuration);
			OnEvent ();
			NextEvent ();
		}
	}

	protected virtual void OnEvent () { }


	//--------------------
	// FIGHT
	//--------------------

	private void InitializeFightState () {
		//Initialize fightstate if it is null
		if (fightState.amountOfEnemies == 0 && fightState.enemyBig == 0 && fightState.minDamage == 0 && fightState.maxDamage == 0)
			fightState = new FightState (true);
	}

	protected void Fight () { //Play one round of fight
		//Test all fighters' BIG, result is the one who lands a blow
		Character.Stat enemyBig = new Character.Stat (null, Character.StatType.BIG, fightState.enemyBig); //Placeholder stat for enemy's BIG

		List<Character.Stat> fighters = new List<Character.Stat>(); //Make a list of all the fighters
		foreach (Character character in charactersInvolved) fighters.Add(character.big); //Add all characters
		for (int i=0;i<fightState.amountOfEnemies;i++) fighters.Add(enemyBig); //Add all enemies

		Character attacker = sm.test(fighters).owner; //Test values against each other, owner of the winning stat becomes attacker

		//If enemy is hit (attacker is a character)
		if (attacker != null) {
			ui.Log(attacker.name + " defeated an enemy !");
			fightState.amountOfEnemies --;
		} 

		//If character is hit (attacker is null, i.e. an enemy)
		else {
			Character target = charactersInvolved[Random.Range(0,charactersInvolved.Count)]; //Pick random character
			float damage = Random.Range(fightState.minDamage, fightState.maxDamage); //Generate damage
			
			//Handle log messages
			if (target.hp > damage && target.hp < Random.value) { //Character takes damage and runs away
				ui.Log(target.name+" took "+sm.Intify(damage)+" damage and ran away !");//Chance to run away after being hit = % hp missing
				target.ExitStopover();
			}

			else ui.Log(target.name+" took "+sm.Intify(damage)+" damage !"); //Character takes damage

			target.Damage(damage); //Deduct character hp
		} 

		//Check if one side has been defeated
		if (fightState.amountOfEnemies == 0) Victory ();
		else if (charactersInvolved.Count == 0) Defeat ();
	}

	private void Victory () {
		ui.Log("Victory !");
		Evacuate();
		OnVictory ();
	}

	protected virtual void OnVictory () { }

	private void Defeat () {
		ui.Log("Retreat !");
		Evacuate();
		OnDefeat ();
	}

	protected virtual void OnDefeat () { }
}