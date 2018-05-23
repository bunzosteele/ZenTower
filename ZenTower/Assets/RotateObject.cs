using System;
using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class RotateObject : MonoBehaviour {
	private void Start ()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedController>();
		Objective = GameObject.FindGameObjectWithTag("Objective");
		s_previousAngle = -1f;
	}
	
	private void Update ()
	{
		if(Objective == null)
		{
			Objective = GameObject.FindGameObjectWithTag("Objective");
			if (Objective == null)
				return;
		}

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
			TwistObject();
		}
	}

	private void DropObject()
	{
		if (grabbedObject == null)
			return;

		var droppedObject = grabbedObject;
		grabbedObject = null;

		var controllers = GameObject.FindGameObjectsWithTag(c_controllerTag);
		foreach (var controller in controllers)
		{
			var rotateObject = controller.GetComponent<RotateObject>();
			if (rotateObject != null && rotateObject.grabbedObject != null && rotateObject.grabbedObject == droppedObject)
				return;
		}

		int angleLock = 360 / GameObject.FindGameObjectWithTag(c_towerTag).GetComponent<TowerDetails>().SidesCount;
		var rotationThreshold = lastDirection > 0
			? angleLock / 3
			: angleLock * 2 / 3;

		float nearestAngle = droppedObject.transform.rotation.eulerAngles.y % angleLock > rotationThreshold
			? (float) Math.Ceiling(droppedObject.transform.rotation.eulerAngles.y / angleLock) * angleLock
			: (float) Math.Floor(droppedObject.transform.rotation.eulerAngles.y / angleLock) * angleLock;

		var objectiveBody = Objective.transform.GetComponent<Rigidbody>();

		objectiveBody.useGravity = true;
		objectiveBody.isKinematic = false;
		objectiveBody.velocity = Quaternion.AngleAxis(nearestAngle-s_previousAngle, Vector3.up) * s_previousObjectiveVelocity;
		objectiveBody.angularVelocity = Quaternion.AngleAxis(nearestAngle - s_previousAngle, Vector3.up) * s_previousObjectiveAngularVelocity;

		droppedObject.transform.Rotate(0, (nearestAngle - droppedObject.transform.rotation.eulerAngles.y), 0);

		if (droppedObject != s_previousObject && !MathUtility.AreEqual(nearestAngle % 360, s_previousAngle % 360))
		{
			if (Objective.GetComponent<Objective>().winnable && !Objective.GetComponent<Objective>().hasLost)
			{
				if(!droppedObject.transform.parent.parent.GetComponent<StarManager>().RemoveStar()){
					Objective.GetComponent<Objective>().LoseLevel();
				}
			}

			s_previousObject = droppedObject;
			s_previousAngle = nearestAngle;
			GameObject.FindGameObjectWithTag("AudioSource").transform.GetChild(2).GetComponent<AudioSource>().Play();
		}
		else
		{
			GameObject.FindGameObjectWithTag("AudioSource").transform.GetChild(3).GetComponent<AudioSource>().Play();
		}

		if (s_previousObject == null && s_previousAngle == -1f)
			s_previousObject = droppedObject;
	}

	private void PickupObject()
	{
		var objectiveScript = Objective.GetComponent<Objective>();
		if (!objectiveScript.winnable || objectiveScript.hasLost)
			return;

		var controllers = GameObject.FindGameObjectsWithTag(c_controllerTag);
		foreach (var controller in controllers)
		{
			var rotateObject = controller.GetComponent<RotateObject>();
			if (rotateObject != null && rotateObject.grabbedObject != null && rotateObject.grabbedObject != hoverObject)
				return;
		}

		grabbedObject = hoverObject != null
			? hoverObject
			: null;

		if (grabbedObject == null)
			return;

		var level = GameObject.FindGameObjectWithTag("Level");
		if (level != null) {
			var tutorial = level.GetComponent<TutorialOne>();
			if (tutorial != null)
				tutorial.CompleteFirstStep();
		}

		m_initialControllerPosition = m_trackedObject.transform.position;
		m_initialObjectPosition = grabbedObject.transform.position;

		//If something is already grabbed, do nothing.
		foreach (var controller in controllers)
		{
			var rotateObject = controller.GetComponent<RotateObject>();
			if (rotateObject != null && rotateObject.grabbedObject != null && controller != gameObject)
				return;
		}

		s_previousAngle = grabbedObject.transform.rotation.eulerAngles.y;
		var objectiveBody = Objective.transform.GetComponent<Rigidbody>();
		s_previousObjectiveVelocity = objectiveBody.velocity;
		s_previousObjectiveAngularVelocity = objectiveBody.angularVelocity;
		objectiveBody.useGravity = false;
		objectiveBody.isKinematic = true;

		hasBuzzed = true;
	}

	private void TwistObject()
	{
		var dX = System.Math.Abs(m_initialControllerPosition.x - m_trackedObject.transform.position.x);
		var dZ = System.Math.Abs(m_initialControllerPosition.z - m_trackedObject.transform.position.z);

		float degrees = 360 / s_towerSides;
		float angle = 0;
		if (dX > dZ)
		{
			float signPosition = (m_initialObjectPosition.z > m_trackedObject.transform.position.z) ? 1.0f : -1.0f;
			float signDirection = (m_initialControllerPosition.x > m_trackedObject.transform.position.x) ? 1.0f : -1.0f;
			angle = (dX / s_towerSize) * degrees * signPosition * signDirection;
			lastDirection = signPosition * signDirection;
		}
		else
		{
			float signPosition = (m_initialObjectPosition.x > m_trackedObject.transform.position.x) ? 1.0f : -1.0f;
			float signDirection = (m_initialControllerPosition.z < m_trackedObject.transform.position.z) ? 1.0f : -1.0f;
			angle = (dZ / s_towerSize) * degrees * signPosition * signDirection;
			lastDirection = signPosition * signDirection;
		}

		grabbedObject.transform.Rotate(0, angle, 0);
		m_initialControllerPosition = m_trackedObject.transform.position;
		m_initialObjectPosition = grabbedObject.transform.position;

		NotifyIfAtOrigin();
	}

	private void NotifyIfAtOrigin()
	{
		if (grabbedObject.transform.rotation.eulerAngles.y > s_previousAngle - 5 && grabbedObject.transform.rotation.eulerAngles.y < s_previousAngle + 5)
		{
			var controllers = Player.instance.hands;
			foreach (var controller in controllers)
			{
				var rotateObject = controller.GetComponent<RotateObject>();
				if (rotateObject != null && rotateObject.grabbedObject != null && rotateObject.grabbedObject == grabbedObject && !hasBuzzed)
				{
					StartCoroutine(StartBuzz(controller));
					hasBuzzed = true;
				}
			}
		}
		if(MathUtility.AreEqual(s_previousAngle, 0) && (Mathf.Abs(grabbedObject.transform.rotation.eulerAngles.y) > 45 && grabbedObject.transform.rotation.eulerAngles.y < 315))
		{
			hasBuzzed = false;
		}else if (!MathUtility.AreEqual(s_previousAngle, 0) && (grabbedObject.transform.rotation.eulerAngles.y < s_previousAngle - 45 || grabbedObject.transform.rotation.eulerAngles.y > s_previousAngle + 45))
		{
			hasBuzzed = false;
		}
	}

	private IEnumerator StartBuzz(Hand hand)
	{
		var startTime = Time.time;
		while (Time.time < startTime + .2f)
		{
			hand.controller.TriggerHapticPulse(500);
			yield return null;
		}
	}

	public void Reset()
	{
		grabbedObject = null;
		s_previousAngle = -1f;
		s_previousObject = null;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag(c_tag))
		{
			hoverObject = other.gameObject;
			if(Objective != null && Objective.transform.position.y >=0 && !Objective.GetComponent<Objective>().hasLost)
				Objective.GetComponent<Objective>().winnable = true;
		}
		else if (other.transform.parent.CompareTag(c_tag))
		{
			hoverObject = other.transform.parent.gameObject;
			if (Objective != null && Objective.transform.position.y >= 0 && !Objective.GetComponent<Objective>().hasLost)
				Objective.GetComponent<Objective>().winnable = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		hoverObject = null;
	}

	[SerializeField]
	private GameObject hoverObject;
	[SerializeField]
	public GameObject grabbedObject;
	[SerializeField]
	public float lastDirection;

	const string c_tag = "Twistable";
	const string c_controllerTag = "GameController";
	const string c_towerTag = "Tower";
	private static GameObject s_previousObject = null;
	private static float s_previousAngle;
	[SerializeField]
	public static float s_towerSize = .4f;
	public static int s_towerSides = 4;
	private Valve.VR.EVRButtonId m_triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.controllerIndex); } }
	private SteamVR_TrackedController m_trackedObject;
	private Vector3 m_initialControllerPosition;
	private Vector3 m_initialObjectPosition;
	public static Vector3 s_previousObjectiveAngularVelocity;
	public static Vector3 s_previousObjectiveVelocity;
	public static GameObject Objective { get; set; }
	private static bool hasBuzzed = false;
}
