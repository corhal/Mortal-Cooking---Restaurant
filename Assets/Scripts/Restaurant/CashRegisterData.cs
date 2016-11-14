using UnityEngine;
using System.Collections;

[System.Serializable]
public class CashRegisterData {

	public int Gold;
	public int GoldStorageLevel;

	public void InitializeFromCashRegister(CashRegister cashRegister) {
		Gold = cashRegister.Gold;
		GoldStorageLevel = cashRegister.GoldStorageLevel;
	}
}
