using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopover : MonoBehaviour {
	[Header("Balancing")]
	public Transform entrance;
	public float distanceToShowText;
	public float distanceToShowSecondText;
	public float distanceToHideText;

	[Header("Balancing : Fight")]
	public float roundDuration;
	public int amountOfEnemies;
	public float damage;
	public float hp;
	
	[Header ("Balancing : Rewards")]
	public int minFood;
	public int maxFood;

	[Header("References")]
	public Transform characterHolder;

	[Header("State")]
	public List<Character> charactersInvolved;

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
	private UIManager ui { get { return gm.uIManager; } }
	private GameObject stopoverSign { get { return ui.stopoverSign; } }
	private Text stopoverText { get { return ui.stopoverText; } }

	void Start () {
		stopoverSign.SetActive(true);
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
		if (distance < distanceToShowText && distance > distanceToShowSecondText)
			stopoverText.text = "You can see something of interest "+ distance +" meters away...";
		else if (distance > distanceToShowSecondText && distance > distanceToHideText)
			stopoverText.text = "You can see something of interest very close...";
		else if (stopoverSign.activeInHierarchy == true)
			stopoverSign.SetActive(false);
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
		yield return new WaitForSeconds(2*roundDuration);

		//Keep playing till one of the sides has been defeated
		while (hp > 0 && charactersInvolved.Count > 0) {
			//Roll the dice
			int ran = Random.Range(1, 1+charactersInvolved.Count+amountOfEnemies);

			//Enemy is hit (rolled lower than the amount of characters)
			if (charactersInvolved.Count >= ran+1) {
				ui.Log("Enemy took damage !");
				hp --;
			} 

			//Character is hit (rolled higher than the amount of characters)
			else {
				//Random character takes damage
				Character target = charactersInvolved[Random.Range(0, charactersInvolved.Count)];
				target.AddHP(-damage);

				//If too injured, flee
				if (Random.value > target.hp) {
					ui.Log(target.name+" took damage and ran away !");
					target.ExitStopover();
				}

				else ui.Log(target.name+" took damage !");
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

		//All characters exit location (inverse "for" loop because we are removing elements from the list)
		for (int i=charactersInvolved.Count-1;i>=0;i--) {
			charactersInvolved[i].ExitStopover();
			yield return new WaitForSeconds(roundDuration);
		}
	}
}
