using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ModalThrowable : Throwable
	{
		[Tooltip("The local point which acts as a positional and rotational offset to use while held with a grip type grab")]
		public Transform gripOffset;

		[Tooltip("The local point which acts as a positional and rotational offset to use while held with a pinch type grab")]
		public Transform pinchOffset;

		protected override void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting();
			switch (grabStarting)
			{
			case GrabTypes.Pinch:
				hand.AttachObject(base.gameObject, grabStarting, attachmentFlags, pinchOffset);
				break;
			case GrabTypes.Grip:
				hand.AttachObject(base.gameObject, grabStarting, attachmentFlags, gripOffset);
				break;
			default:
				hand.AttachObject(base.gameObject, grabStarting, attachmentFlags, attachmentOffset);
				break;
			case GrabTypes.None:
				return;
			}
			hand.HideGrabHint();
		}

		protected override void HandAttachedUpdate(Hand hand)
		{
			if (interactable.skeletonPoser != null)
			{
				interactable.skeletonPoser.SetBlendingBehaviourEnabled("PinchPose", hand.currentAttachedObjectInfo.Value.grabbedWithType == GrabTypes.Pinch);
			}
			base.HandAttachedUpdate(hand);
		}
	}
}
