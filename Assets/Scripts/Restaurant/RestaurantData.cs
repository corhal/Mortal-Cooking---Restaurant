using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class RestaurantData {

	public System.DateTime LastTime;
	public int Gold;
	public int Prestige;
	public int PrestigeLevel;
	public int[] CurrentBuildings;
	public int Session;

	public int Energy;

	public int CookSpawnPointsCount = 5;

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
		Session = restaurant.Session;	
		CurrentBuildings = restaurant.CurrentBuildings;
		Energy = restaurant.Energy;
		CookSpawnPointsCount = restaurant.CookSpawnPoints.Count;
		LastTime = System.DateTime.Now;
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
