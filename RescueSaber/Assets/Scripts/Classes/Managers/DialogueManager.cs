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
	public Dictionary<string, string> parsedDialogue;

	void Start () {
		parsedDialogue = ParseOnStart(dialogueSource.ToString());
	}

	Dictionary<string,string> ParseOnStart (string source) {
		Dictionary<string, string> dic = new Dictionary<string, string>();
		foreach (string s in source.Split('\n')) {
			string key = s.Split(',')[stopoverColumn];
			if (dic.ContainsKey(key)) dic[key] += '\n'+s;
			else dic.Add(key,s);
		}
		return dic;
	}

	public Dictionary<string, Line> ParseOnStopover (string stopoverIndex) {
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