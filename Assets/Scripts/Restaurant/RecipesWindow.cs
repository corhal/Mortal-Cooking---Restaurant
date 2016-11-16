using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipesWindow : MonoBehaviour {

	public GameObject[] CuisineButtonObjects;
	public GameObject CurrentCuisineObject;

	public GameObject RecipePrefab;
	public GameObject RecipeParent;
	public GameObject[] RecipeObjects;
	public RecipeUIElement[] recipeUIElements;

	public void ChangeCuisine(GameObject cuisineObject) {
		CurrentCuisineObject.transform.SetAsFirstSibling ();
		CurrentCuisineObject = cuisineObject;
		CurrentCuisineObject.transform.SetAsLastSibling ();
		UpdateWindow ();
	}

	public void FilterRecipes(int cuisine) {
		int startIndex = cuisine * 8;
		DishRecipe[] cuisineDishRecipes = new DishRecipe[Restaurant.instance.DishesInCuisine];
		for (int i = 0; i < cuisineDishRecipes.Length; i++) {
			/*Debug.Log ("Start index: " + startIndex);
			Debug.Log ("Cuisine dishes length: " + cuisineDishRecipes.Length);
			Debug.Log ("Dishes in restaurant: " + Restaurant.instance.DishRecipes.Length);*/
			cuisineDishRecipes [i] = Restaurant.instance.DishRecipes [startIndex + i];
		}
		for (int i = 0; i < RecipeObjects.Length; i++) {			
			recipeUIElements [i] = RecipeObjects[i].GetComponent<RecipeUIElement> ();
			recipeUIElements [i].dishRecipe = cuisineDishRecipes [i];
		}
		foreach (var recipeUIElement in recipeUIElements) {
			recipeUIElement.UpdateElement ();
		}
	}

	void Awake() {
		Restaurant.OnRestaurantInfoChanged += Restaurant_OnRestaurantInfoChanged;
		CurrentCuisineObject = CuisineButtonObjects [0];
	}

	void Restaurant_OnRestaurantInfoChanged () {
		UpdateWindow ();
	}

	void Start() {
		recipeUIElements = new RecipeUIElement[Restaurant.instance.DishesInCuisine];
		RecipeObjects = new GameObject[recipeUIElements.Length];
		//Debug.Log ("In restaurant: " + Restaurant.instance.DishRecipes.Length);
		for (int i = 0; i <= Restaurant.instance.MaxCuisineByPrestigeLevel[Restaurant.instance.PrestigeLevel - 1]; i++) {
			CuisineButtonObjects [i].GetComponent<Button> ().interactable = true;
		}
		for (int i = 0; i < recipeUIElements.Length; i++) {
			GameObject newRecipe = Instantiate (RecipePrefab, RecipeParent.transform) as GameObject;
			newRecipe.transform.localPosition = new Vector3 (newRecipe.transform.localPosition.x, newRecipe.transform.localPosition.y, 0);
			newRecipe.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			RecipeObjects [i] = newRecipe;
			//Debug.Log ("Recipe object: " + RecipeObjects [i]);
			recipeUIElements [i] = RecipeObjects[i].GetComponent<RecipeUIElement> ();
		}
		//UpdateWindow ();
	}

	public void UpdateWindow() {
		for (int i = 0; i <= Restaurant.instance.MaxCuisineByPrestigeLevel[Restaurant.instance.PrestigeLevel - 1]; i++) {
			CuisineButtonObjects [i].GetComponent<Button> ().interactable = true;
		}
		FilterRecipes (System.Array.IndexOf(CuisineButtonObjects, CurrentCuisineObject));
	}

	void OnDestroy() {
		Restaurant.OnRestaurantInfoChanged -= Restaurant_OnRestaurantInfoChanged;
	}
}
