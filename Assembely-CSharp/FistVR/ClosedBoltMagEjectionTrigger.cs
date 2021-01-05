namespace FistVR
{
	public class ClosedBoltMagEjectionTrigger : FVRInteractiveObject
	{
		public ClosedBoltWeapon Receiver;

		public override bool IsInteractable()
		{
			if (Receiver.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Receiver.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Receiver.Magazine;
				Receiver.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.Input.TouchpadDown && Receiver.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Receiver.Magazine;
				Receiver.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
