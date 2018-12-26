using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopover_2 : Stopover {
	[Space (25)]
	[Header ("Stopover 2 : Decision Making")]
	[Tooltip(
		"Stopover 2 : Decision Making\n\n"+
		"The characters are at a crossroads. One of them will have to take the lead and make a decision for the group.\n"+
		"- BIG : the character loses their cool and starts fighting. They find food.\n"+
		"- CHILL : the character makes friends, gets some food.\n"+
		"- SHARP : the character gets the others to heal them in exchange for food.\n"+
		"- SMOOTH : the character has a chance of convincing someone to join the team." )]
	public bool description;

	[Header("Balancing : BIG")]
	public int minFoodReward;
	public int maxFoodReward;

	[Header("Balancing : CHILL")]
	public int minFoodGift;
	public int maxFoodGift;

	[Header("Balancing : SHARP")]
	public int minFoodShared;
	public int maxFoodShared;
	public float healChance;
	public float minHeal;
	public float maxHeal;
	public float healPercentIfInsufficientFood;

	[Header("State")]
	public Character.Stat pick;


	//--------------------
	// BASIC METHODS
	//--------------------

	protected override void OnStart () { }

	protected override void OnUpdate () { }

	protected override void OnOnDestroy () { }


	//--------------------
	// ENTRIES AND EXITS
	//--------------------

	protected override void OnEnter () { }

	protected override void OnExit () { }


	//--------------------
	// EVENTS
	//--------------------

	protected override void OnEvent () {

		// INTRODUCTION
		if (round == 1) {
			ui.Log(charactersInvolved[0].name + " sees " + fightState.amountOfEnemies + " people");
			ui.Log("Their motives are yet unclear.");
			ui.Log("...");
		}

		// DECISION
		if (round == 4) {
			List<Character.Stat> characterStats = new List<Character.Stat> ();
			foreach (Character character in charactersInvolved) {
				characterStats.Add(character.big);
				characterStats.Add(character.chill);
				characterStats.Add(character.sharp);
				characterStats.Add(character.smooth);
			}
			pick = sm.test(characterStats);
		}

		// BIG
		if (pick.type == Character.StatType.BIG) {
			if (round == 4) ui.Log(pick.owner.name + " loses their cool and attacks !");
			if (round > 4) Fight();
		}

		// CHILL
		if (pick.type == Character.StatType.CHILL) {
			if (round == 4) {
				ui.Log(pick.owner.name + " makes friends with the locals.");
				ui.Log("They offer to share some of their food.");
			}
			if (round == 5) food.AddFood(Random.Range(minFoodGift, maxFoodGift+1));
			if (round == 6) Evacuate();
		}

		// SHARP
		if (pick.type == Character.StatType.SHARP) {
			if (round == 4) {
				int foodShared = Random.Range(minFoodShared, maxFoodShared);
				if (food.EnoughFood(foodShared)) {
					ui.Log(pick.owner.name + " gives them "+foodShared+" food. They offer to heal your crew.");
					food.RemoveFood(foodShared);
					foreach (Character c in charactersInvolved)
						if (Random.value < healChance) c.AddHP(Random.Range(minHeal, maxHeal));
					Evacuate();
				}

				else if (food.EnoughFood(1)) {
					ui.Log(pick.owner.name + " shares the few provisions you have with them. They offer to heal one of your passengers.");
					food.RemoveFood(food.food);
					charactersInvolved[Random.Range(0, charactersInvolved.Count-1)].AddHP(Random.Range(minHeal,maxHeal)*healPercentIfInsufficientFood);
					Evacuate();
				}

				else {
					ui.Log("They ask you for food. "+pick.owner.name+" politely declines and leaves quickly.");
					Evacuate();
				}
			}
		}

		// SMOOTH
		if (pick.type == Character.StatType.SMOOTH) {
			if (round == 4) {
				ui.Log(pick.owner.name + " convinces one of them to join your crew !");
				characterManager.Add();
				Evacuate();
			}
		}
	}


	//--------------------
	// FIGHT
	//--------------------

	protected override void OnVictory () {
		int foodGained = Random.Range(minFoodReward, maxFoodReward+1);
		gm.foodManager.AddFood(foodGained);
		ui.Log("You find "+foodGained+" food in the ruins");
	}

	protected override void OnDefeat () { }	
}