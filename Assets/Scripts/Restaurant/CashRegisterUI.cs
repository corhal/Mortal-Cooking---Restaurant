using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CashRegisterUI : MonoBehaviour {

	public Text InfoText;
	CashRegister cashRegister;

	void Awake() {
		cashRegister = gameObject.GetComponent<CashRegister> ();
		cashRegister.OnGoldChanged += CashRegister_OnGoldChanged;
	}

	void Start() {
		UpdateWindow ();
	}

	void CashRegister_OnGoldChanged () {
		UpdateWindow ();
	}

	public void UpdateWindow() {
		string info = "Level: " + cashRegister.GoldStorageLevel + "\n";
		info += "Gold: " + cashRegister.Gold + "/" + cashRegister.MaxGoldByStorageLevel [cashRegister.GoldStorageLevel - 1] + "\n";
		info += "Stars: " + Restaurant.instance.Stars + "/" + cashRegister.StarRequirementsPerStorageLevel [cashRegister.GoldStorageLevel - 1] +  "\n";
		InfoText.text = info;
	}

	void OnDestroy() {
		cashRegister.OnGoldChanged -= CashRegister_OnGoldChanged;
	}
}
