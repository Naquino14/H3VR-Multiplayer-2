using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class LinearAudioPitch : MonoBehaviour
	{
		public LinearMapping linearMapping;

		public AnimationCurve pitchCurve;

		public float minPitch;

		public float maxPitch;

		public bool applyContinuously = true;

		private AudioSource audioSource;

		private void Awake()
		{
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
			}
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
		}

		private void Update()
		{
			if (applyContinuously)
			{
				Apply();
			}
		}

		private void Apply()
		{
			float t = pitchCurve.Evaluate(linearMapping.value);
			audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
		}
	}
}
