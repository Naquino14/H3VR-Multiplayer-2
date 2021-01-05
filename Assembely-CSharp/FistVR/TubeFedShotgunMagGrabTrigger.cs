namespace FistVR
{
	public class TubeFedShotgunMagGrabTrigger : FVRInteractiveObject
	{
		public TubeFedShotgun Shotgun;

		public override bool IsInteractable()
		{
			if (Shotgun.Magazine == null || Shotgun.HasExtractedRound())
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Shotgun.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Shotgun.Magazine;
				Shotgun.EjectMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
