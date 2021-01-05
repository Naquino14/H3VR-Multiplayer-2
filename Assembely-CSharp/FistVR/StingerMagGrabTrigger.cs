namespace FistVR
{
	public class StingerMagGrabTrigger : FVRInteractiveObject
	{
		public StingerLauncher Launcher;

		public override bool IsInteractable()
		{
			if (Launcher.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Launcher.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Launcher.Magazine;
				Launcher.EjectMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
