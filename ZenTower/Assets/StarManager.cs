using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarManager : MonoBehaviour {
	void Start () {
		Invoke("CreateStars", 1.55f);
	}

	private void CreateStars()
	{
		Stars = new List<GameObject>();
		var tower = GameObject.FindGameObjectWithTag(c_towerTag);
		Vector3 towerPosition = tower.transform.localPosition;
		for (int i = 0; i < StarCount; i++)
		{
			float y = 1.4f;
			float scale = .5f;
			float spacing = 1.5f;
			float xPosition = -5;
			float totalWidth = scale * StarCount + spacing * (StarCount - 1);
			var createdStar = Instantiate(StarObject, new Vector3(xPosition, y, (totalWidth / 2) * - 1 + spacing * i + scale * i + scale / 2), Quaternion.identity);
			createdStar.transform.localScale = new Vector3(scale, scale, scale);
			createdStar.transform.Rotate(new Vector3(-90, 90, 0));
			createdStar.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			if (i < 3)
			{
				var renderer = createdStar.GetComponent<Renderer>();
				renderer.material = Resources.Load("Gold", typeof(Material)) as Material;
			}

			if (ShowTutorial && i == TutorialIndex)
			{
				createdStar.AddComponent<TowerTutorial>();
				createdStar.GetComponent<TowerTutorial>().Message = TutorialMessage;
				createdStar.GetComponent<TowerTutorial>().Position = new Vector3(0, -.8f, 0);
				createdStar.GetComponent<TowerTutorial>().Rotation = new Vector3(0, 90, 0);
				createdStar.GetComponent<TowerTutorial>().isFar = true;
			}
			Stars.Add(createdStar);
		}
	}

	public void CompleteLevel()
	{
		float delay = .1f;
		foreach(var star in Stars)
		{
			Invoke("CelebrateStar", delay);
			star.GetComponent<AnimationScript>().isAnimated = true;
			delay += .45f;
		}
		Invoke("DeleteStars", delay + .45f);
	}

	private void CelebrateStar()
	{
		GameObject star = Stars[CelebrateIndex];
		CelebrateIndex++;
		if (star != null)
		{
			star.GetComponent<AudioSource>().Play();
		}
	}

	public void DeleteStars()
	{
		CelebrateIndex = 0;
		if (Stars == null)
			return;

		while (Stars.Any())
		{
			GameObject starToRemove = Stars.LastOrDefault();
			if (starToRemove != null)
			{
				Stars.Remove(starToRemove);
				Destroy(starToRemove);
			}
		}
	}

	public bool RemoveStar()
	{
		GameObject starToRemove = Stars.LastOrDefault();
		if (starToRemove != null)
		{
			Stars.Remove(starToRemove);
			Destroy(starToRemove);
			return true;
		}

		return false;
	}

	public int GetScore()
	{
		return Stars.Count;
	}

	const string c_towerTag = "Tower";
	public GameObject StarObject;
	public int StarCount;
	public int TutorialIndex;
	private int CelebrateIndex = 0;
	public string TutorialMessage;
	public bool ShowTutorial;
	private List<GameObject> Stars;
}
