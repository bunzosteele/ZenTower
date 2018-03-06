using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Objective : MonoBehaviour {

	// Use this for initialization
	void Start () {
		currentLevelName = gameObject.transform.parent.parent.gameObject.name.Split('(')[0];

		initialObjectivePosition = gameObject.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position.y < -.1)
		{
			var level = gameObject.transform.parent.parent.parent;
			SaveManager.SaveData(currentLevelName, level.GetComponent<StarManager>().GetScore());
			GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().ReloadNavigation();
			DeleteTower();
			CreateTower(gameObject.transform.parent.parent.parent.transform.localScale);
		}else if(Math.Abs(gameObject.transform.position.x) > 1.5 || Math.Abs(gameObject.transform.position.z) > 1.5)
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

	/*
	public void ResetTower()
	{
		GameObject currentLevel;
		GameObject currentTower;
		if(gameObject.transform.parent.parent.parent != null)
		{
			currentLevel = gameObject.transform.parent.parent.parent.gameObject;
			currentTower = gameObject.transform.parent.parent.gameObject;
		}
		else
		{
			currentLevel = gameObject.transform.parent.parent.gameObject;
			currentTower = gameObject.transform.parent.gameObject;
		}


		int i = 0;

		foreach (Transform child in currentTower.transform)
		{
			if(child.tag == "Twistable")
			{
				currentTower.transform.GetChild(i).transform.localEulerAngles = new Vector3(0, rotations[i], 0);
				i++;
			}
		}

		gameObject.transform.SetParent(currentTower.transform);
		gameObject.transform.localPosition = initialObjectivePosition;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		gameObject.GetComponent<Rigidbody>().isKinematic = false;
		foreach (var controller in GameObject.FindGameObjectsWithTag("GameController"))
		{
			var rotateObjectScript = controller.GetComponent<RotateObject>();
			if(rotateObjectScript != null)
				rotateObjectScript.Reset();
		}

		currentLevel.GetComponent<StarManager>().ResetStars();
	}*/

	const string c_filePath = "Assets/Levels/";
	const string c_objectiveTag = "Objective";
	const string c_levelTag = "Level";
	public string currentLevelName;
	public GameObject nextLevel;
	private List<float> rotations = new List<float>();
	private Vector3 initialObjectivePosition;
}
