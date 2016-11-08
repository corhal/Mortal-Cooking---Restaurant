using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {

	public int OpenCost;
	public int[] Items;
	public int[] Weights;

	public delegate void ChestOpenedEventHandler (int item);
	public static event ChestOpenedEventHandler OnChestOpened;

	public void Start() {
		Items = new int[Restaurant.instance.Items.Length];
		Restaurant.instance.Items.CopyTo (Items, 0);
	}

	public int Open() {
		int index = Random.Range (0, Items.Length);
		OnChestOpened (Items[index]);
		return Items [index];
	}
}
