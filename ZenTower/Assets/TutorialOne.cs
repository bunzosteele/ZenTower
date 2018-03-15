using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class TutorialOne : MonoBehaviour
{
	void Start()
	{
		player = Player.instance;
		Invoke("ShowHint", 1.0f);
	}

	public void ShowHint()
	{
		hintCoroutine = StartCoroutine(HintCoroutine("Grab floors of the tower."));
	}

	public void CompleteFirstStep()
	{
		if (hintCoroutine != null)
		{
			foreach (Hand hand in player.hands)
			{
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
			}

			StopCoroutine(hintCoroutine);
			hintCoroutine = StartCoroutine(HintCoroutine("Twist floors to get the ball into the hole."));
		}
	}

	public void CompleteSecondStep()
	{
		if (hintCoroutine != null)
		{
			foreach (Hand hand in player.hands)
			{
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
			}

			StopCoroutine(hintCoroutine);
			hintCoroutine = null;
		}

		CancelInvoke("ShowHint");
	}

	private IEnumerator HintCoroutine(string hint)
	{
		float prevBreakTime = Time.time;
		float prevHapticPulseTime = Time.time;

		while (true)
		{
			bool pulsed = false;

			//Show the hint on each eligible hand
			foreach (Hand hand in player.hands)
			{
				bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, EVRButtonId.k_EButton_SteamVR_Trigger));
				if (!isShowingHint)
				{
					ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger, hint);
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
