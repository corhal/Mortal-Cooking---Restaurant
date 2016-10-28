using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cook : MonoBehaviour {

	Text levelLabel;

	public int Level;
	public int Soulstones;

	public int[,] RangeGoldPerClientByLevel;
	public int[] SoulstoneRequirementsPerLevel;

	void Awake() {
		RangeGoldPerClientByLevel = new int[,] {
			{1, 3},
			{2, 4},
			{3, 5},
			{4, 6},
			{5, 7}
		};
		levelLabel = GetComponentInChildren<Text> ();
	}

	void OnMouseDown() {
		AddSoulstones (5);
	}

	public int GenerateGold() {
		return Random.Range (RangeGoldPerClientByLevel [Level - 1, 0], RangeGoldPerClientByLevel [Level - 1, 1]);
	}

	public void AddSoulstones(int amount) {
		if (Soulstones < SoulstoneRequirementsPerLevel[SoulstoneRequirementsPerLevel.Length - 1]) {
			Soulstones += amount;
			if (Level < SoulstoneRequirementsPerLevel.Length && Soulstones >= SoulstoneRequirementsPerLevel[Level - 1]) {
				Level++;
				levelLabel.text = Level.ToString ();
			}
		}
	}


}
