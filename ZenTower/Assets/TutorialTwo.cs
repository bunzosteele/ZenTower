using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Threading;

public class TutorialTwo : MonoBehaviour
{
	void Start()
	{
		player = Player.instance;
		Invoke("ShowHint", 1.0f);
	}

	public void ShowHint()
	{
		hintCoroutine = StartCoroutine(HintCoroutine("The top button opens the menu.", EVRButtonId.k_EButton_ApplicationMenu));
	}

	public void CompleteFirstStep()
	{
		if (hintCoroutine != null)
		{
			foreach (Hand hand in player.hands)
			{
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_ApplicationMenu);
			}

			StopCoroutine(hintCoroutine);
			hintCoroutine = StartCoroutine(HintCoroutine("Here you can see scores, navigate levels, \n adjust the tower size, and more.", EVRButtonId.k_EButton_SteamVR_Trigger));
			Invoke("CompleteSecondStep", 10f);
		}
	}

	public void CompleteSecondStep()
	{
		if (hintCoroutine != null && MenuToggle.isMenuOpen)
		{
			foreach (Hand hand in player.hands)
			{
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
			}

			StopCoroutine(hintCoroutine);
			hintCoroutine = StartCoroutine(HintCoroutine("Press the top button again to close.", EVRButtonId.k_EButton_ApplicationMenu));
		}
	}

	public void CompleteThirdStep()
	{
		if (hintCoroutine != null)
		{
			foreach (Hand hand in player.hands)
			{
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_ApplicationMenu);
			}

			StopCoroutine(hintCoroutine);
		}
	}

	private IEnumerator HintCoroutine(string hint, EVRButtonId button)
	{
		while (true)
		{
			foreach (Hand hand in player.hands)
			{
				if (hand.controller == null)
					continue;

				bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, button));
				if (!isShowingHint)
					ControllerButtonHints.ShowTextHint(hand, button, hint);

					StartCoroutine(StartBuzz(hand));
				yield return new WaitForSeconds(1f);
			}
			yield return null;
		}
	}

	private IEnumerator StartBuzz(Hand hand)
	{
		var startTime = Time.time;
		while (Time.time < startTime + .1f)
		{
			hand.controller.TriggerHapticPulse(500);
			yield return null;
		}
	}

	private Coroutine hintCoroutine = null;
	private Player player = null;
}
