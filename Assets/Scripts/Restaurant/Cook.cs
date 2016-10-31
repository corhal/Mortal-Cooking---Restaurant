using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cook : MonoBehaviour {

	public int Id;

	public CookData cookData;

	Restaurant restaurant;
	public GameObject ButtonContainer;
	Text levelLabel;

	public bool Selected;

	public int Level;
	public int Soulstones;

	public int[,] RangeGoldPerClientByLevel;
	public int[] SoulstoneRequirementsPerLevel;

	public delegate void CookClickedEventHandler (Cook cook);
	public static event CookClickedEventHandler OnCookClicked;

	void Awake() {
		restaurant = GameObject.FindObjectOfType<Restaurant> ();
		RangeGoldPerClientByLevel = new int[,] {
			{1, 3},
			{2, 4},
			{3, 5},
			{4, 6},
			{5, 7}
		};
		levelLabel = GetComponentInChildren<Text> ();
	}

	void Start() {
		levelLabel.text = Level.ToString ();
	}

	void OnMouseUpAsButton() {		
		Debug.Log ("Clicked cook");
		OnCookClicked (this);
		//AddSoulstones (5);
	}

	public void ToggleSelect() {
		Selected = !Selected;
		ButtonContainer.SetActive (!ButtonContainer.activeSelf);
	}

	public int GenerateGold() {
		return Random.Range (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1]);
	}

	public int GenerateGoldPerClients(int clients) {
		return ((RangeGoldPerClientByLevel [Level - 1, 0] + RangeGoldPerClientByLevel [Level - 1, 1]) / 2) * clients;
	}

	public void AddSoulstones(int amount) {
		if (Soulstones < SoulstoneRequirementsPerLevel[SoulstoneRequirementsPerLevel.Length - 1]) {
			Soulstones += amount;
			if (Level < SoulstoneRequirementsPerLevel.Length && Soulstones >= SoulstoneRequirementsPerLevel[Level - 1]) {
				Level++;
				levelLabel.text = Level.ToString ();
			}
		}
	}

	public void Play() {
		restaurant.SetMission (gameObject.GetComponentInChildren<MissionData> ());
		restaurant.ChangeScene (1);
	}

	public void InitializeFromData(CookData data) {
		Id = data.Id;
		transform.position = new Vector3(data.x, data.y, data.z);
		Level = data.Level;
		Soulstones = data.Soulstones;
	}
}
