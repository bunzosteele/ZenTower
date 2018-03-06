using UnityEngine;

public class FloorShift : MonoBehaviour {
	void Start () {
	}
	
	void Update () {
		float currentY = gameObject.transform.position.y;
		GameObject parent = GameObject.FindWithTag("Tower");
		foreach(Transform child in parent.transform)
		{
			Collider collider = child.GetComponent<Collider>();
			if (collider == null)
				continue;

			if (currentY > child.position.y && currentY < child.position.y + collider.bounds.size.y / 2)
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
