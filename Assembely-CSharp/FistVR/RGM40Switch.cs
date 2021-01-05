namespace FistVR
{
	public class RGM40Switch : FVRInteractiveObject
	{
		public RGM40 Launcher;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			Launcher.SafeEject();
			base.SimpleInteraction(hand);
		}
	}
}
