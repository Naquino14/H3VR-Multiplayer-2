// Decompiled with JetBrains decompiler
// Type: FistVR.FVRAmbienceController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRAmbienceController : MonoBehaviour
  {
    public AudioSource AmbienceA;
    public AudioSource AmbienceB;
    private bool m_isTransitioning;
    private bool m_towardsA = true;
    private float targetVolumeA;
    private float targetVolumeB;
    public List<AudioClip> AmbienceClips = new List<AudioClip>();
    public List<float> AmbienceVolumes = new List<float>();
    public bool UsesAmbienceVolumes;
    public float MaxVolume = 0.3f;
    public List<FVRAmbienceZone> Zones = new List<FVRAmbienceZone>();
    public int PlayerAmbientIndex;
    private int m_startingIndex;
    public float TransitionSpeed = 2f;

    private void Start()
    {
      this.m_startingIndex = this.PlayerAmbientIndex;
      this.AmbienceA.clip = this.AmbienceClips[this.PlayerAmbientIndex];
      this.targetVolumeA = this.MaxVolume;
      this.AmbienceA.Play();
    }

    private void Update()
    {
      int ambientIndex = this.GetAmbientIndex(GM.CurrentPlayerBody.Head.position);
      if (ambientIndex != this.PlayerAmbientIndex)
        this.TransitionToAmbientIndex(ambientIndex);
      this.AmbienceA.volume = Mathf.MoveTowards(this.AmbienceA.volume, this.targetVolumeA, Time.deltaTime * this.TransitionSpeed);
      this.AmbienceB.volume = Mathf.MoveTowards(this.AmbienceB.volume, this.targetVolumeB, Time.deltaTime * this.TransitionSpeed);
      if ((double) this.AmbienceA.volume <= 0.0 || (double) this.AmbienceB.volume <= 0.0)
        this.m_isTransitioning = false;
      if ((double) this.AmbienceA.volume <= 0.0 && this.AmbienceA.isPlaying)
        this.AmbienceA.Stop();
      if ((double) this.AmbienceB.volume > 0.0 || !this.AmbienceB.isPlaying)
        return;
      this.AmbienceB.Stop();
    }

    private void TransitionToAmbientIndex(int i)
    {
      if (this.m_isTransitioning)
        return;
      this.PlayerAmbientIndex = i;
      this.m_isTransitioning = true;
      this.m_towardsA = !this.m_towardsA;
      if (this.m_towardsA)
      {
        this.targetVolumeA = this.MaxVolume;
        if (this.UsesAmbienceVolumes)
          this.targetVolumeA = this.AmbienceVolumes[i];
        this.targetVolumeB = 0.0f;
        this.AmbienceA.clip = this.AmbienceClips[i];
        this.AmbienceA.Play();
      }
      else
      {
        this.targetVolumeB = this.MaxVolume;
        if (this.UsesAmbienceVolumes)
          this.targetVolumeB = this.AmbienceVolumes[i];
        this.targetVolumeA = 0.0f;
        this.AmbienceB.clip = this.AmbienceClips[i];
        this.AmbienceB.Play();
      }
    }

    public bool TestVolumeBool(FVRAmbienceZone z, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = z.t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    public int GetAmbientIndex(Vector3 pos)
    {
      int num = 100;
      bool flag = false;
      for (int index = 0; index < this.Zones.Count; ++index)
      {
        if (this.Zones[index].ZoneIndex < num)
        {
          if (this.TestVolumeBool(this.Zones[index], pos))
          {
            num = this.Zones[index].ZoneIndex;
            flag = true;
          }
          if (num <= 0)
            break;
        }
      }
      return flag ? num : this.m_startingIndex;
    }
  }
}
