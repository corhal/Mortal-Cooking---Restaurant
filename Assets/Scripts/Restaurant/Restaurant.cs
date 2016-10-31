using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Restaurant : MonoBehaviour {

	public int tickPeriod;

	public GameObject BuildButton;
	public System.DateTime LastTime;

	float timer;
	Builder builder;
	Cook selectedCook;

	public GameObject BuildingPrefab;

	public bool NeedsWipe;

	public int BuildingCost;
	public int Gold;
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
			{1, 5},
			{2, 6},
			{3, 7},
			{4, 8},
			{5, 9},
			{6, 10},
			{7, 11},
			{8, 12},
			{9, 13},
			{10, 14}
		};
		builder = gameObject.GetComponent<Builder> ();
		builder.OnBuildingBuilt += Builder_OnBuildingBuilt;
		Cook.OnCookClicked += Cook_OnCookClicked;
	}

	public void InitializeFromData(RestaurantData data) {
		Debug.Log ("Building cost in data: " + data.BuildingCost);
		if (data.BuildingCost != 0) { // Мне так, так стыдно
			BuildingCost = data.BuildingCost;
			BuildButton.GetComponentInChildren<Text> ().text = "Build for " + BuildingCost;
		}

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
				newCook.AddSoulstones (Player.instance.Soulstones);
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
		Debug.Log (ts);
		int seconds = (int)ts.TotalSeconds / tickPeriod;
		Debug.Log ("Ticks passed: " + seconds);
		NeedsWipe = data.NeedsWipe;
		int clientsCount = ((RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0] + RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1]) / 2) * seconds;
		Debug.Log ("Clients visited: " + clientsCount);
		int income = 0;
		foreach (var cook in Cooks) {
			income += cook.GenerateGoldPerClients (clientsCount);
			Debug.Log ("Income from cook: " + income);
		}
		if (data.Gold + income > MaxGoldPerLevel[PrestigeLevel - 1]) {
			Gold = MaxGoldPerLevel [PrestigeLevel - 1];
		} else {
			Gold = data.Gold + income;
		}
	}

	void Cook_OnCookClicked (Cook cook) {
		Debug.Log ("OnCookClicked");
		selectedCook = cook;
		selectedCook.ToggleSelect ();
	}

	void Builder_OnBuildingBuilt (Building building) {
		Gold -= BuildingCost;
		BuildingCost *= 2;
		BuildButton.GetComponentInChildren<Text> ().text = "Build for " + BuildingCost;
		building.IsBuilt = true;
		Buildings.Add (building);
		AddPrestige (building.Prestige);
	}

	void Start() {
		if (Prestige == 0) {			
			InitializeFromData (Player.instance.MyRestaurant);
		}
		if (Cooks.Count == 0) {
			AddCook ();
		}
		BuildButton.GetComponentInChildren<Text> ().text = "Build for " + BuildingCost;
	}

	void Update() {		
		timer += Time.deltaTime;
		if (timer >= tickPeriod && Gold < MaxGoldPerLevel[PrestigeLevel - 1]) {
			timer = 0.0f;
			GenerateGold ();
		}
		UpdateInfoLabel ();
	}

	void GenerateGold() {
		int clientsCount = Random.Range (RangeClientsPerSecondByLevel [PrestigeLevel - 1, 0], RangeClientsPerSecondByLevel [PrestigeLevel - 1, 1]);
		int income = 0;
		foreach (var cook in Cooks) {
			for (int i = 0; i < clientsCount; i++) {
				income += cook.GenerateGold ();
			}
		}
		Gold += income;
	}

	public void Build(GameObject buildingPrefab) {
		if (Gold >= BuildingCost) {			
			builder.DeployBuilding (buildingPrefab);
		} else {
			Debug.Log ("Not enough gold");
		}
	}

	public void AddCook() {		
		GameObject newCookObject = Instantiate (CookPrefab, RandomScreenPoint(), CookPrefab.transform.rotation) as GameObject;
		Cook newCook = newCookObject.GetComponent<Cook> ();
		Cooks.Add (newCook);
		newCook.Id = Cooks.Count;
	}

	void AddPrestige(int amount) {
		if (Prestige < PrestigeRequirementsPerLevel[PrestigeRequirementsPerLevel.Length - 1]) {
			Prestige += amount;
			if (PrestigeLevel < PrestigeRequirementsPerLevel.Length && Prestige >= PrestigeRequirementsPerLevel[PrestigeLevel - 1]) {
				PrestigeLevel++;
				foreach (var level in AdditionalCookLevels) {
					if (PrestigeLevel == level) {
						AddCook ();
					}
				}
			}
		}
	}

	public static Vector3 RandomScreenPoint() {
		float xPos = Random.Range (-2, 2);
		float yPos = Random.Range (-3, 4);
		float zPos = 0;
		return new Vector3 (xPos, yPos, zPos);
	}

	void UpdateInfoLabel() {
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
		Player.instance.Save ();
	}

	public void Quit() {
		Application.Quit ();
	}
}
