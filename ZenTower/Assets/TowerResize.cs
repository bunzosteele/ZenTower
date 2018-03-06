using UnityEngine;

public class TowerResize : MonoBehaviour {

	public void ShrinkTower()
	{
		if (MenuToggle.isMenuOpen)
		{
			var tower = GameObject.FindGameObjectWithTag(c_levelTag);
			Vector3 scale = tower.transform.localScale;
			if (scale.x < .5)
				return;
			tower.transform.localScale = new Vector3(scale.x - .1f, scale.y - .1f, scale.z - .1f);

			GameObject.FindGameObjectWithTag(c_menuTag).GetComponent<LevelNavigation>().ToggleIsDeleting(false);
		}
	}

	public void GrowTower()
	{
		if (MenuToggle.isMenuOpen)
		{
			var tower = GameObject.FindGameObjectWithTag(c_levelTag);
			Vector3 scale = tower.transform.localScale;
			if (scale.x > 1.8f)
				return;
			tower.transform.localScale = new Vector3(scale.x + .1f, scale.y + .1f, scale.z + .1f);

			GameObject.FindGameObjectWithTag(c_menuTag).GetComponent<LevelNavigation>().ToggleIsDeleting(false);
		}
	}

	const string c_levelTag = "Level";
	const string c_menuTag = "Menu";
}
