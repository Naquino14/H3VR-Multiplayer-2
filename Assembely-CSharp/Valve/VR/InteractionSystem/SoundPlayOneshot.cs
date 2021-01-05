using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SoundPlayOneshot : MonoBehaviour
	{
		public AudioClip[] waveFiles;

		private AudioSource thisAudioSource;

		public float volMin;

		public float volMax;

		public float pitchMin;

		public float pitchMax;

		public bool playOnAwake;

		private void Awake()
		{
			thisAudioSource = GetComponent<AudioSource>();
			if (playOnAwake)
			{
				Play();
			}
		}

		public void Play()
		{
			if (thisAudioSource != null && thisAudioSource.isActiveAndEnabled && !Util.IsNullOrEmpty(waveFiles))
			{
				thisAudioSource.volume = Random.Range(volMin, volMax);
				thisAudioSource.pitch = Random.Range(pitchMin, pitchMax);
				thisAudioSource.PlayOneShot(waveFiles[Random.Range(0, waveFiles.Length)]);
			}
		}

		public void Pause()
		{
			if (thisAudioSource != null)
			{
				thisAudioSource.Pause();
			}
		}

		public void UnPause()
		{
			if (thisAudioSource != null)
			{
				thisAudioSource.UnPause();
			}
		}
	}
}
