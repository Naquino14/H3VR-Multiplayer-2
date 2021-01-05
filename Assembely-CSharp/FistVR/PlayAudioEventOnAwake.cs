using UnityEngine;

namespace FistVR
{
	public class PlayAudioEventOnAwake : MonoBehaviour
	{
		public AudioEvent AudioEvent;

		public bool IsDelayed;

		public FVRPooledAudioType Type = FVRPooledAudioType.GenericLongRange;

		private void Start()
		{
			if (IsDelayed)
			{
				float num = Vector3.Distance(GM.CurrentPlayerRoot.position, base.transform.position);
				float delay = num / 343f;
				SM.PlayCoreSoundDelayed(Type, AudioEvent, base.transform.position, delay);
			}
			else
			{
				SM.PlayCoreSound(Type, AudioEvent, base.transform.position);
			}
		}
	}
}
