namespace FistVR
{
	public class ClosedBoltReleaseButton : FVRInteractiveObject
	{
		public ClosedBoltWeapon Weapon;

		public override void Poke(FVRViveHand hand)
		{
			if (Weapon.m_hand == null || hand != Weapon.m_hand)
			{
				Weapon.Bolt.ReleaseBolt();
			}
		}
	}
}
