using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopover_Template 
// --------------------✂--------------------
: Stopover {
	[Space (25)]
	[Header ("Stopover 0 : A template")]
	[Tooltip(
		"Stopover 0 : A template\n\n"+
		"This stopover is a template that you can use. "+
		"To use it, carefully cut the code along the lines." )]
	public bool description;


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
		//if (round == 1) doThing ();
		//if (round == 2) doOtherThing();
	}


	//--------------------
	// FIGHT
	//--------------------

	protected override void OnVictory () { }

	protected override void OnDefeat () { }	
}
// --------------------✂--------------------