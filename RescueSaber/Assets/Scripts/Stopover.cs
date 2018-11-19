using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopover : MonoBehaviour {
	public GameObject stopoverSign;
	public Text stopoverText;
	public List<Character> charactersInvolved;
	private GameManager gm { get { return GameManager.Instance; } }
	public Transform entrance;
	public int hpLeft;
	public Transform characterHolder;

	void Start () {
		stopoverSign = gm.uIManager.stopoverSign;
		stopoverText = gm.uIManager.stopoverText;
		stopoverSign.SetActive(true);
	}

	void Update () {
		int distance = Mathf.FloorToInt(Vector3.Distance(gm.bus.transform.position, transform.position));
		if (distance > 20)
			stopoverText.text = "You can see something of interest "+ distance +" meters away...";
		else if (distance > 10)
			stopoverText.text = "You can see something of interest very close...";
		else if (stopoverSign.activeInHierarchy == true)
			stopoverSign.SetActive(false);
	}

	void OnDestroy () {
		gm.stopover = null;
		stopoverSign.SetActive(false);
	}

	//Called every time a character enters
	public void CheckEvent() {
			if (charactersInvolved.Count == 1) StartCoroutine(Fight());
	}

	IEnumerator Fight() {
		gm.uIManager.Log("Starting fight...");
		yield return new WaitForSeconds(2);

		//Keep playing till one of the sides has been defeated
		while (hpLeft > 0 && charactersInvolved.Count > 0) {
			//Roll the dice
			int ran = Random.Range(1, 6);

			//Enemy is hit (rolled lower than the amount of characters)
			if (charactersInvolved.Count >= ran+1) {
				gm.uIManager.Log("Enemy took damage !");
				hpLeft --;
			} 

			//Character is hit (rolled higher than the amount of characters)
			else {
				Character target = charactersInvolved[Random.Range(0, charactersInvolved.Count)];
				target.AddHP(-0.1f);

				//If too injured, flee
				if (Random.value > target.hp) {
					gm.uIManager.Log(target.name+" took damage and ran away !");
					target.ExitStopover();
				}

				else gm.uIManager.Log(target.name+" took damage !");
			} 

			//Wait
			yield return new WaitForSeconds(1);
		}

		//win
		if (hpLeft == 0) {
			int foodGained = Random.Range(2, 6);
			gm.uIManager.Log("Victory! Food gained : "+foodGained);
			gm.foodManager.AddFood(foodGained);
		} 

		//loss
		else gm.uIManager.Log("Retreat !");

		//All characters exit location (decreasing "for" loop because we are removing elements from the list)
		for (int i=charactersInvolved.Count-1;i>=0;i--) {
			charactersInvolved[i].ExitStopover();
			yield return new WaitForSeconds(1);
		}
	}
}
