using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientUI : MonoBehaviour {
	
	Client myClient;

	//public Text GoldText;
	public Text DropText;
	public Text DishText;

	public Slider LifeTimeSlider;
	public Slider ReadinessSlider;

	void Awake() {
		myClient = gameObject.GetComponent<Client> ();
	}

	void Start() {
		//GoldText.text = "Gold: " + myClient.Gold + "\n";
		string dropInfo = "Possible drop:\n";

		foreach (var item in myClient.ItemRewards) {
			dropInfo += "-" + Restaurant.instance.ItemNames [item] + "\n";
		}

		DropText.text = dropInfo;
		string dishInfo = "Dishes:\n";
		//string critString = "";

		foreach (var dish in myClient.Dishes) {
			int cost = Restaurant.instance.DishCosts [dish];
			/*if (myClient.MyCook != null) {
				foreach (var cookDish in myClient.MyCook.Dishes) {
					if (dish == cookDish) {
						cost *= 2;
						critString = " (crit!)";
					}
				}
			}*/
			dishInfo += dish + ": $" + cost + /*critString +*/ "\n";
			//critString = "";
		}

		DishText.text = dishInfo;

		LifeTimeSlider.minValue = -myClient.MaxLifeTime;
		LifeTimeSlider.maxValue = 0;

		ReadinessSlider.maxValue = myClient.MaxReadiness;
	}

	void Update() {
		LifeTimeSlider.value = -myClient.LifeTimeLeft;
		ReadinessSlider.value = myClient.Readiness;
	}

	public void ChangeCook(Cook cook) {
		string dishInfo = "Dishes:\n";
		string critString = "";
		foreach (var dish in myClient.Dishes) {
			int cost = Restaurant.instance.DishCosts [dish];
			if (myClient.MyCook != null) {
				foreach (var cookDish in myClient.MyCook.Dishes) {
					if (dish == cookDish) {
						cost *= 2;
						critString = " (crit!)";
					}
				}
			}
			dishInfo += dish + ": $" + cost + critString + "\n";
			critString = "";
		}
		DishText.text = dishInfo;
	}
}
