// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotHardened
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotHardened : MonoBehaviour
  {
    public AIEntity E;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> SpawnOnDestroyPoints;
    public Rigidbody RB;
    public EncryptionBotHardened.BotState State;
    private float m_activateTick;
    public float ActivateSpeed = 1f;
    public float DeactivateSpeed = 1f;
    public float CooldownSpeed = 1f;
    private float m_cooldownTick = 1f;
    private float m_explodingTick;
    public float DetonationRange = 10f;
    public float MoveSpeed = 10f;
    [Header("Audio")]
    public AudioEvent AudEvent_Passive;
    public AudioEvent AudEvent_Activating;
    public AudioEvent AudEvent_Deactivating;
    public AudioEvent AudEvent_Scream;
    public AudioEvent AudEvent_Fire;
    public ParticleSystem ExplodingParticles;
    public LayerMask LM_GroundCast;
    private Vector2 DesiredHeight = new Vector2(3f, 4f);
    private float m_desiredHeight = 4f;
    private float m_curDesiredHeight = 4f;
    public List<Transform> OuterPieces = new List<Transform>();
    public Transform Muzzle;
    public GameObject Projectile;
    public float ProjectileSpread;
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    private float m_tickDownToSpeak = 1f;
    private int m_queuedShots;
    private float m_shotRefire = 0.05f;
    public GameObject Defense;
    public ParticleSystem MuzzleFire;
    private Vector3 latestTargetPos = Vector3.zero;
    private float moveTowardTick;
    private float m_respondToEventCooldown = 0.1f;

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

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

    private void Start()
    {
      this.PrimeDics();
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.m_tickDownToSpeak = Random.Range(5f, 20f);
      this.m_desiredHeight = Random.Range(this.DesiredHeight.x, this.DesiredHeight.y);
    }

    private void TestMe() => this.TargetSighted(this.transform.position + Random.onUnitSphere * 8f);

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    private void Fire()
    {
      --this.m_queuedShots;
      Vector3 vector3 = this.latestTargetPos - this.Muzzle.position;
      if (Physics.Raycast(this.Muzzle.position, vector3.normalized, vector3.magnitude, (int) this.LM_GroundCast))
        return;
      if ((Object) this.GunShotProfile != (Object) null)
        SM.GetSoundTravelDistanceMultByEnvironment(this.PlayShotEvent(this.Muzzle.position));
      for (int index = 0; index < 3; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.Projectile, this.Muzzle.position, this.Muzzle.rotation);
        gameObject.transform.Rotate(new Vector3(Random.Range(-this.ProjectileSpread, this.ProjectileSpread), Random.Range(-this.ProjectileSpread, this.ProjectileSpread), 0.0f));
        BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
        component.FlightVelocityMultiplier = 0.2f;
        float muzzleVelocityBase = component.MuzzleVelocityBase;
        component.Fire(muzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) null);
        component.SetSource_IFF(this.E.IFFCode);
      }
      this.MuzzleFire.Emit(2);
      FXM.InitiateMuzzleFlash(this.Muzzle.position, this.Muzzle.forward, 4f, new Color(1f, 0.1f, 0.1f), 8f);
    }

    private FVRSoundEnvironment PlayShotEvent(Vector3 source)
    {
      float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
      float delay = num / 343f;
      FVRSoundEnvironment environment = SM.GetReverbEnvironment(this.transform.position).Environment;
      wwBotWurstGunSoundConfig.BotGunShotSet shotSet = this.GetShotSet(environment);
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
      return environment;
    }

    private void Update()
    {
      if ((double) this.m_respondToEventCooldown > 0.0)
        this.m_respondToEventCooldown -= Time.deltaTime;
      ParticleSystem.EmissionModule emission = this.ExplodingParticles.emission;
      if (Physics.Raycast(this.transform.position, -Vector3.up, this.m_curDesiredHeight, (int) this.LM_GroundCast))
        this.RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
      switch (this.State)
      {
        case EncryptionBotHardened.BotState.Deactivated:
          this.m_tickDownToSpeak -= Time.deltaTime;
          if ((double) this.m_tickDownToSpeak <= 0.0)
          {
            this.m_tickDownToSpeak = Random.Range(8f, 20f);
            if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 50.0)
              SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Passive, this.transform.position).FollowThisTransform(this.transform);
            this.m_curDesiredHeight = this.m_desiredHeight;
            break;
          }
          break;
        case EncryptionBotHardened.BotState.Activating:
          this.m_activateTick += Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick >= 1.0)
          {
            this.m_activateTick = 1f;
            this.SetState(EncryptionBotHardened.BotState.Activated);
          }
          for (int index = 0; index < this.OuterPieces.Count; ++index)
          {
            float num = Mathf.Lerp(1.1f, 1f, this.m_activateTick);
            this.OuterPieces[index].localScale = new Vector3(num, num, num);
          }
          this.m_curDesiredHeight = this.m_desiredHeight * this.m_desiredHeight;
          this.RB.rotation = Quaternion.Slerp(this.RB.rotation, Quaternion.LookRotation((this.latestTargetPos - this.transform.position).normalized), Time.deltaTime * 8f);
          break;
        case EncryptionBotHardened.BotState.Activated:
          this.m_cooldownTick -= Time.deltaTime * this.CooldownSpeed;
          if ((double) this.m_cooldownTick <= 0.0)
            this.SetState(EncryptionBotHardened.BotState.Deactivating);
          if (this.m_queuedShots > 0)
          {
            this.m_shotRefire -= Time.deltaTime;
            if ((double) this.m_shotRefire < 0.0)
            {
              this.m_shotRefire = Random.Range(0.08f, 0.22f);
              this.Fire();
            }
          }
          this.RB.rotation = Quaternion.Slerp(this.RB.rotation, Quaternion.LookRotation((this.latestTargetPos - this.transform.position).normalized), Time.deltaTime * 8f);
          this.m_curDesiredHeight = this.m_desiredHeight * this.m_desiredHeight;
          break;
        case EncryptionBotHardened.BotState.Deactivating:
          this.m_activateTick -= Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick <= 0.0)
          {
            this.m_activateTick = 0.0f;
            this.SetState(EncryptionBotHardened.BotState.Deactivated);
          }
          for (int index = 0; index < this.OuterPieces.Count; ++index)
          {
            float num = Mathf.Lerp(1.1f, 1f, this.m_activateTick);
            this.OuterPieces[index].localScale = new Vector3(num, num, num);
          }
          this.m_curDesiredHeight = this.m_desiredHeight;
          break;
        case EncryptionBotHardened.BotState.Exploding:
          emission.rateOverTimeMultiplier = 80f;
          this.m_explodingTick += Time.deltaTime * 2f;
          if ((double) this.m_explodingTick >= 1.0)
          {
            this.Shatter();
            break;
          }
          break;
      }
      if ((double) this.moveTowardTick <= 0.0)
        return;
      this.moveTowardTick -= Time.deltaTime;
    }

    public void EventReceive(AIEvent e)
    {
      if ((double) this.m_respondToEventCooldown >= 0.100000001490116 || e.IsEntity && e.Entity.IFFCode == this.E.IFFCode)
        return;
      this.TargetSighted(e.Pos);
    }

    private void TargetSighted(Vector3 v)
    {
      if (this.State == EncryptionBotHardened.BotState.Deactivating)
        return;
      this.latestTargetPos = v;
      this.moveTowardTick = 1f;
      this.m_queuedShots = 3;
      if (this.State == EncryptionBotHardened.BotState.Deactivated)
      {
        this.SetState(EncryptionBotHardened.BotState.Activating);
      }
      else
      {
        if (this.State != EncryptionBotHardened.BotState.Activated && this.State != EncryptionBotHardened.BotState.Activating || (double) Vector3.Distance(v, this.transform.position) > (double) this.DetonationRange)
          return;
        this.Explode();
      }
    }

    private void SetState(EncryptionBotHardened.BotState S)
    {
      if (this.State == EncryptionBotHardened.BotState.Exploding || this.State == S)
        return;
      this.State = S;
      switch (this.State)
      {
        case EncryptionBotHardened.BotState.Deactivated:
          this.m_activateTick = 0.0f;
          this.Defense.SetActive(true);
          break;
        case EncryptionBotHardened.BotState.Activating:
          this.m_activateTick = 0.0f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 50.0)
            SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Activating, this.transform.position).FollowThisTransform(this.transform);
          this.Defense.SetActive(false);
          break;
        case EncryptionBotHardened.BotState.Activated:
          this.m_cooldownTick = 1f;
          this.m_activateTick = 1f;
          break;
        case EncryptionBotHardened.BotState.Deactivating:
          this.m_activateTick = 1f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 50.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Deactivating, this.transform.position).FollowThisTransform(this.transform);
          break;
      }
    }

    public void Explode()
    {
      if (this.State == EncryptionBotHardened.BotState.Exploding)
        return;
      this.SetState(EncryptionBotHardened.BotState.Exploding);
      this.m_explodingTick = 0.0f;
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Scream, this.transform.position).FollowThisTransform(this.transform);
    }

    private void Shatter()
    {
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
      {
        Rigidbody component = Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnOnDestroyPoints[index].position, this.SpawnOnDestroyPoints[index].rotation).GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
          component.AddExplosionForce((float) Random.Range(1, 10), this.transform.position + Random.onUnitSphere, 5f);
      }
      Object.Destroy((Object) this.gameObject);
    }

    public enum BotState
    {
      Deactivated,
      Activating,
      Activated,
      Deactivating,
      Exploding,
    }
  }
}
