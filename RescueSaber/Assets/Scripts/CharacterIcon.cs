using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour {
	[Header("References")]
	public Character character;
	public Slider hpBar;
	public Image hpBarColor;
	public Text face;
	public Text charaName;
	
	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private UIManager ui { get { return gm.uIManager; } }

	public void SetHP (float value) {
		hpBar.value = value;
		hpBarColor.color = new Color ( //HP bar color ranges from green (full hp) to red (no hp), always at full saturation
			Mathf.Clamp01(2*(1-value)), 
			Mathf.Clamp01(value*2), 
			0);
	}

	public void SetName (string value) {
		charaName.text = value;
	}

	public void Death () {
		ChangeFace(Character.Hunger.DEAD);
	}

	public void ChangeFace (Character.Hunger state) {
		switch (state) {
			case Character.Hunger.FULL : face.text = ui.fullFace;
				break;
			case Character.Hunger.SATED : face.text = ui.satedFace;
				break;
			case Character.Hunger.HUNGRY : face.text = ui.hungryFace;
				break;
			case Character.Hunger.STARVING : face.text = ui.starvingFace;
				break;
			case Character.Hunger.DEAD : face.text = ui.deadFace;
				break;
		}
	}

	public void Clicked () {
		character.Eat();
	}
}
