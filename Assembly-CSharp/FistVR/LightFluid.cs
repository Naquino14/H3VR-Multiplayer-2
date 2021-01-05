// Decompiled with JetBrains decompiler
// Type: FistVR.LightFluid
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class LightFluid : FVRPhysicalObject, IFVRDamageable
  {
    public ParticleSystem FluidSystem;
    public AudioEvent AudEvent_Gush;
    private float fluidGush;
    public List<GameObject> SpawnsOnDestroy;
    private bool m_isExploded;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!hand.Input.TriggerDown)
        return;
      this.fluidGush = 20f;
      SM.PlayGenericSound(this.AudEvent_Gush, this.transform.position);
    }

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_TotalKinetic <= 500.0 && (double) d.Dam_TotalEnergetic <= 20.0)
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_isExploded)
        return;
      this.m_isExploded = true;
      for (int index = 0; index < this.SpawnsOnDestroy.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnsOnDestroy[index], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.fluidGush <= 0.0)
        return;
      this.fluidGush -= Time.deltaTime * 40f;
      this.fluidGush = Mathf.Clamp(this.fluidGush, 0.0f, this.fluidGush);
      ParticleSystem.EmissionModule emission = this.FluidSystem.emission;
      ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
      rateOverTime.mode = ParticleSystemCurveMode.Constant;
      rateOverTime.constant = this.fluidGush;
      emission.rateOverTime = rateOverTime;
    }
  }
}
