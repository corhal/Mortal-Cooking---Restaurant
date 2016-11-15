using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipesWindow : MonoBehaviour {

	public GameObject RecipePrefab;
	public GameObject RecipeParent;
	public RecipeUIElement[] recipeUIElements;

	void Awake() {
		Restaurant.OnRestaurantInfoChanged += Restaurant_OnRestaurantInfoChanged;
	}

	void Restaurant_OnRestaurantInfoChanged () {
		UpdateWindow ();
	}

	void Start() {
		recipeUIElements = new RecipeUIElement[Restaurant.instance.DishRecipes.Length];
		Debug.Log ("In restaurant: " + Restaurant.instance.DishRecipes.Length);
		for (int i = 0; i < Restaurant.instance.DishRecipes.Length; i++) {
			GameObject newRecipe = Instantiate (RecipePrefab, RecipeParent.transform) as GameObject;
			newRecipe.transform.localPosition = new Vector3 (newRecipe.transform.localPosition.x, newRecipe.transform.localPosition.y, 0);
			newRecipe.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			recipeUIElements [i] = newRecipe.GetComponent<RecipeUIElement> ();
			recipeUIElements [i].dishRecipe = Restaurant.instance.DishRecipes [i];
		}
		UpdateWindow ();
	}

	public void UpdateWindow() {
		foreach (var recipeUIElement in recipeUIElements) {
			recipeUIElement.UpdateElement ();
		}
	}

	void OnDestroy() {
		Restaurant.OnRestaurantInfoChanged -= Restaurant_OnRestaurantInfoChanged;
	}
}
