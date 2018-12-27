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
	public Image icon;
	public Text charaName;
	public Text stats;
	
	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private UIManager ui { get { return gm.uIManager; } }
		private StatsManager sm { get { return gm.statsManager; } }

	public void SetHP (float value) {
		hpBar.value = value;
		hpBarColor.color = new Color ( //HP bar color ranges from green (full hp) to red (no hp), always at full saturation
			Mathf.Clamp01(2*(1-value)), 
			Mathf.Clamp01(value*2), 
			0
		);
	}

	public void SetName (string value) {
		charaName.text = value;
	}

	public void SetStats (float big, float chill, float sharp, float smooth) {
		stats.text = (
			ui.bigIcon + sm.Intify(big) + "  "
			+ ui.chillIcon + sm.Intify(chill) + "  "
			+ ui.sharpIcon + sm.Intify(sharp) + "  "
			+ ui.smoothIcon + sm.Intify(smooth)
		);
	}

	public void SetFace (Character.Hunger state) {
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

	public void Death () {
		SetFace(Character.Hunger.DEAD);
	}

	public void Clicked () {
		character.Eat();
	}
}
