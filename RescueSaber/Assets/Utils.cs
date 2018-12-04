using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoSingleton<Utils> {
	public int Intify (float stat) {
		return (int)(stat*100);
	}
}
