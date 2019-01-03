using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatsManager : MonoSingleton<StatsManager> {	
	public int Intify (float stat) {
		return (int)(stat*100);
	}

	//Test method : pick one among a selection of player stats.
	//Parameters : a list of character stats (possibly a fake one for a numeric value)	
	//Return : one of the input stats. Each stat has a chance of being chosen that is proportionate to its value
	public Character.Stat Test (List<Character.Stat> stats) { 
		string str = "testing : ";
		foreach(Character.Stat s in stats) {
			str += "\n - "+s.owner+"'s BIG = "+s.value;
		}
		//	Debug.Log(str);


		float ran = Random.value * stats.Sum(s => s.value); //Random value between 0 and sum of all input stat values

		float interval = 0;
		Character.Stat chosen = new Character.Stat();

		foreach (Character.Stat s in stats) {
			interval += s.value;
			if (ran < interval) {
				chosen = s;
				break;
			}
		}
		
		return chosen;
	}

	//Out : the degree of success for the test (a random value between 0 and the value of the chosen stat)
	public Character.Stat Test (List<Character.Stat> stats, out float bonus) { 
		Character.Stat result = Test(stats);
		bonus = Random.Range(0, result.value);
		return result;
	}
}
