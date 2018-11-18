using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour {
	public Character character;
	public Slider hpBar;
	public Image hpBarColor;
	public Text charaName;
	public Text face;
	private GameManager gm { get { return GameManager.Instance; } }

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

	public void Death () {
		ChangeFace(Character.Hunger.DEAD);
	}

	public void ChangeFace (Character.Hunger state) {
		switch (state) {
			case Character.Hunger.FULL : face.text = gm.uIManager.full;
				break;
			case Character.Hunger.SATED : face.text = gm.uIManager.sated;
				break;
			case Character.Hunger.HUNGRY : face.text = gm.uIManager.hungry;
				break;
			case Character.Hunger.STARVING : face.text = gm.uIManager.starving;
				break;
			case Character.Hunger.DEAD : face.text = gm.uIManager.dead;
				break;
		}
	}

	public void Clicked () {
		character.Eat();
	}
}
