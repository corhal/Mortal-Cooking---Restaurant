using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Restaurant : MonoBehaviour {

	float timer;

	public int Gold;
	public int Prestige;
	public int PrestigeLevel;
	public int[] PrestigeRequirementsPerLevel;
	public int[,] RangeClientsPerSecondByLevel;

	public List<Cook> Cooks;
	public List<Building> Buildings;

	public GameObject CookPrefab;
	public Text InfoLabel;

	void Awake() {
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
	}

	void Start() {
		AddCook ();
	}

	void Update() {
		timer += Time.deltaTime;
		if (timer >= 1.0f) {
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
		if (Gold >= buildingPrefab.GetComponent<Building>().Cost) {
			Gold -= buildingPrefab.GetComponent<Building>().Cost;
			AddBuilding (buildingPrefab);
		} else {
			Debug.Log ("Not enough gold");
		}
	}

	public void AddCook() {		
		GameObject newCookObject = Instantiate (CookPrefab, RandomScreenPoint(), CookPrefab.transform.rotation) as GameObject;
		Cook newCook = newCookObject.GetComponent<Cook> ();
		Cooks.Add (newCook);
	}

	void AddBuilding(GameObject buildingPrefab) {
		GameObject newBuildingObject = Instantiate (buildingPrefab, RandomScreenPoint(), buildingPrefab.transform.rotation) as GameObject;
		Building newBuilding = newBuildingObject.GetComponent<Building> ();
		Buildings.Add (newBuilding);
		AddPrestige (newBuilding.Prestige);
	}

	void AddPrestige(int amount) {
		if (Prestige < PrestigeRequirementsPerLevel[PrestigeRequirementsPerLevel.Length - 1]) {
			Prestige += amount;
			if (PrestigeLevel < PrestigeRequirementsPerLevel.Length && Prestige >= PrestigeRequirementsPerLevel[PrestigeLevel - 1]) {
				PrestigeLevel++;
			}
		}
	}

	Vector3 RandomScreenPoint() {
		float xPos = Random.Range (-2, 2);
		float yPos = Random.Range (-4, 4);
		float zPos = 0;
		return new Vector3 (xPos, yPos, zPos);
	}

	void UpdateInfoLabel() {
		string info = System.String.Format ("Prestige: {0}/{1}\n" +
											"Level: {2}\n" + 
											"Gold: {3}",
											Prestige, PrestigeRequirementsPerLevel[PrestigeLevel - 1],
											PrestigeLevel,
											Gold);
		InfoLabel.text = info;
	}
}
