using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class RestaurantData {

	public System.DateTime LastTime;
	public int Gold;
	public int Stars;
	public int Starmoney;
	public int RaidTickets;
	public int Prestige;
	public int PrestigeLevel;
	public int[] CurrentBuildings;
	public int Session;
	public bool NotFirstTime;

	public int Energy;

	public DishRecipe[] DishRecipes;
	public int[] ItemCounts;
	public int[] TileTypes;
	public int[] TileSpriteIndexes;
	public int CookSpawnPointsCount = 5;

	public List<BuildingData> BuildingDatas;
	public List<ClientData> ClientDatas;

	public bool NeedsWipe;

	public RestaurantData(int gold, int prestige, int prestigeLevel) {
		Gold = gold;
		Prestige = prestige;
		PrestigeLevel = prestigeLevel;

		BuildingDatas = new List<BuildingData> ();
	}

	public void InitializeFromRestaurant(Restaurant restaurant) {
		NotFirstTime = restaurant.NotFirstTime;

		DishRecipes = new DishRecipe[restaurant.DishRecipes.Length];
		restaurant.DishRecipes.CopyTo (DishRecipes, 0);

		TileTypes = new int[restaurant.Tiles.Length];
		TileSpriteIndexes = new int[restaurant.Tiles.Length];
		for (int i = 0; i < restaurant.Tiles.Length; i++) {
			TileSpriteIndexes [i] = restaurant.Tiles [i].TileSpriteIndex;
			TileTypes [i] = restaurant.Tiles [i].TileType;
		}
		Stars = restaurant.Stars;
		Starmoney = restaurant.Starmoney;	
		ItemCounts = new int[restaurant.ItemCounts.Length];
		restaurant.ItemCounts.CopyTo (ItemCounts, 0);
		Session = restaurant.Session;	
		CurrentBuildings = restaurant.CurrentBuildings;
		Energy = restaurant.Energy;
		RaidTickets = restaurant.RaidTickets;
		LastTime = System.DateTime.Now;
		Gold = restaurant.Gold;
		Prestige = restaurant.Prestige;
		PrestigeLevel = restaurant.PrestigeLevel;

		BuildingDatas.Clear ();
		ClientDatas.Clear ();

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
