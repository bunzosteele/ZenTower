using UnityEngine;

public class FloorShift : MonoBehaviour {
	void Start () {
	}
	
	void Update () {
		if (!gameObject.GetComponent<Objective>().winnable)
			return;

		float currentY = gameObject.transform.position.y;
		GameObject parent = GameObject.FindGameObjectWithTag("Tower");
		foreach(Transform child in parent.transform)
		{
			Collider collider = child.GetComponent<Collider>();
			if (collider == null || child == gameObject.transform)
				continue;

			if (currentY > (child.position.y - collider.bounds.size.y / 3) && currentY < child.position.y + collider.bounds.size.y / 3 * 2)
				currentFloor = child.gameObject;
		}

		if(currentFloor != null)
			transform.SetParent(currentFloor.transform);
	}


	[SerializeField]
	public GameObject currentFloor;
	const string c_tag = "Twistable";
	const float c_floorHeight = .1f;
}
