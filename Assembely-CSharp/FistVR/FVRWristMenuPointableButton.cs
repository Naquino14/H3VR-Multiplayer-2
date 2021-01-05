namespace FistVR
{
	public class FVRWristMenuPointableButton : FVRPointable
	{
		public int ButtonIndex;

		public FVRWristMenu WristMenu;

		public override void BeginHoverDisplay()
		{
			base.BeginHoverDisplay();
			WristMenu.SetSelectedButton(ButtonIndex);
		}

		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			if (hand.Input.TriggerDown)
			{
				WristMenu.InvokeButton(ButtonIndex);
			}
		}
	}
}
