using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelNavigation : MonoBehaviour {

	void Start () {
		LoadNextLevel();
	}

	private void Update()
	{
		var level = GameObject.FindGameObjectWithTag(c_levelTag);

		if (level == null)
			return;

		string currentLevel = level.name.Split('(')[0];

		var buttons = GameObject.FindGameObjectsWithTag(c_navButtonTag);
		string category = GameObject.FindGameObjectWithTag("Category").GetComponent<Text>().text;
		foreach (GameObject button in buttons)
		{
			if (button.transform.GetChild(0).GetComponent<Text>().text.Equals(currentLevel) && category.Equals(CurrentCategory))
				button.GetComponent<Image>().color = new Color(.5f, 1f, .5f, 1);
			else
				button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		}
	}

	public void ReloadNavigation(bool shouldPause = true)
	{
		var buttons = GameObject.FindGameObjectsWithTag(c_navButtonTag);

		foreach(GameObject button in buttons)
		{
			button.GetComponent<Button>().onClick.RemoveAllListeners();
			Destroy(button);
		}
		Initialize(shouldPause);
	}

	private void Initialize(bool shouldPause)
	{
		DirectoryInfo parentDirectory = new DirectoryInfo(c_filePath);
		DirectoryInfo directory = parentDirectory.GetDirectories()[m_pageId];
		var category = GameObject.FindGameObjectWithTag("Category");
		category.GetComponent<Text>().text = directory.Name;
		List<FileInfo> infos = new List<FileInfo>(directory.GetFiles("*.prefab"));
		infos.Sort(new FileComparer());
		float x = -120;
		float y = 120;
		Dictionary<string, Dictionary<string, int>> saveData = SaveManager.LoadData();
		bool isBeat = true;
		foreach (FileInfo info in infos)
		{
			string level = info.Name.Split('.')[0];
			string currentLevel = GameObject.FindGameObjectWithTag(c_levelTag).name.Split('(')[0];
			var newButton = Instantiate(buttonTemplate, new Vector3(x, y, 0), Quaternion.identity);
			newButton.transform.SetParent(gameObject.transform.GetChild(0));
			newButton.transform.GetChild(0).GetComponent<Text>().text = level;
			newButton.transform.GetChild(0).GetComponent<Text>().fontSize = 28;
			newButton.transform.localScale = new Vector3(1f, 1f, 1f);
			newButton.transform.localEulerAngles = new Vector3(0, 0, 0);
			newButton.transform.localPosition = new Vector3(x, y, 0);
			newButton.GetComponent<Button>().onClick.RemoveAllListeners();
			newButton.GetComponent<Button>().onClick.AddListener(delegate { ChangeLevel(category.GetComponent<Text>().text, level); });
			if(!isBeat)
				newButton.GetComponent<Button>().interactable = false;
			if (saveData != null && saveData.ContainsKey(directory.Name) && saveData[directory.Name].ContainsKey(level))
			{
				int score = saveData[directory.Name][level];
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
			if (x >= 180)
			{
				x = -120;
				y -= 60;
			}
		}
		int count = 0;
		if (saveData != null)
			count = saveData.Values.Select(cat => cat.Values.Sum()).Sum();

		var countStar = Instantiate(starTemplate, new Vector3(0, 0, 0), Quaternion.identity);
		var countStarRenderer = countStar.GetComponent<Renderer>();
		countStarRenderer.material = Resources.Load("Gold", typeof(Material)) as Material;
		countStar.transform.SetParent(GameObject.FindGameObjectWithTag("Menu").transform);
		countStar.transform.localScale = new Vector3(7.5f, .01f, 9.9f);
		countStar.transform.localEulerAngles = new Vector3(-90, 0, 0);
		countStar.transform.localPosition = new Vector3(160, 162.5f, 0);

		var countText = GameObject.FindGameObjectWithTag("StarCount").GetComponent<Text>();
		countText.text = count.ToString();

		if(m_pageId > 0)
		{
			var newButton = Instantiate(buttonTemplate, new Vector3(-190, 35, 0), Quaternion.identity);
			newButton.transform.SetParent(gameObject.transform.GetChild(0));
			newButton.transform.GetChild(0).GetComponent<Text>().text = "<";
			newButton.transform.GetChild(0).GetComponent<Text>().fontSize = 28;
			newButton.transform.GetChild(0).GetComponent<Text>().transform.localScale = new Vector3(1, 1.5f, 1);
			newButton.transform.GetChild(0).GetComponent<Text>().resizeTextForBestFit = true;
			newButton.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			newButton.transform.localEulerAngles = new Vector3(0, 0, 0);
			newButton.transform.localPosition = new Vector3(-190, 35, 0);
			newButton.GetComponent<Button>().onClick.RemoveAllListeners();
			newButton.GetComponent<Button>().onClick.AddListener(delegate { PageDown(); });
		}

		if (m_pageId < parentDirectory.GetDirectories().Length - 1)
		{
			var newButton = Instantiate(buttonTemplate, new Vector3(190, 35, 0), Quaternion.identity);
			newButton.transform.SetParent(gameObject.transform.GetChild(0));
			newButton.transform.GetChild(0).GetComponent<Text>().text = ">";
			newButton.transform.GetChild(0).GetComponent<Text>().fontSize = 28;
			newButton.transform.GetChild(0).GetComponent<Text>().transform.localScale = new Vector3(1, 1.5f, 1);
			newButton.transform.GetChild(0).GetComponent<Text>().resizeTextForBestFit = true;
			newButton.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			newButton.transform.localEulerAngles = new Vector3(0, 0, 0);
			newButton.transform.localPosition = new Vector3(190, 35, 0);
			newButton.GetComponent<Button>().onClick.RemoveAllListeners();
			newButton.GetComponent<Button>().onClick.AddListener(delegate { PageUp(); });
			newButton.GetComponent<Button>().interactable = false;
			var requirementText = GameObject.FindGameObjectWithTag("Requirement").GetComponent<Text>();
			requirementText.text = "Unlocks at\n " + (m_pageId + 1) * c_startRequirement + "   s";
			var star = Instantiate(starTemplate, new Vector3(0, 0, 0), Quaternion.identity);
			star.transform.SetParent(GameObject.FindGameObjectWithTag("Requirement").transform);
			star.transform.localScale = new Vector3(5, .01f, 6.6f);
			star.transform.localEulerAngles = new Vector3(-90, 0, 0);
			star.transform.localPosition = new Vector3(7.5f, -12.5f, 0);
			var renderer = star.GetComponent<Renderer>();
			renderer.material = Resources.Load("Gold", typeof(Material)) as Material;

			if (count >= (m_pageId + 1) * c_startRequirement)
			{
				GameObject.FindGameObjectWithTag("Requirement").transform.localPosition = new Vector3(190, 100 - 900, 0);
				newButton.GetComponent<Button>().interactable = true;
			}
			else
			{
				GameObject.FindGameObjectWithTag("Requirement").transform.localPosition = new Vector3(190, 100, 0);
			}
		}
		if(shouldPause)
			PauseNavigation();
	}

	public void LoadNextLevel()
	{
		DirectoryInfo parentDirectory = new DirectoryInfo(c_filePath);
		Dictionary<string, Dictionary<string, int>> saveData = SaveManager.LoadData();
		int pageId = m_pageId;
		string lowestCategory = null;
		string lowestLevel = null;
		int lowestScore = 4;
		int lowestPageId = 0;
		bool levelLoaded = false;
		for(int i = pageId; i < parentDirectory.GetDirectories().Length; i++)
		{
			var directory = parentDirectory.GetDirectories()[i];

			if (saveData == null)
			{
				LoadLevel(directory.Name, "1");
				levelLoaded = true;
				break;
			}

			if (saveData.Values.Select(cat => cat.Values.Sum()).Sum() < pageId * c_startRequirement)
				break;

			pageId++;

			if (!saveData.ContainsKey(directory.Name))
			{
				LoadLevel(directory.Name, "1");
				levelLoaded = true;
				break;
			}

			var categoryScores = saveData[directory.Name];
			List<FileInfo> files = new List<FileInfo>(directory.GetFiles("*.prefab"));
			files.Sort(new FileComparer());
			foreach (FileInfo file in files)
			{
				string fileName = file.Name.Split('.')[0];
				if(categoryScores == null || !categoryScores.ContainsKey(fileName))
				{
					LoadLevel(directory.Name, fileName);
					levelLoaded = true;
					break;
				}

				if(categoryScores[fileName] < lowestScore)
				{
					lowestCategory = directory.Name;
					lowestLevel = fileName;
					lowestScore = categoryScores[fileName];
					lowestPageId = pageId - 1;
				}
			}

			if (levelLoaded)
				break;
		}

		if (!levelLoaded && lowestCategory != null && lowestLevel != null)
		{
			LoadLevel(lowestCategory, lowestLevel);
			m_pageId = lowestPageId;
		}
		else
		{
			m_pageId = pageId - 1;
		}
		if (m_pageId < 0)
			m_pageId = 0;
		CurrentCategory = parentDirectory.GetDirectories()[m_pageId].Name;
		ReloadNavigation();
	}

	public void ChangeLevel(string categoryName, string levelName)
	{
		if (MenuToggle.isMenuOpen)
		{
			CompleteTutorials();
			var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
			currentLevel.GetComponent<StarManager>().DeleteStars();
			ToggleIsDeleting(false);
			CurrentCategory = categoryName;
			GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + CurrentCategory + '/' + levelName + ".prefab");
			var objective = GameObject.FindGameObjectWithTag(c_objectiveTag);
			objective.GetComponent<Objective>().DeleteTower();
			GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
			newLevel.transform.localScale = currentLevel.transform.localScale;
			var newObjective = GameObject.FindGameObjectWithTag("Objective");
			RotateObject.Objective = newObjective;
			ReloadNavigation();
		}
	}

	private void LoadLevel(string categoryName, string levelName)
	{
		CurrentCategory = categoryName;
		var previousLevel = GameObject.FindGameObjectWithTag(c_levelTag);
		GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + CurrentCategory + "/" + levelName + ".prefab");
		GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);

		if(previousLevel != null)
			newLevel.transform.localScale = previousLevel.transform.localScale;

		var newObjective = GameObject.FindGameObjectWithTag("Objective");
		RotateObject.Objective = newObjective;
		CurrentCategory = categoryName;
	}

	public void PauseNavigation(float duration = 1.55f)
	{
		var panel = GameObject.FindGameObjectWithTag(c_menuTag).transform.GetChild(0);
		List<GameObject> pausedButtons = new List<GameObject>();
		foreach (Transform menuItem in panel.transform)
		{
			var button = menuItem.gameObject.GetComponent<Button>();
			if (button == null)
				continue;

			if (button.GetComponent<Button>().interactable)
			{
				button.GetComponent<Button>().interactable = false;
				pausedButtons.Add(menuItem.gameObject);
			}
		}

		m_pausedButtons = pausedButtons;
		Invoke("EnableNavigation", duration);
	}

	private void EnableNavigation()
	{
		if (m_pausedButtons == null || !m_pausedButtons.Any())
			return;

		foreach (GameObject button in m_pausedButtons)
		{
			if(button != null)
				button.GetComponent<Button>().interactable = true;
		}
	}

	public void DeleteScores()
	{
		if (MenuToggle.isMenuOpen)
		{
			if (isDeleting)
			{
				SaveManager.DeleteData();
				m_pageId = 0;
				CurrentCategory = "4x4";
				ChangeLevel("4x4", "1");
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
		if (level == null)
			return;

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

	private void PageUp()
	{
		m_pageId++;
		DirectoryInfo parentDirectory = new DirectoryInfo(c_filePath);
		DirectoryInfo directory = parentDirectory.GetDirectories()[m_pageId];
		ReloadNavigation(false);
	}

	private void PageDown()
	{
		m_pageId--;
		DirectoryInfo parentDirectory = new DirectoryInfo(c_filePath);
		DirectoryInfo directory = parentDirectory.GetDirectories()[m_pageId];
		ReloadNavigation(false);
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
	const string c_menuTag = "Menu";
	const int c_startRequirement = 30;

	public bool isDeleting = false;
	private static int m_pageId = 0;

	[SerializeField]
	GameObject buttonTemplate;
	
	[SerializeField]
	GameObject starTemplate;

	[SerializeField]
	GameObject superStarTemplate;

	public static string CurrentCategory = "4x4";
	private List<GameObject> m_pausedButtons = new List<GameObject>();
}
