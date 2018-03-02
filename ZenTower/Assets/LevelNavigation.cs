using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelNavigation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DirectoryInfo directory = new DirectoryInfo(c_filePath);
		FileInfo[] infos = directory.GetFiles("*.prefab");
		float x = -175;
		float y = 150;
		foreach(FileInfo info in infos)
		{
			string level = info.Name.Split('.')[0];
			var newButton = Instantiate(buttonTemplate, new Vector3(x, y, 0), Quaternion.identity);
			newButton.transform.SetParent(gameObject.transform.GetChild(0));
			newButton.transform.GetChild(0).GetComponent<Text>().text = level;
			newButton.transform.localScale = new Vector3(1f, 1f, 1f);
			newButton.transform.localEulerAngles = new Vector3(0,0,0);
			newButton.transform.localPosition = new Vector3(x, y, 0);
			newButton.GetComponent<Button>().onClick.AddListener(delegate { ChangeLevel(level); });
			x += 70f;
			if(x > 175)
			{
				x = -175;
				y -= 70;
			}
		}
	}

	public void ChangeLevel(string levelName)
	{
		Debug.Log("Trying to load level.");
		GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + levelName + ".prefab");
		var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
		var objective = GameObject.FindGameObjectWithTag(c_objectiveTag);
		objective.GetComponent<Objective>().DeleteTower();
		GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
		newLevel.transform.localScale = currentLevel.transform.localScale;
		RotateObject.Objective = GameObject.FindGameObjectWithTag("Objective");
	}

	const string c_filePath = "Assets/Levels/";
	const string c_objectiveTag = "Objective";
	const string c_levelTag = "Level";

	[SerializeField]
	GameObject buttonTemplate;
}
