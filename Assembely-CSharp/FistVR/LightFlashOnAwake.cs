using UnityEngine;

namespace FistVR
{
	public class LightFlashOnAwake : MonoBehaviour
	{
		public float Intensity;

		public float Range;

		public Color C;

		private void Start()
		{
			FXM.InitiateMuzzleFlash(base.transform.position, Vector3.up, Intensity, C, Range);
		}
	}
}
