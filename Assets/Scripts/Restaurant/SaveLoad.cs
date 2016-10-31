using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

	public static RestaurantData savedData;

	public static void Save() {
		savedData = Player.instance.MyRestaurant;
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd");
		bf.Serialize(file, SaveLoad.savedData);
		file.Close();
	}

	public static void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			savedData = (RestaurantData)bf.Deserialize(file);
			file.Close();
		}
	}

	public static void Delete() {
		savedData = null;
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			File.Delete(Application.persistentDataPath + "/savedGames.gd");
		}
	}
}
