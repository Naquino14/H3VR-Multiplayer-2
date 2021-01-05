namespace FistVR
{
	public class FlaregunToggler : FVRInteractiveObject
	{
		public Flaregun FG;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			FG.ToggleLatchState();
		}
	}
}
