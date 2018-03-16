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

			var floor = GameObject.FindGameObjectWithTag(c_floorTag);
			Vector3 floorScale = floor.transform.localScale;
			floor.transform.localScale = new Vector3(floorScale.x - .1f, floorScale.y - .1f, floorScale.z - .1f);

			GameObject.FindGameObjectWithTag(c_menuTag).GetComponent<LevelNavigation>().ToggleIsDeleting(false);
			RotateObject.s_towerSize = c_defaultTowerSize * (scale.x - .1f);
			SettingsManager.SaveData(new SettingsManager.Settings { Scale = tower.transform.localScale.y });
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

			var floor = GameObject.FindGameObjectWithTag(c_floorTag);
			Vector3 floorScale = floor.transform.localScale;
			floor.transform.localScale = new Vector3(floorScale.x + .1f, floorScale.y + .1f, floorScale.z + .1f);

			GameObject.FindGameObjectWithTag(c_menuTag).GetComponent<LevelNavigation>().ToggleIsDeleting(false);
			RotateObject.s_towerSize = c_defaultTowerSize * (scale.x + .1f);
			SettingsManager.SaveData(new SettingsManager.Settings { Scale = tower.transform.localScale.y });
		}
	}

	const string c_levelTag = "Level";
	const string c_floorTag = "Floor";
	const string c_menuTag = "Menu";
	const float c_defaultTowerSize = .4f;
}
