// Decompiled with JetBrains decompiler
// Type: FistVR.FlamingSwordFire
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlamingSwordFire : MonoBehaviour
  {
    public AudioSource BurningAudio;
    private float m_burningVolume;
    public FVRPhysicalObject PObject;
    public ParticleSystem PSystem;
    private float m_curVolume;

    private void Start()
    {
    }

    private void Update()
    {
      float target = 0.0f;
      float num;
      if (this.PObject.IsHeld)
      {
        num = 200f;
        target = 0.3f;
      }
      else
        num = 0.0f;
      this.m_curVolume = Mathf.MoveTowards(this.m_curVolume, target, Time.deltaTime);
      if ((double) this.m_curVolume < 0.00999999977648258)
      {
        if (this.BurningAudio.isPlaying)
          this.BurningAudio.Stop();
      }
      else
      {
        if (!this.BurningAudio.isPlaying)
          this.BurningAudio.Play();
        this.BurningAudio.volume = this.m_curVolume;
      }
      ParticleSystem.EmissionModule emission = this.PSystem.emission;
      ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
      rateOverTime.mode = ParticleSystemCurveMode.Constant;
      rateOverTime.constantMax = num;
      rateOverTime.constantMin = num;
      emission.rateOverTime = rateOverTime;
    }
  }
}
