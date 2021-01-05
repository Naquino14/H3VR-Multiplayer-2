// Decompiled with JetBrains decompiler
// Type: FistVR.FlameTrap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FlameTrap : MonoBehaviour
  {
    public FlameTrap.FlameTrapType Type;
    public Transform WavingPiece;
    public List<ParticleSystem> ParticleSystems;
    private bool m_isOn;
    public float MaxEmission = 30f;
    public float SpinningSpeed = 1f;
    public AudioSource AudSource_Loop;
    private float spinningVal;

    public void Start()
    {
    }

    public void TurnOn()
    {
      if (this.m_isOn)
        return;
      this.m_isOn = true;
      this.AudSource_Loop.Play();
      for (int index = 0; index < this.ParticleSystems.Count; ++index)
      {
        ParticleSystem.EmissionModule emission = this.ParticleSystems[index].emission;
        ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
        rateOverTime.mode = ParticleSystemCurveMode.Constant;
        rateOverTime.constant = this.MaxEmission;
        emission.rateOverTime = rateOverTime;
      }
    }

    public void TurnOff()
    {
      if (!this.m_isOn)
        return;
      this.m_isOn = false;
      this.AudSource_Loop.Stop();
      for (int index = 0; index < this.ParticleSystems.Count; ++index)
      {
        ParticleSystem.EmissionModule emission = this.ParticleSystems[index].emission;
        ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
        rateOverTime.mode = ParticleSystemCurveMode.Constant;
        rateOverTime.constant = 0.0f;
        emission.rateOverTime = rateOverTime;
      }
    }

    public void Update()
    {
      switch (this.Type)
      {
        case FlameTrap.FlameTrapType.Waving:
          this.spinningVal = Mathf.Sin(Time.time * this.SpinningSpeed) * 70f;
          this.WavingPiece.localEulerAngles = new Vector3(0.0f, 0.0f, this.spinningVal);
          break;
        case FlameTrap.FlameTrapType.Spinning:
          this.spinningVal += Time.deltaTime * this.SpinningSpeed;
          this.spinningVal = Mathf.Repeat(this.spinningVal, 360f);
          this.WavingPiece.localEulerAngles = new Vector3(0.0f, this.spinningVal, 0.0f);
          break;
      }
    }

    public enum FlameTrapType
    {
      Waving,
      Spinning,
    }
  }
}
