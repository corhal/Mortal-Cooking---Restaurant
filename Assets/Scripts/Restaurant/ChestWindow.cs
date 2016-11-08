using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChestWindow : MonoBehaviour {
	
	public Text DropText;
	public Text OpenButtonText;

	void Awake() {
		Chest.OnChestOpened += Chest_OnChestOpened;
	}

	void Chest_OnChestOpened (int item) {
		UpdateWindow (item);
	}

	void Start() {		
		OpenButtonText.text = "Open for " + Restaurant.instance.chest.OpenCost;
	}

	public void UpdateWindow(int item) {
		string dropString = "";
		if (item >= 0) {			
			dropString = Restaurant.instance.ItemNames [item] + " +1 (" + (Restaurant.instance.ItemCounts [item] + 1) + ")"; // тут +1, потому что сначала приходит событие, а потом в ресторане обновляется инфа
		}
		DropText.text = dropString;
	}

	void OnDestroy() {
		Chest.OnChestOpened -= Chest_OnChestOpened;
	}
}
