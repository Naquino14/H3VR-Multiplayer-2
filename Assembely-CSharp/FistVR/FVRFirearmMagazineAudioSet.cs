using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Audio Set", menuName = "AudioPooling/FireArmMagazineAudioSet", order = 0)]
	public class FVRFirearmMagazineAudioSet : ScriptableObject
	{
		public AudioEvent MagazineIn;

		public AudioEvent MagazineOut;

		public AudioEvent MagazineInsertRound;

		public AudioEvent MagazineEjectRound;
	}
}
