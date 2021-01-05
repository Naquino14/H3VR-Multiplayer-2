namespace FistVR
{
	public class MuzzleNoiseMaker : MuzzleDevice
	{
		public bool PlaysTail = true;

		public AudioEvent AudEvent_SoundToPlay;

		public override void OnShot(FVRFireArm f, FVRTailSoundClass tailClass)
		{
			FVRFirearmAudioSet audioClipSet = f.AudioClipSet;
			if (PlaysTail)
			{
				f.PlayShotTail(FVRTailSoundClass.ExplosionBigDistant, FVRSoundEnvironment.InsideWarehouse);
				int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
				GM.CurrentSceneSettings.OnPerceiveableSound(200f, 80f, base.transform.position, playerIFF);
			}
			else
			{
				f.PlayAudioAsHandling(AudEvent_SoundToPlay, base.transform.position);
			}
			base.OnShot(f, tailClass);
		}
	}
}
