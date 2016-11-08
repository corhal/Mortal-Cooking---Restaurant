using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Restaurant : MonoBehaviour {

	public int EnergyCostPerMission;

	public ParticleSystem LevelUpParticles;
	public List<Transform> CookSpawnPoints;

	public int TickPeriod;
	public int EnergyTickPeriod;
	public int EnergyPerTick;
	public int GoldReward;

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

	public int MaxEnergy;
	public int Prestige;
	public int PrestigeLevel;
	public int[] AdditionalCookLevels;
	public int[] PrestigeRequirementsPerLevel;
	public int[] MaxGoldPerLevel;
	public int[,] RangeClientsPerTickByLevel;
	public int ClientsTickPeriod;

	public int[] Dishes;
	public int[] Items;
	public string[] ItemNames;
	public int[] ItemCounts;

	public List<Cook> Cooks;
	public List<Building> Buildings;

	public GameObject[] CookPrefabs;
	public GameObject ClientPrefab;
	public List<Client> CurrentClients;

	public static Restaurant instance;

	public Chest chest;

	public delegate void RestaurantInfoChangedEventHandler ();
	public static event RestaurantInfoChangedEventHandler OnRestaurantInfoChanged;

	public delegate void MissionEndedEventHandler (int gold, IEnumerable<int> items);
	public static event MissionEndedEventHandler OnMissionEnded;

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
		Cook.OnCollectGold += Cook_OnCollectGold;
		Client.OnClientDied += Client_OnClientDied;
	}

	void Client_OnClientDied (Client client) {
		CurrentClients.Remove (client);
		Destroy (client.gameObject);
	}

	void Cook_OnCollectGold (Cook cook) {
		int leftover = 0;
		if (MaxGoldPerLevel[PrestigeLevel - 1] < (Gold + cook.Gold)) {
			leftover = Gold + cook.Gold - MaxGoldPerLevel [PrestigeLevel - 1];
		}
		Debug.Log ("Leftover: " + leftover);
		cook.ShowFlyingText (cook.Gold - leftover);
		AddGold (cook.Gold);
		Debug.Log ("Cook had: " + cook.Gold);
		cook.Gold = leftover;
	}

	public void InitializeFromData(RestaurantData data) {	
		//ItemCounts = new int[data.ItemCounts.Length];
		Tiles = FloorContainer.GetComponentsInChildren<Tile>();
		if (data.TileTypes.Length > 0) {
			for (int i = 0; i < data.TileTypes.Length; i++) {
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
		starmoney = data.Starmoney;

		for (int i = 0; i < data.CookDatas.Count; i++) {
			GameObject cookObject = Instantiate (CookPrefabs[data.CookDatas[i].TypeId]) as GameObject;
			Cook newCook = cookObject.GetComponent<Cook> ();
			newCook.InitializeFromData (data.CookDatas [i]);
			Cooks.Add (newCook);
		}

		for (int i = 0; i < data.BuildingDatas.Count; i++) {
			GameObject buildingObject = Instantiate (Builder.SBuildingPrefabs[data.BuildingDatas[i].TypeId]) as GameObject; // Выглядит стремно, но пока работает верно
			buildingObject.GetComponent<Building> ().InitializeFromData (data.BuildingDatas [i]);
			Buildings.Add(buildingObject.GetComponent<Building> ());
			CurrentBuildings [buildingObject.GetComponent<Building> ().TypeId]++; // не очень изящно, надо подумать, как поменять
		}

		if (data.ClientDatas.Count > 0) {
			clientTimer = 1.0f;
			for (int i = 0; i < data.ClientDatas.Count; i++) {
				GameObject clientObject = Instantiate (ClientPrefab) as GameObject; 
				clientObject.GetComponent<Client> ().InitializeFromData (data.ClientDatas [i]);
				CurrentClients.Add(clientObject.GetComponent<Client> ());
			}
		} else {
			WelcomeClients ();
		}


		System.TimeSpan ts = System.DateTime.Now.Subtract (LastTime);

		if ((int)ts.TotalSeconds > 0) {		
			SkipTime ((int)ts.TotalSeconds);
		}

		while (CookSpawnPoints.Count > data.CookSpawnPointsCount) {
			CookSpawnPoints.RemoveAt(0);
		}

		if (Player.instance.HasWon) {
			Player.instance.HasWon = false;
			AddGold (Player.instance.GoldFromMission);
			List<int> itemsWon = new List<int> ();
			for (int i = 0; i < Player.instance.MyMission.ItemRewards.Length; i++) {
				if (Random.Range(0.0f, 1.0f) < Player.instance.MyMission.ItemChances[i]) {
					ItemCounts [Player.instance.MyMission.ItemRewards [i]]++;
					itemsWon.Add (Player.instance.MyMission.ItemRewards [i]);
				}
			}
			Debug.Log (itemsWon);
			OnMissionEnded (Player.instance.GoldFromMission, itemsWon);
		}
		OnRestaurantInfoChanged ();
	}

	void SkipTime(int seconds) {
		int ticks = seconds / ClientsTickPeriod;

		List<int> removeIndexes = new List<int> ();
		foreach (var client in CurrentClients) {
			if (seconds > (int)client.LifeTimeLeft) {
				removeIndexes.Add(CurrentClients.IndexOf(client));
			}
		}
		foreach (var index in removeIndexes) {
			if (CurrentClients.Count > index) {
				CurrentClients [index].Die ();
			}
		}

		if (CurrentClients.Count == 0) {
			WelcomeClients ();
		}

		int clientsCount = ((RangeClientsPerTickByLevel [PrestigeLevel - 1, 0] + RangeClientsPerTickByLevel [PrestigeLevel - 1, 1]) / 2) * ticks;
		Debug.Log (clientsCount + " clients came why you were absent");
		foreach (var cook in Cooks) {			
			cook.GenerateGoldPerClients (clientsCount, Dishes);
		}

		int energyTicks = seconds / EnergyTickPeriod;
		int dEnergy = energyTicks * EnergyPerTick;

		energy = Mathf.Min (energy + dEnergy, MaxEnergy);
	}

	public void Wait(int hours) {
		int seconds = hours * 60 * 60;
		Session++;
		SkipTime (seconds);
		OnRestaurantInfoChanged ();
	}

	public void AddGold(int amount) {
		gold = Mathf.Min (gold + amount, MaxGoldPerLevel [PrestigeLevel - 1]);
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
		if (Cooks.Count == 0) {
			AddCook ();
		}
	}
		
	int currentClient = 0;
	void Update() {		
		if (clientTimer == 0.0f) {			
			WelcomeClients ();
		}
		if (timer == 0.0f && gold < MaxGoldPerLevel[PrestigeLevel - 1] && CurrentClients.Count > 0) {
			currentClient++;
			if (currentClient >= CurrentClients.Count) {
				currentClient = 0;
			}
			GenerateGold ();
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
		if (timer >= TickPeriod && gold < MaxGoldPerLevel[PrestigeLevel - 1]) {		
			timer = 0.0f;	
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
		for (int i = 0; i < CurrentClients.Count; i++) {
			Destroy (CurrentClients [i].gameObject);
		}
		CurrentClients.Clear ();
		int clientsCount = Random.Range (RangeClientsPerTickByLevel [PrestigeLevel - 1, 0], RangeClientsPerTickByLevel [PrestigeLevel - 1, 1] + 1);
		for (int i = 0; i <= clientsCount; i++) {
			GameObject newClientObject = Instantiate (ClientPrefab, RandomRestaurantPoint(), ClientPrefab.transform.rotation) as GameObject;
			Client newClient = newClientObject.GetComponent<Client> ();
			int index = Random.Range (0, Dishes.Length);
			newClient.Dish = Dishes [index];
			newClient.ItemRewards = new int[3];
			newClient.ItemChances = new float[newClient.ItemRewards.Length];
			for (int j = 0; j < newClient.ItemRewards.Length; j++) {
				index = Random.Range (0, Items.Length);
				newClient.ItemRewards [j] = Items [index];
				newClient.ItemChances [j] = Random.Range (0.3f, 0.8f);
			}

			CurrentClients.Add (newClient);
		}
	}

	void GenerateGold() {
		//int clientsCount = Random.Range (RangeClientsPerTickByLevel [PrestigeLevel - 1, 0], RangeClientsPerTickByLevel [PrestigeLevel - 1, 1] + 1);
		//int income = 0;
		foreach (var cook in Cooks) {		
			if (currentClient >= CurrentClients.Count) {
				currentClient++;
				if (currentClient >= CurrentClients.Count) {
					currentClient = 0;
				}
			}
			if (CurrentClients.Count > 0) {
				cook.GenerateGold (CurrentClients[currentClient]);
			}
		}
		//AddGold(income);
	}

	public void Build(int buildingIndex) {
		builder.DeployBuilding (buildingIndex);
	}

	public void AddCook() {		
		Vector3 spawnPoint = CookSpawnPoints [0].position;
		CookSpawnPoints.RemoveAt (0);

		List<int> typeIds = new List<int> ();
		foreach (var cookPrefab in CookPrefabs) {
			typeIds.Add(cookPrefab.GetComponent<Cook>().TypeId);
		}

		/*foreach (var cook in Cooks) {
			typeIds.Remove (cook.TypeId);
		}*/

		int index = Random.Range (0, typeIds.Count);

		GameObject newCookObject = Instantiate (CookPrefabs [typeIds[index]], spawnPoint, CookPrefabs [typeIds[index]].transform.rotation) as GameObject;
		Cook newCook = newCookObject.GetComponent<Cook> ();

		List<int> tempItems = new List<int> ();
		foreach (var item in Items) {
			tempItems.Add (item);
		}
		for (int i = 0; i < 25; i++) {
			int randIndex = Random.Range (0, tempItems.Count);
			newCook.ItemCollections [i] = tempItems [randIndex];
			tempItems.RemoveAt (randIndex);
		}

		List<int> tempDishes = new List<int> ();
		foreach (var dish in Dishes) {
			tempDishes.Add (dish);
		}
		for (int i = 0; i < newCook.Dishes.Length; i++) {
			int randIndex = Random.Range (0, tempDishes.Count);
			newCook.Dishes [i] = tempDishes [randIndex];
			tempDishes.RemoveAt (randIndex);
		}

		Cooks.Add (newCook);
		newCook.Id = Cooks.Count;
	}

	void AddPrestige(int amount) {
		if (Prestige < PrestigeRequirementsPerLevel[PrestigeRequirementsPerLevel.Length - 1]) {
			Prestige += amount;
			if (PrestigeLevel < PrestigeRequirementsPerLevel.Length && Prestige >= PrestigeRequirementsPerLevel[PrestigeLevel - 1]) {
				PrestigeLevel++;
				LevelUpParticles.Play ();
				foreach (var level in AdditionalCookLevels) {
					if (PrestigeLevel == level) {
						AddCook ();
					}
				}
			}
		}
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
		foreach (var cook in Cooks) {
			cook.CheckItems (ItemCounts);
		}
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
	}

	void OnDestroy() {
		builder.OnBuildingBuilt -= Builder_OnBuildingBuilt;
		Cook.OnCollectGold -= Cook_OnCollectGold;
		Client.OnClientDied -= Client_OnClientDied;
	}
}
