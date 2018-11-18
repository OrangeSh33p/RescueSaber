using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager> {
	private Ray ray;
	private RaycastHit hit;

	RaycastHit GetMouseOver (int layerMask) {
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Input.GetMouseButtonUp(0) && Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask))
			return hit;
		return hit;
	}
}
