using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Objective : MonoBehaviour {

	void Start () {
		currentLevelName = gameObject.transform.parent.parent.gameObject.name.Split('(')[0];

		initialObjectivePosition = gameObject.transform.localPosition;

		var scale = SettingsManager.LoadData().Scale;
		if (scale.HasValue) {
			var tower = GameObject.FindGameObjectWithTag(c_levelTag);
			tower.transform.localScale = new Vector3(scale.Value, scale.Value, scale.Value);
			RotateObject.s_towerSize = c_defaultTowerSize * scale.Value;

			var floor = GameObject.FindGameObjectWithTag(c_floorTag);
			floor.transform.localScale = new Vector3(c_defaultFloorScale * scale.Value, c_defaultFloorScale * scale.Value, c_defaultFloorScale * scale.Value);
		}
	}

	void Update () {
		if(gameObject.transform.position.y < -.1)
		{
			CompleteTutorials();
			var level = gameObject.transform.parent.parent.parent;
			SaveManager.SaveData(currentLevelName, level.GetComponent<StarManager>().GetScore());
			GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().ReloadNavigation();
			DeleteTower();
			CreateTower(gameObject.transform.parent.parent.parent.transform.localScale);
		}else if(Math.Abs(gameObject.transform.position.x) > 1.5 || Math.Abs(gameObject.transform.position.z) > 1.5)
		{
			ResetTower();
		}else if (touchedObject != null)
		{
			ResetTower();
		}
	}

	public void DeleteTower()
	{
		var currentLevel = GameObject.FindGameObjectWithTag("Level");
		foreach (Transform child in currentLevel.transform)
		{
			Destroy(child.gameObject);
		}
		Destroy(currentLevel);
	}

	public void CreateTower(Vector3 scale)
	{
		GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
		newLevel.transform.localScale = scale;
		RotateObject.Objective = GameObject.FindGameObjectWithTag("Objective");
	}

	public void ResetTower()
	{
		var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
		GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + currentLevelName + ".prefab");
		GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
		DeleteTower();
		newLevel.transform.localScale = currentLevel.transform.localScale;
		var newObjective = GameObject.FindGameObjectWithTag("Objective");
		RotateObject.Objective = newObjective;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag(c_spikeTag))
		{
			touchedObject = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		touchedObject = null;
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


	const string c_filePath = "Assets/Levels/";
	const string c_objectiveTag = "Objective";
	const string c_levelTag = "Level";
	const string c_spikeTag = "Spike";
	const string c_floorTag = "Floor";
	const float c_defaultFloorScale = 1.2f;
	const float c_defaultTowerSize = .4f;
	public string currentLevelName;
	public GameObject nextLevel;
	public GameObject touchedObject;
	private List<float> rotations = new List<float>();
	private Vector3 initialObjectivePosition;
}
