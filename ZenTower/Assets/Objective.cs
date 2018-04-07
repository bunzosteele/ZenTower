﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Objective : MonoBehaviour {

	void Start () {
		currentLevelName = gameObject.transform.parent.parent.gameObject.name.Split('(')[0];
		initialObjectivePosition = gameObject.transform.localPosition;
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		s_defaultTowerSize = GameObject.FindGameObjectWithTag("Twistable").GetComponent<BoxCollider>().size.x;
		RotateObject.s_towerSize = s_defaultTowerSize;
		TowerResize.s_defaultTowerSize = s_defaultTowerSize;

		var scale = SettingsManager.LoadData().Scale;
		m_level = gameObject.transform.parent.parent.gameObject;
		if (scale.HasValue) {
			
			m_level.transform.localScale = new Vector3(scale.Value, scale.Value, scale.Value);
			RotateObject.s_towerSize = s_defaultTowerSize * scale.Value;

			var floor = GameObject.FindGameObjectWithTag(c_floorTag);
			if(floor.transform.lossyScale.x == c_defaultFloorScale)
				floor.transform.localScale = new Vector3(c_defaultFloorScale * scale.Value, c_defaultFloorScale * scale.Value, c_defaultFloorScale * scale.Value);
		}

		PlaySpawnAnimations();
		Invoke("ActivateObjective", 1.8f);
	}

	private void ActivateObjective()
	{
		gameObject.GetComponent<Rigidbody>().isKinematic = false;
		var tower = m_level.transform.GetChild(0);
		int childCount = tower.transform.childCount;
		int i = 0;
		int count = 0;
		System.Random random = new System.Random();
		while (count <= childCount)
		{
			Transform floorWrapper = tower.transform.GetChild(i);
			if (floorWrapper.childCount > 0 && floorWrapper.GetChild(0) != null && floorWrapper.GetChild(0).tag == c_twistableTag)
			{
				Transform floor = floorWrapper.GetChild(0);
				floor.parent = tower;
				Destroy(floorWrapper.gameObject);
			}
			i++;
			count++;
		}
	}

	void Update () {
		if (winnable)
		{
			if (gameObject.transform.position.y < -.1)
			{
				CompleteTutorials();
				var level = gameObject.transform.parent.parent.parent;
				var category = LevelNavigation.CurrentCategory;
				SaveManager.SaveData(category, currentLevelName, level.GetComponent<StarManager>().GetScore());
				BeatLevel();
			}
			else if (Math.Abs(gameObject.transform.position.x) > 1.5 || Math.Abs(gameObject.transform.position.z) > 1.5)
			{
				ResetTower();
			}
			else if (touchedObject != null)
			{
				ResetTower();
			}
		}
	}

	private void BeatLevel()
	{
		var starManager = GameObject.FindGameObjectWithTag(c_levelTag).GetComponent<StarManager>();
		GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().PauseNavigation(2f);
		starManager.CompleteLevel();
		m_level.tag = "Untagged";
		winnable = false;
		Invoke("DeleteTower", 2f);
		Invoke("LoadNextLevel", 2f);
	}

	public void DeleteTower()
	{
		m_level.tag = "Untagged";
		spawnAnimation.Play("TowerDespawn");
		winnable = false;
		Invoke("DestroyTower", 1.55f);
	}

	private void LoadNextLevel()
	{
		GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().LoadNextLevel();
	}

	private void DestroyTower()
	{
		var currentLevel = m_level;
		foreach (Transform child in currentLevel.transform)
		{
			Destroy(child.gameObject);
		}
		Destroy(currentLevel);
	}

	public void ResetTower()
	{
		winnable = false;
		CompleteTutorials();
		var currentLevel = GameObject.FindGameObjectWithTag(c_levelTag);
		var category = LevelNavigation.CurrentCategory;
		GameObject nextLevel = AssetDatabase.LoadAssetAtPath<GameObject>(c_filePath + category + '/' + currentLevelName + ".prefab");
		GameObject newLevel = Instantiate(nextLevel, new Vector3(0, 0, 0), Quaternion.identity);
		currentLevel.GetComponent<StarManager>().DeleteStars();
		DeleteTower();
		newLevel.transform.localScale = currentLevel.transform.localScale;
		var newObjective = GameObject.FindGameObjectWithTag("Objective");
		RotateObject.Objective = newObjective;
		GameObject.FindGameObjectWithTag("Menu").GetComponent<LevelNavigation>().PauseNavigation();
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

	private void PlaySpawnAnimations()
	{
		spawnAnimation = m_level.GetComponent<Animation>();
		spawnAnimation.wrapMode = WrapMode.Once;
		spawnAnimation.Play("TowerSpawn");

		var tower = m_level.transform.GetChild(0);
		int childCount = tower.transform.childCount;
		int i = 0;
		int count = 0;
		System.Random random = new System.Random();
		while (count <= childCount)
		{
			Transform floor = tower.transform.GetChild(i);
			if(floor.tag == c_twistableTag)
			{
				GameObject floorWrapper = new GameObject();
				floorWrapper.transform.position = floor.transform.position;
				floor.parent = floorWrapper.transform;
				floorWrapper.transform.parent = tower;
				floorWrapper.AddComponent<Animation>();
				var animation = floorWrapper.GetComponent<Animation>();
				animation.wrapMode = WrapMode.Once;

				string animationName = GetAnimationName(random);
				AnimationClip clip = Resources.Load(animationName, typeof(AnimationClip)) as AnimationClip;
				animation.AddClip(clip, animationName);
				animation.Play(animationName);
			}
			else
			{
				i++;
			}
			count++;
		}
	}

	private string GetAnimationName(System.Random random)
	{
		int roll = random.Next(0, 3);
		switch (roll)
		{
			case 0:
				return "FloorTwistClockwise";
			case 1:
				return "FloorTwistCounterClockwise";
			case 2:
				return "FloorTwistClockwiseFast";
			case 3:
				return "FloorTwistCounterClockwiseFast";
		}

		return "FloorTwistClockwise";
	}


	const string c_filePath = "Assets/Levels/";
	const string c_objectiveTag = "Objective";
	const string c_levelTag = "Level";
	const string c_spikeTag = "Spike";
	const string c_floorTag = "Floor";
	const string c_twistableTag = "Twistable";
	const float c_defaultFloorScale = 1.2f;
	float s_defaultTowerSize = .4f;
	public GameObject m_level;
	private string currentLevelName;
	public GameObject touchedObject;
	private Vector3 initialObjectivePosition;
	private Animation spawnAnimation;
	public Boolean winnable = false;
}
