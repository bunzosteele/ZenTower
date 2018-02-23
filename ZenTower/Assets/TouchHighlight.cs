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

	[Tooltip("The colour to highlight the object when it is touched. This colour will override any globally set colour (for instance on the `VRTK_InteractTouch` script).")]
	public Color touchHighlightColor = Color.clear;
	protected BaseHighlighter objectHighlighter;
}
