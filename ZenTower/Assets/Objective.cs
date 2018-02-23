using UnityEngine;

public class Objective : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position.y < 0)
		{
			CreateTower();
			DeleteTower();
		}
	}

	private void DeleteTower()
	{
		Destroy(level);
	}

	private void CreateTower()
	{
		Instantiate(level, new Vector3(-.3f, 0, -.2f), Quaternion.identity);
		RotateObject.Objective = GameObject.FindGameObjectWithTag("Objective");
	}

	private SteamVR_TrackedObject m_trackedObject;
	public GameObject level;
}
