using System;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var currentLevel = gameObject.transform.parent.gameObject;
		for(int i = 0; i < currentLevel.transform.childCount; i++)
		{
			var child = currentLevel.transform.GetChild(i);
			if(child.tag == "Twistable")
				rotations.Add(currentLevel.transform.GetChild(i).transform.rotation.eulerAngles.y);
		}

		initialObjectivePosition = gameObject.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position.y < -.1)
		{
			DeleteTower();
			CreateTower(gameObject.transform.parent.parent.parent.transform.localScale);
		}else if(Math.Abs(gameObject.transform.position.x) > 1 || Math.Abs(gameObject.transform.position.z) > 1)
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
		var currentLevel = gameObject.transform.parent.parent.parent.gameObject;
		var currentTower = gameObject.transform.parent.parent.gameObject;
		currentLevel.GetComponent<StarManager>().ResetStars();

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
	}

	public GameObject nextLevel;

	private List<float> rotations = new List<float>();
	private Vector3 initialObjectivePosition;
}
