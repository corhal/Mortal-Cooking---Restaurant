using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RecipeUIElement : MonoBehaviour {

	public DishRecipe dishRecipe;

	public Text DishId;
	public Image[] StarImages;
	public Image[] ItemBackgrounds;

	public Text[] Items;
	public GameObject UpButton;

	public void UpdateElement() {
		DishId.text = dishRecipe.Id.ToString ();
		int[] tempRestaurantCollection = new int[Restaurant.instance.ItemCounts.Length];
		Restaurant.instance.ItemCounts.CopyTo (tempRestaurantCollection, 0);

		//Debug.Log ("DishRecipe " + dishRecipe.Id + " is calling current collection");
		int[] currentCollection = dishRecipe.CurrentCollection ();
		int count = 0;
		for (int i = 0; i < currentCollection.Length; i++) {
			if (tempRestaurantCollection[currentCollection[i]] > 0) {
				ItemBackgrounds [i].color = Color.green;
				count++;
				tempRestaurantCollection [currentCollection [i]]--;
			} else {
				ItemBackgrounds [i].color = Color.white;
			}
			Items [i].text = Restaurant.instance.ItemNames[currentCollection [i]];
		}
		UpButton.SetActive (count >= currentCollection.Length && dishRecipe.Level < dishRecipe.MaxLevel);
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

	public void LevelUp() {
		Restaurant.instance.LevelUpDish (dishRecipe);
	}
}
