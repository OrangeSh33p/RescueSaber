using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	//A few useful unicode characters

	//♒︎ ♑︎ ♓︎ ♐︎ ♏︎ ♎︎ ♍︎ ♌︎ ♋︎ ♊︎ ♉︎ ♈︎	  astrological signs (appear bigger)
	//☉ ☽ ☿ ♀︎ ⊕ ♁ ♂︎ ⚳ ♃ ♄ ♅ ⛢ ♆ ♇		 planets
	//🜁 🜃 🜂 🜄							 4 elements
	//🜔 🜍 🜺 🜘 = ⊛ 🜏 🜹 🜅 🜆 🜈 🝛 🜓 🜖	alchemy
	//🝮 🝲 ℥ 🝳 ℈ ℔					   measurement units
	//⚕ ⚵ ⚴ ⚶ ⚷ ☄︎ ⊗ ⚸ ☊ ☋				 astronomy

	[Header("Balancing")]
	public float minTimeBetweenLogs;

	[Header("Balancing : CharaIcon")]
	public string fullFace;
	public string satedFace;
	public string hungryFace;
	public string starvingFace;
	public string deadFace;

	[Header("Balancing : Stats")]
	public string big;
	public string chill;
	public string sharp;
	public string smooth;

	[Header("References")]
	public GameObject stopoverSign;
	public Text stopoverText;
	public Transform console;
	public GameObject logPrefab;

	//State
	private float timeSinceLastLog;
	private List<string> standbyLogs = new List<string>();
	private List<string> permanentStandbyLogs = new List<string>();

	void Update () {
		CheckStandbyLogs();
	}

	//LOGS
	void CheckStandbyLogs () {
		timeSinceLastLog += Time.deltaTime;

		if (timeSinceLastLog > minTimeBetweenLogs) { //If it is time to send a new log
			if (standbyLogs.Count > 0) { //If there is a standby log, send it
				Log(standbyLogs[0]);
				standbyLogs.RemoveAt(0);
			}

			else if (permanentStandbyLogs.Count > 0) { //If there is a permanent standby log, send it
				LogPermanent(permanentStandbyLogs[0]);
				permanentStandbyLogs.RemoveAt(0);
			}
		}

	}

	LogText CreateLogText (string logText) {
			GameObject log = Instantiate (
				logPrefab, 
				console.position, 
				Quaternion.identity, 
				console
				);
			
			log.GetComponent<Text>().text = logText;
			timeSinceLastLog = 0;

			return log.GetComponent<LogText>();
		
	}

	public void Log (string logText) {
		if (timeSinceLastLog > minTimeBetweenLogs) CreateLogText(logText);
		else standbyLogs.Add(logText);
	}

	public void LogPermanent (string logText) {
		if (timeSinceLastLog > minTimeBetweenLogs) CreateLogText(logText).MakePermanent();
		else permanentStandbyLogs.Add(logText);
	}
}
