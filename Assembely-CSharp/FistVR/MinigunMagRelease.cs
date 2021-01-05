namespace FistVR
{
	public class MinigunMagRelease : FVRInteractiveObject
	{
		public FVRFireArm Firearm;

		private bool ShouldRelease;

		private FVRViveHand m_handy;

		public override bool IsInteractable()
		{
			if (Firearm.Magazine == null)
			{
				return false;
			}
			return true;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			ShouldRelease = true;
			m_handy = hand;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (ShouldRelease && m_handy != null)
			{
				if (Firearm.Magazine != null)
				{
					EndInteraction(m_handy);
					FVRFireArmMagazine magazine = Firearm.Magazine;
					Firearm.EjectMag();
					m_handy.ForceSetInteractable(magazine);
					magazine.BeginInteraction(m_handy);
				}
				m_handy = null;
				ShouldRelease = false;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
		}
	}
}
