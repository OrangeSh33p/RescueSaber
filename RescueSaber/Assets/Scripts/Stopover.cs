using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopover : MonoBehaviour {
	
	[Header("Balancing : General")]
	public Transform entrance;

	[Header("Balancing : Warning Sign")]
	public float distanceToShowText;
	public float distanceToShowSecondText;
	public float distanceToHideText;

	[Header("Balancing : Fight")]
	public float roundDuration;
	public int amountOfEnemies;
	public float minDamage;
	public float maxDamage;
	public float hp;
	
	[Header ("Balancing : Rewards")]
	public int minFood;
	public int maxFood;

	[Header("References")]
	public Transform characterHolder;

	[Header("State")]
	public List<Character> charactersInvolved;
	private bool signIsGone = false; //True if sign has been showed once. after that, IT IS GONE

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private UIManager ui { get { return gm.uIManager; } }
			private GameObject stopoverSign { get { return ui.stopoverSign; } }
			private Text stopoverText { get { return ui.stopoverText; } }
		private StatsManager sm { get { return gm.statsManager; } }

	void Start () {
		gm.stopover = this;
	}

	void Update () {
		DisplaySign();
	}

	void OnDestroy () {
		gm.stopover = null;
	}

	void DisplaySign () {
		int distance = Mathf.FloorToInt(Vector3.Distance(gm.bus.transform.position, transform.position));

		if (!signIsGone && distance < distanceToShowText && distance > distanceToShowSecondText) { //If close enough, show stopover sign
			stopoverSign.SetActive(true);
			stopoverText.text = "You can see something of interest "+ distance +" meters away...";
		}

		else if (distance < distanceToShowSecondText && distance > distanceToHideText) { //If closer, change stopover sign text
			stopoverText.text = "You can see something of interest very close...";
		}

		else if (distance < distanceToHideText && stopoverSign.activeInHierarchy) { //If very close, hide sign
			stopoverSign.SetActive(false);
			signIsGone = true;
		}
	}

	//Called by characters to enter the stopover
	public void Enter(Character character) {
		charactersInvolved.Add(character);
		if (charactersInvolved.Count == 1) StartCoroutine(Fight());
	}

	//Called by characters to exit the stopover
	public void Exit (Character character) {
		charactersInvolved.Remove(character);
	}

	IEnumerator Fight() {
		ui.Log("Starting fight...");
		yield return new WaitForSeconds(2 * roundDuration);

		//Keep playing till one of the sides has been defeated
		while (hp > 0 && charactersInvolved.Count > 0) {
			//Roll the dice (pick a random fighter who lands a blow)
			int ran = Random.Range(1, charactersInvolved.Count + amountOfEnemies + 1);

			//Enemy is hit (rolled lower than the amount of characters, i.e. result is a character)
			if (ran <= charactersInvolved.Count) {
				ui.Log("Enemy took damage !");
				hp --;
			} 

			//Character is hit (rolled higher than the amount of characters, i.e. result is an enemy)
			else {
				//Random character takes damage
				Character target = charactersInvolved[Random.Range(0,charactersInvolved.Count)];
				float damage = Random.Range(minDamage, maxDamage);
				
				//Handle log messages
				if (target.hp > damage && target.hp < Random.value) { //Character takes damage and runs away
					ui.Log(target.name+" took "+sm.Intify(damage)+" damage and ran away !");//Chance to run away after being hit = % hp missing
					target.ExitStopover();
				}

				else ui.Log(target.name+" took "+sm.Intify(damage)+" damage !"); //Character takes damage

				//Deduct character hp
				target.Damage(damage);
			} 

			//Wait
			yield return new WaitForSeconds(roundDuration);
		}

		//win
		if (hp == 0) {
			int foodGained = Random.Range(minFood, maxFood+1);
			ui.Log("Victory! Food gained : "+foodGained);
			gm.foodManager.AddFood(foodGained);
		} 

		//loss
		else ui.Log("Retreat !");

		//All characters exit location
		for (int i=charactersInvolved.Count-1;i>=0;i--) { //Inverse "for" loop because we are removing elements from the list
			charactersInvolved[i].ExitStopover();
			yield return new WaitForSeconds(roundDuration);
		}
	}
}
