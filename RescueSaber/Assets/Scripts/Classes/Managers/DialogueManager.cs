using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DialogueManager : MonoSingleton<DialogueManager> {
	public TextAsset dialogueSource;
	[SerializeField]
	public Dictionary<int, string> parsedDialogue;

	void Start () {
		ParseOnStart(dialogueSource.ToString());
	}

	Dictionary<int,string> ParseOnStart (string source) {
		parsedDialogue = new Dictionary<int, string>();
		string[] tmp = source.Split('\n');
		for (int i=1;i<tmp.Length;i++) { //Skip line 0
			string s = tmp[i];
			int currentStopover;
			if (int.TryParse(s.Split(',')[0], out currentStopover)) {
				if (parsedDialogue.ContainsKey(currentStopover)) parsedDialogue[currentStopover] += '\n'+s;
				else parsedDialogue.Add(currentStopover,s);
			}
			else Debug.LogError("Error while parsing "+dialogueSource.name+" : can't read field \"stopover\" of line "+(i+1));
		}
		return parsedDialogue;
	}

	public Dictionary<string, Line> ParseOnStopover (int stopoverIndex) {
		string source = parsedDialogue[stopoverIndex];
		Dictionary<string, Line> dic = new Dictionary<string, Line>();
		string[] tmp = source.Split('\n');
		for (int i=0;i<tmp.Length;i++) {
			string s = tmp[i];
			string[] tmp2 = s.Split(',');
			int currentSegment;
			int currentLine;
			if (int.TryParse(tmp2[1], out currentSegment) && int.TryParse(tmp2[2], out currentLine)) {
				string key = currentSegment.ToString()+','+currentLine.ToString();
				if (dic.ContainsKey(key)) Debug.LogError ("Error : duplicate line in "+dialogueSource.name+'\n'+s);
				else dic.Add(key, new Line(tmp2[4], tmp2[5]));
			}
			else Debug.LogError("Error while parsing "+dialogueSource.name+" at line : \n"+s);
		}
		return dic;
	}
}