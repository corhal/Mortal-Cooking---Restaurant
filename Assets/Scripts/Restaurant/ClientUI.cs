using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientUI : MonoBehaviour {
	
	Client myClient;

	public Text DropText;
	public Text DishText;

	void Awake() {
		myClient = gameObject.GetComponent<Client> ();
	}

	void Start() {
		string dropInfo = "Possible drop:\n";

		foreach (var item in myClient.ItemRewards) {
			dropInfo += "-" + Restaurant.instance.ItemNames [item] + "\n";
		}

		DropText.text = dropInfo;
		string dishInfo = "Dishes:\n";

		foreach (var dish in myClient.Dishes) {
			int cost = Restaurant.instance.DishCosts [dish];
			dishInfo += dish + ": $" + cost + "\n";

		}

		DishText.text = dishInfo;
	}
}
