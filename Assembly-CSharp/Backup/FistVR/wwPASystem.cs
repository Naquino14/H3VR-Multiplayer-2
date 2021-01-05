// Decompiled with JetBrains decompiler
// Type: FistVR.wwPASystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class wwPASystem : MonoBehaviour
  {
    public AudioSource AudioSource;
    public AudioClip PA_Jingle;
    public AudioClip[] PA_Announcement;
    public AudioClip[] PianoMusic;
    public List<Transform> PASourcePoints;
    public Transform Player;
    public wwPASystem.PA_State State;
    private bool m_isPASystemActive = true;
    private bool m_isInSuppressedMode;
    private bool m_isInSilentMode;
    private float m_ModeMultLerp = 1f;

    private void SetRandomPianoTrack()
    {
      int index = Random.Range(1, this.PianoMusic.Length);
      AudioClip audioClip = this.PianoMusic[index];
      this.PianoMusic[index] = this.PianoMusic[0];
      this.PianoMusic[0] = audioClip;
    }

    private void SetRandomPA_Announcement()
    {
      int index = Random.Range(1, this.PA_Announcement.Length);
      AudioClip audioClip = this.PA_Announcement[index];
      this.PA_Announcement[index] = this.PA_Announcement[0];
      this.PA_Announcement[0] = audioClip;
    }

    private void PlayMusic()
    {
      this.State = wwPASystem.PA_State.Music;
      this.SetRandomPianoTrack();
      this.AudioSource.clip = this.PianoMusic[0];
      this.AudioSource.Play();
    }

    public void DestroySpeaker(wwExplodeableSpeaker s) => this.PASourcePoints.Remove(s.transform);

    private void PlayJingle1()
    {
      this.State = wwPASystem.PA_State.Jingle1;
      this.AudioSource.clip = this.PA_Jingle;
      this.AudioSource.Play();
    }

    private void PlayAnnouncement()
    {
      this.State = wwPASystem.PA_State.Announcement;
      this.SetRandomPA_Announcement();
      this.AudioSource.clip = this.PA_Announcement[0];
      this.AudioSource.Play();
    }

    private void PlayJingle2()
    {
      this.State = wwPASystem.PA_State.Jingle2;
      this.AudioSource.clip = this.PA_Jingle;
      this.AudioSource.Play();
    }

    public void Awake() => this.PlayMusic();

    private void Start()
    {
    }

    private void Update()
    {
      this.ModeLerp();
      this.DetermineVolume();
      if (!this.m_isPASystemActive)
        return;
      this.PASystemUpdate();
    }

    public void EngageStandardMode()
    {
      this.m_isInSuppressedMode = false;
      this.m_isInSilentMode = false;
    }

    public void EngageSuppressedMode() => this.m_isInSuppressedMode = true;

    public void EngageSilentMode() => this.m_isInSilentMode = true;

    public void DisEngageSuppressedMode() => this.m_isInSuppressedMode = false;

    public void DisEngageSilentMode() => this.m_isInSilentMode = false;

    private void ModeLerp()
    {
      if (this.m_isInSilentMode)
        this.m_ModeMultLerp = Mathf.MoveTowards(this.m_ModeMultLerp, 0.0f, Time.deltaTime * 0.5f);
      else if (this.m_isInSuppressedMode)
        this.m_ModeMultLerp = Mathf.MoveTowards(this.m_ModeMultLerp, 0.25f, Time.deltaTime * 0.5f);
      else
        this.m_ModeMultLerp = Mathf.MoveTowards(this.m_ModeMultLerp, 1f, Time.deltaTime * 0.5f);
    }

    private void PASystemUpdate()
    {
      switch (this.State)
      {
        case wwPASystem.PA_State.Music:
          if (this.AudioSource.isPlaying)
            break;
          this.PlayJingle1();
          break;
        case wwPASystem.PA_State.Jingle1:
          if (this.AudioSource.isPlaying)
            break;
          this.PlayAnnouncement();
          break;
        case wwPASystem.PA_State.Announcement:
          if (this.AudioSource.isPlaying)
            break;
          this.PlayJingle2();
          break;
        case wwPASystem.PA_State.Jingle2:
          if (this.AudioSource.isPlaying)
            break;
          this.PlayMusic();
          break;
      }
    }

    private void DetermineVolume()
    {
      float num1 = 200f;
      for (int index = 0; index < this.PASourcePoints.Count; ++index)
      {
        if (!((Object) this.PASourcePoints[index] == (Object) null))
        {
          Vector3 vector3_1 = new Vector3(this.Player.position.x, 0.0f, this.Player.position.z);
          Vector3 vector3_2 = new Vector3(this.PASourcePoints[index].position.x, 0.0f, this.PASourcePoints[index].position.z);
          Vector3 from = new Vector3(this.PASourcePoints[index].forward.x, 0.0f, this.PASourcePoints[index].forward.z);
          Vector3 to = vector3_1 - vector3_2;
          float num2 = Vector3.Angle(from, to) / 180f;
          float num3 = to.magnitude * (float) (1.0 + (double) num2 * 0.75);
          if ((double) num3 < (double) num1)
            num1 = num3;
        }
      }
      this.AudioSource.volume = Mathf.Clamp((float) (1.0 - (double) num1 / 50.0) * 0.35f, 0.0f, 0.25f) * this.m_ModeMultLerp;
    }

    public enum PA_State
    {
      Music,
      Jingle1,
      Announcement,
      Jingle2,
    }
  }
}
