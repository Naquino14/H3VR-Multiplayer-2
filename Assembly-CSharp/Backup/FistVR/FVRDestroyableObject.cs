// Decompiled with JetBrains decompiler
// Type: FistVR.FVRDestroyableObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class FVRDestroyableObject : MonoBehaviour, IFVRDamageable
  {
    [Header("Destroyable Params")]
    public float StartingToughness;
    public Vector2 ResistMult_Blunt = new Vector2(0.0f, 1f);
    public Vector2 ResistMult_Piercing = new Vector2(0.0f, 1f);
    public Vector2 ResistMult_Cutting = new Vector2(0.0f, 1f);
    protected float m_currentToughness;
    protected bool m_isDestroyed;
    public bool ReceivesCollisionDamage;
    public bool DestroyThisObjectOnDestruction;
    public GameObject[] SpawnOnDestruction;
    public GameObject[] DetachAddRigidbodyBlowAway;
    public bool UsesParams = true;
    public FVRDestroyableObject.DetachRBParams DetachRigidbodyParams;
    public Vector2 ExplosiveForceToDetach = new Vector2(80f, 150f);
    public FVRDestroyableObject[] SendDestroyEventOnDestruction;
    public Vector2 DestroyEventTimeRange;
    public FVRDestroyableObject.ProgressiveDamageFX[] ProgressiveDamageEffects;
    public bool DoesDecayWhenDamaged;
    public float PercentThresholdForDecay;
    public float DecayRate;
    private FVRPhysicalObject m_po;
    public Rigidbody Rb;
    public bool UsesDestructionStageRenderers;
    public Renderer[] DestructionRenderers;
    private int m_currentDestructionRenderer = -1;

    public void SetToughnessPercentageIfHigher(float f) => this.m_currentToughness = Mathf.Min(this.StartingToughness * f, this.m_currentToughness);

    private void UpdateDestructionRenderers()
    {
      int num = Mathf.Clamp(Mathf.RoundToInt((float) (1.0 - (double) this.m_currentToughness / (double) this.StartingToughness) * (float) this.DestructionRenderers.Length), 0, this.DestructionRenderers.Length - 1);
      if (this.m_currentDestructionRenderer == num)
        return;
      this.m_currentDestructionRenderer = num;
      for (int index = 0; index < this.DestructionRenderers.Length; ++index)
        this.DestructionRenderers[index].enabled = index == this.m_currentDestructionRenderer;
    }

    public virtual void Awake()
    {
      this.m_currentToughness = this.StartingToughness;
      this.m_po = this.GetComponent<FVRPhysicalObject>();
      this.Rb = this.GetComponent<Rigidbody>();
      if (!this.UsesDestructionStageRenderers)
        return;
      this.UpdateDestructionRenderers();
    }

    public virtual void Update()
    {
      for (int index = 0; index < this.ProgressiveDamageEffects.Length; ++index)
        this.ProgressiveDamageEffects[index].UpdateEffect((float) (1.0 - (double) this.m_currentToughness / (double) this.StartingToughness));
      if (this.DoesDecayWhenDamaged && 1.0 - (double) this.m_currentToughness / (double) this.StartingToughness >= (double) this.PercentThresholdForDecay)
        this.m_currentToughness -= this.DecayRate * Time.deltaTime;
      if ((double) this.m_currentToughness > 0.0)
        return;
      this.DestroyEvent();
    }

    public virtual void Damage(FistVR.Damage dam)
    {
      this.m_currentToughness -= Mathf.Clamp((dam.Dam_Blunt - this.ResistMult_Blunt.x) * this.ResistMult_Blunt.y, 0.0f, dam.Dam_Blunt) + Mathf.Clamp((dam.Dam_Cutting - this.ResistMult_Cutting.x) * this.ResistMult_Cutting.y, 0.0f, dam.Dam_Cutting) + Mathf.Clamp((dam.Dam_Piercing - this.ResistMult_Piercing.x) * this.ResistMult_Piercing.y, 0.0f, dam.Dam_Piercing);
      if (this.UsesDestructionStageRenderers)
        this.UpdateDestructionRenderers();
      if ((double) this.m_currentToughness > 0.0)
        return;
      this.DestroyEvent();
    }

    public void OnCollisionEnter(Collision col)
    {
      if (!this.ReceivesCollisionDamage)
        return;
      float num1 = 100f;
      if ((UnityEngine.Object) this.Rb != (UnityEngine.Object) null)
        num1 = this.Rb.mass;
      float num2 = num1;
      if ((UnityEngine.Object) col.rigidbody != (UnityEngine.Object) null)
        num2 = col.rigidbody.mass;
      if ((double) col.relativeVelocity.magnitude < 2.5)
        return;
      float num3 = num2 / num1;
      float num4 = col.relativeVelocity.magnitude * num3;
      Vector3 relativeVelocity = col.relativeVelocity;
      this.Damage(new FistVR.Damage()
      {
        Dam_Blunt = (float) ((double) num4 * (double) relativeVelocity.magnitude * 50.0),
        hitNormal = col.contacts[0].normal,
        strikeDir = col.relativeVelocity.normalized,
        point = col.contacts[0].point,
        Class = FistVR.Damage.DamageClass.Environment
      });
    }

    public virtual void DestroyEvent()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      if ((UnityEngine.Object) this.m_po != (UnityEngine.Object) null && this.m_po.IsHeld && (UnityEngine.Object) this.m_po.m_hand != (UnityEngine.Object) null)
      {
        this.m_po.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
        this.m_po.EndInteraction(this.m_po.m_hand);
      }
      for (int index = 0; index < this.DetachAddRigidbodyBlowAway.Length; ++index)
      {
        if ((UnityEngine.Object) this.DetachAddRigidbodyBlowAway[index] != (UnityEngine.Object) null)
        {
          this.DetachAddRigidbodyBlowAway[index].SetActive(true);
          this.DetachAddRigidbodyBlowAway[index].transform.SetParent((Transform) null);
          Rigidbody rigidbody = !((UnityEngine.Object) this.DetachAddRigidbodyBlowAway[index].GetComponent<Rigidbody>() == (UnityEngine.Object) null) ? this.DetachAddRigidbodyBlowAway[index].GetComponent<Rigidbody>() : this.DetachAddRigidbodyBlowAway[index].AddComponent<Rigidbody>();
          rigidbody.isKinematic = false;
          if ((UnityEngine.Object) this.Rb != (UnityEngine.Object) null)
          {
            rigidbody.velocity = this.Rb.velocity;
            rigidbody.angularVelocity = this.Rb.angularVelocity;
          }
          if (this.UsesParams)
          {
            rigidbody.mass = this.DetachRigidbodyParams.Mass;
            rigidbody.drag = this.DetachRigidbodyParams.Drag;
            rigidbody.angularDrag = this.DetachRigidbodyParams.AngularDrag;
          }
          rigidbody.AddForceAtPosition((rigidbody.transform.position - this.transform.position).normalized * UnityEngine.Random.Range(this.ExplosiveForceToDetach.x, this.ExplosiveForceToDetach.y), this.transform.position);
        }
      }
      for (int index = 0; index < this.SpawnOnDestruction.Length; ++index)
        UnityEngine.Object.Instantiate<GameObject>(this.SpawnOnDestruction[index], this.transform.position, this.transform.rotation);
      for (int index = 0; index < this.SendDestroyEventOnDestruction.Length; ++index)
      {
        if ((UnityEngine.Object) this.SendDestroyEventOnDestruction[index] != (UnityEngine.Object) null)
          this.SendDestroyEventOnDestruction[index].Invoke(nameof (DestroyEvent), UnityEngine.Random.Range(this.DestroyEventTimeRange.x, this.DestroyEventTimeRange.y));
      }
      if (!this.DestroyThisObjectOnDestruction)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    [Serializable]
    public struct DetachRBParams
    {
      public float Mass;
      public float Drag;
      public float AngularDrag;
    }

    [Serializable]
    public class ProgressiveDamageFX
    {
      public ParticleSystem Effect;
      public Vector2 EmitRange;
      public float DamageThreshold;

      public void UpdateEffect(float percent)
      {
        if (!((UnityEngine.Object) this.Effect != (UnityEngine.Object) null) || (double) percent < (double) this.DamageThreshold)
          return;
        ParticleSystem.EmissionModule emission = this.Effect.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        float num = Mathf.Lerp(this.EmitRange.x, this.EmitRange.y, percent);
        rate.constantMax = num;
        rate.constantMin = num;
        emission.rate = rate;
      }
    }
  }
}
