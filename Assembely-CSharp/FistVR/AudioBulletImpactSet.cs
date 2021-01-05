using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New AudioBulletImpactSet", menuName = "AudioPooling/AudioBulletImpactSet", order = 0)]
	public class AudioBulletImpactSet : ScriptableObject
	{
		public BulletImpactSoundType Type;

		public AudioEvent AudEvent_Set;
	}
}
