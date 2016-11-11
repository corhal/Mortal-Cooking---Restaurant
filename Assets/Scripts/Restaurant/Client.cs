using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour {

	public Cook MyCook;

	public Text GoldText;
	public Text DropText;
	public Text DishText;

	public MissionData Mission;
	public int[] Dishes;
	public int[] ItemRewards;
	public float[] ItemChances;
	public float LifeTimeLeft;
	public int Gold;

	public int Readiness;
	public int MaxReadiness;

	public delegate void ClientDiedEventHandler (Client client);
	public static event ClientDiedEventHandler OnClientDied;

	void Awake() {
		Mission = gameObject.GetComponent<MissionData> ();
		// Dishes = new List<int> (); // если этого не делать, юнити вешается.
	}

	public void Start() {
		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);

		string dropInfo = "Possible drop:\n";

		foreach (var item in ItemRewards) {
			dropInfo += "-" + Restaurant.instance.ItemNames [item] + "\n";
		}

		DropText.text = dropInfo;
		string dishInfo = "Dishes:\n";

		foreach (var dish in Dishes) {
			dishInfo += dish + "\n";
		}

		DishText.text = dishInfo;
	}

	public void Play() {
		if (Restaurant.instance.SpendEnergy(Restaurant.instance.EnergyCostPerMission)) {
			Mission.GoldReward = GiveGold (Gold);
			Restaurant.instance.SetMission (Mission);
			Restaurant.instance.ChangeScene (1);
		}
	}

	public void Raid() {
		if (Restaurant.instance.RaidTickets > 0 && Restaurant.instance.SpendEnergy (Restaurant.instance.EnergyCostPerMission)) {
			Mission.GoldReward = GiveGold (Gold);
			Restaurant.instance.RaidMission (Mission);
		}
	}

	public int GiveGold(int amount) {
		int goldToSpend = Mathf.Min (Gold, amount);
		Gold -= goldToSpend;
		if (Gold <= 0) {
			Die ();
		}
		return goldToSpend;
	}

	public void InitializeFromData(ClientData data) {
		transform.position = new Vector3 (data.x, data.y, data.z);
		LifeTimeLeft = data.LifeTimeLeft;
		ItemRewards = new int[data.ItemRewards.Length];
		data.ItemRewards.CopyTo (ItemRewards, 0);
		ItemChances = new float[ItemRewards.Length];
		data.ItemChances.CopyTo (ItemChances, 0);

		/*if (data.Dishes != null) {
			Dishes = new List<int> (data.Dishes);
		}*/

		Dishes = new int[data.Dishes.Length];
	    data.Dishes.CopyTo(Dishes, 0);

		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);

		string dropInfo = "Possible drop:\n";

		foreach (var item in ItemRewards) {
			dropInfo += "-" + Restaurant.instance.ItemNames [item] + "\n";
		}

		DropText.text = dropInfo;
		string dishInfo = "Dishes:\n";

		foreach (var dish in Dishes) {
			dishInfo += dish + "\n";
		}

		DishText.text = dishInfo;
	}

	void Update() {
		GoldText.text = "Gold: " + Gold + "\n\n" + "Possible drop:\n";

		LifeTimeLeft -= Time.deltaTime;
		if (LifeTimeLeft <= 0) {
			Die ();
		}
	}

	public void AddReadiness(int amount) {
		Readiness += amount;
		if (Readiness >= MaxReadiness) {
			Die ();
		}
	}

	public void Die() {
		OnClientDied (this);
	}
}
