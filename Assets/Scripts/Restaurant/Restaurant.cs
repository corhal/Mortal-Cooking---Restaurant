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
	Builder builder;

	public int[] CurrentBuildings;
	public int[] BuildingLayers;

	public int Session;

	int gold;
	public int Gold { get { return gold; } }

	int energy;
	public int Energy { get { return energy; } }

	public int MaxEnergy;
	public int Prestige;
	public int PrestigeLevel;
	public int[] AdditionalCookLevels;
	public int[] PrestigeRequirementsPerLevel;
	public int[] MaxGoldPerLevel;
	public int[,] RangeClientsPerSecondByLevel;

	public List<Cook> Cooks;
	public List<Building> Buildings;

	public GameObject CookPrefab;

	public static Restaurant instance;

	public delegate void RestaurantInfoChangedEventHandler ();
	public static event RestaurantInfoChangedEventHandler OnRestaurantInfoChanged;

	void Awake() {
		if (instance == null) {			
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);  
		}

		RangeClientsPerSecondByLevel = new int[,] {
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
		builder.OnBuildingBuilt += Builder_OnBuildingBuilt;
		CurrentBuildings = new int[10]; // TODO: заменить

	}

	public void InitializeFromData(RestaurantData data) {		
		Session = data.Session;
		Prestige = data.Prestige;
		PrestigeLevel = data.PrestigeLevel;
		LastTime = data.LastTime;
		energy = data.Energy;
		gold = data.Gold;

		for (int i = 0; i < data.CookDatas.Count; i++) {
			GameObject cookObject = Instantiate (CookPrefab) as GameObject;
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

		System.TimeSpan ts = System.DateTime.Now.Subtract (LastTime);

		if ((int)ts.TotalSeconds > 0) {		
			AddGold (SkipTime ((int)ts.TotalSeconds) [0]);		
			energy = Mathf.Min (energy + SkipTime ((int)ts.TotalSeconds) [1], MaxEnergy);
		}

		while (CookSpawnPoints.Count > data.CookSpawnPointsCount) {
			CookSpawnPoints.RemoveAt(0);
		}

		AddGold (Player.instance.GoldFromMission);
	}

	int[] SkipTime(int seconds) {
		int ticks = seconds / TickPeriod;
		int clientsCount = ((RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0] + RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1]) / 2) * ticks;
		int income = 0;
		foreach (var cook in Cooks) {
			income += cook.GenerateGoldPerClients (clientsCount);
		}

		int energyTicks = seconds / EnergyTickPeriod;
		int dEnergy = energyTicks * EnergyPerTick;

		int[] result = new int[2];
		result [0] = income;
		result [1] = dEnergy;
		return result;
	}

	public void Wait(int hours) {
		int seconds = hours * 60 * 60;
		Session++;
		AddGold (gold + SkipTime (seconds) [0]);
		energy = Mathf.Min (energy + SkipTime (seconds) [1], MaxEnergy);
		OnRestaurantInfoChanged ();
	}

	public void AddGold(int amount) {
		gold = Mathf.Min (gold + amount, MaxGoldPerLevel [PrestigeLevel - 1]);
		OnRestaurantInfoChanged ();
	}

	void Builder_OnBuildingBuilt (Building building) {	
		if (gold >= building.Cost) {
			gold -= building.Cost;	
			building.Build();
			CurrentBuildings [building.TypeId]++;
			Buildings.Add (building);
			AddPrestige (building.Prestige);
		} else {
			Destroy (building.gameObject);
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
		
	void Update() {		
		timer += Time.deltaTime;
		energyTimer += Time.deltaTime;
		if (timer >= TickPeriod && gold < MaxGoldPerLevel[PrestigeLevel - 1]) {
			timer = 0.0f;
			GenerateGold ();
		}
		if (energyTimer >= EnergyTickPeriod && energy < MaxEnergy) {
			energyTimer = 0.0f;
			energy += EnergyPerTick;
		}
		float timeToEnergy = EnergyTickPeriod - energyTimer;
		int minutes = (int)timeToEnergy / 60;
		int seconds = (int)timeToEnergy - minutes * 60;
		string time = minutes + ":" + seconds;
	}

	void GenerateGold() {
		int clientsCount = Random.Range (RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0], RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1] + 1);
		int income = 0;
		foreach (var cook in Cooks) {
			for (int i = 0; i < clientsCount; i++) {
				income += cook.GenerateGold ();
			}
		}
		AddGold(income);
	}

	public void Build(int buildingIndex) {
		builder.DeployBuilding (buildingIndex);
	}

	public void AddCook() {		
		Vector3 spawnPoint = CookSpawnPoints [0].position;
		CookSpawnPoints.RemoveAt (0);
		GameObject newCookObject = Instantiate (CookPrefab, spawnPoint, CookPrefab.transform.rotation) as GameObject;
		Cook newCook = newCookObject.GetComponent<Cook> ();
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

	public void Toggle(GameObject window) {
		window.SetActive (!window.activeSelf);
	}

	void OnDestroy() {
		builder.OnBuildingBuilt -= Builder_OnBuildingBuilt;
	}
}
