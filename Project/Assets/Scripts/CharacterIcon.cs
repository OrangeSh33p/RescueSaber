using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour {
	public Character character;
	public Slider hpBar;
	public Image hpBarColor;
	public Text charaName;

	public void SetHP (float value) {
		hpBar.value = value;
		hpBarColor.color = new Color (
			Mathf.Clamp(2*(1-value),0,1), 
			Mathf.Clamp(value*2,0,1), 
			0);
	}

	public void SetName (string value) {
		charaName.text = value;
	}

	public void Order () {
		GetComponent<RectTransform>().position = new Vector3(0, (95 + 50*transform.GetSiblingIndex()), 0);
	}

	public void Clicked () {
		character.Eat();
	}
}
