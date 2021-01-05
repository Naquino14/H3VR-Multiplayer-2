namespace FistVR
{
	public class SingleActionEjectorRod : FVRInteractiveObject
	{
		public SingleActionRevolver Revolver;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			Revolver.EjectPrevCylinder();
		}
	}
}
