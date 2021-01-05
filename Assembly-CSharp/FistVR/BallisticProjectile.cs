// Decompiled with JetBrains decompiler
// Type: FistVR.BallisticProjectile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BallisticProjectile : MonoBehaviour
  {
    [Header("Projectile Parameters")]
    public float Mass;
    public Vector3 Dimensions;
    public float FrontArea;
    public float MuzzleVelocityBase;
    public BallisticProjectileType ProjType;
    public bool DoesIgniteOnHit;
    public float IgnitionChance = 0.2f;
    public float KETotalForHit;
    public float KEPerSquareMeterBase;
    public float FlightVelocityMultiplier = 1f;
    private float m_debugFlightVelGlobal = 1f;
    public float AirDragMultiplier = 1f;
    public float GravityMultiplier = 1f;
    public bool IsDisabledOnFirstImpact;
    public bool GeneratesImpactSound;
    public ImpactType ImpactSoundType = ImpactType.GunshotImpact;
    public bool GeneratesImpactDecals;
    public ImpactEffectMagnitude ImpactFXMagnitude = ImpactEffectMagnitude.Medium;
    public bool GeneratesSuppressionEvent = true;
    public float SuppressionIntensity = 1f;
    public float SuppressionRange = 5f;
    public bool DeletesOnStraightDown = true;
    public int Source_IFF;
    public bool UsesIFFMatSwap;
    public List<Material> IFFSwapMats;
    [Header("Life and Timeouts")]
    public float MaxRange = 500f;
    public float MaxRangeRandom;
    private float m_distanceTraveled;
    private float m_TrailDieTimer = 5f;
    private float m_TrailDieTimerMax = 5f;
    public float m_dieTimerMax = 5f;
    private float m_dieTimerTick = 5f;
    [Header("Tracer")]
    public Transform tracer;
    public Renderer TracerRenderer;
    private bool m_hasTracer;
    public Renderer BulletRenderer;
    private bool m_hasBulletRenderer;
    public GameObject ExtraDisplay;
    private bool m_hasExtraDisplay;
    public float TracerLengthMultiplier = 1f;
    public float TracerWidthMultiplier = 1f;
    [Header("Trails")]
    public bool UsesTrails = true;
    public VRTrail Trail;
    public Color TrailStartColor;
    private int newTrailTick;
    private float m_tracerDistanceScaleFactor = 0.02f;
    private bool m_isMoving;
    private bool m_isInWater;
    private bool m_isTumbling;
    private int m_stallFrames;
    private LayerMask LM;
    private RaycastHit m_hit;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_forward = Vector3.forward;
    private Vector3 m_lastPoint = Vector3.zero;
    private float m_gravMag = 9.81f;
    private float m_initialMuzzleVelocity;
    public BallisticImpactEffectType ImpactEffectTypeOverride = BallisticImpactEffectType.None;
    public BulletHoleDecalType BulletHoleDecalOverride;
    [Header("Submunitions")]
    public List<BallisticProjectile.Submunition> Submunitions;
    private bool m_usesSubmunitions;
    private bool m_hasFiredSubmunitions;
    public bool PassesFirearmReferenceToSubmunitions;
    [Header("BlastJump")]
    public bool DoesBlastJumpOnFire;
    public float BlastJumpAmount;
    private Transform m_cachedHead;
    private FVRFireArm tempFA;
    private bool waitOneFrame;
    private bool hasTurnedOffRends;
    private bool m_isInReferenceTransform;
    private Vector3 m_localReferencePoint;
    private Transform m_transReference;
    private bool needsSecondCast;
    private Collider m_lastColliderHit;
    private float distMoved;
    private bool m_hasPlayedWhoosh;
    private float m_distanceFromPlayer;

    public void SetSource_IFF(int i)
    {
      this.Source_IFF = i;
      if (!this.UsesIFFMatSwap)
        return;
      i = Mathf.Clamp(i, 0, 2);
      this.BulletRenderer.material = this.IFFSwapMats[i];
    }

    private void Awake()
    {
      this.m_cachedHead = GM.CurrentPlayerBody.Head;
      this.m_distanceFromPlayer = Vector3.Distance(this.transform.position, this.m_cachedHead.position);
      this.MaxRange = Mathf.Clamp(this.MaxRange, this.MaxRange, GM.CurrentSceneSettings.MaxProjectileRange);
      this.MaxRange += UnityEngine.Random.Range(0.0f, this.MaxRangeRandom);
      if (this.Submunitions.Count > 0)
        this.m_usesSubmunitions = true;
      this.LM = AM.PLM;
      switch (GM.Options.SimulationOptions.BallisticGravityMode)
      {
        case SimulationOptions.GravityMode.Realistic:
          this.m_gravMag = 9.81f;
          break;
        case SimulationOptions.GravityMode.Playful:
          this.m_gravMag = 5f;
          break;
        case SimulationOptions.GravityMode.OnTheMoon:
          this.m_gravMag = 1.622f;
          break;
        case SimulationOptions.GravityMode.None:
          this.m_gravMag = 0.0f;
          break;
      }
      if ((UnityEngine.Object) this.TracerRenderer != (UnityEngine.Object) null)
      {
        this.m_hasTracer = true;
        this.tracer.gameObject.SetActive(true);
        this.TracerRenderer.enabled = true;
      }
      if ((UnityEngine.Object) this.BulletRenderer != (UnityEngine.Object) null)
        this.m_hasBulletRenderer = true;
      if ((UnityEngine.Object) this.ExtraDisplay != (UnityEngine.Object) null)
        this.m_hasExtraDisplay = true;
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

    public void Fire(Vector3 forwardDir, FVRFireArm firearm) => this.Fire(this.MuzzleVelocityBase, forwardDir, firearm);

    public void Fire(float muzzleVelocity, Vector3 forwardDir, FVRFireArm firearm)
    {
      if ((UnityEngine.Object) firearm != (UnityEngine.Object) null)
        this.SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
      if (this.PassesFirearmReferenceToSubmunitions && (UnityEngine.Object) firearm != (UnityEngine.Object) null)
        this.tempFA = firearm;
      if (this.DoesBlastJumpOnFire)
        GM.CurrentMovementManager.Blast(-forwardDir, this.BlastJumpAmount);
      this.m_initialMuzzleVelocity = muzzleVelocity;
      this.m_isMoving = true;
      this.m_velocity = forwardDir.normalized * muzzleVelocity;
      this.m_lastPoint = this.transform.position;
      if ((UnityEngine.Object) this.Trail != (UnityEngine.Object) null)
        this.Trail.AddPosition(this.transform.position);
      this.UpdateBulletPath();
    }

    private Vector3 ApplyDrag(Vector3 velocity, float materialDensity, float time)
    {
      float num = 3.141593f * Mathf.Pow(this.Dimensions.x * 0.5f, 2f);
      float magnitude = velocity.magnitude;
      Vector3 normalized = velocity.normalized;
      float currentDragCoefficient = this.GetCurrentDragCoefficient(velocity.magnitude);
      Vector3 vector3 = -velocity * (materialDensity * 0.5f * currentDragCoefficient * num / this.Mass) * magnitude;
      return normalized * Mathf.Clamp(magnitude - vector3.magnitude * time, 0.0f, magnitude);
    }

    private float GetCurrentDragCoefficient(float velocityMS) => AM.BDCC.Evaluate(velocityMS * 0.00291545f);

    private void TickDownToDeath()
    {
      if (!this.waitOneFrame)
        this.waitOneFrame = true;
      else if (!this.hasTurnedOffRends)
      {
        this.hasTurnedOffRends = true;
        if (this.m_hasBulletRenderer)
          this.BulletRenderer.enabled = false;
        if (this.m_hasTracer)
          this.TracerRenderer.enabled = false;
        if (this.m_hasExtraDisplay)
          this.ExtraDisplay.SetActive(false);
      }
      this.m_dieTimerTick -= Time.deltaTime;
      if ((double) this.m_dieTimerTick > 0.0)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
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
      Vector3 normalized = this.m_velocity.normalized;
      this.FireSubmunitions(normalized, normalized, this.transform.position, this.m_velocity.magnitude);
    }

    private void UpdateBulletPath()
    {
      if (this.m_stallFrames > 0)
      {
        --this.m_stallFrames;
      }
      else
      {
        bool flag = false;
        Vector3 position = this.transform.position;
        if (this.m_lastPoint != position)
          flag = true;
        this.m_tracerDistanceScaleFactor = Mathf.MoveTowards(this.m_tracerDistanceScaleFactor, 1f, Time.deltaTime * 4f * this.FlightVelocityMultiplier * this.m_debugFlightVelGlobal);
        if (flag)
        {
          Vector3 forward = position - this.m_lastPoint;
          this.transform.rotation = Quaternion.LookRotation(forward);
          if (this.m_hasTracer)
            this.tracer.localScale = new Vector3(this.TracerWidthMultiplier * 0.1f * this.m_tracerDistanceScaleFactor, this.TracerWidthMultiplier * 0.1f * this.m_tracerDistanceScaleFactor, forward.magnitude * this.TracerLengthMultiplier);
        }
        if (this.m_lastPoint != position)
          this.m_lastPoint = position;
        if (!this.m_isMoving)
        {
          this.TickDownToDeath();
        }
        else
        {
          float deltaTime = Time.deltaTime;
          this.UpdateVelocity(deltaTime);
          this.MoveBullet(deltaTime);
          if (!((UnityEngine.Object) this.Trail != (UnityEngine.Object) null))
            return;
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
      }
    }

    private void UpdateVelocity(float t)
    {
      if ((double) this.m_velocity.magnitude < 0.100000001490116 || (double) this.transform.position.y < -350.0)
      {
        this.m_isMoving = false;
      }
      else
      {
        this.m_velocity += Vector3.down * this.m_gravMag * t * this.GravityMultiplier;
        float materialDensity = 1.225f * this.AirDragMultiplier;
        if (this.m_isTumbling)
          materialDensity *= 5f;
        this.m_velocity = this.ApplyDrag(this.m_velocity, materialDensity, t);
      }
    }

    private void MoveBullet(float t)
    {
      if ((double) this.m_velocity.y < 0.0 && this.DeletesOnStraightDown && (double) new Vector3(this.m_velocity.x, 0.0f, this.m_velocity.z).magnitude < 0.100000001490116 || (double) this.transform.position.y < -350.0)
      {
        this.m_isMoving = false;
      }
      else
      {
        Vector3 vector3_1 = this.transform.position;
        if (this.m_isInReferenceTransform && (UnityEngine.Object) this.m_transReference != (UnityEngine.Object) null)
          vector3_1 = this.m_transReference.TransformPoint(this.m_localReferencePoint);
        this.m_isInReferenceTransform = false;
        this.m_transReference = (Transform) null;
        Vector3 normalized = this.m_velocity.normalized;
        float magnitude = this.m_velocity.magnitude;
        float num1 = magnitude * t * this.FlightVelocityMultiplier * this.m_debugFlightVelGlobal;
        if (this.needsSecondCast)
          num1 = Mathf.Clamp(num1 - this.distMoved, 0.0f, num1);
        this.needsSecondCast = false;
        if (!Physics.Raycast(vector3_1, normalized, out this.m_hit, num1, (int) this.LM, QueryTriggerInteraction.Collide))
        {
          this.transform.position = vector3_1 + this.m_velocity * t * this.FlightVelocityMultiplier * this.m_debugFlightVelGlobal;
          if (!this.m_hasPlayedWhoosh && this.Source_IFF != GM.CurrentPlayerBody.GetPlayerIFF())
          {
            Vector3 closestValidPoint = this.GetClosestValidPoint(vector3_1, this.transform.position, this.m_cachedHead.position);
            float num2 = Vector3.Distance(closestValidPoint, this.m_cachedHead.position);
            if ((double) num2 < 3.0)
            {
              float volumeMult = Mathf.Lerp(1f, 0.2f, Mathf.Clamp(num2 - 1f, 0.0f, 2f) * 0.5f);
              if ((double) Vector3.Dot((this.m_cachedHead.position - vector3_1).normalized, normalized) > 0.0)
              {
                this.m_hasPlayedWhoosh = true;
                double num3 = (double) SM.PlayBulletImpactHit(BulletImpactSoundType.ZWhooshes, closestValidPoint, volumeMult, 1f);
              }
            }
          }
          if (!(this.m_velocity.normalized != Vector3.zero))
            ;
        }
        else
        {
          this.m_isTumbling = true;
          if ((UnityEngine.Object) this.m_lastColliderHit != (UnityEngine.Object) null && (UnityEngine.Object) this.m_hit.collider == (UnityEngine.Object) this.m_lastColliderHit)
          {
            this.needsSecondCast = true;
            this.distMoved = this.m_hit.distance;
            this.transform.position = this.m_hit.point - this.m_hit.normal * (1f / 1000f);
            this.MoveBullet(t);
            return;
          }
          bool flag1 = false;
          if ((UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null)
            flag1 = true;
          this.m_lastColliderHit = this.m_hit.collider;
          bool flag2 = false;
          IFVRDamageable component1 = this.m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
          if (component1 == null && flag1)
            component1 = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
          if (component1 != null)
            flag2 = true;
          if (this.DoesIgniteOnHit && (double) UnityEngine.Random.Range(0.0f, 1f) < (double) this.IgnitionChance)
          {
            FVRIgnitable component2 = this.m_hit.collider.transform.gameObject.GetComponent<FVRIgnitable>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
              FXM.Ignite(component2, 1f);
            else if (flag1)
            {
              FVRIgnitable component3 = this.m_hit.collider.attachedRigidbody.GetComponent<FVRIgnitable>();
              if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                FXM.Ignite(component3, 1f);
            }
          }
          bool flag3 = false;
          PMat component4 = this.m_hit.collider.transform.gameObject.GetComponent<PMat>();
          if ((UnityEngine.Object) component4 == (UnityEngine.Object) null && flag1)
            component4 = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<PMat>();
          if ((UnityEngine.Object) component4 != (UnityEngine.Object) null && (UnityEngine.Object) component4.MatDef != (UnityEngine.Object) null)
            flag3 = true;
          Rigidbody rigidbody = (Rigidbody) null;
          bool flag4 = false;
          if (flag1)
          {
            rigidbody = this.m_hit.collider.attachedRigidbody;
            flag4 = true;
            this.m_isInReferenceTransform = true;
            this.m_transReference = this.m_hit.collider.attachedRigidbody.gameObject.transform;
          }
          MatDef matDef = flag3 ? component4.MatDef : PM.DefaultMatDef;
          if (this.GeneratesImpactSound && GM.Options.SimulationOptions.HitSoundMode == SimulationOptions.HitSounds.Enabled)
          {
            float pitchmult = 0.8f;
            float num2 = 1f;
            if (this.ImpactFXMagnitude == ImpactEffectMagnitude.Tiny || this.ImpactFXMagnitude == ImpactEffectMagnitude.Small)
            {
              num2 = 0.4f;
              pitchmult = 1.3f;
            }
            else if (this.ImpactFXMagnitude == ImpactEffectMagnitude.Medium)
            {
              num2 = 0.7f;
              pitchmult = 1f;
            }
            float volumeMult = num2 * Mathf.InverseLerp(0.0f, this.m_initialMuzzleVelocity, magnitude);
            double num3 = (double) SM.PlayBulletImpactHit(matDef.BulletImpactSound, this.m_hit.point, 25f, volumeMult, pitchmult);
          }
          if (!flag1 && this.GeneratesImpactDecals && GM.Options.SimulationOptions.HitDecalMode == SimulationOptions.HitDecals.Enabled)
          {
            BulletHoleDecalType t1 = matDef.BulletHoleType;
            if (t1 != BulletHoleDecalType.None && this.BulletHoleDecalOverride != BulletHoleDecalType.None)
              t1 = this.BulletHoleDecalOverride;
            float damageSize = Mathf.Clamp(this.Dimensions.x, 0.0001f, 0.02f);
            if (t1 != BulletHoleDecalType.None)
              FXM.SpawnBulletDecal(t1, this.m_hit.point, this.m_hit.normal, damageSize);
          }
          BallisticMatSeries matSeries = PM.GetMatSeries(matDef.BallisticType, this.ProjType);
          float ricochetLimit = matSeries.RicochetLimit;
          float num4 = Vector3.Angle(normalized, -this.m_hit.normal);
          bool flag5 = false;
          bool flag6 = false;
          bool flag7 = false;
          bool flag8 = false;
          float num5 = (float) ((double) (0.5f * this.Mass * Mathf.Pow(magnitude, 2f)) / (double) this.FrontArea * 0.00999999977648258 / 140.0);
          float num6 = 0.5f * this.Mass * Mathf.Pow(magnitude, 2f);
          float num7 = 0.0f;
          float num8 = 0.0f;
          float num9 = 0.0f;
          Vector3 vector3_2 = normalized;
          Vector3 vector3_3 = this.m_velocity;
          if ((double) num4 >= (double) ricochetLimit)
            flag5 = true;
          float t2 = Mathf.Max((float) (1.0 - (double) num4 / 90.0), matSeries.MinAngularAbsord);
          float num10 = Mathf.Lerp(0.0f, matSeries.Absorption, t2);
          float num11 = num5 * t2;
          float num12;
          float num13;
          float num14;
          if ((double) num11 > (double) matSeries.PenThreshold && !this.IsDisabledOnFirstImpact)
          {
            flag6 = true;
            float num2 = Mathf.Clamp(matSeries.PenThreshold / num11 + num10, 0.0f, 1f);
            num12 = num2 * num6;
            num13 = num6 - num12;
            num14 = num2 * num5;
          }
          else if ((double) num7 > (double) matSeries.ShatterThreshold || this.IsDisabledOnFirstImpact)
          {
            flag7 = true;
            num12 = num6;
            num13 = 0.0f;
            num14 = num5;
          }
          else
          {
            float num2 = num6 * t2;
            num8 = num6 - num2;
            num9 = num5 * t2;
            flag5 = true;
            float num3 = Mathf.Clamp(num2 / num6 + num10, 0.0f, 1f);
            num12 = num3 * num6;
            num13 = num6 - num12;
            num14 = num5 * num3;
          }
          if (this.m_usesSubmunitions)
          {
            flag7 = true;
            flag8 = true;
          }
          Vector3 vector3_4 = vector3_2;
          Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
          Vector3 vector3_5;
          if (flag5 && !flag7)
          {
            vector3_5 = this.m_hit.point + this.m_hit.normal * (1f / 1000f);
            Vector3 vector3_6 = Vector3.Lerp(Vector3.Reflect(normalized, this.m_hit.normal), onUnitSphere, matSeries.Roughness);
            vector3_2 = Vector3.Lerp(Vector3.ProjectOnPlane(vector3_6, this.m_hit.normal), vector3_6, Mathf.Clamp(t2 * t2, 0.1f, 1f));
            vector3_4 = vector3_2;
          }
          else if (flag6 && !flag7)
          {
            if (matSeries.StopsOnPen)
            {
              num12 = num6;
              num13 = 0.0f;
              num14 = num5;
              flag8 = true;
            }
            vector3_5 = this.m_hit.point - this.m_hit.normal * (1f / 1000f);
            vector3_2 = Vector3.Lerp(normalized, onUnitSphere, matSeries.Roughness * t2);
            if ((double) Vector3.Angle(vector3_2, this.m_hit.normal) < 90.0)
              vector3_2 = Vector3.Reflect(vector3_2, this.m_hit.normal);
            if (matSeries.DownGradesOnPen)
              this.ProjType = matSeries.DownGradesTo;
          }
          else
          {
            vector3_5 = this.m_hit.point;
            Vector3.Lerp(Vector3.Reflect(normalized, this.m_hit.normal), onUnitSphere, matSeries.Roughness);
            vector3_4 = Vector3.Lerp(Vector3.ProjectOnPlane(vector3_2, this.m_hit.normal), vector3_2, Mathf.Clamp(t2 * t2, 0.1f, 1f));
          }
          float num15 = 0.0f;
          if (!flag8 && !flag7 && (double) num13 > 0.0)
          {
            num15 = Mathf.Sqrt(2f * num13) / Mathf.Sqrt(this.Mass);
            vector3_3 = vector3_2 * num15;
          }
          this.transform.position = vector3_5;
          if (this.m_isInReferenceTransform)
            this.m_localReferencePoint = this.m_transReference.InverseTransformPoint(vector3_5);
          this.m_velocity = vector3_3;
          if (flag8 || flag7 || ((double) num15 <= 0.0 || (double) num13 <= 0.0))
          {
            this.m_isMoving = false;
            this.m_velocity = Vector3.zero;
            if ((UnityEngine.Object) this.Trail != (UnityEngine.Object) null)
              this.Trail.AddPosition(this.transform.position);
          }
          float f = num12;
          float num16 = 0.0f;
          if (matSeries.MaterialType == MatBallisticType.MeatSolid || matSeries.MaterialType == MatBallisticType.MeatThick)
            num16 = Mathf.Clamp(num13, 0.0f, matSeries.PenThreshold * 2f * t2);
          if (flag2)
          {
            Damage dam = new Damage();
            dam.Class = Damage.DamageClass.Projectile;
            dam.Dam_TotalKinetic = num12;
            if (flag6)
            {
              switch (this.ProjType)
              {
                case BallisticProjectileType.Slug:
                  dam.Dam_Piercing = num14 * 0.3f;
                  dam.Dam_Blunt = num12 * 0.7f;
                  break;
                case BallisticProjectileType.Expanding:
                  dam.Dam_Piercing = num14 * 0.5f;
                  dam.Dam_Blunt = num12 * 0.5f;
                  break;
                case BallisticProjectileType.FMJ:
                  dam.Dam_Piercing = num14 * 0.7f;
                  dam.Dam_Blunt = num12 * 0.3f;
                  break;
                case BallisticProjectileType.Penetrator:
                  dam.Dam_Piercing = num14 * 0.9f;
                  dam.Dam_Blunt = num12 * 0.1f;
                  break;
              }
              dam.Dam_Piercing += num16 * 1f;
            }
            else if (flag7)
            {
              switch (this.ProjType)
              {
                case BallisticProjectileType.Slug:
                  dam.Dam_Piercing = num14 * 0.1f;
                  dam.Dam_Blunt = num12 * 0.9f;
                  break;
                case BallisticProjectileType.Expanding:
                  dam.Dam_Piercing = num14 * 0.3f;
                  dam.Dam_Blunt = num12 * 0.7f;
                  break;
                case BallisticProjectileType.FMJ:
                  dam.Dam_Piercing = num14 * 0.4f;
                  dam.Dam_Blunt = num12 * 0.6f;
                  break;
                case BallisticProjectileType.Penetrator:
                  dam.Dam_Piercing = num14 * 0.5f;
                  dam.Dam_Blunt = num12 * 0.5f;
                  break;
              }
            }
            else if (flag5)
              dam.Dam_Blunt = num12;
            if (this.DoesIgniteOnHit)
              dam.Dam_Thermal = 50f;
            if (this.Source_IFF == GM.CurrentPlayerBody.GetPlayerIFF() && (GM.CurrentPlayerBody.isDamPowerUp || GM.CurrentPlayerBody.IsDamPowerDown))
            {
              float damageMult = GM.CurrentPlayerBody.GetDamageMult();
              dam.Dam_Piercing *= damageMult;
              dam.Dam_Blunt *= damageMult;
              dam.Dam_Cutting *= damageMult;
              dam.Dam_TotalKinetic *= damageMult;
              dam.Dam_Blinding *= damageMult;
              dam.Dam_Chilling *= damageMult;
              dam.Dam_EMP *= damageMult;
              dam.Dam_Stunning *= damageMult;
              dam.Dam_Thermal *= damageMult;
              dam.Dam_TotalEnergetic *= damageMult;
            }
            dam.point = this.m_hit.point;
            dam.hitNormal = this.m_hit.normal;
            dam.strikeDir = normalized;
            dam.damageSize = this.Dimensions.x;
            dam.Source_IFF = this.Source_IFF;
            f = dam.Dam_Blunt;
            component1.Damage(dam);
          }
          if (flag4)
            rigidbody.AddForceAtPosition(normalized * Mathf.Sqrt(f), this.m_hit.point, ForceMode.Force);
          BallisticImpactEffectType impactEffectType = matDef.ImpactEffectType;
          if (this.ImpactEffectTypeOverride != BallisticImpactEffectType.None)
            impactEffectType = this.ImpactEffectTypeOverride;
          if (flag7 || flag5)
          {
            if ((double) magnitude > 100.0)
              FXM.SpawnImpactEffect(this.m_hit.point, vector3_4, (int) impactEffectType, this.ImpactFXMagnitude, false);
          }
          else if (flag6)
          {
            if ((double) magnitude > 100.0)
            {
              if (flag8 || (double) num15 == 0.0)
                FXM.SpawnImpactEffect(this.m_hit.point, -normalized, (int) impactEffectType, this.ImpactFXMagnitude, false);
              else
                FXM.SpawnImpactEffect(this.m_hit.point, -normalized, (int) impactEffectType, this.ImpactFXMagnitude, true);
            }
          }
          else
            Debug.Log((object) "No effect played, that's weird");
          if (this.GeneratesSuppressionEvent)
          {
            this.GeneratesSuppressionEvent = false;
            GM.CurrentSceneSettings.OnSuppressingEvent(vector3_5, normalized, this.Source_IFF, this.SuppressionIntensity, this.SuppressionRange);
          }
          this.FireSubmunitions(vector3_4, normalized, this.m_hit.point, this.m_velocity.magnitude);
          this.m_stallFrames = UnityEngine.Random.Range(0, 3);
        }
        this.m_distanceTraveled += Vector3.Distance(this.transform.position, vector3_1);
      }
    }

    private void FireSubmunitions(
      Vector3 shatterRicochetDir,
      Vector3 velNorm,
      Vector3 hitPoint,
      float VelocityOverride)
    {
      if (!this.m_usesSubmunitions || this.m_hasFiredSubmunitions)
        return;
      this.m_hasFiredSubmunitions = true;
      for (int index1 = 0; index1 < this.Submunitions.Count; ++index1)
      {
        BallisticProjectile.Submunition submunition = this.Submunitions[index1];
        Vector3 forward = shatterRicochetDir;
        Vector3 position = hitPoint;
        for (int index2 = 0; index2 < submunition.NumToSpawn; ++index2)
        {
          GameObject prefab = submunition.Prefabs[UnityEngine.Random.Range(0, submunition.Prefabs.Count)];
          float muzzleVelocity = UnityEngine.Random.Range(submunition.Speed.x, submunition.Speed.y);
          switch (submunition.Trajectory)
          {
            case BallisticProjectile.Submunition.SubmunitionTrajectoryType.Random:
              forward = UnityEngine.Random.onUnitSphere;
              break;
            case BallisticProjectile.Submunition.SubmunitionTrajectoryType.Backwards:
              forward = Vector3.Lerp(-velNorm, shatterRicochetDir, 0.5f);
              break;
            case BallisticProjectile.Submunition.SubmunitionTrajectoryType.Forwards:
              forward = velNorm;
              break;
            case BallisticProjectile.Submunition.SubmunitionTrajectoryType.ForwardsCone:
              forward = Vector3.Lerp(UnityEngine.Random.onUnitSphere, velNorm, submunition.ConeLerp);
              break;
          }
          switch (submunition.SpawnLogic)
          {
            case BallisticProjectile.Submunition.SubmunitionSpawnLogic.Outside:
              position += this.m_hit.normal * (1f / 1000f);
              break;
            case BallisticProjectile.Submunition.SubmunitionSpawnLogic.Inside:
              position -= this.m_hit.normal * (1f / 1000f);
              break;
          }
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, position, Quaternion.LookRotation(forward));
          switch (submunition.Type)
          {
            case BallisticProjectile.Submunition.SubmunitionType.GameObject:
              Explosion component1 = gameObject.GetComponent<Explosion>();
              if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
                component1.IFF = this.Source_IFF;
              ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
              if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
              {
                component2.IFF = this.Source_IFF;
                break;
              }
              break;
            case BallisticProjectile.Submunition.SubmunitionType.Projectile:
              BallisticProjectile component3 = gameObject.GetComponent<BallisticProjectile>();
              component3.Source_IFF = this.Source_IFF;
              component3.Fire(muzzleVelocity, gameObject.transform.forward, (FVRFireArm) null);
              break;
            case BallisticProjectile.Submunition.SubmunitionType.Rigidbody:
              gameObject.GetComponent<Rigidbody>().velocity = forward * muzzleVelocity;
              break;
            case BallisticProjectile.Submunition.SubmunitionType.StickyBomb:
              gameObject.GetComponent<Rigidbody>().velocity = forward * VelocityOverride;
              if (this.PassesFirearmReferenceToSubmunitions)
              {
                MF2_StickyBomb component4 = gameObject.GetComponent<MF2_StickyBomb>();
                component4.SetIFF(this.Source_IFF);
                if ((UnityEngine.Object) component4 != (UnityEngine.Object) null && (UnityEngine.Object) this.tempFA != (UnityEngine.Object) null && (this.tempFA as ClosedBoltWeapon).UsesStickyDetonation)
                {
                  (this.tempFA as ClosedBoltWeapon).RegisterStickyBomb(component4);
                  break;
                }
                break;
              }
              break;
            case BallisticProjectile.Submunition.SubmunitionType.MeleeThrown:
              gameObject.GetComponent<Rigidbody>().velocity = forward * muzzleVelocity;
              break;
            case BallisticProjectile.Submunition.SubmunitionType.Demonade:
              gameObject.GetComponent<Rigidbody>().velocity = forward * muzzleVelocity;
              gameObject.GetComponent<MF2_Demonade>().SetIFF(this.Source_IFF);
              break;
          }
        }
      }
    }

    public Vector3 GetClosestValidPoint(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
      Vector3 rhs = vPoint - vA;
      Vector3 normalized = (vB - vA).normalized;
      float num1 = Vector3.Distance(vA, vB);
      float num2 = Vector3.Dot(normalized, rhs);
      if ((double) num2 <= 0.0)
        return vA;
      if ((double) num2 >= (double) num1)
        return vB;
      Vector3 vector3 = normalized * num2;
      return vA + vector3;
    }

    [ContextMenu("Migrate")]
    public void Migrate()
    {
      FVRProjectile component = this.GetComponent<FVRProjectile>();
      this.Mass = component.Mass;
      this.Dimensions = component.Dimensions;
      this.MuzzleVelocityBase = component.MuzzleVelocity;
      this.ImpactFXMagnitude = component.ImpactFXMagnitude;
      this.MaxRange = component.MaxRange;
      this.MaxRangeRandom = component.MaxRangeRandom;
      this.m_dieTimerMax = component.m_dieTimerMax;
      this.tracer = component.tracer;
      this.TracerRenderer = component.m_tracerRenderer;
      this.BulletRenderer = component.m_bulletRenderer;
      this.ExtraDisplay = component.ExtraDisplay;
      this.TracerLengthMultiplier = component.TracerLengthMultiplier;
      this.TracerWidthMultiplier = component.TracerWidthMultiplier;
      this.UsesTrails = component.UsesTrails;
      this.TrailStartColor = component.TrailStartColor;
    }

    [ContextMenu("AreaCalc")]
    public void AreaCalc()
    {
      float num1 = 3.141593f * Mathf.Pow(this.Dimensions.x * 0.5f, 2f);
      this.FrontArea = num1;
      float num2 = 0.5f * this.Mass * Mathf.Pow(this.MuzzleVelocityBase, 2f);
      this.KETotalForHit = num2;
      this.KEPerSquareMeterBase = (float) ((double) num2 / (double) num1 * 0.00999999977648258 / 140.0);
    }

    [Serializable]
    public class Submunition
    {
      public List<GameObject> Prefabs;
      public int NumToSpawn;
      public BallisticProjectile.Submunition.SubmunitionTrajectoryType Trajectory;
      public BallisticProjectile.Submunition.SubmunitionType Type;
      public BallisticProjectile.Submunition.SubmunitionSpawnLogic SpawnLogic;
      public Vector2 Speed = new Vector2();
      public float ConeLerp = 0.85f;

      public enum SubmunitionType
      {
        GameObject,
        Projectile,
        Rigidbody,
        StickyBomb,
        MeleeThrown,
        Demonade,
      }

      public enum SubmunitionTrajectoryType
      {
        Random,
        RicochetDir,
        Backwards,
        Forwards,
        ForwardsCone,
      }

      public enum SubmunitionSpawnLogic
      {
        Outside,
        Inside,
        On,
      }
    }
  }
}
