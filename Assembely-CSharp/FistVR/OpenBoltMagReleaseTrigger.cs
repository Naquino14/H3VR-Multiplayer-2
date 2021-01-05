namespace FistVR
{
	public class OpenBoltMagReleaseTrigger : FVRInteractiveObject
	{
		public OpenBoltReceiver Receiver;

		public bool IsBeltBox;

		public override bool IsInteractable()
		{
			if (Receiver.Magazine == null)
			{
				return false;
			}
			if (Receiver.BeltDD != null && Receiver.BeltDD.isBeltGrabbed())
			{
				return false;
			}
			if (Receiver.Magazine.IsBeltBox == IsBeltBox && (!Receiver.ConnectedToBox || Receiver.Magazine.CanBeTornOut))
			{
				return true;
			}
			return false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Receiver.Magazine != null)
			{
				EndInteraction(hand);
				FVRFireArmMagazine magazine = Receiver.Magazine;
				if (Receiver.Magazine.IsBeltBox && Receiver.ConnectedToBox && Receiver.Magazine.CanBeTornOut)
				{
					Receiver.BeltDD.ForceRelease();
					Receiver.Magazine.UpdateBulletDisplay();
				}
				Receiver.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}
	}
}
