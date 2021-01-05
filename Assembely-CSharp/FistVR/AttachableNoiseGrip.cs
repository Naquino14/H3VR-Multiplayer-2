namespace FistVR
{
	public class AttachableNoiseGrip : AttachableForegrip
	{
		public NoiseGrip NG;

		public override void PassHandInput(FVRViveHand hand, FVRInteractiveObject o)
		{
			NG.ProcessInput(hand, o);
			base.PassHandInput(hand, o);
		}
	}
}
