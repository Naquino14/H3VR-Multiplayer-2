using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu]
	public class FVRHapticBuzzProfile : ScriptableObject
	{
		public AnimationCurve BuzzCurve;

		public float BuzzLength;

		public int Freq;

		public float AmpMult = 0.5f;
	}
}
