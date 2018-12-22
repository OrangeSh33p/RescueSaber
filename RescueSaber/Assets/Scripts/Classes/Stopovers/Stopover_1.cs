using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopover_1 : Stopover {
	[Space (25)]
	[Header ("Stopover 1 : Fight test")]
	[Tooltip(
		"Stopover 1 : Fight test\n\n"+
		"This stopover consists of a single fight againt 3 to 5 enemies. You get food as reward." )]
	public bool description;

	[Header("Balancing : Fight")]
	public int minEnemies;
	public int maxEnemies;
	public float minDamage;
	public float maxDamage;
	public float enemyBig;
	
	[Header ("Balancing : Rewards")]
	public int minFood;
	public int maxFood;


	//--------------------
	// ENTRIES AND EXITS
	//--------------------

	protected override void OnEnter () {
		if (charactersInvolved.Count == 1) {
			int amountOfEnemies = Random.Range(minEnemies, maxEnemies+1);
			ui.Log(amountOfEnemies + " bandits jump you !");
			fightState = new FightState(amountOfEnemies, enemyBig, minDamage, maxDamage);
		} 
	}


	//--------------------
	// EVENTS
	//--------------------

	protected override void OnEvent() {
		if (charactersInvolved.Count != 0) Fight();
	}


	//--------------------
	// FIGHT
	//--------------------

	protected override void OnVictory() {
			int foodGained = Random.Range(minFood, maxFood+1);
			gm.foodManager.AddFood(foodGained);
			ui.Log("You find "+foodGained+" food in the ruins");
	}
}
