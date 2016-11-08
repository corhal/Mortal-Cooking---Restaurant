using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Client : MonoBehaviour {

	public Text GoldText;
	public Text DropText;
	public Text DishText;

	public MissionData Mission;
	public int Dish;
	public int[] ItemRewards;
	public float[] ItemChances;
	public float LifeTimeLeft;
	public int Gold;

	public delegate void ClientDiedEventHandler (Client client);
	public static event ClientDiedEventHandler OnClientDied;

	void Awake() {
		Mission = gameObject.GetComponent<MissionData> ();
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
		DishText.text = "Dish: " + Dish;
	}

	public void Play() {
		if (Restaurant.instance.SpendEnergy(Restaurant.instance.EnergyCostPerMission)) {
			Mission.GoldReward = GiveGold (Gold);
			Restaurant.instance.SetMission (Mission);
			Restaurant.instance.ChangeScene (1);
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

		Dish = data.Dish;

		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);

		string dropInfo = "Possible drop:\n";

		foreach (var item in ItemRewards) {
			dropInfo += "-" + Restaurant.instance.ItemNames [item] + "\n";
		}

		DropText.text = dropInfo;
		DishText.text = "Dish: " + Dish;
	}

	void Update() {
		GoldText.text = "Gold: " + Gold + "\n\n" + "Possible drop:\n";

		LifeTimeLeft -= Time.deltaTime;
		if (LifeTimeLeft <= 0) {
			Die ();
		}
	}

	public void Die() {
		OnClientDied (this);
	}
}
