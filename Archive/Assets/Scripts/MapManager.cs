using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public GameObject tile;
	public GameObject tileHolder;

	void Start () 
	{
		for (float i = 0; i <= 20; i++)
		{
			for (float j = 0; j <= 20; j++)
			{
				Instantiate (tile, new Vector3(i, 0.1f, j), transform.rotation, tileHolder.transform);
			}
		}

	}
}
