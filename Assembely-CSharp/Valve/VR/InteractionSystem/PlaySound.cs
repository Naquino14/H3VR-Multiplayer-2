using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(AudioSource))]
	public class PlaySound : MonoBehaviour
	{
		[Tooltip("List of audio clips to play.")]
		public AudioClip[] waveFile;

		[Tooltip("Stops the currently playing clip in the audioSource. Otherwise clips will overlap/mix.")]
		public bool stopOnPlay;

		[Tooltip("After the audio clip finishes playing, disable the game object the sound is on.")]
		public bool disableOnEnd;

		[Tooltip("Loop the sound after the wave file variation has been chosen.")]
		public bool looping;

		[Tooltip("If the sound is looping and updating it's position every frame, stop the sound at the end of the wav/clip length. ")]
		public bool stopOnEnd;

		[Tooltip("Start a wave file playing on awake, but after a delay.")]
		public bool playOnAwakeWithDelay;

		[Header("Random Volume")]
		public bool useRandomVolume = true;

		[Tooltip("Minimum volume that will be used when randomly set.")]
		[Range(0f, 1f)]
		public float volMin = 1f;

		[Tooltip("Maximum volume that will be used when randomly set.")]
		[Range(0f, 1f)]
		public float volMax = 1f;

		[Header("Random Pitch")]
		[Tooltip("Use min and max random pitch levels when playing sounds.")]
		public bool useRandomPitch = true;

		[Tooltip("Minimum pitch that will be used when randomly set.")]
		[Range(-3f, 3f)]
		public float pitchMin = 1f;

		[Tooltip("Maximum pitch that will be used when randomly set.")]
		[Range(-3f, 3f)]
		public float pitchMax = 1f;

		[Header("Random Time")]
		[Tooltip("Use Retrigger Time to repeat the sound within a time range")]
		public bool useRetriggerTime;

		[Tooltip("Inital time before the first repetion starts")]
		[Range(0f, 360f)]
		public float timeInitial;

		[Tooltip("Minimum time that will pass before the sound is retriggered")]
		[Range(0f, 360f)]
		public float timeMin;

		[Tooltip("Maximum pitch that will be used when randomly set.")]
		[Range(0f, 360f)]
		public float timeMax;

		[Header("Random Silence")]
		[Tooltip("Use Retrigger Time to repeat the sound within a time range")]
		public bool useRandomSilence;

		[Tooltip("Percent chance that the wave file will not play")]
		[Range(0f, 1f)]
		public float percentToNotPlay;

		[Header("Delay Time")]
		[Tooltip("Time to offset playback of sound")]
		public float delayOffsetTime;

		private AudioSource audioSource;

		private AudioClip clip;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			clip = audioSource.clip;
			if (audioSource.playOnAwake)
			{
				if (useRetriggerTime)
				{
					InvokeRepeating("Play", timeInitial, Random.Range(timeMin, timeMax));
				}
				else
				{
					Play();
				}
			}
			else if (!audioSource.playOnAwake && playOnAwakeWithDelay)
			{
				PlayWithDelay(delayOffsetTime);
				if (useRetriggerTime)
				{
					InvokeRepeating("Play", timeInitial, Random.Range(timeMin, timeMax));
				}
			}
			else if (audioSource.playOnAwake && playOnAwakeWithDelay)
			{
				PlayWithDelay(delayOffsetTime);
				if (useRetriggerTime)
				{
					InvokeRepeating("Play", timeInitial, Random.Range(timeMin, timeMax));
				}
			}
		}

		public void Play()
		{
			if (looping)
			{
				PlayLooping();
			}
			else
			{
				PlayOneShotSound();
			}
		}

		public void PlayWithDelay(float delayTime)
		{
			if (looping)
			{
				Invoke("PlayLooping", delayTime);
			}
			else
			{
				Invoke("PlayOneShotSound", delayTime);
			}
		}

		public AudioClip PlayOneShotSound()
		{
			if (!audioSource.isActiveAndEnabled)
			{
				return null;
			}
			SetAudioSource();
			if (stopOnPlay)
			{
				audioSource.Stop();
			}
			if (disableOnEnd)
			{
				Invoke("Disable", clip.length);
			}
			audioSource.PlayOneShot(clip);
			return clip;
		}

		public AudioClip PlayLooping()
		{
			SetAudioSource();
			if (!audioSource.loop)
			{
				audioSource.loop = true;
			}
			audioSource.Play();
			if (stopOnEnd)
			{
				Invoke("Stop", audioSource.clip.length);
			}
			return clip;
		}

		public void Disable()
		{
			base.gameObject.SetActive(value: false);
		}

		public void Stop()
		{
			audioSource.Stop();
		}

		private void SetAudioSource()
		{
			if (useRandomVolume)
			{
				audioSource.volume = Random.Range(volMin, volMax);
				if (useRandomSilence && (float)Random.Range(0, 1) < percentToNotPlay)
				{
					audioSource.volume = 0f;
				}
			}
			if (useRandomPitch)
			{
				audioSource.pitch = Random.Range(pitchMin, pitchMax);
			}
			if (waveFile.Length > 0)
			{
				audioSource.clip = waveFile[Random.Range(0, waveFile.Length)];
				clip = audioSource.clip;
			}
		}
	}
}
