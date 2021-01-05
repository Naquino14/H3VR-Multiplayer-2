namespace FistVR
{
	public class DetpackButton : FVRInteractiveObject
	{
		public MF2_Detpack Det;

		public bool isStart = true;

		public override void Poke(FVRViveHand hand)
		{
			if (!(Det.QuickbeltSlot != null))
			{
				base.Poke(hand);
				if (isStart)
				{
					Det.InitiateCountDown(hand);
				}
				else
				{
					Det.ResetCountDown(hand);
				}
			}
		}
	}
}
