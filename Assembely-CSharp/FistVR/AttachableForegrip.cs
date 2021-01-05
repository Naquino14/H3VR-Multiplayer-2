using UnityEngine;

namespace FistVR
{
	public class AttachableForegrip : FVRFireArmAttachmentInterface
	{
		public Transform ForePosePoint;

		public FVRFireArm OverrideFirearm;

		public bool DoesBracing = true;

		public override void BeginInteraction(FVRViveHand hand)
		{
			FVRFireArm fVRFireArm = OverrideFirearm;
			if (fVRFireArm == null)
			{
				fVRFireArm = Attachment.GetRootObject() as FVRFireArm;
			}
			if (fVRFireArm != null && fVRFireArm.Foregrip != null)
			{
				FVRAlternateGrip component = fVRFireArm.Foregrip.GetComponent<FVRAlternateGrip>();
				hand.ForceSetInteractable(component);
				component.BeginInteractionFromAttachedGrip(this, hand);
			}
		}

		public virtual void PassHandInput(FVRViveHand hand, FVRInteractiveObject o)
		{
		}
	}
}
