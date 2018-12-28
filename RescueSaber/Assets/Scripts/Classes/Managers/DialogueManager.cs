using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DialogueManager : MonoSingleton<DialogueManager> {
	public TextAsset dialogueSource;
	public List<string> parsedDialogue;

	void Start () {
		parsedDialogue = new List<string>();
		List<string> tmp = dialogueSource.ToString().Split('\n').ToList();

		int previousStopover = 0;
		for (int i=0;i<tmp.Count;i++) {
			int currentStopover;
			if (int.TryParse(tmp[i].Split(',')[0], out currentStopover)) {
				if (currentStopover == previousStopover) parsedDialogue[currentStopover-1] += tmp[i] + "\n";
				else if (currentStopover == previousStopover + 1) {
					parsedDialogue.Add(tmp[i] + "\n");
					previousStopover ++;
				}
				else Debug.LogError("Error while parsing "+dialogueSource.name+" : field \"stopover\" of line "+i+
					" doesn't have expected value of "+currentStopover+" or "+(currentStopover+1));
			}
			else Debug.LogError("Error while parsing "+dialogueSource.name+" : can't read field \"stopover\" of line "+i);
		}
	}
}
