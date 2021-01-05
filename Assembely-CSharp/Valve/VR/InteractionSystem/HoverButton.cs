using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class HoverButton : MonoBehaviour
	{
		public Transform movingPart;

		public Vector3 localMoveDistance = new Vector3(0f, -0.1f, 0f);

		[Range(0f, 1f)]
		public float engageAtPercent = 0.95f;

		[Range(0f, 1f)]
		public float disengageAtPercent = 0.9f;

		public HandEvent onButtonDown;

		public HandEvent onButtonUp;

		public HandEvent onButtonIsPressed;

		public bool engaged;

		public bool buttonDown;

		public bool buttonUp;

		private Vector3 startPosition;

		private Vector3 endPosition;

		private Vector3 handEnteredPosition;

		private bool hovering;

		private Hand lastHoveredHand;

		private void Start()
		{
			if (movingPart == null && base.transform.childCount > 0)
			{
				movingPart = base.transform.GetChild(0);
			}
			startPosition = movingPart.localPosition;
			endPosition = startPosition + localMoveDistance;
			handEnteredPosition = endPosition;
		}

		private void HandHoverUpdate(Hand hand)
		{
			hovering = true;
			lastHoveredHand = hand;
			bool wasEngaged = engaged;
			float num = Vector3.Distance(movingPart.parent.InverseTransformPoint(hand.transform.position), endPosition);
			float num2 = Vector3.Distance(handEnteredPosition, endPosition);
			if (num > num2)
			{
				num2 = num;
				handEnteredPosition = movingPart.parent.InverseTransformPoint(hand.transform.position);
			}
			float value = num2 - num;
			float num3 = Mathf.InverseLerp(0f, localMoveDistance.magnitude, value);
			if (num3 > engageAtPercent)
			{
				engaged = true;
			}
			else if (num3 < disengageAtPercent)
			{
				engaged = false;
			}
			movingPart.localPosition = Vector3.Lerp(startPosition, endPosition, num3);
			InvokeEvents(wasEngaged, engaged);
		}

		private void LateUpdate()
		{
			if (!hovering)
			{
				movingPart.localPosition = startPosition;
				handEnteredPosition = endPosition;
				InvokeEvents(engaged, isEngaged: false);
				engaged = false;
			}
			hovering = false;
		}

		private void InvokeEvents(bool wasEngaged, bool isEngaged)
		{
			buttonDown = !wasEngaged && isEngaged;
			buttonUp = wasEngaged && !isEngaged;
			if (buttonDown && onButtonDown != null)
			{
				onButtonDown.Invoke(lastHoveredHand);
			}
			if (buttonUp && onButtonUp != null)
			{
				onButtonUp.Invoke(lastHoveredHand);
			}
			if (isEngaged && onButtonIsPressed != null)
			{
				onButtonIsPressed.Invoke(lastHoveredHand);
			}
		}
	}
}
