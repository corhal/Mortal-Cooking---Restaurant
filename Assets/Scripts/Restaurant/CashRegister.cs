using UnityEngine;
using System.Collections;

public class CashRegister : MonoBehaviour {

	public GameObject GoldBubble;
	public GameObject FlyingTextPrefab;

	public int Gold;
	public int GoldStorageLevel;
	public int[] MaxGoldByStorageLevel;

	public int[] StarRequirementsPerStorageLevel;

	public delegate void CollectGoldEventHandler (CashRegister cashRegister);
	public static event CollectGoldEventHandler OnCollectGold;

	public delegate void GoldChangedEventHandler ();
	public event GoldChangedEventHandler OnGoldChanged;

	public void CollectGold() {
		GoldBubble.SetActive (false);
		OnCollectGold (this);
		OnGoldChanged ();
	}

	public void LevelUpStorage() {
		if (GoldStorageLevel < 10 && Restaurant.instance.SpendStars(StarRequirementsPerStorageLevel[GoldStorageLevel-1])) {
			GoldStorageLevel++;
		}
	}

	public void TakeGold(int amount) {
		Gold = Mathf.Min (Gold + amount, MaxGoldByStorageLevel [GoldStorageLevel - 1]);
		if (Gold > MaxGoldByStorageLevel[GoldStorageLevel - 1] / 3) {
			GoldBubble.SetActive (true);
		}
		OnGoldChanged ();
	}

	public int GiveGold(int amount) {
		if (Gold - amount >= 0) {
			Gold -= amount;
			return amount;
		} else {
			int gold = Gold;
			Gold = 0;
			return gold;
		}
		OnGoldChanged ();
	}

	public void ShowFlyingText(int gold) {
		GameObject flyingTextObject = Instantiate (FlyingTextPrefab, transform.position, transform.rotation) as GameObject;
		FlyingText flyingText = flyingTextObject.GetComponent<FlyingText> ();
		flyingText.myText.text = "+" + gold;
	}

	public void InitializeFromData(CashRegisterData data) {		
		Gold = data.Gold;	
		GoldStorageLevel = data.GoldStorageLevel;

		if (Gold > MaxGoldByStorageLevel[GoldStorageLevel - 1] / 3) {
			GoldBubble.SetActive (true);
		}
		OnGoldChanged ();
	}
}
