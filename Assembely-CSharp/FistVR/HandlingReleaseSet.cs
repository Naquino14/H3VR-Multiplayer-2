using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New AudioImpactSet", menuName = "AudioPooling/HandlingReleaseSet", order = 0)]
	public class HandlingReleaseSet : ScriptableObject
	{
		public HandlingReleaseType Type;

		public AudioEvent ReleaseSet;
	}
}
