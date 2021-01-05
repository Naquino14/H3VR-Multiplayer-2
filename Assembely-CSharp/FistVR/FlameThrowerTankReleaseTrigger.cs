namespace FistVR
{
	public class FlameThrowerTankReleaseTrigger : FVRInteractiveObject
	{
		public FVRFireArm FA;

		public override bool IsInteractable()
		{
			if (FA.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (FA.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = FA.Magazine;
				FA.EjectMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
