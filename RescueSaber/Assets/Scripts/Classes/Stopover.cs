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
	private bool signIsGone = false; //True if sign has been showed once. after that, IT IS GONE

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private UIManager ui { get { return gm.uIManager; } }
			private GameObject stopoverSign { get { return ui.stopoverSign; } }
			private Text stopoverText { get { return ui.stopoverText; } }
		private StopoverManager som { get { return gm.stopoverManager; } }
		private StatsManager sm { get { return gm.statsManager; } }

	//Fightstate struct (used to initialise a fight)
	protected struct FightState {
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

	//BASIC METHODS
	void Start () {
		gm.stopover = this;
		OnStart ();
	}

	protected virtual void OnStart () {}

	void Update () {
		DisplaySign();
		OnUpdate ();
	}

	protected virtual void OnUpdate () {}

	void OnDestroy () {
		gm.stopover = null;
		OnOnDestroy ();
	}

	protected virtual void OnOnDestroy () {}

	//SIGN
	void DisplaySign () {
		int distance = Mathf.FloorToInt(Vector3.Distance(gm.bus.transform.position, transform.position));

		if (!signIsGone && distance < som.distanceToShowText && distance > som.distanceToShowSecondText) { //If close enough, show stopover sign
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

	//ENTRIES AND EXITS
	public void Enter(Character character) { //Called by characters to enter the stopover
		charactersInvolved.Add(character);
		OnEnter();
	}

	protected virtual void OnEnter () {}
	
	public void Exit (Character character) { //Called by characters to exit the stopover
		charactersInvolved.Remove(character);
		OnExit();
	}

	protected virtual void OnExit () {}	
	
	//FIGHT
	protected IEnumerator Fight(FightState fightState) {
		yield return new WaitForSeconds(som.roundDuration);

		//Keep playing till one of the sides has been defeated
		while (fightState.amountOfEnemies > 0 && charactersInvolved.Count > 0) {
			//Test all fighters' BIG, result is the one who lands a blow
			Character.Stat enemyBig = new Character.Stat (null, Character.StatType.BIG, fightState.enemyBig); //Placeholder stat for enemy's BIG

			List<Character.Stat> fighters = new List<Character.Stat>(); //Make a list of all the fighters
			foreach (Character character in charactersInvolved) fighters.Add(character.big); //Add all characters
			for (int i=0;i<fightState.amountOfEnemies;i++) fighters.Add(enemyBig); //Add all enemies

			Character attacker = sm.test(fighters).owner; //Test values against each other, owner of the winning stat becomes attacker


			//Enemy is hit (attacker is a character)
			if (attacker != null) {
				ui.Log(attacker.name + " defeated an enemy !");
				fightState.amountOfEnemies --;
			} 

			//Character is hit (attacker is null, i.e. an enemy)
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

			//Wait till the start of next round
			yield return new WaitForSeconds(som.roundDuration);
		}

		//Determine outcome
		if (fightState.amountOfEnemies == 0) OnVictory ();
		else OnDefeat ();

		//All characters exit location
		for (int i=charactersInvolved.Count-1;i>=0;i--) { //Inverse "for" loop because we are removing elements from the list
			charactersInvolved[i].ExitStopover();
			yield return new WaitForSeconds(som.roundDuration);
		}
	}

	private void Victory () {
		ui.Log("Victory!");
		OnVictory ();
	}

	protected virtual void OnVictory () { }

	private void Defeat () {
		ui.Log("Retreat !");
		OnDefeat ();
	}

	protected virtual void OnDefeat () { }
}
