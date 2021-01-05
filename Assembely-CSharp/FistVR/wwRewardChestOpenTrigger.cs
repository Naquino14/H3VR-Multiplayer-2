namespace FistVR
{
	public class wwRewardChestOpenTrigger : FVRInteractiveObject
	{
		public wwRewardChest Chest;

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			Chest.OpenChest();
		}
	}
}
