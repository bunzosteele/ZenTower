using UnityEngine;

public class LevelChanger : MonoBehaviour {
	private void Start()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedObject>();
	}

	private void Update()
	{
		if (m_controller == null)
		{
			Debug.Log("No controller found.");
			return;
		}

		if (m_controller.GetPressUp(m_button))
		{
			Debug.Log("Button release");
			CreateTower();
		}
	}

	private void CreateTower()
	{
		Instantiate(level, new Vector3(-.3f, 0, -.2f), Quaternion.identity);
	}

	const string c_tag = "Grabbable";
	private Valve.VR.EVRButtonId m_button = Valve.VR.EVRButtonId.k_EButton_Grip;
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.index); } }
	private SteamVR_TrackedObject m_trackedObject;
	public GameObject level;
}
