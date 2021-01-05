// Decompiled with JetBrains decompiler
// Type: FistVR.RotwienerNest_Nodule
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RotwienerNest_Nodule : MonoBehaviour, IFVRDamageable
  {
    public RotwienerNest_Nodule.NoduleState State;
    public RotwienerNest_Nodule.NoduleType Type;
    public GameObject Core;
    public GameObject Wrap_Closed;
    public GameObject Wrap_Open;
    public Collider C;
    private RotwienerNest m_nest;
    private RotwienerNest_Tendril m_tendril;
    public Transform t_Core;
    public Transform t_Wrap_Closed;
    public AudioSource AudSource_loop;
    public AudioEvent AudEvent_Scream;
    public GameObject SpawnOnSpode;
    [Header("DamageMults")]
    public float Mult_Projectile = 1f;
    public float Mult_Melee = 1f;
    public float Mult_Explosive = 1f;
    public float Mult_Piercing = 1f;
    public float Mult_Cutting = 1f;
    public float Mult_Blunt = 1f;
    public float Mult_Thermal = 1f;
    private float m_timeSinceScream;
    private float m_bounceX;
    private float m_bounceY;
    private float m_bounceZ;
    private float m_speed = 1f;
    private float m_baseSpeed = 1f;
    [Header("Bleeding Logic")]
    public float Mustard = 100f;
    private float m_maxMustard = 100f;
    public float BleedDamageMult = 0.5f;
    public float BleedRateMult = 1f;
    public float BleedVFXIntensity = 0.3f;
    private bool m_isBleeding;
    private float m_bleedRate;
    private bool m_needsToSpawnBleedEvent;
    private float m_bloodLossForVFX;
    private Vector3 m_bloodLossPoint;
    private Vector3 m_bloodLossDir;
    public GameObject DamageFX_SmallMustardBurst;
    public GameObject DamageFX_LargeMustardBurst;
    public GameObject DamageFX_MustardSpoutSmall;
    public GameObject DamageFX_MustardSpoutLarge;
    public GameObject DamageFX_Explosion;
    private List<RotwienerNest_Nodule.BleedingEvent> m_bleedingEvents = new List<RotwienerNest_Nodule.BleedingEvent>();

    public void SetType(
      RotwienerNest_Nodule.NoduleType type,
      RotwienerNest n,
      RotwienerNest_Tendril t)
    {
      this.Type = type;
      this.m_nest = n;
      this.m_tendril = t;
      this.m_bounceY = 0.33f;
      this.m_bounceZ = 0.66f;
    }

    public void SetState(RotwienerNest_Nodule.NoduleState s, bool isInit)
    {
      if (s == this.State && !isInit)
        return;
      this.State = s;
      switch (s)
      {
        case RotwienerNest_Nodule.NoduleState.Protected:
          this.Wrap_Closed.SetActive(true);
          this.Wrap_Open.SetActive(false);
          break;
        case RotwienerNest_Nodule.NoduleState.Unprotected:
          this.Wrap_Closed.SetActive(false);
          this.Wrap_Open.SetActive(true);
          if (this.Type != RotwienerNest_Nodule.NoduleType.Tendril)
            break;
          this.AudSource_loop.pitch = UnityEngine.Random.Range(0.5f, 0.7f);
          this.AudSource_loop.Play();
          break;
        case RotwienerNest_Nodule.NoduleState.Naked:
          this.Wrap_Closed.SetActive(false);
          this.Wrap_Open.SetActive(false);
          break;
        case RotwienerNest_Nodule.NoduleState.Destroyed:
          this.AudSource_loop.Stop();
          this.DestroyMe();
          if (isInit)
            break;
          UnityEngine.Object.Instantiate<GameObject>(this.SpawnOnSpode, this.Core.transform.position, UnityEngine.Random.rotation);
          GM.CurrentSceneSettings.OnPerceiveableSound(200f, 100f, this.transform.position, 0);
          break;
      }
    }

    private void DestroyMe()
    {
      if (this.Type == RotwienerNest_Nodule.NoduleType.Tendril)
      {
        this.m_tendril.NoduleDestroyed(this);
      }
      else
      {
        if (this.Type != RotwienerNest_Nodule.NoduleType.Core)
          return;
        this.m_nest.NoduleDestroyed(this);
      }
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.State == RotwienerNest_Nodule.NoduleState.Destroyed || this.State == RotwienerNest_Nodule.NoduleState.Protected)
        return;
      float num1 = 1f;
      float num2 = 0.0f;
      if (d.Class == FistVR.Damage.DamageClass.Projectile || d.Class == FistVR.Damage.DamageClass.Explosive || d.Class == FistVR.Damage.DamageClass.Melee)
      {
        num2 = d.Dam_Piercing * this.Mult_Piercing + d.Dam_Cutting * this.Mult_Cutting + d.Dam_Blunt * this.Mult_Blunt + d.Dam_Thermal * this.Mult_Thermal;
        if (d.Class == FistVR.Damage.DamageClass.Projectile)
          num2 *= this.Mult_Projectile;
        else if (d.Class == FistVR.Damage.DamageClass.Melee)
          num2 *= this.Mult_Melee;
        else if (d.Class == FistVR.Damage.DamageClass.Explosive)
          num2 *= this.Mult_Explosive;
        float bloodAmount = Mathf.Clamp((float) (((double) num2 - 50.0) * 0.0500000007450581) * num1, 0.0f, 1000f);
        this.AccurueBleedingHit(d.point, d.strikeDir, bloodAmount);
        this.m_speed = 3f * this.m_baseSpeed;
      }
      if ((double) num2 <= 500.0 || (double) this.m_timeSinceScream < 3.0)
        return;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Scream, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) / 343f);
      this.m_timeSinceScream = 0.0f;
      GM.CurrentSceneSettings.OnPerceiveableSound(50f, 60f, this.transform.position, 0);
    }

    public void Update()
    {
      this.m_baseSpeed = Mathf.Lerp(this.Mustard / this.m_maxMustard, 3f, 1f);
      this.m_speed = Mathf.MoveTowards(this.m_speed, this.m_baseSpeed, Time.deltaTime * 2f * this.m_baseSpeed);
      if ((double) this.m_timeSinceScream < 10.0)
        this.m_timeSinceScream += Time.deltaTime;
      if (this.State == RotwienerNest_Nodule.NoduleState.Protected)
      {
        this.m_bounceX = Mathf.Repeat(this.m_bounceX + Time.deltaTime * 0.3f, 1f);
        this.m_bounceY = Mathf.Repeat(this.m_bounceY + Time.deltaTime * 0.3f, 1f);
        this.m_bounceZ = Mathf.Repeat(this.m_bounceZ + Time.deltaTime * 0.3f, 1f);
        this.t_Wrap_Closed.localScale = new Vector3((float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceX * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceY * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceZ * 3.14159274101257 * 2.0)) * 0.025000000372529));
      }
      else if (this.State == RotwienerNest_Nodule.NoduleState.Unprotected)
      {
        this.m_bounceX = Mathf.Repeat(this.m_bounceX + Time.deltaTime * 0.8f * this.m_speed, 1f);
        this.m_bounceY = Mathf.Repeat(this.m_bounceY + Time.deltaTime * 0.8f * this.m_speed, 1f);
        this.m_bounceZ = Mathf.Repeat(this.m_bounceZ + Time.deltaTime * 0.8f * this.m_speed, 1f);
        this.t_Core.localScale = new Vector3((float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceX * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceY * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceZ * 3.14159274101257 * 2.0)) * 0.025000000372529));
      }
      else if (this.State == RotwienerNest_Nodule.NoduleState.Naked)
      {
        this.m_bounceX = Mathf.Repeat(this.m_bounceX + Time.deltaTime * 0.8f * this.m_speed, 1f);
        this.m_bounceY = Mathf.Repeat(this.m_bounceY + Time.deltaTime * 0.8f * this.m_speed, 1f);
        this.m_bounceZ = Mathf.Repeat(this.m_bounceZ + Time.deltaTime * 0.8f * this.m_speed, 1f);
        this.t_Core.localScale = new Vector3((float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceX * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceY * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceZ * 3.14159274101257 * 2.0)) * 0.025000000372529));
      }
      this.BleedingUpdate();
    }

    public void AccurueBleedingHit(Vector3 point, Vector3 dir, float bloodAmount)
    {
      this.m_needsToSpawnBleedEvent = true;
      this.m_bloodLossPoint = this.C.ClosestPoint(point);
      this.m_bloodLossDir = dir;
      this.m_bloodLossForVFX += bloodAmount * this.BleedDamageMult;
    }

    private void BleedingUpdate()
    {
      if (this.m_needsToSpawnBleedEvent && (double) this.Mustard > 0.0)
      {
        this.m_needsToSpawnBleedEvent = false;
        if ((double) this.m_bloodLossForVFX >= 10.0)
        {
          UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_LargeMustardBurst, this.m_bloodLossPoint, Quaternion.LookRotation(this.m_bloodLossDir));
          this.m_bleedingEvents.Add(new RotwienerNest_Nodule.BleedingEvent(this.DamageFX_MustardSpoutLarge, this.transform, this.m_bloodLossForVFX, this.m_bloodLossPoint, -this.m_bloodLossDir, this.m_bloodLossForVFX * 0.25f, this.BleedVFXIntensity));
        }
        if ((double) this.m_bloodLossForVFX >= 1.0)
        {
          UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_SmallMustardBurst, this.m_bloodLossPoint, Quaternion.LookRotation(-this.m_bloodLossDir));
          this.m_bleedingEvents.Add(new RotwienerNest_Nodule.BleedingEvent(this.DamageFX_MustardSpoutSmall, this.transform, this.m_bloodLossForVFX, this.m_bloodLossPoint, -this.m_bloodLossDir, this.m_bloodLossForVFX * 0.25f, this.BleedVFXIntensity));
        }
      }
      this.m_bleedRate = 0.0f;
      if (this.m_bleedingEvents.Count > 0)
      {
        float deltaTime = Time.deltaTime;
        for (int index = this.m_bleedingEvents.Count - 1; index >= 0; --index)
        {
          if (this.m_bleedingEvents[index].IsDone())
          {
            if ((UnityEngine.Object) this.m_bleedingEvents[index].m_system != (UnityEngine.Object) null)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bleedingEvents[index].m_system.gameObject);
            this.m_bleedingEvents.RemoveAt(index);
          }
          else
            this.m_bleedRate += this.m_bleedingEvents[index].Update(deltaTime, this.Mustard);
        }
      }
      if ((double) this.Mustard > 0.0 && (double) this.m_bleedRate > 0.0)
      {
        this.Mustard -= this.m_bleedRate * this.BleedRateMult;
        if ((double) this.Mustard <= 0.0)
          this.SetState(RotwienerNest_Nodule.NoduleState.Destroyed, false);
      }
      this.m_bloodLossForVFX = 0.0f;
    }

    public enum NoduleState
    {
      Protected,
      Unprotected,
      Naked,
      Destroyed,
    }

    public enum NoduleType
    {
      Tendril,
      Core,
    }

    [Serializable]
    public class BleedingEvent
    {
      public ParticleSystem m_system;
      public float mustardLeftToBleed;
      public float BleedIntensity;
      public float BleedVFXIntensity;

      public BleedingEvent(
        GameObject PrefabToSpawn,
        Transform p,
        float bloodAmount,
        Vector3 pos,
        Vector3 dir,
        float bleedIntensity,
        float vfxIntensity)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PrefabToSpawn, pos, Quaternion.LookRotation(dir));
        this.m_system = gameObject.GetComponent<ParticleSystem>();
        this.mustardLeftToBleed = bloodAmount;
        gameObject.transform.SetParent(p);
        this.BleedIntensity = bleedIntensity;
        this.BleedVFXIntensity = vfxIntensity;
      }

      public float Update(float t, float totalMustardLeft)
      {
        float num;
        if ((double) this.mustardLeftToBleed > 0.0 && (double) totalMustardLeft > 0.0)
        {
          num = Mathf.Clamp(this.BleedIntensity * t, 0.0f, this.mustardLeftToBleed);
          this.mustardLeftToBleed -= num;
        }
        else
        {
          this.BleedIntensity = 0.0f;
          num = 0.0f;
        }
        if ((UnityEngine.Object) this.m_system != (UnityEngine.Object) null)
        {
          ParticleSystem.EmissionModule emission = this.m_system.emission;
          ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
          rateOverTime.mode = ParticleSystemCurveMode.Constant;
          float max = 10f * this.BleedVFXIntensity;
          rateOverTime.constant = Mathf.Clamp(this.BleedIntensity * 2f, 0.0f, max);
          emission.rateOverTime = rateOverTime;
        }
        return num;
      }

      public bool IsDone() => (double) this.mustardLeftToBleed <= 0.0 && this.m_system.particleCount <= 0;

      public void EndBleedEvent()
      {
        this.mustardLeftToBleed = 0.0f;
        if (!((UnityEngine.Object) this.m_system != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_system);
      }

      public void Dispose()
      {
        if (!((UnityEngine.Object) this.m_system != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_system);
      }
    }
  }
}
