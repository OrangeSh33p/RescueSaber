using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopover_Template /* copy from here */ : Stopover {
	[Space (25)]
	[Header ("Stopover 0 : A template")]
	[Tooltip(
		"Stopover 0 : A template\n\n"+
		"This stopover is a template that you can use." )]
	public bool description;

	//BASIC METHODS
	protected override void OnStart () { }

	protected override void OnUpdate () { }

	protected override void OnOnDestroy () { }

	//ENTRIES AND EXITS
	protected override void OnEnter () { }

	protected override void OnExit () { }	

	//FIGHT
	protected override void OnVictory () { }

	protected override void OnDefeat () { }	
} /* to here */
