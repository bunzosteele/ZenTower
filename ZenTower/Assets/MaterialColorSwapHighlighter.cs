
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
using System.Linq;

public class MaterialColorSwapHighlighter : BaseHighlighter
	{
		[Tooltip("The emission colour of the texture will be the highlight colour but this percent darker.")]
		public float emissionDarken = 50f;
		[Tooltip("A custom material to use on the highlighted object.")]
		public Material customMaterial;

		protected Dictionary<string, Material[]> originalSharedRendererMaterials = new Dictionary<string, Material[]>();
		protected Dictionary<string, Material[]> originalRendererMaterials = new Dictionary<string, Material[]>();
		protected Dictionary<string, Coroutine> faderRoutines;
		protected bool resetMainTexture = false;

		/// <summary>
		/// The Initialise method sets up the highlighter for use.
		/// </summary>
		/// <param name="color">Not used.</param>
		/// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'resetMainTexture', bool&gt;` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.</param>
		public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
		{
			originalSharedRendererMaterials = new Dictionary<string, Material[]>();
			originalRendererMaterials = new Dictionary<string, Material[]>();
			faderRoutines = new Dictionary<string, Coroutine>();
			resetMainTexture = GetOption<bool>(options, "resetMainTexture");
			ResetHighlighter();
		}

		/// <summary>
		/// The ResetHighlighter method stores the object's materials and shared materials prior to highlighting.
		/// </summary>
		public override void ResetHighlighter()
		{
			StoreOriginalMaterials();
		}

		/// <summary>
		/// The Highlight method initiates the change of colour on the object and will fade to that colour (from a base white colour) for the given duration.
		/// </summary>
		/// <param name="color">The colour to highlight to.</param>
		/// <param name="duration">The time taken to fade to the highlighted colour.</param>
		public override void Highlight(Color? color, float duration = 0f)
		{
			if (color == null)
			{
				return;
			}
			ChangeToHighlightColor((Color)color, duration);
		}

		/// <summary>
		/// The Unhighlight method returns the object back to it's original colour.
		/// </summary>
		/// <param name="color">Not used.</param>
		/// <param name="duration">Not used.</param>
		public override void Unhighlight(Color? color = null, float duration = 0f)
		{
			if (originalRendererMaterials == null)
			{
				return;
			}

			if (faderRoutines != null)
			{
				foreach (var fadeRoutine in faderRoutines)
				{
					StopCoroutine(fadeRoutine.Value);
				}
				faderRoutines.Clear();
			}

			foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
			{
				var objectReference = renderer.gameObject.GetInstanceID().ToString();
				if (!originalRendererMaterials.ContainsKey(objectReference))
				{
					continue;
				}

				renderer.materials = originalRendererMaterials[objectReference];
				renderer.sharedMaterials = originalSharedRendererMaterials[objectReference];
			}
		}

		protected virtual void StoreOriginalMaterials()
		{
			originalSharedRendererMaterials.Clear();
			originalRendererMaterials.Clear();
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
			{
				var objectReference = renderer.gameObject.GetInstanceID().ToString();
				originalSharedRendererMaterials[objectReference] = renderer.sharedMaterials;
				originalRendererMaterials[objectReference] = renderer.materials;
				renderer.sharedMaterials = originalSharedRendererMaterials[objectReference];
			}
		}

		protected virtual void ChangeToHighlightColor(Color color, float duration = 0f)
		{
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true).Where(r => r.tag != "Objective"))
			{
				var swapCustomMaterials = new Material[renderer.materials.Length];

				for (int i = 0; i < renderer.materials.Length; i++)
				{
					var material = renderer.materials[i];
					if (customMaterial)
					{
						material = customMaterial;
						swapCustomMaterials[i] = material;
					}

					var faderRoutineID = material.GetInstanceID().ToString();

					if (faderRoutines.ContainsKey(faderRoutineID) && faderRoutines[faderRoutineID] != null)
					{
						StopCoroutine(faderRoutines[faderRoutineID]);
						faderRoutines.Remove(faderRoutineID);
					}

					material.EnableKeyword("_EMISSION");

					if (resetMainTexture && material.HasProperty("_MainTex"))
					{
						renderer.material.SetTexture("_MainTex", new Texture());
					}

					if (material.HasProperty("_Color"))
					{
						if (duration > 0f)
						{
							faderRoutines[faderRoutineID] = StartCoroutine(CycleColor(material, material.color, color, duration));
						}
						else
						{
							material.color = color;
							if (material.HasProperty("_EmissionColor"))
							{
								material.SetColor("_EmissionColor", ColorDarken(color, emissionDarken));
							}
						}
					}
				}

				if (customMaterial)
				{
					renderer.materials = swapCustomMaterials;
				}
			}
		}

		protected virtual IEnumerator CycleColor(Material material, Color startColor, Color endColor, float duration)
		{
			var elapsedTime = 0f;
			while (elapsedTime <= duration)
			{
				elapsedTime += Time.deltaTime;
				if (material.HasProperty("_Color"))
				{
					material.color = Color.Lerp(startColor, endColor, (elapsedTime / duration));
				}
				if (material.HasProperty("_EmissionColor"))
				{
					material.SetColor("_EmissionColor", Color.Lerp(startColor, ColorDarken(endColor, emissionDarken), (elapsedTime / duration)));
				}
				yield return null;
			}
		}

		/// <summary>
		/// The ColorDarken method takes a given colour and darkens it by the given percentage.
		/// </summary>
		/// <param name="color">The source colour to apply the darken to.</param>
		/// <param name="percent">The percent to darken the colour by.</param>
		/// <returns>The new colour with the darken applied.</returns>
		private static Color ColorDarken(Color color, float percent)
		{
			return new Color(NumberPercent(color.r, percent), NumberPercent(color.g, percent), NumberPercent(color.b, percent), color.a);
		}

		/// <summary>
		/// The NumberPercent method is used to determine the percentage of a given value.
		/// </summary>
		/// <param name="value">The value to determine the percentage from</param>
		/// <param name="percent">The percentage to find within the given value.</param>
		/// <returns>A float containing the percentage value based on the given input.</returns>
		private static float NumberPercent(float value, float percent)
		{
			percent = Mathf.Clamp(percent, 0f, 100f);
			return (percent == 0f ? value : (value - (percent / 100f)));
		}
}
