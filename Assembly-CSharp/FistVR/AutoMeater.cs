// Decompiled with JetBrains decompiler
// Type: FistVR.AutoMeater
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AutoMeater : MonoBehaviour, IFVRDamageable
  {
    private AutoMeater.AutoMeaterState m_state;
    public AIEntity E;
    public FVRPhysicalObject PO;
    public Rigidbody RB;
    public float HeightOffGround = 0.185f;
    public float MaxBlindFireTime = 1f;
    public GameObject[] DisableOnDeathState;
    public bool SetGunsToFast;
    public float IdleRandomAngleRange = 45f;
    public Vector2 IdleNewLookTime = new Vector2(5f, 10f);
    public float AlertCoolDownTime = 30f;
    [Header("--Vocalizations System--")]
    public AudioEvent Vocal_Alerted;
    public AudioEvent Vocal_StandDownToIdle;
    public AudioEvent Vocal_StandDownToAlert;
    public AudioEvent Vocal_Scream;
    public float MaxVocalSpeed = 1f;
    private float m_timeSinceVocal = 10f;
    [Header("--Target Priority System--")]
    public AITargetPrioritySystem Priority;
    [Header("--Fire Control System--")]
    public AutoMeater.AutoMeaterFireControl FireControl;
    [Header("--Motor System--")]
    public AutoMeater.AutoMeaterMotor Motor;
    [Header("--Flight System--")]
    public AutoMeater.AutoMeaterFlightSystem FlightSystem;
    public bool UsesFlightSystem;
    public Vector2 IdleNewDestinationTime = new Vector2(5f, 10f);
    public float MaxFlightSpeed = 4f;
    public float Radius = 0.5f;
    public LayerMask LM_Flight;
    private RaycastHit m_hit;
    public bool UsesBlades;
    public List<AutoMeaterBlade> Blades;
    public bool AttemptsToRam;
    private bool m_hasPriority;
    private bool m_hasFireControl;
    private bool m_hasMotor;
    [Header("--References--")]
    public Transform SideToSideTransform;
    public Transform UpDownTransform;
    public HingeJoint SideToSideHinge;
    public HingeJoint UpDownHinge;
    public GameObject TargetLaser;
    private bool m_usesUpDownTransform;
    private bool m_usesUpDownHinge;
    public LayerMask LM_FriendlyFireCheck;
    public AutoMeaterHitZone TerminalHitZone;
    [Header("Configparams")]
    public float sideMotorSpeed = 720f;
    public float updownMotorSpeed = 360f;
    public float updownRotClamp = 45f;
    [Header("DestructionShards")]
    public bool UsesDestructionShards;
    public List<Transform> ShardPoints;
    public List<GameObject> Shards;
    private bool m_hasSpawnedShards;
    private bool m_isTickingDownToRemove;
    private float m_removeTickDown = 5f;
    private Vector3 m_idleLookPoint;
    private float m_idleNewLookCountDown;
    private Vector3 m_idleDestination;
    private float m_idleDestinationCountDown;
    private float m_alertCountDown = 30f;
    private float m_dodgeTickDown = 0.2f;
    private Vector3 m_dodgeDir;
    private float m_minHeight = 0.5f;
    protected float AttachedRotationMultiplier = 60f;
    protected float AttachedPositionMultiplier = 9000f;
    protected float AttachedRotationFudge = 1000f;
    protected float AttachedPositionFudge = 1000f;
    private bool m_controlledMovement;
    public Vector3 m_targPos;
    public Quaternion m_targRot;
    private float m_flightRecoveryTime;

    private void Start()
    {
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      if (this.Priority != null)
      {
        this.m_hasPriority = true;
        this.Priority.Init(this.E, 5, 5f, 3f);
      }
      if (this.FireControl != null)
      {
        this.m_hasFireControl = true;
        this.FireControl.Init(this);
      }
      if (this.Motor != null)
      {
        this.m_hasMotor = true;
        this.Motor.Init(this, this.transform, this.SideToSideHinge, this.UpDownHinge, this.SideToSideTransform, this.UpDownTransform, this.sideMotorSpeed, this.updownMotorSpeed, this.updownRotClamp);
      }
      if ((UnityEngine.Object) this.UpDownTransform != (UnityEngine.Object) null)
        this.m_usesUpDownTransform = true;
      if ((UnityEngine.Object) this.UpDownHinge != (UnityEngine.Object) null)
        this.m_usesUpDownHinge = true;
      if (this.UsesFlightSystem)
      {
        this.FlightSystem.Init(this, this.RB);
        this.m_targPos = this.transform.position;
        this.m_targRot = this.transform.rotation;
      }
      this.SetState(AutoMeater.AutoMeaterState.Idle);
      if (this.UsesDestructionShards && this.E.IFFCode == 1 && (GM.CurrentPlayerBody.GetPlayerIFF() == 0 || GM.CurrentPlayerBody.GetPlayerIFF() == -3))
        this.E.IFFCode = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.SetGunsToFast)
      {
        for (int index = 0; index < this.FireControl.Firearms.Count; ++index)
          this.FireControl.Firearms[index].SetUseFastProjectile(true);
      }
      if (!((UnityEngine.Object) GM.TNH_Manager != (UnityEngine.Object) null))
        return;
      GM.TNH_Manager.AddToMiscEnemies(this.gameObject);
    }

    public void KillMe()
    {
      if (!((UnityEngine.Object) this.TerminalHitZone != (UnityEngine.Object) null))
        return;
      this.TerminalHitZone.BlowUp();
    }

    public void TickDownToClear(float f)
    {
      this.m_isTickingDownToRemove = true;
      this.m_removeTickDown = f;
    }

    public void Vocalize(AudioEvent e, float rangeLimit)
    {
      if ((double) this.m_timeSinceVocal < (double) this.MaxVocalSpeed || e.Clips.Count <= 0)
        return;
      this.m_timeSinceVocal = 0.0f;
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
      float delay = num / 343f;
      if ((double) num >= (double) rangeLimit)
        return;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.NPCBarks, e, this.transform.position, delay);
    }

    public void SetUseFastProjectile(bool b)
    {
      if (this.FireControl == null)
        return;
      this.FireControl.SetUseFastProjectile(b);
    }

    public void DamageEvent(Vector3 p, float value, AutoMeater.AMHitZoneType type)
    {
      if (this.m_state == AutoMeater.AutoMeaterState.Dead)
        return;
      this.EventReceive(new AIEvent(p, AIEvent.AIEType.Damage, value));
      if (type != AutoMeater.AMHitZoneType.Motor)
        return;
      this.Motor.DisruptSystem(1f - value);
    }

    public void DestroyComponent(
      AutoMeater.AMHitZoneType Type,
      GameObject SpawnThis,
      GameObject AndThis,
      Transform SpawnThisHere,
      bool DestroysTurret)
    {
      UnityEngine.Object.Instantiate<GameObject>(SpawnThis, SpawnThisHere.position, SpawnThisHere.rotation);
      if ((UnityEngine.Object) AndThis != (UnityEngine.Object) null)
        UnityEngine.Object.Instantiate<GameObject>(AndThis, SpawnThisHere.position, SpawnThisHere.rotation);
      if (this.m_state == AutoMeater.AutoMeaterState.Dead)
        return;
      if (DestroysTurret)
        Type = AutoMeater.AMHitZoneType.Generator;
      if (DestroysTurret && this.UsesDestructionShards && !this.m_hasSpawnedShards)
      {
        this.m_hasSpawnedShards = true;
        for (int index = 0; index < this.ShardPoints.Count; ++index)
          UnityEngine.Object.Instantiate<GameObject>(this.Shards[index], this.ShardPoints[index].position, this.ShardPoints[index].rotation);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      switch (Type)
      {
        case AutoMeater.AMHitZoneType.FireControl:
          this.FireControl.DestroySystem();
          if (!((UnityEngine.Object) this.TargetLaser != (UnityEngine.Object) null))
            break;
          this.TargetLaser.SetActive(false);
          break;
        case AutoMeater.AMHitZoneType.Magazine:
          this.SetState(AutoMeater.AutoMeaterState.Dead);
          if (!((UnityEngine.Object) this.TargetLaser != (UnityEngine.Object) null))
            break;
          this.TargetLaser.SetActive(false);
          break;
        case AutoMeater.AMHitZoneType.Motor:
          this.Motor.DestroySystem();
          break;
        case AutoMeater.AMHitZoneType.TargetPriority:
          this.Priority.DestroySystem();
          break;
        case AutoMeater.AMHitZoneType.Generator:
          this.SetState(AutoMeater.AutoMeaterState.Dead);
          this.FireControl.DestroySystem();
          this.Motor.DestroySystem();
          this.Priority.DestroySystem();
          this.m_isTickingDownToRemove = true;
          if (!((UnityEngine.Object) this.TargetLaser != (UnityEngine.Object) null))
            break;
          this.TargetLaser.SetActive(false);
          break;
      }
    }

    public void EventReceive(AIEvent e)
    {
      if (this.m_state == AutoMeater.AutoMeaterState.Static || e.IsEntity && e.Entity.IFFCode == this.E.IFFCode)
        return;
      if (e.Type == AIEvent.AIEType.Damage)
      {
        if (this.m_state != AutoMeater.AutoMeaterState.Idle)
          return;
        this.SetState(AutoMeater.AutoMeaterState.Alert);
      }
      else
      {
        if (e.Type != AIEvent.AIEType.Visual || !this.m_hasPriority)
          return;
        this.Priority.ProcessEvent(e);
        if (this.m_state != AutoMeater.AutoMeaterState.Idle)
          return;
        this.Vocalize(this.Vocal_Alerted, 40f);
        this.SetState(AutoMeater.AutoMeaterState.Alert);
      }
    }

    private void SetState(AutoMeater.AutoMeaterState s)
    {
      if (this.m_state == s || this.m_state == AutoMeater.AutoMeaterState.Dead)
        return;
      this.m_state = s;
      switch (this.m_state)
      {
        case AutoMeater.AutoMeaterState.Idle:
          this.GenerateNewIdleLookPoint();
          this.m_idleNewLookCountDown = UnityEngine.Random.Range(this.IdleNewLookTime.x, this.IdleNewLookTime.y);
          if (!this.UsesFlightSystem)
            break;
          this.GenerateNewIdleDestination();
          this.m_idleDestinationCountDown = UnityEngine.Random.Range(this.IdleNewDestinationTime.x, this.IdleNewDestinationTime.y);
          this.m_targPos = this.transform.position;
          this.m_targRot = this.transform.rotation;
          break;
        case AutoMeater.AutoMeaterState.Alert:
          this.m_alertCountDown = this.AlertCoolDownTime;
          if (!this.UsesFlightSystem)
            break;
          this.m_targPos = this.transform.position;
          this.m_targRot = this.transform.rotation;
          break;
        case AutoMeater.AutoMeaterState.Engaging:
          this.m_dodgeDir = UnityEngine.Random.onUnitSphere;
          this.m_minHeight = UnityEngine.Random.Range(0.8f, 2f);
          this.m_dodgeTickDown = UnityEngine.Random.Range(0.1f, 0.4f);
          break;
        case AutoMeater.AutoMeaterState.Dead:
          this.Vocalize(this.Vocal_Scream, 40f);
          this.m_hasPriority = false;
          this.m_hasMotor = false;
          this.m_hasFireControl = false;
          if ((UnityEngine.Object) this.RB != (UnityEngine.Object) null)
            this.RB.useGravity = true;
          for (int index = 0; index < this.DisableOnDeathState.Length; ++index)
          {
            if ((UnityEngine.Object) this.DisableOnDeathState[index] != (UnityEngine.Object) null)
              this.DisableOnDeathState[index].SetActive(false);
          }
          break;
      }
    }

    private void Update()
    {
      if ((double) this.m_timeSinceVocal < 10.0)
        this.m_timeSinceVocal += Time.deltaTime;
      switch (this.m_state)
      {
        case AutoMeater.AutoMeaterState.Static:
          this.UpdateState_Static();
          break;
        case AutoMeater.AutoMeaterState.Idle:
          this.UpdateState_Idle();
          break;
        case AutoMeater.AutoMeaterState.Alert:
          this.UpdateState_Alert();
          break;
        case AutoMeater.AutoMeaterState.Engaging:
          this.UpdateState_Engaging();
          break;
      }
      if (!this.m_isTickingDownToRemove)
        return;
      this.m_removeTickDown -= Time.deltaTime;
      if ((double) this.m_removeTickDown >= 0.0)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void FixedUpdate()
    {
      if (!this.UsesFlightSystem)
        return;
      this.UpdateFlight();
    }

    private void UpdateState_Static()
    {
    }

    private void GenerateNewIdleLookPoint()
    {
      Vector3 vector3 = this.transform.forward * 5f;
      this.m_idleLookPoint = this.transform.position + Quaternion.AngleAxis(UnityEngine.Random.Range(-this.IdleRandomAngleRange, this.IdleRandomAngleRange), Vector3.up) * vector3;
    }

    private void GenerateNewIdleDestination()
    {
      Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
      if (Physics.SphereCast(this.transform.position, this.Radius, onUnitSphere, out this.m_hit, 10f, (int) this.LM_Flight, QueryTriggerInteraction.Ignore))
        this.m_idleDestination = this.transform.position + onUnitSphere * this.m_hit.distance;
      else
        this.m_idleDestination = this.transform.position + onUnitSphere * 10f;
    }

    private void UpdateState_Idle()
    {
      if (this.m_hasMotor && this.m_hasPriority)
      {
        this.Priority.Compute();
        this.Motor.SetMaxSpeedMagnitude(0.2f);
        this.Motor.RotateToFacePoint(this.m_idleLookPoint);
      }
      if (this.m_hasFireControl)
        this.FireControl.Tick(Time.deltaTime, false, 180f, 180f, 400f);
      if ((double) this.m_idleNewLookCountDown > 0.0)
      {
        this.m_idleNewLookCountDown -= Time.deltaTime;
      }
      else
      {
        this.GenerateNewIdleLookPoint();
        this.m_idleNewLookCountDown = UnityEngine.Random.Range(this.IdleNewLookTime.x, this.IdleNewLookTime.y);
      }
      if (!this.UsesFlightSystem)
        return;
      if ((double) this.m_idleDestinationCountDown > 0.0)
      {
        this.m_idleDestinationCountDown -= Time.deltaTime;
      }
      else
      {
        this.GenerateNewIdleDestination();
        this.m_idleDestinationCountDown = UnityEngine.Random.Range(this.IdleNewDestinationTime.x, this.IdleNewDestinationTime.y);
      }
      this.FlightSystem.OrientToFacePoint(this.m_idleDestination, 0.2f);
      this.FlightSystem.FlyTowardsPoint(this.m_idleDestination, 0.2f, 1f);
    }

    private void UpdateState_Alert()
    {
      if (this.m_hasMotor && this.m_hasPriority)
      {
        this.Priority.Compute();
        this.Motor.SetMaxSpeedMagnitude(0.5f);
        this.Motor.RotateToFacePoint(this.Priority.GetTargetPoint());
      }
      if (this.m_hasFireControl)
        this.FireControl.Tick(Time.deltaTime, false, 180f, 180f, 400f);
      if (this.m_hasPriority && this.Priority.HasFreshTarget() && (double) this.Priority.GetTimeSinceTopTargetSeen() <= (double) this.MaxBlindFireTime)
      {
        this.Vocalize(this.Vocal_Alerted, 40f);
        this.SetState(AutoMeater.AutoMeaterState.Engaging);
      }
      if ((double) this.m_alertCountDown > 0.0)
      {
        this.m_alertCountDown -= Time.deltaTime;
      }
      else
      {
        this.Vocalize(this.Vocal_StandDownToIdle, 20f);
        this.SetState(AutoMeater.AutoMeaterState.Idle);
      }
      if (!this.UsesFlightSystem)
        return;
      Vector3 p = !this.m_hasPriority ? this.transform.position + Vector3.Slerp(this.transform.forward * 5f, UnityEngine.Random.onUnitSphere * 5f, 0.3f) : this.Priority.GetTargetPoint();
      this.FlightSystem.OrientToFacePoint(p, 1f);
      this.FlightSystem.FlyTowardsPoint(p, 0.5f, 1f);
    }

    private void UpdateState_Engaging()
    {
      if (this.m_hasMotor && this.m_hasPriority)
      {
        this.Priority.Compute();
        this.Motor.SetMaxSpeedMagnitude(1f);
        this.Motor.RotateToFacePoint(this.Priority.GetTargetPoint());
      }
      if (this.m_hasFireControl)
      {
        float angleToTargetHoriz = 180f;
        float angleToTargetVertical = 180f;
        float distToTarget = 400f;
        if (this.m_hasPriority)
        {
          if (this.m_usesUpDownTransform)
          {
            angleToTargetHoriz = this.Priority.GetAngleToHorizontal(this.SideToSideTransform);
            angleToTargetVertical = this.Priority.GetAngleToVertical(this.FireControl.Firearms[0].Muzzle);
          }
          else
          {
            angleToTargetHoriz = this.Priority.GetAngleToHorizontal(this.transform);
            angleToTargetVertical = this.Priority.GetAngleToVertical(this.transform);
          }
          distToTarget = this.Priority.GetDistanceToTarget(this.transform);
        }
        this.FireControl.Tick(Time.deltaTime, true, angleToTargetHoriz, angleToTargetVertical, distToTarget);
      }
      if (this.m_hasPriority && (!this.Priority.HasFreshTarget() || (double) this.Priority.GetTimeSinceTopTargetSeen() > (double) this.MaxBlindFireTime))
      {
        this.Vocalize(this.Vocal_StandDownToAlert, 20f);
        this.SetState(AutoMeater.AutoMeaterState.Alert);
      }
      if (!this.UsesFlightSystem)
        return;
      Vector3 p = !this.m_hasPriority ? this.transform.position + Vector3.Slerp(this.transform.forward * 5f, UnityEngine.Random.onUnitSphere * 5f, 0.3f) : this.Priority.GetTargetPoint();
      this.FlightSystem.OrientToFacePoint(p, 1f);
      float distanceThreshold1 = 2.5f;
      if (this.AttemptsToRam)
        distanceThreshold1 = 0.3f;
      this.FlightSystem.FlyTowardsPoint(p, 0.5f, distanceThreshold1);
      if (!this.m_hasPriority)
        return;
      if ((double) this.m_dodgeTickDown > 0.0)
      {
        this.m_dodgeTickDown -= Time.deltaTime;
      }
      else
      {
        this.m_dodgeDir = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 2f);
        this.m_minHeight = UnityEngine.Random.Range(0.8f, 2f);
        this.m_dodgeTickDown = UnityEngine.Random.Range(0.1f, 0.4f);
      }
      if (this.Priority.RecentEvents.Count <= 0 || !this.Priority.RecentEvents[0].IsEntity)
        return;
      float num1 = Vector3.Angle(this.transform.position - p, this.Priority.RecentEvents[0].Entity.GetThreatFacing());
      float num2 = 35f;
      if (this.AttemptsToRam)
        num2 = 18f;
      if ((double) num1 >= (double) num2)
        return;
      float distanceThreshold2 = 2.5f;
      if (this.AttemptsToRam)
        distanceThreshold2 = 2.5f;
      this.FlightSystem.FlyTowardsPoint(p + this.m_dodgeDir, 1f, distanceThreshold2);
    }

    private void UpdateFlight()
    {
      if (this.m_state == AutoMeater.AutoMeaterState.Dead || this.m_state == AutoMeater.AutoMeaterState.Static)
      {
        if (!this.UsesBlades)
          return;
        for (int index = 0; index < this.Blades.Count; ++index)
          this.Blades[index].ShutDown();
      }
      else
      {
        if (this.m_controlledMovement)
        {
          this.RB.useGravity = false;
          if ((UnityEngine.Object) this.PO != (UnityEngine.Object) null)
            this.PO.DistantGrabbable = false;
          if (this.UsesBlades)
          {
            for (int index = 0; index < this.Blades.Count; ++index)
              this.Blades[index].Reactivate();
          }
        }
        else
        {
          this.RB.useGravity = true;
          if ((UnityEngine.Object) this.PO != (UnityEngine.Object) null)
            this.PO.DistantGrabbable = true;
          if (this.UsesBlades)
          {
            for (int index = 0; index < this.Blades.Count; ++index)
              this.Blades[index].ShutDown();
          }
        }
        if (!this.m_controlledMovement)
        {
          if ((double) this.m_flightRecoveryTime > 0.0)
          {
            this.m_flightRecoveryTime -= Time.deltaTime;
          }
          else
          {
            this.m_targPos = this.transform.position;
            this.m_targRot = this.transform.rotation;
            this.m_controlledMovement = true;
          }
        }
        else
        {
          Vector3 position = this.transform.position;
          Quaternion rotation = this.transform.rotation;
          Vector3 vector3 = this.m_targPos - position;
          Quaternion quaternion = this.m_targRot * Quaternion.Inverse(rotation);
          float deltaTime = Time.deltaTime;
          float angle;
          Vector3 axis;
          quaternion.ToAngleAxis(out angle, out axis);
          if ((double) angle > 180.0)
            angle -= 360f;
          if ((double) angle != 0.0)
            this.RB.angularVelocity = Vector3.MoveTowards(this.RB.angularVelocity, deltaTime * angle * axis * this.AttachedRotationMultiplier, this.AttachedRotationFudge * Time.fixedDeltaTime);
          this.RB.velocity = Vector3.MoveTowards(this.RB.velocity, vector3 * this.AttachedPositionMultiplier * deltaTime, this.AttachedPositionFudge * deltaTime);
        }
      }
    }

    private void OnCollisionEnter(Collision col)
    {
      if (!this.UsesFlightSystem || !((UnityEngine.Object) col.collider.attachedRigidbody != (UnityEngine.Object) null))
        return;
      float magnitude = col.relativeVelocity.magnitude;
      if ((double) magnitude <= 5.0)
        return;
      this.m_controlledMovement = false;
      this.m_flightRecoveryTime = Mathf.Max(this.m_flightRecoveryTime, Mathf.Clamp(magnitude * 0.25f, 0.0f, 2.5f));
    }

    public void Damage(FistVR.Damage d)
    {
      this.m_controlledMovement = false;
      this.m_flightRecoveryTime = Mathf.Max(this.m_flightRecoveryTime, UnityEngine.Random.Range(0.2f, 1f));
      if ((double) d.Dam_EMP <= 0.0)
        return;
      this.Motor.DisruptSystem(d.Dam_EMP);
      this.FireControl.DisruptSystem(d.Dam_EMP);
    }

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    public enum AutoMeaterState
    {
      Static,
      Idle,
      Alert,
      Engaging,
      Dead,
    }

    public enum AMHitZoneType
    {
      FireControl,
      Magazine,
      Motor,
      TargetPriority,
      Generator,
    }

    [Serializable]
    public class AutoMeaterFireControl
    {
      private AutoMeater M;
      public List<AutoMeater.AutoMeaterFirearm> Firearms;
      private bool m_isDestroyed;
      private float m_disruptedTick;

      public void Init(AutoMeater m)
      {
        this.M = m;
        for (int index = 0; index < this.Firearms.Count; ++index)
          this.Firearms[index].Init(this.M, this);
      }

      public void DisruptSystem(float f) => this.m_disruptedTick = Mathf.Max(this.m_disruptedTick, f);

      public void SetUseFastProjectile(bool b)
      {
        for (int index = 0; index < this.Firearms.Count; ++index)
          this.Firearms[index].SetUseFastProjectile(b);
      }

      public void Tick(
        float t,
        bool fireAtWill,
        float angleToTargetHoriz,
        float angleToTargetVertical,
        float distToTarget)
      {
        if ((double) this.m_disruptedTick > 0.0)
          this.m_disruptedTick -= Time.deltaTime;
        for (int index = 0; index < this.Firearms.Count; ++index)
        {
          if (!this.m_isDestroyed)
          {
            if ((double) this.m_disruptedTick > 0.0)
            {
              int num = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.9f));
              this.Firearms[index].SetFireAtWill(Convert.ToBoolean(num), UnityEngine.Random.Range(0.0f, 100f));
            }
            if ((double) this.Firearms[index].FiringAngleLimit > (double) angleToTargetHoriz && (double) this.Firearms[index].FiringAngleLimitVertical > (double) angleToTargetVertical && (double) this.Firearms[index].RangeLimit >= (double) distToTarget)
              this.Firearms[index].SetFireAtWill(fireAtWill, distToTarget);
            else
              this.Firearms[index].SetFireAtWill(false, distToTarget);
          }
          this.Firearms[index].Tick(t);
        }
      }

      public void DestroySystem()
      {
        for (int index = 0; index < this.Firearms.Count; ++index)
          this.Firearms[index].SetFireAtWill(false, 400f);
        this.m_isDestroyed = true;
      }
    }

    [Serializable]
    public class AutoMeaterMotor
    {
      private AutoMeater M;
      private Transform m_base;
      private Transform m_sideToSideTransform;
      private Transform m_upAndDownMotor;
      private HingeJoint m_hingeJoint;
      private HingeJoint m_upDownJoint;
      private float m_sideMotorMaxSpeed;
      private float m_upDownMotorMaxSpeed;
      private float m_maxVerticalRot;
      private float m_currentSpeedMagnitude;
      private bool m_isDestroyed;
      private float m_disruptedTick;
      private bool usesUpDownHinger;

      public void Init(
        AutoMeater m,
        Transform baseTransform,
        HingeJoint sideToSideHinge,
        HingeJoint upDownHinge,
        Transform sideMotor,
        Transform upDownMotor,
        float sideMotorMaxSpeed,
        float upDownMotorMaxSpeed,
        float maxVerticalRot)
      {
        this.M = m;
        this.m_base = baseTransform;
        this.m_upAndDownMotor = upDownMotor;
        this.m_sideToSideTransform = sideMotor;
        this.m_hingeJoint = sideToSideHinge;
        this.m_upDownJoint = upDownHinge;
        if ((UnityEngine.Object) this.m_upDownJoint != (UnityEngine.Object) null)
          this.usesUpDownHinger = true;
        this.m_sideMotorMaxSpeed = sideMotorMaxSpeed;
        this.m_upDownMotorMaxSpeed = upDownMotorMaxSpeed;
        this.m_maxVerticalRot = maxVerticalRot;
      }

      public void DisruptSystem(float f) => this.m_disruptedTick = Mathf.Max(this.m_disruptedTick, f);

      public void DestroySystem() => this.m_isDestroyed = true;

      public void SetMaxSpeedMagnitude(float f) => this.m_currentSpeedMagnitude = Mathf.Clamp(f, 0.0f, 1f);

      public void RotateToFacePoint(Vector3 p)
      {
        if (this.m_isDestroyed)
          return;
        if ((double) this.m_disruptedTick > 0.0)
          this.m_disruptedTick -= Time.deltaTime;
        Vector3 vector3_1 = !this.M.m_usesUpDownTransform ? p - this.M.transform.position : p - this.m_upAndDownMotor.position;
        Debug.DrawLine(this.m_sideToSideTransform.position, p, Color.yellow);
        if ((double) this.m_disruptedTick > 0.0)
          vector3_1 = Vector3.Slerp(vector3_1, UnityEngine.Random.onUnitSphere, 0.9f);
        Vector3 rhs = Vector3.ProjectOnPlane(vector3_1, this.m_base.up);
        Vector3 forward1 = this.m_base.forward;
        float num1 = Mathf.Atan2(Vector3.Dot(this.m_base.up, Vector3.Cross(forward1, rhs)), Vector3.Dot(forward1, rhs)) * 57.29578f;
        JointSpring spring1 = this.m_hingeJoint.spring;
        spring1.targetPosition = num1;
        this.m_hingeJoint.spring = spring1;
        this.m_sideToSideTransform.rotation = Quaternion.LookRotation(this.m_hingeJoint.transform.forward, this.m_base.up);
        Debug.DrawLine(this.m_sideToSideTransform.position, this.m_sideToSideTransform.position + rhs, Color.cyan);
        if (!this.M.m_usesUpDownTransform)
          return;
        Vector3 target = Vector3.RotateTowards(this.m_sideToSideTransform.forward, Vector3.ProjectOnPlane(vector3_1, this.m_upAndDownMotor.right), this.m_maxVerticalRot * ((float) Math.PI / 180f), 1f);
        Vector3 vector3_2 = !this.usesUpDownHinger ? Vector3.RotateTowards(this.m_upAndDownMotor.forward, target, (float) Math.PI / 180f * this.m_upDownMotorMaxSpeed * this.m_currentSpeedMagnitude * Time.deltaTime, 1f) : Vector3.RotateTowards(this.m_sideToSideTransform.forward, target, (float) Math.PI / 180f * this.m_upDownMotorMaxSpeed * this.m_currentSpeedMagnitude * Time.deltaTime, 1f);
        Debug.DrawLine(this.m_sideToSideTransform.position, this.m_sideToSideTransform.position + vector3_2, Color.green);
        if (this.usesUpDownHinger)
        {
          Vector3 forward2 = this.m_sideToSideTransform.forward;
          float num2 = Mathf.Atan2(Vector3.Dot(this.m_sideToSideTransform.right, Vector3.Cross(forward2, vector3_2)), Vector3.Dot(forward2, vector3_2)) * 57.29578f;
          JointSpring spring2 = this.m_upDownJoint.spring;
          spring1.targetPosition = num2;
          this.m_upDownJoint.spring = spring1;
          this.m_upAndDownMotor.rotation = Quaternion.LookRotation(this.m_upDownJoint.transform.forward, this.m_base.up);
        }
        else
          this.m_upAndDownMotor.rotation = Quaternion.LookRotation(vector3_2, this.m_base.up);
      }
    }

    [Serializable]
    public class AutoMeaterFirearm
    {
      public AutoMeaterFirearmSoundProfile GunShotProfile;
      private Dictionary<FVRSoundEnvironment, AutoMeaterFirearmSoundProfile.GunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, AutoMeaterFirearmSoundProfile.GunShotSet>();
      private bool m_hasProfile;
      private AutoMeater M;
      private AutoMeater.AutoMeaterFireControl FC;
      public AutoMeater.AutoMeaterFirearm.FiringState State;
      private float m_refireTick;
      private float m_cooldownTick;
      private bool m_fireAtWill;
      private float m_distToTarget = 1f;
      public Transform Muzzle;
      public GameObject Projectile;
      public int NumProjectiles = 1;
      public Vector2 RefireCycle;
      public Vector2 BurstCooldownRange;
      public int BurstCountMin = 1;
      public int BurstCountMax = 1;
      private int m_burstsLeftToFire = 1;
      public float FiringAngleLimit = 5f;
      public float FiringAngleLimitVertical = 20f;
      public float AccuracyRange = 1f;
      public float RangeLimit = 150f;
      private bool m_usesFastProjectile;
      public bool UsesMuzzleFire;
      public ParticleSystem[] PSystemsMuzzle;
      public int MuzzlePAmount;
      public bool DoesFlashOnFire;
      public bool ExplodesOnEmpty;
      [Header("MagazineSystem")]
      public int Ammo = 1000;
      public int StartingAmmo = 1000;
      public bool UsesRefillMag;
      public bool HasMag = true;
      public GameObject EmptyMagazinePrefab;
      public Transform MagazineEjectPos;
      public List<GameObject> MagazineProxy;
      public AudioEvent AudEvent_Eject;
      [Header("FlameThrower Config")]
      public bool IsFlameThrower;
      public AudioEvent AudEvent_Ignite;
      public AudioEvent AudEvent_Extinguish;
      public AudioSource AudSource_FireLoop;
      private float m_hasBeenFiring;
      private bool m_hasFiredStartSound;
      private bool m_isFiring;
      public ParticleSystem FireParticles;
      public Vector2 FireWidthRange;
      public Vector2 SpeedRangeMin;
      public Vector2 SpeedRangeMax;
      public Vector2 SizeRangeMin;
      public Vector2 SizeRangeMax;

      public void SetUseFastProjectile(bool b) => this.m_usesFastProjectile = b;

      public void Init(AutoMeater m, AutoMeater.AutoMeaterFireControl f)
      {
        this.M = m;
        this.FC = f;
        if ((UnityEngine.Object) this.GunShotProfile != (UnityEngine.Object) null)
          this.m_hasProfile = true;
        if (!this.m_hasProfile)
          return;
        this.PrimeDics();
      }

      public void Load()
      {
        this.HasMag = true;
        this.Ammo = this.StartingAmmo;
        if (this.MagazineProxy == null)
          return;
        for (int index = 0; index < this.MagazineProxy.Count; ++index)
          this.MagazineProxy[index].SetActive(true);
      }

      public void EjectMag()
      {
        if (!this.UsesRefillMag || !this.HasMag || (UnityEngine.Object) this.EmptyMagazinePrefab == (UnityEngine.Object) null)
          return;
        if (this.MagazineProxy != null)
        {
          for (int index = 0; index < this.MagazineProxy.Count; ++index)
            this.MagazineProxy[index].SetActive(false);
        }
        this.Ammo = 0;
        this.HasMag = false;
        Rigidbody component = UnityEngine.Object.Instantiate<GameObject>(this.EmptyMagazinePrefab, this.MagazineEjectPos.position, this.MagazineEjectPos.rotation).GetComponent<Rigidbody>();
        component.velocity = this.M.transform.up + UnityEngine.Random.onUnitSphere * 0.2f * 2.5f;
        component.angularVelocity = UnityEngine.Random.onUnitSphere * 3f;
        SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Eject, this.MagazineEjectPos.position);
      }

      public void SetFireAtWill(bool b, float d)
      {
        this.m_fireAtWill = b;
        this.m_distToTarget = d;
        if (this.m_fireAtWill)
          return;
        this.m_cooldownTick = this.BurstCooldownRange.x * 0.5f;
        this.State = AutoMeater.AutoMeaterFirearm.FiringState.Cooldown;
      }

      private void UpdateFlameThrower(float t)
      {
        if (this.m_fireAtWill && (double) this.m_distToTarget <= (double) this.RangeLimit)
        {
          this.m_isFiring = true;
          if ((double) this.m_hasBeenFiring < 2.0)
            this.m_hasBeenFiring += Time.deltaTime;
          if (!this.m_hasFiredStartSound)
          {
            this.m_hasFiredStartSound = true;
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Ignite, this.Muzzle.position);
          }
          this.AudSource_FireLoop.volume = Mathf.Clamp(this.m_hasBeenFiring * 2f, 0.0f, 0.4f);
          this.AudSource_FireLoop.pitch = Mathf.Lerp(0.5f, 1.5f, this.m_distToTarget / this.RangeLimit);
          if (this.AudSource_FireLoop.isPlaying)
            return;
          this.AudSource_FireLoop.Play();
        }
        else
        {
          this.m_hasFiredStartSound = false;
          this.m_hasBeenFiring = 0.0f;
          this.StopFiring();
        }
      }

      private void UpdateFire()
      {
        ParticleSystem.EmissionModule emission = this.FireParticles.emission;
        ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
        if (this.m_isFiring)
        {
          rateOverTime.mode = ParticleSystemCurveMode.Constant;
          rateOverTime.constantMax = 40f;
          rateOverTime.constantMin = 40f;
          float num1 = 1f - Mathf.Clamp(this.m_distToTarget / this.RangeLimit, 0.0f, 1f);
          float num2 = num1 * num1;
          float t = num2 * num2;
          ParticleSystem.MainModule main = this.FireParticles.main;
          ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
          startSpeed.mode = ParticleSystemCurveMode.TwoConstants;
          startSpeed.constantMax = Mathf.Lerp(this.SpeedRangeMax.x, this.SpeedRangeMax.y, t);
          startSpeed.constantMin = Mathf.Lerp(this.SpeedRangeMin.x, this.SpeedRangeMin.y, t);
          main.startSpeed = startSpeed;
          ParticleSystem.MinMaxCurve startSize = main.startSize;
          startSize.mode = ParticleSystemCurveMode.TwoConstants;
          startSize.constantMax = Mathf.Lerp(this.SizeRangeMax.x, this.SizeRangeMax.y, t);
          startSize.constantMin = Mathf.Lerp(this.SizeRangeMin.x, this.SizeRangeMin.y, t);
          main.startSize = startSize;
          this.FireParticles.shape.angle = Mathf.Lerp(this.FireWidthRange.x, this.FireWidthRange.y, t);
        }
        else
        {
          rateOverTime.mode = ParticleSystemCurveMode.Constant;
          rateOverTime.constantMax = 0.0f;
          rateOverTime.constantMin = 0.0f;
        }
        emission.rateOverTime = rateOverTime;
      }

      private void StopFiring()
      {
        if (this.m_isFiring)
        {
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Extinguish, this.Muzzle.position);
          this.AudSource_FireLoop.Stop();
          this.AudSource_FireLoop.volume = 0.0f;
        }
        this.m_isFiring = false;
        this.m_hasFiredStartSound = false;
      }

      public void Tick(float t)
      {
        if (this.IsFlameThrower)
        {
          this.UpdateFlameThrower(t);
          this.UpdateFire();
        }
        else
        {
          switch (this.State)
          {
            case AutoMeater.AutoMeaterFirearm.FiringState.FiringBurst:
              if ((double) this.m_refireTick > 0.0)
              {
                this.m_refireTick -= Time.deltaTime;
                break;
              }
              if (this.m_burstsLeftToFire > 0)
              {
                if (this.Ammo < 1 && this.UsesRefillMag && this.HasMag)
                  this.EjectMag();
                if (!this.m_fireAtWill || this.Ammo <= 0 || !this.HasMag && this.UsesRefillMag)
                  break;
                this.FireShot();
                --this.m_burstsLeftToFire;
                this.m_refireTick = UnityEngine.Random.Range(this.RefireCycle.x, this.RefireCycle.y);
                --this.Ammo;
                if (this.Ammo > 0 || !this.ExplodesOnEmpty)
                  break;
                this.M.KillMe();
                break;
              }
              this.State = AutoMeater.AutoMeaterFirearm.FiringState.Cooldown;
              this.m_cooldownTick = UnityEngine.Random.Range(this.BurstCooldownRange.x, this.BurstCooldownRange.y);
              break;
            case AutoMeater.AutoMeaterFirearm.FiringState.Cooldown:
              if ((double) this.m_cooldownTick > 0.0)
              {
                this.m_cooldownTick -= t;
                break;
              }
              this.State = AutoMeater.AutoMeaterFirearm.FiringState.FiringBurst;
              this.m_burstsLeftToFire = UnityEngine.Random.Range(this.BurstCountMin, this.BurstCountMax + 1);
              this.m_refireTick = 0.0f;
              break;
          }
        }
      }

      private void FireShot()
      {
        if (this.m_hasProfile)
          this.PlayShotEvent(this.Muzzle.position);
        GameObject projectile = this.Projectile;
        for (int index = 0; index < this.NumProjectiles; ++index)
        {
          this.Muzzle.localEulerAngles = new Vector3(UnityEngine.Random.Range(-this.AccuracyRange, this.AccuracyRange), UnityEngine.Random.Range(-this.AccuracyRange, this.AccuracyRange), 0.0f);
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(projectile, this.Muzzle.position, this.Muzzle.rotation);
          if (!this.m_usesFastProjectile)
            gameObject.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = 0.2f;
          gameObject.GetComponent<BallisticProjectile>().Fire(this.Muzzle.forward, (FVRFireArm) null);
          gameObject.GetComponent<BallisticProjectile>().Source_IFF = this.M.E.IFFCode;
        }
        if (!this.UsesMuzzleFire)
          return;
        for (int index = 0; index < this.PSystemsMuzzle.Length; ++index)
          this.PSystemsMuzzle[index].Emit(this.MuzzlePAmount);
        if (!this.DoesFlashOnFire)
          return;
        FXM.InitiateMuzzleFlashLowPriority(this.Muzzle.position, this.Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      }

      private void PlayShotEvent(Vector3 source)
      {
        float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
        float delay = num / 343f;
        AutoMeaterFirearmSoundProfile.GunShotSet shotSet = this.GetShotSet(SM.GetSoundEnvironment(this.M.transform.position));
        if ((double) num < 20.0)
          SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotNear, shotSet.ShotSet_Near, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
        else if ((double) num < 100.0)
        {
          float y = Mathf.Lerp(0.4f, 0.2f, (float) (((double) num - 20.0) / 80.0));
          Vector2 vol = new Vector2(y * 0.95f, y);
          SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotFarDistant, shotSet.ShotSet_Far, source, vol, shotSet.ShotSet_Distant.PitchRange, delay);
        }
        else
          SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotFarDistant, shotSet.ShotSet_Distant, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
      }

      private AutoMeaterFirearmSoundProfile.GunShotSet GetShotSet(
        FVRSoundEnvironment e)
      {
        return this.m_shotDic[e];
      }

      private void PrimeDics()
      {
        for (int index1 = 0; index1 < this.GunShotProfile.ShotSets.Count; ++index1)
        {
          for (int index2 = 0; index2 < this.GunShotProfile.ShotSets[index1].EnvironmentsUsed.Count; ++index2)
            this.m_shotDic.Add(this.GunShotProfile.ShotSets[index1].EnvironmentsUsed[index2], this.GunShotProfile.ShotSets[index1]);
        }
      }

      public enum FiringState
      {
        FiringBurst,
        Cooldown,
      }
    }

    [Serializable]
    public class AutoMeaterFlightSystem
    {
      private AutoMeater M;
      private Rigidbody RB;
      private bool m_isDestroyed;

      public void Init(AutoMeater m, Rigidbody rb)
      {
        this.M = m;
        this.RB = rb;
      }

      public void OrientToFacePoint(Vector3 p, float speedMultiplier)
      {
        if (this.m_isDestroyed)
          return;
        Quaternion to = Quaternion.LookRotation(p - this.M.transform.position, Vector3.Cross(this.M.transform.forward, Vector3.up));
        if (this.M.AttemptsToRam)
          to = Quaternion.LookRotation(p - this.M.transform.position, Vector3.up);
        this.M.m_targRot = Quaternion.RotateTowards(this.M.m_targRot, to, 180f * speedMultiplier * Time.deltaTime);
      }

      public void FlyTowardsPoint(Vector3 p, float speedMultiplier, float distanceThreshold)
      {
        if (this.m_isDestroyed)
          return;
        bool flag = false;
        if (Physics.Raycast(this.M.transform.position, -Vector3.up, out this.M.m_hit, this.M.m_minHeight, (int) this.M.LM_Flight, QueryTriggerInteraction.Ignore))
        {
          flag = true;
          p.y = this.M.m_hit.point.y + this.M.m_minHeight;
        }
        if ((double) Vector3.Distance(p, this.M.m_targPos) > (double) distanceThreshold)
        {
          this.M.m_targPos = Vector3.MoveTowards(this.M.m_targPos, p, this.M.MaxFlightSpeed * Time.deltaTime * speedMultiplier);
        }
        else
        {
          Vector3 vector3 = (p - this.M.transform.position).normalized * distanceThreshold;
          Vector3 target = p - vector3;
          target.y = p.y;
          this.M.m_targPos = Vector3.MoveTowards(this.M.m_targPos, target, this.M.MaxFlightSpeed * Time.deltaTime * speedMultiplier);
        }
      }
    }
  }
}
