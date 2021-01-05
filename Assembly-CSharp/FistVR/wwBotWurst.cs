// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurst
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class wwBotWurst : MonoBehaviour
  {
    [Header("Core Refs")]
    public wwBotManager Manager;
    public wwBotWurstConfig Config;
    public NavMeshAgent Agent;
    public wwBotWurstNavPointGroup NavPointGroup;
    public wwBotWurstGun[] Guns;
    public List<wwBotWurstModernGun> ModernGuns = new List<wwBotWurstModernGun>();
    public Transform ModernGunMount;
    public wwBotWurstHat Hat;
    public GameObject DropOnDeath;
    private float tick = 4f;
    public Rigidbody RB_Head;
    public Rigidbody RB_Torso;
    public Rigidbody RB_Bottom;
    public wwBotWurstDamageablePiece[] Pieces;
    public Transform Target;
    public LayerMask LM_Sight;
    public LayerMask LM_SmokeDetect;
    public LayerMask LM_IFF;
    public bool CanBeStunned = true;
    private bool m_isStunned;
    private float m_stunTick;
    private bool m_isInSmoke;
    public float Max_LinearSpeed_Walk;
    public float Max_LinearSpeed_Combat;
    public float Max_LinearSpeed_Run;
    public float Acceleration;
    public float Max_AngularSpeed;
    public float MaxViewAngle;
    public float MaxViewDistance;
    public float MaxFiringAngle;
    public float MaxFiringDistance;
    public wwBotWurst.BotState State = wwBotWurst.BotState.Patrolling;
    private float m_timeTilNextLookRot = 1f;
    private Vector3 m_targetFacing = Vector3.zero;
    private float m_timeTilPickNewPatrolPoint = 1f;
    private Vector3 m_patrolDestinationPoint = Vector3.zero;
    private Vector3 m_fleeDestinationPoint = Vector3.zero;
    public Vector3 LastPlaceTargetSeen = Vector3.zero;
    private float m_timeSinceTargetSeen = 10f;
    private Vector3 m_combatDestinationPoint = Vector3.zero;
    private bool m_hasSearchPoint;
    private Vector3 m_searchingDestinationPoint = Vector3.zero;
    private float m_atSearchPointTick;
    private bool m_isAggrod;
    private bool m_isPosse;
    private int m_index;
    public bool CanCivGetScared = true;
    private float m_possibleSoundPlayTick = 10f;
    private bool m_canSpeak;
    private float m_senseTick;
    private float standingAroundAggroTime;

    private void Awake()
    {
      this.Agent.updateRotation = false;
      this.PickNewFacing();
      this.Agent.acceleration = this.Config.Acceleration;
      this.Pieces[0].ParentAttachingJoint = this.Pieces[0].gameObject.GetComponent<Joint>();
      this.Pieces[1].ParentAttachingJoint = this.Pieces[1].gameObject.GetComponent<Joint>();
      this.Pieces[1].Child = this.Pieces[0].transform;
      this.Pieces[2].Child = this.Pieces[1].transform;
      this.Pieces[0].SetIsHead(true);
      this.Pieces[0].SetLife((float) this.Config.Life_Head);
      this.Pieces[1].SetLife((float) this.Config.Life_Torso);
      this.Pieces[2].SetLife((float) this.Config.Life_Bottom);
      this.Max_LinearSpeed_Walk = this.Config.LinearSpeed_Walk;
      this.Max_LinearSpeed_Combat = this.Config.LinearSpeed_Combat;
      this.Max_LinearSpeed_Run = this.Config.LinearSpeed_Run;
      this.Acceleration = this.Config.Acceleration;
      this.Max_AngularSpeed = this.Config.MaxAngularSpeed;
      this.MaxViewAngle = this.Config.MaxViewAngle;
      this.MaxViewDistance = this.Config.MaxViewDistance;
      this.MaxFiringAngle = this.Config.AngularFiringRange;
      this.MaxFiringDistance = this.Config.MaximumFiringRange;
      if (!((Object) this.Config != (Object) null))
        return;
      this.m_possibleSoundPlayTick = Random.Range(this.Config.CalloutFrequencyRange.x, this.Config.CalloutFrequencyRange.y);
    }

    private void Start()
    {
      if ((Object) this.Agent != (Object) null)
        this.Agent.enabled = true;
      this.StartCoroutine(this.MeshKill());
    }

    [DebuggerHidden]
    private IEnumerator MeshKill() => (IEnumerator) new wwBotWurst.\u003CMeshKill\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public void BlowUpHead()
    {
      if (!((Object) this.Pieces[0] != (Object) null))
        return;
      this.Pieces[0].Splode();
      this.BotExplosionPiece(this.Pieces[0], this.Pieces[0].transform.position, -this.Pieces[0].transform.up);
    }

    private float GetCurrentSpeedMultiplier()
    {
      if (this.m_isStunned)
        return 0.15f;
      return this.m_isInSmoke ? 0.3f : 1f;
    }

    public void ReConfig(wwBotWurstConfig c, float lifeMult = 1f)
    {
      if ((Object) this.Agent != (Object) null)
        this.Agent.acceleration = c.Acceleration;
      this.Pieces[0].SetLife((float) c.Life_Head * lifeMult);
      this.Pieces[1].SetLife((float) c.Life_Torso * lifeMult);
      this.Pieces[2].SetLife((float) c.Life_Bottom * lifeMult);
      this.Max_LinearSpeed_Walk = c.LinearSpeed_Walk * Random.Range(0.9f, 1.1f);
      this.Max_LinearSpeed_Combat = c.LinearSpeed_Combat * Random.Range(0.9f, 1.1f);
      this.Max_LinearSpeed_Run = c.LinearSpeed_Run * Random.Range(0.9f, 1.1f);
      this.Acceleration = c.Acceleration;
      this.Max_AngularSpeed = c.MaxAngularSpeed;
      this.MaxViewAngle = c.MaxViewAngle;
      this.MaxViewDistance = c.MaxViewDistance;
      this.MaxFiringAngle = c.AngularFiringRange;
      this.MaxFiringDistance = c.MaximumFiringRange;
    }

    public void HealthOverride(float head, float body, float lower)
    {
      this.Pieces[0].SetLife(head);
      this.Pieces[1].SetLife(body);
      this.Pieces[2].SetLife(lower);
    }

    public void ConfigBot(
      int index,
      bool isPosse,
      wwBotManager manager,
      wwBotWurstNavPointGroup navgroup,
      Transform targetOverride)
    {
      this.Manager = manager;
      this.m_index = index;
      this.m_isPosse = isPosse;
      this.NavPointGroup = navgroup;
      this.Target = targetOverride;
      if ((Object) this.Hat != (Object) null)
      {
        this.Hat.HatBanditIndex = index;
        this.Hat.IsPosse = isPosse;
        this.Hat.Manager = manager;
      }
      this.m_possibleSoundPlayTick = Random.Range(this.Config.CalloutFrequencyRange.x, this.Config.CalloutFrequencyRange.y);
    }

    private void Update()
    {
      if ((double) this.m_timeSinceTargetSeen < 30.0)
        this.m_timeSinceTargetSeen += Time.deltaTime;
      this.StateUpdate();
    }

    public void HatRemoved()
    {
      if ((Object) this.Hat != (Object) null && (Object) this.Hat.RootRigidbody == (Object) null)
      {
        this.Hat.transform.SetParent((Transform) null);
        this.Hat.RootRigidbody = this.Hat.gameObject.AddComponent<Rigidbody>();
      }
      this.Pieces[0].Splode();
      this.BotExplosionPiece(this.Pieces[0], this.Pieces[0].transform.position, -this.Pieces[0].transform.up);
    }

    public void BotExplosionPiece(wwBotWurstDamageablePiece p, Vector3 point, Vector3 strikeDir)
    {
      if ((Object) this.Agent != (Object) null)
        this.Agent.enabled = false;
      this.RB_Bottom.isKinematic = false;
      if ((Object) this.Hat != (Object) null && (Object) this.Hat.RootRigidbody == (Object) null)
      {
        this.Hat.Remove();
        if (this.Hat.IsPosse)
        {
          Object.Destroy((Object) this.Hat.gameObject);
        }
        else
        {
          this.Hat.transform.SetParent((Transform) null);
          this.Hat.RootRigidbody = this.Hat.gameObject.AddComponent<Rigidbody>();
        }
      }
      foreach (wwBotWurstGun gun in this.Guns)
        gun.SetFireAtWill(false);
      foreach (wwBotWurstModernGun modernGun in this.ModernGuns)
        modernGun.SetFireAtWill(false);
      for (int index = 0; index < this.Pieces.Length; ++index)
      {
        if ((Object) this.Pieces[index] != (Object) null)
          this.Pieces[index].StartCountingDown();
      }
      if ((Object) p == (Object) this.Pieces[0] || (Object) p == (Object) this.Pieces[1] || !((Object) p == (Object) this.Pieces[2]))
        ;
      if ((Object) this.Manager != (Object) null)
        this.Manager.SpawnedBots.Remove(this.gameObject);
      if ((Object) this.Manager != (Object) null)
        this.Manager.BotKilled(this.m_index, this.transform.position + Vector3.up * 1.5f);
      GM.CurrentSceneSettings.ShotEventReceivers.Remove(this.gameObject);
      if ((Object) this.DropOnDeath != (Object) null)
      {
        Rigidbody component = Object.Instantiate<GameObject>(this.DropOnDeath, this.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
          component.velocity = Random.onUnitSphere * Random.Range(0.5f, 3f);
      }
      Object.Destroy((Object) p.gameObject);
      if (!((Object) this != (Object) null))
        return;
      Object.Destroy((Object) this);
    }

    public void StunBot(float f)
    {
      if (!this.CanBeStunned)
        return;
      if (!this.m_isStunned)
        this.m_isStunned = true;
      this.m_stunTick = Mathf.Max(f, this.m_stunTick);
      this.LastPlaceTargetSeen = this.transform.position + Random.onUnitSphere * Random.Range(1f, 5f);
    }

    private void SensingLoop()
    {
      if (this.CanBeStunned && this.m_isStunned)
      {
        this.m_stunTick -= Time.deltaTime;
        if ((double) this.m_stunTick <= 0.0)
          this.m_isStunned = false;
      }
      if ((double) this.m_senseTick > 0.0)
      {
        this.m_senseTick -= Time.deltaTime;
      }
      else
      {
        this.m_senseTick = Random.Range(0.1f, 0.25f);
        if (Physics.CheckSphere(this.RB_Head.transform.position + this.RB_Head.transform.up * 0.3f, 0.4f, (int) this.LM_SmokeDetect, QueryTriggerInteraction.Collide))
        {
          this.m_isInSmoke = true;
          this.m_timeSinceTargetSeen += Time.deltaTime * 2f;
        }
        else
        {
          this.m_isInSmoke = false;
          int num1 = Random.Range(0, 2);
          Vector3 zero = Vector3.zero;
          Vector3 end = Vector3.zero;
          if ((Object) this.Target != (Object) null)
            end = this.Target.position;
          else if ((Object) GM.CurrentPlayerBody != (Object) null)
            end = num1 <= 0 ? GM.CurrentPlayerBody.Torso.transform.position : GM.CurrentPlayerBody.Head.transform.position;
          if (this.m_isStunned)
            end = this.transform.position + Random.onUnitSphere * Random.Range(3f, 10f);
          Vector3 to = end - this.RB_Head.transform.position;
          float num2 = Vector3.Angle(this.RB_Head.transform.forward, to);
          bool flag = true;
          if ((Object) GM.CurrentPlayerBody != (Object) null && GM.CurrentPlayerBody.IsGhosted)
            flag = false;
          if (!flag || (double) num2 >= (double) this.MaxViewAngle || ((double) to.magnitude >= (double) this.MaxViewDistance || Physics.Linecast(this.RB_Head.transform.position, end, (int) this.LM_Sight, QueryTriggerInteraction.Ignore)))
            return;
          this.m_timeSinceTargetSeen = 0.0f;
          this.LastPlaceTargetSeen = end;
          if (this.Config.CanFight)
            this.State = wwBotWurst.BotState.Fighting;
          if (this.Config.CanFight || !this.m_isAggrod || !this.CanCivGetScared)
            return;
          this.State = wwBotWurst.BotState.RunAway;
        }
      }
    }

    private void StateUpdate()
    {
      if ((Object) this.Agent == (Object) null || !this.Agent.isActiveAndEnabled)
        return;
      switch (this.State)
      {
        case wwBotWurst.BotState.StandingAround:
          if (this.Config.CanFight)
            this.SensingLoop();
          this.State_StandingAround();
          break;
        case wwBotWurst.BotState.Patrolling:
          this.SensingLoop();
          this.State_Patrolling();
          break;
        case wwBotWurst.BotState.Fighting:
          this.SensingLoop();
          this.State_Fighting();
          break;
        case wwBotWurst.BotState.Searching:
          this.SensingLoop();
          this.State_Searching();
          break;
        case wwBotWurst.BotState.RunAway:
          this.SensingLoop();
          this.State_RunAway();
          break;
        case wwBotWurst.BotState.Dead:
          this.State_Dead();
          break;
      }
      if ((double) this.m_possibleSoundPlayTick > 0.0)
      {
        this.m_possibleSoundPlayTick -= Time.deltaTime;
        this.m_canSpeak = false;
      }
      else
        this.m_canSpeak = true;
    }

    private void SayHook(AudioEvent hookEvent, float rangeLimit)
    {
      if (hookEvent.Clips.Count <= 0)
        return;
      this.m_possibleSoundPlayTick = Random.Range(this.Config.CalloutFrequencyRange.x, this.Config.CalloutFrequencyRange.y);
      this.m_canSpeak = false;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) >= (double) rangeLimit)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, hookEvent, this.transform.position);
    }

    private void TurnTowardsFacing(float speed)
    {
      Vector3 vector3 = Vector3.RotateTowards(this.transform.forward, this.m_targetFacing, speed * Time.deltaTime, 1f);
      vector3.y = 0.0f;
      this.RB_Head.AddTorque(Vector3.up * Vector3.Angle(vector3, this.transform.forward) * 0.4f, ForceMode.VelocityChange);
      this.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
    }

    public bool GetFireClear(Vector3 source, Vector3 target) => this.m_isInSmoke || this.m_isStunned || !Physics.Linecast(source, target, (int) this.LM_IFF, QueryTriggerInteraction.Ignore);

    private void FaceTowards(Vector3 point)
    {
      Vector3 vector3 = point - this.transform.position + this.transform.forward * 0.0001f;
      vector3.y = 0.0f;
      vector3.Normalize();
      this.m_targetFacing = vector3;
    }

    private void PickNewFacing()
    {
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      onUnitSphere.Normalize();
      this.m_targetFacing = onUnitSphere;
    }

    public void RegisterHit(Damage d, bool IsDeath)
    {
      if (this.Config.CanFight)
        this.State = wwBotWurst.BotState.Fighting;
      else if (this.CanCivGetScared)
      {
        this.m_isAggrod = true;
        this.State = wwBotWurst.BotState.RunAway;
      }
      if ((Object) this.transform != (Object) null && !this.m_isStunned)
      {
        this.LastPlaceTargetSeen = this.transform.position - d.strikeDir * 10f;
        this.m_timeSinceTargetSeen = 0.0f;
      }
      if (!IsDeath)
        return;
      GM.CurrentSceneSettings.OnBotKill(d);
    }

    public void ShotEvent(Vector3 pos)
    {
      if (!this.Config.CanFight || this.m_isStunned || ((double) this.MaxViewDistance * 3.0 < (double) Vector3.Distance(this.transform.position, pos) || this.State == wwBotWurst.BotState.Fighting))
        return;
      this.State = wwBotWurst.BotState.Searching;
      this.m_timeSinceTargetSeen = 0.15f;
      Vector3 vector3 = Random.onUnitSphere * 3f;
      vector3.y = 0.0f;
      this.LastPlaceTargetSeen = pos + vector3;
    }

    private void State_StandingAround()
    {
      if ((double) this.m_timeTilNextLookRot > 0.0)
      {
        this.m_timeTilNextLookRot -= Time.deltaTime;
      }
      else
      {
        this.m_timeTilNextLookRot = Random.Range(this.Config.LookAroundNewPointFrequency.x, this.Config.LookAroundNewPointFrequency.y);
        this.PickNewFacing();
      }
      this.TurnTowardsFacing(this.Max_AngularSpeed * 0.25f);
    }

    private void State_Patrolling()
    {
      if (!this.Agent.hasPath && !this.Agent.pathPending || (double) this.m_timeTilPickNewPatrolPoint < 0.0)
      {
        this.m_patrolDestinationPoint = this.NavPointGroup.GetRandomPatrolPoint();
        this.SetBotDest(this.m_patrolDestinationPoint);
        this.Agent.speed = this.Max_LinearSpeed_Walk * this.GetCurrentSpeedMultiplier();
        this.m_timeTilPickNewPatrolPoint = Random.Range(this.Config.PatrolNewPointFrequency.x, this.Config.PatrolNewPointFrequency.y);
      }
      if (this.m_canSpeak)
      {
        if (this.Config.CanFight)
          this.SayHook(this.Config.SpeakEvent_Patrol, 12f);
        else if ((Object) GM.CurrentPlayerBody != (Object) null)
          this.SayHook(this.Config.SpeakEvent_Greetings, 12f);
        else
          this.SayHook(this.Config.SpeakEvent_Patrol, 12f);
      }
      this.m_timeTilPickNewPatrolPoint -= Time.deltaTime;
      Vector3 vector3 = this.Agent.desiredVelocity.normalized + this.transform.forward * 0.0001f;
      vector3.y = 0.0f;
      this.m_targetFacing = vector3;
      this.TurnTowardsFacing(this.Max_AngularSpeed * 0.5f);
    }

    private void State_Fighting()
    {
      bool b1 = false;
      float magnitude = (this.LastPlaceTargetSeen - this.transform.position).magnitude;
      Vector3 b2 = this.m_combatDestinationPoint;
      if (this.ModernGuns.Count > 0)
      {
        if (this.m_canSpeak && (Object) this.ModernGuns[0] != (Object) null)
        {
          if (this.ModernGuns[0].FireState != wwBotWurstModernGun.FiringState.GoingToReload && this.ModernGuns[0].FireState != wwBotWurstModernGun.FiringState.Reloading && this.ModernGuns[0].FireState != wwBotWurstModernGun.FiringState.RecoveringFromReload)
            this.SayHook(this.Config.SpeakEvent_InCombat, 24f);
          else
            this.SayHook(this.Config.SpeakEvent_Reloading, 24f);
        }
      }
      else if (this.m_canSpeak && (Object) this.Guns[0] != (Object) null)
      {
        if (this.Guns[0].FireState != wwBotWurstGun.FiringState.firing && this.Guns[0].FireState != wwBotWurstGun.FiringState.cycledown && this.Guns[0].FireState != wwBotWurstGun.FiringState.cycleup)
          this.SayHook(this.Config.SpeakEvent_InCombat, 24f);
        else
          this.SayHook(this.Config.SpeakEvent_Reloading, 24f);
      }
      if (this.Config.IsMelee)
      {
        if ((double) Vector3.Distance(new Vector3(this.LastPlaceTargetSeen.x, 0.0f, this.LastPlaceTargetSeen.z), new Vector3(this.m_combatDestinationPoint.x, 0.0f, this.m_combatDestinationPoint.z)) > 0.5)
        {
          b2 = this.LastPlaceTargetSeen;
          this.Agent.speed = this.Max_LinearSpeed_Combat * this.GetCurrentSpeedMultiplier();
          if ((Object) GM.CurrentPlayerBody != (Object) null)
            b2 += GM.CurrentPlayerBody.Head.forward;
        }
      }
      else
        b2 = (double) magnitude >= (double) this.Config.PreferedDistanceRange.x ? ((double) magnitude <= (double) this.Config.PreferedDistanceRange.y ? this.NavPointGroup.GetClosestCoverFromAttacker(this.transform.position, this.LastPlaceTargetSeen) : this.NavPointGroup.GetClosestDestinationToTarget(this.LastPlaceTargetSeen)) : this.NavPointGroup.GetBestPointToFleeTo(this.transform.position, this.LastPlaceTargetSeen);
      if ((double) Vector3.Distance(this.m_combatDestinationPoint, b2) > 1.0 && !this.Agent.pathPending)
      {
        this.m_combatDestinationPoint = b2;
        this.SetBotDest(this.m_combatDestinationPoint);
        this.Agent.speed = this.Max_LinearSpeed_Combat * this.GetCurrentSpeedMultiplier();
      }
      if ((double) magnitude <= (double) this.MaxFiringDistance)
        b1 = true;
      this.FaceTowards(this.LastPlaceTargetSeen);
      this.TurnTowardsFacing(this.Max_AngularSpeed * 1f);
      foreach (wwBotWurstGun gun in this.Guns)
        gun.SetFireAtWill(b1);
      foreach (wwBotWurstModernGun modernGun in this.ModernGuns)
        modernGun.SetFireAtWill(b1);
      if ((double) this.m_timeSinceTargetSeen <= (double) this.Config.TimeBlindFiring)
        return;
      this.State = wwBotWurst.BotState.Searching;
      this.m_atSearchPointTick = Random.Range(this.Config.WaitAtSearchPointRange.x, this.Config.WaitAtSearchPointRange.y);
      this.m_hasSearchPoint = false;
    }

    private void State_Searching()
    {
      foreach (wwBotWurstGun gun in this.Guns)
        gun.SetFireAtWill(false);
      foreach (wwBotWurstModernGun modernGun in this.ModernGuns)
        modernGun.SetFireAtWill(false);
      if (!this.m_hasSearchPoint)
      {
        this.m_searchingDestinationPoint = this.NavPointGroup.GetClosestDestinationToTarget(this.LastPlaceTargetSeen);
        this.SetBotDest(this.m_searchingDestinationPoint);
        this.Agent.speed = this.Max_LinearSpeed_Run * this.GetCurrentSpeedMultiplier();
      }
      if (this.m_canSpeak)
        this.SayHook(this.Config.SpeakEvent_Searching, 16f);
      if ((double) Vector3.Distance(this.transform.position, this.m_searchingDestinationPoint) < 1.0)
        this.m_atSearchPointTick -= Time.deltaTime;
      if ((double) this.m_atSearchPointTick <= 0.0)
        this.State = wwBotWurst.BotState.Patrolling;
      if ((double) this.m_timeSinceTargetSeen >= 0.100000001490116)
        return;
      this.State = wwBotWurst.BotState.Fighting;
    }

    private void State_RunAway()
    {
      float num = Vector3.Distance(this.transform.position, this.LastPlaceTargetSeen);
      if (this.m_canSpeak)
        this.SayHook(this.Config.SpeakEvent_RunningAway, 16f);
      if (!this.Agent.hasPath && !this.Agent.pathPending && (double) num < (double) this.Config.PreferedDistanceRange.y)
      {
        this.m_fleeDestinationPoint = this.NavPointGroup.GetBestPointToFleeTo(this.transform.position, this.LastPlaceTargetSeen);
        this.SetBotDest(this.m_fleeDestinationPoint);
        this.Agent.speed = this.Max_LinearSpeed_Run * this.GetCurrentSpeedMultiplier();
      }
      if ((double) num < (double) this.Config.PreferedDistanceRange.x)
      {
        Vector3 bestPointToFleeTo = this.NavPointGroup.GetBestPointToFleeTo(this.transform.position, this.LastPlaceTargetSeen);
        if ((double) Vector3.Distance(this.m_fleeDestinationPoint, bestPointToFleeTo) > 5.0)
        {
          this.m_fleeDestinationPoint = bestPointToFleeTo;
          this.SetBotDest(this.m_fleeDestinationPoint);
          this.Agent.speed = this.Max_LinearSpeed_Run * this.GetCurrentSpeedMultiplier();
        }
      }
      this.FaceTowards(this.m_fleeDestinationPoint);
      this.TurnTowardsFacing(this.Max_AngularSpeed * 1f);
    }

    private void State_Dead()
    {
    }

    private void SetBotDest(Vector3 p)
    {
      if (!((Object) this.Agent != (Object) null) || !this.Agent.isOnNavMesh)
        return;
      this.Agent.SetDestination(p);
    }

    public enum BotState
    {
      StandingAround,
      Patrolling,
      Fighting,
      Searching,
      RunAway,
      Dead,
    }
  }
}
