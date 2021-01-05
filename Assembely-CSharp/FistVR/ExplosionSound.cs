using UnityEngine;

namespace FistVR
{
	public class ExplosionSound : MonoBehaviour
	{
		public AudioEvent AudioEvent_Explosion_Near;

		public AudioEvent AudioEvent_Explosion_Far;

		public FVRTailSoundClass NearTailClass = FVRTailSoundClass.Explosion;

		public FVRTailSoundClass DistantTailClass;

		public Vector2 TailPitchRangeNear = new Vector2(1f, 1f);

		public Vector2 TailPitchRangeFar = new Vector2(1f, 1f);

		public Vector2 TailPitchRangeDistant = new Vector2(1f, 1f);

		public bool FireOnStart;

		private bool m_waitingToExplode;

		private bool m_hasExploded;

		private float dist;

		private float delay;

		public float Loudness = 150f;

		public int IFF;

		private void Start()
		{
			if (FireOnStart)
			{
				Invoke("Explode", 0.05f);
			}
		}

		public void CancelSound()
		{
			CancelInvoke("Explode");
		}

		private void Explode()
		{
			dist = Vector3.Distance(GM.CurrentPlayerRoot.position, base.transform.position);
			delay = dist / 343f;
			ExplodeSound();
		}

		private void Update()
		{
		}

		public void ExplodeSound()
		{
			bool flag = SM.DoesReverbSystemExist();
			FVRReverbEnvironment fVRReverbEnvironment = null;
			FVRSoundEnvironment fVRSoundEnvironment;
			if (flag)
			{
				fVRReverbEnvironment = SM.GetReverbEnvironment(base.transform.position);
				fVRSoundEnvironment = fVRReverbEnvironment.Environment;
			}
			else
			{
				fVRSoundEnvironment = SM.GetSoundEnvironment(base.transform.position);
			}
			if (dist < 20f)
			{
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.Explosion, AudioEvent_Explosion_Near, base.transform.position, delay);
				SM.AudioSourcePool corePool = SM.GetCorePool(FVRPooledAudioType.ExplosionTail);
				AudioEvent tailSet = SM.GetTailSet(NearTailClass, fVRSoundEnvironment);
				FVRPooledAudioSource fVRPooledAudioSource = corePool.PlayClipVolumePitchOverride(tailSet, base.transform.position, 1f * tailSet.VolumeRange, TailPitchRangeNear);
				if ((flag && fVRReverbEnvironment == SM.GetPlayerReverbEnvironment()) || flag)
				{
					fVRPooledAudioSource.SetLowPassFreq(22000f);
				}
			}
			else if (dist < 100f)
			{
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.Explosion, AudioEvent_Explosion_Far, base.transform.position, delay);
				SM.AudioSourcePool corePool2 = SM.GetCorePool(FVRPooledAudioType.ExplosionTail);
				AudioEvent tailSet2 = SM.GetTailSet(NearTailClass, fVRSoundEnvironment);
				float num = Mathf.Lerp(1f, 0.6f, (dist - 20f) / 80f);
				corePool2.PlayDelayedClip(delay, tailSet2, base.transform.position, num * tailSet2.VolumeRange, TailPitchRangeFar);
			}
			else
			{
				SM.AudioSourcePool corePool3 = SM.GetCorePool(FVRPooledAudioType.ExplosionTail);
				AudioEvent tailSet3 = SM.GetTailSet(DistantTailClass, fVRSoundEnvironment);
				corePool3.PlayDelayedClip(delay, tailSet3, base.transform.position, tailSet3.VolumeRange, TailPitchRangeDistant);
			}
			Vector3 position = base.transform.position;
			GM.CurrentSceneSettings.OnPerceiveableSound(Loudness, Loudness * SM.GetSoundTravelDistanceMultByEnvironment(fVRSoundEnvironment), position, IFF);
			GM.CurrentSceneSettings.OnSuppressingEvent(position, Vector3.up, IFF, Mathf.Clamp(Loudness * 0.01f, 1f, 2f), Mathf.Clamp(Loudness * 0.1f, 5f, 20f));
		}
	}
}
