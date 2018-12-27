using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoSingleton<DialogueManager> {
	public TextAsset dialogues;

	void Start () {
		Debug.Log(dialogues.text);
	}
}
