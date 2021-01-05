// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwCharcoal
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RotrwCharcoal : FVRPhysicalObject, IFVRDamageable
  {
    public bool IsFluided;
    public bool IsOnFire;
    public ParticleSystem FireSystem;
    private float m_fuelLeft = 1f;
    private float m_fuelBurnDownTime = 0.005f;
    public RotrwCharcoal.RendererState State;
    public Renderer Rend_Unburned_Unfluided;
    public Renderer Rend_Unburned_Fluided;
    public Renderer Rend_Burning;
    public Renderer Rend_BurntOut;
    private float m_timeTilFireCheck = 1f;
    public LayerMask LM_DamMask;
    public AudioEvent AudEvent_Ignite;
    private bool m_isPSystemBurning;

    protected override void Start()
    {
      base.Start();
      this.m_fuelBurnDownTime = Random.Range(this.m_fuelBurnDownTime * 1f, this.m_fuelBurnDownTime * 1.2f);
    }

    private void SetRendererState(RotrwCharcoal.RendererState s)
    {
      if (this.State == s)
        return;
      switch (s)
      {
        case RotrwCharcoal.RendererState.UnburnedWet:
          this.Rend_Unburned_Unfluided.enabled = false;
          this.Rend_Unburned_Fluided.enabled = true;
          this.Rend_Burning.enabled = false;
          this.Rend_BurntOut.enabled = false;
          break;
        case RotrwCharcoal.RendererState.Burning:
          this.Rend_Unburned_Unfluided.enabled = false;
          this.Rend_Unburned_Fluided.enabled = false;
          this.Rend_Burning.enabled = true;
          this.Rend_BurntOut.enabled = false;
          break;
        case RotrwCharcoal.RendererState.BurntOut:
          this.Rend_Unburned_Unfluided.enabled = false;
          this.Rend_Unburned_Fluided.enabled = false;
          this.Rend_Burning.enabled = false;
          this.Rend_BurntOut.enabled = true;
          break;
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.IsOnFire)
      {
        this.UpdateFireEmission(5f);
        this.m_fuelLeft -= Time.deltaTime * this.m_fuelBurnDownTime;
        if ((double) this.m_timeTilFireCheck > 0.0)
        {
          this.m_timeTilFireCheck -= Time.deltaTime;
        }
        else
        {
          this.m_timeTilFireCheck = Random.Range(1f, 1.5f);
          this.DamageBubble();
        }
        if ((double) this.m_fuelLeft < 0.100000001490116)
          this.SetRendererState(RotrwCharcoal.RendererState.BurntOut);
        if ((double) this.m_fuelLeft > 0.0)
          return;
        this.PutOut();
      }
      else
        this.UpdateFireEmission(0.0f);
    }

    private void DamageBubble()
    {
      Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, 0.1f, (int) this.LM_DamMask, QueryTriggerInteraction.Collide);
      List<IFVRDamageable> fvrDamageableList = new List<IFVRDamageable>();
      if (colliderArray.Length > 0)
      {
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          IFVRDamageable component1 = colliderArray[index].gameObject.GetComponent<IFVRDamageable>();
          if (component1 != null && !fvrDamageableList.Contains(component1))
            fvrDamageableList.Add(component1);
          FVRIgnitable component2 = colliderArray[index].transform.gameObject.GetComponent<FVRIgnitable>();
          if ((Object) component2 == (Object) null && (Object) colliderArray[index].attachedRigidbody != (Object) null)
            colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
          if ((Object) component2 != (Object) null)
            FXM.Ignite(component2, 1f);
        }
      }
      if (fvrDamageableList.Count <= 0)
        return;
      for (int index = fvrDamageableList.Count - 1; index >= 0; --index)
      {
        if ((Object) fvrDamageableList[index] != (Object) null)
        {
          FistVR.Damage dam = new FistVR.Damage()
          {
            Dam_Thermal = 50f,
            Dam_TotalEnergetic = 50f,
            Class = FistVR.Damage.DamageClass.Explosive,
            damageSize = 0.1f,
            hitNormal = Random.onUnitSphere,
            point = this.transform.position
          };
          dam.strikeDir = -dam.hitNormal;
          fvrDamageableList[index].Damage(dam);
        }
      }
    }

    public void FluidMe()
    {
      this.IsFluided = true;
      if (this.State != RotrwCharcoal.RendererState.UnburnedNoFluid)
        return;
      this.SetRendererState(RotrwCharcoal.RendererState.UnburnedWet);
    }

    private void OnParticleCollision(GameObject other)
    {
      if (this.IsFluided || !other.CompareTag("LighterFluid"))
        return;
      this.FluidMe();
    }

    public void Ignite()
    {
      if (this.IsOnFire || !this.IsFluided || (double) this.m_fuelLeft <= 0.0)
        return;
      this.IsOnFire = true;
      this.SetRendererState(RotrwCharcoal.RendererState.Burning);
      this.FireSystem.emission.enabled = true;
      this.UpdateFireEmission(5f);
      SM.PlayGenericSound(this.AudEvent_Ignite, this.transform.position);
    }

    public void PutOut()
    {
      this.IsOnFire = false;
      this.m_fuelLeft = 0.0f;
      this.FireSystem.emission.enabled = false;
      this.SetRendererState(RotrwCharcoal.RendererState.BurntOut);
    }

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal > 0.0)
        this.Ignite();
      if ((double) d.Dam_TotalKinetic <= 500.0 || (double) this.m_fuelLeft >= 0.25)
        return;
      Object.Destroy((Object) this.gameObject);
    }

    private void UpdateFireEmission(float f)
    {
      ParticleSystem.EmissionModule emission = this.FireSystem.emission;
      ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
      rateOverTime.mode = ParticleSystemCurveMode.Constant;
      rateOverTime.constant = f;
      emission.rateOverTime = rateOverTime;
    }

    public enum RendererState
    {
      UnburnedNoFluid,
      UnburnedWet,
      Burning,
      BurntOut,
    }
  }
}
