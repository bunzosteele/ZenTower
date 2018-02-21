﻿using UnityEngine;

public class GrabObject : MonoBehaviour {
	// Use this for initialization
	private void Start ()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if(m_controller == null)
		{
			Debug.Log("No controller found.");
			return;
		}

		if (m_controller.GetPressDown(m_triggerButton))
		{
			PickupObject();
		}

		if (m_controller.GetPressUp(m_triggerButton))
		{
			DropObject();
		}

		if(grabbedObject != null)
		{
			MoveObject();
		}
	}

	private void DropObject()
	{
		float nearestAngle = grabbedObject.transform.rotation.eulerAngles.y % 90 > 45
			? (float) System.Math.Ceiling(grabbedObject.transform.rotation.eulerAngles.y / 90) * 90
			: (float) System.Math.Floor(grabbedObject.transform.rotation.eulerAngles.y / 90) * 90;

		grabbedObject.AddComponent<SpringJoint>();
		var springJoint = grabbedObject.GetComponent<SpringJoint>();
		

		//grabbedObject.transform.Rotate(0, (nearestAngle - grabbedObject.transform.rotation.eulerAngles.y), 0);

		grabbedObject = null;
	}

	private void PickupObject()
	{
		grabbedObject = hoverObject != null
			? hoverObject
			: null;

		Destroy(grabbedObject.GetComponent<SpringJoint>());
		m_initialControllerPosition = m_controller.transform.pos;
		m_initialObjectPosition = grabbedObject.transform.position;
	}

	private void MoveObject()
	{
		Vector2 initialVector = new Vector2(m_initialObjectPosition.x - m_initialControllerPosition.x, m_initialObjectPosition.z - m_initialControllerPosition.z);
		Vector2 currentVector = new Vector2(m_initialObjectPosition.x - m_controller.transform.pos.x, m_initialObjectPosition.z - m_controller.transform.pos.z);
		float sign = (initialVector.y < currentVector.y) ? -1.0f : 1.0f;
		var angle = Vector2.Angle(initialVector, currentVector) * sign;

		grabbedObject.transform.Rotate(0, angle * 10, 0);
		m_initialControllerPosition = m_controller.transform.pos;
		m_initialObjectPosition = grabbedObject.transform.position;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag(c_tag))
		{
			hoverObject = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		hoverObject = null;
	}

	[SerializeField]
	private GameObject hoverObject;
	[SerializeField]
	private GameObject grabbedObject;

	const string c_tag = "Grabbable";
	private Valve.VR.EVRButtonId m_triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.index); } }
	private SteamVR_TrackedObject m_trackedObject;
	private Vector3 m_initialControllerPosition;
	private Vector3 m_initialObjectPosition;
}
