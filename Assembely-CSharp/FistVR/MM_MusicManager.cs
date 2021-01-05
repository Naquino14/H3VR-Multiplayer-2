using UnityEngine;

namespace FistVR
{
	public class MM_MusicManager : MonoBehaviour
	{
		public AudioSource Music;

		public AudioSource AmbientMusic;

		private float tarVolume = 0.25f;

		private float tarVolumeAmbient = 0.1f;

		private bool m_isMusicEnabled = true;

		private void Start()
		{
			Music.volume = 0f;
			GM.CurrentSceneSettings.PlayerDeathEvent += PlayerDied;
			AmbientMusic.volume = 0f;
		}

		public void PlayMusic()
		{
			Music.volume = 0.01f;
			tarVolume = 0.25f;
			tarVolumeAmbient = 0f;
			if (!Music.isPlaying)
			{
				Music.Play();
			}
		}

		public void FadeOutMusic()
		{
			AmbientMusic.volume = 0.01f;
			tarVolume = 0f;
			tarVolumeAmbient = 0.1f;
			if (!AmbientMusic.isPlaying)
			{
				AmbientMusic.Play();
			}
		}

		private void Update()
		{
			if (Music.isPlaying)
			{
				Music.volume = Mathf.MoveTowards(Music.volume, tarVolume, 0.2f * Time.deltaTime);
				if (Music.volume <= 0f)
				{
					Music.Stop();
				}
			}
			if (AmbientMusic.isPlaying)
			{
				AmbientMusic.volume = Mathf.MoveTowards(AmbientMusic.volume, tarVolumeAmbient, 0.2f * Time.deltaTime);
				if (AmbientMusic.volume <= 0f)
				{
					AmbientMusic.Stop();
				}
			}
		}

		public bool IsMusicEnabled()
		{
			return m_isMusicEnabled;
		}

		public void DisableMusic()
		{
			if (m_isMusicEnabled)
			{
				m_isMusicEnabled = false;
				Music.enabled = false;
				AmbientMusic.enabled = false;
			}
		}

		public void EnableMusic()
		{
			if (!m_isMusicEnabled)
			{
				m_isMusicEnabled = true;
				Music.enabled = true;
				AmbientMusic.enabled = true;
				FadeOutMusic();
			}
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.PlayerDeathEvent -= PlayerDied;
		}

		private void PlayerDied(bool killedSelf)
		{
			FadeOutMusic();
		}
	}
}
