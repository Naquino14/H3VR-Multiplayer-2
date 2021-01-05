using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class UIElement : MonoBehaviour
	{
		public CustomEvents.UnityEventHand onHandClick;

		protected Hand currentHand;

		protected virtual void Awake()
		{
			Button component = GetComponent<Button>();
			if ((bool)component)
			{
				component.onClick.AddListener(OnButtonClick);
			}
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			currentHand = hand;
			InputModule.instance.HoverBegin(base.gameObject);
			ControllerButtonHints.ShowButtonHint(hand, hand.uiInteractAction);
		}

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			InputModule.instance.HoverEnd(base.gameObject);
			ControllerButtonHints.HideButtonHint(hand, hand.uiInteractAction);
			currentHand = null;
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			if (hand.uiInteractAction != null && hand.uiInteractAction.GetStateDown(hand.handType))
			{
				InputModule.instance.Submit(base.gameObject);
				ControllerButtonHints.HideButtonHint(hand, hand.uiInteractAction);
			}
		}

		protected virtual void OnButtonClick()
		{
			onHandClick.Invoke(currentHand);
		}
	}
}
