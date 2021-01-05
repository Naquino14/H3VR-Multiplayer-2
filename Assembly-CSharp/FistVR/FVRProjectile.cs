// Decompiled with JetBrains decompiler
// Type: FistVR.FVRProjectile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRProjectile : MonoBehaviour
  {
    private bool m_isActive;
    private bool m_isMoving;
    private Transform m_shotOrigin;
    [Header("Bullet Parameters")]
    public string DisplayName;
    public float PointsDamage = 1f;
    public bool DoesIgnite;
    public bool DoesFreeze;
    public bool DoesDisrupt;
    public float Mass;
    public Vector3 Dimensions;
    public float NoseContactRadius = 0.0005f;
    public bool IsExpanding;
    private bool m_isExpanded;
    public float MuzzleVelocity;
    public bool UsesAirDrag = true;
    public float GravityMultiplier = 1f;
    private bool m_isInWater;
    public PMat PMat;
    private PMaterialDefinition m_pMatDef;
    public ImpactEffectMagnitude ImpactFXMagnitude = ImpactEffectMagnitude.Medium;
    [Header("Submunitions")]
    public FVRProjectile.ProjectilePayload[] Payloads;
    public bool IsDisabledOnFirstImpact = true;
    private bool hasFiredSubmunition;
    [Header("References")]
    public Transform tracer;
    public Renderer m_tracerRenderer;
    public Renderer m_bulletRenderer;
    public GameObject ExtraDisplay;
    public float TracerLengthMultiplier = 1f;
    public float TracerWidthMultiplier = 1f;
    public bool UsesTrails = true;
    public VRTrail Trail;
    public Color TrailStartColor;
    private LayerMask LM;
    private RaycastHit m_hit;
    [Header("Life and Timeouts")]
    public float MaxRange = 500f;
    public float MaxRangeRandom;
    private float m_distanceTraveled;
    private float m_TrailDieTimer = 5f;
    private float m_TrailDieTimerMax = 5f;
    public float m_dieTimerMax = 5f;
    private float m_dieTimerTick = 5f;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_forward = Vector3.forward;
    private float m_penetration;
    private float m_tumbling;
    private float m_initialMuzzleVelocity;
    private Collider m_lastHitCollider;
    private Rigidbody m_lastHitRigidbody;
    private Vector3 m_lastHitPoint;
    private PMat m_lastHitPMat;
    private IFVRReceiveDamageable m_lastHitDamageable;
    private Vector2 m_lastHitUVCoords = Vector2.zero;
    private List<Vector3> m_pastPositions = new List<Vector3>();
    private List<float> m_pastVelocities = new List<float>();
    private FVRFireArm m_firearmSource;
    private bool m_isPlayer;
    public bool DeletesOnStraightDown = true;
    private int newTrailTick;

    private void Awake()
    {
      this.MaxRange += UnityEngine.Random.Range(0.0f, this.MaxRangeRandom);
      this.LM = AM.PLM;
      this.CalculatePenetrationStat();
      if ((UnityEngine.Object) this.tracer != (UnityEngine.Object) null)
      {
        this.tracer.gameObject.SetActive(true);
        Renderer component = this.tracer.GetComponent<Renderer>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          this.m_tracerRenderer = component;
      }
      if (GM.Options.QuickbeltOptions.AreBulletTrailsEnabled && this.UsesTrails)
      {
        this.Trail = this.gameObject.AddComponent<VRTrail>();
        this.Trail.Color = this.TrailStartColor;
      }
      this.m_TrailDieTimerMax = GM.Options.QuickbeltOptions.TrailDecayTimes[GM.Options.QuickbeltOptions.TrailDecaySetting];
      this.m_TrailDieTimer = this.m_TrailDieTimerMax;
      this.m_dieTimerTick = this.m_dieTimerMax;
      if (!((UnityEngine.Object) this.Trail != (UnityEngine.Object) null) || (double) this.m_TrailDieTimer <= (double) this.m_dieTimerTick)
        return;
      this.m_dieTimerTick = this.m_TrailDieTimer;
    }

    public void SetDamageType(DamageDealt.DamageType type)
    {
    }

    public void SetPlayerDamage(bool b) => this.m_isPlayer = b;

    public void Fire(Vector3 forwardDir, FVRFireArm firearm) => this.Fire(this.MuzzleVelocity, forwardDir, firearm);

    public void SetIsInWater(bool b) => this.m_isInWater = b;

    public void Fire(float muzzleVelocity, Vector3 forwardDir, FVRFireArm firearm)
    {
      this.m_initialMuzzleVelocity = muzzleVelocity;
      this.m_isActive = true;
      this.m_isMoving = true;
      this.m_velocity = forwardDir.normalized * muzzleVelocity;
      this.m_pastPositions.Add(this.transform.position);
      this.m_pastVelocities.Add(this.m_velocity.magnitude);
      if ((UnityEngine.Object) this.Trail != (UnityEngine.Object) null)
        this.Trail.AddPosition(this.transform.position);
      this.m_firearmSource = firearm;
      this.UpdateBulletPath();
    }

    private void TickDownToDeath()
    {
      if ((UnityEngine.Object) this.tracer != (UnityEngine.Object) null)
      {
        this.tracer.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        if ((UnityEngine.Object) this.m_tracerRenderer != (UnityEngine.Object) null)
          this.m_tracerRenderer.enabled = false;
      }
      if ((UnityEngine.Object) this.m_bulletRenderer != (UnityEngine.Object) null)
        this.m_bulletRenderer.enabled = false;
      if ((UnityEngine.Object) this.ExtraDisplay != (UnityEngine.Object) null)
        this.ExtraDisplay.SetActive(false);
      this.m_dieTimerTick -= Time.deltaTime;
      if ((double) this.m_dieTimerTick > 0.0)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void ClearLastHitData()
    {
      this.m_lastHitCollider = (Collider) null;
      this.m_lastHitRigidbody = (Rigidbody) null;
      this.m_lastHitPMat = (PMat) null;
      this.m_lastHitDamageable = (IFVRReceiveDamageable) null;
    }

    private void ExpandBullet()
    {
      if (!this.IsExpanding || this.m_isExpanded)
        return;
      this.m_isExpanded = true;
      this.Dimensions.x += this.Dimensions.z;
      this.Dimensions.y = this.Dimensions.x;
      this.Dimensions.z *= 0.25f;
      this.NoseContactRadius *= 2f;
    }

    private void CalculatePenetrationStat()
    {
      if (this.m_isExpanded)
        this.m_penetration = 1f;
      else
        this.m_penetration = Mathf.Clamp((float) (1.0 - (double) this.NoseContactRadius / (double) (this.Dimensions.x * 0.5f)), 0.1f, 1f) * 10f;
    }

    private Vector3 ApplyDrag(
      Vector3 velocity,
      float materialDensity,
      float cascadedTime,
      bool isContact,
      bool isAirDrag)
    {
      float num = !isAirDrag ? this.GetCurrentBulletArea(isContact) : 3.141593f * Mathf.Pow(this.Dimensions.x * 0.5f, 2f);
      float currentDragCoefficient = this.GetCurrentDragCoefficient(velocity.magnitude);
      Vector3 vector3 = -velocity * (materialDensity * 0.5f * currentDragCoefficient * num / this.Mass) * velocity.magnitude;
      return velocity.normalized * Mathf.Clamp(velocity.magnitude - vector3.magnitude * cascadedTime, 0.0f, velocity.magnitude);
    }

    private float GetCurrentBulletArea(bool isContact)
    {
      float num;
      if (isContact)
      {
        num = Mathf.Lerp(3.141593f * Mathf.Pow(this.NoseContactRadius, 2f), this.NoseContactRadius * this.Dimensions.z, this.m_tumbling);
      }
      else
      {
        float b = (float) (3.14159274101257 * (double) this.Dimensions.x * 0.5) * Mathf.Sqrt(Mathf.Pow(this.Dimensions.z * 0.5f, 2f) + Mathf.Pow(this.Dimensions.x * 0.5f, 2f));
        num = Mathf.Lerp((float) (3.14159274101257 * (double) Mathf.Pow(this.Dimensions.x * 0.5f, 2f) + 6.28318548202515 * (double) this.Dimensions.x * 0.5 * (double) this.Dimensions.z), b, this.m_penetration * 0.1f);
      }
      return num;
    }

    private float GetCurrentDragCoefficient(float velocityMS) => this.m_isExpanded ? 1f : AM.BDCC.Evaluate(velocityMS * 0.00291545f);

    private void FireSubmunitions(Vector3 point)
    {
      if (this.Payloads.Length <= 0 || this.hasFiredSubmunition)
        return;
      this.hasFiredSubmunition = true;
      if (this.IsDisabledOnFirstImpact)
        this.m_isMoving = false;
      for (int index1 = 0; index1 < this.Payloads.Length; ++index1)
      {
        if (this.Payloads[index1].Submunitions.Length > 0)
        {
          for (int index2 = 0; index2 < this.Payloads[index1].SubmunitionNumber; ++index2)
          {
            int index3 = UnityEngine.Random.Range(0, this.Payloads[index1].Submunitions.Length);
            Quaternion rotation = this.transform.rotation;
            if (this.Payloads[index1].IsRandomRot)
              rotation = UnityEngine.Random.rotation;
            if (this.Payloads[index1].IsBackwards)
              rotation = Quaternion.LookRotation(Vector3.up);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Payloads[index1].Submunitions[index3], point, rotation);
            FVRProjectile component = gameObject.GetComponent<FVRProjectile>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            {
              if (this.Payloads[index1].IsBackwards)
                component.Fire(component.MuzzleVelocity, gameObject.transform.forward, this.m_firearmSource);
              else
                component.Fire(component.MuzzleVelocity, Vector3.Lerp(this.transform.forward, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(0.05f, 0.7f)), this.m_firearmSource);
            }
            if (this.Payloads[index1].IsSubmunitionRomanCandle)
              gameObject.GetComponent<RomanCandleCharge>().Fire();
            if (this.Payloads[index1].IsSubmunitionRigidbody)
              gameObject.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(this.Payloads[index1].RBSpeed.x, this.Payloads[index1].RBSpeed.y);
          }
        }
      }
    }

    private void FixedUpdate()
    {
      if ((UnityEngine.Object) this.Trail != (UnityEngine.Object) null)
      {
        this.m_TrailDieTimer -= Time.deltaTime;
        if ((double) this.m_TrailDieTimer < 0.0)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.Trail);
        else
          this.Trail.Color.a = this.m_TrailDieTimer / this.m_TrailDieTimerMax;
      }
      this.UpdateBulletPath();
      if ((double) this.m_distanceTraveled <= (double) this.MaxRange || !this.m_isMoving)
        return;
      this.m_isMoving = false;
      this.FireSubmunitions(this.transform.position);
    }

    private float GetDamageVelocityScaled(float MaxDam, Vector3 velocity)
    {
      float num = velocity.magnitude / this.m_initialMuzzleVelocity;
      return MaxDam * num;
    }

    private void UpdateBulletPath()
    {
      if (!this.m_isActive || !this.m_isMoving)
      {
        this.TickDownToDeath();
      }
      else
      {
        this.UpdateVelocity(Time.fixedDeltaTime);
        this.MoveBullet(Time.fixedDeltaTime);
        this.m_pastPositions.Add(this.transform.position);
        this.m_pastVelocities.Add(this.m_velocity.magnitude);
        if ((UnityEngine.Object) this.Trail != (UnityEngine.Object) null)
        {
          if (this.newTrailTick > 0)
          {
            --this.newTrailTick;
          }
          else
          {
            this.Trail.AddPosition(this.transform.position);
            this.newTrailTick = Mathf.RoundToInt(this.m_distanceTraveled / 100f);
          }
        }
        if (!((UnityEngine.Object) this.tracer != (UnityEngine.Object) null))
          return;
        this.tracer.localScale = new Vector3(0.04f * this.TracerWidthMultiplier, 0.04f * this.TracerWidthMultiplier, Mathf.Min(this.m_velocity.magnitude * Time.deltaTime * this.TracerLengthMultiplier, this.m_distanceTraveled));
      }
    }

    private void UpdateVelocity(float cascadedTime)
    {
      if ((double) this.m_velocity.magnitude < 0.100000001490116 || (double) this.transform.position.y < -100.0)
      {
        this.m_isMoving = false;
      }
      else
      {
        if ((UnityEngine.Object) this.m_lastHitCollider == (UnityEngine.Object) null)
        {
          float num = 9.81f;
          switch (GM.Options.SimulationOptions.BallisticGravityMode)
          {
            case SimulationOptions.GravityMode.Realistic:
              num = 9.81f;
              break;
            case SimulationOptions.GravityMode.Playful:
              num = 5f;
              break;
            case SimulationOptions.GravityMode.OnTheMoon:
              num = 1.622f;
              break;
            case SimulationOptions.GravityMode.None:
              num = 0.0f;
              break;
          }
          if (this.m_isInWater)
            num = 0.0f;
          this.m_velocity += Vector3.down * num * cascadedTime * this.GravityMultiplier;
        }
        float materialDensity = 1.225f;
        if (!this.UsesAirDrag)
          materialDensity = 0.0f;
        if (this.m_isInWater)
          materialDensity = 16f;
        Vector3 vector3_1 = this.m_velocity * this.Mass;
        float num1 = 0.0f;
        bool flag1 = false;
        bool flag2 = false;
        if ((UnityEngine.Object) this.m_lastHitCollider != (UnityEngine.Object) null)
        {
          if (this.transform.position != this.m_lastHitPoint && this.m_lastHitCollider.Raycast(new Ray(this.transform.position, this.m_lastHitPoint - this.transform.position), out this.m_hit, (this.m_lastHitPoint - this.transform.position).magnitude))
          {
            this.m_tumbling += Mathf.Lerp(0.0f, Mathf.Clamp(1f - Vector3.Dot(this.transform.forward, this.m_hit.normal), 0.0f, 1f), this.m_lastHitPMat.GetStiffness() * 0.3f);
            float magnitude = this.m_velocity.magnitude;
            this.m_velocity = Vector3.Lerp(this.m_velocity, Vector3.ProjectOnPlane(this.m_velocity, this.m_hit.normal).normalized * magnitude, Mathf.Clamp(this.m_tumbling, 0.0f, 0.5f));
            flag1 = true;
          }
          else
          {
            this.ExpandBullet();
            this.CalculatePenetrationStat();
            this.m_tumbling += cascadedTime * (this.m_lastHitPMat.GetStiffness() / this.PMat.Def.stiffness) * this.m_lastHitPMat.GetBounciness();
            Vector3 lhs = UnityEngine.Random.onUnitSphere;
            if ((double) Vector3.Dot(lhs, this.m_velocity.normalized) < 0.0)
              lhs = -lhs;
            this.m_velocity = Vector3.Lerp(this.m_velocity, lhs * this.m_velocity.magnitude, Mathf.Clamp(this.m_tumbling, 0.0f, 0.9f));
            Vector3 origin = this.transform.position + this.m_velocity * cascadedTime;
            float magnitude = (this.transform.position - origin).magnitude;
            if (this.transform.position != origin && this.m_lastHitCollider.Raycast(new Ray(origin, this.transform.position - origin), out this.m_hit, magnitude))
            {
              num1 = Mathf.Clamp(Vector3.Distance(this.m_hit.point, this.transform.position) / magnitude, 0.0f, 1f);
              flag2 = true;
            }
            else
              num1 = 1f;
          }
        }
        else
          flag1 = true;
        if (flag1)
          this.ClearLastHitData();
        this.m_velocity = (double) num1 <= 0.0 ? (!this.m_isInWater ? this.ApplyDrag(this.m_velocity, materialDensity, cascadedTime, false, true) : this.ApplyDrag(this.m_velocity, materialDensity, cascadedTime, false, false)) : this.ApplyDrag(this.m_velocity, this.m_lastHitPMat.GetDensity(), cascadedTime * num1, false, false);
        Vector3 vector3_2 = this.m_velocity * this.Mass;
        if ((UnityEngine.Object) this.m_lastHitPMat != (UnityEngine.Object) null)
        {
          DamageDealt dam = new DamageDealt();
          Vector3 vector3_3 = (vector3_2 - vector3_1) / (Time.fixedDeltaTime * num1);
          float currentBulletArea = this.GetCurrentBulletArea(false);
          float num2 = vector3_3.magnitude / currentBulletArea;
          float num3 = num2 / 1000000f;
          float num4 = (float) ((double) Mathf.Pow(currentBulletArea, 0.25f) * (double) num2 / 1000000.0);
          dam.force = vector3_1 - vector3_2;
          dam.PointsDamage = this.GetDamageVelocityScaled(this.PointsDamage, this.m_velocity) * (Time.fixedDeltaTime * num1);
          dam.MPa = num3;
          dam.MPaRootMeter = num4;
          dam.point = this.transform.position;
          dam.hitNormal = -this.transform.forward;
          dam.strikeDir = this.transform.forward;
          dam.uvCoords = Vector2.zero;
          dam.SourceFirearm = this.m_firearmSource;
          dam.IsPlayer = this.m_isPlayer;
          dam.IsInitialContact = false;
          dam.IsInside = true;
          dam.DoesIgnite = this.DoesIgnite;
          dam.DoesFreeze = this.DoesFreeze;
          dam.DoesDisrupt = this.DoesDisrupt;
          if (this.m_lastHitDamageable != null)
            this.m_lastHitDamageable.Damage(dam);
        }
        if ((UnityEngine.Object) this.m_lastHitRigidbody != (UnityEngine.Object) null)
          this.m_lastHitRigidbody.AddForceAtPosition(vector3_1 - vector3_2, this.transform.position, ForceMode.Impulse);
        if (!flag2)
          return;
        this.ClearLastHitData();
      }
    }

    private void MoveBullet(float cascadedTime)
    {
      if ((double) this.m_velocity.y < 0.0 && this.DeletesOnStraightDown && (double) new Vector3(this.m_velocity.x, 0.0f, this.m_velocity.z).magnitude < 0.100000001490116 || (double) this.transform.position.y < -100.0)
      {
        this.m_isMoving = false;
      }
      else
      {
        Vector3 position = this.transform.position;
        if (Physics.Raycast(this.transform.position, this.m_velocity.normalized, out this.m_hit, this.m_velocity.magnitude * cascadedTime, (int) this.LM, QueryTriggerInteraction.Collide) && !this.m_hit.collider.gameObject.CompareTag("PlayerHand"))
        {
          this.GravityMultiplier = 1f;
          IFVRReceiveDamageable component = this.m_hit.collider.transform.gameObject.GetComponent<IFVRReceiveDamageable>();
          if (component == null && (UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null)
            component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRReceiveDamageable>();
          this.m_lastHitDamageable = component == null ? (IFVRReceiveDamageable) null : component;
          this.m_lastHitUVCoords = this.m_hit.textureCoord;
          this.m_lastHitPMat = !((UnityEngine.Object) this.m_hit.collider.gameObject.GetComponent<PMat>() != (UnityEngine.Object) null) ? PM.DefaultMat : this.m_hit.collider.gameObject.GetComponent<PMat>();
          this.m_lastHitRigidbody = !((UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null) ? (Rigidbody) null : this.m_hit.collider.attachedRigidbody;
          DamageDealt dam = new DamageDealt();
          dam.point = this.m_hit.point;
          dam.PointsDamage = this.GetDamageVelocityScaled(this.PointsDamage, this.m_velocity);
          dam.ShotOrigin = this.m_shotOrigin;
          dam.hitNormal = this.m_hit.normal;
          dam.strikeDir = this.transform.forward;
          dam.uvCoords = this.m_lastHitUVCoords;
          dam.SourceFirearm = this.m_firearmSource;
          dam.IsPlayer = this.m_isPlayer;
          dam.IsInitialContact = true;
          dam.IsInside = false;
          dam.DoesIgnite = this.DoesIgnite;
          dam.DoesFreeze = this.DoesFreeze;
          dam.DoesDisrupt = this.DoesDisrupt;
          if ((double) this.m_velocity.magnitude > 100.0)
            FXM.SpawnImpactEffect(this.m_hit.point, this.m_hit.normal, (int) this.m_lastHitPMat.Def.impactCategory, this.ImpactFXMagnitude, false);
          Vector3 vector3_1 = this.m_velocity * this.Mass;
          Vector3 vector3_2 = UnityEngine.Random.onUnitSphere;
          if ((double) Vector3.Dot(vector3_2, this.m_hit.normal) > 0.0)
            vector3_2 = -vector3_2;
          Vector3 vector3_3 = Vector3.Slerp(this.m_hit.normal, vector3_2, UnityEngine.Random.Range(0.0f, this.m_lastHitPMat.GetRoughness()));
          Vector3 normalized1 = this.m_velocity.normalized;
          Vector3.Dot(normalized1, -vector3_3);
          Vector3 normalized2 = this.m_velocity.normalized;
          float num1 = Vector3.Dot(normalized2, vector3_3);
          float num2 = (float) (1.0 / ((1.0 + (double) this.m_lastHitPMat.GetStiffness()) / (1.0 + (double) this.m_penetration)));
          float f1 = (float) (1.0 - (1.0 - (double) num1 * (double) num1) / ((double) num2 * (double) num2));
          bool flag = true;
          float num3 = 1f;
          float f2 = 0.0f;
          if ((double) f1 > 0.0)
          {
            this.m_lastHitCollider = this.m_hit.collider;
            this.m_lastHitPoint = this.m_hit.point;
            float num4 = Mathf.Sqrt(f1);
            this.m_velocity = (num2 * normalized2 + (-num2 * num1 - num4) * -vector3_3).normalized * this.m_velocity.magnitude;
            if ((double) Vector3.Dot(this.m_velocity.normalized, this.m_hit.normal) > 0.0)
              this.m_velocity = Vector3.Reflect(this.m_velocity, this.m_hit.normal);
            float num5 = this.m_velocity.magnitude * this.Mass;
            f2 = this.GetCurrentBulletArea(true);
            float num6 = num5 / (Time.fixedDeltaTime * 0.1f) / f2 / 1000000f;
            if ((double) num6 >= (double) this.m_lastHitPMat.GetYieldStrength())
            {
              flag = false;
              this.m_velocity = Vector3.ClampMagnitude(this.m_velocity, this.m_velocity.magnitude * ((num6 - this.m_lastHitPMat.GetYieldStrength()) / num6));
            }
            else
            {
              flag = true;
              num3 = Mathf.Clamp((float) (1.0 - (double) num6 / (double) this.m_lastHitPMat.GetYieldStrength()), 0.0f, 1f);
            }
          }
          if (!flag)
          {
            Vector3 vector3_4 = this.m_velocity * this.Mass;
            this.transform.position = this.m_hit.point - this.m_hit.normal * (1f / 400f);
            float num4 = ((vector3_4 - vector3_1) / (Time.fixedDeltaTime * 0.1f)).magnitude / f2;
            float num5 = num4 / 1000000f;
            float num6 = (float) ((double) Mathf.Pow(f2, 0.25f) * (double) num4 / 1000000.0);
            dam.force = vector3_1 - vector3_4;
            dam.MPa = num5;
            dam.MPaRootMeter = num6;
            this.ExpandBullet();
            this.CalculatePenetrationStat();
            if ((UnityEngine.Object) this.m_lastHitRigidbody != (UnityEngine.Object) null && this.m_lastHitDamageable == null)
              this.m_lastHitRigidbody.AddForceAtPosition(vector3_1 - vector3_4, this.m_hit.point, ForceMode.Impulse);
            this.FireSubmunitions(this.m_hit.point + this.m_hit.normal * 0.05f);
            if (this.m_velocity.normalized != Vector3.zero)
              this.transform.rotation = Quaternion.LookRotation(this.m_velocity.normalized);
            if (this.m_lastHitDamageable != null)
              this.m_lastHitDamageable.Damage(dam);
          }
          if (flag)
          {
            this.transform.position = this.m_hit.point + this.m_hit.normal * (1f / 400f);
            this.m_velocity = Vector3.Reflect(this.m_velocity, vector3_3);
            this.transform.rotation = Quaternion.LookRotation(this.m_velocity.normalized);
            this.m_velocity *= Mathf.Clamp((float) (((double) this.PMat.Def.bounciness + (double) this.m_lastHitPMat.GetBounciness()) * 0.5), 0.0f, 1f);
            this.m_velocity *= num3;
            this.m_tumbling += Mathf.Abs(Vector3.Dot(vector3_3, normalized1));
            Vector3 vector3_4 = this.m_velocity * this.Mass;
            Vector3 vector3_5 = (vector3_4 - vector3_1) / (Time.fixedDeltaTime * 0.1f);
            float currentBulletArea = this.GetCurrentBulletArea(true);
            float num4 = vector3_5.magnitude / currentBulletArea;
            float num5 = num4 / 1000000f;
            float num6 = (float) ((double) Mathf.Pow(currentBulletArea, 0.25f) * (double) num4 / 1000000.0);
            dam.force = vector3_1 - vector3_4;
            dam.MPa = num5;
            dam.MPaRootMeter = num6;
            if ((UnityEngine.Object) this.m_lastHitRigidbody != (UnityEngine.Object) null)
              this.m_lastHitRigidbody.AddForceAtPosition(vector3_1 - vector3_4, this.m_hit.point, ForceMode.Impulse);
            if (this.m_lastHitDamageable != null)
              this.m_lastHitDamageable.Damage(dam);
            this.FireSubmunitions(this.m_hit.point + this.m_hit.normal * 0.05f);
          }
        }
        else
        {
          this.transform.position = this.transform.position + this.m_velocity * cascadedTime;
          if (this.m_velocity.normalized != Vector3.zero)
            this.transform.rotation = Quaternion.LookRotation(this.m_velocity);
        }
        this.m_distanceTraveled += Vector3.Distance(this.transform.position, position);
      }
    }

    [Serializable]
    public class ProjectilePayload
    {
      public GameObject[] Submunitions;
      public int SubmunitionNumber;
      public bool IsRandomRot = true;
      public bool IsSubmunitionRomanCandle;
      public bool IsBackwards;
      public bool IsSubmunitionRigidbody;
      public Vector2 RBSpeed = new Vector2();
    }
  }
}
