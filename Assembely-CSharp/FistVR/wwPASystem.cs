using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class wwPASystem : MonoBehaviour
	{
		public enum PA_State
		{
			Music,
			Jingle1,
			Announcement,
			Jingle2
		}

		public AudioSource AudioSource;

		public AudioClip PA_Jingle;

		public AudioClip[] PA_Announcement;

		public AudioClip[] PianoMusic;

		public List<Transform> PASourcePoints;

		public Transform Player;

		public PA_State State;

		private bool m_isPASystemActive = true;

		private bool m_isInSuppressedMode;

		private bool m_isInSilentMode;

		private float m_ModeMultLerp = 1f;

		private void SetRandomPianoTrack()
		{
			int num = Random.Range(1, PianoMusic.Length);
			AudioClip audioClip = PianoMusic[num];
			PianoMusic[num] = PianoMusic[0];
			PianoMusic[0] = audioClip;
		}

		private void SetRandomPA_Announcement()
		{
			int num = Random.Range(1, PA_Announcement.Length);
			AudioClip audioClip = PA_Announcement[num];
			PA_Announcement[num] = PA_Announcement[0];
			PA_Announcement[0] = audioClip;
		}

		private void PlayMusic()
		{
			State = PA_State.Music;
			SetRandomPianoTrack();
			AudioSource.clip = PianoMusic[0];
			AudioSource.Play();
		}

		public void DestroySpeaker(wwExplodeableSpeaker s)
		{
			PASourcePoints.Remove(s.transform);
		}

		private void PlayJingle1()
		{
			State = PA_State.Jingle1;
			AudioSource.clip = PA_Jingle;
			AudioSource.Play();
		}

		private void PlayAnnouncement()
		{
			State = PA_State.Announcement;
			SetRandomPA_Announcement();
			AudioSource.clip = PA_Announcement[0];
			AudioSource.Play();
		}

		private void PlayJingle2()
		{
			State = PA_State.Jingle2;
			AudioSource.clip = PA_Jingle;
			AudioSource.Play();
		}

		public void Awake()
		{
			PlayMusic();
		}

		private void Start()
		{
		}

		private void Update()
		{
			ModeLerp();
			DetermineVolume();
			if (m_isPASystemActive)
			{
				PASystemUpdate();
			}
		}

		public void EngageStandardMode()
		{
			m_isInSuppressedMode = false;
			m_isInSilentMode = false;
		}

		public void EngageSuppressedMode()
		{
			m_isInSuppressedMode = true;
		}

		public void EngageSilentMode()
		{
			m_isInSilentMode = true;
		}

		public void DisEngageSuppressedMode()
		{
			m_isInSuppressedMode = false;
		}

		public void DisEngageSilentMode()
		{
			m_isInSilentMode = false;
		}

		private void ModeLerp()
		{
			if (m_isInSilentMode)
			{
				m_ModeMultLerp = Mathf.MoveTowards(m_ModeMultLerp, 0f, Time.deltaTime * 0.5f);
			}
			else if (m_isInSuppressedMode)
			{
				m_ModeMultLerp = Mathf.MoveTowards(m_ModeMultLerp, 0.25f, Time.deltaTime * 0.5f);
			}
			else
			{
				m_ModeMultLerp = Mathf.MoveTowards(m_ModeMultLerp, 1f, Time.deltaTime * 0.5f);
			}
		}

		private void PASystemUpdate()
		{
			switch (State)
			{
			case PA_State.Music:
				if (!AudioSource.isPlaying)
				{
					PlayJingle1();
				}
				break;
			case PA_State.Jingle1:
				if (!AudioSource.isPlaying)
				{
					PlayAnnouncement();
				}
				break;
			case PA_State.Announcement:
				if (!AudioSource.isPlaying)
				{
					PlayJingle2();
				}
				break;
			case PA_State.Jingle2:
				if (!AudioSource.isPlaying)
				{
					PlayMusic();
				}
				break;
			}
		}

		private void DetermineVolume()
		{
			float num = 1f;
			float num2 = 200f;
			bool flag = false;
			for (int i = 0; i < PASourcePoints.Count; i++)
			{
				if (!(PASourcePoints[i] == null))
				{
					Vector3 vector = new Vector3(Player.position.x, 0f, Player.position.z);
					Vector3 vector2 = new Vector3(PASourcePoints[i].position.x, 0f, PASourcePoints[i].position.z);
					Vector3 from = new Vector3(PASourcePoints[i].forward.x, 0f, PASourcePoints[i].forward.z);
					Vector3 to = vector - vector2;
					float num3 = Vector3.Angle(from, to);
					float num4 = num3 / 180f;
					float num5 = to.magnitude * (1f + num4 * 0.75f);
					if (num5 < num2)
					{
						num2 = num5;
					}
				}
			}
			num = 1f - num2 / 50f;
			float num6 = Mathf.Clamp(num * 0.35f, 0f, 0.25f);
			AudioSource.volume = num6 * m_ModeMultLerp;
		}
	}
}
