using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour 
{
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.LeftArrow))
			transform.position += new Vector3 (-1, 0, 0);
		
		if (Input.GetKeyDown (KeyCode.RightArrow))
			transform.position += new Vector3 (1, 0, 0);
		
		if (Input.GetKeyDown (KeyCode.UpArrow))
			transform.position += new Vector3 (0, 0, 1);
		
		if (Input.GetKeyDown (KeyCode.DownArrow))
			transform.position += new Vector3 (0, 0, -1);



		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100))
			{
				Vector3 tile = hit.collider.transform.position;
				Vector3 pos = transform.position;

				float xDist = Mathf.Abs(tile.x - pos.x);
				float zDist = Mathf.Abs(tile.z - pos.z);

				if (xDist != 0 || zDist != 0)
				{
					if (Random.Range(0, xDist + zDist) < xDist)
						transform.position += new Vector3 (Mathf.Sign(tile.x - pos.x), 0, 0);
					else
						transform.position += new Vector3 (0, 0, Mathf.Sign(tile.z - pos.z));
				}
			}
		}
	}


}
