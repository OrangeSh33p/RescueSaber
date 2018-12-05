using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatsManager : MonoSingleton<StatsManager> {
	float bonus;
	
	public int Intify (float stat) {
		return (int)(stat*100);
	}

	public Character.Stat test (List<Character.Stat> stats, out float bonus) {	
		bonus = 0;	
		float ran = Random.value * stats.Sum(s => s.value); //Random value between 0 and sum of all input stat values

		float interval = 0;
		Character.Stat chosen = new Character.Stat();

		foreach (Character.Stat s in stats) {
			interval += s.value;
			if (ran < interval) {
				chosen = s;
				bonus = (interval - ran);
				break;
			}
		}
		
		return chosen;
	}
}
