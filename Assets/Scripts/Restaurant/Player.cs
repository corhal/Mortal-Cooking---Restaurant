using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class Player : MonoBehaviour {

	public RestaurantData MyRestaurant;
	public PlayerData MyPlayer;

	//public bool ShouldRaidCook;
	public bool FirstTime = true;
	public bool HasWon;
	public int StarCount;
	public static Player instance;
	public MissionData MyMission;

	public int currentCookId;
	public int Soulstones;

	public int GoldFromMission;

	void Awake() {
		
		//SaveLoad.Delete ();
		//SaveLoad.Load ();

		if (instance == null) {
			//if not, set instance to this
			instance = this;
		}

		//If instance already exists and it's not this:
		else if (instance != this) {

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy (gameObject);  
		}

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
	}

	void Start() {		
		SaveLoad.Load ();
		Debug.Log (SaveLoad.savedData);
		if (SaveLoad.savedData != null) {				
			MyRestaurant = SaveLoad.savedData;
			Debug.Log (MyRestaurant.Session.ToString());

			Debug.Log (MyRestaurant.Session.ToString());
			if (Restaurant.instance.Prestige == 0) {
				//Restaurant.instance.InitializeFromData (MyRestaurant);
			}
		} else {
			MyRestaurant = new RestaurantData (0, 0, 1);
		}
	}

	public void SetMission(MissionData missionData) {
		currentCookId = missionData.GetComponentInParent<Cook> ().Id;
		Debug.Log (missionData.GetComponentInParent<Cook> ().Id);
		MyMission.InitializeFromMissionData (missionData);
	}

	public void Save() {
		MyRestaurant.InitializeFromRestaurant (Restaurant.instance);
		Debug.Log ("Date-time when player saves: " + MyRestaurant.LastTime);
		SaveLoad.Save ();
	}

	public void Load() {
		Debug.Log ("Player loads");
		SaveLoad.Load ();
		if (SaveLoad.savedData != null) {				
			MyRestaurant = SaveLoad.savedData;
		}
	}

	/*public void OnLevelWasLoaded() {

	}*/
}
