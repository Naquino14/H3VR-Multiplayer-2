namespace FistVR
{
	public class BoltActionMagEjectionTrigger : FVRInteractiveObject
	{
		public BoltActionRifle Rifle;

		public override bool IsInteractable()
		{
			if (Rifle.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Rifle.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Rifle.Magazine;
				Rifle.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
