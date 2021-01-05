namespace FistVR
{
	public class FVRFireArmBeltRemovalTrigger : FVRInteractiveObject
	{
		public FVRFireArm FireArm;

		public override bool IsInteractable()
		{
			if (!FireArm.HasBelt)
			{
				return false;
			}
			if (FireArm.UsesTopCover && !FireArm.IsTopCoverUp)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (FireArm.ConnectedToBox && FireArm.Magazine != null)
			{
				FireArm.HasBelt = false;
				FireArm.ConnectedToBox = false;
				hand.ForceSetInteractable(FireArm.Magazine.BeltGrabTrigger);
				FireArm.Magazine.BeltGrabTrigger.BeginInteraction(hand);
			}
			else
			{
				FVRFireArmBeltSegment fVRFireArmBeltSegment = FireArm.BeltDD.StripBeltSegment(hand.Input.Pos);
				hand.ForceSetInteractable(fVRFireArmBeltSegment);
				fVRFireArmBeltSegment.BeginInteraction(hand);
			}
		}
	}
}
