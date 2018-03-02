using UnityEngine;

public class LevelReset : MonoBehaviour {

	public void ResetLevel()
	{
		var objective = GameObject.FindGameObjectWithTag(c_objectiveTag);
		objective.GetComponent<Objective>().ResetTower();
	}

	const string c_objectiveTag = "Objective";
}
