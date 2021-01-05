namespace FistVR
{
	public class PotatoGunMagGrabTrigger : FVRInteractiveObject
	{
		public PotatoGun Patoot;

		public override bool IsInteractable()
		{
			if (Patoot.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Patoot.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Patoot.Magazine;
				Patoot.EjectMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
