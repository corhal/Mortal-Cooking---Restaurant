using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cook : MonoBehaviour {

	public int Id;

	public int[] Dishes;
	public CookData cookData;

	Restaurant restaurant;
	public GameObject ButtonContainer;
	public GameObject RaidButton;
	public bool RaidReady;

	public bool Selected;

	public int Level;
	public int Soulstones;

	public int[,] RangeGoldPerClientByLevel;
	public int[] SoulstoneRequirementsPerLevel;

	public delegate void CookClickedEventHandler (Cook cook);
	public static event CookClickedEventHandler OnCookClicked;

	public Text LevelLabel;
	public Text SoulstonesLabel;
	public Slider SoulstonesSlider;

	void Awake() {
		restaurant = GameObject.FindObjectOfType<Restaurant> ();
		RangeGoldPerClientByLevel = new int[,] {
			{1, 3},
			{2, 4},
			{3, 5},
			{4, 6},
			{5, 7}
		};
		LevelLabel = GetComponentInChildren<Text> ();
	}

	void Start() {
		UpdateLabels ();
	}

	void OnMouseUpAsButton() {		
		Debug.Log ("Clicked cook");
		OnCookClicked (this);
		//AddSoulstones (5);
	}

	public void ToggleSelect() {
		Selected = !Selected;
		ButtonContainer.SetActive (!ButtonContainer.activeSelf);
		if (!RaidReady) {
			RaidButton.SetActive (false);
		}
		UpdateLabels ();
	}

	public void Deselect() {
		Selected = false;
		ButtonContainer.SetActive (false);
		UpdateLabels ();
	}

	public int GenerateGold() {
		int gold = Random.Range (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1] + 1);
		//Debug.Log("Gold min: " + (RangeGoldPerClientByLevel [Level - 1, 0] + ", max: " + RangeGoldPerClientByLevel [Level - 1, 1] + ", rolled: " + gold));
		return gold;
	}

	public int GenerateGoldPerClients(int clients) {		
		return ((RangeGoldPerClientByLevel [Level - 1, 0] + RangeGoldPerClientByLevel [Level - 1, 1]) / 2) * clients;
	}

	public void AddSoulstones(int amount) {
		if (Soulstones < SoulstoneRequirementsPerLevel[SoulstoneRequirementsPerLevel.Length - 1]) {
			Soulstones += amount;
			if (Level < SoulstoneRequirementsPerLevel.Length && Soulstones >= SoulstoneRequirementsPerLevel[Level - 1]) {
				Level++;
			}
			UpdateLabels ();
		}
	}

	public void Play() {
		if (restaurant.SpendEnergy(restaurant.EnergyCostPerMission)) {
			restaurant.SetMission (gameObject.GetComponentInChildren<MissionData> ());
			restaurant.ChangeScene (1);
		}
	}

	public void Raid() {
		if (restaurant.SpendEnergy(restaurant.EnergyCostPerMission)) {
			float coinToss = Random.Range (0.0f, 1.0f);
			int rand = Random.Range (1, 4);
			if (coinToss < 0.5f) {
				rand = 0;
			}
			AddSoulstones (rand);
			restaurant.AddGold (restaurant.GoldReward);
		}
	}

	public void InitializeFromData(CookData data) {
		Id = data.Id;
		transform.position = new Vector3(data.x, data.y, data.z);
		Level = data.Level;
		Soulstones = data.Soulstones;
		RaidReady = data.RaidReady;
		Dishes = new int[data.Dishes.Length];
		data.Dishes.CopyTo (Dishes, 0);
	}

	public void UpdateLabels() {
		LevelLabel.text = Level.ToString ();
		SoulstonesLabel.text = Soulstones + "/" + SoulstoneRequirementsPerLevel [Level - 1];
		SoulstonesSlider.maxValue = SoulstoneRequirementsPerLevel [Level - 1];
		SoulstonesSlider.value = Soulstones;
	}
}
