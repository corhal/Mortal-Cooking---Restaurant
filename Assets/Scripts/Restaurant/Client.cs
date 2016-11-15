using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour {

	public MissionData Mission;
	public int[] Dishes;
	public int[] ItemRewards;
	public float[] ItemChances;

	void Awake() {
		ClientUI myUI = gameObject.GetComponent<ClientUI> ();
		Mission = gameObject.GetComponent<MissionData> ();
	}

	public void Start() {
		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];
		Mission.Dishes = new int[Dishes.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);
		Dishes.CopyTo (Mission.Dishes, 0);
	}

	public void Play() {
		//Restaurant.instance.PlayMission (Mission);
		Restaurant.instance.ShowMissionStartWindow(Mission);
	}

	public void Raid() {
		// Пока не просим рейдтикеты
		if (/*Restaurant.instance.RaidTickets > 0 &&*/ Restaurant.instance.SpendEnergy (Restaurant.instance.EnergyCostPerMission)) {			
			Restaurant.instance.RaidMission (Mission);
		}
	}

	public void InitializeFromData(ClientData data) {
		transform.position = new Vector3 (data.x, data.y, data.z);
		ItemRewards = new int[data.ItemRewards.Length];
		data.ItemRewards.CopyTo (ItemRewards, 0);
		ItemChances = new float[ItemRewards.Length];
		data.ItemChances.CopyTo (ItemChances, 0);

		Dishes = new int[data.Dishes.Length];
	    data.Dishes.CopyTo(Dishes, 0);

		Mission.ItemRewards = new int[ItemRewards.Length];
		Mission.ItemChances = new float[ItemChances.Length];

		ItemRewards.CopyTo (Mission.ItemRewards, 0);
		ItemChances.CopyTo (Mission.ItemChances, 0);
	}
}
