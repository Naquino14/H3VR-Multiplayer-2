namespace FistVR
{
	public class FVRCappedGrenadeCap : FVRInteractiveObject
	{
		public FVRCappedGrenade Grenade;

		public bool IsPrimaryCap = true;

		public override bool IsInteractable()
		{
			if (Grenade.QuickbeltSlot != null)
			{
				return false;
			}
			if (!IsPrimaryCap && !Grenade.IsPrimaryCapRemoved)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			Grenade.CapRemoved(IsPrimaryCap, hand, this);
		}
	}
}
