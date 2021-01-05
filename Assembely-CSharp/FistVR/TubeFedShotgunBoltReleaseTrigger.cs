namespace FistVR
{
	public class TubeFedShotgunBoltReleaseTrigger : FVRInteractiveObject
	{
		public TubeFedShotgun Shotgun;

		public override void Poke(FVRViveHand hand)
		{
			if (Shotgun.m_hand == null || hand != Shotgun.m_hand)
			{
				Shotgun.BoltReleasePressed();
			}
		}
	}
}
