using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoSingleton<FoodManager>{
	public FoodIcon foodIcon;
	public float food;
	public float minHpRegen;
	public float maxHpRegen;

	private Ray ray;
	private RaycastHit hit;
	private int characterIconLayerMask = 1 << 10;

	void Update() {
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Input.GetMouseButtonUp(0) && Physics.Raycast (ray, out hit, Mathf.Infinity, characterIconLayerMask)) {
			Debug.Log("hit "+hit.collider.name);
		}
	} 

	public void SetFood (float value) {
		foodIcon.SetFood(value);
		food = value;
	}

	public void AddFood (float value) {
		float newFood = Mathf.Max(food+value, 0);

		foodIcon.SetFood(newFood);
		food = newFood;
	}

	public bool EnoughFood (float value) {
		return food >= value;
	}
}
