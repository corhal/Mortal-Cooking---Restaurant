using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestaurantUI : MonoBehaviour {

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

	void Awake () {
		Restaurant.OnRestaurantInfoChanged += Restaurant_OnRestaurantInfoChanged;
	}

	void Restaurant_OnRestaurantInfoChanged () {
		UpdateInfoLabel ();
	}

	void UpdateInfoLabel() { // TODO: разбить на раздельные методы
		PrestigeLabel.text = Restaurant.instance.Prestige + "/" + Restaurant.instance.PrestigeRequirementsPerLevel [Restaurant.instance.PrestigeLevel - 1];
		PrestigeLevelLabel.text = Restaurant.instance.PrestigeLevel.ToString();
		PrestigeSlider.maxValue = Restaurant.instance.PrestigeRequirementsPerLevel [Restaurant.instance.PrestigeLevel - 1];
		PrestigeSlider.value = Restaurant.instance.Prestige;

		string goldString = Restaurant.instance.Gold.ToString();
		string maxGoldString = Restaurant.instance.MaxGoldPerLevel [Restaurant.instance.PrestigeLevel - 1].ToString ();
		if (Restaurant.instance.Gold > 1000) {
			goldString = (Restaurant.instance.Gold / 1000) + "k";
		}
		if (Restaurant.instance.MaxGoldPerLevel [Restaurant.instance.PrestigeLevel - 1] > 1000) {
			maxGoldString = (Restaurant.instance.MaxGoldPerLevel [Restaurant.instance.PrestigeLevel - 1] / 1000) + "k";
		}

		GoldText.text = goldString + "/" + maxGoldString;
		GoldSlider.maxValue = Restaurant.instance.MaxGoldPerLevel [Restaurant.instance.PrestigeLevel - 1];
		GoldSlider.value = Restaurant.instance.Gold;

		EnergyText.text = Restaurant.instance.Energy + "/" + Restaurant.instance.MaxEnergy;
		EnergySlider.maxValue = Restaurant.instance.MaxEnergy;
		EnergySlider.value = Restaurant.instance.Energy;

		SessionLabel.text = "Session: " + Restaurant.instance.Session;

		int clientsMin = Restaurant.instance.RangeClientsPerSecondByLevel [Restaurant.instance.PrestigeLevel - 1, 0];
		int clientsMax = Restaurant.instance.RangeClientsPerSecondByLevel [Restaurant.instance.PrestigeLevel - 1, 1];
		int goldMin = 0;
		int goldMax = 0;
		foreach (var cook in Restaurant.instance.Cooks) {
			goldMin += cook.RangeGoldPerClientByLevel [cook.Level - 1, 0];
			goldMax += cook.RangeGoldPerClientByLevel [cook.Level - 1, 1];
		}
		int incomeMin = goldMin * clientsMin;
		int incomeMax = goldMax * clientsMax;

		ClientsLabel.text = "Clients per 5 sec:\n" + Restaurant.instance.RangeClientsPerSecondByLevel [Restaurant.instance.PrestigeLevel - 1, 0] + " - " + Restaurant.instance.RangeClientsPerSecondByLevel [Restaurant.instance.PrestigeLevel - 1, 1] + "\n" +
			"Average income:\n" + incomeMin + " - " + incomeMax;
	}

	void OnDestroy() {
		Restaurant.OnRestaurantInfoChanged -= Restaurant_OnRestaurantInfoChanged;
	}
}
