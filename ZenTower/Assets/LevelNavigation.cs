using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			button.GetComponent<Button>().onClick.RemoveAllListeners();
			Destroy(button);
		}

		Initialize();
	}

	private void Initialize()
	{
		DirectoryInfo directory = new DirectoryInfo(c_filePath);
		List<FileInfo> infos = new List<FileInfo>(directory.GetFiles("*.prefab"));
		infos.Sort(new FileComparer());
		float x = -145;
		float y = 120;
		Dictionary<string, int> saveData = SaveManager.LoadData();
		bool isBeat = true;
		foreach (FileInfo info in infos)
		{
			string level = info.Name.Split('.')[0];
			var newButton = Instantiate(buttonTemplate, new Vector3(x, y, 0), Quaternion.identity);
			newButton.transform.SetParent(gameObject.transform.GetChild(0));
			newButton.transform.GetChild(0).GetComponent<Text>().text = level;
			newButton.transform.GetChild(0).GetComponent<Text>().fontSize = 28;
			newButton.transform.localScale = new Vector3(1f, 1f, 1f);
			newButton.transform.localEulerAngles = new Vector3(0, 0, 0);
			newButton.transform.localPosition = new Vector3(x, y, 0);
			newButton.GetComponent<Button>().onClick.RemoveAllListeners();
			newButton.GetComponent<Button>().onClick.AddListener(delegate { ChangeLevel(level); });
			if(!isBeat)
				newButton.GetComponent<Button>().interactable = false;
			if (saveData.ContainsKey(level))
			{
				int score = saveData[level];
				if (score > 0 && score < 4)
				{
					int starX = -12;
					for (int i = 0; i < score; i++)
					{
						var star = Instantiate(starTemplate, new Vector3(0, 0, 0), Quaternion.identity);
						var renderer = star.GetComponent<Renderer>();
						renderer.material = Resources.Load("Gold", typeof(Material)) as Material;
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
					star.transform.localScale = new Vector3(7.5f, .01f, 9.9f);
					star.transform.localEulerAngles = new Vector3(-90, 0, 0);
					star.transform.localPosition = new Vector3(0, -10, 0);
				}
			}
			else
			{
				isBeat = false;
			}

			x += 60f;
			if (x > 175)
			{
				x = -145;
				y -= 60;
			}
		}
		int count = saveData.Values.Sum();

		var countStar = Instantiate(starTemplate, new Vector3(0, 0, 0), Quaternion.identity);
		var countStarRenderer = countStar.GetComponent<Renderer>();
		countStarRenderer.material = Resources.Load("Gold", typeof(Material)) as Material;
		countStar.transform.SetParent(GameObject.FindGameObjectWithTag("Menu").transform);
		countStar.transform.localScale = new Vector3(7.5f, .01f, 9.9f);
		countStar.transform.localEulerAngles = new Vector3(-90, 0, 0);
		countStar.transform.localPosition = new Vector3(160, 162.5f, 0);

		var countText = GameObject.FindGameObjectWithTag("StarCount").GetComponent<Text>();
		countText.text = count.ToString();
	}

	public void ChangeLevel(string levelName)
	{
		if (MenuToggle.isMenuOpen)
		{
			CompleteTutorials();
			ToggleIsDeleting(false);
			GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + levelName + ".prefab");
			var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
			var objective = GameObject.FindGameObjectWithTag(c_objectiveTag);
			objective.GetComponent<Objective>().DeleteTower();
			GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
			newLevel.transform.localScale = currentLevel.transform.localScale;
			var newObjective = GameObject.FindGameObjectWithTag("Objective");
			RotateObject.Objective = newObjective;
		}
	}

	public void DeleteScores()
	{
		if (MenuToggle.isMenuOpen)
		{
			if (isDeleting)
			{
				SaveManager.DeleteData();
				ChangeLevel("1");
				ReloadNavigation();
				ToggleIsDeleting(false);
			}
			else
			{
				ToggleIsDeleting(true);
			}
		}
	}

	private void CompleteTutorials()
	{
		var level = GameObject.FindGameObjectWithTag(c_levelTag);
		var tutorialOne = level.GetComponent<TutorialOne>();
		if (tutorialOne != null)
			tutorialOne.CompleteSecondStep();

		var tutorialTwo = level.GetComponent<TutorialTwo>();
		if (tutorialTwo != null)
			tutorialTwo.CompleteThirdStep();

		var tutorialThree = level.GetComponent<TutorialThree>();
		if (tutorialThree != null)
			tutorialThree.CompleteFirstStep();
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
			GameObject.FindGameObjectWithTag(c_deleteButtonTag).transform.GetChild(0).GetComponent<Text>().text = "Delete All Progress";
			isDeleting = false;
		}
	}

	private class FileComparer: Comparer<FileInfo>
	{
		public override int Compare(FileInfo x, FileInfo y)
		{
			var intX = int.Parse(x.Name.Split('.')[0]);
			var intY = int.Parse(y.Name.Split('.')[0]);
			return intX - intY;
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
