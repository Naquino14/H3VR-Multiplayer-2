namespace FistVR
{
	public class GP25_SimpleSwitch : FVRInteractiveObject
	{
		public enum GP25SwitchType
		{
			Safety,
			Ejector
		}

		public GP25 Launcher;

		public GP25SwitchType Type;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			if (Type == GP25SwitchType.Safety)
			{
				Launcher.ToggleSafety();
			}
			else if (Type == GP25SwitchType.Ejector)
			{
				Launcher.SafeEject();
			}
			base.SimpleInteraction(hand);
		}
	}
}
