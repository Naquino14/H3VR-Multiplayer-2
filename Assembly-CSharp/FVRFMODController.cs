// Decompiled with JetBrains decompiler
// Type: FVRFMODController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
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
    this.MasterBus = RuntimeManager.GetBus("bus:/Music");
    this.Initialize();
  }

  public void SetMasterVolume(float i)
  {
    int num = (int) this.MasterBus.setVolume(i);
  }

  public void Initialize()
  {
    RuntimeManager.LoadBank(this.BankPreload);
    for (int index = 0; index < this.StateEvents.Count; ++index)
      this.musictracks.Add(RuntimeManager.CreateInstance(this.StateEvents[index]));
  }

  public void Tick(float t)
  {
    if (!this.m_isTransitioning)
      return;
    this.m_musicTickDown -= t;
    if ((double) this.m_musicTickDown > 0.0)
      return;
    this.m_isTransitioning = false;
    this.m_currentMusicIndex = this.m_nextMusicIndex;
    EventInstance musictrack = this.musictracks[this.m_currentMusicIndex];
    int num = (int) musictrack.start();
    this.SetIntParameter(musictrack, "Intensity", 1f);
  }

  public void SetIntParameterByIndex(int i, string s, float f) => this.SetIntParameter(this.musictracks[i], s, f);

  public void SetIntParameter(EventInstance e, string s, float f)
  {
    int num = (int) e.setParameterValue(s, f);
  }

  public void StopEventByIndex(int i, bool shouldDeadStop)
  {
    EventInstance musictrack = this.musictracks[i];
    if (shouldDeadStop)
    {
      int num1 = (int) musictrack.stop(STOP_MODE.IMMEDIATE);
    }
    else
    {
      int num2 = (int) musictrack.stop(STOP_MODE.ALLOWFADEOUT);
    }
  }

  public void SwitchTo(int musicIndex, float timeDelayStart, bool shouldStop, bool shouldDeadStop)
  {
    if (this.m_isTransitioning)
      return;
    EventInstance musictrack1 = this.musictracks[musicIndex];
    if (this.m_currentMusicIndex < 0)
    {
      this.m_currentMusicIndex = musicIndex;
      int num = (int) musictrack1.start();
    }
    else
    {
      EventInstance musictrack2 = this.musictracks[this.m_currentMusicIndex];
      if (shouldStop)
      {
        if (shouldDeadStop)
        {
          int num1 = (int) musictrack2.stop(STOP_MODE.IMMEDIATE);
        }
        else
        {
          int num2 = (int) musictrack2.stop(STOP_MODE.ALLOWFADEOUT);
        }
      }
      else
        this.SetIntParameter(musictrack2, "Intensity", 0.0f);
      this.m_nextMusicIndex = musicIndex;
      if ((double) timeDelayStart > 0.0)
      {
        this.m_isTransitioning = true;
        this.m_musicTickDown = timeDelayStart;
      }
      else
      {
        this.m_currentMusicIndex = musicIndex;
        int num3 = (int) musictrack1.start();
        this.SetIntParameter(musictrack1, "Intensity", 1f);
      }
    }
  }

  private void OnDestroy() => this.StopAllEvents();

  private void StopAllEvents()
  {
    int num = (int) this.MasterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
  }
}
