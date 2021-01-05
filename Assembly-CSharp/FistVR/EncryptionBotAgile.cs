// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotAgile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotAgile : MonoBehaviour
  {
    public AIEntity E;
    public Rigidbody RB;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> DestroyPoints;
    public float PatrolRange_Lateral;
    public float PatrolHeightAddMax;
    public List<Vector3> PatrolPoints = new List<Vector3>();
    private int m_curPatrolPoint;
    public EncryptionBotAgile.BotState State;
    private float m_activateTick;
    public float ActivateSpeed = 1f;
    public float DeactivateSpeed = 1f;
    public float CooldownSpeed = 1f;
    private float m_cooldownTick = 1f;
    private float m_explodingTick;
    public float Speed_Patrol;
    [Header("Targetting")]
    public AITargetPrioritySystem Priority;
    public GameObject ScanBeamRoot;
    public List<Renderer> ScanBeams;
    public LayerMask LM_ScanBeam;
    private RaycastHit h;
    private Vector3 BeamForward = Vector3.zero;
    [Header("Audio")]
    public AudioEvent AudEvent_Passive;
    public AudioEvent AudEvent_Activating;
    public AudioEvent AudEvent_Deactivating;
    public AudioEvent AudEvent_Scream;
    public AudioEvent AudEvent_Evade;
    public List<Transform> Muzzles;
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public GameObject ProjectilePrefab;
    public ParticleSystem ExplodingParticles;
    public FVRFireArmRoundDisplayData RoundData;
    private float m_tickDownToSpeak = 1f;
    private bool m_hasPriority;
    private float m_refire = 0.1f;
    private bool canFire;
    private float m_EvasionTick = 1f;

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

    private void GeneratePatrolPoints()
    {
      for (int index = 0; index < 10; ++index)
      {
        Vector3 position = this.transform.position;
        position.y += Random.Range(0.0f, this.PatrolHeightAddMax);
        Vector3 onUnitSphere = Random.onUnitSphere;
        onUnitSphere.y = 0.0f;
        onUnitSphere.Normalize();
        this.PatrolPoints.Add(position + onUnitSphere * Random.Range(this.PatrolRange_Lateral * 0.5f, this.PatrolRange_Lateral));
      }
    }

    private void Start()
    {
      this.PrimeDics();
      this.GeneratePatrolPoints();
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.m_tickDownToSpeak = Random.Range(5f, 20f);
      if (this.Priority != null)
      {
        this.m_hasPriority = true;
        this.Priority.Init(this.E, 5, 3f, 1.5f);
      }
      this.BeamForward = this.ScanBeamRoot.transform.forward;
      this.transform.position += Vector3.up * Random.Range(300f, 500f);
    }

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    public void EventReceive(AIEvent e)
    {
      if (!e.IsEntity || e.Entity.IFFCode == this.E.IFFCode || ((Object) e.Entity == (Object) this.E || e.Type != AIEvent.AIEType.Visual))
        return;
      this.Priority.ProcessEvent(e);
      this.m_cooldownTick = 1f;
      if (this.State != EncryptionBotAgile.BotState.Patrol)
        return;
      this.SetState(EncryptionBotAgile.BotState.Activating);
    }

    public void Evade(Vector3 strikeDir)
    {
      if (this.State == EncryptionBotAgile.BotState.Evading)
        return;
      this.SetState(EncryptionBotAgile.BotState.Evading);
      this.Priority.ProcessEvent(new AIEvent(this.transform.position - strikeDir * 100f, AIEvent.AIEType.Visual, 1f));
    }

    private void FireControl()
    {
      if ((double) this.m_refire > 0.0)
      {
        this.m_refire -= Time.deltaTime;
        this.canFire = false;
      }
      else
        this.canFire = true;
      if (!this.canFire || !this.Priority.HasFreshTarget())
        return;
      this.Fire(this.GetClosestMuzzle(), this.Priority.GetTargetPoint());
    }

    private Transform GetClosestMuzzle()
    {
      float num1 = 3000f;
      Transform transform = (Transform) null;
      for (int index = 0; index < this.Muzzles.Count; ++index)
      {
        float num2 = Vector3.Distance(this.Priority.GetTargetPoint(), this.Muzzles[index].position);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          transform = this.Muzzles[index];
        }
      }
      return transform;
    }

    private void Fire(Transform Muzzle, Vector3 point)
    {
      this.m_refire = Random.Range(0.1f, 0.3f);
      float num = Mathf.Abs(this.RoundData.BulletDropCurve.Evaluate(Vector3.Distance(Muzzle.position, point) * (1f / 1000f)));
      point += Vector3.up * num;
      Vector3 forward = point - Muzzle.position;
      float max = 0.3f;
      for (int index = 0; index < 3; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.ProjectilePrefab, Muzzle.position, Quaternion.LookRotation(forward));
        gameObject.transform.Rotate(new Vector3(Random.Range(-max, max), Random.Range(-max, max), 0.0f));
        BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
        component.FlightVelocityMultiplier = Random.Range(0.4f, 0.5f);
        float muzzleVelocityBase = component.MuzzleVelocityBase;
        component.Fire(muzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) null);
        component.SetSource_IFF(this.E.IFFCode);
      }
    }

    private void FlyToCurrentPatrolPoint()
    {
      Vector3 forward = this.PatrolPoints[this.m_curPatrolPoint] - this.transform.position;
      this.RB.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 2f));
      this.RB.MovePosition(this.RB.position + forward.normalized * 10f * Time.deltaTime);
      if ((double) forward.magnitude < 20.0)
      {
        ++this.m_curPatrolPoint;
        if (this.m_curPatrolPoint >= this.PatrolPoints.Count)
          this.m_curPatrolPoint = 0;
      }
      this.UpdateScanBeams();
    }

    private void FlyToTarget()
    {
      Vector3 forward = this.Priority.GetTargetPoint() - this.transform.position;
      this.RB.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 2f));
      Vector3 vector3 = forward.normalized * 10f;
      if ((double) forward.magnitude > 30.0)
        this.RB.MovePosition(this.RB.position + vector3 * 2f * Time.deltaTime);
      this.UpdateScanBeams();
    }

    private void UpdateScanBeams()
    {
      for (int index = 0; index < this.ScanBeams.Count; ++index)
      {
        float num = 350f;
        if (Physics.Raycast(this.ScanBeams[index].transform.position, this.ScanBeams[index].transform.forward, out this.h, 350f, (int) this.LM_ScanBeam))
          num = this.h.distance + 0.2f;
        this.ScanBeams[index].transform.localScale = new Vector3(0.01f, 0.01f, num * 0.2f);
      }
    }

    private void UpdateScanBeamSystem()
    {
      Vector3 b = this.E.transform.forward;
      if (this.State == EncryptionBotAgile.BotState.Activating || this.State == EncryptionBotAgile.BotState.Activated)
        b = this.Priority.GetTargetPoint() - this.transform.position;
      this.BeamForward = Vector3.Slerp(this.BeamForward, b, Time.deltaTime * 4f);
      this.ScanBeamRoot.transform.rotation = Quaternion.LookRotation(this.BeamForward, Vector3.zero);
    }

    private void Update()
    {
      if (this.State != EncryptionBotAgile.BotState.Exploding)
        this.Priority.Compute();
      this.UpdateScanBeamSystem();
      ParticleSystem.EmissionModule emission = this.ExplodingParticles.emission;
      switch (this.State)
      {
        case EncryptionBotAgile.BotState.Patrol:
          this.FlyToCurrentPatrolPoint();
          this.m_tickDownToSpeak -= Time.deltaTime;
          if ((double) this.m_tickDownToSpeak > 0.0)
            break;
          this.m_tickDownToSpeak = Random.Range(8f, 20f);
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 300.0)
            SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Passive, this.transform.position).FollowThisTransform(this.transform);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotAgile.BotState.Activating:
          this.m_activateTick += Time.deltaTime * this.ActivateSpeed;
          this.FlyToCurrentPatrolPoint();
          if ((double) this.m_activateTick >= 1.0)
            this.SetState(EncryptionBotAgile.BotState.Activated);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotAgile.BotState.Activated:
          this.m_cooldownTick -= Time.deltaTime * this.CooldownSpeed;
          this.FlyToTarget();
          this.FireControl();
          if ((double) this.m_cooldownTick <= 0.0)
            this.SetState(EncryptionBotAgile.BotState.Deactivating);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotAgile.BotState.Deactivating:
          this.FlyToCurrentPatrolPoint();
          this.m_activateTick -= Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick <= 0.0)
          {
            this.m_activateTick = 0.0f;
            this.SetState(EncryptionBotAgile.BotState.Patrol);
          }
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotAgile.BotState.Exploding:
          emission.rateOverTimeMultiplier = 80f;
          this.m_explodingTick += Time.deltaTime * 2f;
          if ((double) this.m_explodingTick < 1.0)
            break;
          this.Shatter();
          break;
        case EncryptionBotAgile.BotState.Evading:
          emission.rateOverTimeMultiplier = 0.0f;
          this.m_EvasionTick -= Time.deltaTime;
          if ((double) this.m_EvasionTick >= 0.0)
            break;
          if (this.Priority.HasFreshTarget())
          {
            this.m_refire = 0.0f;
            this.SetState(EncryptionBotAgile.BotState.Activated);
            break;
          }
          this.SetState(EncryptionBotAgile.BotState.Patrol);
          break;
      }
    }

    private void SetState(EncryptionBotAgile.BotState S)
    {
      if (this.State == EncryptionBotAgile.BotState.Exploding || this.State == S)
        return;
      this.State = S;
      switch (this.State)
      {
        case EncryptionBotAgile.BotState.Patrol:
          this.ScanBeamRoot.SetActive(true);
          this.m_activateTick = 0.0f;
          this.RB.isKinematic = true;
          this.RB.useGravity = false;
          break;
        case EncryptionBotAgile.BotState.Activating:
          this.ScanBeamRoot.SetActive(true);
          this.m_activateTick = 0.0f;
          this.RB.isKinematic = true;
          this.RB.useGravity = false;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 350.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Activating, this.transform.position).FollowThisTransform(this.transform);
          break;
        case EncryptionBotAgile.BotState.Activated:
          this.ScanBeamRoot.SetActive(true);
          this.RB.isKinematic = true;
          this.RB.useGravity = false;
          this.m_cooldownTick = 1f;
          this.m_activateTick = 1f;
          break;
        case EncryptionBotAgile.BotState.Deactivating:
          this.ScanBeamRoot.SetActive(true);
          this.RB.isKinematic = true;
          this.RB.useGravity = false;
          this.m_activateTick = 1f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 350.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Deactivating, this.transform.position).FollowThisTransform(this.transform);
          break;
        case EncryptionBotAgile.BotState.Exploding:
          this.ScanBeamRoot.SetActive(false);
          this.RB.isKinematic = false;
          this.RB.useGravity = true;
          break;
        case EncryptionBotAgile.BotState.Evading:
          this.ScanBeamRoot.SetActive(false);
          this.RB.isKinematic = false;
          this.RB.useGravity = false;
          Vector3 onUnitSphere = Random.onUnitSphere;
          onUnitSphere.y = Mathf.Abs(onUnitSphere.y);
          onUnitSphere *= Random.Range(25f, 35f);
          this.RB.velocity = onUnitSphere;
          this.RB.angularVelocity = Random.onUnitSphere * (float) Random.Range(7, 15);
          this.m_EvasionTick = Random.Range(0.4f, 1.2f);
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 350.0)
            break;
          FVRPooledAudioSource pooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Evade, this.transform.position);
          if (!((Object) pooledAudioSource != (Object) null))
            break;
          pooledAudioSource.FollowThisTransform(this.transform);
          break;
      }
    }

    public void Explode()
    {
      if (this.State == EncryptionBotAgile.BotState.Exploding)
        return;
      this.SetState(EncryptionBotAgile.BotState.Exploding);
      this.m_explodingTick = 0.0f;
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Scream, this.transform.position).FollowThisTransform(this.transform);
    }

    private void Shatter()
    {
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
      {
        Rigidbody component = Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.DestroyPoints[index].position, this.DestroyPoints[index].rotation).GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
          component.AddExplosionForce((float) Random.Range(1, 10), this.transform.position + Random.onUnitSphere, 5f);
      }
      Object.Destroy((Object) this.gameObject);
    }

    public void OnCollisionEnter() => this.Explode();

    private void PrimeDics()
    {
      if (!((Object) this.GunShotProfile != (Object) null))
        return;
      for (int index1 = 0; index1 < this.GunShotProfile.ShotSets.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.GunShotProfile.ShotSets[index1].EnvironmentsUsed.Count; ++index2)
          this.m_shotDic.Add(this.GunShotProfile.ShotSets[index1].EnvironmentsUsed[index2], this.GunShotProfile.ShotSets[index1]);
      }
    }

    public enum BotState
    {
      Patrol,
      Activating,
      Activated,
      Deactivating,
      Exploding,
      Evading,
    }
  }
}
