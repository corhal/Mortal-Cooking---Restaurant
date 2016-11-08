using UnityEngine;
using System.Collections;

public class MissionData : MonoBehaviour {

	public int MinPointsPerAction;
	public int MaxPointPerAction;

	public float MinSpawnTimer;
	public float MaxSpawnTimer;

	public int GoldReward;
	public int[] StarGoals;

	public int[] IngredientCounts;

	public int[] ItemRewards;
	public float[] ItemChances;

	public string Variation;

	public void InitializeFromMissionData(MissionData missionData) {
		GoldReward = missionData.GoldReward;
		MinPointsPerAction = missionData.MinPointsPerAction;
		MaxPointPerAction = missionData.MaxPointPerAction;
		MinSpawnTimer = missionData.MinSpawnTimer;
		MaxSpawnTimer = missionData.MaxSpawnTimer;
		StarGoals = new int[missionData.StarGoals.Length];
		missionData.StarGoals.CopyTo (StarGoals, 0);
		IngredientCounts = new int[missionData.IngredientCounts.Length];
		missionData.IngredientCounts.CopyTo (IngredientCounts, 0);
		Variation = missionData.Variation;
		ItemRewards = new int[missionData.ItemRewards.Length];
		ItemChances = new float[ItemRewards.Length];
		missionData.ItemRewards.CopyTo(ItemRewards, 0);
		missionData.ItemChances.CopyTo(ItemChances, 0);
	}
}
