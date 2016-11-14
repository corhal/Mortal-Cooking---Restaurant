using UnityEngine;
using System.Collections;

public class GhostClient {

	//public Cook MyCook;

	public int[] Dishes;

	public float MaxLifeTime;
	public float LifeTimeLeft;

	//public int Gold;

	public int Readiness;
	public int MaxReadiness;

	public GhostClient(/*Cook myCook,*/ int[] dishes, float maxLifeTime, /*int gold,*/ int maxReadiness) {
		//MyCook = myCook;
		Dishes = new int[dishes.Length];
		dishes.CopyTo (Dishes, 0);
		MaxLifeTime = maxLifeTime;
		LifeTimeLeft = MaxLifeTime;
		//Gold = gold;
		MaxReadiness = maxReadiness;
		Readiness = 0;
	}
}
