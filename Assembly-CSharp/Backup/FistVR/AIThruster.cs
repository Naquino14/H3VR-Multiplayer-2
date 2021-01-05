// Decompiled with JetBrains decompiler
// Type: FistVR.AIThruster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIThruster : FVRDestroyableObject
  {
    [Header("Thruster Params")]
    public ParticleSystem SmallBooster;
    public ParticleSystem LargeBooster;
    public Rigidbody BaseRB;
    private float m_ThrustMagnitude;
    private int framesTilNextBoost;
    public AIThrusterControlBox ControlBox;

    public float GetMagnitude() => this.m_ThrustMagnitude;

    public float Thrust()
    {
      this.BaseRB.AddForceAtPosition(-this.transform.forward * this.m_ThrustMagnitude * 4f, this.transform.position);
      if ((double) this.m_ThrustMagnitude < 0.800000011920929)
        this.m_ThrustMagnitude += Time.deltaTime * 16f;
      else
        this.m_ThrustMagnitude += Time.deltaTime * 1f;
      this.m_ThrustMagnitude = Mathf.Clamp(this.m_ThrustMagnitude, 0.0f, 1f);
      return this.m_ThrustMagnitude;
    }

    public void KillThrust() => this.m_ThrustMagnitude = 0.0f;

    public override void DestroyEvent() => base.DestroyEvent();

    public override void Update()
    {
      base.Update();
      if (this.framesTilNextBoost > 0)
        --this.framesTilNextBoost;
      if ((double) this.m_ThrustMagnitude > 0.800000011920929)
      {
        this.SetBooster(this.LargeBooster, 10f);
        this.SetBooster(this.SmallBooster, 0.0f);
        if (this.framesTilNextBoost <= 0)
        {
          this.framesTilNextBoost = Random.Range(4, 8);
          if (GM.CurrentSceneSettings.IsSceneLowLight)
            FXM.InitiateMuzzleFlash(this.LargeBooster.transform.position, this.transform.forward, Random.Range(0.25f, 1.5f), new Color(1f, 0.8f, 0.5f), Random.Range(1f, 2f));
          else
            FXM.InitiateMuzzleFlash(this.LargeBooster.transform.position, this.transform.forward, Random.Range(0.1f, 0.4f), new Color(1f, 0.8f, 0.5f), Random.Range(0.5f, 2f));
        }
      }
      else if ((double) this.m_ThrustMagnitude > 0.0)
      {
        this.SetBooster(this.LargeBooster, 0.0f);
        this.SetBooster(this.SmallBooster, 10f);
      }
      else
      {
        this.SetBooster(this.LargeBooster, 0.0f);
        this.SetBooster(this.SmallBooster, 0.0f);
      }
      this.m_ThrustMagnitude -= Time.deltaTime * 2f;
      this.m_ThrustMagnitude = Mathf.Clamp(this.m_ThrustMagnitude, 0.0f, 1f);
    }

    private void SetBooster(ParticleSystem pSystem, float rate)
    {
      if (!((Object) pSystem != (Object) null))
        return;
      ParticleSystem.EmissionModule emission = pSystem.emission;
      ParticleSystem.MinMaxCurve rate1 = emission.rate;
      rate1.mode = ParticleSystemCurveMode.Constant;
      rate1.constantMax = rate;
      rate1.constantMin = rate;
      emission.rate = rate1;
    }
  }
}
