using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;
	private Valve.VR.EVRButtonId touchpadAxis = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

	private float speed = 2f;

	public bool isMainController; // true =  movement, false = rotation

	private void Awake()
	{
		trackedObject = GetComponent<SteamVR_TrackedObject>();
	}

	void Update()
	{
		device = SteamVR_Controller.Input((int) trackedObject.index);

		Vector2 axis = device.GetAxis(touchpadAxis);

		if (isMainController)
		{
			transform.parent.position += (transform.right * axis.x + transform.forward * axis.y) * Time.deltaTime * speed;
			transform.parent.position = new Vector3(transform.parent.position.x, 0, transform.parent.position.z);
		}
		else
		{
			transform.parent.transform.Rotate(new Vector3(0, axis.x * speed, 0));
		}
	}
}