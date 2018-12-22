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
		"- CHILL : the character makes friends, gets some HP healed.\n"+
		"- SHARP : the character gets the others to heal them in exchange for food.\n"+
		"- SMOOTH : the character has a chance of convincing someone to join the team." )]
	public bool description;

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
		if (round == 1) {
			ui.Log(charactersInvolved[0].name + "sees " + fightState.amountOfEnemies + " people");
			ui.Log("Their motives are yet unclear.");
			ui.Log("...");
		}

		if (round == 4) {
			List<Character.Stat> characterStats = new List<Character.Stat> ();
			foreach (Character character in charactersInvolved) {
				characterStats.Add(character.big);
				characterStats.Add(character.chill);
				characterStats.Add(character.sharp);
				characterStats.Add(character.smooth);
			}
			pick = sm.test(characterStats);

			if (pick.type == Character.StatType.BIG) {
				ui.Log(pick.owner.name + " loses their cool and attacks !");
			}

			if (pick.type == Character.StatType.CHILL) {
				ui.Log(pick.owner.name + " makes friends with the locals.");
				ui.Log("They offer to help you heal your wounded.");
			}

			if (pick.type == Character.StatType.SHARP) {
				ui.Log(pick.owner.name + " gives them some food. They offer to heal your crew.");
			}
			
			if (pick.type == Character.StatType.SMOOTH) {
				ui.Log(pick.owner.name + " convinces one of them to join your crew !");
			}
		}

		if (pick.type == Character.StatType.BIG)
			if (round > 4)
				Fight();

		if (pick.type == Character.StatType.CHILL) {
			ui.Log(pick.owner.name + " is chill");
		}

		if (pick.type == Character.StatType.SHARP) {
			ui.Log(pick.owner.name + " is sharp");
		}
		
		if (pick.type == Character.StatType.SMOOTH) {
			ui.Log(pick.owner.name + " is smooth");
		}

		if (round == 10) {
			ui.Log("evacuating");
			Evacuate();
		}
	}


	//--------------------
	// FIGHT
	//--------------------

	protected override void OnVictory () { }

	protected override void OnDefeat () { }	
}
