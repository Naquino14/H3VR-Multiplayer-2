// Decompiled with JetBrains decompiler
// Type: FistVR.MeatCrab
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class MeatCrab : FVRPhysicalObject, IFVRDamageable
  {
    [Header("Refs")]
    public AIEntity E;
    public AITargetPrioritySystem Priority;
    private bool m_hasPriority;
    public Transform GroundPoint;
    public Transform MeatMesh;
    public Transform[] Spikes;
    public Transform Drill;
    public float Radius;
    public float Height;
    public float MeatMeshOGScale = 0.58f;
    public Vector2 DrillExtents;
    public Vector2 SpikeXRotations;
    private float[] m_initialSpikeRots = new float[3];
    private float m_spikeLerp;
    private Vector3 m_initialDrillLocalPos;
    private float m_drillRot;
    [Header("Decision Params")]
    public Vector2 ThinkTimeRange;
    public MeatCrab.MeatCrabState State;
    private float m_thinkingTick = 1f;
    private Vector3 m_navPoint;
    private NavMeshHit m_hit;
    private Vector3 m_targPos;
    private Quaternion m_targRot;
    private bool m_controlledMovement = true;
    [Header("Hopping Params")]
    public float HopSpeed;
    public Vector2 HopDistanceRange;
    public Vector2 HopHeightRange;
    public AnimationCurve HopHeightCurve;
    private float m_hopLerp;
    private Vector3 m_hopFrom;
    private Vector3 m_hopTo;
    private Vector3 m_hopForwardFrom;
    private Vector3 m_hopForwardTo;
    private float m_speedMult = 1f;
    private float m_heightMult = 1f;
    [Header("Lunging Params")]
    public float LungeRange;
    public float LungeVelocity;
    public LayerMask LM_LungeAttack;
    public float LungeAttackAngle = 50f;
    [Header("Ballistic Params")]
    public float RecoveryCheckTime = 0.1f;
    private float m_recoveryCheckTick = 0.5f;
    public LayerMask LM_Recovery;
    private RaycastHit m_rayHit;
    public float DownwardRecoveryCheckDistance = 0.2f;
    private float m_recoverySampleDistance = 0.5f;
    private float m_timeWhileBallistic = 1f;
    [Header("AttachedParams")]
    public float DetachForce = 4f;
    private Transform m_attachTransform;
    private Vector3 m_attachLocalPos;
    private Vector3 m_attachLocalFace;
    private Vector3 m_attachLocalUp;
    private float m_timeAttached;
    protected float AttachedRotationMultiplier2 = 60f;
    protected float AttachedPositionMultiplier2 = 9000f;
    protected float AttachedRotationFudge2 = 1000f;
    protected float AttachedPositionFudge2 = 1000f;
    public AudioEvent AudEvent_HitGround;
    public AudioEvent AudEvent_Leap;
    public AudioEvent AudEvent_HitTarget;
    public AudioSource AudSource_DrillSound;
    private float m_drillLerp;
    [Header("DamageStuff")]
    public float PointsLife = 4000f;
    public float LungeDamage = 250f;
    public bool DoesPoisonAttack;
    public GameObject DeathFX;
    private bool m_dead;

    protected override void Start()
    {
      base.Start();
      this.Init();
      if (this.Priority == null)
        return;
      this.m_hasPriority = true;
      this.Priority.Init(this.E, 3, 5f, 3f);
    }

    private void Init()
    {
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.InitiateThinking(this.GroundPoint.position, this.GroundPoint.forward);
      for (int index = 0; index < this.m_initialSpikeRots.Length; ++index)
        this.m_initialSpikeRots[index] = this.Spikes[index].localEulerAngles.y;
      this.m_initialDrillLocalPos = this.Drill.localPosition;
      if (!((Object) GM.TNH_Manager != (Object) null))
        return;
      GM.TNH_Manager.AddToMiscEnemies(this.gameObject);
    }

    public void EventReceive(AIEvent e)
    {
      if (e.IsEntity && e.Entity.IFFCode == this.E.IFFCode || (e.Type != AIEvent.AIEType.Visual || !this.m_hasPriority))
        return;
      this.Priority.ProcessEvent(e);
    }

    public override bool IsInteractable() => this.State == MeatCrab.MeatCrabState.Still;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.IsHeld)
      {
        this.State = MeatCrab.MeatCrabState.Ballistic;
        this.m_controlledMovement = false;
      }
      this.Crabdate();
      if (this.State == MeatCrab.MeatCrabState.Ballistic || this.State == MeatCrab.MeatCrabState.Hopping || this.State == MeatCrab.MeatCrabState.Lunging)
        this.DistantGrabbable = true;
      else
        this.DistantGrabbable = false;
    }

    protected override void FVRFixedUpdate()
    {
      this.CrabMover();
      base.FVRFixedUpdate();
    }

    private void Crabdate()
    {
      this.Priority.Compute();
      switch (this.State)
      {
        case MeatCrab.MeatCrabState.Still:
          this.Crabdate_Still();
          break;
        case MeatCrab.MeatCrabState.Hopping:
          this.Crabdate_Hopping();
          break;
        case MeatCrab.MeatCrabState.Lunging:
          this.Crabdate_Lunging();
          break;
        case MeatCrab.MeatCrabState.Ballistic:
          this.Crabdate_Ballistic();
          break;
        case MeatCrab.MeatCrabState.Attached:
          this.Crabdate_Attached();
          break;
      }
    }

    private void InitiateThinking(Vector3 navPoint, Vector3 forward)
    {
      this.m_controlledMovement = true;
      this.m_navPoint = navPoint;
      this.State = MeatCrab.MeatCrabState.Still;
      this.m_targPos = navPoint + Vector3.up * this.Height;
      this.m_targRot = Quaternion.LookRotation(forward, Vector3.up);
      this.m_thinkingTick = Random.Range(this.ThinkTimeRange.x, this.ThinkTimeRange.y);
    }

    private void InitiateHop(Vector3 posToHopTo, Vector3 newForwardDir)
    {
      this.m_controlledMovement = true;
      this.State = MeatCrab.MeatCrabState.Hopping;
      this.m_hopLerp = 0.0f;
      this.m_hopFrom = this.m_navPoint;
      this.m_hopTo = posToHopTo;
      this.m_hopForwardFrom = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);
      this.m_hopForwardTo = newForwardDir;
    }

    private void InitiateLunge(Vector3 pointToLungeAt)
    {
      this.m_controlledMovement = false;
      this.State = MeatCrab.MeatCrabState.Lunging;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < 10.0)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Leap, this.transform.position);
      this.RootRigidbody.velocity = this.LungeVelocity * (pointToLungeAt - this.transform.position).normalized;
    }

    private void InitiateBallistic()
    {
      this.m_controlledMovement = false;
      this.RootRigidbody.mass = 1f;
      this.m_timeWhileBallistic = Random.Range(0.0f, 1f);
      this.State = MeatCrab.MeatCrabState.Ballistic;
    }

    private void InitiateAttached(Transform t, Vector3 pos, Vector3 facingDir, Vector3 upDir)
    {
      this.m_controlledMovement = true;
      this.State = MeatCrab.MeatCrabState.Attached;
      this.RootRigidbody.mass = 0.1f;
      this.m_attachTransform = t;
      this.m_attachLocalPos = t.InverseTransformPoint(pos);
      this.m_attachLocalFace = t.InverseTransformDirection(facingDir);
      this.m_attachLocalUp = t.InverseTransformDirection(upDir);
      this.m_timeAttached = 0.0f;
    }

    private void Crabdate_Still()
    {
      this.m_thinkingTick -= Time.deltaTime;
      if ((double) this.m_thinkingTick > 0.0)
        return;
      bool flag = true;
      if (this.Priority.HasFreshTarget())
      {
        Vector3 targetPoint = this.Priority.GetTargetPoint();
        Vector3 newForwardDir = Vector3.ProjectOnPlane(targetPoint - this.transform.position, Vector3.up);
        float magnitude = newForwardDir.magnitude;
        newForwardDir.Normalize();
        if ((double) magnitude > (double) this.LungeRange)
        {
          float num = Random.Range(this.HopDistanceRange.x, this.HopDistanceRange.y);
          this.m_speedMult = 1f + num;
          this.m_heightMult = num / this.HopDistanceRange.y;
          Vector3 posToHopTo = this.m_navPoint + newForwardDir * num;
          if (!NavMesh.Raycast(this.m_navPoint, this.m_navPoint + newForwardDir * num, out this.m_hit, -1))
          {
            flag = false;
            this.InitiateHop(posToHopTo, newForwardDir);
          }
        }
        else
        {
          flag = false;
          this.InitiateLunge(targetPoint + Vector3.up * Random.Range(0.1f, 0.7f));
        }
      }
      if (!flag)
        return;
      Vector3 normalized = Vector3.ProjectOnPlane(Vector3.Slerp(this.transform.forward, Random.onUnitSphere, 0.25f), Vector3.up).normalized;
      float num1 = Random.Range(this.HopDistanceRange.x, this.HopDistanceRange.y);
      this.m_speedMult = 1f + num1;
      this.m_heightMult = num1 / this.HopDistanceRange.y;
      Vector3 posToHopTo1 = this.m_navPoint + normalized * num1;
      if (NavMesh.Raycast(this.m_navPoint, this.m_navPoint + normalized * num1, out this.m_hit, -1))
      {
        this.m_speedMult = 1f;
        this.m_heightMult = 0.25f;
        this.InitiateHop(this.m_navPoint, Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up).normalized);
      }
      else
        this.InitiateHop(posToHopTo1, normalized);
    }

    private void Crabdate_Hopping()
    {
      this.m_hopLerp += this.HopSpeed * this.m_speedMult * Time.deltaTime;
      Vector3 vector3 = Vector3.Lerp(this.m_hopFrom, this.m_hopTo, this.m_hopLerp);
      float num = this.HopHeightCurve.Evaluate(this.m_hopLerp);
      vector3.y += num * this.HopHeightRange.x * this.m_heightMult;
      this.MeatMesh.localScale = new Vector3(this.MeatMeshOGScale, this.MeatMeshOGScale + num * 0.1f, this.MeatMeshOGScale);
      Quaternion quaternion = Quaternion.LookRotation(Vector3.Slerp(this.m_hopForwardFrom, this.m_hopForwardTo, this.m_hopLerp), Vector3.up);
      this.m_targPos = vector3 + Vector3.up * this.Height;
      this.m_targRot = quaternion;
      if ((double) this.m_hopLerp < 1.0)
        return;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < 10.0)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_HitGround, this.transform.position);
      this.InitiateThinking(this.m_hopTo, this.m_hopForwardTo);
    }

    private void Crabdate_Lunging()
    {
      Vector3 position = this.transform.position;
      Vector3 normalized1 = this.RootRigidbody.velocity.normalized;
      float maxDistance = (float) ((double) this.RootRigidbody.velocity.magnitude * (double) Time.deltaTime * 2.0);
      if (!Physics.SphereCast(new Ray(position, normalized1), this.Radius, out this.m_rayHit, maxDistance, (int) this.LM_LungeAttack, QueryTriggerInteraction.Collide))
        return;
      IFVRDamageable component1 = this.m_rayHit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component1 == null && (Object) this.m_rayHit.collider.attachedRigidbody != (Object) null)
        component1 = this.m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component1 != null)
        component1.Damage(new FistVR.Damage()
        {
          Class = FistVR.Damage.DamageClass.Melee,
          hitNormal = this.transform.up,
          Dam_TotalKinetic = this.LungeDamage,
          Dam_Blunt = this.LungeDamage,
          Dam_Stunning = 3f,
          strikeDir = this.m_rayHit.normal,
          point = this.m_rayHit.point,
          Source_IFF = this.E.IFFCode,
          damageSize = 0.1f
        });
      bool flag = false;
      AIEntity component2 = this.m_rayHit.collider.attachedRigidbody.GetComponent<AIEntity>();
      AIEntity component3 = this.m_rayHit.collider.GetComponent<AIEntity>();
      AIEntity aiEntity = (AIEntity) null;
      if ((Object) component3 != (Object) null)
        aiEntity = component3;
      else if ((Object) component2 != (Object) null)
        aiEntity = component2;
      if ((Object) aiEntity == (Object) null && (Object) this.m_rayHit.collider.attachedRigidbody != (Object) null)
      {
        FVRPlayerHitbox component4 = this.m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
        if ((Object) component4 != (Object) null)
        {
          aiEntity = component4.MyE;
          flag = true;
          if (this.DoesPoisonAttack)
            GM.CurrentPlayerBody.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.Medium, PowerUpDuration.Short, false, true);
        }
        if ((Object) aiEntity == (Object) null)
        {
          SosigLink component5 = this.m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component5 != (Object) null)
          {
            aiEntity = component5.S.E;
            component5.S.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.Medium, PowerUpDuration.Short, false, true);
          }
        }
      }
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < 10.0)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_HitTarget, this.transform.position);
      if ((Object) aiEntity != (Object) null && (Object) this.m_rayHit.collider.attachedRigidbody != (Object) null && aiEntity.IFFCode != this.E.IFFCode && ((double) Vector3.Angle(aiEntity.GetThreatFacing(), -normalized1) < (double) this.LungeAttackAngle || !flag))
      {
        Vector3 pos = this.m_rayHit.point + normalized1 * -this.Radius;
        Vector3 normalized2 = (pos - this.m_rayHit.collider.transform.position).normalized;
        Vector3 rhs = Vector3.Cross(-Vector3.up, normalized2);
        Vector3 facingDir = Vector3.Cross(normalized2, rhs);
        Vector3 up = aiEntity.transform.up;
        Vector3 vector3 = pos - this.m_rayHit.collider.transform.position;
        this.InitiateAttached(this.m_rayHit.collider.transform, pos, facingDir, normalized2);
      }
      else
      {
        this.InitiateBallistic();
        this.RootRigidbody.velocity = Vector3.Slerp(-normalized1, Random.onUnitSphere, 0.3f) * Random.Range(this.LungeVelocity * 0.3f, this.LungeVelocity * 0.7f);
      }
    }

    private void Crabdate_Ballistic()
    {
      if ((double) this.m_spikeLerp > 0.0)
      {
        this.m_spikeLerp -= Time.deltaTime * 10f;
        for (int index = 0; index < this.Spikes.Length; ++index)
          this.Spikes[index].localEulerAngles = new Vector3(Mathf.Lerp(this.SpikeXRotations.x, this.SpikeXRotations.y, this.m_spikeLerp), this.m_initialSpikeRots[index], 0.0f);
      }
      if ((double) this.m_drillLerp > 0.0)
      {
        this.m_drillLerp -= Time.deltaTime * 5f;
        this.AudSource_DrillSound.volume = this.m_drillLerp * 0.5f;
        this.AudSource_DrillSound.pitch = Mathf.Lerp(0.5f, 1f, this.m_drillLerp);
        this.m_drillRot += (float) ((double) Time.deltaTime * (double) this.m_drillLerp * 4000.0);
        this.m_drillRot = Mathf.Repeat(this.m_drillRot, 360f);
        this.Drill.localEulerAngles = new Vector3(0.0f, this.m_drillRot, 0.0f);
        this.Drill.localPosition = new Vector3(this.m_initialDrillLocalPos.x, Mathf.Lerp(this.DrillExtents.x, this.DrillExtents.y, this.m_drillLerp), this.m_initialDrillLocalPos.z);
      }
      else if (this.AudSource_DrillSound.isPlaying)
        this.AudSource_DrillSound.Stop();
      if ((double) this.m_timeWhileBallistic < 2.0)
        this.m_timeWhileBallistic += Time.deltaTime;
      if ((double) this.m_recoveryCheckTick > 0.0)
      {
        this.m_recoveryCheckTick -= Time.deltaTime;
      }
      else
      {
        this.m_recoveryCheckTick = this.RecoveryCheckTime;
        if ((double) this.m_timeWhileBallistic < 1.5 || !Physics.Raycast(this.transform.position, Vector3.down, out this.m_rayHit, this.DownwardRecoveryCheckDistance, (int) this.LM_Recovery, QueryTriggerInteraction.Ignore))
          return;
        if (NavMesh.SamplePosition(this.m_rayHit.point, out this.m_hit, this.m_recoverySampleDistance, -1))
        {
          this.InitiateThinking(this.m_hit.position, Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up));
        }
        else
        {
          this.m_recoverySampleDistance += 0.1f;
          if ((double) this.m_recoverySampleDistance <= 2.0)
            return;
          this.Die();
        }
      }
    }

    private void Crabdate_Attached()
    {
      this.m_timeAttached += Time.deltaTime;
      if ((double) this.m_timeAttached > 8.0)
        this.InitiateBallistic();
      if ((double) this.m_spikeLerp < 1.0)
      {
        this.m_spikeLerp += Time.deltaTime * 4f;
        for (int index = 0; index < this.Spikes.Length; ++index)
          this.Spikes[index].localEulerAngles = new Vector3(Mathf.Lerp(this.SpikeXRotations.x, this.SpikeXRotations.y, this.m_spikeLerp), this.m_initialSpikeRots[index], 0.0f);
      }
      if ((double) this.m_drillLerp < 1.0)
      {
        this.m_drillLerp += Time.deltaTime * 0.25f;
        if (!this.AudSource_DrillSound.isPlaying)
          this.AudSource_DrillSound.Play();
        this.AudSource_DrillSound.volume = this.m_drillLerp * 0.5f;
        this.AudSource_DrillSound.pitch = Mathf.Lerp(0.5f, 1f, this.m_drillLerp);
        this.m_drillRot += (float) ((double) Time.deltaTime * (double) this.m_drillLerp * 4000.0);
        this.m_drillRot = Mathf.Repeat(this.m_drillRot, 360f);
        this.Drill.localEulerAngles = new Vector3(0.0f, this.m_drillRot, 0.0f);
        this.Drill.localPosition = new Vector3(this.m_initialDrillLocalPos.x, Mathf.Lerp(this.DrillExtents.x, this.DrillExtents.y, this.m_drillLerp), this.m_initialDrillLocalPos.z);
      }
      if ((double) this.m_drillLerp <= 0.800000011920929 || !Physics.Raycast(this.transform.position, -this.transform.up, out this.m_rayHit, 1f, (int) this.LM_LungeAttack, QueryTriggerInteraction.Collide))
        return;
      IFVRDamageable component = this.m_rayHit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component == null && (Object) this.m_rayHit.collider.attachedRigidbody != (Object) null)
        component = this.m_rayHit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component == null)
        return;
      component.Damage(new FistVR.Damage()
      {
        Class = FistVR.Damage.DamageClass.Melee,
        hitNormal = this.transform.up,
        Dam_TotalKinetic = 250f,
        Dam_Blunt = 1000f,
        Dam_Stunning = 3f,
        strikeDir = this.m_rayHit.normal,
        point = this.m_rayHit.point,
        Source_IFF = this.E.IFFCode,
        damageSize = 0.1f
      });
    }

    private void CrabMover()
    {
      if (!this.m_controlledMovement)
        return;
      Vector3 position = this.transform.position;
      Quaternion rotation = this.transform.rotation;
      Vector3 vector3_1 = this.m_targPos - position;
      Quaternion quaternion1 = this.m_targRot * Quaternion.Inverse(rotation);
      if (this.State == MeatCrab.MeatCrabState.Attached)
      {
        if ((Object) this.m_attachTransform == (Object) null)
        {
          this.InitiateBallistic();
          return;
        }
        Vector3 vector3_2 = this.m_attachTransform.TransformPoint(this.m_attachLocalPos);
        Quaternion quaternion2 = Quaternion.LookRotation(this.m_attachTransform.TransformDirection(this.m_attachLocalFace), this.m_attachTransform.TransformDirection(this.m_attachLocalUp));
        vector3_1 = vector3_2 - position;
        quaternion1 = quaternion2 * Quaternion.Inverse(rotation);
      }
      float deltaTime = Time.deltaTime;
      float angle;
      Vector3 axis;
      quaternion1.ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0)
        this.RootRigidbody.angularVelocity = Vector3.MoveTowards(this.RootRigidbody.angularVelocity, deltaTime * angle * axis * this.AttachedRotationMultiplier2, this.AttachedRotationFudge2 * Time.fixedDeltaTime);
      this.RootRigidbody.velocity = Vector3.MoveTowards(this.RootRigidbody.velocity, vector3_1 * this.AttachedPositionMultiplier2 * deltaTime, this.AttachedPositionFudge2 * deltaTime);
    }

    public override void OnCollisionEnter(Collision col)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (this.State == MeatCrab.MeatCrabState.Attached && ((Object) col.collider.attachedRigidbody == (Object) null || col.collider.gameObject.layer == LayerMask.NameToLayer("AgentBody")))
        flag2 = true;
      switch (this.State)
      {
        case MeatCrab.MeatCrabState.Still:
          if (!flag2 && (Object) col.collider.attachedRigidbody != (Object) null && (double) col.relativeVelocity.magnitude > 3.0)
          {
            flag1 = true;
            break;
          }
          break;
        case MeatCrab.MeatCrabState.Hopping:
          if (!flag2 && (Object) col.collider.attachedRigidbody != (Object) null && (double) col.relativeVelocity.magnitude > 1.0)
          {
            flag1 = true;
            break;
          }
          break;
        case MeatCrab.MeatCrabState.Lunging:
          flag1 = true;
          break;
        case MeatCrab.MeatCrabState.Attached:
          if (!flag2 && (Object) col.collider.attachedRigidbody != (Object) null && (double) col.relativeVelocity.magnitude > (double) this.DetachForce)
          {
            flag1 = true;
            break;
          }
          break;
      }
      if (flag1)
        this.InitiateBallistic();
      base.OnCollisionEnter(col);
    }

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_TotalKinetic > 50.0)
      {
        this.InitiateBallistic();
        this.RootRigidbody.AddForceAtPosition(d.Dam_TotalKinetic * 0.01f * d.strikeDir, d.point);
      }
      this.PointsLife -= d.Dam_TotalKinetic;
      this.PointsLife -= d.Dam_Thermal * 0.2f;
      if ((double) this.PointsLife >= 0.0)
        return;
      this.Die();
    }

    private void Die()
    {
      if (this.m_dead)
        return;
      this.m_dead = true;
      Object.Instantiate<GameObject>(this.DeathFX, this.transform.position, Random.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public enum MeatCrabState
    {
      Still,
      Hopping,
      Lunging,
      Ballistic,
      Attached,
    }
  }
}
