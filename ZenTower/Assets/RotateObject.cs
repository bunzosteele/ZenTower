using UnityEngine;

public class RotateObject : MonoBehaviour {
	private void Start ()
	{
		m_hingeJoint = GetComponent<HingeJoint>();
	}
	
	private void Update ()
	{
		if (otherObject == null)
			return;

		var controller = SteamVR_Controller.Input((int) otherObject.GetComponent<SteamVR_TrackedObject>().index);

		if (controller == null)
			Debug.Log("Everything didn't go according to plan");

		if (controller.GetPressDown(m_triggerButton))
		{
			AttachToController();
		}

		if (controller.GetPressUp(m_triggerButton))
		{
			ReleaseController();
		}
	}

	private void ReleaseController()
	{
		m_hingeJoint.connectedBody = null;
	}

	private void AttachToController()
	{
		m_hingeJoint.connectedBody = otherObject != null
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

	const string c_tag = "GameController";
	private Valve.VR.EVRButtonId m_triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private HingeJoint m_hingeJoint;
}
