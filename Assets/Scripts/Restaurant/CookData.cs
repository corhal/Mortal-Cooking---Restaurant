using UnityEngine;
using System.Collections;

[System.Serializable]
public class CookData {

	public float x;
	public float y;
	public float z;

	public int TypeId;
	public int Id;
	public int[] Dishes;
	public int Level;
	public int GoldStorageLevel;
	public int Soulstones;
	public bool RaidReady;
	public int Gold;

	public int[] ItemCollections;

	public void InitializeFromCook(Cook cook) {
		x = cook.transform.position.x;
		y = cook.transform.position.y;
		z = cook.transform.position.z;

		ItemCollections = new int[cook.ItemCollections.Length];
		cook.ItemCollections.CopyTo (ItemCollections, 0);
		GoldStorageLevel = cook.GoldStorageLevel;
		Gold = cook.Gold;
		TypeId = cook.TypeId;
		Id = cook.Id;
		Level = cook.Level;
		//Soulstones = cook.Soulstones;
		RaidReady = cook.RaidReady;
		Dishes = new int[cook.Dishes.Length];
		cook.Dishes.CopyTo (Dishes, 0);
	}
}
