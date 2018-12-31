using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line {
	public string character;
	public string text;

	public Line (string character, string text) {
		this.character = character;
		this.text = text;
	}
}
