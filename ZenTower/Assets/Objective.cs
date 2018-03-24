using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Objective : MonoBehaviour {

	void Start () {
		currentLevelName = gameObject.transform.parent.parent.gameObject.name.Split('(')[0];
		initialObjectivePosition = gameObject.transform.localPosition;
		s_defaultTowerSize = GameObject.FindGameObjectWithTag("Twistable").GetComponent<BoxCollider>().size.x;
		RotateObject.s_towerSize = s_defaultTowerSize;
		TowerResize.s_defaultTowerSize = s_defaultTowerSize;

		var scale = SettingsManager.LoadData().Scale;
		if (scale.HasValue) {
			var level = GameObject.FindGameObjectWithTag(c_levelTag);
			level.transform.localScale = new Vector3(scale.Value, scale.Value, scale.Value);
			RotateObject.s_towerSize = s_defaultTowerSize * scale.Value;

			var floor = GameObject.FindGameObjectWithTag(c_floorTag);
			if(floor.transform.lossyScale.x == c_defaultFloorScale)
				floor.transform.localScale = new Vector3(c_defaultFloorScale * scale.Value, c_defaultFloorScale * scale.Value, c_defaultFloorScale * scale.Value);
		}
	}

	void Update () {
		if(gameObject.transform.position.y < -.1)
		{
			CompleteTutorials();
			var level = gameObject.transform.parent.parent.parent;
			var category = LevelNavigation.CurrentCategory;
			SaveManager.SaveData(category, currentLevelName, level.GetComponent<StarManager>().GetScore());
			GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().ReloadNavigation();
			DeleteTower();
			GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().LoadNextLevel();
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

	public void ResetTower()
	{
		CompleteTutorials();
		var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
		var category = LevelNavigation.CurrentCategory;
		GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + category + '/' + currentLevelName + ".prefab");
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
	float s_defaultTowerSize = .4f;
	public string currentLevelName;
	public GameObject touchedObject;
	private List<float> rotations = new List<float>();
	private Vector3 initialObjectivePosition;
}
