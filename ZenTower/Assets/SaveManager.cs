using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
	public static void SaveData(string categoryName, string levelName, int score)
	{
		Dictionary<string, Dictionary<string, int>> parsedSave;
		if (File.Exists(Application.persistentDataPath + c_fileName))
		{
			parsedSave = LoadData();
			if (parsedSave.ContainsKey(categoryName) && parsedSave[categoryName].ContainsKey(levelName))
			{
				int previousScore = parsedSave[categoryName][levelName];
				if(score > previousScore)
				{
					parsedSave[categoryName][levelName] = score;
				}
			}
			else
			{
				if (parsedSave.ContainsKey(categoryName))
				{
					parsedSave[categoryName].Add(levelName, score);
				}
				else
				{
					var newSave = new Dictionary<string, int>();
					newSave.Add(levelName, score);
					parsedSave.Add(categoryName, newSave);
				}
			}
		}
		else
		{
			var newSave = new Dictionary<string, int>();
			newSave.Add(levelName, score);

			parsedSave = new Dictionary<string, Dictionary<string, int>>();
			parsedSave.Add(categoryName, newSave);
		}

		string jsonToWrite = JsonConvert.SerializeObject(parsedSave);
		File.WriteAllText(Application.persistentDataPath + c_fileName, jsonToWrite);
	}

	public static Dictionary<string, Dictionary<string, int>> LoadData()
	{
		if (!File.Exists(Application.persistentDataPath + c_fileName))
			return null;
		using (StreamReader r = new StreamReader(Application.persistentDataPath + c_fileName))
		{
			string savedata = r.ReadToEnd();
			return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(savedata);
		}
	}

	public static void DeleteData()
	{
		string jsonToWrite = "{}";
		File.WriteAllText(Application.persistentDataPath + c_fileName, jsonToWrite);
	}

	private const string c_fileName = "saves.json";
}