using UnityEngine;

public class MenuToggle : MonoBehaviour {
	private void Start()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedController>();
		m_trackedObject.MenuButtonUnclicked -= ToggleMenu;
		m_trackedObject.MenuButtonUnclicked += ToggleMenu;
		m_menu = GameObject.FindGameObjectWithTag(c_menuTag);
		gameObject.AddComponent<SteamVR_LaserPointer>();
		gameObject.AddComponent<InterfaceInput>();
		gameObject.GetComponent<SteamVR_LaserPointer>().enabled = false;
		gameObject.GetComponent<InterfaceInput>().enabled = false;
		isMenuOpen = false;
	}

	private void ToggleMenu(object sender, ClickedEventArgs e)
	{
		var tutorial = GameObject.FindGameObjectWithTag("Level").GetComponent<TutorialTwo>();
		if (!isMenuOpen)
		{
			if (tutorial != null)
				tutorial.CompleteFirstStep();

			var controllerPosition = gameObject.transform.position;
			var controllerRotation = gameObject.transform.localRotation.eulerAngles.y;
			PositionMenu(controllerPosition, controllerRotation);
			gameObject.GetComponent<SteamVR_LaserPointer>().enabled = true;
			gameObject.GetComponent<InterfaceInput>().enabled = true;
			var lazers = GameObject.FindGameObjectsWithTag(c_lazerTag);
			foreach (var lazer in lazers)
			{
				if(lazer.transform.parent.parent == gameObject.transform)
					lazer.GetComponent<MeshRenderer>().enabled = true;
			}
			isMenuOpen = true;
		}
		else
		{
			if (tutorial != null)
				tutorial.CompleteThirdStep();

			m_menu.transform.position = new Vector3(.1f, -1f, 1.8f);
			m_menu.transform.Rotate(new Vector3(0, 360 - lastRotation, 0));
			m_menu.GetComponent<LevelNavigation>().ToggleIsDeleting(false);
			gameObject.GetComponent<SteamVR_LaserPointer>().enabled = false;
			gameObject.GetComponent<InterfaceInput>().enabled = false;
			var lazers = GameObject.FindGameObjectsWithTag(c_lazerTag);

			foreach(var lazer in lazers)
				lazer.GetComponent<MeshRenderer>().enabled = false;

			isMenuOpen = false;
		}
	}

	private void PositionMenu(Vector3 controllerPosition, float controllerRotation)
	{
		if (controllerRotation >= (360 - 22.5f) || controllerRotation < 22.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x, 1f, controllerPosition.z + 3f);
			lastRotation = 0;
		}
		else if (controllerRotation >= 22.5 && controllerRotation < 67.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x + 1.73f, 1f, controllerPosition.z + 1.73f);
			lastRotation = 45 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
		else if (controllerRotation >= 67.5 && controllerRotation < 112.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x + 3f, 1f, controllerPosition.z);
			lastRotation = 90 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
		else if (controllerRotation >= 112.5 && controllerRotation < 157.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x + 1.73f, 1f, controllerPosition.z - 1.73f);
			lastRotation = 135 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
		else if (controllerRotation >= 157.5 && controllerRotation < 202.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x, 1f, controllerPosition.z - 3f);
			lastRotation = 180 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
		else if (controllerRotation >= 202.5 && controllerRotation < 247.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x - 1.73f, 1f, controllerPosition.z - 1.73f);
			lastRotation = 225 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
		else if (controllerRotation >= 247.5 && controllerRotation < 292.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x - 3, 1f, controllerPosition.z);
			lastRotation = 270 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
		else if (controllerRotation >= 292.5 && controllerRotation < 337.5)
		{
			m_menu.transform.position = new Vector3(controllerPosition.x - 1.73f, 1f, controllerPosition.z + 1.73f);
			lastRotation = 315 - 360;
			m_menu.transform.Rotate(new Vector3(0, lastRotation, 0));
		}
	}

	const string c_menuTag = "Menu";
	const string c_lazerTag = "Lazer";
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.controllerIndex); } }
	private SteamVR_TrackedController m_trackedObject;
	private GameObject m_menu;
	private static float lastRotation;
	public static GameObject Objective { get; set; }
	public static bool isMenuOpen = false;
}
