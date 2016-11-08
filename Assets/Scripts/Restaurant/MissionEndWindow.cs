using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MissionEndWindow : MonoBehaviour {

	public Text DebriefingText;
	public GameObject ContentContainer; // =\

	void Awake() {		
		Restaurant.OnMissionEnded += Restaurant_OnMissionEnded;
	}

	void Restaurant_OnMissionEnded (int gold, IEnumerable<int> items) {
		UpdateInfo(gold, items);
	}

	public void UpdateInfo(int gold, IEnumerable<int> items) {
		ContentContainer.SetActive (true);
		string debriefing = "Your reward:\n";
		debriefing += "Gold: " + gold + "\n";
		foreach (var item in items) {
			debriefing += "-" + Restaurant.instance.ItemNames [item] + " +1 (" + Restaurant.instance.ItemCounts [item] + ")\n";
		}
		DebriefingText.text = debriefing;
	}

	void OnDestroy() {
		Restaurant.OnMissionEnded -= Restaurant_OnMissionEnded;
	}
}
