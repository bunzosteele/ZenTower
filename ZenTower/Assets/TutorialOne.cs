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
		hintCoroutine = StartCoroutine(HintCoroutine("Grab floors of the tower.", EVRButtonId.k_EButton_SteamVR_Trigger));
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
			hintCoroutine = StartCoroutine(HintCoroutine("Twist floors to get the ball into the hole.", EVRButtonId.k_EButton_SteamVR_Trigger, false));
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

	private IEnumerator HintCoroutine(string hint, EVRButtonId button, bool hasBuzz = true)
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

				if (hasBuzz)
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
