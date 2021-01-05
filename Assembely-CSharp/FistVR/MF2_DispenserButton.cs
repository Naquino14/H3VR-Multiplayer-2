namespace FistVR
{
	public class MF2_DispenserButton : FVRInteractiveObject
	{
		public enum DispenserButtonType
		{
			Heal,
			Reload,
			Reset,
			Turret,
			Magazine
		}

		public enum DispenserSide
		{
			Front,
			Rear
		}

		public MF2_Dispenser Dispenser;

		public DispenserButtonType ButtonType;

		public DispenserSide Side;

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			Dispenser.ButtonPressed(ButtonType, Side);
		}
	}
}
