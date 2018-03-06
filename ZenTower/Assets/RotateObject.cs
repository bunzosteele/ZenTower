using UnityEngine;

public class RotateObject : MonoBehaviour {
	private void Start ()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedController>();
		Objective = GameObject.FindGameObjectWithTag("Objective");
		previousAngle = -1f;
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

		float nearestAngle = grabbedObject.transform.rotation.eulerAngles.y % c_angleLock > c_angleLock / 2
			? (float) System.Math.Ceiling(grabbedObject.transform.rotation.eulerAngles.y / c_angleLock) * c_angleLock
			: (float) System.Math.Floor(grabbedObject.transform.rotation.eulerAngles.y / c_angleLock) * c_angleLock;

		grabbedObject.transform.Rotate(0, (nearestAngle - grabbedObject.transform.rotation.eulerAngles.y), 0);

		Objective.transform.Translate(new Vector3(0, 0.0001f, 0));
		Objective.GetComponent<Rigidbody>().useGravity = true;
		Objective.GetComponent<Rigidbody>().isKinematic = false;

		if (grabbedObject != previousObject && nearestAngle != previousAngle)
		{
			grabbedObject.transform.parent.parent.GetComponent<StarManager>().RemoveStar();
			previousObject = grabbedObject;
			previousAngle = nearestAngle;
		}

		if (previousObject == null && previousAngle == -1f)
			previousObject = grabbedObject;

		grabbedObject = null;
	}

	private void PickupObject()
	{
		grabbedObject = hoverObject != null
			? hoverObject
			: null;

		if (grabbedObject == null)
			return;

		Objective.GetComponent<Rigidbody>().useGravity = false;
		Objective.GetComponent<Rigidbody>().isKinematic = true;
		if (grabbedObject != null)
		{
			m_initialControllerPosition = m_trackedObject.transform.position;
			m_initialObjectPosition = grabbedObject.transform.position;
			previousAngle = grabbedObject.transform.rotation.eulerAngles.y;
		}
	}

	private void TwistObject()
	{
		var dX = System.Math.Abs(m_initialControllerPosition.x - m_trackedObject.transform.position.x);
		var dZ = System.Math.Abs(m_initialControllerPosition.z - m_trackedObject.transform.position.z);

		float degrees = 360 / towerSides;
		float angle = 0;
		if (dX > dZ)
		{
			float signPosition = (m_initialObjectPosition.z > m_trackedObject.transform.position.z) ? 1.0f : -1.0f;
			float signDirection = (m_initialControllerPosition.x > m_trackedObject.transform.position.x) ? 1.0f : -1.0f;
			angle = (dX / towerSize) * degrees * signPosition * signDirection;
		}
		else
		{
			float signPosition = (m_initialObjectPosition.x > m_trackedObject.transform.position.x) ? 1.0f : -1.0f;
			float signDirection = (m_initialControllerPosition.z < m_trackedObject.transform.position.z) ? 1.0f : -1.0f;
			angle = (dZ / towerSize) * degrees * signPosition * signDirection;
		}

		grabbedObject.transform.Rotate(0, angle, 0);
		m_initialControllerPosition = m_trackedObject.transform.position;
		m_initialObjectPosition = grabbedObject.transform.position;
	}

	public void Reset()
	{
		grabbedObject = null;
		previousAngle = -1f;
		previousObject = null;
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
	[SerializeField]
	private GameObject previousObject = null;
	[SerializeField]
	private float previousAngle;
	[SerializeField]
	public float towerSize;
	[SerializeField]
	public float towerSides;



	const string c_tag = "Twistable";
	const int c_angleLock = 90;
	private Valve.VR.EVRButtonId m_triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.controllerIndex); } }
	private SteamVR_TrackedController m_trackedObject;
	private Vector3 m_initialControllerPosition;
	private Vector3 m_initialObjectPosition;
	public static GameObject Objective { get; set; }
}
