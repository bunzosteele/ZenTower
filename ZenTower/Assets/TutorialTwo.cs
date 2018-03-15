using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class TutorialTwo : MonoBehaviour
{
	void Start()
	{
		player = Player.instance;
		Invoke("ShowHint", 1.0f);
	}

	public void ShowHint()
	{
		hintCoroutine = StartCoroutine(HintCoroutine("The menu button will open a menu off to the side.", EVRButtonId.k_EButton_ApplicationMenu));
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
			hintCoroutine = StartCoroutine(HintCoroutine("Press the menu button again to close.", EVRButtonId.k_EButton_ApplicationMenu));
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
		float prevBreakTime = Time.time;
		float prevHapticPulseTime = Time.time;

		while (true)
		{
			bool pulsed = false;

			//Show the hint on each eligible hand
			foreach (Hand hand in player.hands)
			{
				bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, button));
				if (!isShowingHint)
				{
					ControllerButtonHints.ShowTextHint(hand, button, hint);
					prevBreakTime = Time.time;
					prevHapticPulseTime = Time.time;
				}

				if (Time.time > prevHapticPulseTime + 0.05f)
				{
					//Haptic pulse for a few seconds
					pulsed = true;

					hand.controller.TriggerHapticPulse(500);
				}
			}

			if (Time.time > prevBreakTime + 3.0f)
			{
				//Take a break for a few seconds
				yield return new WaitForSeconds(3.0f);

				prevBreakTime = Time.time;
			}

			if (pulsed)
			{
				prevHapticPulseTime = Time.time;
			}

			yield return null;
		}
	}

	private Coroutine hintCoroutine = null;
	private Player player = null;
}
