using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New AudioImpactSet", menuName = "AudioPooling/HandlingGrabSet", order = 0)]
	public class HandlingGrabSet : ScriptableObject
	{
		public HandlingGrabType Type;

		public AudioEvent GrabSet_Light;

		public AudioEvent GrabSet_Hard;
	}
}
