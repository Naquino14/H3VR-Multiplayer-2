using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Clip Set", menuName = "AudioPooling/PooledClipSet", order = 0)]
	public class FVRPooledAudioClipSet : ScriptableObject
	{
		public AudioEvent AudioEvent;
	}
}
