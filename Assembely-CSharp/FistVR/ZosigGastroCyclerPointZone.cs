namespace FistVR
{
	public class ZosigGastroCyclerPointZone : FVRPointable
	{
		public ZosigGastroCycler Cycler;

		public int PointIndex;

		public bool IsEnabled;

		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			if (IsEnabled)
			{
				Cycler.InitiatePoint(PointIndex);
				if (hand.Input.TriggerDown)
				{
					Cycler.ClickPoint(PointIndex);
				}
			}
		}

		public override void EndPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			Cycler.EndPoint(PointIndex);
		}
	}
}
