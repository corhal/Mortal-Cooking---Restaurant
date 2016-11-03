using UnityEngine;
using System.Collections;

[System.Serializable]
public class CookData {

	public float x;
	public float y;
	public float z;

	public int Id;
	public int[] Dishes;
	public int Level;
	public int Soulstones;
	public bool RaidReady;

	public void InitializeFromCook(Cook cook) {
		x = cook.transform.position.x;
		y = cook.transform.position.y;
		z = cook.transform.position.z;

		Id = cook.Id;
		Level = cook.Level;
		Soulstones = cook.Soulstones;
		RaidReady = cook.RaidReady;
		Dishes = new int[cook.Dishes.Length];
		cook.Dishes.CopyTo (Dishes, 0);
	}
}
