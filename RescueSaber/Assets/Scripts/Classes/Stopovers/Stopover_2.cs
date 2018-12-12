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
		"- BIG : the character loses his cool and starts fighting. They find food.\n"+
		"- CHILL : the character makes friends, gets some HP healed.\n"+
		"- SHARP : the character gets the others to heal them in exchange for food.\n"+
		"- SMOOTH : the character has a chance of convincing someone to join the team." )]
	public bool description;

	[Header("Balancing : Fight")]
	public int amountOfEnemies;

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private UIManager ui { get { return gm.uIManager; } }
			private GameObject stopoverSign { get { return ui.stopoverSign; } }
		private StopoverManager som { get { return gm.stopoverManager; } }
		private StatsManager stats { get { return gm.statsManager; } }

	//BASIC METHODS
	protected override void OnStart () { }

	protected override void OnUpdate () { }

	protected override void OnOnDestroy () { }

	//ENTRIES AND EXITS
	protected override void OnEnter () {
	//	StartCoroutine (Events());
	}

	protected override void OnExit () { }	

	//FIGHT
	protected override void OnVictory () { }

	protected override void OnDefeat () { }	

/* 	//EVENTS
	private IEnumerator Events() {
		ui.Log(charactersInvolved[0].name + "sees " + amountOfEnemies + " people");
		yield return new WaitForSeconds(som.roundDuration);
		ui.Log("Their motives are yet unclear.");
		yield return new WaitForSeconds(2 * som.roundDuration);

		List<Character.Stat> characterStats = new List<Character.Stat> ();
		foreach (Character character in charactersInvolved) {
			characterStats.Add(character.big);
			characterStats.Add(character.chill);
			characterStats.Add(character.sharp);
			characterStats.Add(character.smooth);
		}
		Character.Stat winner = stats.test(characterStats);

		if (winner.type == Character.StatType.BIG) {
			ui.Log(winner.owner.name + " loses their cool and attacks !");
 			Fight(new FightState(false));
		}

		if (winner.type == Character.StatType.CHILL) {
			ui.Log(winner.owner.name + " makes friends with the locals.");
			yield return new WaitForSeconds(som.roundDuration);
			ui.Log("They offer to help you heal your wounded.");

		}

	} */
}
