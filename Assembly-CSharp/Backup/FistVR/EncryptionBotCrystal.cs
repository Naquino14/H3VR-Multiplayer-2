// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotCrystal
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotCrystal : MonoBehaviour
  {
    public bool IsBoss;
    public AIEntity E;
    public Rigidbody RB;
    public List<EncryptionBotCrystal.Crystal> Crystals;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> DestroyPoints;
    public float DamThreshold;
    public EncryptionBotCrystal.BotState State;
    private float m_activateTick;
    public float ActivateSpeed = 1f;
    public float DeactivateSpeed = 1f;
    public float CooldownSpeed = 1f;
    private float m_cooldownTick = 1f;
    public float ExplodingSpeed = 2f;
    private float m_explodingTick;
    public float DetonationRange = 10f;
    public float TalkRange = 250f;
    [Header("Targetting")]
    public AITargetPrioritySystem Priority;
    [Header("Audio")]
    public AudioEvent AudEvent_Passive;
    public AudioEvent AudEvent_Activating;
    public AudioEvent AudEvent_Deactivating;
    public AudioEvent AudEvent_Scream;
    public AudioEvent AudEvent_LaserChargeUp;
    public AudioSource AudSource_LaserHit;
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public ParticleSystem ExplodingParticles;
    public LayerMask LM_GroundCast;
    public Vector2 DesiredHeight = new Vector2(4f, 6f);
    private float m_desiredHeight = 4f;
    private float m_tickDownToSpeak = 1f;
    private bool m_hasPriority;
    private EncryptionBotCrystal.FireControlState m_FCState;
    private int m_numBots;
    public List<GameObject> BotPrefabs;
    private List<GameObject> m_spawnedBots = new List<GameObject>();
    public List<Transform> BotSpawnPositions;
    public List<Transform> BotConnectionBeams;
    public GameObject EnergyShield;
    private float m_TimeTilFCChange = 1f;
    private float m_spawnTickDown = 10f;
    private EncryptionBotCrystal.Crystal PulseCrystal1;
    private EncryptionBotCrystal.Crystal PulseCrystal2;
    private EncryptionBotCrystal.Crystal PulseCrystal3;
    private int howManyMorePulseCrystalShots = 6;
    private int whichPulseCrystalNext;
    private float PulseCrystalRefire = 0.2f;
    public GameObject PulseCrystalProjectilePrefab;
    public LayerMask LM_FireClear;
    public int NumShots = 1;
    public float ProjectileSpread = 0.2f;

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

    private void Start()
    {
      this.PrimeDics();
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.m_tickDownToSpeak = UnityEngine.Random.Range(5f, 20f);
      this.m_desiredHeight = UnityEngine.Random.Range(this.DesiredHeight.x, this.DesiredHeight.y);
      if (this.Priority != null)
      {
        this.m_hasPriority = true;
        this.Priority.Init(this.E, 5, 3f, 1.5f);
      }
      for (int index = 0; index < this.Crystals.Count; ++index)
        this.Crystals[index].WP.SetMC(this.Crystals[index]);
      if (!this.IsBoss)
        return;
      this.SpawnBotGroup();
    }

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    public void Regrow()
    {
      this.Crystals.Shuffle<EncryptionBotCrystal.Crystal>();
      for (int index = 0; index < this.Crystals.Count; ++index)
      {
        if (!this.Crystals[index].Alive)
        {
          this.Crystals[index].Regrow();
          break;
        }
      }
    }

    public void EventReceive(AIEvent e)
    {
      if (!e.IsEntity || e.Entity.IFFCode == this.E.IFFCode || e.Type != AIEvent.AIEType.Visual)
        return;
      this.Priority.ProcessEvent(e);
      this.m_cooldownTick = 1f;
      if (this.State != EncryptionBotCrystal.BotState.Deactivated)
        return;
      this.SetState(EncryptionBotCrystal.BotState.Activating);
    }

    private int GetNumActiveCrystals()
    {
      int num = 0;
      for (int index = 0; index < this.Crystals.Count; ++index)
      {
        if (this.Crystals[index].Alive && this.Crystals[index].Active)
          ++num;
      }
      return num;
    }

    private void DeActivateAllCrystals()
    {
      for (int index = 0; index < this.Crystals.Count; ++index)
        this.Crystals[index].Deactivate();
    }

    private void ActivateAllCrystals()
    {
      for (int index = 0; index < this.Crystals.Count; ++index)
        this.Crystals[index].Activate();
    }

    public void CrystalHit(EncryptionBotCrystal.Crystal c, float amount)
    {
      if (!c.Alive || !c.Active || (double) Mathf.Max(0.0f, amount - this.DamThreshold) <= 0.0)
        return;
      c.Damage(amount);
    }

    private void ActivateRandomCrystal()
    {
      this.Crystals.Shuffle<EncryptionBotCrystal.Crystal>();
      for (int index = 0; index < this.Crystals.Count; ++index)
      {
        if (this.Crystals[index].Alive && !this.Crystals[index].Active)
        {
          this.Crystals[index].Activate();
          break;
        }
      }
    }

    private EncryptionBotCrystal.Crystal GetClosestToPoint(Vector3 p)
    {
      float num1 = 3000f;
      EncryptionBotCrystal.Crystal crystal = (EncryptionBotCrystal.Crystal) null;
      for (int index = 0; index < this.Crystals.Count; ++index)
      {
        if (this.Crystals[index].Alive && !this.Crystals[index].Active)
        {
          float num2 = Vector3.Distance(this.Crystals[index].GO_On.transform.position, p);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            crystal = this.Crystals[index];
          }
        }
      }
      return crystal;
    }

    private EncryptionBotCrystal.Crystal ActivateClosestInactiveCrystalToTarget()
    {
      this.Crystals.Shuffle<EncryptionBotCrystal.Crystal>();
      float num1 = 3000f;
      EncryptionBotCrystal.Crystal crystal = (EncryptionBotCrystal.Crystal) null;
      for (int index = 0; index < this.Crystals.Count; ++index)
      {
        if (this.Crystals[index].Alive && !this.Crystals[index].Active)
        {
          float num2 = Vector3.Distance(this.Crystals[index].GO_On.transform.position, this.Priority.GetTargetPoint());
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            crystal = this.Crystals[index];
          }
        }
      }
      crystal?.Activate();
      return crystal;
    }

    private void BotUpdate()
    {
      if (!this.IsBoss)
        return;
      if (this.m_spawnedBots.Count > 0)
      {
        for (int index = this.m_spawnedBots.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.m_spawnedBots[index] == (UnityEngine.Object) null)
            this.m_spawnedBots.RemoveAt(index);
        }
      }
      for (int index = 0; index < this.m_spawnedBots.Count && !((UnityEngine.Object) this.m_spawnedBots[index] == (UnityEngine.Object) null); ++index)
      {
        EncryptionBotCrystal.Crystal closestToPoint = this.GetClosestToPoint(this.m_spawnedBots[index].transform.position);
        Vector3 position = this.transform.position;
        if (closestToPoint != null && (UnityEngine.Object) closestToPoint.GO_On != (UnityEngine.Object) null)
          position = closestToPoint.GO_On.transform.position;
        Vector3 forward = this.m_spawnedBots[index].transform.position - position;
        this.BotConnectionBeams[index].transform.position = position;
        this.BotConnectionBeams[index].transform.localScale = new Vector3(1f, 1f, forward.magnitude);
        this.BotConnectionBeams[index].gameObject.SetActive(true);
        this.BotConnectionBeams[index].transform.rotation = Quaternion.LookRotation(forward);
      }
      if (this.m_spawnedBots.Count < 3 || (UnityEngine.Object) this.m_spawnedBots[2] == (UnityEngine.Object) null)
        this.BotConnectionBeams[2].gameObject.SetActive(false);
      if (this.m_spawnedBots.Count < 2 || (UnityEngine.Object) this.m_spawnedBots[1] == (UnityEngine.Object) null)
        this.BotConnectionBeams[1].gameObject.SetActive(false);
      if (this.m_spawnedBots.Count < 1 || (UnityEngine.Object) this.m_spawnedBots[0] == (UnityEngine.Object) null)
        this.BotConnectionBeams[0].gameObject.SetActive(false);
      this.m_numBots = this.m_spawnedBots.Count;
      if (this.m_numBots > 0 || !this.EnergyShield.activeSelf)
        return;
      this.LowerShield();
    }

    private void SpawnBotGroup()
    {
      this.BotSpawnPositions.Shuffle<Transform>();
      bool flag = false;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        if (this.BotSpawnPositions.Count > 0)
        {
          for (int index2 = this.BotSpawnPositions.Count - 1; index2 >= 0; --index2)
          {
            this.SpawnBot(this.BotSpawnPositions[index1]);
            flag = true;
            this.BotSpawnPositions.RemoveAt(index1);
            if (this.m_spawnedBots.Count >= 3)
              break;
          }
        }
        if (this.m_spawnedBots.Count >= 3)
          break;
      }
      if (!flag)
        return;
      this.RaiseShield();
    }

    private void RaiseShield() => this.EnergyShield.SetActive(true);

    private void LowerShield() => this.EnergyShield.SetActive(false);

    private void SpawnBot(Transform point) => this.m_spawnedBots.Add(UnityEngine.Object.Instantiate<GameObject>(this.BotPrefabs[UnityEngine.Random.Range(0, this.BotPrefabs.Count)], point.position, point.rotation));

    private void FireControl()
    {
      if (this.m_FCState == EncryptionBotCrystal.FireControlState.Thinking)
      {
        this.m_TimeTilFCChange -= Time.deltaTime;
        if ((double) this.m_TimeTilFCChange > 0.0)
          return;
        float num = UnityEngine.Random.Range(0.0f, 1f);
        if (this.IsBoss && this.m_numBots <= 0 && this.BotSpawnPositions.Count > 0)
          this.SetFCState(EncryptionBotCrystal.FireControlState.BotSpawn);
        else if ((double) num >= 0.0)
          this.SetFCState(EncryptionBotCrystal.FireControlState.Pulses);
        else
          this.SetFCState(EncryptionBotCrystal.FireControlState.BeamUppercut);
      }
      else if (this.m_FCState == EncryptionBotCrystal.FireControlState.Pulses)
      {
        if (this.howManyMorePulseCrystalShots <= 0)
          this.SetFCState(EncryptionBotCrystal.FireControlState.Thinking);
        this.PulseCrystalRefire -= Time.deltaTime;
        if ((double) this.PulseCrystalRefire > 0.0)
          return;
        if (this.whichPulseCrystalNext == 0 && this.PulseCrystal1 != null)
          this.FirePulseShot(this.PulseCrystal1);
        if (this.whichPulseCrystalNext == 1 && this.PulseCrystal2 != null)
          this.FirePulseShot(this.PulseCrystal2);
        if (this.whichPulseCrystalNext == 2 && this.PulseCrystal3 != null)
          this.FirePulseShot(this.PulseCrystal3);
        --this.howManyMorePulseCrystalShots;
        ++this.whichPulseCrystalNext;
        if (this.whichPulseCrystalNext > 2)
          this.whichPulseCrystalNext = 0;
        this.PulseCrystalRefire = UnityEngine.Random.Range(0.2f, 0.8f);
        this.PulseCrystalRefire *= this.PulseCrystalRefire;
      }
      else if (this.m_FCState == EncryptionBotCrystal.FireControlState.BotSpawn)
      {
        this.m_spawnTickDown -= Time.deltaTime;
        if ((double) this.m_spawnTickDown > 0.0)
          return;
        this.SpawnBotGroup();
        this.SetFCState(EncryptionBotCrystal.FireControlState.Thinking);
      }
      else if (this.m_FCState != EncryptionBotCrystal.FireControlState.BeamUppercut)
        ;
    }

    private void FirePulseShot(EncryptionBotCrystal.Crystal c)
    {
      if (!c.Alive)
        return;
      Vector3 forward = this.Priority.GetTargetPoint() - c.GO_Off.transform.position;
      if (Physics.Raycast(c.GO_On.transform.position, forward.normalized, 4f, (int) this.LM_FireClear))
        return;
      this.RB.AddForceAtPosition(-forward.normalized * UnityEngine.Random.Range(0.2f, 2f), c.GO_Off.transform.position, ForceMode.VelocityChange);
      SM.GetSoundTravelDistanceMultByEnvironment(this.PlayShotEvent(c.GO_Off.transform.position));
      for (int index = 0; index < this.NumShots; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.PulseCrystalProjectilePrefab, c.GO_Off.transform.position, Quaternion.LookRotation(forward));
        gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(-this.ProjectileSpread, this.ProjectileSpread), UnityEngine.Random.Range(-this.ProjectileSpread, this.ProjectileSpread), 0.0f));
        BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
        component.FlightVelocityMultiplier = UnityEngine.Random.Range(0.4f, 0.5f);
        float muzzleVelocityBase = component.MuzzleVelocityBase;
        component.Fire(muzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) null);
        component.SetSource_IFF(this.E.IFFCode);
      }
      FXM.InitiateMuzzleFlash(c.GO_Off.transform.position, UnityEngine.Random.onUnitSphere, 4f, new Color(1f, 0.1f, 0.1f), 8f);
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

    private void SetFCState(EncryptionBotCrystal.FireControlState fcs)
    {
      this.m_FCState = fcs;
      switch (fcs)
      {
        case EncryptionBotCrystal.FireControlState.Thinking:
          this.m_TimeTilFCChange = UnityEngine.Random.Range(1.5f, 6f);
          this.DeActivateAllCrystals();
          break;
        case EncryptionBotCrystal.FireControlState.Pulses:
          this.PulseCrystalRefire = UnityEngine.Random.Range(0.8f, 2.5f);
          this.howManyMorePulseCrystalShots = 0;
          this.PulseCrystal1 = this.ActivateClosestInactiveCrystalToTarget();
          if (this.PulseCrystal1 != null)
            this.howManyMorePulseCrystalShots += UnityEngine.Random.Range(2, 4);
          this.PulseCrystal2 = this.ActivateClosestInactiveCrystalToTarget();
          if (this.PulseCrystal1 != null)
            this.howManyMorePulseCrystalShots += UnityEngine.Random.Range(1, 5);
          this.PulseCrystal3 = this.ActivateClosestInactiveCrystalToTarget();
          if (this.PulseCrystal1 != null)
            this.howManyMorePulseCrystalShots += UnityEngine.Random.Range(1, 4);
          this.ActivateRandomCrystal();
          this.ActivateRandomCrystal();
          break;
        case EncryptionBotCrystal.FireControlState.BotSpawn:
          this.m_spawnTickDown = 5f;
          this.ActivateAllCrystals();
          break;
      }
    }

    private void Update()
    {
      if (this.State != EncryptionBotCrystal.BotState.Exploding)
        this.Priority.Compute();
      this.BotUpdate();
      ParticleSystem.EmissionModule emission = this.ExplodingParticles.emission;
      int num = 0;
      for (int index = 0; index < this.Crystals.Count; ++index)
      {
        if (this.Crystals[index].Alive)
          ++num;
      }
      if (num <= 0)
        this.Explode();
      switch (this.State)
      {
        case EncryptionBotCrystal.BotState.Deactivated:
          this.m_tickDownToSpeak -= Time.deltaTime;
          if ((double) this.m_tickDownToSpeak <= 0.0)
          {
            this.m_tickDownToSpeak = UnityEngine.Random.Range(8f, 20f);
            if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 80.0)
              SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Passive, this.transform.position).FollowThisTransform(this.transform);
            emission.rateOverTimeMultiplier = 0.0f;
          }
          if (!Physics.Raycast(this.transform.position, -Vector3.up, this.m_desiredHeight, (int) this.LM_GroundCast))
            break;
          this.RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
          break;
        case EncryptionBotCrystal.BotState.Activating:
          this.m_activateTick += Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick >= 1.0)
            this.SetState(EncryptionBotCrystal.BotState.Activated);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotCrystal.BotState.Activated:
          this.m_cooldownTick -= Time.deltaTime * this.CooldownSpeed;
          this.FireControl();
          if ((double) this.m_cooldownTick <= 0.0)
            this.SetState(EncryptionBotCrystal.BotState.Deactivating);
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotCrystal.BotState.Deactivating:
          this.m_activateTick -= Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick <= 0.0)
          {
            this.m_activateTick = 0.0f;
            this.SetState(EncryptionBotCrystal.BotState.Deactivated);
          }
          emission.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotCrystal.BotState.Exploding:
          emission.rateOverTimeMultiplier = 80f;
          this.m_explodingTick += Time.deltaTime * this.ExplodingSpeed;
          if ((double) this.m_explodingTick < 1.0)
            break;
          this.Shatter();
          break;
      }
    }

    private void SetState(EncryptionBotCrystal.BotState S)
    {
      if (this.State == EncryptionBotCrystal.BotState.Exploding || this.State == S)
        return;
      this.State = S;
      switch (this.State)
      {
        case EncryptionBotCrystal.BotState.Deactivated:
          this.m_activateTick = 0.0f;
          break;
        case EncryptionBotCrystal.BotState.Activating:
          this.m_activateTick = 0.0f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 250.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Activating, this.transform.position).FollowThisTransform(this.transform);
          break;
        case EncryptionBotCrystal.BotState.Activated:
          this.m_cooldownTick = 1f;
          this.m_activateTick = 1f;
          break;
        case EncryptionBotCrystal.BotState.Deactivating:
          this.m_activateTick = 1f;
          this.SetFCState(EncryptionBotCrystal.FireControlState.Thinking);
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 250.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Deactivating, this.transform.position).FollowThisTransform(this.transform);
          break;
      }
    }

    public void Explode()
    {
      if (this.State == EncryptionBotCrystal.BotState.Exploding)
        return;
      this.SetState(EncryptionBotCrystal.BotState.Exploding);
      this.m_explodingTick = 0.0f;
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Scream, this.transform.position).FollowThisTransform(this.transform);
    }

    private void Shatter()
    {
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
      {
        Rigidbody component = UnityEngine.Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.DestroyPoints[index].position, this.DestroyPoints[index].rotation).GetComponent<Rigidbody>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.AddExplosionForce((float) UnityEngine.Random.Range(1, 10), this.transform.position + UnityEngine.Random.onUnitSphere, 5f);
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void PrimeDics()
    {
      if (!((UnityEngine.Object) this.GunShotProfile != (UnityEngine.Object) null))
        return;
      for (int index1 = 0; index1 < this.GunShotProfile.ShotSets.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.GunShotProfile.ShotSets[index1].EnvironmentsUsed.Count; ++index2)
          this.m_shotDic.Add(this.GunShotProfile.ShotSets[index1].EnvironmentsUsed[index2], this.GunShotProfile.ShotSets[index1]);
      }
    }

    [Serializable]
    public class Crystal
    {
      public GameObject GO_Off;
      public GameObject GO_On;
      public GameObject GO_Phys;
      public GameObject Splode;
      public EncryptionBotCrystalWeakPoint WP;
      public bool Alive = true;
      public bool Active;
      public float Life = 5000f;

      public void Activate()
      {
        if (!this.Alive)
          return;
        this.Active = true;
        this.GO_On.SetActive(true);
        this.GO_Off.SetActive(false);
      }

      public void Deactivate()
      {
        if (!this.Alive)
          return;
        this.Active = false;
        this.GO_On.SetActive(false);
        this.GO_Off.SetActive(true);
      }

      public void Kill()
      {
        if (!this.Alive)
          return;
        UnityEngine.Object.Instantiate<GameObject>(this.Splode, this.GO_Off.transform.position, UnityEngine.Random.rotation);
        this.Alive = false;
        this.GO_Phys.SetActive(false);
        this.GO_On.SetActive(false);
        this.GO_Off.SetActive(false);
      }

      public void Damage(float d)
      {
        if (!this.Active || !this.Alive)
          return;
        this.Life -= d;
        if ((double) this.Life > 0.0)
          return;
        this.Kill();
      }

      public void Regrow()
      {
        if (this.Alive)
          return;
        this.Alive = true;
        this.Active = false;
        this.GO_On.SetActive(false);
        this.GO_Off.SetActive(true);
        this.GO_Phys.SetActive(true);
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

    public enum FireControlState
    {
      Thinking,
      Pulses,
      BeamUppercut,
      BotSpawn,
    }
  }
}
