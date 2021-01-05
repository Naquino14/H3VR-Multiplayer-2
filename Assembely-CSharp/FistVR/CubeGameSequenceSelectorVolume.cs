namespace FistVR
{
	public class CubeGameSequenceSelectorVolume : FVRInteractiveObject
	{
		public CubeGameSequenceSelectorv1 Selector;

		public int Num;

		public override void Poke(FVRViveHand hand)
		{
			Selector.SelectSequence(Num);
		}
	}
}
