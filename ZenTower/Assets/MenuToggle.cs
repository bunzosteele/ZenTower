﻿using UnityEngine;

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
		if (m_menu.transform.position.y < 0)
		{
			Debug.Log("Activating Menu");
			m_menu.transform.position = new Vector3(.3f, 1f, -1.5f);
			gameObject.GetComponent<SteamVR_LaserPointer>().enabled = true;
			gameObject.GetComponent<InterfaceInput>().enabled = true;
			var lazers = GameObject.FindGameObjectsWithTag(c_lazerTag);
			foreach (var lazer in lazers)
			{
				lazer.GetComponent<MeshRenderer>().enabled = true;
			}
			isMenuOpen = true;
		}
		else
		{
			Debug.Log("Disabling Menu");
			m_menu.transform.position = new Vector3(.3f, -1f, -1.5f);
			m_menu.GetComponent<LevelNavigation>().ToggleIsDeleting(false);
			gameObject.GetComponent<SteamVR_LaserPointer>().enabled = false;
			gameObject.GetComponent<InterfaceInput>().enabled = false;
			var lazers = GameObject.FindGameObjectsWithTag(c_lazerTag);

			foreach(var lazer in lazers)
			{
				lazer.GetComponent<MeshRenderer>().enabled = false;
			}
			isMenuOpen = false;
		}
	}

	const string c_menuTag = "Menu";
	const string c_lazerTag = "Lazer";
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.controllerIndex); } }
	private SteamVR_TrackedController m_trackedObject;
	private GameObject m_menu;
	public static GameObject Objective { get; set; }
	public static bool isMenuOpen = false;
}
