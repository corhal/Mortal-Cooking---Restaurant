using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopWindow : MonoBehaviour {

	public GameObject BuildingNodePrefab;
	public GameObject NodeParent;

	Storage storage;
	public BuildingNode[] BuildingNodes;
	//public int[] Costs;
	public int[] CurrentBuildings;
	public int[] MaxBuildings;

	void Awake() {
		storage = Player.instance.gameObject.GetComponent<Storage> ();
		BuildingNodes = new BuildingNode[storage.FurnitureSprites.Length];
		for (int i = 0; i < storage.FurnitureSprites.Length; i++) {
			GameObject node = Instantiate (BuildingNodePrefab, NodeParent.transform) as GameObject;
			node.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			BuildingNodes [i] = node.GetComponent<BuildingNode> ();
			BuildingNodes [i].BuildButton.GetComponent<Button>().onClick.RemoveAllListeners ();

			int index = i;
			BuildingNodes [index].BuildButton.GetComponent<Button>().onClick.AddListener (delegate {				
				Restaurant.instance.Build(index);
			});

			BuildingNodes [i].BuildButton.GetComponent<Button>().onClick.AddListener (delegate {				
				Restaurant.instance.Toggle(this.gameObject);
			});

			BuildingNodes [i].MyBuilding = Builder.SBuildingPrefabs [i].GetComponent<Building> ();
			Debug.Log("Cost of building: " + BuildingNodes[i].MyBuilding.Cost);
		}
		//Costs = new int[Restaurant.instance.Costs.Length];
		//Restaurant.instance.Costs.CopyTo (Costs, 0);
		UpdateWindow ();
	}

	public void UpdateWindow() {
		Debug.Log ("Updating window");
		for (int i = 0; i < storage.FurnitureSprites.Length; i++) {
			int cost = BuildingNodes [i].MyBuilding.InitialCost; // Временно, потому что мы обращаемся к хранилищу префабов
			int prestige = BuildingNodes [i].MyBuilding.InitialPrestige;
			string costString = cost.ToString ();
			if (cost > 1000) {
				costString = (cost / 1000) + "k";
			}
			BuildingNodes [i].BuildButton.GetComponentInChildren<Text> ().text = costString;
			BuildingNodes [i].BuildingImage.sprite = storage.FurnitureSprites [i];
			BuildingNodes [i].BuildingImage.SetNativeSize ();
			BuildingNodes [i].RewardText.text = "+" + prestige;
			BuildingNodes [i].CountText.text = Restaurant.instance.CurrentBuildings [i] + "/" + BuildingNodes [i].MyBuilding.InitialCountLimitByPrestigeLevel[Restaurant.instance.PrestigeLevel - 1];
			if (Restaurant.instance.CurrentBuildings[i] >= BuildingNodes [i].MyBuilding.InitialCountLimitByPrestigeLevel[Restaurant.instance.PrestigeLevel - 1]) {
				BuildingNodes [i].BuildButton.SetActive (false);
			} else {
				BuildingNodes [i].BuildButton.SetActive (true);
			}
		}
	}

	void OnDestroy() {
		for (int i = 0; i < storage.FurnitureSprites.Length; i++) {
			BuildingNodes [i].BuildButton.GetComponent<Button>().onClick.RemoveAllListeners ();
		}
	}
}
