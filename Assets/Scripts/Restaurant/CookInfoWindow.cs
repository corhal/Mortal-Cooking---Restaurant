using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CookInfoWindow : MonoBehaviour {

	public GameObject ContentContainer;
	public Text LevelText;
	public Text DishText;
	public Text GoldText;

	void Awake() {
		Cook.OnInfoShow += Cook_OnInfoShow;
	}

	void Cook_OnInfoShow (Cook cook) {
		ContentContainer.SetActive (true);
		UpdateInfoWindow (cook);
	}
	
	public void UpdateInfoWindow (Cook cook) {			
		string levelString = "Level " + cook.Level + "\n";
		int[] itemCollection = cook.CurrentCollection ();
		for (int i = 0; i < itemCollection.Length; i++) {
			levelString += Restaurant.instance.ItemNames[itemCollection [i]] + ": " + Restaurant.instance.ItemCounts [itemCollection [i]] + "/1\n";
		}
		LevelText.text = levelString;

		string dishString = "Best dishes:\n";
		for (int i = 0; i < cook.Dishes.Length; i++) {
			dishString += "- " + cook.Dishes [i] + "\n";
		}
		DishText.text = dishString;

		string goldString = "Gold per client: " + cook.RangeGoldPerClientByLevel [cook.Level - 1, 0] + "-" + cook.RangeGoldPerClientByLevel [cook.Level - 1, 1] + "\n";
		goldString += "Gold now: " + cook.Gold + "/" + cook.MaxGoldByLevel [cook.Level - 1];
		GoldText.text = goldString;
	}

	void OnDestroy() {
		Cook.OnInfoShow -= Cook_OnInfoShow;
	}
}
