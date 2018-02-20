using UnityEngine;

public class GrabObject : MonoBehaviour {
	// Use this for initialization
	private void Start ()
	{
		m_trackedObject = GetComponent<SteamVR_TrackedObject>();
		m_fixedJoint = GetComponent<FixedJoint>();
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
	}

	private void DropObject()
	{
		m_fixedJoint.connectedBody = null;
	}

	private void PickupObject()
	{
		m_fixedJoint.connectedBody = otherObject != null
			? otherObject.GetComponent<Rigidbody>()
			: null;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag(c_tag))
		{
			otherObject = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		otherObject = null;
	}

	[SerializeField]
	private GameObject otherObject;
	

	const string c_tag = "Grabbable";
	private Valve.VR.EVRButtonId m_triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.index); } }
	private SteamVR_TrackedObject m_trackedObject;
	private FixedJoint m_fixedJoint;
}
