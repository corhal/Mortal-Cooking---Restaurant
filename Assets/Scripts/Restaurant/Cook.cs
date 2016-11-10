using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cook : MonoBehaviour {

	public int Id;

	public int[] Dishes;
	public CookData cookData;

	Restaurant restaurant;
	public GameObject FlyingTextPrefab;

	public GameObject GoldBubble;
	public bool RaidReady;

	public bool Selected;

	public int Level;
	public int GoldStorageLevel;
	public int[] StarRequirementsPerStorageLevel;
	//public int Soulstones;

	public int[,] RangeGoldPerClientByLevel;
	//public int[] SoulstoneRequirementsPerLevel;

	public int TypeId;

	public int Gold;
	public int[] MaxGoldByStorageLevel;

	public int ItemCollectionLength; // Я об этом очень пожалею. Это длина отрезков, на которые нужно резать следующий массив.
	public int[] ItemCollections; // Могут подряд идти несколько одинаковых итемов

	public delegate void CollectGoldEventHandler (Cook cook);
	public static event CollectGoldEventHandler OnCollectGold;

	public delegate void ShowInfoEventHandler (Cook cook);
	public static event ShowInfoEventHandler OnInfoShow;

	CookUI myUI;

	void Awake() {
		restaurant = GameObject.FindObjectOfType<Restaurant> ();
		RangeGoldPerClientByLevel = new int[,] {
			{1, 3},
			{2, 4},
			{3, 5},
			{4, 6},
			{5, 7}
		};
		myUI = gameObject.GetComponent<CookUI>();
	}

	void Start() {
		myUI.UpdateLabels ();
	}

	public void CheckItems(int[] itemCounts) { // здесь хранятся КОЛИЧЕСТВА предметов по ИНДЕКСУ		
		int[] itemCollection = CurrentCollection ();

		int count = 0;
		foreach (var item in itemCollection) { // Пока предметы не могут повторяться(
			if (itemCounts[item] > 0) {
				count++;
			}
		}
		if (count >= ItemCollectionLength) {
			Level++;
			myUI.UpdateLabels ();
		}
	}

	public int[] CurrentCollection () {
		int curLevel = Level - 1;
		int startIndex = curLevel * ItemCollectionLength;
		int[] itemCollection = new int[ItemCollectionLength]; // здесь хранятся ИНДЕКСЫ предметов
		for (int i = 0; i < itemCollection.Length; i++) {
			itemCollection [i] = ItemCollections [startIndex];
			startIndex++;
		}
		return itemCollection;
	}

	public void CollectGold() {
		GoldBubble.SetActive (false);
		OnCollectGold (this);
	}

	public void GoldStorageLevelUp () {
		GoldStorageLevel++;
	}

	public int GenerateGold() {
		int gold = Random.Range (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1] + 1);		
		return gold;
	}

	public int GenerateGold(int dish) {
		int gold = Random.Range (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1] + 1);
		foreach (var myDish in Dishes) {
			if (myDish == dish) {
				gold *= 2;
			}
		}
		ShowFlyingText (gold);
		return gold;
	}

	public void GenerateGold(Client client, bool showText) {
		bool crit = false;
		int gold = Random.Range (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1] + 1);
		foreach (var myDish in Dishes) {
			if (myDish == client.Dish) {
				gold *= 2;
				crit = true;
			}
		}
		gold = client.GiveGold (gold);
		if (Gold < MaxGoldByStorageLevel[GoldStorageLevel - 1]) {
			if (showText) {
				GameObject flyingTextObject = Instantiate (FlyingTextPrefab, transform.position, transform.rotation) as GameObject;
				FlyingText flyingText = flyingTextObject.GetComponent<FlyingText> ();
				flyingText.myText.text = "+" + gold;
				if (crit) {
					flyingText.myText.color = Color.yellow;
				}
			}
		}

		Gold = Mathf.Min (Gold + gold, MaxGoldByStorageLevel[GoldStorageLevel - 1]);
		if (Gold > MaxGoldByStorageLevel[GoldStorageLevel - 1] / 3) {
			GoldBubble.SetActive (true);
		}
	}

	public void ShowFlyingText(int gold) {
		GameObject flyingTextObject = Instantiate (FlyingTextPrefab, transform.position, transform.rotation) as GameObject;
		FlyingText flyingText = flyingTextObject.GetComponent<FlyingText> ();
		flyingText.myText.text = "+" + gold;
	}

	public void GenerateGoldPerClients(int clients, int[] restaurantDishes) {	
		int dishCount = 0;

		foreach (var myDish in Dishes) {
			foreach (var dish in restaurantDishes) {
				if (myDish == dish) {
					dishCount++;
				}
			}
		}
		float critChance = (float)dishCount / (float)restaurantDishes.Length;

		int goldPerClient = (int)Utility.MathExpectation (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1] + 1);
		goldPerClient += (int)((float)goldPerClient * critChance);

		int gold = goldPerClient * clients;
		Gold = Mathf.Min (Gold + gold, MaxGoldByStorageLevel[GoldStorageLevel - 1]);
		if (Gold > MaxGoldByStorageLevel[GoldStorageLevel - 1] / 3) {
			GoldBubble.SetActive (true);
		}	
	}

	public void InitializeFromData(CookData data) {
		ItemCollections = new int[data.ItemCollections.Length];
		data.ItemCollections.CopyTo (ItemCollections, 0);

		TypeId = data.TypeId;
		Id = data.Id;
		transform.position = new Vector3(data.x, data.y, data.z);
		Level = data.Level;
		GoldStorageLevel = data.GoldStorageLevel;
		//Soulstones = data.Soulstones;
		RaidReady = data.RaidReady;
		Dishes = new int[data.Dishes.Length];
		data.Dishes.CopyTo (Dishes, 0);
		Gold = data.Gold;
	}

	public void ShowInfo() {
		OnInfoShow (this);
	}
}
