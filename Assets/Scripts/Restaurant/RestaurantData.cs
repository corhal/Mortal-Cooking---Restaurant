using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class RestaurantData {

	public System.DateTime LastTime;
	public int Gold;
	public int Starmoney;
	public int Prestige;
	public int PrestigeLevel;
	public int[] CurrentBuildings;
	public int Session;

	public int Energy;

	public int[] ItemCounts;
	public int[] TileTypes;
	public int CookSpawnPointsCount = 5;

	public List<CookData> CookDatas;
	public List<BuildingData> BuildingDatas;
	public List<ClientData> ClientDatas;

	public bool NeedsWipe;

	public RestaurantData(int gold, int prestige, int prestigeLevel) {
		Gold = gold;
		Prestige = prestige;
		PrestigeLevel = prestigeLevel;

		CookDatas = new List<CookData> ();
		BuildingDatas = new List<BuildingData> ();
	}

	public void InitializeFromRestaurant(Restaurant restaurant) {
		TileTypes = new int[restaurant.Tiles.Length];
		for (int i = 0; i < restaurant.Tiles.Length; i++) {
			TileTypes [i] = restaurant.Tiles [i].TileType;
		}
		Starmoney = restaurant.Starmoney;	
		ItemCounts = new int[restaurant.ItemCounts.Length];
		restaurant.ItemCounts.CopyTo (ItemCounts, 0);
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
		ClientDatas.Clear ();

		for (int i = 0; i < restaurant.Cooks.Count; i++) {
			CookDatas.Add (new CookData ());
			CookDatas [i].InitializeFromCook (restaurant.Cooks [i]);
		}

		for (int i = 0; i < restaurant.Buildings.Count; i++) {
			BuildingDatas.Add (new BuildingData ());
			BuildingDatas [i].InitializeFromBuilding (restaurant.Buildings [i]);
		}

		for (int i = 0; i < restaurant.CurrentClients.Count; i++) {
			ClientDatas.Add (new ClientData ());
			ClientDatas [i].InitializeFromClient (restaurant.CurrentClients [i]);
		}

		Debug.Log ("Client datas when restaurant saves: " + ClientDatas.Count);
	}
}
