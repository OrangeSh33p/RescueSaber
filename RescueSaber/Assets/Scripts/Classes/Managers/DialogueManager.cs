using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DialogueManager : MonoSingleton<DialogueManager> {
	[Header("Balancing")]
	public int stopoverColumn;//0
	public int segmentColumn;//1
	public int lineColumn;//2
	public int characterColumn;//4
	public int textColumn;//5

	[Header("References")]
	public TextAsset dialogueSource;

	[Header("State")]
	public Dictionary<int, string> parsedDialogue;

	void Start () {
		parsedDialogue = ParseOnStart(dialogueSource.ToString());
	}

	Dictionary<int,string> ParseOnStart (string source) {
		Dictionary<int, string> dic = new Dictionary<int, string>();
		foreach (string s in source.Split('\n')) {
			string currentStopover = s.Split(',')[stopoverColumn];
			if (dic.ContainsKey(currentStopover)) dic[currentStopover] += '\n'+s;
			else dic.Add(currentStopover,s);
		}
		return dic;
	}

	public Dictionary<string, Line> ParseOnStopover (int stopoverIndex) {
		string source = parsedDialogue[stopoverIndex];
		Dictionary<string, Line> dic = new Dictionary<string, Line>();
		foreach (string s in source.Split('\n')) {
			string[] tmp = s.Split(',');
			string key = tmp[segmentColumn]+','+tmp[lineColumn];
			if (dic.ContainsKey(key)) Debug.LogError ("Error : duplicate line in "+dialogueSource.name+'\n'+s);
			else dic.Add(key, new Line(tmp[characterColumn], tmp[textColumn]));
		}
		return dic;
	}
}