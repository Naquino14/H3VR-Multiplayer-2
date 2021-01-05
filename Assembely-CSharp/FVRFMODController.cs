using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FVRFMODController : MonoBehaviour
{
	[EventRef]
	public List<string> StateEvents;

	public List<EventInstance> musictracks = new List<EventInstance>();

	private Bus MasterBus;

	private EventInstance m_currentMusicEvent;

	private int m_currentMusicIndex = -1;

	private bool m_isTransitioning;

	private int m_nextMusicIndex = -1;

	private float m_musicTickDown;

	private string BankPreload = "MX_TAH";

	public void Start()
	{
		MasterBus = RuntimeManager.GetBus("bus:/Music");
		Initialize();
	}

	public void SetMasterVolume(float i)
	{
		MasterBus.setVolume(i);
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

	public void Tick(float t)
	{
		if (m_isTransitioning)
		{
			m_musicTickDown -= t;
			if (m_musicTickDown <= 0f)
			{
				m_isTransitioning = false;
				m_currentMusicIndex = m_nextMusicIndex;
				EventInstance e = musictracks[m_currentMusicIndex];
				e.start();
				SetIntParameter(e, "Intensity", 1f);
			}
		}
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

	public void SwitchTo(int musicIndex, float timeDelayStart, bool shouldStop, bool shouldDeadStop)
	{
		if (m_isTransitioning)
		{
			return;
		}
		EventInstance e = musictracks[musicIndex];
		if (m_currentMusicIndex < 0)
		{
			m_currentMusicIndex = musicIndex;
			e.start();
			return;
		}
		EventInstance e2 = musictracks[m_currentMusicIndex];
		if (shouldStop)
		{
			if (shouldDeadStop)
			{
				e2.stop(STOP_MODE.IMMEDIATE);
			}
			else
			{
				e2.stop(STOP_MODE.ALLOWFADEOUT);
			}
		}
		else
		{
			SetIntParameter(e2, "Intensity", 0f);
		}
		m_nextMusicIndex = musicIndex;
		if (timeDelayStart > 0f)
		{
			m_isTransitioning = true;
			m_musicTickDown = timeDelayStart;
		}
		else
		{
			m_currentMusicIndex = musicIndex;
			e.start();
			SetIntParameter(e, "Intensity", 1f);
		}
	}

	private void OnDestroy()
	{
		StopAllEvents();
	}

	private void StopAllEvents()
	{
		MasterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
	}
}
