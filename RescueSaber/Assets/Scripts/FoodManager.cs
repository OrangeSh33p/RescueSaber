using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoSingleton<FoodManager> {
	[Header("Balancing")]
	public float minHpRegen;
	public float maxHpRegen;
	public int startFood;

	[Header("References")]
	public FoodIcon foodIcon;

	//State
	private int food;

	void Start () {
		SetFood(startFood);
	}

	public void SetFood (int value) {
		foodIcon.SetFood(value);
		food = value;
	}

	public void AddFood (int value) {
		SetFood(Mathf.Max(food + value, 0));
	}

	public void RemoveFood (int value) {
		SetFood(Mathf.Max(food - value, 0));
	}

	public bool EnoughFood (float value) {
		return food >= value;
	}
}
