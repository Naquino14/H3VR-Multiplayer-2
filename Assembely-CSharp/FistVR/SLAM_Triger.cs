namespace FistVR
{
	public class SLAM_Triger : FVRInteractiveObject
	{
		public SLAM S;

		public bool IsFlipTrigger;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			S.TriggerFlipped(IsFlipTrigger);
			base.SimpleInteraction(hand);
		}
	}
}
