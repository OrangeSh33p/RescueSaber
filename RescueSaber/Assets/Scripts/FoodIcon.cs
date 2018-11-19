using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodIcon : MonoBehaviour {
	public Text foodText;
	private GameManager gm { get { return GameManager.Instance; } }

	public void SetFood (float value) {
		foodText.text = "Food : "+value;
	}
}
