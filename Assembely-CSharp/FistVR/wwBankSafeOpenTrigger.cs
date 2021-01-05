namespace FistVR
{
	public class wwBankSafeOpenTrigger : FVRInteractiveObject
	{
		public wwBankSafe Safe;

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			if (Safe.SafeState == 1)
			{
				Safe.SetState(2, playSound: true, stateEvent: true);
			}
		}
	}
}
