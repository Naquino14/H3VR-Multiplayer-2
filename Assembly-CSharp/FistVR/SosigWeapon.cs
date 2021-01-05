// Decompiled with JetBrains decompiler
// Type: FistVR.SosigWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class SosigWeapon : MonoBehaviour, IFVRDamageable
  {
    [Header("Sosig Weapon Stuff")]
    public int Quality = 1;
    private int m_baseQuality = 1;
    public SosigWeapon.SosigWeaponType Type;
    public bool UsesFakeType;
    public SosigWeapon.SosigWeaponType FakeType;
    public bool IsAlsoThrowable;
    private SosigWeapon.SosigWeaponType m_backupType;
    public bool PrimesOnFirstSwing;
    public bool IsDroppedOnStrongHit = true;
    public SosigWeapon.SosiggunAmmoType AmmoType;
    public float ShotLoudness;
    public Sosig SosigHoldingThis;
    public SosigHand HandHoldingThis;
    public Sosig SosigWithInventory;
    public SosigInventory.Slot InventorySlotWithThis;
    public FVRPhysicalObject O;
    public AIEntity E;
    public int SourceIFF;
    public bool IsHeldByBot;
    public bool IsInBotInventory;
    public Transform RecoilHolder;
    private List<Rigidbody> m_ignoreRBs = new List<Rigidbody>();
    public AudioImpactController ImpactController;
    private bool m_hasImpactController;
    [Header("Aiming Params")]
    public float Hipfire_HorizontalLimit = 10f;
    public float Hipfire_VerticalLimit = 60f;
    public float Aim_HorizontalLimit = 20f;
    public float Aim_VerticalLimit = 85f;
    public float MaxAngularFireRange = 5f;
    public AnimationCurve AngularFiringLimitByRange;
    public Vector2 RangeMinMax = new Vector2(1f, 100f);
    public Vector2 PreferredRange = new Vector2(0.0f, 10f);
    [Header("Gun Mechanics Params")]
    public SosigWeapon.SosigWeaponMechaState MechaState;
    public bool UsesSustainedSound;
    public AudioEvent AudEvent_SustainedEmit;
    public AudioEvent AudEvent_SustainedTerminate;
    public AudioSource AudSource_SustainedLoop;
    public float SustainEnergyPerShot = 0.2f;
    private float m_sustainEnergy;
    public Transform Muzzle;
    public Transform CyclingPart;
    public bool HasCyclingPart = true;
    public SosigWeapon.Axis CycleAxis;
    public SosigWeapon.InterpStyle CycleInterpStyle;
    public Vector2 CycleForwardBackValues;
    public float CycleSpeedForward = 1f;
    public float CycleSpeedBackward = 1f;
    private float m_cycleLerp;
    public int ShotsPerCycle = 1;
    private int m_shotsSoFarThisCycle;
    public int ShotsPerLoad = 5;
    public Vector2 AmmoClampRange = new Vector2(3f, 5f);
    private int m_shotsLeft = 5;
    public float FlightVelocityMultiplier = 1f;
    public GameObject Projectile;
    public int ProjectilesPerShot = 1;
    public float ProjectileSpread;
    public bool isFullAuto;
    public Vector4 RecoilPerShot = new Vector4(1f, 10f, 0.01f, 0.03f);
    private float m_recoilLinear;
    private float m_recoilX;
    private float m_recoilY;
    [Header("Melee Params")]
    public SosigWeapon.SosigMeleeWeaponType MeleeType;
    public List<SosigWeapon.SosigMeleeWeaponType> MeleeTypeCycleList;
    public bool DoesMeleeTypeCycle;
    private int m_meleeTypeCycle;
    private float m_tickDownToTypeCycle = 4f;
    public bool DoesHeightAdjust = true;
    public SosigWeapon.SosigMeleeState MeleeState;
    public Vector3 MeleeStateSpeeds = new Vector3(1f, 5f, 2f);
    public Vector2 CloseAttackRange = new Vector2(0.85f, 1f);
    public Vector2 DistantAttackRange = new Vector2(1.8f, 2.6f);
    private float m_sosigMeleeStateTick;
    private float m_sosigMeleeLerp;
    [Header("New Melee Params")]
    public List<SosigMeleeAnimationSet> AvailableIdleAnims;
    private SosigMeleeAnimationSet m_currentIdleAnim;
    public List<SosigMeleeAnimationSet> AvailableReadyAnims;
    private SosigMeleeAnimationSet m_currentReadyAnim;
    public List<SosigMeleeAnimationSet> AvailableAttackAnims;
    private SosigMeleeAnimationSet m_currentAttackAnim;
    [Header("ThrownMelee Params")]
    public bool DoesCastForPlayerHitBox;
    private bool m_isThrownTagCastTag;
    public LayerMask LM_Thrown;
    public Transform ThrownCastPos1;
    public Transform ThrownCastPos2;
    private RaycastHit m_thrownHit;
    [Header("Grenade Params")]
    public List<GameObject> SpawnOnExplode;
    public float GrenadeFuseTime = 3f;
    private float m_startFuseTime;
    private bool m_isFuseTickingDown;
    public SosigGrenadePin Pin;
    public NavMeshObstacle Obstacle;
    public bool UsesFusePulse;
    public AudioEvent AudEvent_FusePulse;
    public ParticleSystem FusePSystem;
    private float m_fuse_PitchStart = 1.8f;
    private float m_fuse_PitchEnd = 3.7f;
    private float m_fuse_StartRefire = 0.4f;
    private float m_fuse_EndRefire = 0.02f;
    [Header("Spawn Params")]
    [Header("Sosig Usage Params")]
    public SosigWeapon.SosigWeaponUsageState UsageState;
    public float ReloadTime = 1f;
    private float m_reloadingLerp;
    public Vector2 Usage_RefireRange = new Vector2(0.25f, 0.5f);
    private float m_refireTick = 1f;
    public bool UsesBurstLimit;
    public int BurstLimit = 3;
    public int BurstLimitRange;
    public Vector2 BurstDelayRange = new Vector2(0.1f, 0.25f);
    private int m_shotsLeftTilBurstLimit = 3;
    public LayerMask LM_IFFCheck;
    public LayerMask LM_EnvCheck;
    private RaycastHit m_hit;
    [Header("VFX Params")]
    public bool UsesMuzzleFire;
    public bool DoesFlashOnFire;
    public ParticleSystem[] PSystemsMuzzle;
    public int MuzzlePAmount;
    public GameObject Laser;
    private bool m_hasLaser;
    [Header("Audio Params")]
    public wwBotWurstGunSoundConfig GunShotProfile;
    private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();
    public float MinHandlingDistance = 15f;
    public bool UsesCyclingSounds;
    public bool UsesReloadingSounds;
    [Header("AutoDestroyLoop")]
    private bool m_setToAutoDestroy;
    private float m_autoDestroyTickDown = 30f;
    private bool m_ammoClampOnPlayerPickup;
    private bool m_hasClamped;
    [Header("Destructibility")]
    public bool IsDestructible;
    public List<GameObject> ShardPrefabs;
    public List<Transform> ShardPositions;
    public Vector2 CollisionLife = new Vector2(20f, 35f);
    private float m_collisionLife = 1f;
    private bool m_shouldShatter;
    private bool m_isShattered;
    public AudioEvent AudEvent_Shatter;
    public bool TakesDamage;
    public float Life = 10000f;
    private float m_startingLife = 10000f;
    public bool UsesDestructionStageRenderers;
    public Renderer[] DestructionRenderers;
    private int m_currentDestructionRenderer = -1;
    public Vector3 ResistMultBCP = new Vector3(1f, 1f, 1f);
    private bool m_shouldBeAbleToStab;
    public SosigWeapon.SosigWeaponLayerState LayerState;
    private float m_fuse_tick;
    private Vector3 lastPos;
    private bool m_hasExploded;
    private float m_idleTick;
    private float m_readyLoopTick;
    private float m_attackLerpTick;
    private Vector3 m_currentStateSpeeds = Vector3.one;
    private float checkHeightTick = 0.1f;
    private float heightOffset;
    private float attackHoldLimit = 1f;
    private float readyAbort = 1f;
    private float timeSinceFired;
    private float DamRefire = 0.1f;

    public SosigWeapon.SosigWeaponType GetMyType() => this.UsesFakeType ? this.FakeType : this.Type;

    public void SetAmmoClamping(bool b) => this.m_ammoClampOnPlayerPickup = b;

    protected void Start()
    {
      this.m_startingLife = this.Life;
      this.m_shotsLeft = this.ShotsPerLoad;
      this.PrimeDics();
      this.m_baseQuality = this.Quality;
      this.m_shotsLeftTilBurstLimit = this.BurstLimit;
      this.m_startFuseTime = this.GrenadeFuseTime;
      this.m_currentStateSpeeds = this.MeleeStateSpeeds;
      if ((Object) this.ImpactController != (Object) null)
        this.m_hasImpactController = true;
      if ((Object) this.Laser != (Object) null)
        this.m_hasLaser = true;
      if (this.AvailableIdleAnims.Count > 0)
        this.m_currentIdleAnim = this.AvailableIdleAnims[Random.Range(0, this.AvailableIdleAnims.Count)];
      if (this.AvailableReadyAnims.Count > 0)
        this.m_currentReadyAnim = this.AvailableReadyAnims[Random.Range(0, this.AvailableReadyAnims.Count)];
      if (this.AvailableAttackAnims.Count > 0)
        this.m_currentAttackAnim = this.AvailableAttackAnims[Random.Range(0, this.AvailableAttackAnims.Count)];
      this.m_collisionLife = Random.Range(this.CollisionLife.x, this.CollisionLife.y);
      this.m_backupType = this.Type;
      if (!this.O.MP.IsMeleeWeapon || !this.O.MP.CanNewStab)
        return;
      this.m_shouldBeAbleToStab = true;
    }

    private void UpdateDestructionRenderers()
    {
      int num = Mathf.Clamp(Mathf.RoundToInt((float) (1.0 - (double) this.Life / (double) this.m_startingLife) * (float) this.DestructionRenderers.Length), 0, this.DestructionRenderers.Length - 1);
      if (this.m_currentDestructionRenderer == num)
        return;
      this.m_currentDestructionRenderer = num;
      for (int index = 0; index < this.DestructionRenderers.Length; ++index)
        this.DestructionRenderers[index].enabled = index == this.m_currentDestructionRenderer;
    }

    public void BotPickup(Sosig S)
    {
      this.O.SetAllCollidersToLayer(false, "AgentMeleeWeapon");
      if (!this.O.MP.IsMeleeWeapon)
        return;
      for (int index = 0; index < S.Links.Count; ++index)
        this.O.MP.IgnoreRBs.Add(S.Links[index].R);
    }

    public void BotDrop()
    {
      if (!this.O.MP.IsMeleeWeapon)
        return;
      this.O.MP.IgnoreRBs.Clear();
    }

    public void PlayerPickup()
    {
      if (this.m_hasClamped)
        return;
      if (this.m_ammoClampOnPlayerPickup)
      {
        this.m_hasClamped = true;
        this.m_shotsLeft = Mathf.Clamp(this.m_shotsLeft, 0, Mathf.RoundToInt(Random.Range(this.AmmoClampRange.x, this.AmmoClampRange.y)));
      }
      if (!this.O.MP.IsMeleeWeapon)
        return;
      this.O.MP.IgnoreRBs.Clear();
    }

    public bool IsUsable() => !this.O.MP.IsJointedToObject && (this.Type != SosigWeapon.SosigWeaponType.Gun || this.m_shotsLeft > 0);

    public void SetIgnoreRBs(List<Rigidbody> rbs)
    {
      for (int index = 0; index < rbs.Count; ++index)
      {
        this.m_ignoreRBs.Add(rbs[index]);
        if (this.m_hasImpactController)
          this.ImpactController.IgnoreRBs.Add(rbs[index]);
      }
    }

    public void ClearRBs()
    {
      this.m_ignoreRBs.Clear();
      if (!this.m_hasImpactController)
        return;
      this.ImpactController.IgnoreRBs.Clear();
    }

    public void SetAutoDestroy(bool b) => this.m_setToAutoDestroy = b;

    public void Update()
    {
      if (this.m_shouldShatter)
        this.Shatter();
      if ((double) this.timeSinceFired < 10.0)
        this.timeSinceFired += 10f;
      if ((double) this.DamRefire > 0.0)
        this.DamRefire -= Time.deltaTime;
      if (this.O.IsHeld)
        this.m_autoDestroyTickDown = 30f;
      if (this.DoesCastForPlayerHitBox && this.m_isThrownTagCastTag && Physics.Linecast(this.ThrownCastPos1.position, this.ThrownCastPos2.position, out this.m_thrownHit, (int) this.LM_Thrown, QueryTriggerInteraction.Collide))
      {
        Debug.Log((object) "yup");
        this.m_isThrownTagCastTag = false;
        IFVRDamageable component = this.m_thrownHit.collider.gameObject.GetComponent<IFVRDamageable>();
        FistVR.Damage dam = new FistVR.Damage();
        if (component != null)
        {
          dam.Class = FistVR.Damage.DamageClass.Melee;
          dam.point = this.m_hit.point;
          dam.hitNormal = this.m_hit.normal;
          dam.strikeDir = this.O.RootRigidbody.velocity.normalized;
          dam.damageSize = 0.02f;
          float num = Mathf.Clamp(this.O.RootRigidbody.velocity.magnitude, 2f, 10f);
          switch (this.MeleeType)
          {
            case SosigWeapon.SosigMeleeWeaponType.Bladed:
              dam.Dam_Blunt = 5f * num;
              dam.Dam_Cutting = 75f * num;
              dam.Dam_Piercing = 5f * num;
              dam.Dam_TotalKinetic = 80f * num;
              break;
            case SosigWeapon.SosigMeleeWeaponType.Blunt:
              dam.Dam_Blunt = 35f * num;
              dam.Dam_Piercing = 10f * num;
              dam.Dam_TotalKinetic = 45f * num;
              break;
            case SosigWeapon.SosigMeleeWeaponType.Stabbing:
              dam.Dam_Blunt = 5f * num;
              dam.Dam_Piercing = 40f * num;
              dam.Dam_Cutting = 25f * num;
              dam.Dam_TotalKinetic = 75f * num;
              break;
            case SosigWeapon.SosigMeleeWeaponType.Shield:
              dam.Dam_Blunt = 45f * num;
              dam.Dam_Cutting = 10f * num;
              dam.Dam_Piercing = 5f * num;
              dam.Dam_TotalKinetic = 60f * num;
              break;
          }
          if (component != null)
          {
            component.Damage(dam);
            this.DamRefire = 0.1f;
          }
        }
      }
      if (this.UsesSustainedSound)
      {
        float sustainEnergy = this.m_sustainEnergy;
        this.m_sustainEnergy -= Time.deltaTime;
        if ((double) this.m_sustainEnergy <= 0.0 && (double) sustainEnergy > 0.0)
        {
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_SustainedTerminate, this.transform.position);
          if (this.AudSource_SustainedLoop.isPlaying)
            this.AudSource_SustainedLoop.Stop();
        }
      }
      if (this.m_shouldBeAbleToStab)
      {
        if (this.O.IsHeld || this.IsHeldByBot)
          this.O.MP.CanNewStab = true;
        if (this.IsInBotInventory)
        {
          this.O.MP.CanNewStab = false;
          this.O.MP.DeJoint();
        }
      }
      if (this.m_setToAutoDestroy && !this.O.IsHeld && (!this.IsHeldByBot && !this.IsInBotInventory) && (Object) this.O.QuickbeltSlot == (Object) null)
      {
        this.m_autoDestroyTickDown -= Time.deltaTime;
        if ((double) this.m_autoDestroyTickDown < 0.0)
          Object.Destroy((Object) this.gameObject);
      }
      float num1 = 1f;
      if (this.O.IsHeld)
        num1 = 0.2f;
      if (this.m_isFuseTickingDown)
      {
        this.GrenadeFuseTime -= Time.deltaTime * num1;
        if (this.UsesFusePulse)
        {
          float t = Mathf.Pow(Mathf.Clamp((float) (1.0 - (double) this.GrenadeFuseTime / (double) this.m_startFuseTime), 0.0f, 1f), 2f);
          if ((double) this.m_fuse_tick <= 0.0)
          {
            this.m_fuse_tick = Mathf.Lerp(this.m_fuse_StartRefire, this.m_fuse_EndRefire, t);
            float num2 = Mathf.Lerp(this.m_fuse_PitchStart, this.m_fuse_PitchEnd, t);
            SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.Generic, this.AudEvent_FusePulse, this.transform.position, new Vector2(0.3f, 0.3f), new Vector2(num2, num2), Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
            this.FusePSystem.Emit(2);
          }
          else
            this.m_fuse_tick -= Time.deltaTime;
        }
        if ((double) this.GrenadeFuseTime <= 0.0)
          this.Explode();
      }
      switch (this.LayerState)
      {
        case SosigWeapon.SosigWeaponLayerState.Agent:
          if (this.O.IsHeld)
          {
            this.O.SetAllCollidersToLayer(false, "Default");
            this.LayerState = SosigWeapon.SosigWeaponLayerState.Player;
            break;
          }
          break;
        case SosigWeapon.SosigWeaponLayerState.Player:
          if (!this.O.IsHeld && (Object) this.O.QuickbeltSlot == (Object) null)
          {
            this.LayerState = SosigWeapon.SosigWeaponLayerState.Agent;
            this.O.SetAllCollidersToLayer(false, "AgentMeleeWeapon");
            break;
          }
          break;
      }
      if ((this.O.IsHeld || (Object) this.O.QuickbeltSlot != (Object) null) && (this.IsHeldByBot && this.HandHoldingThis != null))
        this.HandHoldingThis.DropHeldObject();
      this.E.IFFCode = this.O.IsHeld || this.IsHeldByBot || (this.IsInBotInventory || (Object) this.O.QuickbeltSlot != (Object) null) || this.m_isFuseTickingDown ? -3 : -2;
      if (this.Type == SosigWeapon.SosigWeaponType.Gun)
      {
        this.Update_Gun();
        this.UpdateRecoil();
      }
      this.O.DistantGrabbable = !this.IsHeldByBot && !this.IsInBotInventory;
      if (this.m_hasLaser)
      {
        if (this.IsHeldByBot || this.O.IsHeld)
        {
          if (!this.Laser.activeSelf)
            this.Laser.SetActive(true);
        }
        else if (this.Laser.activeSelf)
          this.Laser.SetActive(false);
      }
      this.lastPos = this.transform.position;
    }

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
      for (int index = 0; index < this.SpawnOnExplode.Count; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SpawnOnExplode[index], this.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        Explosion component1 = gameObject.GetComponent<Explosion>();
        if ((Object) component1 != (Object) null)
          component1.IFF = this.SourceIFF;
        ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
        if ((Object) component2 != (Object) null)
          component2.IFF = this.SourceIFF;
        Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
        if ((Object) component3 != (Object) null)
          component3.velocity = this.O.RootRigidbody.velocity;
      }
      Object.Destroy((Object) this.gameObject);
    }

    public void FuseGrenade()
    {
      if (this.Type != SosigWeapon.SosigWeaponType.Grenade && !this.PrimesOnFirstSwing || this.m_isFuseTickingDown)
        return;
      this.Obstacle.enabled = true;
      this.m_isFuseTickingDown = true;
    }

    private void Update_Gun()
    {
      switch (this.MechaState)
      {
        case SosigWeapon.SosigWeaponMechaState.CyclingBack:
          this.m_cycleLerp += Time.deltaTime * this.CycleSpeedBackward;
          if (this.HasCyclingPart)
            this.SetAnimatedComponent(this.CyclingPart, Mathf.Lerp(this.CycleForwardBackValues.x, this.CycleForwardBackValues.y, this.m_cycleLerp), this.CycleInterpStyle, this.CycleAxis);
          if ((double) this.m_cycleLerp >= 1.0)
          {
            this.m_cycleLerp = 1f;
            if (this.m_shotsLeft <= 0)
            {
              if (!this.O.IsHeld && this.IsHeldByBot && this.UsageState != SosigWeapon.SosigWeaponUsageState.Reloading)
              {
                if (!this.SosigHoldingThis.Inventory.HasAmmo((int) this.AmmoType))
                {
                  this.HandHoldingThis.DropHeldObject();
                  return;
                }
                this.UsageState = SosigWeapon.SosigWeaponUsageState.Reloading;
                if (this.UsesReloadingSounds && this.GunShotProfile.Reloading.Clips.Count > 0 && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < (double) this.MinHandlingDistance)
                {
                  SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.Reloading, this.transform.position);
                  break;
                }
                break;
              }
              break;
            }
            if (this.UsesCyclingSounds && this.GunShotProfile.EjectionBack.Clips.Count > 0 && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < (double) this.MinHandlingDistance)
              SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.EjectionBack, this.transform.position);
            this.MechaState = SosigWeapon.SosigWeaponMechaState.CyclingForward;
            break;
          }
          break;
        case SosigWeapon.SosigWeaponMechaState.CyclingForward:
          this.m_cycleLerp -= Time.deltaTime * this.CycleSpeedForward;
          if (this.HasCyclingPart)
            this.SetAnimatedComponent(this.CyclingPart, Mathf.Lerp(this.CycleForwardBackValues.x, this.CycleForwardBackValues.y, this.m_cycleLerp), this.CycleInterpStyle, this.CycleAxis);
          if ((double) this.m_cycleLerp <= 0.0)
          {
            this.m_cycleLerp = 0.0f;
            this.MechaState = SosigWeapon.SosigWeaponMechaState.ReadyToFire;
            if (this.UsesCyclingSounds && this.GunShotProfile.EjectionForward.Clips.Count > 0 && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < (double) this.MinHandlingDistance)
            {
              SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, this.GunShotProfile.EjectionForward, this.transform.position);
              break;
            }
            break;
          }
          break;
      }
      if (this.HasCyclingPart && this.MechaState != SosigWeapon.SosigWeaponMechaState.CyclingBack && (this.MechaState != SosigWeapon.SosigWeaponMechaState.CyclingForward && this.m_shotsLeft <= 0))
        this.SetAnimatedComponent(this.CyclingPart, this.CycleForwardBackValues.y, this.CycleInterpStyle, this.CycleAxis);
      if (this.UsageState != SosigWeapon.SosigWeaponUsageState.Reloading)
        return;
      this.m_reloadingLerp += Time.deltaTime;
      if ((double) this.m_reloadingLerp <= (double) this.ReloadTime || !this.IsHeldByBot)
        return;
      this.MechaState = SosigWeapon.SosigWeaponMechaState.CyclingForward;
      this.m_reloadingLerp = 0.0f;
      this.UsageState = SosigWeapon.SosigWeaponUsageState.Firing;
      if (this.SosigHoldingThis.Inventory.HasAmmo((int) this.AmmoType))
        this.m_shotsLeft = this.SosigHoldingThis.Inventory.ReloadFromType((int) this.AmmoType, this.ShotsPerLoad);
      if (this.m_shotsLeft > 0)
        return;
      this.HandHoldingThis.DropHeldObject();
    }

    private float GetCurrentAngularLimit(float rangeToTarget) => this.AngularFiringLimitByRange.Evaluate(rangeToTarget / 100f) * this.MaxAngularFireRange;

    public bool GetFireClear(Vector3 source, Vector3 dir, float range, float distToTarget)
    {
      if (Physics.Raycast(source, dir, out this.m_hit, range, (int) this.LM_IFFCheck, QueryTriggerInteraction.Ignore))
      {
        AIEntityIFFBeacon component = this.m_hit.collider.attachedRigidbody.GetComponent<AIEntityIFFBeacon>();
        if ((Object) this.SosigHoldingThis != (Object) null && (Object) component != (Object) null && component.IFF == this.SosigHoldingThis.E.IFFCode)
          return false;
      }
      float maxDistance = Mathf.Min(distToTarget, 4f);
      return !Physics.Raycast(source, dir, out this.m_hit, maxDistance, (int) this.LM_EnvCheck, QueryTriggerInteraction.Ignore);
    }

    private void StartNewIdleAnim()
    {
      this.m_currentIdleAnim = this.AvailableIdleAnims[Random.Range(0, this.AvailableIdleAnims.Count)];
      this.m_idleTick = 0.0f;
      this.m_currentStateSpeeds.z = Random.Range(this.MeleeStateSpeeds.z * 0.9f, this.MeleeStateSpeeds.z * 1.1f);
    }

    private void StartNewReadyAnim()
    {
      this.m_currentReadyAnim = this.AvailableReadyAnims[Random.Range(0, this.AvailableReadyAnims.Count)];
      this.m_readyLoopTick = 0.0f;
      this.m_currentStateSpeeds.x = Random.Range(this.MeleeStateSpeeds.x * 0.9f, this.MeleeStateSpeeds.x * 1.4f);
    }

    private void StartNewAttackAnim()
    {
      this.m_currentAttackAnim = this.AvailableAttackAnims[Random.Range(0, this.AvailableAttackAnims.Count)];
      this.m_attackLerpTick = Random.Range(-2f, -0.8f);
      this.attackHoldLimit = Random.Range(1f, 1.8f);
      this.m_currentStateSpeeds.y = Random.Range(this.MeleeStateSpeeds.y * 0.9f, this.MeleeStateSpeeds.y * 1.1f);
      if (!this.PrimesOnFirstSwing || this.m_isFuseTickingDown)
        return;
      this.Pin.ForceExpelPin();
    }

    private void AbortAttack()
    {
      if (this.MeleeState != SosigWeapon.SosigMeleeState.Attacking)
        return;
      this.MeleeState = SosigWeapon.SosigMeleeState.Ready;
      this.StartNewReadyAnim();
    }

    public void UseMelee(Sosig.SosigObjectUsageFocus usage, bool isActive, Vector3 targetPoint)
    {
      if (this.IsAlsoThrowable && (double) Vector3.Distance(this.transform.position, targetPoint) > 5.0)
        this.Type = SosigWeapon.SosigWeaponType.Grenade;
      switch (usage)
      {
        case Sosig.SosigObjectUsageFocus.MaintainHeldAtRest:
          this.MeleeIdleCycle(isActive);
          break;
        case Sosig.SosigObjectUsageFocus.AttackTarget:
          this.MeleeAttackCycle(targetPoint);
          break;
        case Sosig.SosigObjectUsageFocus.AimAtReady:
          this.MeleeIdleCycle(isActive);
          break;
      }
    }

    public void MeleeIdleCycle(bool isActive)
    {
      if (!this.IsHeldByBot)
        return;
      if ((Object) this.m_currentIdleAnim == (Object) null)
        this.StartNewIdleAnim();
      this.MeleeState = SosigWeapon.SosigMeleeState.Ready;
      this.m_readyLoopTick = 0.0f;
      Vector3 zero = Vector3.zero;
      Vector3 forward = Vector3.forward;
      Vector3 up = Vector3.up;
      this.m_idleTick += Time.deltaTime * this.m_currentStateSpeeds.z;
      if (!isActive)
        this.m_idleTick = 0.0f;
      Vector3 vector3_1 = this.m_currentIdleAnim.GetPos(this.m_idleTick, false, true);
      Vector3 vector3_2 = this.m_currentIdleAnim.GetForward(this.m_idleTick, false, true);
      Vector3 vector3_3 = this.m_currentIdleAnim.GetUp(this.m_idleTick, false, true);
      if (!this.HandHoldingThis.IsRightHand)
      {
        vector3_1 = Vector3.Reflect(vector3_1, Vector3.right);
        vector3_2 = Vector3.Reflect(vector3_2, Vector3.right);
        vector3_3 = Vector3.Reflect(vector3_3, Vector3.right);
      }
      if ((double) this.m_idleTick >= 1.0)
        this.StartNewIdleAnim();
      this.HandHoldingThis.SetPoseDirect(this.HandHoldingThis.Root.TransformPoint(vector3_1), Quaternion.LookRotation(this.HandHoldingThis.Root.TransformDirection(vector3_2), this.HandHoldingThis.Root.TransformDirection(vector3_3)));
    }

    public void MeleeAttackCycle(Vector3 targetPoint)
    {
      if (!this.IsHeldByBot)
        return;
      Vector3 vector3_1 = Vector3.zero;
      Vector3 vector3_2 = Vector3.forward;
      Vector3 vector3_3 = Vector3.up;
      switch (this.MeleeState)
      {
        case SosigWeapon.SosigMeleeState.Ready:
          if ((Object) this.m_currentReadyAnim == (Object) null)
            this.StartNewReadyAnim();
          this.m_readyLoopTick += Time.deltaTime * this.m_currentStateSpeeds.x;
          vector3_1 = this.m_currentReadyAnim.GetPos(this.m_readyLoopTick, false, true);
          vector3_2 = this.m_currentReadyAnim.GetForward(this.m_readyLoopTick, false, true);
          vector3_3 = this.m_currentReadyAnim.GetUp(this.m_readyLoopTick, false, true);
          if (!this.HandHoldingThis.IsRightHand)
          {
            vector3_1 = Vector3.Reflect(vector3_1, Vector3.right);
            vector3_2 = Vector3.Reflect(vector3_2, Vector3.right);
            vector3_3 = Vector3.Reflect(vector3_3, Vector3.right);
          }
          if ((double) this.m_readyLoopTick >= (double) this.readyAbort)
          {
            this.m_readyLoopTick = 0.0f;
            this.MeleeState = SosigWeapon.SosigMeleeState.Attacking;
            this.StartNewAttackAnim();
            this.m_attackLerpTick += 1f - this.readyAbort;
            break;
          }
          break;
        case SosigWeapon.SosigMeleeState.Attacking:
          if ((Object) this.m_currentAttackAnim == (Object) null)
            this.StartNewAttackAnim();
          this.m_attackLerpTick += Time.deltaTime * this.m_currentStateSpeeds.y;
          vector3_1 = this.m_currentAttackAnim.GetPos(this.m_attackLerpTick, true, false);
          vector3_2 = this.m_currentAttackAnim.GetForward(this.m_attackLerpTick, true, false);
          vector3_3 = this.m_currentAttackAnim.GetUp(this.m_attackLerpTick, true, false);
          if (!this.HandHoldingThis.IsRightHand)
          {
            vector3_1 = Vector3.Reflect(vector3_1, Vector3.right);
            vector3_2 = Vector3.Reflect(vector3_2, Vector3.right);
            vector3_3 = Vector3.Reflect(vector3_3, Vector3.right);
          }
          if ((double) this.m_attackLerpTick >= (double) this.attackHoldLimit)
          {
            float num = Random.Range(0.0f, 1f);
            if ((double) num > 0.899999976158142)
            {
              this.MeleeState = SosigWeapon.SosigMeleeState.Attacking;
              this.StartNewAttackAnim();
              this.m_attackLerpTick = Random.Range(-0.8f, -0.3f);
              break;
            }
            if ((double) num > 0.600000023841858)
            {
              this.readyAbort = Random.Range(0.2f, 0.8f);
              this.m_attackLerpTick = 0.0f;
              this.MeleeState = SosigWeapon.SosigMeleeState.Ready;
              this.StartNewReadyAnim();
              break;
            }
            this.readyAbort = 1f;
            this.m_attackLerpTick = 0.0f;
            this.MeleeState = SosigWeapon.SosigMeleeState.Ready;
            this.StartNewReadyAnim();
            break;
          }
          break;
      }
      Vector3 pos = this.HandHoldingThis.Root.TransformPoint(vector3_1);
      Vector3 forward = this.HandHoldingThis.Root.TransformDirection(vector3_2);
      Vector3 upwards = this.HandHoldingThis.Root.TransformDirection(vector3_3);
      this.checkHeightTick -= Time.deltaTime;
      if ((double) this.checkHeightTick <= 0.0)
      {
        this.checkHeightTick = Random.Range(0.25f, 0.7f);
        float num = Mathf.Clamp(Mathf.Abs(targetPoint.y - this.HandHoldingThis.Root.position.y), 0.0f, 0.2f);
        this.heightOffset = (double) targetPoint.y >= (double) this.HandHoldingThis.Root.position.y || (double) num <= 0.100000001490116 ? ((double) targetPoint.y <= (double) this.HandHoldingThis.Root.position.y || (double) num <= 0.100000001490116 ? 0.0f : num * 0.3f) : -num;
      }
      if (this.DoesHeightAdjust)
        pos += Vector3.up * this.heightOffset;
      this.HandHoldingThis.SetPoseDirect(pos, Quaternion.LookRotation(forward, upwards));
    }

    public bool TryToThrowAt(Vector3 targetPoint, bool isReadyToThrow)
    {
      if (this.IsAlsoThrowable && (double) Vector3.Distance(this.transform.position, targetPoint) < 4.0)
        this.Type = this.m_backupType;
      if (!isReadyToThrow || (double) Vector3.Distance(this.HandHoldingThis.Target.position, this.transform.position) > 0.150000005960464)
        return false;
      Vector3 vector3_1 = targetPoint - this.transform.position;
      Vector3 s0;
      Vector3 s1;
      int num;
      if ((double) vector3_1.magnitude < 12.0)
      {
        Vector3 vector3_2 = vector3_1.normalized * 14f;
        s0 = vector3_2;
        s1 = vector3_2;
        num = 1;
      }
      else
        num = fts.solve_ballistic_arc(this.transform.position, Vector3.Distance(this.transform.position, targetPoint), targetPoint, Mathf.Abs(Physics.gravity.y), out s0, out s1);
      if (num > 0 && (Object) this.Pin != (Object) null)
        this.Pin.ForceExpelPin();
      if (num <= 0)
        return false;
      Vector3 vector3_3 = s0;
      if (num > 1)
        vector3_3 = s1;
      if (this.IsHeldByBot)
        this.SourceIFF = this.SosigHoldingThis.E.IFFCode;
      else if (this.O.IsHeld)
        this.SourceIFF = GM.CurrentPlayerBody.GetPlayerIFF();
      this.HandHoldingThis.ThrowObject(s0 * 1f, targetPoint);
      if (this.DoesCastForPlayerHitBox)
        this.m_isThrownTagCastTag = true;
      return true;
    }

    public void TryToFireGun(
      Vector3 targetPos,
      bool isPanicFiring,
      bool targetPointIdentified,
      bool isClutching,
      float recoilMult,
      bool isHipfiring)
    {
      if (this.UsageState != SosigWeapon.SosigWeaponUsageState.Firing)
        return;
      if ((double) this.m_refireTick > 0.0)
      {
        this.m_refireTick -= Time.deltaTime;
      }
      else
      {
        if (!targetPointIdentified && !isPanicFiring)
          return;
        Vector3 vector = targetPos - this.Muzzle.position;
        float magnitude = vector.magnitude;
        if ((double) magnitude > (double) this.RangeMinMax.y && !isPanicFiring)
          return;
        float currentAngularLimit = this.GetCurrentAngularLimit(magnitude);
        if (isHipfiring)
          currentAngularLimit *= 4f;
        float num1 = Vector3.Angle(Vector3.ProjectOnPlane(vector, Vector3.up), Vector3.ProjectOnPlane(this.Muzzle.forward, Vector3.up));
        float num2 = Vector3.Angle(Vector3.ProjectOnPlane(vector, this.transform.right), Vector3.ProjectOnPlane(this.Muzzle.forward, this.transform.right));
        float b = 0.5f;
        if (this.UsesBurstLimit && this.m_shotsLeftTilBurstLimit < 2)
          b = 5f;
        if ((double) magnitude < 4.0)
          b = 5f;
        if ((double) magnitude < 2.0)
          b = 10f;
        if ((double) magnitude < 1.0)
          b = currentAngularLimit;
        bool flag = false;
        if (isClutching && (double) this.timeSinceFired < 0.25)
          flag = true;
        if (((double) num1 > (double) currentAngularLimit || (double) num2 > (double) Mathf.Min(currentAngularLimit, b)) && (!isPanicFiring && !flag) || !isPanicFiring && !flag && !this.GetFireClear(this.Muzzle.position, this.Muzzle.forward, this.RangeMinMax.y, magnitude))
          return;
        this.FireGun(recoilMult);
      }
    }

    private void UpdateRecoil()
    {
      bool flag1 = false;
      bool flag2 = false;
      if ((double) this.m_recoilX > 0.0)
      {
        this.m_recoilX = Mathf.MoveTowards(this.m_recoilX, 0.0f, (float) ((double) Time.deltaTime * (double) this.RecoilPerShot.y * 4.0));
        flag1 = true;
      }
      if ((double) Mathf.Abs(this.m_recoilY) > 0.0)
      {
        this.m_recoilY = Mathf.MoveTowards(this.m_recoilY, 0.0f, (float) ((double) Time.deltaTime * (double) this.RecoilPerShot.y * 3.0 * 0.5));
        flag1 = true;
      }
      if ((double) this.m_recoilLinear > 0.0)
      {
        this.m_recoilLinear = Mathf.MoveTowards(this.m_recoilLinear, 0.0f, (float) ((double) Time.deltaTime * (double) this.RecoilPerShot.w * 1.0));
        flag2 = true;
      }
      if (flag2)
        this.RecoilHolder.localPosition = new Vector3(0.0f, 0.0f, this.m_recoilLinear);
      if (!flag1)
        return;
      this.RecoilHolder.localEulerAngles = new Vector3(this.m_recoilX, this.m_recoilY, 0.0f);
    }

    private void Recoil(float recoilMult)
    {
      this.m_recoilX += this.RecoilPerShot.x * recoilMult;
      this.m_recoilY += Random.Range(this.RecoilPerShot.x * 0.5f, (float) (-(double) this.RecoilPerShot.x * 0.5)) * recoilMult;
      this.m_recoilLinear += this.RecoilPerShot.z;
      this.m_recoilX = Mathf.Clamp(this.m_recoilX, 0.0f, this.RecoilPerShot.y);
      this.m_recoilY = Mathf.Clamp(this.m_recoilY, 0.0f, this.RecoilPerShot.y * 0.2f);
      this.m_recoilLinear = Mathf.Clamp(this.m_recoilLinear, 0.0f, this.RecoilPerShot.w);
      this.RecoilHolder.localPosition = new Vector3(0.0f, 0.0f, this.m_recoilLinear);
      this.RecoilHolder.localEulerAngles = new Vector3(this.m_recoilX, this.m_recoilY, 0.0f);
    }

    public bool FireGun(float recoilMult)
    {
      if (this.m_shotsLeft <= 0 || this.MechaState != SosigWeapon.SosigWeaponMechaState.ReadyToFire)
        return false;
      if (this.IsHeldByBot)
        this.SosigHoldingThis.E.ProcessLoudness(this.ShotLoudness);
      if ((Object) this.Muzzle == (Object) null)
        return false;
      this.timeSinceFired = 0.0f;
      if (this.UsesSustainedSound)
      {
        if ((double) this.m_sustainEnergy <= 0.0)
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_SustainedEmit, this.transform.position);
        if (!this.AudSource_SustainedLoop.isPlaying)
          this.AudSource_SustainedLoop.Play();
        this.m_sustainEnergy = this.SustainEnergyPerShot;
      }
      for (int index = 0; index < this.ProjectilesPerShot; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.Projectile, this.Muzzle.position, this.Muzzle.rotation);
        gameObject.transform.Rotate(new Vector3(Random.Range(-this.ProjectileSpread, this.ProjectileSpread), Random.Range(-this.ProjectileSpread, this.ProjectileSpread), 0.0f));
        BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
        component.FlightVelocityMultiplier = this.FlightVelocityMultiplier;
        float muzzleVelocityBase = component.MuzzleVelocityBase;
        if (this.IsHeldByBot && this.SosigHoldingThis.isDamPowerUp)
          muzzleVelocityBase *= this.SosigHoldingThis.BuffIntensity_DamPowerUpDown;
        component.Fire(muzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) null);
        if (this.IsHeldByBot)
          component.SetSource_IFF(this.SosigHoldingThis.E.IFFCode);
        else if (this.O.IsHeld)
          component.SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
      }
      this.Recoil(recoilMult);
      if ((Object) this.GunShotProfile != (Object) null)
      {
        float multByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(this.PlayShotEvent(this.Muzzle.position));
        if (this.IsHeldByBot)
          GM.CurrentSceneSettings.OnPerceiveableSound(this.ShotLoudness, this.ShotLoudness * multByEnvironment, this.transform.position, this.SosigHoldingThis.E.IFFCode);
        else if (this.O.IsHeld)
          GM.CurrentSceneSettings.OnPerceiveableSound(this.ShotLoudness, this.ShotLoudness * multByEnvironment, this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
      }
      if (this.UsesMuzzleFire)
      {
        for (int index = 0; index < this.PSystemsMuzzle.Length; ++index)
          this.PSystemsMuzzle[index].Emit(this.MuzzlePAmount);
      }
      if (this.DoesFlashOnFire)
        FXM.InitiateMuzzleFlash(this.Muzzle.position, this.Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      if (this.IsHeldByBot)
      {
        if (!this.SosigHoldingThis.IsInfiniteAmmo)
        {
          if (this.SosigHoldingThis.IsAmmoDrain)
            this.m_shotsLeft = 0;
          else
            --this.m_shotsLeft;
        }
      }
      else if (!GM.CurrentPlayerBody.IsInfiniteAmmo)
      {
        if (GM.CurrentPlayerBody.IsAmmoDrain)
          this.m_shotsLeft = 0;
        else
          --this.m_shotsLeft;
      }
      ++this.m_shotsSoFarThisCycle;
      if (this.m_shotsSoFarThisCycle >= this.ShotsPerCycle)
      {
        this.MechaState = SosigWeapon.SosigWeaponMechaState.CyclingBack;
        this.m_cycleLerp = 0.0f;
        this.m_shotsSoFarThisCycle = 0;
      }
      if (this.UsesBurstLimit)
        --this.m_shotsLeftTilBurstLimit;
      this.m_refireTick = Random.Range(this.Usage_RefireRange.x, this.Usage_RefireRange.y);
      if (this.m_shotsLeftTilBurstLimit <= 0)
      {
        this.m_shotsLeftTilBurstLimit = Random.Range(this.BurstLimit, this.BurstLimit + 1 + this.BurstLimitRange);
        this.m_refireTick += Random.Range(this.BurstDelayRange.x, this.BurstDelayRange.y);
      }
      return true;
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

    private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(
      FVRSoundEnvironment e)
    {
      return this.m_shotDic[e];
    }

    private void OnCollisionEnter(Collision col)
    {
      this.ProcessCollision(col);
      this.m_isThrownTagCastTag = false;
    }

    private void Shatter()
    {
      if (this.m_isShattered)
        return;
      this.m_isShattered = true;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Shatter, this.transform.position);
      for (int index = 0; index < this.ShardPrefabs.Count; ++index)
        Object.Instantiate<GameObject>(this.ShardPrefabs[index], this.ShardPositions[index].position, this.ShardPositions[index].rotation).GetComponent<Rigidbody>().velocity = this.O.RootRigidbody.velocity + Random.onUnitSphere * 0.2f;
      Object.Destroy((Object) this.gameObject);
    }

    private void ProcessCollision(Collision col)
    {
      bool flag = false;
      if (this.IsInBotInventory)
        return;
      if ((Object) col.collider.attachedRigidbody != (Object) null)
        flag = true;
      if (!flag)
        return;
      if (this.IsDestructible && this.O.IsHeld)
      {
        this.m_collisionLife -= col.relativeVelocity.magnitude;
        if ((double) this.m_collisionLife <= 0.0)
          this.m_shouldShatter = true;
      }
      if (this.IsHeldByBot && this.SosigHoldingThis.IgnoreRBs.Contains(col.collider.attachedRigidbody) || this.IsInBotInventory)
        return;
      if (this.Type == SosigWeapon.SosigWeaponType.Melee)
      {
        if (this.IsHeldByBot && this.MeleeState == SosigWeapon.SosigMeleeState.Attacking)
        {
          this.DoMeleeDamageInCollision(col);
          if (LayerMask.LayerToName(col.collider.gameObject.layer) != "AgentBody")
            this.AbortAttack();
        }
        else if (this.O.IsHeld)
          this.DoMeleeDamageInCollision(col);
        else if (!this.IsHeldByBot && !this.O.IsHeld && (double) this.O.RootRigidbody.velocity.magnitude > 2.0)
          this.DoMeleeDamageInCollision(col);
      }
      float magnitude = col.relativeVelocity.magnitude;
      float num = 6f;
      if (this.Type == SosigWeapon.SosigWeaponType.Melee)
        num = 15f;
      if (this.MeleeState == SosigWeapon.SosigMeleeState.Attacking)
        num = 25f;
      if (!this.IsDroppedOnStrongHit || ((double) col.relativeVelocity.magnitude <= (double) num || !this.IsHeldByBot || (!(LayerMask.LayerToName(col.collider.gameObject.layer) != "AgentBody") || !this.HandHoldingThis.S.DoesDropWeaponsOnBallistic)))
        return;
      this.HandHoldingThis.DropHeldObject();
    }

    private void DoMeleeDamageInCollision(Collision col)
    {
      if ((double) this.DamRefire > 0.0)
        return;
      IFVRDamageable fvrDamageable = col.collider.transform.GetComponent<IFVRDamageable>() ?? col.collider.attachedRigidbody.GetComponent<IFVRDamageable>();
      FistVR.Damage dam = new FistVR.Damage();
      dam.Class = FistVR.Damage.DamageClass.Melee;
      dam.point = col.contacts[0].point;
      dam.hitNormal = col.contacts[0].normal;
      dam.strikeDir = this.O.RootRigidbody.velocity.normalized;
      dam.damageSize = 0.02f;
      float num = Mathf.Clamp(col.relativeVelocity.magnitude, 2f, 10f);
      if (this.IsHeldByBot && (this.SosigHoldingThis.IsMuscleMeat || this.SosigHoldingThis.IsWeakMeat))
        num *= this.SosigHoldingThis.BuffIntensity_MuscleMeatWeak;
      if (this.O.IsHeld)
      {
        num = Mathf.Clamp(this.O.m_hand.Input.VelLinearWorld.magnitude, 0.0f, 10f);
        if (GM.CurrentPlayerBody.IsMuscleMeat || GM.CurrentPlayerBody.IsWeakMeat)
          num *= GM.CurrentPlayerBody.GetMuscleMeatPower();
      }
      switch (this.MeleeType)
      {
        case SosigWeapon.SosigMeleeWeaponType.Bladed:
          dam.Dam_Blunt = 5f * num;
          dam.Dam_Cutting = 75f * num;
          dam.Dam_Piercing = 5f * num;
          dam.Dam_TotalKinetic = 80f * num;
          break;
        case SosigWeapon.SosigMeleeWeaponType.Blunt:
          dam.Dam_Blunt = 35f * num;
          dam.Dam_Piercing = 10f * num;
          dam.Dam_TotalKinetic = 45f * num;
          break;
        case SosigWeapon.SosigMeleeWeaponType.Stabbing:
          dam.Dam_Blunt = 5f * num;
          dam.Dam_Piercing = 40f * num;
          dam.Dam_Cutting = 25f * num;
          dam.Dam_TotalKinetic = 75f * num;
          break;
        case SosigWeapon.SosigMeleeWeaponType.Shield:
          dam.Dam_Blunt = 45f * num;
          dam.Dam_Cutting = 10f * num;
          dam.Dam_Piercing = 5f * num;
          dam.Dam_TotalKinetic = 60f * num;
          break;
      }
      if (fvrDamageable == null)
        return;
      fvrDamageable.Damage(dam);
      this.DamRefire = 0.1f;
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.TakesDamage)
      {
        this.Life -= d.Dam_Blunt * this.ResistMultBCP.x + d.Dam_Cutting * this.ResistMultBCP.y + d.Dam_Piercing * this.ResistMultBCP.z;
        if (this.UsesDestructionStageRenderers)
          this.UpdateDestructionRenderers();
        if ((double) this.Life < 0.0)
          this.Shatter();
      }
      if (d.Class == FistVR.Damage.DamageClass.Melee && this.Type == SosigWeapon.SosigWeaponType.Melee)
        return;
      if (this.IsDroppedOnStrongHit && this.IsHeldByBot && ((double) d.Dam_TotalKinetic > 600.0 && this.HandHoldingThis.S.DoesDropWeaponsOnBallistic))
        this.HandHoldingThis.DropHeldObject();
      if (this.IsDroppedOnStrongHit && this.IsInBotInventory && ((double) d.Dam_TotalKinetic > 1000.0 && this.InventorySlotWithThis.I.S.DoesDropWeaponsOnBallistic))
        this.InventorySlotWithThis.DetachHeldObject();
      if (this.Type != SosigWeapon.SosigWeaponType.Grenade || (double) d.Dam_Thermal <= 5.0)
        return;
      this.FuseGrenade();
      this.GrenadeFuseTime = Random.Range(this.GrenadeFuseTime * 0.1f, this.GrenadeFuseTime * 0.25f);
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

    public void SetAnimatedComponent(
      Transform t,
      float val,
      SosigWeapon.InterpStyle interp,
      SosigWeapon.Axis axis)
    {
      switch (interp)
      {
        case SosigWeapon.InterpStyle.Translate:
          Vector3 localPosition = t.localPosition;
          switch (axis)
          {
            case SosigWeapon.Axis.X:
              localPosition.x = val;
              break;
            case SosigWeapon.Axis.Y:
              localPosition.y = val;
              break;
            case SosigWeapon.Axis.Z:
              localPosition.z = val;
              break;
          }
          t.localPosition = localPosition;
          break;
        case SosigWeapon.InterpStyle.Rotation:
          Vector3 zero = Vector3.zero;
          switch (axis)
          {
            case SosigWeapon.Axis.X:
              zero.x = val;
              break;
            case SosigWeapon.Axis.Y:
              zero.y = val;
              break;
            case SosigWeapon.Axis.Z:
              zero.z = val;
              break;
          }
          t.localEulerAngles = zero;
          break;
      }
    }

    public enum SosigWeaponType
    {
      Gun,
      Melee,
      Grenade,
      Ammo,
      Healing,
      Shield,
    }

    public enum SosiggunAmmoType
    {
      None,
      PistolLight,
      PistolStrong,
      PistolExotic,
      RifleLight,
      RifleStrong,
      RifleSniper,
      Machinegun,
      Shotgun,
      Grenade,
      Rocket,
      Zosig,
    }

    public enum SosigWeaponMechaState
    {
      ReadyToFire,
      CyclingBack,
      CyclingForward,
    }

    public enum SosigMeleeWeaponType
    {
      Bladed,
      Blunt,
      Stabbing,
      Shield,
    }

    public enum SosigMeleeState
    {
      Ready,
      Attacking,
      Recovery,
    }

    public enum SosigWeaponUsageState
    {
      Firing,
      Reloading,
    }

    public enum SosigWeaponLayerState
    {
      Agent,
      Player,
    }

    public enum InterpStyle
    {
      Translate,
      Rotation,
    }

    public enum Axis
    {
      X,
      Y,
      Z,
    }
  }
}
