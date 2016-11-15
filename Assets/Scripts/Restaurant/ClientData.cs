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

	public int[] ItemRewards;
	public float[] ItemChances;


	public void InitializeFromClient(Client client) {			
		x = client.transform.position.x;
		y = client.transform.position.y;
		z = client.transform.position.z;

		ItemRewards = new int[client.ItemRewards.Length];
		client.ItemRewards.CopyTo (ItemRewards, 0);
		ItemChances = new float[ItemRewards.Length];
		client.ItemChances.CopyTo (ItemChances, 0);

		Dishes = new int[client.Dishes.Length];
		client.Dishes.CopyTo (Dishes, 0);
	}
}
