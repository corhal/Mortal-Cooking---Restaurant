﻿using UnityEngine;
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
	public Text StarmoneyText;
	public Text EnergyText;
	public Text SessionLabel;
	public Text EnergyTipText;
	public Text StarsText;
	public Text RaidTicketsText;

	void Awake () {
		Restaurant.OnRestaurantInfoChanged += Restaurant_OnRestaurantInfoChanged;
	}

	void Restaurant_OnRestaurantInfoChanged () {		
		UpdateInfoLabel ();
	}

	void UpdateInfoLabel() { // TODO: разбить на раздельные методы
		StarsText.text = "Stars: " + Restaurant.instance.Stars;
		RaidTicketsText.text = "Tickets: " + Restaurant.instance.RaidTickets;
		PrestigeLabel.text = Restaurant.instance.Prestige + "/" + Restaurant.instance.PrestigeRequirementsPerLevel [Restaurant.instance.PrestigeLevel - 1];
		PrestigeLevelLabel.text = Restaurant.instance.PrestigeLevel.ToString();
		PrestigeSlider.maxValue = Restaurant.instance.PrestigeRequirementsPerLevel [Restaurant.instance.PrestigeLevel - 1];
		PrestigeSlider.value = Restaurant.instance.Prestige;

		string goldString = Restaurant.instance.Gold.ToString();
		if (Restaurant.instance.Gold > 1000) {
			int hundreds = Restaurant.instance.Gold / 100;
			string num = hundreds.ToString ();
			string firstPart = num.Substring (0, num.Length - 1);
			string secondPart = num.Substring (num.Length - 1, 1);
			goldString = firstPart + "." + secondPart + "k";
		}

		GoldText.text = goldString;
		GoldSlider.maxValue = 0;
		GoldSlider.value = 0;

		EnergyText.text = Restaurant.instance.Energy + "/" + Restaurant.instance.MaxEnergy;
		EnergySlider.maxValue = Restaurant.instance.MaxEnergy;
		EnergySlider.value = Restaurant.instance.Energy;

		SessionLabel.text = "Session: " + Restaurant.instance.Session;

		int clientsMin = Restaurant.instance.RangeClientsPerTickByLevel [Restaurant.instance.PrestigeLevel - 1, 0];
		int clientsMax = Restaurant.instance.RangeClientsPerTickByLevel [Restaurant.instance.PrestigeLevel - 1, 1];
		int goldMin = 0;
		int goldMax = 0;

		int incomeMin = goldMin * clientsMin;
		int incomeMax = goldMax * clientsMax;

		ClientsLabel.text = "Clients per 5 sec:\n" + Restaurant.instance.RangeClientsPerTickByLevel [Restaurant.instance.PrestigeLevel - 1, 0] + " - " + Restaurant.instance.RangeClientsPerTickByLevel [Restaurant.instance.PrestigeLevel - 1, 1] + "\n" +
			"Average income:\n" + incomeMin + " - " + incomeMax;

		StarmoneyText.text = Restaurant.instance.Starmoney.ToString ();
	}

	void OnDestroy() {
		Restaurant.OnRestaurantInfoChanged -= Restaurant_OnRestaurantInfoChanged;
	}
}
