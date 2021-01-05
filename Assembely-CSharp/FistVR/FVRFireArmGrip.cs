namespace FistVR
{
	public class FVRFireArmGrip : FVRInteractiveObject
	{
		public enum FireArmGripType
		{
			Fore,
			Mid,
			Rear,
			Handle
		}

		public FireArmGripType GripType = FireArmGripType.Mid;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
		}
	}
}
