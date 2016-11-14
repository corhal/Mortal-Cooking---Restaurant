using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour {

	public Cook MyCook;

	public MissionData Mission;
	public int[] Dishes;
	public int[] ItemRewards;
	public float[] ItemChances;

	public float MaxLifeTime;
	public float LifeTimeLeft;

	//public int Gold;

	public bool[] Crits;
	public bool[] FreeDishes;
	public int[] DishReadiness;
	public int[] MaxDishReadiness;

	public int Readiness;
	public int MaxReadiness;

	public delegate void ClientDiedEventHandler (Client client);
	public static event ClientDiedEventHandler OnClientDied;

	public delegate void DishReadyEventHandler (int dish);
	public event DishReadyEventHandler OnDishReady;

	void Awake() {
		ClientUI myUI = gameObject.GetComponent<ClientUI> ();
		Mission = gameObject.GetComponent<MissionData> ();
		LifeTimeLeft = MaxLifeTime;
		// Dishes = new List<int> (); // если этого не делать, юнити вешается.
	}

	public void ChangeCook(Cook cook) {
		//ClientUI myUI = gameObject.GetComponent<ClientUI> ();
		MyCook = cook;
		//myUI.ChangeCook (cook);
	}

	public void Start() {
		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);
	}

	public void Play() {
		if (Restaurant.instance.SpendEnergy(Restaurant.instance.EnergyCostPerMission)) {
			
			Mission.GoldReward = CalculateGoldReward();
			Restaurant.instance.SetMission (Mission);
			AddReadiness (100);
			Restaurant.instance.ChangeScene (1);
		}
	}

	int CalculateGoldReward() {
		int goldReward = 0;
		foreach (var dish in Dishes) {
			int addition = Restaurant.instance.DishCosts [dish];
			if (MyCook != null) {
				foreach (var cookDish in MyCook.Dishes) {
					if (dish == cookDish) {
						addition *= 2;
					}
				}
			}
			goldReward += addition;
		}
		return goldReward;
	}

	public void Raid() {
		// Пока не просим рейдтикеты
		if (/*Restaurant.instance.RaidTickets > 0 &&*/ Restaurant.instance.SpendEnergy (Restaurant.instance.EnergyCostPerMission)) {
			Mission.GoldReward = CalculateGoldReward ();
			AddReadiness (100);
			Restaurant.instance.RaidMission (Mission);
		}
	}

	public void InitializeFromData(ClientData data) {
		transform.position = new Vector3 (data.x, data.y, data.z);
		Readiness = data.Readiness;
		LifeTimeLeft = data.LifeTimeLeft;
		ItemRewards = new int[data.ItemRewards.Length];
		data.ItemRewards.CopyTo (ItemRewards, 0);
		ItemChances = new float[ItemRewards.Length];
		data.ItemChances.CopyTo (ItemChances, 0);

		Dishes = new int[data.Dishes.Length];
	    data.Dishes.CopyTo(Dishes, 0);

		Crits = new bool[data.Crits.Length];
		data.Crits.CopyTo (Crits, 0);

		FreeDishes = new bool[data.FreeDishes.Length];
		data.FreeDishes.CopyTo (FreeDishes, 0);

		DishReadiness = new int[data.DishReadiness.Length];
		data.DishReadiness.CopyTo (DishReadiness, 0);

		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);
	}

	void Update() {
		ReduceLifetime (Time.deltaTime);
	}

	public void ReduceLifetime(float amount) {
		LifeTimeLeft -= amount;
		if (LifeTimeLeft <= 0) {
			Die ();
		}
	}

	public void AddReadiness(int dish, int amount) {
		int index = System.Array.IndexOf (Dishes, dish);
		DishReadiness [index] += amount;
		if (DishReadiness[index] >= MaxDishReadiness[index]) {
			OnDishReady (dish);
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
