using UnityEngine;

public class TouchHighlight : MonoBehaviour {
	private void Start()
	{
		InitialiseHighlighter();
	}

	private void Update()
	{
	}

	protected virtual void InitialiseHighlighter()
	{
		if (touchHighlightColor != Color.clear && objectHighlighter == null)
		{
			objectHighlighter = BaseHighlighter.GetActiveHighlighter(gameObject);
			if (objectHighlighter == null)
			{
				objectHighlighter = gameObject.AddComponent<MaterialColorSwapHighlighter>();
			}
			objectHighlighter.Initialise(touchHighlightColor);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag(c_tag))
		{
			var controllers = GameObject.FindGameObjectsWithTag(c_tag);
			foreach (var controller in controllers)
			{
				var rotateObject = controller.GetComponent<RotateObject>();
				if (rotateObject != null && rotateObject.grabbedObject != null && rotateObject.grabbedObject != gameObject)
				{
					objectHighlighter.Unhighlight();
					return;
				}
			}

			objectHighlighter.Highlight(touchHighlightColor);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		objectHighlighter.Unhighlight();
	}

	const string c_tag = "GameController";
	private Color highlightColor = Color.blue;
	private Color startColor;
	private Renderer m_renderer;

	public Color touchHighlightColor = Color.clear;
	protected BaseHighlighter objectHighlighter;
}
