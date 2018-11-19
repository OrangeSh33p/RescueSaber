using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public GameObject stopoverSign;
	public Text stopoverText;
	public Transform console;
	public GameObject logPrefab;
	public string full;
	public string sated;
	public string hungry;
	public string starving;
	public string dead;

	private float timeSinceLastLog;
	private List<string> standbyLogs = new List<string>();
	private List<string> permanentStandbyLogs = new List<string>();

	void Update () {
		timeSinceLastLog += Time.deltaTime;
		if (timeSinceLastLog > 1 && standbyLogs.Count > 0) {
			Log(standbyLogs[0]);
			standbyLogs.RemoveAt(0);
		}

		if (timeSinceLastLog > 1 && permanentStandbyLogs.Count > 0) {
			LogPermanent(permanentStandbyLogs[0]);
			permanentStandbyLogs.RemoveAt(0);
		}
	}

	public Text Log (string logText) {
		if (timeSinceLastLog > 1) {
			Text log = Instantiate (
				logPrefab, 
				console.position, 
				Quaternion.identity, 
				console
				).GetComponent<Text>();
			
			log.text = logText;
			timeSinceLastLog = 0;
			return log;

		} else standbyLogs.Add(logText);

		return null;
	}

	public void LogPermanent (string logText) {

		if (timeSinceLastLog > 1) Log(logText).GetComponent<LogText>().permanent = true;
		else permanentStandbyLogs.Add(logText);
	}
}
