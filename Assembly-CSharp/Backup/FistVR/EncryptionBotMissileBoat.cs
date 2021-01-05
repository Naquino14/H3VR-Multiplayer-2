// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotMissileBoat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotMissileBoat : MonoBehaviour
  {
    public AIEntity E;
    public Rigidbody RB;
    public GameObject MissilePrefab;
    public List<Transform> LauncherMuzzles;
    private List<bool> m_isLoaded = new List<bool>();
    private List<float> m_reloadTime = new List<float>();
    private List<StingerMissile> m_missiles = new List<StingerMissile>();
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> DestroyPoints;
    public EncryptionBotMissileBoat.BotState State;
    private float m_activateTick;
    public float ActivateSpeed = 1f;
    public float DeactivateSpeed = 1f;
    public float CooldownSpeed = 1f;
    private float m_cooldownTick = 1f;
    private float m_explodingTick;
    public float DetonationRange = 10f;
    [Header("Targetting")]
    public AITargetPrioritySystem Priority;
    [Header("Audio")]
    public AudioEvent AudEvent_Passive;
    public AudioEvent AudEvent_Activating;
    public AudioEvent AudEvent_Deactivating;
    public AudioEvent AudEvent_Scream;
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public ParticleSystem ExplodingParticles;
    public LayerMask LM_GroundCast;
    public Vector2 DesiredHeight = new Vector2(4f, 6f);
    private float m_desiredHeight = 4f;
    private float m_tickDownToSpeak = 1f;
    private bool m_hasPriority;
    private float m_refire = 0.5f;
    private bool canFire;

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

    private void Start()
    {
      this.PrimeDics();
      for (int index = 0; index < 3; ++index)
      {
        this.m_isLoaded.Add(false);
        this.m_missiles.Add((StingerMissile) null);
        this.m_reloadTime.Add(1f);
      }
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.m_tickDownToSpeak = Random.Range(5f, 20f);
      this.m_desiredHeight = Random.Range(this.DesiredHeight.x, this.DesiredHeight.y);
      if (this.Priority == null)
        return;
      this.m_hasPriority = true;
      this.Priority.Init(this.E, 5, 3f, 1.5f);
    }

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    public void EventReceive(AIEvent e)
    {
      if (!e.IsEntity || e.Entity.IFFCode == this.E.IFFCode || e.Type != AIEvent.AIEType.Visual)
        return;
      this.Priority.ProcessEvent(e);
      this.m_cooldownTick = 1f;
      if (this.State == EncryptionBotMissileBoat.BotState.Deactivated)
        this.SetState(EncryptionBotMissileBoat.BotState.Activating);
      if (!this.m_hasPriority)
        ;
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
      if (this.canFire && this.Priority.HasFreshTarget())
      {
        for (int index = 0; index < this.m_isLoaded.Count; ++index)
        {
          if (this.m_isLoaded[index])
          {
            this.Fire(index, this.Priority.GetTargetPoint());
            break;
          }
        }
      }
      for (int index1 = 0; index1 < this.m_isLoaded.Count; ++index1)
      {
        if (!this.m_isLoaded[index1])
        {
          List<float> reloadTime;
          int index2;
          (reloadTime = this.m_reloadTime)[index2 = index1] = reloadTime[index2] - Time.deltaTime;
          if ((double) this.m_reloadTime[index1] < 0.0)
            this.m_isLoaded[index1] = true;
        }
      }
    }

    private void Fire(int whichMuzzle, Vector3 point)
    {
      this.m_isLoaded[whichMuzzle] = false;
      this.m_reloadTime[whichMuzzle] = Random.Range(8f, 15f);
      this.m_refire = Random.Range(1.5f, 3f);
      GameObject gameObject = Object.Instantiate<GameObject>(this.MissilePrefab, this.LauncherMuzzles[whichMuzzle].position, this.LauncherMuzzles[whichMuzzle].rotation);
      gameObject.transform.Rotate(new Vector3((float) Random.Range(-2, 2), (float) Random.Range(-2, 2), 0.0f));
      StingerMissile component = gameObject.GetComponent<StingerMissile>();
      component.SetMotorPower(20f);
      component.SetMaxSpeed(20f);
      component.SetTurnSpeed(Random.Range(3.4f, 4f));
      if (this.Priority.IsTargetEntity())
        component.Fire(point, 20f);
      else
        component.Fire(point, 20f);
      this.m_missiles[whichMuzzle] = component;
    }

    private void Update()
    {
      if (this.State != EncryptionBotMissileBoat.BotState.Exploding)
        this.Priority.Compute();
      ParticleSystem.EmissionModule emission = this.ExplodingParticles.emission;
      switch (this.State)
      {
        case EncryptionBotMissileBoat.BotState.Deactivated:
          this.m_tickDownToSpeak -= Time.deltaTime;
          if ((double) this.m_tickDownToSpeak <= 0.0)
          {
            this.m_tickDownToSpeak = Random.Range(8f, 20f);
            if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 80.0)
              SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Passive, this.transform.position).FollowThisTransform(this.transform);
            emission.rateOverTimeMultiplier = 0.0f;
          }
          if (!Physics.Raycast(this.transform.position, -Vector3.up, this.m_desiredHeight, (int) this.LM_GroundCast))
            break;
          this.RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
          break;
        case EncryptionBotMissileBoat.BotState.Activating:
          this.m_activateTick += Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick >= 1.0)
            this.SetState(EncryptionBotMissileBoat.BotState.Activated);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotMissileBoat.BotState.Activated:
          this.m_cooldownTick -= Time.deltaTime * this.CooldownSpeed;
          this.FireControl();
          if ((double) this.m_cooldownTick <= 0.0)
            this.SetState(EncryptionBotMissileBoat.BotState.Deactivating);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotMissileBoat.BotState.Deactivating:
          this.m_activateTick -= Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick <= 0.0)
          {
            this.m_activateTick = 0.0f;
            this.SetState(EncryptionBotMissileBoat.BotState.Deactivated);
          }
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotMissileBoat.BotState.Exploding:
          emission.rateOverTimeMultiplier = 80f;
          this.m_explodingTick += Time.deltaTime * 2f;
          if ((double) this.m_explodingTick < 1.0)
            break;
          this.Shatter();
          break;
      }
    }

    private void SetState(EncryptionBotMissileBoat.BotState S)
    {
      if (this.State == EncryptionBotMissileBoat.BotState.Exploding || this.State == S)
        return;
      this.State = S;
      switch (this.State)
      {
        case EncryptionBotMissileBoat.BotState.Deactivated:
          this.m_activateTick = 0.0f;
          break;
        case EncryptionBotMissileBoat.BotState.Activating:
          this.m_activateTick = 0.0f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 250.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Activating, this.transform.position).FollowThisTransform(this.transform);
          break;
        case EncryptionBotMissileBoat.BotState.Activated:
          this.m_cooldownTick = 1f;
          this.m_activateTick = 1f;
          break;
        case EncryptionBotMissileBoat.BotState.Deactivating:
          this.m_activateTick = 1f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 250.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Deactivating, this.transform.position).FollowThisTransform(this.transform);
          break;
      }
    }

    public void Explode()
    {
      if (this.State == EncryptionBotMissileBoat.BotState.Exploding)
        return;
      this.SetState(EncryptionBotMissileBoat.BotState.Exploding);
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
      Deactivated,
      Activating,
      Activated,
      Deactivating,
      Exploding,
    }
  }
}
