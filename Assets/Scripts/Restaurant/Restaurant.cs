using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Restaurant : MonoBehaviour {

	public int EnergyCostPerMission;

	public ParticleSystem LevelUpParticles;
	public GameObject InfoPanel;

	public List<Transform> CookSpawnPoints;

	public int TickPeriod;
	public int EnergyTickPeriod;
	public int EnergyPerTick;
	public int GoldReward;

	public GameObject BuildButton;
	public System.DateTime LastTime;

	float timer;
	float energyTimer; // Я ужасный человек
	Builder builder;
	Cook selectedCook;

	public GameObject BuildingPrefab;

	public bool NeedsWipe;

	//public int BuildingCost;

	public int[] Costs;
	public int[] PrestigeRewards;
	public int[,] BuildingLimitsByBuildingsByLevels;
	public int[] CurrentBuildings;
	public int[] BuildingLayers;

	public int Session;

	public int Gold;
	public int Energy;
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
	public Text InfoLabel;
	public Text PrestigeLevelLabel;
	public Text PrestigeLabel;
	public Text ClientsLabel;
	public Slider PrestigeSlider;
	public Slider GoldSlider;
	public Slider EnergySlider;
	public Text GoldText;
	public Text EnergyText;

	public Text SessionLabel;

	public Text EnergyTipText;

	public static Restaurant instance;

	void Awake() {
		if (instance == null) {
			//if not, set instance to this
			instance = this;
		}

		//If instance already exists and it's not this:
		else if (instance != this) {

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
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

		BuildingLimitsByBuildingsByLevels = new int[,] {
			{2, 4, 5, 6, 6, 6, 6, 6, 6, 6}, // в строках декоры, в столбцах уровни
			{0, 2, 3, 3, 3, 3, 3, 3, 3, 3},
			{0, 1, 3, 4, 4, 4, 4, 4, 4, 4},
			{0, 0, 3, 5, 5, 5, 5, 5, 5, 5},
			{0, 0, 0, 3, 5, 5, 5, 5, 5, 5},
			{0, 0, 0, 0, 2, 3, 3, 3, 3, 3},
			{0, 0, 0, 0, 0, 3, 4, 4, 4, 4},
			{0, 0, 0, 0, 0, 0, 3, 5, 5, 5},
			{0, 0, 0, 0, 0, 0, 0, 2, 4, 4},
			{0, 0, 0, 0, 0, 0, 0, 0, 2, 3},
		};
		builder = gameObject.GetComponent<Builder> ();
		builder.OnBuildingBuilt += Builder_OnBuildingBuilt;
		Cook.OnCookClicked += Cook_OnCookClicked;
		CurrentBuildings = new int[BuildingLimitsByBuildingsByLevels.Length];

	}

	public void InitializeFromData(RestaurantData data) {
		//Debug.Log ("Building cost in data: " + data.BuildingCost);
		Session = data.Session;

		if (Session == 0) {
			Session = 1;
		}
		Debug.Log (CurrentBuildings.Length);
		if (data.CurrentBuildings != null) {
			CurrentBuildings = data.CurrentBuildings; // Не просто грязный, а эпически грязный хак
		}
		Debug.Log (CurrentBuildings.Length);

		/*if (data.BuildingCost != 0) { // Мне так, так стыдно
			BuildingCost = data.BuildingCost;
			BuildButton.GetComponentInChildren<Text> ().text = "Build for " + BuildingCost;
		}*/

		Prestige = data.Prestige;
		PrestigeLevel = data.PrestigeLevel;

		if (PrestigeLevel == 0) { // Мне стыдно
			PrestigeLevel = 1;
		}

		for (int i = 0; i < Cooks.Count; i++) {
			Destroy (Cooks [i].gameObject);
		}
		Cooks.Clear ();
		for (int i = 0; i < Buildings.Count; i++) {
			Destroy (Buildings [i].gameObject);
		}
		Buildings.Clear ();

		for (int i = 0; i < data.CookDatas.Count; i++) {
			GameObject cookObject = Instantiate (CookPrefab) as GameObject;
			Cook newCook = cookObject.GetComponent<Cook> ();
			newCook.InitializeFromData (data.CookDatas [i]);
			if (newCook.Id == Player.instance.currentCookId) {
				if (Player.instance.StarCount == 3) {
					newCook.RaidReady = true;
				}
				newCook.AddSoulstones (Player.instance.Soulstones);
				//Debug.Log ("Player has " + Player.instance.GoldFromMission);
			}
			Cooks.Add (newCook);
		}

		for (int i = 0; i < data.BuildingDatas.Count; i++) {
			GameObject buildingObject = Instantiate (BuildingPrefab) as GameObject;
			buildingObject.GetComponent<Building> ().InitializeFromData (data.BuildingDatas [i]);
			Buildings.Add(buildingObject.GetComponent<Building> ());
		}

		LastTime = data.LastTime;

		System.TimeSpan ts = System.DateTime.Now.Subtract (LastTime);

		if ((int)ts.TotalSeconds > 0) {		

			Gold = Mathf.Min (data.Gold + SkipTime ((int)ts.TotalSeconds) [0], MaxGoldPerLevel [PrestigeLevel - 1]);
			Energy = Mathf.Min (data.Energy + SkipTime ((int)ts.TotalSeconds) [1], MaxEnergy);
		}

		AddGold (Player.instance.GoldFromMission);

		if (Cooks.Count != 0) { // и вот таким жалким способом я проверяю, не первый ли это раз
			while (CookSpawnPoints.Count > data.CookSpawnPointsCount) {
				CookSpawnPoints.RemoveAt(0);
			}
		}
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
		//Gold = Mathf.Min (Gold + SkipTime (seconds) [0], MaxGoldPerLevel [PrestigeLevel - 1]);
		AddGold (Gold + SkipTime (seconds) [0]);
		Energy = Mathf.Min (Energy + SkipTime (seconds) [1], MaxEnergy);
		UpdateInfoLabel ();
	}

	public void AddGold(int amount) {
		Gold = Mathf.Min (Gold + amount, MaxGoldPerLevel [PrestigeLevel - 1]);
	}

	void Cook_OnCookClicked (Cook cook) {
		selectedCook = cook;
		selectedCook.ToggleSelect ();
	}

	void Builder_OnBuildingBuilt (Building building) {
		Gold -= Costs[building.SpriteIndex];
		//BuildingCost *= 2;
		//BuildButton.GetComponentInChildren<Text> ().text = "Build for " + BuildingCost;
		building.IsBuilt = true;
		CurrentBuildings [building.SpriteIndex]++;
		Buildings.Add (building);
		AddPrestige (PrestigeRewards[building.SpriteIndex]);
	}

	void Start() {
		if (Prestige == 0) {			
			Player.instance.Load ();
			InitializeFromData (Player.instance.MyRestaurant);
		}
		if (Cooks.Count == 0) {
			AddCook ();
		}
		//BuildButton.GetComponentInChildren<Text> ().text = "Build for " + BuildingCost;
	}
		
	void Update() {		
		timer += Time.deltaTime;
		energyTimer += Time.deltaTime;
		if (timer >= TickPeriod && Gold < MaxGoldPerLevel[PrestigeLevel - 1]) {
			timer = 0.0f;
			GenerateGold ();
		}
		if (energyTimer >= EnergyTickPeriod && Energy < MaxEnergy) {
			energyTimer = 0.0f;
			Energy += EnergyPerTick;
		}
		float timeToEnergy = EnergyTickPeriod - energyTimer;
		int minutes = (int)timeToEnergy / 60;
		int seconds = (int)timeToEnergy - minutes * 60;
		string time = minutes + ":" + seconds;
		EnergyTipText.text = System.String.Format ("+{0} energy in:\n{1}", EnergyPerTick, time);
		UpdateInfoLabel ();
	}

	void GenerateGold() {
		int clientsCount = Random.Range (RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0], RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1] + 1);
		//Debug.Log ("Clients min: " + RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0] + ", max " + RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1] + ", came: " + clientsCount);
		int income = 0;
		foreach (var cook in Cooks) {
			for (int i = 0; i < clientsCount; i++) {
				income += cook.GenerateGold ();
			}
		}
		AddGold(income);
	}

	public void Build(int buildingIndex) {
		Debug.Log ("Building index: " + buildingIndex);
		if (Gold >= Costs[buildingIndex]) {			
			builder.DeployBuilding (buildingIndex);
		} else {
			Debug.Log ("Not enough gold");
		}
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
	}

	public static Vector3 RandomScreenPoint() {
		Vector3 camPos = Camera.main.gameObject.transform.position;
		float xPos = Random.Range (camPos.x - 2, camPos.x + 2);
		float yPos = Random.Range (-1, 1);
		float zPos = 0;
		return new Vector3 (xPos, yPos, zPos);
	}

	void UpdateInfoLabel() {
		PrestigeLabel.text = Prestige + "/" + PrestigeRequirementsPerLevel [PrestigeLevel - 1];
		PrestigeLevelLabel.text = PrestigeLevel.ToString();
		PrestigeSlider.maxValue = PrestigeRequirementsPerLevel [PrestigeLevel - 1];
		PrestigeSlider.value = Prestige;

		string goldString = Gold.ToString();
		string maxGoldString = MaxGoldPerLevel [PrestigeLevel - 1].ToString ();
		if (Gold > 1000) {
			goldString = (Gold / 1000) + "k";
		}
		if (MaxGoldPerLevel [PrestigeLevel - 1] > 1000) {
			maxGoldString = (MaxGoldPerLevel [PrestigeLevel - 1] / 1000) + "k";
		}

		GoldText.text = goldString + "/" + maxGoldString;
		GoldSlider.maxValue = MaxGoldPerLevel [PrestigeLevel - 1];
		GoldSlider.value = Gold;

		EnergyText.text = Energy + "/" + MaxEnergy;
		EnergySlider.maxValue = MaxEnergy;
		EnergySlider.value = Energy;

		SessionLabel.text = "Session: " + Session;

		int clientsMin = RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0];
		int clientsMax = RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1];
		int goldMin = 0;
		int goldMax = 0;
		foreach (var cook in Cooks) {
			goldMin += cook.RangeGoldPerClientByLevel [cook.Level - 1, 0];
			goldMax += cook.RangeGoldPerClientByLevel [cook.Level - 1, 1];
		}
		int incomeMin = goldMin * clientsMin;
		int incomeMax = goldMax * clientsMax;

		ClientsLabel.text = "Clients per 5 sec:\n" + RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0] + " - " + RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1] + "\n" +
			"Average income:\n" + incomeMin + " - " + incomeMax;
		
		string info = System.String.Format ("Prestige: {0}/{1}\n" +
											"Level: {2}\n" + 
											"Gold: {3}/{4}",
											Prestige, PrestigeRequirementsPerLevel[PrestigeLevel - 1],
											PrestigeLevel,
											Gold, MaxGoldPerLevel[PrestigeLevel - 1]);
		InfoLabel.text = info;
	}

	public void ChangeScene(int scene) {		
		Debug.Log ("Scene changed!");
		Cook.OnCookClicked -= Cook_OnCookClicked;
		Player.instance.Save ();
		SceneManager.LoadScene (scene);
	}

	public void Wipe() {		
		SaveLoad.Delete ();
		Destroy (Player.instance.gameObject);
		Cook.OnCookClicked -= Cook_OnCookClicked;
		SceneManager.LoadScene (0);
	}

	public void SetMission(MissionData missionData) {
		//Debug.Log (missionData.gameObject);
		Player.instance.SetMission(missionData);
	}

	//bool needsWipe = false;

	void OnApplicationQuit() {
		Session++;
		Player.instance.Save ();
	}

	public void Quit() {
		Application.Quit ();
	}

	public bool SpendEnergy(int amount) {		
		if (Energy >= amount) {
			Debug.Log ("Trying to spend " + amount + " energy, have " + Energy);
			Energy -= amount;
			UpdateInfoLabel ();
			return true;
		} else {			
			return false;
		}
	}

	public void AddEnergy(int amount) {
		Energy += amount;
	}

	public void Toggle(GameObject window) {
		window.SetActive (!window.activeSelf);
	}
}
