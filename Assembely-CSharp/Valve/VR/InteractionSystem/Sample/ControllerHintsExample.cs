using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class ControllerHintsExample : MonoBehaviour
	{
		private Coroutine buttonHintCoroutine;

		private Coroutine textHintCoroutine;

		public void ShowButtonHints(Hand hand)
		{
			if (buttonHintCoroutine != null)
			{
				StopCoroutine(buttonHintCoroutine);
			}
			buttonHintCoroutine = StartCoroutine(TestButtonHints(hand));
		}

		public void ShowTextHints(Hand hand)
		{
			if (textHintCoroutine != null)
			{
				StopCoroutine(textHintCoroutine);
			}
			textHintCoroutine = StartCoroutine(TestTextHints(hand));
		}

		public void DisableHints()
		{
			if (buttonHintCoroutine != null)
			{
				StopCoroutine(buttonHintCoroutine);
				buttonHintCoroutine = null;
			}
			if (textHintCoroutine != null)
			{
				StopCoroutine(textHintCoroutine);
				textHintCoroutine = null;
			}
			Hand[] hands = Player.instance.hands;
			foreach (Hand hand in hands)
			{
				ControllerButtonHints.HideAllButtonHints(hand);
				ControllerButtonHints.HideAllTextHints(hand);
			}
		}

		private IEnumerator TestButtonHints(Hand hand)
		{
			ControllerButtonHints.HideAllButtonHints(hand);
			while (true)
			{
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
				{
					ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
					if (action.GetActive(hand.handType))
					{
						ControllerButtonHints.ShowButtonHint(hand, action);
						yield return new WaitForSeconds(1f);
						ControllerButtonHints.HideButtonHint(hand, action);
						yield return new WaitForSeconds(0.5f);
					}
					yield return null;
				}
				ControllerButtonHints.HideAllButtonHints(hand);
				yield return new WaitForSeconds(1f);
			}
		}

		private IEnumerator TestTextHints(Hand hand)
		{
			ControllerButtonHints.HideAllTextHints(hand);
			while (true)
			{
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
				{
					ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
					if (action.GetActive(hand.handType))
					{
						ControllerButtonHints.ShowTextHint(hand, action, action.GetShortName());
						yield return new WaitForSeconds(3f);
						ControllerButtonHints.HideTextHint(hand, action);
						yield return new WaitForSeconds(0.5f);
					}
					yield return null;
				}
				ControllerButtonHints.HideAllTextHints(hand);
				yield return new WaitForSeconds(3f);
			}
		}
	}
}
