using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Objective : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var currentLevel = gameObject.transform.parent.gameObject;
		for(int i = 0; i < currentLevel.transform.childCount -1; i++)
		{
			rotations.Add(currentLevel.transform.GetChild(i).transform.rotation.eulerAngles.y);
		}

		initialObjectivePosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position.y < -.1)
		{
			DeleteTower();
			CreateTower();
		}else if(Math.Abs(gameObject.transform.position.x) > 1 || Math.Abs(gameObject.transform.position.z) > 1)
		{
			ResetTower();
		}
	}

	private void DeleteTower()
	{
		var currentLevel = gameObject.transform.parent.parent.gameObject;
		foreach (Transform child in currentLevel.transform)
		{
			Destroy(child.gameObject);
		}
		Destroy(currentLevel);
	}

	private void CreateTower()
	{
		Instantiate(nextLevel, new Vector3(-.3f, .05f, -.07f), Quaternion.identity);
		RotateObject.Objective = GameObject.FindGameObjectWithTag("Objective");
	}

	private void ResetTower()
	{
		var currentLevel = gameObject.transform.parent.parent.gameObject;
		int i = 0;
		foreach (float rotation in rotations)
		{
			currentLevel.transform.GetChild(i).transform.localEulerAngles = new Vector3 (0, rotation, 0);
			i++;
		}

		gameObject.transform.SetParent(currentLevel.transform);
		gameObject.transform.SetPositionAndRotation(initialObjectivePosition, Quaternion.identity);
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		gameObject.GetComponent<Rigidbody>().isKinematic = false;
		foreach (var controller in GameObject.FindGameObjectsWithTag("GameController"))
		{
			var rotateObjectScript = controller.GetComponent<RotateObject>();
			if(rotateObjectScript != null)
				rotateObjectScript.Reset();
		}
		currentLevel.GetComponent<StarManager>().ResetStars();
	}

	public GameObject nextLevel;

	private List<float> rotations = new List<float>();
	private Vector3 initialObjectivePosition;
}
