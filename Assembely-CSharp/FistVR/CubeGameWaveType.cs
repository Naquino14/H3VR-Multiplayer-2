using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu]
	public class CubeGameWaveType : ScriptableObject
	{
		public CubeGameWaveElement[] Elements;

		public float TimeForWave = 10f;

		public float ReloadTimeAfter = 10f;
	}
}
