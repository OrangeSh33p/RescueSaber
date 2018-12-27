using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopoverManager : MonoSingleton<StopoverManager> {
	[Header("Balancing : Warning Sign")]
	public float distanceToShowText;
	public float distanceToShowSecondText;
	public float distanceToHideText;

	[Header("Balancing : Fight")]
	public float roundDuration;
	public int defaultAmountOfEnemies;
	public float defaultEnemyBig;
	public float defaultMinDamage;
	public float defaultMaxDamage;
}
