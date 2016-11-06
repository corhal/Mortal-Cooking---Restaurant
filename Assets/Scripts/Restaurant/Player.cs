using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class Player : MonoBehaviour {

	public RestaurantData MyRestaurant;
	public PlayerData MyPlayer;

	public bool FirstTime = true;
	public bool HasWon;
	public int StarCount;
	public static Player instance;
	public MissionData MyMission;

	public int currentCookId;
	public int Soulstones;

	public int GoldFromMission;

	void Awake() {
		if (instance == null) {			
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start() {		
		SaveLoad.Load ();
		if (SaveLoad.savedData != null) {				
			MyRestaurant = SaveLoad.savedData;
		} 
	}

	public void SetMission(MissionData missionData) {
		currentCookId = missionData.GetComponentInParent<Cook> ().Id;
		Debug.Log (missionData.GetComponentInParent<Cook> ().Id);
		MyMission.InitializeFromMissionData (missionData);
	}

	public void Save() {
		MyRestaurant.InitializeFromRestaurant (Restaurant.instance);	
		SaveLoad.Save ();
	}

	public void Load() {
		Debug.Log ("Player loads");
		SaveLoad.Load ();
		if (SaveLoad.savedData != null) {				
			MyRestaurant = SaveLoad.savedData;
		}
	}
}
