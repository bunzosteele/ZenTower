using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
	public static void SaveData(string levelName, int score)
	{
		Dictionary<string, int> parsedSave;
		if (File.Exists(Application.persistentDataPath + c_fileName))
		{
			parsedSave = LoadData();
			if (parsedSave.ContainsKey(levelName))
			{
				int previousScore = parsedSave[levelName];
				if(score > previousScore)
				{
					parsedSave[levelName] = score;
				}
			}
			else
			{
				parsedSave.Add(levelName, score);
			}
		}
		else
		{
			parsedSave = new Dictionary<string, int>();
			parsedSave.Add(levelName, score);
		}

		string jsonToWrite = JsonConvert.SerializeObject(parsedSave);
		File.WriteAllText(Application.persistentDataPath + c_fileName, jsonToWrite);
	}

	public static Dictionary<string, int> LoadData()
	{
		using (StreamReader r = new StreamReader(Application.persistentDataPath + c_fileName))
		{
			string savedata = r.ReadToEnd();
			return JsonConvert.DeserializeObject<Dictionary<string, int>>(savedata);
		}
	}

	public static void DeleteData()
	{
		string jsonToWrite = "{}";
		File.WriteAllText(Application.persistentDataPath + c_fileName, jsonToWrite);
	}

	private const string c_fileName = "saves.json";
}