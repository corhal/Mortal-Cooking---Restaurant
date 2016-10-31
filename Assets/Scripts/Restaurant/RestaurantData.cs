using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class RestaurantData {

	public System.DateTime LastTime;
	public int BuildingCost;
	public int Gold;
	public int Prestige;
	public int PrestigeLevel;

	public List<CookData> CookDatas;
	public List<BuildingData> BuildingDatas;

	public bool NeedsWipe;

	public RestaurantData(int gold, int prestige, int prestigeLevel) {
		Gold = gold;
		Prestige = prestige;
		PrestigeLevel = prestigeLevel;

		CookDatas = new List<CookData> ();
		BuildingDatas = new List<BuildingData> ();
	}

	public void InitializeFromRestaurant(Restaurant restaurant) {
		Debug.Log ("Saving restaurant...");
		BuildingCost = restaurant.BuildingCost;
		LastTime = System.DateTime.Now;
		NeedsWipe = restaurant.NeedsWipe;
		Gold = restaurant.Gold;
		Prestige = restaurant.Prestige;
		PrestigeLevel = restaurant.PrestigeLevel;

		CookDatas.Clear ();
		BuildingDatas.Clear ();

		for (int i = 0; i < restaurant.Cooks.Count; i++) {
			CookDatas.Add (new CookData ());
			CookDatas [i].InitializeFromCook (restaurant.Cooks [i]);
		}

		for (int i = 0; i < restaurant.Buildings.Count; i++) {
			BuildingDatas.Add (new BuildingData ());
			BuildingDatas [i].InitializeFromBuilding (restaurant.Buildings [i]);
		}
	}
}
