using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarManager : MonoBehaviour {
	void Start () {
		Vector3 relativePosition = gameObject.transform.position;
		Stars = new List<GameObject>();
		for (int i = 0; i < StarCount; i++) {
			var createdStar = Instantiate(StarObject, new Vector3(relativePosition.x, relativePosition.y + 1, relativePosition.z + .5f - .15f * i), Quaternion.identity);
			createdStar.transform.localScale = new Vector3(.05f, .05f, .05f);
			createdStar.transform.parent = gameObject.transform;
			createdStar.transform.Rotate(new Vector3(-90, 90, 0));
			Stars.Add(createdStar);
		}
	}
	
	void Update () {
		
	}

	public void RemoveStar()
	{
		GameObject starToRemove = Stars.FirstOrDefault();
		if (starToRemove != null)
		{
			Stars.Remove(starToRemove);
			Destroy(starToRemove);
		}
	}

	public GameObject StarObject;
	public int StarCount;
	private List<GameObject> Stars;
}
