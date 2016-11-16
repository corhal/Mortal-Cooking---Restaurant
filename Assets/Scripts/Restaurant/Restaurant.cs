using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Restaurant : MonoBehaviour {

	/*
	 * Запомни, дорогой читатель: если ты пытаешься инициализировать List<T> другим List<T>, но другой List<T> null,
	 * Юнити не кидает эксепшен. Он вешается и забирает твой компьютер с собой в могилу.
	 */

	public MissionStartWindow StartWindow;

	public int EnergyCostPerMission;
	public int InitialGoldReward;
	public ParticleSystem LevelUpParticles;

	public int TickPeriod;
	public int EnergyTickPeriod;
	public int EnergyPerTick;

	public System.DateTime LastTime;

	float timer;
	float energyTimer; // Я ужасный человек
	float clientTimer;
	Builder builder;

	public int[] CurrentBuildings;
	public int[] BuildingLayers;

	public GameObject FloorContainer;
	public Tile[] Tiles;

	public int Session;

	int gold;
	public int Gold { get { return gold; } }

	int starmoney;
	public int Starmoney { get { return starmoney; } }

	int energy;
	public int Energy { get { return energy; } }

	int stars;
	public int Stars { get { return stars; } }

	int raidTickets;
	public int RaidTickets { get { return raidTickets; } }

	public int MaxEnergy;
	public int Prestige;
	public int PrestigeLevel;
	public int[] PrestigeRequirementsPerLevel;
	public int[,] RangeClientsPerTickByLevel;
	public int ClientsTickPeriod;

	public int[] MaxCuisineByPrestigeLevel;
	public int DishesInCuisine;

	DishRecipe[] dishRecipes;
	public DishRecipe[] AllDishRecipes { get { return dishRecipes; } }
	//public DishRecipe[] DishRecipes;
	public DishRecipe[] DishRecipes {
		get {
			DishRecipe[] recipes = new DishRecipe[DishesInCuisine * (1 + MaxCuisineByPrestigeLevel[PrestigeLevel - 1])];
			for (int i = 0; i < recipes.Length; i++) {
				recipes [i] = dishRecipes [i]; // + DishesInCuisine * MaxCuisineByPrestigeLevel [PrestigeLevel - 1]];
			}
			return recipes;
		}
	}

	//public int[] Dishes;
	public int[] DishCosts;
	public int[] Items;
	public string[] ItemNames;
	public int[] ItemCounts;

	public List<Building> Buildings;

	public GameObject ClientPrefab;
	public List<Client> CurrentClients;

	public static Restaurant instance;

	public Chest chest;

	public delegate void RestaurantInfoChangedEventHandler ();
	public static event RestaurantInfoChangedEventHandler OnRestaurantInfoChanged;

	public delegate void MissionEndedEventHandler (int gold, IEnumerable<int> items);
	public static event MissionEndedEventHandler OnMissionEnded;

	public bool IsWindowOpen;

	public bool NotFirstTime;

	void Awake() {
		if (instance == null) {			
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);  
		}

		RangeClientsPerTickByLevel = new int[,] {
			{1, 3},
			{2, 4},
			{3, 5},
			{4, 6},
			{5, 7},
			{6, 8},
			{7, 9},
			{8, 10},
			{9, 11},
			{10, 12}
		};
	
		builder = gameObject.GetComponent<Builder> ();
		chest = gameObject.GetComponent<Chest> ();
		builder.OnBuildingBuilt += Builder_OnBuildingBuilt;
		CurrentBuildings = new int[10]; // TODO: заменить

		/*Debug.Log ("Making new dishes");
		DishRecipes = new DishRecipe[40];
		for (int i = 0; i < DishRecipes.Length; i++) {
			DishRecipes [i] = new DishRecipe (i, 1, 5, new int[5] {10, 20, 30, 40, 50}); // бывает id 0
		}*/
	}

	public bool SpendStars(int amount) {
		if (Stars >= amount) {
			stars -= amount;
			return true;
		} else {
			return false;
		}
	}

	public void InitializeFromData(RestaurantData data) {			
		NotFirstTime = data.NotFirstTime;

		if (NotFirstTime) {
			Debug.Log ("Should copy dishes from data");
			dishRecipes = new DishRecipe[data.DishRecipes.Length];
			data.DishRecipes.CopyTo (dishRecipes, 0);
		} else {
			Debug.Log ("Making new dishes");
			dishRecipes = new DishRecipe[40];
			for (int i = 0; i < dishRecipes.Length; i++) {	
				int cuisine = i / DishesInCuisine;
				dishRecipes [i] = new DishRecipe (i, 1, 5, cuisine, new int[5] {10 + cuisine * 10, 20 + cuisine * 10, 30 + cuisine * 10, 40 + cuisine * 10, 50 + cuisine * 10}); // бывает id 0
				// Debug.Log((i / DishesInCuisine).ToString());
			}
		}

		Tiles = FloorContainer.GetComponentsInChildren<Tile>();
		if (data.TileTypes.Length > 0) {
			for (int i = 0; i < data.TileTypes.Length; i++) {
				Tiles [i].TileSpriteIndex = data.TileSpriteIndexes [i];
				Tiles [i].ChangeTile (data.TileTypes [i]);
			}
		}
		data.ItemCounts.CopyTo (ItemCounts, 0);	
		Session = data.Session;
		Prestige = data.Prestige;
		PrestigeLevel = data.PrestigeLevel;
		LastTime = data.LastTime;
		energy = data.Energy;
		gold = data.Gold;
		raidTickets = data.RaidTickets;
		starmoney = data.Starmoney;
		stars = data.Stars;

		for (int i = 0; i < data.BuildingDatas.Count; i++) {
			GameObject buildingObject = Instantiate (Builder.SBuildingPrefabs[data.BuildingDatas[i].TypeId]) as GameObject; // Выглядит стремно, но пока работает верно
			buildingObject.GetComponent<Building> ().InitializeFromData (data.BuildingDatas [i]);
			Buildings.Add(buildingObject.GetComponent<Building> ());
			CurrentBuildings [data.BuildingDatas[i].TypeId]++; // не очень изящно, надо подумать, как поменять
		}

		if (data.ClientDatas.Count > 0) {
			clientTimer = 1.0f;
			for (int i = 0; i < data.ClientDatas.Count; i++) {
				GameObject clientObject = Instantiate (ClientPrefab) as GameObject; 
				clientObject.GetComponent<Client> ().InitializeFromData (data.ClientDatas [i]);
				CurrentClients.Add (clientObject.GetComponent<Client> ());
			}
		}

		System.TimeSpan ts = System.DateTime.Now.Subtract (LastTime);
		int seconds = (int)ts.TotalSeconds;

		if (NotFirstTime && seconds > 0) {	
			SkipTime (seconds);
		}

		if (Player.instance.HasWon) {
			Player.instance.HasWon = false;
			stars += Player.instance.StarCount;
			AddGold (Player.instance.GoldFromMission);
			List<int> itemsWon = new List<int> ();
			for (int i = 0; i < Player.instance.MyMission.ItemRewards.Length; i++) {
				if (Random.Range(0.0f, 1.0f) < Player.instance.MyMission.ItemChances[i]) {
					AddItem (Player.instance.MyMission.ItemRewards [i]);
					itemsWon.Add (Player.instance.MyMission.ItemRewards [i]);
				}
			}
			Debug.Log (itemsWon);
			OnMissionEnded (Player.instance.GoldFromMission, itemsWon);
		}
		OnRestaurantInfoChanged ();
		NotFirstTime = true;
	}

	void SkipTime(int seconds) {
		int ticks = seconds / ClientsTickPeriod;

		if (CurrentClients.Count == 0) {
			WelcomeClients ();
		}

		int energyTicks = seconds / EnergyTickPeriod;
		int dEnergy = energyTicks * EnergyPerTick;

		energy = Mathf.Min (energy + dEnergy, MaxEnergy);
	}

	public void Wait(int hours) {
		int seconds = hours * 60 * 60;
		starmoney += 50;
		Session++;
		SkipTime (seconds);
		OnRestaurantInfoChanged ();
	}

	public void AddGold(int amount) {
		gold += amount;
		OnRestaurantInfoChanged ();
	}

	void Builder_OnBuildingBuilt (Building building) {	
		if (SpendGold (building.Cost)) {
			building.Build();
			CurrentBuildings [building.TypeId]++;
			Buildings.Add (building);
			AddPrestige (building.Prestige);
		} else {
			Destroy (building.gameObject);
		}
	}

	public bool SpendGold(int amount) {
		if (gold >= amount) {
			gold -= amount;
			OnRestaurantInfoChanged ();
			return true;
		} else {
			return false;
		}
	}

	void Start() {
		if (Prestige == 0) {			
			Player.instance.Load ();
			InitializeFromData (Player.instance.MyRestaurant);
		}
	}		

	void Update() {		
		if ((clientTimer == 0.0f || CurrentClients.Count == 0)) {			
			WelcomeClients ();
		}
		if (energyTimer == 0.0f && energy < MaxEnergy) {				
			energy += EnergyPerTick;
		}
		timer += Time.deltaTime;
		energyTimer += Time.deltaTime;
		clientTimer += Time.deltaTime;
		if (clientTimer >= ClientsTickPeriod) {
			clientTimer = 0.0f;
		}
		if (energyTimer >= EnergyTickPeriod && energy < MaxEnergy) {	
			energyTimer = 0.0f;
		}
	}

	Vector3 RandomRestaurantPoint() {
		float x = Random.Range (-7, 2);
		x = Utility.SnapNumberToFactor (x, 0.75f);
		float y = Random.Range (-4, 2);
		y = Utility.SnapNumberToFactor (y, 0.75f);
		float z = 0;
		return new Vector3 (x, y, z);
	}

	void WelcomeClients() {
		Debug.Log ("Welcoming clients");
		for (int i = 0; i < CurrentClients.Count; i++) {
			Destroy (CurrentClients [i].gameObject);
		}
		CurrentClients.Clear ();
		int clientsCount = Random.Range (RangeClientsPerTickByLevel [PrestigeLevel - 1, 0], RangeClientsPerTickByLevel [PrestigeLevel - 1, 1] + 1);
		for (int i = 0; i <= clientsCount; i++) {
			GameObject newClientObject = Instantiate (ClientPrefab, RandomRestaurantPoint(), ClientPrefab.transform.rotation) as GameObject;
			Client newClient = newClientObject.GetComponent<Client> ();

			for (int j = 0; j < newClient.Dishes.Length; j++) {
				int index = Random.Range (0, DishRecipes.Length);
				newClient.Dishes[j] = DishRecipes [index].Id;
			}

			newClient.ItemRewards = new int[3];
			newClient.ItemChances = new float[newClient.ItemRewards.Length];
			for (int j = 0; j < newClient.ItemRewards.Length; j++) {
				int index = Random.Range (0, Items.Length);
				newClient.ItemRewards [j] = Items [index];
				newClient.ItemChances [j] = Random.Range (0.3f, 0.8f);
			}
			CurrentClients.Add (newClient);
		}
	}

	public void Build(int buildingIndex) {
		builder.DeployBuilding (buildingIndex);
	}

	int CalculateGoldForMission(MissionData mission) {
		int gold = 0; // InitialGoldReward;
		foreach (var dish in mission.Dishes) {
			gold += DishRecipes [dish].CostsByLevel [DishRecipes [dish].Level - 1];
		}
		return gold;
	}

	public void CheckoutClient(Client client) {
		CurrentClients.Remove (client);
		Destroy (client.gameObject);
	}

	public void PlayMission(MissionData mission) {
		if (SpendEnergy(EnergyCostPerMission)) {			
			mission.GoldReward = CalculateGoldForMission(mission);
			SetMission (mission);
			CheckoutClient(mission.gameObject.GetComponent<Client>());
			if (CurrentClients.Count == 0) {			
				WelcomeClients ();
			}
			ChangeScene (1);
		}
	}

	public void RaidMission(MissionData mission) {		
		if (/*Restaurant.instance.RaidTickets > 0 &&*/ Restaurant.instance.SpendEnergy (Restaurant.instance.EnergyCostPerMission)) {			
			mission.GoldReward = CalculateGoldForMission(mission);
			raidTickets--; // эта проверка сейчас происходит в клиенте =\
			AddGold (mission.GoldReward);

			stars += 3;

			List<int> itemsWon = new List<int> ();
			for (int i = 0; i < mission.ItemRewards.Length; i++) {
				if (Random.Range(0.0f, 1.0f) < mission.ItemChances[i]) {
					AddItem (mission.ItemRewards [i]);
					itemsWon.Add (mission.ItemRewards [i]);
				}
			}

			CheckoutClient(mission.gameObject.GetComponent<Client>());

			if (CurrentClients.Count == 0) {			
				WelcomeClients ();
			}

			OnMissionEnded (mission.GoldReward, itemsWon);

			OnRestaurantInfoChanged ();
		}
	}

	public void AddPrestige(int amount) {
		if (Prestige < PrestigeRequirementsPerLevel[PrestigeRequirementsPerLevel.Length - 1]) {
			Prestige += amount;
			if (PrestigeLevel < PrestigeRequirementsPerLevel.Length && Prestige >= PrestigeRequirementsPerLevel[PrestigeLevel - 1]) {
				PrestigeLevel++;
				LevelUpParticles.Play ();
			}
		}
		OnRestaurantInfoChanged ();
	}

	public void LevelUpDish(DishRecipe dishRecipe) {
		int[] currentCollection = dishRecipe.CurrentCollection ();
		foreach (var item in currentCollection) {
			ItemCounts [item]--;
		}
		dishRecipe.LevelUp ();
		OnRestaurantInfoChanged ();
	}

	public void ChangeScene(int scene) {		
		Debug.Log ("Scene changed!");
		Player.instance.Save ();
		SceneManager.LoadScene (scene);
	}

	public void Wipe() {		
		SaveLoad.Delete ();
		Destroy (Player.instance.gameObject);
		SceneManager.LoadScene (0);
	}

	public void SetMission(MissionData missionData) {
		Player.instance.SetMission(missionData);
	}

	void OnApplicationQuit() {
		Session++;
		Player.instance.Save ();
	}

	public void Quit() {
		Application.Quit ();
	}

	public bool SpendEnergy(int amount) {		
		if (energy >= amount) {
			Debug.Log ("Trying to spend " + amount + " energy, have " + energy);
			energy -= amount;
			OnRestaurantInfoChanged ();
			return true;
		} else {			
			return false;
		}
	}

	public void AddEnergy(int amount) {
		energy += amount;
		OnRestaurantInfoChanged ();
	}

	public void AddItem(int item) {
		ItemCounts [item]++;
	}

	public void OpenChest() {
		if (starmoney >= chest.OpenCost) {
			starmoney -= chest.OpenCost;
			AddItem (chest.Open ());
		}
		OnRestaurantInfoChanged ();
	}

	public void Toggle(GameObject window) {
		window.SetActive (!window.activeSelf);
		if (window.activeSelf) {
			IsWindowOpen = true;
		} else {
			IsWindowOpen = false;
		}
	}

	public void BreakItem(int item) {
		if (ItemCounts[item] > 0) {
			ItemCounts [item]--;
			raidTickets++;
			OnRestaurantInfoChanged ();
		}
	}

	public void ShowMissionStartWindow(MissionData missionData) {
		StartWindow.OpenWindow (missionData);
	}

	void OnDestroy() {
		builder.OnBuildingBuilt -= Builder_OnBuildingBuilt;
	}
}
