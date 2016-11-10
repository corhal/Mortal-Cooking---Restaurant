using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryWindow : MonoBehaviour {

	public GameObject TextPrefab;
	public GameObject TextParent;
	List<Text> texts;

	void Awake() {
		Restaurant.OnRestaurantInfoChanged += Restaurant_OnRestaurantInfoChanged;
	}

	void Restaurant_OnRestaurantInfoChanged () {
		UpdateWindow ();
	}

	void Start() {
		texts = new List<Text> ();
		foreach (var item in Restaurant.instance.Items) {
			GameObject newText = Instantiate (TextPrefab, TextParent.transform) as GameObject;
			newText.transform.localPosition = new Vector3 (newText.transform.localPosition.x, newText.transform.localPosition.y, 0);
			newText.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			texts.Add(newText.GetComponent<Text> ());
			newText.GetComponent<InventoryItem> ().MyItem = item;
		}
		UpdateWindow ();
	}

	public void UpdateWindow() {
		if (texts != null) {
			for (int i = 0; i < Restaurant.instance.Items.Length; i++) {
				texts [i].text = Restaurant.instance.ItemNames [Restaurant.instance.Items [i]] + ": " + Restaurant.instance.ItemCounts [i];
				Button button = texts [i].gameObject.GetComponentInChildren<Button> ();
				if (button != null) { // у меня определенно проблемы с порядком событий, но я пока не понимаю, как их решить
					if (Restaurant.instance.ItemCounts [i] > 0) {
						button.gameObject.SetActive (true);
					} else {
						button.gameObject.SetActive (false);
					}
				}
			}
		}
	}

	void OnDestroy() {
		Restaurant.OnRestaurantInfoChanged -= Restaurant_OnRestaurantInfoChanged;
	}
}
