using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClientData {

	public float x;
	public float y;
	public float z;

	//public int Dish;
	public int[] Dishes;
	public float LifeTimeLeft;

	public int[] ItemRewards;
	public float[] ItemChances;

	public int Readiness;

	public bool[] Crits;
	public bool[] FreeDishes;
	public int[] DishReadiness;

	public void InitializeFromClient(Client client) {			
		x = client.transform.position.x;
		y = client.transform.position.y;
		z = client.transform.position.z;

		Readiness = client.Readiness;
		ItemRewards = new int[client.ItemRewards.Length];
		client.ItemRewards.CopyTo (ItemRewards, 0);
		ItemChances = new float[ItemRewards.Length];
		client.ItemChances.CopyTo (ItemChances, 0);

		/*if (client.Dishes != null) {
			Dishes = new List<int> (client.Dishes);
		}*/

		Dishes = new int[client.Dishes.Length];
		client.Dishes.CopyTo (Dishes, 0);

		Crits = new bool[client.Crits.Length];
		client.Crits.CopyTo (Crits, 0);

		FreeDishes = new bool[client.FreeDishes.Length];
		client.FreeDishes.CopyTo (FreeDishes, 0);

		DishReadiness = new int[client.DishReadiness.Length];
		client.DishReadiness.CopyTo (DishReadiness, 0);

		LifeTimeLeft = client.LifeTimeLeft;
	}
}
