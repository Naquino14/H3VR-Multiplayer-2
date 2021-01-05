namespace FistVR
{
	public class MF2_SapperSwitch : FVRInteractiveObject
	{
		public MF2_Sapper Sapper;

		public int WhichSwitch;

		public AudioEvent AudEvent_Switch;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			Sapper.ToggleSwitch(WhichSwitch);
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Switch, base.transform.position);
		}
	}
}
