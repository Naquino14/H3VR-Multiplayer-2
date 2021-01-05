// Decompiled with JetBrains decompiler
// Type: FistVR.AIClunk
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIClunk : MonoBehaviour
  {
    private bool IsActivated = true;
    public AIBallJointTurret Turret;
    public AINavigator Navigator;
    public AISensorSystem SensorSystem;
    private AudioSource m_aud;
    public AudioClip AudClip_Die;
    public GameObject[] ExplosionPrefabs;
    public AIClunk.ClunkState State = AIClunk.ClunkState.None;
    public float ExplosionDelay = 3f;
    private float m_guardNewFacingTick;
    private float m_guardSwitchToPatrolTick = 10f;
    private float m_patrolNewFacingTick;
    private float m_patrolNewDestinationTick;
    private float m_patrolSwitchToGuardTick = 120f;
    public float MinEngagementRange = 10f;
    public float MaxEngagementRange = 30f;
    private bool m_evadeNextDirIsRandom;
    private float m_evadeNextDirTickDown = 1f;
    private float m_evadeNextMoveTickDown = 1f;
    public float EvadeCooldown = 20f;
    private float m_evadeGlobalCooldown = 20f;
    private float m_retreatNextDirTickDown = 1f;
    public float HuntingCooldown = 20f;
    private float m_huntTickDown = 20f;

    private void Awake()
    {
      this.m_aud = this.GetComponent<AudioSource>();
      this.SetState(AIClunk.ClunkState.Guard);
      this.Turret.SensorSystem = this.SensorSystem;
      this.m_huntTickDown = this.HuntingCooldown;
      this.m_evadeGlobalCooldown = this.EvadeCooldown;
    }

    private void Update() => this.UpdateClunk();

    private void UpdateClunk()
    {
      if (!this.IsActivated)
        return;
      this.StateControl();
      this.SensorControl();
      this.NavigatorControl();
    }

    public void Die()
    {
      if (!this.IsActivated)
        return;
      this.Navigator.SetPlateDisabled(true);
      this.Navigator.enabled = false;
      this.IsActivated = false;
      this.m_aud.PlayOneShot(this.AudClip_Die, 1f);
      this.Invoke("Explode", this.ExplosionDelay);
    }

    private void Explode()
    {
      for (int index = 0; index < this.ExplosionPrefabs.Length; ++index)
        Object.Instantiate<GameObject>(this.ExplosionPrefabs[index], this.transform.position + Vector3.up * 0.3f, Quaternion.identity);
      Object.Destroy((Object) this.gameObject);
    }

    private void SetState(AIClunk.ClunkState newState)
    {
      if (newState == this.State)
        return;
      Debug.Log((object) ("Entered State: " + newState.ToString()));
      this.State = newState;
      this.EnterState();
    }

    private void EnterState()
    {
      switch (this.State)
      {
        case AIClunk.ClunkState.Guard:
          this.EnterState_Guard();
          break;
        case AIClunk.ClunkState.Patrol:
          this.EnterState_Patrol();
          break;
        case AIClunk.ClunkState.Engage:
          this.EnterState_Engage();
          break;
        case AIClunk.ClunkState.Evade:
          this.EnterState_Evade();
          break;
        case AIClunk.ClunkState.Retreat:
          this.EnterState_Retreat();
          break;
        case AIClunk.ClunkState.Hunt:
          this.EnterState_Hunt();
          break;
      }
    }

    private void StateControl()
    {
      switch (this.State)
      {
        case AIClunk.ClunkState.Guard:
          this.State_Guard();
          break;
        case AIClunk.ClunkState.Patrol:
          this.State_Patrol();
          break;
        case AIClunk.ClunkState.Engage:
          this.State_Engage();
          break;
        case AIClunk.ClunkState.Evade:
          this.State_Evade();
          break;
        case AIClunk.ClunkState.Retreat:
          this.State_Retreat();
          break;
        case AIClunk.ClunkState.Hunt:
          this.State_Hunt();
          break;
      }
    }

    private void SensorControl()
    {
      if (!((Object) this.SensorSystem != (Object) null))
        return;
      this.SensorSystem.UpdateSensorSystem();
    }

    private void NavigatorControl()
    {
      if (!((Object) this.Navigator != (Object) null))
        return;
      this.Navigator.UpdateNavigationSystem();
    }

    private Vector3 GetRandomTurretFacing()
    {
      Vector3 vector3_1 = Vector3.zero;
      if ((Object) this.Turret != (Object) null)
      {
        Vector3 position = this.Turret.AimingTransform.position;
        Vector3 vector3_2 = this.Turret.AimingTransform.forward * Random.Range(-1f, 1f) + this.Turret.AimingTransform.right * Random.Range(-1f, 1f);
        vector3_2.Normalize();
        vector3_2 *= 6f;
        vector3_1 = position + vector3_2;
      }
      return vector3_1;
    }

    private void EnterState_Guard()
    {
      this.m_guardNewFacingTick = 0.0f;
      this.m_guardSwitchToPatrolTick = 10f;
      if (!((Object) this.Turret != (Object) null))
        return;
      this.Turret.SetFireAtWill(false);
    }

    private void State_Guard()
    {
      if ((Object) this.Turret != (Object) null)
      {
        if ((double) this.m_guardNewFacingTick <= 0.0)
        {
          this.m_guardNewFacingTick = Random.Range(3f, 6f);
          this.Turret.SetTargetPoint(this.GetRandomTurretFacing());
        }
        else
          this.m_guardNewFacingTick -= Time.deltaTime;
        if ((double) this.m_guardSwitchToPatrolTick <= 0.0)
        {
          this.SetState(AIClunk.ClunkState.Patrol);
        }
        else
        {
          this.m_guardSwitchToPatrolTick -= Time.deltaTime;
          if (!((Object) this.SensorSystem != (Object) null) || this.SensorSystem.PriorityTarget == null)
            return;
          Debug.Log((object) this.SensorSystem.PriorityTarget.Tr.gameObject.name);
          this.SetState(AIClunk.ClunkState.Engage);
        }
      }
      else
        this.SetState(AIClunk.ClunkState.Retreat);
    }

    private void EnterState_Patrol()
    {
      this.m_patrolNewFacingTick = 0.0f;
      this.m_patrolNewDestinationTick = 0.0f;
      this.m_patrolSwitchToGuardTick = 120f;
      if (!((Object) this.Turret != (Object) null))
        return;
      this.Turret.SetFireAtWill(false);
    }

    private void State_Patrol()
    {
      if ((Object) this.SensorSystem != (Object) null && this.SensorSystem.PriorityTarget != null)
        this.SetState(AIClunk.ClunkState.Engage);
      else if ((Object) this.Turret != (Object) null)
      {
        if ((double) this.m_patrolNewFacingTick <= 0.0)
        {
          this.m_patrolNewFacingTick = Random.Range(2f, 4f);
          this.Turret.SetTargetPoint(this.GetRandomTurretFacing());
        }
        else
          this.m_patrolNewFacingTick -= Time.deltaTime;
        if ((Object) this.Navigator != (Object) null)
        {
          if ((double) this.m_patrolNewDestinationTick <= 0.0 || this.Navigator.IsAtDestination)
          {
            this.m_patrolNewDestinationTick = Random.Range(18f, 30f);
            this.Navigator.SetMovementIntensity(0.15f);
            this.Navigator.SetNewNavDestination(this.Navigator.GetRandomNearDestination());
          }
          else
            this.m_patrolNewDestinationTick -= Time.deltaTime;
        }
        if ((double) this.m_patrolSwitchToGuardTick <= 0.0)
          this.SetState(AIClunk.ClunkState.Guard);
        else
          this.m_patrolSwitchToGuardTick -= Time.deltaTime;
      }
      else
        this.SetState(AIClunk.ClunkState.Retreat);
    }

    private void EnterState_Engage()
    {
      if (!((Object) this.Turret != (Object) null))
        return;
      this.Turret.SetFireAtWill(true);
    }

    private void State_Engage()
    {
      if ((Object) this.SensorSystem == (Object) null || (Object) this.Turret == (Object) null)
        this.SetState(AIClunk.ClunkState.Evade);
      else if (this.SensorSystem.PriorityTarget == null)
      {
        if (this.SensorSystem.LastPriorityTarget != null && (Object) this.SensorSystem.LastPriorityTarget.Tr != (Object) null)
          this.SetState(AIClunk.ClunkState.Hunt);
        else
          this.SetState(AIClunk.ClunkState.Evade);
      }
      else
      {
        this.Turret.SetTargetPoint(this.SensorSystem.PriorityTarget.LastKnownPosition);
        if (!((Object) this.Navigator != (Object) null))
          return;
        float a = Vector3.Distance(this.transform.position, this.SensorSystem.PriorityTarget.LastKnownPosition);
        float b = Vector3.Distance(this.Navigator.GetDestination(), this.SensorSystem.PriorityTarget.LastKnownPosition);
        float num = Mathf.Min(a, b);
        if ((double) num < (double) this.MinEngagementRange)
        {
          this.Navigator.SetNewNavDestination(this.Navigator.GetFurthestNearPointFrom(this.SensorSystem.PriorityTarget.LastKnownPosition));
          this.Navigator.SetMovementIntensity(1f);
        }
        if (!this.Navigator.IsAtDestination)
          return;
        if ((double) num < (double) this.MinEngagementRange)
        {
          this.Navigator.SetNewNavDestination(this.Navigator.GetFurthestNearPointFrom(this.SensorSystem.PriorityTarget.LastKnownPosition));
          this.Navigator.SetMovementIntensity(1f);
        }
        else if ((double) a > (double) this.MaxEngagementRange)
        {
          this.Navigator.SetNewNavDestination(this.Navigator.GetNearestNearPointFrom(this.SensorSystem.PriorityTarget.LastKnownPosition));
          this.Navigator.SetMovementIntensity(1f);
        }
        else
        {
          this.Navigator.SetNewNavDestination(this.Navigator.GetRandomNearDestination());
          this.Navigator.SetMovementIntensity(0.3f);
        }
      }
    }

    private void EnterState_Evade()
    {
      if ((Object) this.Turret != (Object) null)
        this.Turret.SetFireAtWill(true);
      this.m_evadeNextDirTickDown = 1f;
      this.m_evadeNextMoveTickDown = 1f;
      this.m_evadeGlobalCooldown = this.EvadeCooldown;
    }

    private void State_Evade()
    {
      if ((Object) this.Turret == (Object) null)
        this.SetState(AIClunk.ClunkState.Retreat);
      else if ((Object) this.Navigator == (Object) null || (double) this.m_evadeGlobalCooldown <= 0.0)
        this.SetState(AIClunk.ClunkState.Guard);
      else if ((Object) this.SensorSystem != (Object) null && this.SensorSystem.PriorityTarget != null)
      {
        this.SetState(AIClunk.ClunkState.Engage);
      }
      else
      {
        if ((double) this.m_evadeGlobalCooldown > 0.0)
          this.m_evadeGlobalCooldown -= Time.deltaTime;
        if ((double) this.m_evadeNextDirTickDown <= 0.0)
        {
          this.m_evadeNextDirTickDown = Random.Range(1f, 3f);
          if (this.m_evadeNextDirIsRandom || (Object) this.SensorSystem == (Object) null || this.SensorSystem.LastPriorityTarget == null)
          {
            Vector3 onUnitSphere = Random.onUnitSphere;
            onUnitSphere.y = 0.0f;
            onUnitSphere.Normalize();
            Vector3 vector3 = onUnitSphere * 8f;
            vector3.y = Random.Range(-2f, 2f);
            this.Turret.SetTargetPoint(vector3 + this.transform.position);
          }
          else
            this.Turret.SetTargetPoint(this.SensorSystem.LastPriorityTarget.LastKnownPosition);
          this.m_evadeNextDirIsRandom = !this.m_evadeNextDirIsRandom;
        }
        else
          this.m_evadeNextDirTickDown -= Time.deltaTime;
        if ((double) this.m_evadeNextMoveTickDown <= 0.0 || this.Navigator.IsAtDestination)
        {
          this.m_evadeNextMoveTickDown = Random.Range(3f, 5f);
          this.Navigator.SetMovementIntensity(1f);
        }
        else
          this.m_evadeNextMoveTickDown -= Time.deltaTime;
      }
    }

    private void EnterState_Retreat()
    {
      if ((Object) this.Turret != (Object) null)
        this.Turret.SetFireAtWill(false);
      this.m_retreatNextDirTickDown = 1f;
    }

    private void State_Retreat()
    {
      if ((Object) this.Turret != (Object) null)
      {
        this.m_retreatNextDirTickDown -= Time.deltaTime;
        if ((double) this.m_retreatNextDirTickDown <= 0.0)
        {
          this.m_retreatNextDirTickDown = Random.Range(3f, 5f);
          Vector3 onUnitSphere = Random.onUnitSphere;
          onUnitSphere.y = 0.0f;
          onUnitSphere.Normalize();
          onUnitSphere *= 8f;
          onUnitSphere.y = Random.Range(-2f, 2f);
          onUnitSphere += this.transform.position;
          this.Turret.SetTargetPoint(onUnitSphere);
        }
      }
      if (!((Object) this.Navigator != (Object) null) || !this.Navigator.IsAtDestination || (!((Object) this.SensorSystem != (Object) null) || this.SensorSystem.LastPriorityTarget == null))
        return;
      this.Navigator.TryToSetDestinationTo(this.SensorSystem.LastPriorityTarget.LastKnownPosition);
    }

    private void EnterState_Hunt()
    {
      if ((Object) this.Turret != (Object) null)
        this.Turret.SetFireAtWill(false);
      this.m_huntTickDown = this.HuntingCooldown;
    }

    private void State_Hunt()
    {
      if ((Object) this.SensorSystem == (Object) null || (Object) this.Turret == (Object) null)
        this.SetState(AIClunk.ClunkState.Evade);
      else if ((Object) this.Navigator == (Object) null || this.SensorSystem.LastPriorityTarget == null)
        this.SetState(AIClunk.ClunkState.Guard);
      else if (this.SensorSystem.PriorityTarget != null)
      {
        this.SetState(AIClunk.ClunkState.Engage);
      }
      else
      {
        this.Turret.SetTargetPoint(this.SensorSystem.LastPriorityTarget.LastKnownPosition);
        if ((double) Vector3.Distance(this.Navigator.GetDestination(), this.SensorSystem.LastPriorityTarget.LastKnownPosition) > 3.0)
          this.Navigator.TryToSetDestinationTo(this.SensorSystem.LastPriorityTarget.LastKnownPosition);
        this.m_huntTickDown -= Time.deltaTime;
        if (!this.Navigator.IsAtDestination && (double) this.m_huntTickDown > 0.0)
          return;
        this.SetState(AIClunk.ClunkState.Guard);
      }
    }

    public enum ClunkState
    {
      None = -1, // 0xFFFFFFFF
      Guard = 0,
      Patrol = 1,
      Engage = 2,
      Evade = 3,
      Retreat = 4,
      Hunt = 5,
    }
  }
}
