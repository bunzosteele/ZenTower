using UnityEngine;

public class FloorShift : MonoBehaviour {
	void Start () {
	}
	
	void Update () {
		float currentY = gameObject.transform.position.y;
		var parent = GameObject.FindWithTag("Tower");
		foreach(Transform child in parent.transform)
		{
			if (currentY > child.position.y && currentY < child.position.y + c_floorHeight/2)
				currentFloor = child.gameObject;
		}

		if(currentFloor != null)
			transform.SetParent(currentFloor.transform);
	}


	[SerializeField]
	private GameObject currentFloor;
	const string c_tag = "Twistable";
	const float c_floorHeight = .1f;
}
