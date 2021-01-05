namespace FistVR
{
	public class HandgunMagReleaseTrigger : FVRInteractiveObject
	{
		public Handgun Handgun;

		public override bool IsInteractable()
		{
			if (Handgun.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Handgun.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Handgun.Magazine;
				Handgun.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
