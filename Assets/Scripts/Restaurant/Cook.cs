using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cook : MonoBehaviour {

	public int Id;

	public int[] DishLengthByLevel;
	public int[] Dishes;
	public CookData cookData;

	public int CurrentDish;
	public int CurrentDishIndex;
	public Client currentClient;

	Restaurant restaurant;
	public GameObject FlyingTextPrefab;

	public GameObject GoldBubble;
	public bool RaidReady;

	public float CookPeriod;
	public int CookPower;
	float timer;

	public bool Selected;
	public bool IsCooking;

	public int Level;

	public int[,] RangeGoldPerClientByLevel;

	public int TypeId;

	public int ItemCollectionLength; // Я об этом очень пожалею. Это длина отрезков, на которые нужно резать следующий массив.
	public int[] ItemCollections; // Могут подряд идти несколько одинаковых итемов

	public delegate void ShowInfoEventHandler (Cook cook);
	public static event ShowInfoEventHandler OnInfoShow;

	public delegate void FreeNowEventHandler (Cook cook);
	public static event FreeNowEventHandler OnCookFreeNow;

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

		line = gameObject.GetComponent<LineRenderer> ();
		pointsList = new List<Vector3> ();
		Client.OnClientDied += Client_OnClientDied;
	}

	void Client_OnClientDied (Client client) {		
		if (currentClient != null) {
			currentClient.OnDishReady -= Client_OnDishReady;
		}
		currentClient = null;
		CurrentDish = -1;
		CurrentDishIndex = -1;
		IsCooking = false;
		DrawLine (gameObject.transform.position);
		OnCookFreeNow (this);
	}

	void Start() {
		myUI.UpdateLabels ();
	}

	public bool CheckItems(int[] itemCounts) { // здесь хранятся КОЛИЧЕСТВА предметов по ИНДЕКСУ		
		int[] itemCollection = CurrentCollection ();

		if (itemCollection != null) {
			int count = 0;
			foreach (var item in itemCollection) { // Пока предметы не могут повторяться(
				if (itemCounts[item] > 0) {
					count++;
				}
			}
			if (count >= ItemCollectionLength) {
				return true;
			}
		}
		return false;
	}

	public void LevelUp() {
		Level++;
		myUI.UpdateLabels ();
	}

	public int[] CurrentCollection () {
		if (Level < 5) {
			int curLevel = Level - 1;
			int startIndex = curLevel * ItemCollectionLength;
			int[] itemCollection = new int[ItemCollectionLength]; // здесь хранятся ИНДЕКСЫ предметов
			for (int i = 0; i < itemCollection.Length; i++) {
				itemCollection [i] = ItemCollections [startIndex];
				startIndex++;
			}
			return itemCollection;
		}
		return null;
	}

	public void ChangeClient(Client client) {
		timer = 0.0f;
		client.ChangeCook (this);
		currentClient = client;
		DrawLine (client.gameObject.transform.position);
	}
	
	/*public void CheckoutGhostClient(GhostClient ghostClient) {
		int income = 0;
		int rewardsCount = 3;

		for (int i = 0; i < rewardsCount; i++) {
			int addition = 0;
			addition = Restaurant.instance.DishCosts [ghostClient.Dishes [i]];

			foreach (var dish in Dishes) {
				if (ghostClient.Dishes [i] == dish) {					
					addition *= 2;
				}
			}
			income += addition;
		}
		//AddGold (income, false);
		Debug.Log("Checking out GHOST client, adding " + income + " gold");
		Debug.Log ("GHOST client readiness was " + currentClient.Readiness);
	}*/	

	void Update() {
		timer += Time.deltaTime;
		if (IsCooking && currentClient != null && timer >= CookPeriod) {
			ServeClient ();
			timer = 0.0f;
		}
	}

	public void ServeClient() {
		if (currentClient != null) {
			currentClient.AddReadiness (CurrentDish, CookPower);
		}
	}

	public void TakeOrder(Client client, int dish) {
		currentClient = client;
		//client.ChangeCook (this); // теперь у клиента может быть больше 1 повара!
		client.OnDishReady += Client_OnDishReady;
		CurrentDish = dish;
		CurrentDishIndex = System.Array.IndexOf (client.Dishes, CurrentDish);
		client.FreeDishes [CurrentDishIndex] = false;
		for (int i = 0; i < DishLengthByLevel[Level - 1]; i++) {
			if (CurrentDish == Dishes[i]) {				
				client.Crits [CurrentDishIndex] = true;
			}
		}
		DrawLine (client.gameObject.transform.position);
		IsCooking = true;
	}

	void Client_OnDishReady (int dish) {
		Debug.Log ("Dish ready");
		if (currentClient != null) {
			currentClient.OnDishReady -= Client_OnDishReady;
			currentClient.AddReadiness (1);
		}
		currentClient = null;
		CurrentDish = -1;
		CurrentDishIndex = -1;
		IsCooking = false;
		DrawLine (gameObject.transform.position);
		OnCookFreeNow (this);
	}

	public void ShowFlyingText(int gold) {
		GameObject flyingTextObject = Instantiate (FlyingTextPrefab, transform.position, transform.rotation) as GameObject;
		FlyingText flyingText = flyingTextObject.GetComponent<FlyingText> ();
		flyingText.myText.text = "+" + gold;
	}

	public void InitializeFromData(CookData data) {
		ItemCollections = new int[data.ItemCollections.Length];
		data.ItemCollections.CopyTo (ItemCollections, 0);

		TypeId = data.TypeId;
		Id = data.Id;
		transform.position = new Vector3(data.x, data.y, data.z);
		Level = data.Level;
		RaidReady = data.RaidReady;
		Dishes = new int[data.Dishes.Length];
		data.Dishes.CopyTo (Dishes, 0);

		if (Restaurant.instance.CurrentClients.Count > 0 && data.CurrentClientIndex >= 0 && data.CurrentClientIndex < Restaurant.instance.CurrentClients.Count) {		
			TakeOrder(Restaurant.instance.CurrentClients [data.CurrentClientIndex], data.CurrentDish);
			//currentClient = Restaurant.instance.CurrentClients [data.CurrentClientIndex];
		}
	}

	public void ShowInfo() {
		OnInfoShow (this);
	}

	LineRenderer line;
	public List<Vector3> pointsList;

	public void DrawLine(Vector3 endPoint) {
		line.SetVertexCount (0);
		pointsList.RemoveRange (0, pointsList.Count);
		line.SetColors (Color.blue, Color.blue);

		pointsList.Add (gameObject.transform.position);
		pointsList.Add (endPoint);

		Vector3[] vectors = new Vector3[pointsList.Count];
		vectors [0] = gameObject.transform.position;
		vectors [1] = endPoint;

		line.SetVertexCount (pointsList.Count);
		line.SetPositions (vectors);
	}

	void OnDestroy() {
		if (currentClient != null) {
			currentClient.OnDishReady -= Client_OnDishReady;
		}
		Client.OnClientDied -= Client_OnClientDied;
	}
}
