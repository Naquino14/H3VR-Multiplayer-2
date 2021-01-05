using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FistVR
{
	public class ZosigMusicController : MonoBehaviour
	{
		[Serializable]
		public class ZosigTrack
		{
			public ZosigMusicController C;

			public ZosigTrackName Track;

			public float CurIntensity;

			public float TarIntensity;

			public float StopValue;

			public float StopCooldown;

			private bool m_isWaitingToStart;

			private bool m_isPlaying;

			public void AttemptToStart()
			{
				if (StopCooldown > 0f)
				{
					m_isWaitingToStart = true;
					return;
				}
				m_isWaitingToStart = false;
				Start();
			}

			private void Start()
			{
				if (!m_isPlaying)
				{
					StopCooldown = 0f;
					m_isPlaying = true;
					C.SetIntParameterByIndex((int)Track, "Stop", 0f);
					CurIntensity = 0f;
					TarIntensity = 0f;
					C.musictracks[(int)Track].start();
				}
			}

			public void Stop()
			{
				m_isWaitingToStart = false;
				if (m_isPlaying)
				{
					m_isPlaying = false;
					StopValue = 1f;
					StopCooldown = 10f;
					C.SetIntParameterByIndex((int)Track, "Stop", 1f);
				}
			}

			public void Tick(float time)
			{
				if (StopCooldown > 0f)
				{
					StopCooldown -= Time.deltaTime;
					if (StopCooldown <= 0f)
					{
						EventInstance eventInstance = C.musictracks[(int)Track];
						C.StopEventByIndex((int)Track, shouldDeadStop: false);
					}
				}
				if (m_isWaitingToStart && StopCooldown <= 0f)
				{
					m_isWaitingToStart = false;
					Start();
				}
				CurIntensity = Mathf.MoveTowards(CurIntensity, TarIntensity, time);
				C.SetIntParameterByIndex((int)Track, "ROTW Intensity", CurIntensity);
			}
		}

		public enum ZosigTrackName
		{
			HBH,
			BCW,
			Lakeside,
			Graveyard,
			OldMines,
			Wienerton,
			FortWienerton,
			OldWienerton,
			EastWienerton
		}

		[EventRef]
		public List<string> StateEvents;

		public List<EventInstance> musictracks = new List<EventInstance>();

		private Bus MasterBus;

		private EventInstance m_currentMusicEvent;

		private int m_currentMusicIndex = -1;

		private bool m_isTransitioning;

		private int m_nextMusicIndex = -1;

		private float m_musicTickDown;

		private string BankPreload = "MX_ROTW";

		public List<ZosigTrack> Tracks = new List<ZosigTrack>();

		private float m_intensityRaw;

		private ZosigTrack m_currentTrack;

		public void Start()
		{
			GM.CurrentSceneSettings.SosigKillEvent += DeadSosigDetected;
			GM.CurrentSceneSettings.ShotFiredEvent += ShotFired;
			GM.CurrentSceneSettings.PlayerTookDamageEvent += PlayerTookDamage;
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= DeadSosigDetected;
			GM.CurrentSceneSettings.ShotFiredEvent -= ShotFired;
			GM.CurrentSceneSettings.PlayerTookDamageEvent -= PlayerTookDamage;
			StopAllEvents();
		}

		public void PlayerTookDamage(float percent)
		{
			m_intensityRaw += percent * 20f;
		}

		public void ShotFired(FVRFireArm firearm)
		{
			m_intensityRaw += 0.2f;
		}

		public void DeadSosigDetected(Sosig s)
		{
			m_intensityRaw += 3f;
		}

		public void Init()
		{
			MasterBus = RuntimeManager.GetBus("bus:/Music");
			for (int i = 0; i < Tracks.Count; i++)
			{
				Tracks[i].C = this;
			}
			Initialize();
		}

		public void SetMasterVolume(float i)
		{
			MasterBus.setVolume(i);
		}

		public void SetIntensity(float intensity)
		{
			for (int i = 0; i < Tracks.Count; i++)
			{
				Tracks[i].TarIntensity = intensity;
			}
		}

		public void Initialize()
		{
			RuntimeManager.LoadBank(BankPreload);
			for (int i = 0; i < StateEvents.Count; i++)
			{
				EventInstance item = RuntimeManager.CreateInstance(StateEvents[i]);
				musictracks.Add(item);
			}
		}

		public void SwitchToTrack(ZosigTrackName track)
		{
			if (m_currentTrack != null)
			{
				if (m_currentTrack.Track != track)
				{
					m_currentTrack.Stop();
					m_currentTrack = Tracks[(int)track];
					m_currentTrack.AttemptToStart();
				}
			}
			else
			{
				m_currentTrack = Tracks[(int)track];
				m_currentTrack.AttemptToStart();
			}
		}

		public void Tick(float t)
		{
			for (int i = 0; i < Tracks.Count; i++)
			{
				Tracks[i].Tick(t);
			}
			m_intensityRaw = Mathf.Clamp(m_intensityRaw, 0f, 10f);
			m_intensityRaw -= Time.deltaTime * 0.4f;
			float value = m_intensityRaw * 0.3f;
			value = Mathf.Clamp(value, 0f, 2f);
			SetIntensity(value);
		}

		public void SetIntParameterByIndex(int i, string s, float f)
		{
			SetIntParameter(musictracks[i], s, f);
		}

		public void SetIntParameter(EventInstance e, string s, float f)
		{
			e.setParameterValue(s, f);
		}

		public void StopEventByIndex(int i, bool shouldDeadStop)
		{
			EventInstance eventInstance = musictracks[i];
			if (shouldDeadStop)
			{
				eventInstance.stop(STOP_MODE.IMMEDIATE);
			}
			else
			{
				eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
			}
		}

		private void StopAllEvents()
		{
			MasterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
		}
	}
}
