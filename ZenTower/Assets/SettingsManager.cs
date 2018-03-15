using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public static class SettingsManager
{
	public static void SaveData(Settings settings)
	{
		Settings parsedSettings;
		if (File.Exists(Application.persistentDataPath + c_fileName))
		{
			parsedSettings = LoadData();
			if (settings.Scale.HasValue)
				parsedSettings.Scale = settings.Scale;
		}
		else
		{
			parsedSettings = settings;
		}

		string jsonToWrite = JsonConvert.SerializeObject(parsedSettings);
		File.WriteAllText(Application.persistentDataPath + c_fileName, jsonToWrite);

	}

	public static Settings LoadData()
	{
		try
		{
			using (StreamReader r = new StreamReader(Application.persistentDataPath + c_fileName))
			{
				string savedata = r.ReadToEnd();
				return JsonConvert.DeserializeObject<Settings>(savedata);
			}
		}catch(Exception e)
		{
			Debug.Log(e.Message);
		}
		return new Settings();
	}

	public static void DeleteData()
	{
		string jsonToWrite = "{}";
		File.WriteAllText(Application.persistentDataPath + c_fileName, jsonToWrite);
	}

	public class Settings
	{
		public float? Scale { get; set; }
	}

	private const string c_fileName = "settings.json";
}