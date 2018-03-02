using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarManager : MonoBehaviour {
	void Start () {
		Vector3 relativePosition = gameObject.transform.position;
		Stars = new List<GameObject>();
		Vector3 towerPosition = GameObject.FindGameObjectWithTag(c_towerTag).transform.localPosition;
		for (int i = 0; i < StarCount; i++) {
			var createdStar = Instantiate(StarObject, new Vector3(towerPosition.x, gameObject.transform.localScale.y + .4f, towerPosition.z -(.15f * StarCount / 2) + .15f * i), Quaternion.identity);
			createdStar.transform.localScale = new Vector3(.05f, .05f, .05f);
			createdStar.transform.parent = gameObject.transform;
			createdStar.transform.Rotate(new Vector3(-90, 90, 0));
			if(i < 3)
			{
				var renderer = createdStar.GetComponent<Renderer>();
				renderer.material = Resources.Load("Gold", typeof(Material)) as Material;
			}
			Stars.Add(createdStar);
		}
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

	public void ResetStars()
	{
		while (Stars.Any())
		{
			GameObject starToRemove = Stars.FirstOrDefault();
			if (starToRemove != null)
			{
				Stars.Remove(starToRemove);
				Destroy(starToRemove);
			}
		}

		Vector3 relativePosition = gameObject.transform.position;
		Stars = new List<GameObject>();
		for (int i = 0; i < StarCount; i++)
		{
			var createdStar = Instantiate(StarObject, new Vector3(relativePosition.x, relativePosition.y + 1.5f, relativePosition.z + .5f - .15f * i), Quaternion.identity);
			createdStar.transform.localScale = new Vector3(.05f, .05f, .05f);
			createdStar.transform.parent = gameObject.transform;
			createdStar.transform.Rotate(new Vector3(-90, 90, 0));
			Stars.Add(createdStar);
		}
	}

	const string c_towerTag = "Tower";
	public GameObject StarObject;
	public int StarCount;
	private List<GameObject> Stars;
}
