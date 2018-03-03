using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelNavigation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Initialize();
	}

	public void ReloadNavigation()
	{
		var buttons = GameObject.FindGameObjectsWithTag(c_navButtonTag);

		foreach(GameObject button in buttons)
		{
			Destroy(button);
		}

		Initialize();
	}

	private void Initialize()
	{
		DirectoryInfo directory = new DirectoryInfo(c_filePath);
		FileInfo[] infos = directory.GetFiles("*.prefab");
		float x = -175;
		float y = 150;
		Dictionary<string, int> saveData = SaveManager.LoadData();
		foreach (FileInfo info in infos)
		{
			string level = info.Name.Split('.')[0];
			var newButton = Instantiate(buttonTemplate, new Vector3(x, y, 0), Quaternion.identity);
			newButton.transform.SetParent(gameObject.transform.GetChild(0));
			newButton.transform.GetChild(0).GetComponent<Text>().text = level;
			newButton.transform.localScale = new Vector3(1f, 1f, 1f);
			newButton.transform.localEulerAngles = new Vector3(0, 0, 0);
			newButton.transform.localPosition = new Vector3(x, y, 0);
			newButton.GetComponent<Button>().onClick.AddListener(delegate { ChangeLevel(level); });
			if (saveData.ContainsKey(level))
			{
				int score = saveData[level];
				if (score > 0 && score < 4)
				{
					int starX = -12;
					for (int i = 0; i < score; i++)
					{
						var star = Instantiate(starTemplate, new Vector3(0, 0, 0), Quaternion.identity);
						star.transform.SetParent(newButton.transform);
						star.transform.localScale = new Vector3(5, .01f, 6.6f);
						star.transform.localEulerAngles = new Vector3(-90, 0, 0);
						star.transform.localPosition = new Vector3(starX, -12, 0);
						starX += 12;
					}
				}else if(score > 3)
				{
					var star = Instantiate(superStarTemplate, new Vector3(0, 0, 0), Quaternion.identity);
					star.transform.SetParent(newButton.transform);
					star.transform.localScale = new Vector3(10, .01f, 13.2f);
					star.transform.localEulerAngles = new Vector3(-90, 0, 0);
					star.transform.localPosition = new Vector3(0, -6, 0);
				}
			}

			x += 70f;
			if (x > 175)
			{
				x = -175;
				y -= 70;
			}
		}
	}

	public void ChangeLevel(string levelName)
	{
		ToggleIsDeleting(false);
		GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + levelName + ".prefab");
		var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
		var objective = GameObject.FindGameObjectWithTag(c_objectiveTag);
		objective.GetComponent<Objective>().DeleteTower();
		GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
		newLevel.transform.localScale = currentLevel.transform.localScale;
		RotateObject.Objective = GameObject.FindGameObjectWithTag("Objective");
	}

	public void DeleteScores()
	{
		if (isDeleting)
		{
			SaveManager.DeleteData();
			ReloadNavigation();
			ToggleIsDeleting(false);
		}
		else
		{
			ToggleIsDeleting(true);
		}
	}

	public void ToggleIsDeleting(bool isOn)
	{
		if (isOn)
		{
			GameObject.FindGameObjectWithTag(c_deleteButtonTag).transform.GetChild(0).GetComponent<Text>().text = "Are you sure?";
			isDeleting = true;
		}
		else
		{
			GameObject.FindGameObjectWithTag(c_deleteButtonTag).transform.GetChild(0).GetComponent<Text>().text = "Delete Scores";
			isDeleting = false;
		}
	}

	const string c_filePath = "Assets/Levels/";
	const string c_objectiveTag = "Objective";
	const string c_levelTag = "Level";
	const string c_navButtonTag = "NavButton";
	const string c_deleteButtonTag = "DeleteButton";

	public bool isDeleting = false;

	[SerializeField]
	GameObject buttonTemplate;
	
	[SerializeField]
	GameObject starTemplate;

	[SerializeField]
	GameObject superStarTemplate;
}
