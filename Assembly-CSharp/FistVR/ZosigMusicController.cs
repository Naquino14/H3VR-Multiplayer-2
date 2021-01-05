// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigMusicController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigMusicController : MonoBehaviour
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
    private string BankPreload = "MX_ROTW";
    public List<ZosigMusicController.ZosigTrack> Tracks = new List<ZosigMusicController.ZosigTrack>();
    private float m_intensityRaw;
    private ZosigMusicController.ZosigTrack m_currentTrack;

    public void Start()
    {
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.DeadSosigDetected);
      GM.CurrentSceneSettings.ShotFiredEvent += new FVRSceneSettings.ShotFired(this.ShotFired);
      GM.CurrentSceneSettings.PlayerTookDamageEvent += new FVRSceneSettings.PlayerTookDamage(this.PlayerTookDamage);
    }

    private void OnDestroy()
    {
      GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.DeadSosigDetected);
      GM.CurrentSceneSettings.ShotFiredEvent -= new FVRSceneSettings.ShotFired(this.ShotFired);
      GM.CurrentSceneSettings.PlayerTookDamageEvent -= new FVRSceneSettings.PlayerTookDamage(this.PlayerTookDamage);
      this.StopAllEvents();
    }

    public void PlayerTookDamage(float percent) => this.m_intensityRaw += percent * 20f;

    public void ShotFired(FVRFireArm firearm) => this.m_intensityRaw += 0.2f;

    public void DeadSosigDetected(Sosig s) => this.m_intensityRaw += 3f;

    public void Init()
    {
      this.MasterBus = RuntimeManager.GetBus("bus:/Music");
      for (int index = 0; index < this.Tracks.Count; ++index)
        this.Tracks[index].C = this;
      this.Initialize();
    }

    public void SetMasterVolume(float i)
    {
      int num = (int) this.MasterBus.setVolume(i);
    }

    public void SetIntensity(float intensity)
    {
      for (int index = 0; index < this.Tracks.Count; ++index)
        this.Tracks[index].TarIntensity = intensity;
    }

    public void Initialize()
    {
      RuntimeManager.LoadBank(this.BankPreload);
      for (int index = 0; index < this.StateEvents.Count; ++index)
        this.musictracks.Add(RuntimeManager.CreateInstance(this.StateEvents[index]));
    }

    public void SwitchToTrack(ZosigMusicController.ZosigTrackName track)
    {
      if (this.m_currentTrack != null)
      {
        if (this.m_currentTrack.Track == track)
          return;
        this.m_currentTrack.Stop();
        this.m_currentTrack = this.Tracks[(int) track];
        this.m_currentTrack.AttemptToStart();
      }
      else
      {
        this.m_currentTrack = this.Tracks[(int) track];
        this.m_currentTrack.AttemptToStart();
      }
    }

    public void Tick(float t)
    {
      for (int index = 0; index < this.Tracks.Count; ++index)
        this.Tracks[index].Tick(t);
      this.m_intensityRaw = Mathf.Clamp(this.m_intensityRaw, 0.0f, 10f);
      this.m_intensityRaw -= Time.deltaTime * 0.4f;
      this.SetIntensity(Mathf.Clamp(this.m_intensityRaw * 0.3f, 0.0f, 2f));
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

    private void StopAllEvents()
    {
      int num = (int) this.MasterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
    }

    [Serializable]
    public class ZosigTrack
    {
      public ZosigMusicController C;
      public ZosigMusicController.ZosigTrackName Track;
      public float CurIntensity;
      public float TarIntensity;
      public float StopValue;
      public float StopCooldown;
      private bool m_isWaitingToStart;
      private bool m_isPlaying;

      public void AttemptToStart()
      {
        if ((double) this.StopCooldown > 0.0)
        {
          this.m_isWaitingToStart = true;
        }
        else
        {
          this.m_isWaitingToStart = false;
          this.Start();
        }
      }

      private void Start()
      {
        if (this.m_isPlaying)
          return;
        this.StopCooldown = 0.0f;
        this.m_isPlaying = true;
        this.C.SetIntParameterByIndex((int) this.Track, "Stop", 0.0f);
        this.CurIntensity = 0.0f;
        this.TarIntensity = 0.0f;
        int num = (int) this.C.musictracks[(int) this.Track].start();
      }

      public void Stop()
      {
        this.m_isWaitingToStart = false;
        if (!this.m_isPlaying)
          return;
        this.m_isPlaying = false;
        this.StopValue = 1f;
        this.StopCooldown = 10f;
        this.C.SetIntParameterByIndex((int) this.Track, nameof (Stop), 1f);
      }

      public void Tick(float time)
      {
        if ((double) this.StopCooldown > 0.0)
        {
          this.StopCooldown -= Time.deltaTime;
          if ((double) this.StopCooldown <= 0.0)
          {
            EventInstance musictrack = this.C.musictracks[(int) this.Track];
            this.C.StopEventByIndex((int) this.Track, false);
          }
        }
        if (this.m_isWaitingToStart && (double) this.StopCooldown <= 0.0)
        {
          this.m_isWaitingToStart = false;
          this.Start();
        }
        this.CurIntensity = Mathf.MoveTowards(this.CurIntensity, this.TarIntensity, time);
        this.C.SetIntParameterByIndex((int) this.Track, "ROTW Intensity", this.CurIntensity);
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
      EastWienerton,
    }
  }
}
