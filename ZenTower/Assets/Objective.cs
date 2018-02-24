using UnityEngine;

public class Objective : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position.y < 0)
		{
			DeleteTower();
			CreateTower();
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
		Instantiate(level, new Vector3(-.3f, .05f, -.07f), Quaternion.identity);
		RotateObject.Objective = GameObject.FindGameObjectWithTag("Objective");
	}

	private SteamVR_TrackedObject m_trackedObject;
	public GameObject level;
}
