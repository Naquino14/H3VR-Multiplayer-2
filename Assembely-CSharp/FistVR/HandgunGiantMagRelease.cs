namespace FistVR
{
	public class HandgunGiantMagRelease : FVRInteractiveObject
	{
		public Handgun Handgun;

		public override void Poke(FVRViveHand hand)
		{
			if (Handgun.m_hand == null || hand != Handgun.m_hand)
			{
				Handgun.EjectMag();
			}
			base.Poke(hand);
		}
	}
}
