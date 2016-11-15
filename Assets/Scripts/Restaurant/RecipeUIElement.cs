using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RecipeUIElement : MonoBehaviour {

	public DishRecipe dishRecipe;

	public Text DishId;
	public Image[] StarImages;

	public Text[] Items;
	public GameObject UpButton;

	public void UpdateElement() {
		DishId.text = dishRecipe.Id.ToString ();
		int[] currentCollection = dishRecipe.CurrentCollection ();
		int count = 0;
		for (int i = 0; i < currentCollection.Length; i++) {
			if (Restaurant.instance.ItemCounts[currentCollection[i]] > 0) {
				count++;
			}
			Items [i].text = currentCollection [i].ToString();
		}
		UpButton.SetActive (count >= currentCollection.Length);
		SetStars ();
	}

	public void SetStars() {
		foreach (var starImage in StarImages) {
			starImage.sprite = Player.instance.gameObject.GetComponent<Storage> ().StarSprites [0];
		}
		for (int i = 0; i < dishRecipe.Level; i++) {
			StarImages [i].sprite = Player.instance.gameObject.GetComponent<Storage> ().StarSprites [1];
		}
	}
}
