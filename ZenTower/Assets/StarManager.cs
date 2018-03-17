using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarManager : MonoBehaviour {
	void Start () {
		Stars = new List<GameObject>();
		var tower = GameObject.FindGameObjectWithTag(c_towerTag);
		Vector3 towerPosition = tower.transform.localPosition;
		for (int i = 0; i < StarCount; i++) {
			float y = 1.4f * gameObject.transform.localScale.y;
			float scale = .05f * gameObject.transform.localScale.y;
			float spacing = .15f * gameObject.transform.localScale.y;
			float xPosition = towerPosition.x * gameObject.transform.localScale.x;
			var createdStar = Instantiate(StarObject, new Vector3(xPosition, y, towerPosition.z - (spacing * StarCount / 2) + spacing * i), Quaternion.identity);
			createdStar.transform.localScale = new Vector3(scale, scale, scale);
			createdStar.transform.parent = gameObject.transform;
			createdStar.transform.Rotate(new Vector3(-90, 90, 0));
			createdStar.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			if(i < 3)
			{
				var renderer = createdStar.GetComponent<Renderer>();
				renderer.material = Resources.Load("Gold", typeof(Material)) as Material;
			}

			if(ShowTutorial && i == TutorialIndex)
			{
				createdStar.AddComponent<TowerTutorial>();
				createdStar.GetComponent<TowerTutorial>().Message = TutorialMessage;
				float yScale = tower.transform.lossyScale.y > 1
					? (tower.transform.lossyScale.y - 1) / 2 + 1
					: Mathf.Abs((tower.transform.lossyScale.y - 1) / 2) + tower.transform.lossyScale.y;
				createdStar.GetComponent<TowerTutorial>().Position = new Vector3(0, -.35f * yScale, 0);
				createdStar.GetComponent<TowerTutorial>().Rotation = new Vector3(0, 90, 0);
			}
			Stars.Add(createdStar);
		}
	}

	public void RemoveStar()
	{
		GameObject starToRemove = Stars.LastOrDefault();
		if (starToRemove != null)
		{
			Stars.Remove(starToRemove);
			Destroy(starToRemove);
		}
	}

	public int GetScore()
	{
		return Stars.Count;
	}

	const string c_towerTag = "Tower";
	public GameObject StarObject;
	public int StarCount;
	public int TutorialIndex;
	public string TutorialMessage;
	public bool ShowTutorial;
	private List<GameObject> Stars;
}
