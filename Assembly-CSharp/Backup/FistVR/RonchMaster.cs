// Decompiled with JetBrains decompiler
// Type: FistVR.RonchMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RonchMaster : MonoBehaviour
  {
    [Header("Master Connections")]
    public RonchMover Mover;
    public List<RonchWaypoint> Waypoints;
    public RonchMaster.RonchState CurState;
    public Queue<RonchMaster.RonchState> StateQueue = new Queue<RonchMaster.RonchState>();
    private bool m_advanceToNextState;
    public RonchWaypoint InitWaypoint;
    private RonchWaypoint m_lastWaypoint;
    private RonchWaypoint m_curWaypoint;
    private RonchWaypoint m_nextWaypoint;
    private Vector3 m_currentFacingDir = new Vector3(0.0f, 0.0f, 1f);
    private Vector3 m_targetFacingDir = new Vector3(0.0f, 0.0f, 1f);
    public float WalkSpeed = 5f;
    public float RunSpeed = 10f;
    [Header("Weapons")]
    public List<RonchWeapon> Gau8s;
    public RonchWeapon MissileBank;
    public RonchWeapon FlashNades;
    public RonchWeapon SmokeNades;
    public List<RonchWeapon> Gau8s_Defensive;
    public RonchWeapon LaserNads;
    public Transform Laser_BaseDir;
    public Transform Laser_RotatingPart_Horizontal;
    public Transform Laser_RotatingPart_Vertical;
    public float Laser_Range = 20f;
    public Transform TargetPoint;
    [Header("Head")]
    public Transform Head;
    public Transform BaseHeadPos;
    public Transform HeadLower;
    [Header("LifeBar")]
    public Transform LifeBarHolder;
    public Transform LifeBar;
    public RonchMaster.DamageStage DamState;
    public RonchRayDome RayDome;
    public GameObject Entity_Cockpit;
    public RonchCockpit Cockpit;
    [Header("Dying Sequence")]
    public List<GameObject> ExplosionSetRandom;
    public List<Transform> ExplosionPlaces;
    public List<GameObject> FinalExplosionPrefabs;
    public Transform FinalExplosionPlace;
    private float m_TickDownDying = 8f;
    private float m_tickDownTilNextSplode = 0.1f;
    private int m_placeTick;
    [Header("Sight")]
    public LayerMask LM_SightBlock;
    public LayerMask LM_LaserBlock;
    [Header("Speaking")]
    public AudioEvent AudEvent_Intro;
    public AudioEvent AudEvent_Moving;
    public AudioEvent AudEvent_Turning;
    public AudioEvent AudEvent_Firing;
    public AudioEvent AudEvent_Dead;
    private HG_ModeManager_MeatleGear MM;
    private bool justFiredWeapon = true;
    private float m_turningLerp;
    private float m_timeUntilFireOpenLaser = 6f;
    private float m_timeUntilFireOpen = 15f;
    private bool m_isNextNadeFlash = true;
    private float m_timeUntilNextDefensiveMachineGun = 20f;

    public void SetModeManager(HG_ModeManager_MeatleGear m) => this.MM = m;

    private void Start()
    {
      this.m_curWaypoint = this.InitWaypoint;
      this.m_lastWaypoint = this.InitWaypoint;
      this.DecideNextActions();
      this.Mover.transform.SetParent((Transform) null);
      this.TargetPoint.SetParent((Transform) null);
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Intro, this.HeadLower.position);
    }

    private void DecideNextActions()
    {
      if (this.StateQueue.Count > 0)
      {
        this.SetState(this.StateQueue.Dequeue());
      }
      else
      {
        if (this.justFiredWeapon)
        {
          this.QueueUpMoveToRandomNeighborToWalkTo();
          this.justFiredWeapon = false;
        }
        else
        {
          this.SetTargetFacingDir(this.TargetPoint.position);
          this.StateQueue.Enqueue(RonchMaster.RonchState.TurnToFacing);
          this.QueueWeaponFiringBasedOnDistance();
          this.justFiredWeapon = true;
        }
        this.DecideNextActions();
      }
    }

    private void QueueUpMoveToRandomNeighborToWalkTo()
    {
      this.m_nextWaypoint = this.GetRandomNeighbor();
      this.m_lastWaypoint = this.m_curWaypoint;
      this.SetTargetFacingDir(this.m_nextWaypoint);
      this.StateQueue.Enqueue(RonchMaster.RonchState.TurnToFacing);
      this.StateQueue.Enqueue(RonchMaster.RonchState.MoveToNode);
    }

    private void QueueUpMoveToRandomNeighborToLeapTo()
    {
      this.m_nextWaypoint = this.GetRandomNeighbor();
      this.m_lastWaypoint = this.m_curWaypoint;
      this.SetTargetFacingDir(this.m_nextWaypoint);
      this.StateQueue.Enqueue(RonchMaster.RonchState.TurnToFacing);
      this.StateQueue.Enqueue(RonchMaster.RonchState.MoveToNode);
    }

    private void QueueWeaponFiringBasedOnDistance()
    {
      if ((double) Random.Range(0.0f, 1f) > 0.5)
        this.StateQueue.Enqueue(RonchMaster.RonchState.FireBackMissiles);
      else
        this.StateQueue.Enqueue(RonchMaster.RonchState.FireGau8s);
    }

    private RonchWaypoint GetRandomWaypoint() => this.Waypoints[Random.Range(0, this.Waypoints.Count)];

    private RonchWaypoint GetRandomNeighbor()
    {
      List<RonchWaypoint> ronchWaypointList = new List<RonchWaypoint>();
      for (int index = 0; index < this.m_curWaypoint.Neighbors.Count; ++index)
        ronchWaypointList.Add(this.m_curWaypoint.Neighbors[index]);
      if (ronchWaypointList.Contains(this.m_lastWaypoint))
        ronchWaypointList.Remove(this.m_lastWaypoint);
      return ronchWaypointList.Count > 0 ? ronchWaypointList[Random.Range(0, ronchWaypointList.Count)] : this.m_curWaypoint.Neighbors[Random.Range(0, this.m_curWaypoint.Neighbors.Count)];
    }

    private RonchWaypoint GetClosestWaypointToEnemy() => this.Waypoints[Random.Range(0, this.Waypoints.Count)];

    private void SetTargetFacingDir(RonchWaypoint wp)
    {
      Vector3 vector3 = wp.transform.position - this.m_curWaypoint.transform.position;
      vector3.Normalize();
      this.m_targetFacingDir = vector3;
    }

    private void SetTargetFacingDir(Vector3 targetPoint)
    {
      Vector3 vector3 = targetPoint - this.Mover.transform.position;
      vector3.Normalize();
      this.m_targetFacingDir = vector3;
    }

    private void SetState(RonchMaster.RonchState s)
    {
      this.CurState = s;
      switch (this.CurState)
      {
        case RonchMaster.RonchState.TurnToFacing:
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Turning, this.HeadLower.position);
          this.m_turningLerp = 0.0f;
          break;
        case RonchMaster.RonchState.MoveToNode:
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Moving, this.HeadLower.position);
          break;
        case RonchMaster.RonchState.FireBackMissiles:
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Firing, this.HeadLower.position);
          this.MissileBank.SetTargetPos(this.TargetPoint);
          this.MissileBank.BeginFiringSequence();
          break;
        case RonchMaster.RonchState.FireGau8s:
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Firing, this.HeadLower.position);
          for (int index = 0; index < this.Gau8s.Count; ++index)
            this.Gau8s[index].BeginFiringSequence();
          break;
        case RonchMaster.RonchState.Dead:
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Dead, this.HeadLower.position);
          this.MM.GronchDied();
          break;
      }
    }

    private void Update()
    {
      Vector3 forward = GM.CurrentPlayerBody.transform.position - this.Mover.transform.position;
      forward.y = 0.0f;
      this.LifeBarHolder.rotation = Quaternion.LookRotation(forward, Vector3.up);
      switch (this.CurState)
      {
        case RonchMaster.RonchState.None:
          this.DecideNextActions();
          break;
        case RonchMaster.RonchState.TurnToFacing:
          this.Update_TurnToFacing();
          break;
        case RonchMaster.RonchState.MoveToNode:
          this.Update_MoveToNode();
          break;
        case RonchMaster.RonchState.FireBackMissiles:
          this.Update_FirebackMissiles();
          break;
        case RonchMaster.RonchState.FireGau8s:
          this.Update_FireGau8s();
          break;
        case RonchMaster.RonchState.Dying:
          this.Update_Dying();
          break;
      }
      this.UpdateHealthState();
    }

    private void UpdateHealthState()
    {
      bool flag = true;
      float x = 1f;
      switch (this.DamState)
      {
        case RonchMaster.DamageStage.Raydome:
          if ((Object) this.RayDome != (Object) null)
          {
            x = Mathf.Clamp(this.RayDome.GetLifeRatio(), 0.01f, 1f);
            flag = true;
            break;
          }
          this.SetDamStateToCockpit();
          flag = true;
          break;
        case RonchMaster.DamageStage.Cockpit:
          if ((Object) this.Cockpit != (Object) null)
          {
            x = Mathf.Clamp(this.Cockpit.GetLifeRatio(), 0.01f, 1f);
            flag = true;
            if ((double) this.Cockpit.GetLifeRatio() <= 9.99999974737875E-05)
            {
              this.SetDamStateToDying();
              flag = false;
              break;
            }
            break;
          }
          break;
      }
      if (flag)
      {
        this.LifeBarHolder.gameObject.SetActive(true);
        this.LifeBar.localScale = new Vector3(x, 1f, 1f);
      }
      else
        this.LifeBarHolder.gameObject.SetActive(false);
    }

    private void SetDamStateToCockpit()
    {
      this.DamState = RonchMaster.DamageStage.Cockpit;
      this.Cockpit.SetCanTakeDamage(true);
      this.Entity_Cockpit.gameObject.SetActive(true);
      this.HeadLower.localEulerAngles = new Vector3(25f, 0.0f, 0.0f);
    }

    private void SetDamStateToDying()
    {
      this.DamState = RonchMaster.DamageStage.Dying;
      this.Cockpit.SetCanTakeDamage(false);
      this.Entity_Cockpit.gameObject.SetActive(false);
      this.SetState(RonchMaster.RonchState.Dying);
      this.m_TickDownDying = 10f;
      for (int index = 0; index < this.Gau8s.Count; ++index)
      {
        this.Gau8s[index].DestroyGun();
        this.Gau8s_Defensive[index].DestroyGun();
      }
      this.MissileBank.DestroyGun();
      this.FlashNades.DestroyGun();
      this.SmokeNades.DestroyGun();
      this.LaserNads.DestroyGun();
    }

    private void SetDamStateToDead() => this.DamState = RonchMaster.DamageStage.Dead;

    private void Update_TurnToFacing()
    {
      this.HeadUpdate();
      this.DefensiveWeaponUpdate();
      float num = Mathf.Clamp(this.Mover.UpdateWalking(Time.deltaTime * 2f) / Time.deltaTime, 0.0f, 8f);
      this.m_targetFacingDir.y = 0.0f;
      this.m_currentFacingDir = Vector3.RotateTowards(this.m_currentFacingDir, this.m_targetFacingDir, (float) ((double) Time.deltaTime * (double) num * 0.0799999982118607), 1f);
      this.m_currentFacingDir.y = 0.0f;
      this.Mover.SetFacing(this.m_currentFacingDir);
      if ((double) Vector3.Angle(this.m_currentFacingDir, this.m_targetFacingDir) < 1.0)
      {
        this.m_currentFacingDir = this.m_targetFacingDir;
        this.CurState = RonchMaster.RonchState.None;
        this.DecideNextActions();
      }
      Debug.DrawLine(this.Mover.transform.position, this.Mover.transform.position + this.m_currentFacingDir * 30f, Color.yellow);
      Debug.DrawLine(this.Mover.transform.position, this.Mover.transform.position + this.m_targetFacingDir * 30f, Color.red);
    }

    private void Update_MoveToNode()
    {
      this.HeadUpdate();
      this.DefensiveWeaponUpdate();
      if (!this.Mover.MoveTowardsPosition(this.m_nextWaypoint, Mathf.Clamp(this.Mover.UpdateWalking(Time.deltaTime * 2f) / Time.deltaTime, 0.0f, 8f) * 0.35f))
        return;
      this.CurState = RonchMaster.RonchState.None;
      this.m_curWaypoint = this.m_nextWaypoint;
      this.DecideNextActions();
    }

    private void Update_FirebackMissiles()
    {
      this.HeadUpdate();
      this.DefensiveWeaponUpdate();
    }

    private void Update_FireGau8s()
    {
      this.HeadUpdate();
      this.DefensiveWeaponUpdate();
    }

    private void Update_Dying()
    {
      this.m_TickDownDying -= Time.deltaTime;
      this.m_tickDownTilNextSplode -= Time.deltaTime;
      if ((double) this.m_tickDownTilNextSplode <= 0.0)
      {
        this.m_tickDownTilNextSplode = Random.Range(0.15f, 0.35f);
        for (int index = 0; index < this.ExplosionSetRandom.Count; ++index)
        {
          Object.Instantiate<GameObject>(this.ExplosionSetRandom[index], this.ExplosionPlaces[this.m_placeTick].position, this.ExplosionPlaces[this.m_placeTick].rotation);
          ++this.m_placeTick;
          if (this.m_placeTick >= this.ExplosionPlaces.Count)
            this.m_placeTick = 0;
        }
        this.Mover.Shudder();
      }
      if ((double) this.m_TickDownDying > 0.0)
        return;
      for (int index = 0; index < this.FinalExplosionPrefabs.Count; ++index)
        Object.Instantiate<GameObject>(this.FinalExplosionPrefabs[index], this.FinalExplosionPlace.position, Random.rotation);
      this.Mover.SetToDeathPose();
      this.SetState(RonchMaster.RonchState.Dead);
      this.HeadLower.gameObject.SetActive(false);
    }

    private void DefensiveWeaponUpdate()
    {
      Vector3 vector3 = this.TargetPoint.position - this.Laser_BaseDir.position;
      if ((double) vector3.magnitude < 50.0)
      {
        vector3.y = 0.0f;
        this.Laser_RotatingPart_Horizontal.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.Laser_RotatingPart_Horizontal.forward, vector3, Time.deltaTime * 2f, 1f), Vector3.up);
        this.Laser_RotatingPart_Vertical.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.Laser_RotatingPart_Vertical.forward, Vector3.RotateTowards(this.Laser_BaseDir.forward, Vector3.ProjectOnPlane(this.TargetPoint.position - this.Laser_RotatingPart_Vertical.position, this.Laser_RotatingPart_Horizontal.right), 1.047198f, 1f), Time.deltaTime * 2f, 1f), Vector3.up);
        if ((double) this.m_timeUntilFireOpenLaser > 0.0)
          this.m_timeUntilFireOpenLaser -= Time.deltaTime;
        else if ((double) Vector3.Angle(this.LaserNads.Muzzle.forward, vector3) < 25.0)
        {
          this.LaserNads.BeginFiringSequence();
          this.m_timeUntilFireOpenLaser = Random.Range(10f, 15f);
        }
      }
      Vector3 from1 = this.TargetPoint.position - this.Mover.transform.position;
      from1.y = 0.0f;
      if ((double) from1.magnitude > 40.0 && (double) Vector3.Angle(from1, this.Mover.transform.forward) < 45.0)
      {
        if ((double) this.m_timeUntilFireOpen > 0.0)
          this.m_timeUntilFireOpen -= Time.deltaTime;
        else if (this.m_isNextNadeFlash)
        {
          this.FlashNades.Muzzle.rotation = Quaternion.LookRotation(GM.CurrentPlayerBody.transform.position + Vector3.up * 6f - this.FlashNades.Muzzle.position, Vector3.up);
          this.FlashNades.BeginFiringSequence();
          this.m_isNextNadeFlash = false;
          this.m_timeUntilFireOpen = Random.Range(10f, 25f);
        }
        else
        {
          this.SmokeNades.Muzzle.rotation = Quaternion.LookRotation(GM.CurrentPlayerBody.transform.position + Vector3.up * 6f - this.SmokeNades.Muzzle.position, Vector3.up);
          this.SmokeNades.BeginFiringSequence();
          this.m_isNextNadeFlash = true;
          this.m_timeUntilFireOpen = Random.Range(10f, 25f);
        }
      }
      Vector3 from2 = this.TargetPoint.position - this.Head.position;
      from2.y = 0.0f;
      if ((double) Vector3.Angle(from2, this.Head.forward) >= 30.0 || (double) from2.magnitude <= 30.0 || (double) from2.magnitude >= 120.0)
        return;
      this.m_timeUntilNextDefensiveMachineGun -= Time.deltaTime;
      if ((double) this.m_timeUntilNextDefensiveMachineGun > 0.0)
        return;
      this.m_timeUntilNextDefensiveMachineGun = Random.Range(20f, 40f);
      for (int index = 0; index < this.Gau8s_Defensive.Count; ++index)
        this.Gau8s_Defensive[index].BeginFiringSequence();
    }

    private void HeadUpdate()
    {
      this.Head.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.Head.forward, Vector3.RotateTowards(this.BaseHeadPos.forward, this.TargetPoint.position - this.BaseHeadPos.position, 0.5235988f, 1f), Time.deltaTime, 1f), Vector3.up);
      if (this.DamState == RonchMaster.DamageStage.Raydome && (Object) this.RayDome != (Object) null)
      {
        if ((double) Vector3.Angle(this.RayDome.transform.forward, GM.CurrentPlayerBody.transform.position - this.RayDome.transform.position) >= 90.0 || Physics.Linecast(this.RayDome.RayDomeSightPose.position, GM.CurrentPlayerBody.Head.position, (int) this.LM_SightBlock, QueryTriggerInteraction.Ignore))
          return;
        this.TargetPoint.position = GM.CurrentPlayerBody.Head.position;
      }
      else
      {
        if (this.DamState != RonchMaster.DamageStage.Cockpit || !((Object) this.HeadLower != (Object) null) || ((double) Vector3.Angle(this.BaseHeadPos.transform.forward, GM.CurrentPlayerBody.transform.position - this.HeadLower.transform.position) >= 120.0 || Physics.Linecast(this.BaseHeadPos.position, GM.CurrentPlayerBody.Head.position, (int) this.LM_SightBlock, QueryTriggerInteraction.Ignore)))
          return;
        this.TargetPoint.position = GM.CurrentPlayerBody.Head.position;
      }
    }

    public void FiringComplete() => this.DecideNextActions();

    public void Dispose()
    {
      Object.Destroy((Object) this.Mover.gameObject);
      Object.Destroy((Object) this.TargetPoint.gameObject);
      Object.Destroy((Object) this.gameObject);
    }

    public enum RonchState
    {
      None = 0,
      Roar = 1,
      TurnToFacing = 10, // 0x0000000A
      MoveToNode = 11, // 0x0000000B
      LeapToNode = 12, // 0x0000000C
      FireRailgun = 20, // 0x00000014
      FireBackMissiles = 21, // 0x00000015
      FireGau8s = 22, // 0x00000016
      Dying = 30, // 0x0000001E
      Dead = 31, // 0x0000001F
    }

    public enum DamageStage
    {
      Raydome,
      Cockpit,
      Dying,
      Dead,
    }
  }
}
