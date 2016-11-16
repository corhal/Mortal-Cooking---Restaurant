using UnityEngine;
using System.Collections;

[System.Serializable]
public class DishRecipe {

	int id;
	public int Id { get { return id; } }

	int level;
	public int Level { get { return level; } }

	int maxLevel;
	public int MaxLevel { get { return maxLevel; } }

	int cuisine;
	public int Cuisine { get { return cuisine; } }

	int[] costsByLevel;
	public int[] CostsByLevel { get { return costsByLevel; } }

	int itemCollectionLength = 6;

	int[] itemCollectionsByLevel;
	public int[] ItemCollectionsByLevel { get { return itemCollectionsByLevel; } }

	public DishRecipe (int id, int level, int maxLevel, int cuisine, int[] costsByLevel) {
		this.id = id;
		this.level = level;
		this.maxLevel = maxLevel;
		this.cuisine = cuisine;
		this.costsByLevel = new int[costsByLevel.Length];
		costsByLevel.CopyTo (this.costsByLevel, 0);
		itemCollectionsByLevel = new int[MaxLevel * itemCollectionLength];
		for (int i = 0; i < itemCollectionsByLevel.Length; i++) {
			int item = Random.Range (0, Restaurant.instance.Items.Length);
			itemCollectionsByLevel [i] = item;
		}
	}

	public int[] CurrentCollection () {
		int[] currentCollection = new int[itemCollectionLength];
		for (int i = 0; i < itemCollectionLength; i++) {
			/*Debug.Log ("Item collections by level length: " + itemCollectionsByLevel.Length);
			Debug.Log ("Level - 1: " + (Level - 1));
			Debug.Log ("Trying to access item " + (i + (Level - 1) * itemCollectionLength).ToString());*/
			currentCollection [i] = itemCollectionsByLevel [i + (Level - 1) * itemCollectionLength];
		}
		return currentCollection;
	}

	public void LevelUp () {
		level++;
	}
}
