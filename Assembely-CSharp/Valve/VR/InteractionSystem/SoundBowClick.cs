using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SoundBowClick : MonoBehaviour
	{
		public AudioClip bowClick;

		public AnimationCurve pitchTensionCurve;

		public float minPitch;

		public float maxPitch;

		private AudioSource thisAudioSource;

		private void Awake()
		{
			thisAudioSource = GetComponent<AudioSource>();
		}

		public void PlayBowTensionClicks(float normalizedTension)
		{
			float num = pitchTensionCurve.Evaluate(normalizedTension);
			thisAudioSource.pitch = (maxPitch - minPitch) * num + minPitch;
			thisAudioSource.PlayOneShot(bowClick);
		}
	}
}
