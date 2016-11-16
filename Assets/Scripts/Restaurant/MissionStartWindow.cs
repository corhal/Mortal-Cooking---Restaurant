using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionStartWindow : MonoBehaviour {

	int maxRecipeStars = 5;
	public MissionData Mission;
	public GameObject ContentContainer; // =\
	public Text[] ClientDishes;
	public Text[] GoldRewards;
	public Text DropText;

	public Image[] RecipeStars;

	Image[] Stars(int index) {
		Image[] stars = new Image[maxRecipeStars];
		for (int i = 0; i < maxRecipeStars; i++) {
			stars [i] = RecipeStars [i + index * maxRecipeStars];
		}
		return stars;
	}

	public void OpenWindow (MissionData missionData) {
		ContentContainer.SetActive (true);
		Mission = missionData;
		for (int i = 0; i < ClientDishes.Length; i++) {
			int dish = missionData.Dishes [i];
			ClientDishes [i].text = dish.ToString ();
			Image[] stars = Stars (i);
			DishRecipe dishRecipe = Restaurant.instance.DishRecipes [dish];
			foreach (var star in stars) {
				star.sprite = Player.instance.gameObject.GetComponent<Storage> ().StarSprites [0];
			}
			for (int j = 0; j < dishRecipe.Level; j++) {
				stars [j].sprite = Player.instance.gameObject.GetComponent<Storage> ().StarSprites [1];
			}
			int cost = dishRecipe.CostsByLevel [dishRecipe.Level - 1];
			int additionalCost = cost - dishRecipe.CostsByLevel [0];
			string dishCostString = "$" + cost;
			if (additionalCost > 0) {
				dishCostString += " (+$" + additionalCost + ")";
			} 
			GoldRewards [i].text = dishCostString;
		}
		string dropString = "Possible drop:\n";
		foreach (var item in missionData.ItemRewards) {
			dropString += "- " + Restaurant.instance.ItemNames [item] + "\n";
		}
		DropText.text = dropString;
	}

	public void Play() {
		Restaurant.instance.PlayMission (Mission);
	}

	public void Raid() {
		Restaurant.instance.RaidMission (Mission);
	}

}
