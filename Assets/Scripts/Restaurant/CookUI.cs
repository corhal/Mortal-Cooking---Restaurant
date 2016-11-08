using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CookUI : MonoBehaviour {

	public Text LevelLabel;
	public Text ItemsLabel;

	public Text LevelText;
	public Text DishesText;
	public Text GoldText;

	Cook cook;

	void Awake() {
		cook = gameObject.GetComponent<Cook> ();
	}

	public void UpdateLabels() {
		LevelLabel.text = cook.Level.ToString ();
	}
}
