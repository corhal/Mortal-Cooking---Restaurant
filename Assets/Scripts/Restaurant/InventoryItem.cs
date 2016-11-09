using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour {

	public int MyItem;

	public void BreakItem() {
		Restaurant.instance.BreakItem (MyItem);
	}
}
