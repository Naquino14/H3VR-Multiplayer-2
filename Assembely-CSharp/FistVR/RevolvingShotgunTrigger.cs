namespace FistVR
{
	public class RevolvingShotgunTrigger : FVRInteractiveObject
	{
		public enum TrigType
		{
			Loading,
			Grabbing,
			CockHammer
		}

		public RevolvingShotgun Shotgun;

		public TrigType TType;

		public override bool IsInteractable()
		{
			if (TType == TrigType.Grabbing && Shotgun.CylinderLoaded)
			{
				return true;
			}
			return false;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (TType == TrigType.CockHammer)
			{
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (TType == TrigType.Grabbing && Shotgun.CylinderLoaded)
			{
				EndInteraction(hand);
				Speedloader speedloader = Shotgun.EjectCylinder();
				hand.ForceSetInteractable(speedloader);
				speedloader.BeginInteraction(hand);
			}
		}
	}
}
