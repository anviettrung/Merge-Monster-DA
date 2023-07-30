using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AVT
{
	public class SaveLoad : Singleton<SaveLoad>
	{
		//public GameDatabase gameDB;
		public string fileName = "gamesave.gd";
		public GameSaveFile saveFile;
		public static GameSaveFile SaveFile => Instance.saveFile;

		public void Awake()
		{
			saveFile = new GameSaveFile();

			saveFile.CallFunction("InitBeforeLoad");
			if (PlayerPrefs.HasKey("old_player"))
			{
				saveFile.Load(fileName);
			}
			saveFile.CallFunction("InitAfterLoad");

			saveFile.Save(fileName);
			PlayerPrefs.SetInt("old_player", 1);
			PlayerPrefs.Save();

			DontDestroyOnLoad(Instance);
		}

		public void OnApplicationQuit()
		{
			saveFile.Save(fileName);
		}

		void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
				saveFile.Save(fileName);
		}
	}

	public partial class GameSaveFile 
	{

	}

	public static class GameSaveFileExtend
	{
		public static void CallFunction(this GameSaveFile saveFile, string funcName, object[] parameters = null)
		{
			MethodInfo method = saveFile.GetType().GetMethod(funcName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			method?.Invoke(saveFile, parameters);
		}

		public static void InitList<T>(this List<T> l, params T[] default_values)
		{
			if (l == null)
			{
				l = new List<T>();
			}

			for (int i = 0; i < default_values.Length; i++)
			{
				if (l.Contains(default_values[i]) == false)
					l.Add(default_values[i]);
			}
		}

		public static void Save(this GameSaveFile saveFile, string fileName)
		{
			BinaryFormatter bf = new BinaryFormatter();
			//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
			FileStream file = File.Create(Application.persistentDataPath + "/" + fileName); //you can call it anything you want
			string json = JsonUtility.ToJson(saveFile);
			bf.Serialize(file, json);
			file.Close();
		}

		public static bool Load(this GameSaveFile saveFile, string fileName)
		{
			if (File.Exists(Application.persistentDataPath + "/" + fileName))
			{
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
				string json = (string)bf.Deserialize(file);
				JsonUtility.FromJsonOverwrite(json, saveFile);
				file.Close();

				return true;
			}

			return false;
		}
	}
}