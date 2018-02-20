using UnityEngine;

public class TurnOnGravity : MonoBehaviour {

	private void Start () {
		m_trackedObject = GetComponent<SteamVR_TrackedObject>();
	}

	private void Update () {
		if (m_controller == null)
		{
			Debug.Log("No controller found.");
			return;
		}

		if (m_controller.GetPressUp(m_button))
		{
			Debug.Log("Button release");
			ToggleGravity();
		}
	}

	private void ToggleGravity()
	{
		GameObject[] affectedObjects = GameObject.FindGameObjectsWithTag(c_tag);
		foreach(GameObject affectedObject in affectedObjects){
			Rigidbody rigidBody = affectedObject.GetComponent<Rigidbody>();
			rigidBody.useGravity = !rigidBody.useGravity;
			rigidBody.velocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
			Debug.Log("Gavity toggled on object: " + affectedObject.name);
		}
	}

	const string c_tag = "Grabbable";
	private Valve.VR.EVRButtonId m_button = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
	private SteamVR_Controller.Device m_controller { get { return SteamVR_Controller.Input((int) m_trackedObject.index); } }
	private SteamVR_TrackedObject m_trackedObject;
}
