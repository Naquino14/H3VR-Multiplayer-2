using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New AudioImpactSet", menuName = "AudioPooling/HandlingReleaseIntoSlotSet", order = 0)]
	public class HandlingReleaseIntoSlotSet : ScriptableObject
	{
		public HandlingReleaseIntoSlotType Type;

		public AudioEvent ReleaseIntoSlotSet;
	}
}
