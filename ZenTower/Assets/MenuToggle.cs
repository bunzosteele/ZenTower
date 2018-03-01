using UnityEngine;

public class MenuToggle : MonoBehaviour {
	private void Start()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedController>();
		m_trackedObject.MenuButtonUnclicked -= ToggleMenu;
		m_trackedObject.MenuButtonUnclicked += ToggleMenu;
		m_menu = GameObject.FindGameObjectWithTag(c_menuTag);
	}

	private void ToggleMenu(object sender, ClickedEventArgs e)
	{
		if (m_menu.transform.position.y < 0)
		{
			Debug.Log("Activating Menu");
			m_menu.transform.position = new Vector3(.3f, 1f, -1.5f);
			gameObject.AddComponent<SteamVR_LaserPointer>();
			gameObject.AddComponent<InterfaceInput>();

			return;
		}
		else
		{
			Debug.Log("Disabling Menu");
			m_menu.transform.position = new Vector3(.3f, -1f, -1.5f);
			Destroy(gameObject.GetComponent<InterfaceInput>());
			Destroy(gameObject.GetComponent<SteamVR_LaserPointer>());

			var lazers = GameObject.FindGameObjectsWithTag(c_lazerTag);
			foreach (GameObject lazer in lazers)
			{
				Destroy(lazer);
			}
		}
	}

	const string c_menuTag = "Menu";
	const string c_lazerTag = "Lazer";
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.controllerIndex); } }
	private SteamVR_TrackedController m_trackedObject;
	private GameObject m_menu;
	public static GameObject Objective { get; set; }
}
