namespace FistVR
{
	public class AttachableNoiseAttachment : FVRFireArmAttachment
	{
		public NoiseGrip NG;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			NG.ProcessInput(hand, this);
			base.UpdateInteraction(hand);
		}
	}
}
