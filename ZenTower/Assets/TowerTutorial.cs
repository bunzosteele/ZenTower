using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class TowerTutorial : MonoBehaviour
{
	void Start()
	{
		player = Player.instance;
		textHintPrefab = Resources.Load("ControllerTextHint", typeof(GameObject)) as GameObject;
		ShowHint();
	}

	public void ShowHint()
	{
		CreateAndAddButtonInfo();
		hintCoroutine = StartCoroutine(HintCoroutine(Message));
	}

	public void CompleteFirstStep()
	{
		if (hintCoroutine != null)
		{
			HideText();

			StopCoroutine(hintCoroutine);
		}
	}

	private IEnumerator HintCoroutine(string hint)
	{
		while (true)
		{
			bool isShowingHint = !GetActiveHintText().Equals(gameObject.name);
			if (!isShowingHint)
			{
				ShowText(hint);
			}
			yield return null;
		}
	}

	public void CreateAndAddButtonInfo()
	{
		Transform anchorTransform = gameObject.transform;

		TowerHintInfo hintInfo = new TowerHintInfo();
		towerHintInfo = hintInfo;

		hintInfo.componentName = anchorTransform.name;

		//Get the local transform for the anchor
		hintInfo.localTransform = anchorTransform;

		hintInfo.textEndOffsetDir = hintInfo.localTransform.right;

		//Create the text hint object
		Vector3 hintStartPos = hintInfo.localTransform.position;
		hintInfo.textHintObject = GameObject.Instantiate(textHintPrefab, hintStartPos, Quaternion.identity) as GameObject;
		hintInfo.textHintObject.name = "Hint_" + hintInfo.componentName + "_Start";
		hintInfo.textHintObject.transform.SetParent(gameObject.transform);
		hintInfo.textHintObject.layer = gameObject.layer;

		//Get all the relevant child objects
		hintInfo.textStartAnchor = hintInfo.textHintObject.transform.Find("Start");
		hintInfo.textEndAnchor = hintInfo.textHintObject.transform.Find("End");
		hintInfo.canvasOffset = hintInfo.textHintObject.transform.Find("CanvasOffset");
		hintInfo.line = hintInfo.textHintObject.transform.Find("Line").GetComponent<LineRenderer>();
		hintInfo.textCanvas = hintInfo.textHintObject.GetComponentInChildren<Canvas>();
		hintInfo.text = hintInfo.textCanvas.GetComponentInChildren<Text>();
		hintInfo.textMesh = hintInfo.textCanvas.GetComponentInChildren<TextMesh>();

		var tower = GameObject.FindGameObjectWithTag(c_towerTag);
		hintInfo.textHintObject.SetActive(false);
		hintInfo.textEndAnchor.position += Position * tower.transform.lossyScale.y;
		hintInfo.canvasOffset.Rotate(Rotation);
		hintInfo.canvasOffset.position += Position * tower.transform.lossyScale.y;

		hintInfo.textStartAnchor.position = hintStartPos;

		if (hintInfo.text != null)
		{
			hintInfo.text.text = hintInfo.componentName;
		}

		if (hintInfo.textMesh != null)
		{
			hintInfo.textMesh.text = hintInfo.componentName;
		}

		centerPosition += hintInfo.textStartAnchor.position;

		// Scale hint components to match player size
		hintInfo.textCanvas.transform.localScale = Vector3.Scale(hintInfo.textCanvas.transform.localScale, player.transform.localScale);
		hintInfo.textStartAnchor.transform.localScale = Vector3.Scale(hintInfo.textStartAnchor.transform.localScale, player.transform.localScale);
		hintInfo.textEndAnchor.transform.localScale = Vector3.Scale(hintInfo.textEndAnchor.transform.localScale, player.transform.localScale);
		hintInfo.line.transform.localScale = Vector3.Scale(hintInfo.line.transform.localScale, player.transform.localScale);
	}

	private void HideText()
	{
		towerHintInfo.textHintObject.SetActive(false);
		towerHintInfo.textHintActive = false;
	}

	private string GetActiveHintText()
	{
		return towerHintInfo.text.text;
	}

	private void ShowText(string text)
	{
		towerHintInfo.textHintObject.SetActive(true);
		towerHintInfo.textHintActive = true;

		if (towerHintInfo.text != null)
		{
			towerHintInfo.text.text = text;
		}

		if (towerHintInfo.textMesh != null)
		{
			towerHintInfo.textMesh.text = text;
		}

		Transform lineTransform = towerHintInfo.line.transform;

		towerHintInfo.line.useWorldSpace = false;
		towerHintInfo.line.SetPosition(0, lineTransform.InverseTransformPoint(towerHintInfo.textStartAnchor.position));
		towerHintInfo.line.SetPosition(1, lineTransform.InverseTransformPoint(towerHintInfo.textEndAnchor.position));
	}

	private class TowerHintInfo
	{
		public string componentName;
		public Transform localTransform;

		public GameObject textHintObject;
		public Transform textStartAnchor;
		public Transform textEndAnchor;
		public Vector3 textEndOffsetDir;
		public Transform canvasOffset;

		public Text text;
		public TextMesh textMesh;
		public Canvas textCanvas;
		public LineRenderer line;

		public bool textHintActive = false;
	}

	const string c_towerTag = "Tower";
	private TowerHintInfo towerHintInfo;
	private Coroutine hintCoroutine = null;
	private GameObject textHintPrefab;
	private Vector3 centerPosition = Vector3.zero;
	public Vector3 Position;
	public Vector3 Rotation;
	private static Player player;

	public string Message;
}
