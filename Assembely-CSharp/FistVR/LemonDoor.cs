namespace FistVR
{
	public class LemonDoor : ZosigQuestManager
	{
		private ZosigGameManager M;

		public string FlagToOpen;

		private bool isUp = true;

		public AudioEvent AudEvent_DoorShut;

		public override void Init(ZosigGameManager m)
		{
			M = m;
			if (M.FlagM.GetFlagValue(FlagToOpen) > 0)
			{
				base.gameObject.SetActive(value: false);
				isUp = false;
			}
		}

		private void Update()
		{
			if (isUp && M.FlagM.GetFlagValue(FlagToOpen) > 0)
			{
				isUp = false;
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_DoorShut, base.transform.position);
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
