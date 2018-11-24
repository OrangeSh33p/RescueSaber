using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodIcon : MonoBehaviour {
	public Text foodText;

	public void SetFood (float value) {
		foodText.text = "Food : "+value;
	}
}
