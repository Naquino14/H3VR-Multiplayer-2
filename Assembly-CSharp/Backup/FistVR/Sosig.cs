// Decompiled with JetBrains decompiler
// Type: FistVR.Sosig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class Sosig : MonoBehaviour
  {
    public Sosig.SosigType Type;
    [Header("Connections")]
    public AIEntity E;
    public List<SosigLink> Links;
    public MeshFilter[] Meshes;
    public Renderer[] Renderers;
    public NavMeshAgent Agent;
    private List<CharacterJoint> m_joints = new List<CharacterJoint>();
    public Transform DeParentOnSpawn;
    private int m_originalIFFTeam;
    [Header("--Target Priority System--")]
    public AITargetPrioritySystem Priority;
    private bool m_hasPriority;
    private bool m_hasConfiguredPriority;
    public Transform PriorityTesterRay;
    public bool IsDebuggingPriority;
    [Header("Body Logic")]
    public Sosig.SosigBodyState BodyState;
    private float m_recoveryFromBallisticTick;
    private float m_recoveryFromBallisticLerp;
    private bool m_recoveringFromBallisticState;
    private Vector3 m_recoveryFromBallisticStartPos;
    private Quaternion m_recoveryFromBallisticStartRot;
    private float m_maxJointLimit = 6f;
    [Header("HeadIconLogic")]
    public Sosig.SosigHeadIconState HeadIconState;
    public List<GameObject> HeadIcons;
    private float m_rotato;
    [Header("Speech System")]
    public SosigSpeechSet Speech;
    public float m_tickDownToNextStateSpeech = 10f;
    public float m_tickDownToPainSpeechAvailability = 10f;
    [Header("Bleeding Logic")]
    public float Mustard = 100f;
    private float m_maxMustard = 100f;
    public float BleedDamageMult = 0.5f;
    public float BleedRateMult = 1f;
    public float BleedVFXIntensity = 0.3f;
    private bool m_needsToSpawnBleedEvent;
    private float m_bloodLossForVFX;
    private Vector3 m_bloodLossPoint;
    private Vector3 m_bloodLossDir;
    private SosigLink m_linkToBleed;
    public GameObject DamageFX_SmallMustardBurst;
    public GameObject DamageFX_LargeMustardBurst;
    public GameObject DamageFX_MustardSpoutSmall;
    public GameObject DamageFX_MustardSpoutLarge;
    public GameObject DamageFX_Explosion;
    public GameObject DamageFX_Vaporize;
    public bool UsesGibs;
    public Vector3[] GibLocalPoses = new Vector3[8]
    {
      new Vector3()
      {
        x = -0.0644829f,
        y = 0.0619502f,
        z = 0.05735424f
      },
      new Vector3()
      {
        x = -0.06146608f,
        y = 0.06169732f,
        z = -0.05414369f
      },
      new Vector3()
      {
        x = 0.0427421f,
        y = 0.0777971f,
        z = 0.05667716f
      },
      new Vector3()
      {
        x = 0.04047735f,
        y = 0.08263668f,
        z = -0.04825908f
      },
      new Vector3()
      {
        x = 0.04711252f,
        y = -0.08044733f,
        z = 0.06141138f
      },
      new Vector3()
      {
        x = -0.04509961f,
        y = -0.05805374f,
        z = -0.05172355f
      },
      new Vector3()
      {
        x = 0.0655114f,
        y = -0.06081913f,
        z = -0.036587f
      },
      new Vector3()
      {
        x = -0.07710075f,
        y = -0.08449232f,
        z = 0.04284497f
      }
    };
    public List<GameObject> GibPrefabs = new List<GameObject>();
    public Material GibMaterial;
    public Material InvulnMaterial;
    public Material InvisMaterial;
    public Material VaporizeMaterial;
    public Material FrozenMaterial;
    public Sosig.MaterialType CurMat;
    private bool m_hasInvulnMaterial;
    private bool m_hasInvisMaterial;
    private bool m_hasFrozenMaterial;
    private bool m_hasVaporizeMaterial;
    private List<Sosig.BleedingEvent> m_bleedingEvents = new List<Sosig.BleedingEvent>();
    [Header("Damage Logic")]
    public float ShudderThreshold = 2f;
    private float ConfusionThreshold = 0.3f;
    private float ConfusionMultiplier = 6f;
    private float StunThreshold = 1.4f;
    private float StunMultiplier = 2f;
    private bool m_isBleeding;
    private float m_bleedRate;
    private bool m_isStunned;
    public float m_stunTimeLeft;
    private float m_maxStunTime = 6f;
    private bool m_isConfused;
    public float m_confusedTime;
    private float m_maxConfusedTime = 6f;
    private bool m_isBlinded;
    private float m_blindTime;
    private float m_maxBlindTime = 10f;
    private float m_suppressionLevel;
    private Vector3 m_suppressionDir = Vector3.zero;
    private float m_alertnessLevel;
    public bool HasABrain = true;
    public bool DoesDropWeaponsOnBallistic = true;
    public bool CanPickup_Ranged = true;
    public bool CanPickup_Melee = true;
    public bool CanPickup_Other = true;
    private float m_tickDownToWrithe;
    private Vector2 m_writheTickRange = new Vector2(0.1f, 0.4f);
    private bool m_isCountingDownToStagger;
    private float m_tickDownToStagger;
    private float m_staggerAmountToApply;
    public GameObject[] HitDecalPrefabs;
    private Vector2 HitDecalSizeRange = new Vector2(0.0055f, 0.1f);
    public int MaxDecalsPerFrame = 1;
    public int MaxTotalDecals = 6;
    private int m_hitDecalsThisFrameSoFar;
    private int m_nextDecalToMoveIndex;
    private List<Transform> m_spawnedHitDecals = new List<Transform>();
    private bool m_doesJointBreakKill_Head = true;
    private bool m_doesJointBreakKill_Upper;
    private bool m_doesJointBreakKill_Lower;
    private bool m_doesSeverKill_Head = true;
    private bool m_doesSeverKill_Upper = true;
    private bool m_doesSeverKill_Lower = true;
    private bool m_doesExplodeKill_Head = true;
    private bool m_doesExplodeKill_Upper = true;
    private bool m_doesExplodeKill_Lower = true;
    [Header("Physics Stuff")]
    public Transform CoreTarget;
    public Rigidbody CoreRB;
    public HashSet<Rigidbody> IgnoreRBs = new HashSet<Rigidbody>();
    public float AttachedRotationMultiplier = 6f;
    public float AttachedPositionMultiplier = 900f;
    public float AttachedRotationFudge = 100f;
    public float AttachedPositionFudge = 100f;
    [Header("Meshes")]
    public Mesh[] Meshes_Whole;
    public Mesh[] Meshes_Severed_Top;
    public Mesh[] Meshes_Severed_Bottom;
    public Mesh[] Meshes_Severed_Both;
    public bool UsesLinkMeshOverride;
    private bool[] m_jointsBroken = new bool[4];
    private bool[] m_jointsSevered = new bool[4];
    private bool[] m_linksDestroyed = new bool[4];
    private bool m_isBackBroken;
    private bool m_isHobbled;
    private bool m_isTickingDownToClear;
    private float m_tickDownToClear = 15f;
    public SosigConfigTemplate TestingSosigTemplate;
    [Header("Dam Resistances")]
    public bool AppliesDamageResistToIntegrityLoss;
    public float DamMult_Projectile = 1f;
    public float DamMult_Explosive = 1f;
    public float DamMult_Melee = 1f;
    public float DamMult_Piercing = 1f;
    public float DamMult_Blunt = 1f;
    public float DamMult_Cutting = 1f;
    public float DamMult_Thermal = 1f;
    public float DamMult_Chilling = 1f;
    public float DamMult_EMP = 1f;
    public bool CanBeSuppresed = true;
    public float SuppressionMult = 1f;
    private float GrenadeThrowLag = 3f;
    public bool CanBeGrabbed = true;
    public bool CanBeSevered = true;
    public bool CanBeStabbed = true;
    public float SearchExtentsModifier = 1f;
    private bool isSpeaking;
    public bool IsAllowedToSpeak = true;
    private FVRPooledAudioSource m_speakingSource;
    private float m_entityJiggleTick = 0.4f;
    [Header("Brain Logic")]
    public Sosig.SosigOrder CurrentOrder;
    public Sosig.SosigOrder FallbackOrder;
    private float m_tickDownToRandomLook = 1f;
    private Vector2 m_randomLookTickRange = new Vector2(2f, 20f);
    private float m_tickDownToRandomWanderPoint = 1f;
    private Vector2 m_randomWanderPointRange = new Vector2(3f, 20f);
    private bool m_hardGuard;
    private Vector3 m_guardDominantDirection = Vector3.forward;
    private Vector3 m_guardPoint = Vector3.zero;
    private Vector3 m_wanderPoint = Vector3.zero;
    private float m_guardInvestigateDistanceThreshold = 10f;
    private float m_weaponSwapCheckTick = 1f;
    private Vector2 m_weaponSwapTickRange = new Vector2(1f, 2f);
    private float m_investigateCooldown = 5f;
    private Vector3 m_takeCoverDiveDir = Vector3.forward;
    private bool m_hasDoveYet;
    private float m_assaultPointOverridesSkirmishPointWhenFurtherThan = 200f;
    private Vector3 m_assaultPoint = Vector3.zero;
    private Sosig.SosigMoveSpeed m_assaultSpeed = Sosig.SosigMoveSpeed.Running;
    private float m_timeTilAssaultDirChange = 2f;
    private float m_assaultCross;
    public float HipFiringVelocityThreshold = 1f;
    private float m_entityRecognition;
    private Vector3 m_skirmishPoint = Vector3.zero;
    private float m_tickDownToPickNewSkirmishPoint = 1f;
    private AICoverPoint m_curCoverPoint;
    private float CoverSearchRange = 10f;
    private bool m_wasAttacking;
    private float horizOffset;
    private float DesiredMeleeDistance = 1f;
    private float m_timeTilReloadShout = 10f;
    private Vector3 m_investigateNoiseDir = Vector3.zero;
    private float m_investigateNoiseTick = 1f;
    private float m_searchForEquipmentTickDown = 10f;
    private float m_searchForEquipmentCoolDown;
    [Header("Hand Logic")]
    public List<SosigHand> Hands = new List<SosigHand>();
    public SosigInventory Inventory = new SosigInventory();
    public SosigHand Hand_Primary;
    public SosigHand Hand_Secondary;
    public Sosig.SosigObjectUsageFocus ObjectUsageFocus = Sosig.SosigObjectUsageFocus.MaintainHeldAtRest;
    public float EquipmentPickupDistance = 1.5f;
    private float m_searchForWeaponsTick = 1f;
    private Vector2 m_searchForWeaponsRefire = new Vector2(1f, 2f);
    public LayerMask LM_SearchForWeapons;
    private AIEntity m_targetEquipmentToPickup;
    private bool m_hasTargetEquipment;
    private bool m_hasHandsInit;
    private Vector3 m_aimPoint = Vector3.zero;
    private Vector3 m_aimWander = Vector3.zero;
    private Vector3 m_aimWanderVel = Vector3.zero;
    private Vector3 m_aimWanderRandom = Vector3.zero;
    private float m_aimWanderTick;
    public GameObject AimTester;
    private Transform aimTester;
    private float m_timeSinceGrenadeThrow;
    [Header("Movement Logic")]
    public Sosig.SosigMovementState MovementState;
    public float Speed_Crawl = 0.3f;
    public float Speed_Sneak = 0.6f;
    public float Speed_Walk = 3.5f;
    public float Speed_Run = 1.4f;
    public float Speed_Turning = 2f;
    public float MovementRotMagnitude = 10f;
    public Sosig.SosigMoveSpeed MoveSpeed = Sosig.SosigMoveSpeed.Walking;
    public AnimationCurve BodyBobCurve_Horizontal;
    public AnimationCurve BodyBobCurve_Vertical;
    public float MaxBobIntensityHorizontal = 0.03f;
    public float MaxBobIntensityVertical = 0.1f;
    public float BobSpeedMult = 1f;
    private float m_bobTick;
    public AudioEvent AudEvent_FootSteps;
    public Transform Pose_Standing;
    public Transform Pose_Crouching;
    public Transform Pose_Prone;
    private Transform m_targetPose;
    private Vector3 m_targetLocalPos;
    private Quaternion m_targetLocalRot;
    private Vector3 m_poseLocalEulers_Standing;
    private Vector3 m_poseLocalEulers_Crouching;
    private Vector3 m_poseLocalEulers_Prone;
    public Sosig.SosigBodyPose BodyPose;
    private Vector3 m_navToPoint = Vector3.zero;
    private Vector3 m_faceTowards = Vector3.forward;
    private NavMeshPath m_cachedPath;
    private NavMeshHit m_navMeshHit;
    private bool m_isOnOffMeshLink;
    private NavMeshLinkExtension linkExtensions;
    private float extensionSpeed = 1f;
    private float targetSpeed = 1f;
    private Vector3 destinationPoint = Vector3.zero;
    private Vector3 curEuluer = Vector3.zero;
    private Vector3 tarEuluer = Vector3.zero;
    private bool debug_haspath;
    private bool debug_pathpending;
    private bool m_hasSetDestYet;
    private Vector3 lastDest = Vector3.zero;
    private int m_linkIndex;
    private Vector3 fakeEntityPos;
    private bool m_hasFootStepDown;
    private float m_ballisticRecoveryAttemptTick;
    private Vector2 m_ballsticRecoveryAttemptRange = new Vector2(2f, 4f);
    private bool m_isHealing;
    private bool m_isDamResist;
    private bool m_isInvuln;
    private bool m_isDamPowerUp;
    private bool m_isInfiniteAmmo;
    private bool m_isGhosted;
    private bool m_isMuscleMeat;
    private bool m_isCyclops;
    private bool m_isBlort;
    private bool m_isSpeedup;
    private float m_buffTime_Heal;
    private float m_buffTime_DamResist;
    private float m_buffTime_Invuln;
    private float m_buffTime_DamPowerUp;
    private float m_buffTime_InfiniteAmmo;
    private float m_buffTime_Ghosted;
    private float m_buffTime_MuscleMeat;
    private float m_buffTime_Cyclops;
    private float m_buffTime_Blort;
    private float m_buffTime_Speedup;
    private bool m_isHurting;
    private bool m_isDamMult;
    private bool m_isFragile;
    private bool m_isDamPowerDown;
    private bool m_isAmmoDrain;
    private bool m_isSuperVisible;
    private bool m_isWeakMeat;
    private bool m_isBiClops;
    private bool m_isDlort;
    private bool m_isDebuff;
    private bool m_isFrozen;
    private bool m_isChillOut;
    private bool m_isVomitting;
    private float m_debuffTime_Debuff;
    private float m_debuffTime_Hurt;
    private float m_debuffTime_DamMult;
    private float m_debuffTime_Fragile;
    private float m_debuffTime_DamPowerDown;
    private float m_debuffTime_AmmoDrain;
    private float m_debuffTime_SuperVisible;
    private float m_debuffTime_WeakMeat;
    private float m_debuffTime_BiClops;
    private float m_debuffTime_Dlort;
    private float m_debuffTime_Freeze;
    private float m_debuffTime_ChillOut;
    private float m_debuffTime_Vomit;
    private float m_buffIntensity_HealHarm = 20f;
    private float m_buffIntensity_DamResistHarm = 0.6f;
    private float m_buffIntensity_DamPowerUpDown = 1f;
    private float m_buffIntensity_MuscleMeatWeak = 4f;
    private float m_buffIntensity_CyclopsPower = 4f;
    private GameObject m_vfx_cyclops;
    private GameObject m_vfx_bicyclops;
    private GameObject m_vfx_vomit;
    private GameObject[] BuffSystems = new GameObject[14];
    private GameObject[] DeBuffSystems = new GameObject[14];
    private bool m_isVaporizing;
    private bool m_isVaporized;
    private float m_vaporizeTime = 4f;
    private List<Transform> m_vaporizeSystems;
    private float m_timeSinceLastDamage = 10f;
    private float m_storedShudder;
    private int m_lastIFFDamageSource = -1;
    private Damage.DamageClass m_diedFromClass;
    private Sosig.SosigDeathType m_diedFromType;
    private bool m_receivedHeadShot;

    public void SetOriginalIFFTeam(int i) => this.m_originalIFFTeam = i;

    public int GetOriginalIFFTeam() => this.m_originalIFFTeam;

    [ContextMenu("SetGibPoses")]
    public void SetGibPoses()
    {
      for (int index = 0; index < this.GibPrefabs.Count; ++index)
        this.GibLocalPoses[index] = this.GibPrefabs[index].transform.position;
    }

    public void SetInvulnMaterial(Material m)
    {
      this.InvulnMaterial = m;
      this.m_hasInvulnMaterial = true;
    }

    public void SetInvisMaterial(Material m)
    {
      this.InvisMaterial = m;
      this.m_hasInvisMaterial = true;
    }

    public void SetFrozenMaterial(Material m)
    {
      this.FrozenMaterial = m;
      this.m_hasFrozenMaterial = true;
    }

    public void SetVaporizeMaterial(Material m)
    {
      this.VaporizeMaterial = m;
      this.m_hasVaporizeMaterial = true;
    }

    public bool IsStunned => this.m_isStunned;

    public bool IsConfused => this.m_isConfused;

    public bool IsBlinded => this.m_isBlinded;

    public float SuppressionLevel => this.m_suppressionLevel;

    private Vector3 SuppressionDir => this.m_suppressionDir;

    public float AlertnessLevel => this.m_alertnessLevel;

    public void Configure(SosigConfigTemplate t)
    {
      this.AppliesDamageResistToIntegrityLoss = t.AppliesDamageResistToIntegrityLoss;
      this.Mustard = t.TotalMustard;
      this.m_maxMustard = t.TotalMustard;
      this.BleedDamageMult = t.BleedDamageMult;
      this.BleedRateMult = t.BleedRateMultiplier;
      this.BleedVFXIntensity = t.BleedVFXIntensity;
      this.SearchExtentsModifier = t.SearchExtentsModifier;
      this.ShudderThreshold = t.ShudderThreshold;
      this.ConfusionThreshold = t.ConfusionThreshold;
      this.ConfusionMultiplier = t.ConfusionMultiplier;
      this.m_maxConfusedTime = t.ConfusionTimeMax;
      this.StunThreshold = t.StunThreshold;
      this.StunMultiplier = t.StunMultiplier;
      this.m_maxStunTime = t.StunTimeMax;
      this.HasABrain = t.HasABrain;
      this.DoesDropWeaponsOnBallistic = t.HasABrain;
      this.m_assaultPointOverridesSkirmishPointWhenFurtherThan = t.AssaultPointOverridesSkirmishPointWhenFurtherThan;
      this.E.MaximumSightRange = t.ViewDistance;
      this.E.MaxHearingDistance = t.HearingDistance;
      this.CanPickup_Ranged = t.CanPickup_Ranged;
      this.CanPickup_Melee = t.CanPickup_Melee;
      this.CanPickup_Other = t.CanPickup_Other;
      this.m_doesJointBreakKill_Head = t.DoesJointBreakKill_Head;
      this.m_doesJointBreakKill_Upper = t.DoesJointBreakKill_Upper;
      this.m_doesJointBreakKill_Lower = t.DoesJointBreakKill_Lower;
      this.m_doesSeverKill_Head = t.DoesSeverKill_Head;
      this.m_doesSeverKill_Upper = t.DoesSeverKill_Upper;
      this.m_doesSeverKill_Lower = t.DoesSeverKill_Lower;
      this.m_doesExplodeKill_Head = t.DoesExplodeKill_Head;
      this.m_doesExplodeKill_Upper = t.DoesExplodeKill_Upper;
      this.m_doesExplodeKill_Lower = t.DoesExplodeKill_Lower;
      this.Speed_Crawl = t.CrawlSpeed;
      this.Speed_Sneak = t.SneakSpeed;
      this.Speed_Walk = t.WalkSpeed;
      this.Speed_Run = t.RunSpeed;
      this.Speed_Turning = t.TurnSpeed;
      this.MovementRotMagnitude = t.MovementRotMagnitude;
      this.DamMult_Projectile = t.DamMult_Projectile;
      this.DamMult_Explosive = t.DamMult_Explosive;
      this.DamMult_Melee = t.DamMult_Melee;
      this.DamMult_Piercing = t.DamMult_Piercing;
      this.DamMult_Blunt = t.DamMult_Blunt;
      this.DamMult_Cutting = t.DamMult_Cutting;
      this.DamMult_Thermal = t.DamMult_Thermal;
      this.DamMult_Chilling = t.DamMult_Chilling;
      this.DamMult_EMP = t.DamMult_EMP;
      this.CanBeSuppresed = t.CanBeSurpressed;
      this.SuppressionMult = t.SuppressionMult;
      this.CanBeGrabbed = t.CanBeGrabbed;
      this.CanBeSevered = t.CanBeSevered;
      this.CanBeStabbed = t.CanBeStabbed;
      this.m_maxJointLimit = t.MaxJointLimit;
      if (t.OverrideSpeech)
        this.Speech = t.OverrideSpeechSet;
      for (int index = 0; index < this.Links.Count; ++index)
      {
        this.Links[index].DamMult = t.LinkDamageMultipliers[index];
        this.Links[index].StaggerMagnitude = t.LinkStaggerMultipliers[index];
        this.Links[index].SetIntegrity(UnityEngine.Random.Range(t.StartingLinkIntegrity[index].x, t.StartingLinkIntegrity[index].y));
        if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) t.StartingChanceBrokenJoint[index])
          this.Links[index].BreakJoint(true, Damage.DamageClass.Abstract);
      }
      if (this.Priority != null)
      {
        this.m_hasPriority = true;
        this.m_hasConfiguredPriority = true;
        this.Priority.Init(this.E, t.TargetCapacity, t.TargetTrackingTime, t.NoFreshTargetTime);
      }
      this.UpdateRenderers();
      if ((UnityEngine.Object) this.InvulnMaterial != (UnityEngine.Object) null)
        this.m_hasInvulnMaterial = true;
      if ((UnityEngine.Object) this.InvisMaterial != (UnityEngine.Object) null)
        this.m_hasInvisMaterial = true;
      if ((UnityEngine.Object) this.VaporizeMaterial != (UnityEngine.Object) null)
        this.m_hasVaporizeMaterial = true;
      if (!((UnityEngine.Object) this.FrozenMaterial != (UnityEngine.Object) null))
        return;
      this.m_hasFrozenMaterial = true;
    }

    private void Start()
    {
      this.Init();
      this.m_tickDownToNextStateSpeech = UnityEngine.Random.Range(5f, 20f);
      this.m_tickDownToPainSpeechAvailability = UnityEngine.Random.Range(1f, 6f);
      this.DeParentOnSpawn.SetParent((Transform) null);
      if ((UnityEngine.Object) this.TestingSosigTemplate != (UnityEngine.Object) null)
        this.Configure(this.TestingSosigTemplate);
      this.E.UsesFakedPosition = true;
      this.SetDominantGuardDirection(this.transform.forward);
      if (GM.Options.SimulationOptions.SosigClownMode)
      {
        this.DamageFX_SmallMustardBurst = FXM.GetClownFX(0);
        this.DamageFX_LargeMustardBurst = FXM.GetClownFX(1);
        this.DamageFX_MustardSpoutSmall = FXM.GetClownFX(2);
        this.DamageFX_MustardSpoutLarge = FXM.GetClownFX(3);
        this.DamageFX_Explosion = FXM.GetClownFX(4);
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(FXM.GetClownFX(5), this.Links[0].transform.position, this.Links[0].transform.rotation);
        gameObject.transform.position = this.Links[0].transform.position + this.Links[0].transform.forward * 0.12f - this.Links[0].transform.up * 0.12f;
        gameObject.transform.SetParent(this.Links[0].transform);
      }
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].O.DistantGrabbable = false;
      }
    }

    public bool IsTickingDownToClear() => this.m_isTickingDownToClear;

    public void TickDownToClear(float f)
    {
      this.m_isTickingDownToClear = true;
      this.m_tickDownToClear = Mathf.Min(this.m_tickDownToClear, f);
    }

    public void DestroyAllHeldObjects()
    {
      if (this.Inventory.Slots.Count > 0)
      {
        for (int index = this.Inventory.Slots.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.Inventory.Slots[index].HeldObject != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.Inventory.Slots[index].HeldObject.gameObject);
        }
      }
      if ((UnityEngine.Object) this.Hand_Primary.HeldObject != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.Hand_Primary.HeldObject.gameObject);
      if (!((UnityEngine.Object) this.Hand_Secondary.HeldObject != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.Hand_Secondary.HeldObject.gameObject);
    }

    public void KillSosig() => this.SosigDies(Damage.DamageClass.Abstract, Sosig.SosigDeathType.Unknown);

    public void ClearSosig()
    {
      if ((UnityEngine.Object) this.DeParentOnSpawn != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.DeParentOnSpawn.gameObject);
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].LinkExplodes(Damage.DamageClass.Abstract);
      }
      for (int index = 0; index < this.Hands.Count; ++index)
        this.Hands[index].DropHeldObject();
      this.Inventory.DropAllObjects();
      this.ClearCoverPoint();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public void DeSpawnSosig()
    {
      this.ClearCoverPoint();
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.Links[index].gameObject);
        this.DestroyAllHeldObjects();
        if ((UnityEngine.Object) this != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
    }

    private void Init()
    {
      for (int index = 0; index < this.Links.Count; ++index)
        this.IgnoreRBs.Add(this.Links[index].R);
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index].J != (UnityEngine.Object) null)
          this.m_joints.Add(this.Links[index].J);
      }
      if ((UnityEngine.Object) this.E != (UnityEngine.Object) null)
      {
        this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
        this.E.AIReceiveSuppressionEvent += new AIEntity.AIReceiveSuppression(this.SuppresionEvent);
      }
      this.Agent.Warp(this.transform.position);
      this.Agent.enabled = true;
      this.m_cachedPath = new NavMeshPath();
      if (this.Priority != null && !this.m_hasConfiguredPriority)
      {
        this.m_hasPriority = true;
        this.Priority.Init(this.E, 5, 2f, 1.5f);
      }
      this.InitHands();
      this.Inventory.Init();
      if (GM.CurrentAIManager.HasCPM)
        this.CoverSearchRange = GM.CurrentAIManager.CPM.DefaultSearchRange;
      this.m_targetPose = this.Pose_Standing;
      this.m_targetLocalPos = this.Pose_Standing.localPosition;
      this.m_targetLocalRot = this.Pose_Standing.localRotation;
      this.m_poseLocalEulers_Standing = this.Pose_Standing.localEulerAngles;
      this.m_poseLocalEulers_Crouching = this.Pose_Crouching.localEulerAngles;
      this.m_poseLocalEulers_Prone = this.Pose_Prone.localEulerAngles;
      this.UpdateJoints(1f);
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) this.E != (UnityEngine.Object) null))
        return;
      this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);
      this.E.AIReceiveSuppressionEvent -= new AIEntity.AIReceiveSuppression(this.SuppresionEvent);
    }

    private bool ShouldContinueTickDown()
    {
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null && this.Links[index].O.IsHeld)
          return false;
      }
      return true;
    }

    public bool CanCurrentlyBeHeld() => this.CanBeGrabbed && (this.m_isStunned || this.BodyState != Sosig.SosigBodyState.InControl || this.CurrentOrder != Sosig.SosigOrder.Skirmish);

    private void Update()
    {
      if (this.m_isTickingDownToClear)
      {
        if (this.ShouldContinueTickDown())
          this.m_tickDownToClear -= Time.deltaTime;
        if ((double) this.m_tickDownToClear <= 0.0)
          this.ClearSosig();
      }
      if ((double) this.m_timeSinceLastDamage < 10.0)
        this.m_timeSinceLastDamage += Time.deltaTime;
      if (this.m_isInvuln || this.m_isDamResist)
        this.m_suppressionLevel = 0.0f;
      if ((double) this.m_suppressionLevel > 0.0 && this.BodyState == Sosig.SosigBodyState.InControl)
        this.m_suppressionLevel -= Time.deltaTime * 0.25f;
      this.BrainUpdate();
      this.SpeechUpdate();
      this.HeadIconUpdate();
      this.BodyUpdate();
      this.BleedingUpdate();
      this.VaporizeUpdate();
      this.BuffUpdate();
      this.HandUpdate();
      this.LegsUpdate();
      this.EntityUpdate();
      if (!this.IsDebuggingPriority)
        return;
      if (this.Priority.HasFreshTarget() && this.DoIHaveAWeaponInMyHand())
      {
        this.PriorityTesterRay.gameObject.SetActive(true);
        this.PriorityTesterRay.position = this.transform.position + Vector3.up * 1.5f;
        Vector3 forward = this.Priority.GetTargetPoint() - this.PriorityTesterRay.position;
        this.PriorityTesterRay.rotation = Quaternion.LookRotation(forward);
        this.PriorityTesterRay.localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
      }
      else
        this.PriorityTesterRay.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
      this.SosigPhys();
      this.HandPhysUpdate();
      this.Inventory.PhysHold();
    }

    public void EventReceive(AIEvent e)
    {
      if (this.CurrentOrder == Sosig.SosigOrder.Disabled || e.IsEntity && e.Entity.IFFCode == this.E.IFFCode || (this.m_isConfused || !this.m_hasPriority))
        return;
      if (e.Type == AIEvent.AIEType.Visual)
        this.Priority.ProcessEvent(e);
      else if (e.Type == AIEvent.AIEType.Damage)
      {
        if (this.Priority.HasFreshTarget() && this.Priority.IsTargetEntity())
          return;
        this.Priority.ProcessEvent(e);
      }
      else
      {
        if (e.Type != AIEvent.AIEType.Sonic)
          return;
        this.m_alertnessLevel += Mathf.Clamp(1f - e.Value, 0.0f, 1.1f);
        if (this.Priority.HasFreshTarget() && this.Priority.IsTargetEntity())
          return;
        this.Priority.ProcessEvent(e);
      }
    }

    public void SuppresionEvent(Vector3 pos, Vector3 dir, int IFF, float intensity, float range)
    {
      if (this.CurrentOrder == Sosig.SosigOrder.Disabled || !this.HasABrain || (!this.CanBeSuppresed || this.m_isInvuln) || (this.m_isDamResist || this.DoIHaveAShieldInMyHand() || ((double) this.m_suppressionLevel >= 1.0 || IFF < 0)) || IFF == this.E.IFFCode)
        return;
      float num1 = Mathf.Clamp(Vector3.Distance(pos, this.transform.position) - 1f, 0.0f, range);
      float num2 = intensity * ((range - num1) / range);
      if ((double) num2 <= 0.0)
        return;
      dir.y = 0.0f;
      dir.z += 0.0001f;
      dir.Normalize();
      this.m_suppressionDir = Vector3.Lerp(this.m_suppressionDir, dir, 0.5f);
      this.m_suppressionDir.Normalize();
      this.m_suppressionLevel += Mathf.Clamp(num2, 0.0f, 1f) * this.SuppressionMult;
    }

    private void SpeechUpdate()
    {
      if ((double) this.m_tickDownToNextStateSpeech > 0.0)
        this.m_tickDownToNextStateSpeech -= Time.deltaTime;
      else
        this.SpeechUpdate_State();
      if ((double) this.m_tickDownToPainSpeechAvailability > 0.0)
        this.m_tickDownToPainSpeechAvailability -= Time.deltaTime;
      if ((UnityEngine.Object) this.m_speakingSource != (UnityEngine.Object) null)
      {
        this.isSpeaking = true;
        if (this.m_speakingSource.Source.isPlaying)
          return;
        this.m_speakingSource = (FVRPooledAudioSource) null;
        this.isSpeaking = false;
      }
      else
        this.isSpeaking = false;
    }

    private void SpeechUpdate_State()
    {
      if (!this.CanSpeakState())
        return;
      switch (this.CurrentOrder)
      {
        case Sosig.SosigOrder.GuardPoint:
          this.Speak_State(this.Speech.OnWander);
          break;
        case Sosig.SosigOrder.Investigate:
          this.Speak_State(this.Speech.OnInvestigate);
          break;
        case Sosig.SosigOrder.SearchForEquipment:
          this.Speak_State(this.Speech.OnSearchingForGuns);
          break;
        case Sosig.SosigOrder.TakeCover:
          this.Speak_State(this.Speech.OnTakingCover);
          break;
        case Sosig.SosigOrder.Wander:
          this.Speak_State(this.Speech.OnWander);
          break;
        case Sosig.SosigOrder.Assault:
          this.Speak_State(this.Speech.OnAssault);
          break;
      }
    }

    private float GetSpeakDelay()
    {
      if (this.Speech.LessTalkativeSkirmish && this.CurrentOrder == Sosig.SosigOrder.Skirmish)
        return UnityEngine.Random.Range(2f, 6f);
      return this.CurrentOrder == Sosig.SosigOrder.Investigate ? UnityEngine.Random.Range(3f, 10f) : UnityEngine.Random.Range(8f, 45f);
    }

    public void KillSpeech()
    {
      if (!((UnityEngine.Object) this.m_speakingSource != (UnityEngine.Object) null))
        return;
      this.m_speakingSource.Source.Stop();
      this.m_speakingSource = (FVRPooledAudioSource) null;
      this.isSpeaking = false;
    }

    public bool CanSpeakState() => (double) this.m_tickDownToNextStateSpeech <= 0.0 && this.IsAllowedToSpeak && (!this.IsConfused && !this.IsStunned) && (!this.m_recoveringFromBallisticState && !this.isSpeaking);

    private void Speak_State(List<AudioClip> clips)
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead || clips.Count <= 0)
        return;
      AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
      this.m_tickDownToNextStateSpeech = clip.length + this.GetSpeakDelay();
      Vector3 position = this.transform.position;
      bool flag = true;
      if ((UnityEngine.Object) this.Links[0] != (UnityEngine.Object) null)
      {
        position = this.Links[0].transform.position;
        flag = false;
      }
      float num = 1f;
      if (this.IsFrozen)
        num = 0.8f;
      if (this.IsSpeedUp)
        num = 1.8f;
      this.m_speakingSource = GM.CurrentAIManager.Speak(clip, this.Speech.BaseVolume, this.Speech.BasePitch * num, position, AIManager.SpeakType.chat);
      if (!((UnityEngine.Object) this.m_speakingSource != (UnityEngine.Object) null) || flag)
        return;
      this.m_speakingSource.FollowThisTransform(this.Links[0].transform);
    }

    private void DelayedSpeakPain() => this.Speak_Pain(this.Speech.OnPain);

    private bool CanSpeakPain() => (double) this.m_tickDownToPainSpeechAvailability <= 0.0 && !this.isSpeaking;

    private void Speak_Pain(List<AudioClip> clips)
    {
      bool flag1 = false;
      if (this.Speech.ForceDeathSpeech && (clips == this.Speech.OnDeath || clips == this.Speech.OnDeathAlt))
        flag1 = true;
      if (this.BodyState == Sosig.SosigBodyState.Dead && !flag1 || clips.Count <= 0 || !this.CanSpeakPain() && !flag1)
        return;
      if (flag1)
        this.KillSpeech();
      AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
      this.m_tickDownToPainSpeechAvailability = clip.length + UnityEngine.Random.Range(1.1f, 1.2f);
      Vector3 position = this.transform.position;
      bool flag2 = true;
      if ((UnityEngine.Object) this.Links[0] != (UnityEngine.Object) null)
      {
        position = this.Links[0].transform.position;
        flag2 = false;
      }
      float num = 1f;
      if (this.IsFrozen)
        num = 0.8f;
      if (this.IsSpeedUp)
        num = 1.8f;
      this.m_speakingSource = !flag1 ? GM.CurrentAIManager.Speak(clip, this.Speech.BaseVolume, this.Speech.BasePitch * num, position, AIManager.SpeakType.pain) : GM.CurrentAIManager.Speak(clip, this.Speech.BaseVolume, this.Speech.BasePitch * num, position, AIManager.SpeakType.death);
      if (!((UnityEngine.Object) this.m_speakingSource != (UnityEngine.Object) null) || flag2)
        return;
      this.m_speakingSource.FollowThisTransform(this.Links[0].transform);
    }

    private void EntityUpdate()
    {
      if ((double) this.m_entityJiggleTick > 0.0)
        this.m_entityJiggleTick -= Time.deltaTime;
      else
        this.m_entityJiggleTick = UnityEngine.Random.Range(0.1f, 1f);
      if (this.m_linksDestroyed[0])
        this.E.MaxHearingDistance = 3f;
      if (this.m_isBlinded || this.BodyState == Sosig.SosigBodyState.Dead)
      {
        this.E.ReceivesEvent_Visual = false;
        this.E.ReceivesEvent_Sonic = false;
        this.E.ReceivesEvent_Damage = false;
      }
      else
      {
        this.E.ReceivesEvent_Visual = true;
        this.E.ReceivesEvent_Sonic = true;
        this.E.ReceivesEvent_Damage = true;
      }
      switch (this.BodyPose)
      {
        case Sosig.SosigBodyPose.Standing:
          this.E.VisibilityMultiplier = 1f;
          break;
        case Sosig.SosigBodyPose.Crouching:
          this.E.VisibilityMultiplier = 1f;
          break;
        case Sosig.SosigBodyPose.Prone:
          this.E.VisibilityMultiplier = 1.5f;
          break;
      }
      if (this.m_isSuperVisible)
        this.E.VisibilityMultiplier = 0.3f;
      if (this.m_isGhosted)
        this.E.VisibilityMultiplier = 2.5f;
      float num = 0.1f;
      if (!this.DoIHaveAGun())
        num += 0.03f;
      if (!this.DoIHaveAWeaponInMyHand())
        num += 0.1f;
      if (this.m_isHobbled)
        num += 0.1f;
      if (this.m_isBackBroken)
        num += 0.2f;
      if (this.m_isBlinded)
        num += 0.3f;
      if (this.m_isConfused)
        num += 0.2f;
      switch (this.BodyState)
      {
        case Sosig.SosigBodyState.Ballistic:
          num += 0.8f;
          break;
        case Sosig.SosigBodyState.Dead:
          num += 5f;
          break;
      }
      this.E.DangerMultiplier = num;
    }

    public void HeadIconUpdate()
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead)
        this.SetHeadIcon(Sosig.SosigHeadIconState.None);
      else if (this.m_isBlinded)
        this.SetHeadIcon(Sosig.SosigHeadIconState.Blinded);
      else if (this.m_isConfused)
        this.SetHeadIcon(Sosig.SosigHeadIconState.Confused);
      else if ((double) this.m_suppressionLevel > 0.200000002980232)
        this.SetHeadIcon(Sosig.SosigHeadIconState.Suppressed);
      else if (this.CurrentOrder == Sosig.SosigOrder.Skirmish)
        this.SetHeadIcon(Sosig.SosigHeadIconState.Exclamation);
      else if (this.CurrentOrder == Sosig.SosigOrder.Investigate)
        this.SetHeadIcon(Sosig.SosigHeadIconState.Investigating);
      else
        this.SetHeadIcon(Sosig.SosigHeadIconState.None);
      if (this.m_linksDestroyed[0] || this.HeadIconState != Sosig.SosigHeadIconState.Confused)
        return;
      this.m_rotato += Time.deltaTime * 180f;
      if ((double) this.m_rotato > 360.0)
        this.m_rotato -= 360f;
      this.HeadIcons[1].transform.localEulerAngles = new Vector3(0.0f, this.m_rotato, 0.0f);
    }

    public void SetHeadIcon(Sosig.SosigHeadIconState state)
    {
      if (this.HeadIconState == state)
        return;
      this.HeadIconState = state;
      if (this.m_linksDestroyed[0] || !((UnityEngine.Object) this.Links[0] != (UnityEngine.Object) null))
        return;
      for (int index = 0; index < this.HeadIcons.Count; ++index)
      {
        if ((Sosig.SosigHeadIconState) index == state - 1)
        {
          if (!this.HeadIcons[index].activeSelf)
            this.HeadIcons[index].SetActive(true);
        }
        else if (this.HeadIcons[index].activeSelf)
          this.HeadIcons[index].SetActive(false);
      }
    }

    public void SetGuardInvestigateDistanceThreshold(float d) => this.m_guardInvestigateDistanceThreshold = d;

    public Vector3 GetGuardPoint() => this.m_guardPoint;

    public Vector3 GetGuardDir() => this.m_guardDominantDirection;

    public void SetAssaultPointOverrideDistance(float f) => this.m_assaultPointOverridesSkirmishPointWhenFurtherThan = f;

    public Vector3 GetAssaultPoint() => this.m_assaultPoint;

    private void BrainUpdate()
    {
      switch (this.CurrentOrder)
      {
        case Sosig.SosigOrder.Disabled:
          this.BrainUpdate_Disabled();
          break;
        case Sosig.SosigOrder.GuardPoint:
          this.BrainUpdate_GuardPoint();
          break;
        case Sosig.SosigOrder.Skirmish:
          this.BrainUpdate_Skirmish();
          break;
        case Sosig.SosigOrder.Investigate:
          this.BrainUpdate_Investigate();
          break;
        case Sosig.SosigOrder.SearchForEquipment:
          this.BrainUpdate_SearchForEquipment();
          break;
        case Sosig.SosigOrder.TakeCover:
          this.BrainUpdate_TakeCover();
          break;
        case Sosig.SosigOrder.Wander:
          this.BrainUpdate_Wander();
          break;
        case Sosig.SosigOrder.Assault:
          this.BrainUpdate_Assault();
          break;
      }
      this.AlertnessUpdate();
    }

    public void SetCurrentOrder(Sosig.SosigOrder o)
    {
      if (this.CurrentOrder == o)
        return;
      this.CurrentOrder = o;
      if (o != Sosig.SosigOrder.Skirmish)
        this.ClearCoverPoint();
      switch (this.CurrentOrder)
      {
        case Sosig.SosigOrder.Skirmish:
          this.m_tickDownToNextStateSpeech = Mathf.Max(this.m_tickDownToNextStateSpeech, UnityEngine.Random.Range(0.4f, 5f));
          GM.CurrentSceneSettings.OnPerceiveableSound(30f, 20f * SM.GetSoundTravelDistanceMultByEnvironment(SM.GetReverbEnvironment(this.transform.position).Environment), this.transform.position, 0);
          break;
        case Sosig.SosigOrder.Investigate:
          this.m_tickDownToNextStateSpeech = UnityEngine.Random.Range(0.1f, 0.25f);
          this.m_investigateCooldown = UnityEngine.Random.Range(8f, 11f);
          break;
        case Sosig.SosigOrder.Wander:
          this.m_tickDownToNextStateSpeech = UnityEngine.Random.Range(15f, 25f);
          break;
      }
    }

    public void CommandAssaultPoint(Vector3 point)
    {
      this.SetCurrentOrder(Sosig.SosigOrder.Assault);
      this.FallbackOrder = Sosig.SosigOrder.Assault;
      this.m_assaultPoint = point;
    }

    public void UpdateAssaultPoint(Vector3 point) => this.m_assaultPoint = point;

    public void SetAssaultSpeed(Sosig.SosigMoveSpeed speed) => this.m_assaultSpeed = speed;

    public void UpdateGuardPoint(Vector3 point) => this.m_guardPoint = point;

    public void CommandGuardPoint(Vector3 point, bool hardguard)
    {
      this.SetCurrentOrder(Sosig.SosigOrder.GuardPoint);
      this.FallbackOrder = Sosig.SosigOrder.GuardPoint;
      this.m_guardPoint = point;
      this.m_hardGuard = hardguard;
    }

    private void AlertnessUpdate()
    {
      if (this.CurrentOrder == Sosig.SosigOrder.Assault || this.CurrentOrder == Sosig.SosigOrder.Skirmish || (this.FallbackOrder == Sosig.SosigOrder.Assault || this.FallbackOrder == Sosig.SosigOrder.Skirmish))
        this.m_alertnessLevel = 1f;
      else if (this.CurrentOrder != Sosig.SosigOrder.SearchForEquipment && this.CurrentOrder != Sosig.SosigOrder.TakeCover)
      {
        if (this.Priority.HasFreshTarget() && this.Priority.IsTargetEntity())
        {
          Mathf.Lerp(2f, 0.2f, Vector3.Distance(this.Priority.GetTargetPoint(), this.transform.position) * 0.01f);
          this.m_alertnessLevel += Time.deltaTime * 1f;
        }
        else if (this.Priority.HasFreshTarget())
          this.m_alertnessLevel += Time.deltaTime * 0.01f;
        else
          this.m_alertnessLevel -= Time.deltaTime * 0.025f;
      }
      this.m_alertnessLevel = Mathf.Clamp(this.m_alertnessLevel, 0.0f, 1.25f);
    }

    private bool StateBailCheck_Equipment()
    {
      if (this.m_isBlinded)
        return false;
      if ((double) this.m_searchForEquipmentCoolDown > 0.0)
        this.m_searchForEquipmentCoolDown -= Time.deltaTime;
      else if (!this.DoIHaveAWeaponAtAll())
      {
        this.m_searchForEquipmentTickDown = UnityEngine.Random.Range(5f, 10f);
        this.SetCurrentOrder(Sosig.SosigOrder.SearchForEquipment);
        return true;
      }
      return false;
    }

    private bool StateBailCheck_ShouldISkirmish()
    {
      if (this.m_isBlinded)
        return false;
      if (this.Priority.HasFreshTarget())
      {
        if (this.Priority.IsTargetEntity())
        {
          float num = 0.5f;
          if (this.CurrentOrder == Sosig.SosigOrder.Investigate)
            num = 1f;
          if ((double) this.m_entityRecognition < 1.0)
            this.m_entityRecognition += Time.deltaTime * num;
          if ((double) this.m_entityRecognition >= 1.0)
          {
            this.SetCurrentOrder(Sosig.SosigOrder.Skirmish);
            return true;
          }
        }
        else if ((double) this.m_entityRecognition > 0.0)
          this.m_entityRecognition -= Time.deltaTime;
        if (this.CurrentOrder != Sosig.SosigOrder.Investigate && (double) this.m_alertnessLevel >= 1.0)
        {
          this.SetCurrentOrder(Sosig.SosigOrder.Investigate);
          this.m_investigateCooldown = UnityEngine.Random.Range(8f, 11f);
          return true;
        }
      }
      else if ((double) this.m_entityRecognition > 0.0)
        this.m_entityRecognition -= Time.deltaTime;
      return false;
    }

    private bool StateBailCheck_ShouldITakeCover() => this.m_isBlinded && false;

    private void WeaponEquipCycle()
    {
      this.m_weaponSwapCheckTick -= Time.deltaTime;
      if ((double) this.m_weaponSwapCheckTick > 0.0)
        return;
      this.m_weaponSwapCheckTick = UnityEngine.Random.Range(this.m_weaponSwapTickRange.x, this.m_weaponSwapTickRange.y);
      if (!this.EquipBestPrimary())
        return;
      this.EquipSecondaryCycle();
    }

    public void SetDominantGuardDirection(Vector3 v) => this.m_guardDominantDirection = v;

    private void RandomLookCycle(float TickDownSpeedMult, float TickRangeMult)
    {
      float num = 1f;
      if (this.m_isStunned)
        num = 0.1f;
      if (this.m_isConfused)
        num = 6f;
      this.m_tickDownToRandomLook -= Time.deltaTime * TickDownSpeedMult * num;
      if ((double) this.m_tickDownToRandomLook > 0.0)
        return;
      this.m_tickDownToRandomLook = UnityEngine.Random.Range(this.m_randomLookTickRange.x, this.m_randomLookTickRange.y) * TickRangeMult;
      this.m_faceTowards = this.GetRandomLookDir();
    }

    private void RandomLookCycleDominantDirection(float TickDownSpeedMult, float TickRangeMult)
    {
      float num = 1f;
      if (this.m_isStunned)
        num = 0.1f;
      if (this.m_isConfused)
        num = 6f;
      this.m_tickDownToRandomLook -= Time.deltaTime * TickDownSpeedMult * num;
      if ((double) this.m_tickDownToRandomLook > 0.0)
        return;
      this.m_tickDownToRandomLook = UnityEngine.Random.Range(this.m_randomLookTickRange.x, this.m_randomLookTickRange.y) * TickRangeMult;
      this.m_faceTowards = Vector3.Lerp(this.m_guardDominantDirection, this.GetRandomLookDir(), 0.2f);
      this.m_faceTowards.y = 0.0f;
      this.m_faceTowards.Normalize();
    }

    private void RandomWanderCycle(float TickDownSpeedMult, float TickRangeMult)
    {
      this.m_tickDownToRandomWanderPoint -= Time.deltaTime * TickDownSpeedMult;
      if ((double) this.m_tickDownToRandomWanderPoint > 0.0)
        return;
      this.m_tickDownToRandomWanderPoint = UnityEngine.Random.Range(this.m_randomWanderPointRange.x, this.m_randomWanderPointRange.y) * TickRangeMult;
      this.m_wanderPoint = this.GetNewRandomWanderPoint(this.Agent.transform.position);
    }

    private void BrainUpdate_Disabled()
    {
      this.SetBodyPose(Sosig.SosigBodyPose.Standing);
      this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest);
      this.SetMovementState(Sosig.SosigMovementState.Idle);
    }

    private void BrainUpdate_GuardPoint()
    {
      if (this.m_hasPriority)
        this.Priority.Compute();
      if (this.StateBailCheck_ShouldITakeCover() || this.StateBailCheck_ShouldISkirmish())
        return;
      this.WeaponEquipCycle();
      this.EquipmentScanCycle(new Vector3(3f, 3f, 3f), 1f);
      if (this.Priority.HasFreshTarget())
      {
        this.m_faceTowards = this.Priority.GetTargetPoint() - this.Agent.transform.position;
        this.m_faceTowards.y = 1f / 1000f;
      }
      else
        this.RandomLookCycleDominantDirection(1f, 1f);
      this.TryToGetTo(this.m_guardPoint);
      this.SetMovementSpeedBasedOnDistance();
      this.SetMovementState(Sosig.SosigMovementState.MoveToPoint);
      this.SetBodyPose(Sosig.SosigBodyPose.Standing);
      this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest);
    }

    private void BrainUpdate_Wander()
    {
      if (this.m_hasPriority)
        this.Priority.Compute();
      if (this.StateBailCheck_Equipment() || this.StateBailCheck_ShouldITakeCover() || this.StateBailCheck_ShouldISkirmish())
        return;
      this.WeaponEquipCycle();
      this.EquipmentScanCycle(new Vector3(10f, 3f, 10f), 0.2f);
      this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest);
      if (this.m_hasTargetEquipment && (UnityEngine.Object) this.m_targetEquipmentToPickup != (UnityEngine.Object) null)
      {
        this.RandomLookCycle(1f, 1f);
        if (this.m_isConfused || this.m_isBlinded)
          this.TryToGetTo(this.m_wanderPoint);
        else
          this.TryToGetTo(this.m_targetEquipmentToPickup.transform.position);
      }
      else
      {
        this.RandomLookCycle(1f, 0.2f);
        this.RandomWanderCycle(1f, 1f);
        this.TryToGetTo(this.m_wanderPoint);
      }
      this.SetMovementSpeed(Sosig.SosigMoveSpeed.Walking);
      this.SetMovementState(Sosig.SosigMovementState.MoveToPoint);
      this.SetBodyPose(Sosig.SosigBodyPose.Standing);
    }

    private void ClearCoverPoint()
    {
      if (!((UnityEngine.Object) this.m_curCoverPoint != (UnityEngine.Object) null))
        return;
      this.m_curCoverPoint.IsClaimed = false;
      this.m_curCoverPoint = (AICoverPoint) null;
    }

    private void ShouldIPickANewSkirmishPoint(bool hasAGun, bool isReloading)
    {
      if (hasAGun || !this.DoIHaveAWeaponInMyHand())
      {
        this.m_tickDownToPickNewSkirmishPoint -= Time.deltaTime;
        if ((double) this.m_tickDownToPickNewSkirmishPoint > 0.0)
          return;
        this.m_tickDownToPickNewSkirmishPoint = !GM.CurrentAIManager.HasCPM ? UnityEngine.Random.Range(0.1f, 1f) : 0.1f;
        if (this.AmIReloading() || (UnityEngine.Object) this.m_curCoverPoint != (UnityEngine.Object) null && (double) this.m_suppressionLevel > 0.5)
          return;
        this.m_skirmishPoint = this.GetNewRandomSkirmishPoint(this.Agent.transform.position);
      }
      else
      {
        if (hasAGun || !this.DoIHaveAWeaponInMyHand() || (!this.Priority.HasFreshTarget() || !this.Priority.IsTargetEntity()))
          return;
        Vector3 sourcePosition = this.Priority.GetTargetGroundPoint() + this.CoreTarget.right * this.horizOffset;
        Vector3 vector3 = this.Agent.transform.position - sourcePosition;
        vector3.y = 0.0f;
        SosigWeapon heldMeleeWeapon = this.GetHeldMeleeWeapon();
        if (heldMeleeWeapon.MeleeState == SosigWeapon.SosigMeleeState.Attacking)
        {
          if (!this.m_wasAttacking)
          {
            this.horizOffset = UnityEngine.Random.Range(-0.2f, 0.2f);
            this.DesiredMeleeDistance = UnityEngine.Random.Range(heldMeleeWeapon.CloseAttackRange.x, heldMeleeWeapon.CloseAttackRange.y);
          }
          this.m_wasAttacking = true;
        }
        else
        {
          if (this.m_wasAttacking)
          {
            this.horizOffset = UnityEngine.Random.Range(-0.8f, 0.8f);
            this.DesiredMeleeDistance = UnityEngine.Random.Range(heldMeleeWeapon.DistantAttackRange.x, heldMeleeWeapon.DistantAttackRange.x);
          }
          this.m_wasAttacking = false;
        }
        float desiredMeleeDistance = this.DesiredMeleeDistance;
        if (NavMesh.Raycast(sourcePosition, sourcePosition + vector3.normalized * desiredMeleeDistance, out this.m_navMeshHit, -1))
          this.m_skirmishPoint = sourcePosition + vector3.normalized * this.m_navMeshHit.distance * 0.8f;
        else
          this.m_skirmishPoint = sourcePosition + vector3.normalized * desiredMeleeDistance;
      }
    }

    private Vector3 GetNewRandomSkirmishPoint(Vector3 startPos)
    {
      Vector2 engagementDistance = this.GetPreferedEngagementDistance();
      Vector3 targetPoint = this.Priority.GetTargetPoint();
      if (GM.CurrentAIManager.HasCPM && this.DoIHaveAGun())
      {
        Vector3 goalPos = Vector3.zero;
        Vector3 suppressionDir = this.SuppressionDir;
        bool usesTargetPoint = true;
        bool usesGoalPoint = false;
        if (this.FallbackOrder == Sosig.SosigOrder.Assault)
        {
          usesGoalPoint = true;
          goalPos = this.m_assaultPoint;
        }
        bool usesTakeCoverFromDir = (double) this.m_suppressionLevel > 0.200000002980232;
        float nextSearchRange = 0.0f;
        AICoverPoint cp;
        if (GM.CurrentAIManager.CPM.GetBestTacticalPoint(this.CoverSearchRange, this.transform.position, targetPoint, goalPos, suppressionDir, engagementDistance, usesTargetPoint, usesGoalPoint, usesTakeCoverFromDir, out cp, this.m_curCoverPoint, out nextSearchRange))
        {
          this.ClearCoverPoint();
          this.m_curCoverPoint = cp;
          cp.IsClaimed = true;
          return this.m_curCoverPoint.Pos;
        }
        this.CoverSearchRange = nextSearchRange;
      }
      Vector3 sourcePosition = this.Agent.transform.position;
      Vector3 vector3_1 = targetPoint - this.transform.position;
      float magnitude = vector3_1.magnitude;
      Vector3 vector3_2 = Vector3.zero;
      if ((double) magnitude < (double) engagementDistance.x)
        vector3_2 = vector3_1.normalized * -2f;
      else if ((double) magnitude > (double) engagementDistance.y)
        vector3_2 = vector3_1.normalized * 2f;
      Vector3 vector3_3 = UnityEngine.Random.onUnitSphere + vector3_2;
      vector3_3.y = 0.0f;
      float num = UnityEngine.Random.Range(0.5f, 3f);
      if (!this.DoIHaveAGun() && this.Priority.HasFreshTarget() && this.Priority.IsTargetEntity())
      {
        sourcePosition = this.Priority.GetTargetGroundPoint();
        num = UnityEngine.Random.Range(0.7f, 1.1f);
      }
      return NavMesh.Raycast(sourcePosition, sourcePosition + vector3_3.normalized * num, out this.m_navMeshHit, -1) ? sourcePosition + vector3_3.normalized * this.m_navMeshHit.distance * 0.5f : sourcePosition + vector3_3.normalized * num * UnityEngine.Random.Range(0.6f, 1f);
    }

    private void BrainUpdate_Skirmish()
    {
      if (this.m_hasPriority)
        this.Priority.Compute();
      if (this.StateBailCheck_Equipment() || this.StateBailCheck_ShouldITakeCover())
        return;
      this.WeaponEquipCycle();
      this.EquipmentScanCycle(new Vector3(this.EquipmentPickupDistance, 3f, this.EquipmentPickupDistance), 1.5f);
      if (!this.Priority.HasFreshTarget())
      {
        this.SetCurrentOrder(Sosig.SosigOrder.Investigate);
      }
      else
      {
        bool hasAGun = this.DoIHaveAGun();
        bool flag1 = this.DoIHaveAWeaponInMyHand();
        bool isReloading = this.AmIReloading();
        bool flag2 = true;
        Vector3 v = this.m_skirmishPoint;
        if (this.FallbackOrder == Sosig.SosigOrder.Assault && (double) Vector3.Distance(this.Priority.GetTargetPoint(), this.Agent.transform.position) > (double) this.m_assaultPointOverridesSkirmishPointWhenFurtherThan)
        {
          v = this.m_assaultPoint;
          flag2 = false;
        }
        if (flag2)
        {
          this.ShouldIPickANewSkirmishPoint(hasAGun, isReloading);
          v = this.m_skirmishPoint;
        }
        this.TryToGetTo(v);
        this.m_faceTowards = this.Priority.GetTargetPoint() - this.Agent.transform.position;
        this.m_faceTowards.y = 0.0f;
        this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.AttackTarget);
        if (hasAGun)
        {
          if ((double) this.m_suppressionLevel > 0.200000002980232 && !this.Agent.isOnOffMeshLink)
          {
            if ((UnityEngine.Object) this.m_curCoverPoint != (UnityEngine.Object) null)
            {
              if ((double) Vector3.Distance(this.m_curCoverPoint.Pos, this.transform.position) > 2.0)
                this.SetMovementSpeed(Sosig.SosigMoveSpeed.Running);
              else
                this.SetMovementSpeed(Sosig.SosigMoveSpeed.Walking);
            }
            else
              this.SetMovementSpeed(Sosig.SosigMoveSpeed.Sneaking);
          }
          else if (isReloading)
            this.SetMovementSpeed(Sosig.SosigMoveSpeed.Walking);
          else
            this.SetMovementSpeed(Sosig.SosigMoveSpeed.Running);
        }
        else
          this.SetMovementSpeed(Sosig.SosigMoveSpeed.Running);
        if (this.CanSpeakState())
        {
          if (isReloading && this.Speech.OnReloading.Count > 0)
            this.Speak_State(this.Speech.OnReloading);
          else if ((this.IsHealing || this.IsInvuln) && this.Speech.OnMedic.Count > 0)
            this.Speak_State(this.Speech.OnMedic);
          else
            this.Speak_State(this.Speech.OnSkirmish);
        }
        this.m_timeTilReloadShout -= Time.deltaTime;
        if ((double) this.m_timeTilReloadShout <= 0.0)
        {
          this.m_timeTilReloadShout = UnityEngine.Random.Range(8f, 20f);
          if (this.CanSpeakState() && this.Speech.OnReloading.Count > 0)
            this.Speak_State(this.Speech.OnReloading);
        }
        if (this.HasABrain && !this.Agent.isOnOffMeshLink && ((double) this.m_suppressionLevel > 0.200000002980232 || !flag1 || isReloading))
        {
          if (isReloading || !flag1 || (double) this.m_suppressionLevel > 0.800000011920929)
            this.SetBodyPose(Sosig.SosigBodyPose.Prone);
          else
            this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        }
        else if ((double) this.Agent.velocity.magnitude > 0.400000005960464)
          this.SetBodyPose(Sosig.SosigBodyPose.Standing);
        else
          this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
      }
    }

    private void BrainUpdate_Investigate()
    {
      if (this.m_hasPriority)
        this.Priority.Compute();
      if (this.StateBailCheck_Equipment() || this.StateBailCheck_ShouldITakeCover() || this.StateBailCheck_ShouldISkirmish())
        return;
      if (!this.Priority.HasFreshTarget())
      {
        this.m_investigateCooldown -= Time.deltaTime;
        if (this.FallbackOrder == Sosig.SosigOrder.Assault && this.m_assaultSpeed == Sosig.SosigMoveSpeed.Running)
          this.m_investigateCooldown -= Time.deltaTime * 2f;
        if ((double) this.m_investigateCooldown <= 0.0)
        {
          this.SetCurrentOrder(this.FallbackOrder);
          return;
        }
      }
      this.WeaponEquipCycle();
      this.EquipmentScanCycle(new Vector3(this.EquipmentPickupDistance, 3f, this.EquipmentPickupDistance), 0.2f);
      this.m_investigateNoiseTick -= Time.deltaTime;
      if ((double) this.m_investigateNoiseTick <= 0.0)
      {
        this.m_investigateNoiseTick = UnityEngine.Random.Range(0.5f, 2f);
        this.m_investigateNoiseDir = UnityEngine.Random.onUnitSphere;
        this.m_investigateNoiseDir.y = 0.0f;
        this.m_investigateNoiseDir.Normalize();
        this.m_investigateNoiseDir *= UnityEngine.Random.Range(0.5f, 3f);
      }
      bool flag = true;
      Vector3 targetPoint = this.Priority.GetTargetPoint();
      Vector3 position = this.transform.position;
      if (this.FallbackOrder == Sosig.SosigOrder.GuardPoint)
      {
        Vector3 guardPoint = this.m_guardPoint;
        float num1 = Vector3.Distance(targetPoint, guardPoint);
        float num2 = this.m_guardInvestigateDistanceThreshold;
        if (this.m_hardGuard)
          num2 = this.m_guardInvestigateDistanceThreshold * 0.5f;
        if ((double) num1 > (double) num2)
          flag = false;
      }
      if ((double) this.m_alertnessLevel >= 0.5)
      {
        if (flag)
        {
          this.TryToGetTo(targetPoint + this.m_investigateNoiseDir);
          this.m_wanderPoint = targetPoint;
        }
        if (this.DoIHaveAGun())
          this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.AimAtReady);
        else
          this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest);
      }
      else
        this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest);
      Vector3 vector3 = targetPoint - this.Agent.transform.position;
      vector3.y = 0.0f;
      float magnitude = vector3.magnitude;
      if ((double) this.m_suppressionLevel > 0.200000002980232 && this.HasABrain)
      {
        this.SetMovementSpeed(Sosig.SosigMoveSpeed.Sneaking);
        this.SetBodyPose(Sosig.SosigBodyPose.Prone);
        this.m_faceTowards = -this.SuppressionDir;
      }
      else if ((double) magnitude > 20.0)
      {
        this.SetMovementSpeed(Sosig.SosigMoveSpeed.Running);
        this.SetBodyPose(Sosig.SosigBodyPose.Standing);
        this.m_faceTowards = vector3;
      }
      else if ((double) magnitude > 6.0)
      {
        this.SetMovementSpeed(Sosig.SosigMoveSpeed.Walking);
        this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        this.m_faceTowards = vector3;
      }
      else if ((double) magnitude > 3.0)
      {
        this.SetMovementSpeed(Sosig.SosigMoveSpeed.Sneaking);
        this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        this.RandomLookCycle(5f, 0.2f);
      }
      else
      {
        this.SetMovementSpeed(Sosig.SosigMoveSpeed.Running);
        this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        this.m_faceTowards = vector3;
      }
    }

    private void BrainUpdate_TakeCover()
    {
      if (this.BodyState == Sosig.SosigBodyState.InControl && !this.m_hasDoveYet)
      {
        this.m_hasDoveYet = true;
        this.SetBodyState(Sosig.SosigBodyState.Ballistic);
        this.m_recoveryFromBallisticTick = UnityEngine.Random.Range(1f, 1.5f);
        for (int index = 0; index < this.Links.Count; ++index)
        {
          if (!this.m_linksDestroyed[index] && !this.m_jointsSevered[index])
            this.Links[index].R.velocity = this.m_takeCoverDiveDir * UnityEngine.Random.Range(4f, 6f);
        }
      }
      else
      {
        if (this.BodyState != Sosig.SosigBodyState.InControl || !this.m_hasDoveYet)
          return;
        this.SetCurrentOrder(Sosig.SosigOrder.Skirmish);
      }
    }

    private void BrainUpdate_SearchForEquipment()
    {
      if (this.m_hasPriority)
        this.Priority.Compute();
      if (this.DoIHaveAWeaponAtAll())
      {
        this.SetCurrentOrder(this.FallbackOrder);
      }
      else
      {
        this.EquipmentScanCycle(new Vector3(10f, 3f, 10f), 2f);
        this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest);
        if ((UnityEngine.Object) this.m_targetEquipmentToPickup == (UnityEngine.Object) null || this.m_targetEquipmentToPickup.IFFCode != -2)
          this.m_hasTargetEquipment = false;
        if (this.m_hasTargetEquipment)
        {
          this.RandomLookCycle(1f, 1f);
          if (this.m_isConfused || this.m_isBlinded)
            this.TryToGetTo(this.m_wanderPoint);
          else
            this.TryToGetTo(this.m_targetEquipmentToPickup.transform.position);
        }
        else
        {
          this.RandomLookCycle(3f, 1f);
          this.RandomWanderCycle(3f, 1f);
          this.TryToGetTo(this.m_wanderPoint);
        }
        this.SetMovementSpeed(Sosig.SosigMoveSpeed.Running);
        this.SetMovementState(Sosig.SosigMovementState.MoveToPoint);
        if ((double) this.Agent.velocity.magnitude > 0.400000005960464)
          this.SetBodyPose(Sosig.SosigBodyPose.Standing);
        else
          this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        this.WeaponEquipCycle();
        if (!this.Priority.HasFreshTarget() || !this.Priority.IsTargetEntity() || (double) Vector3.Angle(-Vector3.ProjectOnPlane(this.m_faceTowards, Vector3.up), this.Priority.RecentEvents[0].Entity.SensoryFrame.forward) >= 15.0)
          return;
        this.SetCurrentOrder(Sosig.SosigOrder.TakeCover);
        this.m_hasDoveYet = false;
        Vector3 a = this.Priority.RecentEvents[0].Entity.SensoryFrame.right;
        if ((double) UnityEngine.Random.value > 0.5)
          a = -a;
        Vector3 vector3 = Vector3.Slerp(a, UnityEngine.Random.onUnitSphere, 0.3f);
        vector3.y = 0.0f;
        this.m_takeCoverDiveDir = vector3;
      }
    }

    private void BrainUpdate_Assault()
    {
      if (this.m_hasPriority)
        this.Priority.Compute();
      if (this.StateBailCheck_Equipment() || this.StateBailCheck_ShouldITakeCover() || this.StateBailCheck_ShouldISkirmish())
        return;
      this.WeaponEquipCycle();
      this.EquipmentScanCycle(new Vector3(this.EquipmentPickupDistance, 3f, this.EquipmentPickupDistance), 0.2f);
      this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.AimAtReady);
      Vector3 vector3_1 = this.m_assaultPoint - this.Agent.transform.position;
      vector3_1.y = 0.0f;
      float magnitude = vector3_1.magnitude;
      this.TryToGetTo(this.m_assaultPoint);
      this.m_timeTilAssaultDirChange -= Time.deltaTime;
      if ((double) this.m_timeTilAssaultDirChange <= 0.0)
      {
        if (this.m_assaultSpeed == Sosig.SosigMoveSpeed.Crawling || this.m_assaultSpeed == Sosig.SosigMoveSpeed.Sneaking)
        {
          this.m_timeTilAssaultDirChange = UnityEngine.Random.Range(3f, 10f);
          this.m_assaultCross = UnityEngine.Random.Range(0.8f, -0.8f);
        }
        else if (this.m_assaultSpeed == Sosig.SosigMoveSpeed.Walking)
        {
          this.m_timeTilAssaultDirChange = UnityEngine.Random.Range(2f, 7f);
          this.m_assaultCross = UnityEngine.Random.Range(0.5f, -0.5f);
        }
        else if (this.m_assaultSpeed == Sosig.SosigMoveSpeed.Running)
        {
          this.m_timeTilAssaultDirChange = UnityEngine.Random.Range(1f, 5f);
          this.m_assaultCross = UnityEngine.Random.Range(0.2f, -0.2f);
        }
      }
      Vector3 velocity = this.Agent.velocity;
      if ((double) velocity.magnitude < 0.00999999977648258)
        velocity += this.transform.forward;
      Vector3 b = Vector3.Cross(velocity.normalized, Vector3.up);
      if ((double) this.m_assaultCross < 0.0)
        b = Vector3.Cross(velocity.normalized, -Vector3.up);
      Vector3 vector3_2 = Vector3.Slerp(this.m_faceTowards, Vector3.Slerp(velocity, b, Mathf.Abs(this.m_assaultCross)).normalized, Time.deltaTime);
      vector3_2.y = 0.0f;
      int assaultSpeed = (int) this.m_assaultSpeed;
      if ((double) magnitude > 10.0)
      {
        this.SetMovementSpeed((Sosig.SosigMoveSpeed) Mathf.Min(assaultSpeed, 3));
        this.SetBodyPose(Sosig.SosigBodyPose.Standing);
        this.m_faceTowards = vector3_2;
      }
      else if ((double) magnitude > 2.0)
      {
        this.SetMovementSpeed((Sosig.SosigMoveSpeed) Mathf.Min(assaultSpeed, 3));
        this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        this.m_faceTowards = vector3_2;
      }
      else
      {
        this.SetMovementSpeed((Sosig.SosigMoveSpeed) Mathf.Min(assaultSpeed, 1));
        this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
        this.RandomLookCycle(2f, 0.2f);
      }
      this.SetMovementState(Sosig.SosigMovementState.MoveToPoint);
    }

    private void BrainUpdate_DefendPoint()
    {
    }

    public void InitHands()
    {
      if (this.m_hasHandsInit)
        return;
      this.m_hasHandsInit = true;
      if ((double) UnityEngine.Random.Range(0.0f, 1f) > 0.100000001490116)
      {
        this.Hand_Primary = this.Hands[1];
        this.Hand_Secondary = this.Hands[0];
      }
      else
      {
        this.Hand_Primary = this.Hands[0];
        this.Hand_Secondary = this.Hands[1];
      }
      if ((UnityEngine.Object) this.Hands[0].Point_ShieldHold == (UnityEngine.Object) null)
        this.Hands[0].Point_ShieldHold = this.Hands[0].Point_Aimed;
      if ((UnityEngine.Object) this.Hands[1].Point_ShieldHold == (UnityEngine.Object) null)
        this.Hands[1].Point_ShieldHold = this.Hands[1].Point_Aimed;
      this.m_searchForWeaponsTick = UnityEngine.Random.Range(this.m_searchForWeaponsRefire.x, this.m_searchForWeaponsRefire.y);
    }

    private void SetHandObjectUsage(Sosig.SosigObjectUsageFocus o)
    {
      if (o == this.ObjectUsageFocus)
        return;
      this.ObjectUsageFocus = o;
    }

    public bool ForceEquip(SosigWeapon w)
    {
      SosigHand h;
      if (this.GetEmptyHand(out h))
      {
        w.transform.position = h.Point_HipFire.position + h.Point_HipFire.forward * 0.3f;
        w.transform.rotation = h.Point_HipFire.rotation;
        h.PickUp(w);
        return true;
      }
      if (!this.Inventory.IsThereAFreeSlot())
        return false;
      SosigInventory.Slot freeSlot = this.Inventory.GetFreeSlot();
      freeSlot.PlaceObjectIn(w);
      w.transform.position = freeSlot.Target.transform.position;
      w.transform.rotation = freeSlot.Target.transform.rotation;
      return true;
    }

    private void HandUpdate()
    {
      if ((double) this.m_timeSinceGrenadeThrow < 15.0)
        this.m_timeSinceGrenadeThrow += Time.deltaTime;
      switch (this.ObjectUsageFocus)
      {
        case Sosig.SosigObjectUsageFocus.EmptyHands:
          this.HandUpdate_EmptyHands();
          break;
        case Sosig.SosigObjectUsageFocus.MaintainHeldAtRest:
          this.HandUpdate_MaintainHeldAtRest();
          break;
        case Sosig.SosigObjectUsageFocus.AttackTarget:
          this.HandUpdate_AttackTarget();
          break;
        case Sosig.SosigObjectUsageFocus.AimAtReady:
          this.HandUpdate_AimAtReady();
          break;
      }
      this.EquipmentPickupCycle();
    }

    private void HandUpdate_EmptyHands()
    {
      if (this.Hand_Primary.IsHoldingObject)
        this.Hand_Primary.PutAwayHeldObject();
      if (!this.Hand_Secondary.IsHoldingObject)
        return;
      this.Hand_Secondary.PutAwayHeldObject();
    }

    private void HandUpdate_MaintainHeldAtRest()
    {
      for (int index = 0; index < this.Hands.Count; ++index)
      {
        if (this.Hands[index].IsHoldingObject && (this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee || this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Shield))
        {
          this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Melee);
          this.Hands[index].HeldObject.UseMelee(Sosig.SosigObjectUsageFocus.MaintainHeldAtRest, this.CurrentOrder != Sosig.SosigOrder.Disabled, this.Hands[index].Target.position);
        }
        else
          this.Hands[index].SetHandPose(SosigHand.SosigHandPose.AtRest);
      }
    }

    private void HandUpdate_AimAtReady()
    {
      for (int index = 0; index < this.Hands.Count; ++index)
      {
        if (this.Hands[index].IsHoldingObject && (this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee || this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Shield))
        {
          this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Melee);
          this.Hands[index].HeldObject.UseMelee(Sosig.SosigObjectUsageFocus.AimAtReady, this.CurrentOrder != Sosig.SosigOrder.Disabled, this.Hands[index].Target.position);
        }
        else
          this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Aimed);
      }
    }

    private void HandUpdate_AttackTarget()
    {
      Vector3 targetPoint = this.Priority.GetTargetPoint();
      if (this.m_isBlinded)
        targetPoint += UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 5f) * Time.deltaTime;
      if (this.Priority.HasFreshTarget())
      {
        for (int index = 0; index < this.Hands.Count; ++index)
          this.Hands[index].SetAimTowardPoint(targetPoint);
      }
      else
      {
        for (int index = 0; index < this.Hands.Count; ++index)
          this.Hands[index].ClearAimPoint();
      }
      bool flag1 = false;
      if (this.DoIHaveAShieldInMyHand())
        flag1 = true;
      if ((double) this.Agent.velocity.magnitude > (double) this.HipFiringVelocityThreshold || flag1)
      {
        for (int index = 0; index < this.Hands.Count; ++index)
          this.Hands[index].SetHandPose(SosigHand.SosigHandPose.HipFire);
      }
      else
      {
        for (int index = 0; index < this.Hands.Count; ++index)
          this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Aimed);
      }
      for (int index = 0; index < this.Hands.Count; ++index)
      {
        if (this.Hands[index].IsHoldingObject)
        {
          if (this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Gun)
          {
            if (this.Hands[index].HeldObject.UsageState == SosigWeapon.SosigWeaponUsageState.Reloading)
              this.Hands[index].SetHandPose(SosigHand.SosigHandPose.AtRest);
            float timeTargetSeen = this.Priority.GetTimeTargetSeen();
            float num1 = Vector3.Distance(targetPoint, this.transform.position);
            float num2 = Mathf.Lerp(0.0f, 3f, num1 / 500f);
            float num3 = Mathf.Lerp(1f, 2f, this.m_suppressionLevel);
            bool targetPointIdentified = false;
            if ((double) timeTargetSeen >= (double) num2 * (double) num3)
              targetPointIdentified = true;
            float num4 = 1f;
            bool isHipfiring = false;
            if (this.Hands[index].Pose == SosigHand.SosigHandPose.HipFire)
            {
              num4 += 0.8f;
              isHipfiring = true;
            }
            float recoilMult = num4 + num3;
            bool isClutching = false;
            if ((double) this.m_suppressionLevel > 0.699999988079071)
              isClutching = true;
            this.m_aimWanderTick -= Time.deltaTime;
            if ((double) this.m_aimWanderTick <= 0.0)
            {
              this.m_aimWanderTick = 0.15f;
              this.m_aimWanderRandom = UnityEngine.Random.onUnitSphere;
            }
            this.m_aimWander = Vector3.SmoothDamp(this.m_aimWander, this.m_aimWanderRandom, ref this.m_aimWanderVel, 0.15f);
            Mathf.Lerp(1f, 10f, num1 / 300f);
            Mathf.Lerp(0.2f, 0.5f, num1 / 300f);
            float num5 = Mathf.Lerp(1f, 0.0f, timeTargetSeen * 0.2f);
            float num6 = 0.0f;
            if (isHipfiring)
              num6 = 0.6f;
            float num7 = Mathf.Lerp(0.0f, 0.8f, this.m_suppressionLevel);
            float num8 = Mathf.Max(num5, num6, num7);
            targetPoint += this.m_aimWander * num8;
            if ((UnityEngine.Object) this.AimTester != (UnityEngine.Object) null)
            {
              if ((UnityEngine.Object) this.aimTester != (UnityEngine.Object) null)
                this.aimTester.transform.position = targetPoint;
              else
                this.aimTester = UnityEngine.Object.Instantiate<GameObject>(this.AimTester, targetPoint, Quaternion.identity).transform;
            }
            this.Hands[index].HeldObject.TryToFireGun(targetPoint, this.IsPanicFiring(), targetPointIdentified, isClutching, recoilMult, isHipfiring);
          }
          else if (this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee)
          {
            this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Melee);
            this.Hands[index].HeldObject.UseMelee(Sosig.SosigObjectUsageFocus.AttackTarget, this.CurrentOrder != Sosig.SosigOrder.Disabled, targetPoint);
          }
          else if (this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Shield)
          {
            this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Melee);
            this.Hands[index].HeldObject.UseMelee(Sosig.SosigObjectUsageFocus.AttackTarget, this.CurrentOrder != Sosig.SosigOrder.Disabled, targetPoint);
          }
          else if (this.Hands[index].HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade)
          {
            float timeTargetSeen = this.Priority.GetTimeTargetSeen();
            float num1 = Mathf.Lerp(0.0f, 2f, Vector3.Distance(targetPoint, this.transform.position) / 500f);
            float num2 = Mathf.Lerp(1f, 2f, this.m_suppressionLevel);
            bool flag2 = false;
            if ((double) timeTargetSeen >= (double) num1 * (double) num2)
              flag2 = true;
            bool flag3 = false;
            if ((double) this.Priority.GetTimeSinceTopTargetSeen() < 0.100000001490116)
              flag3 = true;
            bool isReadyToThrow = false;
            if (flag3 && flag2)
              isReadyToThrow = true;
            if ((double) this.m_timeSinceGrenadeThrow < (double) this.GrenadeThrowLag)
              isReadyToThrow = false;
            if (flag2)
            {
              this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Aimed);
              this.Hands[index].HeldObject.TryToThrowAt(targetPoint, isReadyToThrow);
            }
            else
              this.Hands[index].SetHandPose(SosigHand.SosigHandPose.Aimed);
          }
        }
      }
    }

    public bool IsPanicFiring() => !this.HasABrain || this.m_isConfused || this.m_isBlinded && (double) this.m_timeSinceLastDamage < 2.0;

    private void EquipmentPickupCycle()
    {
      if (this.m_hasTargetEquipment && (UnityEngine.Object) this.m_targetEquipmentToPickup != (UnityEngine.Object) null && this.m_targetEquipmentToPickup.IFFCode != -2)
      {
        this.m_hasTargetEquipment = false;
        this.m_targetEquipmentToPickup = (AIEntity) null;
      }
      if (this.m_isStunned || this.m_isBlinded)
        return;
      if ((UnityEngine.Object) this.m_targetEquipmentToPickup == (UnityEngine.Object) null)
        this.m_hasTargetEquipment = false;
      if (!this.m_hasTargetEquipment || (double) Vector3.Distance(this.m_targetEquipmentToPickup.transform.position, this.Agent.transform.position) >= (double) this.EquipmentPickupDistance || !this.PickUpEquipment(this.m_targetEquipmentToPickup))
        return;
      this.m_targetEquipmentToPickup = (AIEntity) null;
      this.m_hasTargetEquipment = false;
    }

    private void EquipmentScanCycle(Vector3 extents, float tickDownSpeed)
    {
      if (this.AreMyHandsFull() && this.IsMyInventoryFull())
      {
        this.m_hasTargetEquipment = false;
        this.m_targetEquipmentToPickup = (AIEntity) null;
      }
      else if ((double) this.m_searchForWeaponsTick > 0.0)
      {
        this.m_searchForWeaponsTick -= Time.deltaTime * tickDownSpeed;
      }
      else
      {
        this.m_searchForWeaponsTick = UnityEngine.Random.Range(this.m_searchForWeaponsRefire.x, this.m_searchForWeaponsRefire.y);
        if (this.m_isBlinded)
          return;
        Collider[] colliderArray = Physics.OverlapBox(this.Agent.transform.position + Vector3.up * 1f, extents * this.SearchExtentsModifier, Quaternion.identity, (int) this.LM_SearchForWeapons, QueryTriggerInteraction.Collide);
        float num = 20f;
        AIEntity aiEntity = (AIEntity) null;
        bool flag = false;
        SosigWeapon sosigWeapon = (SosigWeapon) null;
        if (colliderArray.Length > 0)
        {
          for (int index = 0; index < colliderArray.Length; ++index)
          {
            AIEntity component1 = colliderArray[index].GetComponent<AIEntity>();
            if (component1.IFFCode == -2)
            {
              float magnitude = (component1.GetPos() - this.Agent.transform.position).magnitude;
              if ((double) magnitude < (double) num)
              {
                SosigWeapon component2 = component1.FacingTransform.GetComponent<SosigWeapon>();
                if (this.Inventory.DoINeed(component2) && (component2.IsUsable() || this.Inventory.HasAmmoFor(component2)) && ((component2.Type != SosigWeapon.SosigWeaponType.Gun || this.CanPickup_Ranged) && (component2.Type != SosigWeapon.SosigWeaponType.Melee || this.CanPickup_Melee)) && ((component2.Type == SosigWeapon.SosigWeaponType.Gun || component2.Type == SosigWeapon.SosigWeaponType.Melee || this.CanPickup_Other) && (!flag || sosigWeapon.Type != SosigWeapon.SosigWeaponType.Gun || component2.Type != SosigWeapon.SosigWeaponType.Melee && component2.Type != SosigWeapon.SosigWeaponType.Grenade)))
                {
                  num = magnitude;
                  aiEntity = component1;
                  flag = true;
                  sosigWeapon = component2;
                }
              }
            }
          }
        }
        if (!flag)
          return;
        this.m_hasTargetEquipment = true;
        this.m_targetEquipmentToPickup = aiEntity;
      }
    }

    private bool PickUpEquipment(AIEntity e)
    {
      SosigHand h;
      if (this.GetEmptyHand(out h))
      {
        h.PickUp(e.FacingTransform.GetComponent<SosigWeapon>());
        return true;
      }
      return this.Inventory.IsThereAFreeSlot() && this.Inventory.PutObjectInMe(e.FacingTransform.GetComponent<SosigWeapon>());
    }

    private bool EquipBestPrimary()
    {
      int bestItemQuality = this.Inventory.GetBestItemQuality(SosigWeapon.SosigWeaponType.Gun);
      if (this.Hand_Primary.IsHoldingObject)
      {
        if (this.Hand_Primary.HeldObject.Type == SosigWeapon.SosigWeaponType.Gun)
        {
          int quality = this.Hand_Primary.HeldObject.Quality;
          if (bestItemQuality > 0 && bestItemQuality > quality)
          {
            this.Inventory.SwapObjectFromHandToObjectInInventory(this.Hand_Primary.HeldObject, this.Inventory.GetBestGunOut());
            return false;
          }
        }
        else if (this.Inventory.IsThereAFreeSlot() && this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Gun))
        {
          SosigWeapon heldObject = this.Hand_Primary.HeldObject;
          this.Hand_Primary.DropHeldObject();
          this.Inventory.PutObjectInMe(heldObject);
          return false;
        }
      }
      else
      {
        if (this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Gun))
        {
          SosigWeapon bestGunOut = this.Inventory.GetBestGunOut();
          bestGunOut.InventorySlotWithThis.I.DropObjectInSlot(bestGunOut.InventorySlotWithThis);
          this.Hand_Primary.PickUp(bestGunOut);
          return false;
        }
        if (this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Melee))
        {
          SosigWeapon bestMeleeWeaponOut = this.Inventory.GetBestMeleeWeaponOut();
          bestMeleeWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestMeleeWeaponOut.InventorySlotWithThis);
          this.Hand_Primary.PickUp(bestMeleeWeaponOut);
          return false;
        }
      }
      return true;
    }

    private void EquipSecondaryCycle()
    {
      if (this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.Type == SosigWeapon.SosigWeaponType.Gun && this.Inventory.IsThereAFreeSlot())
      {
        SosigWeapon heldObject = this.Hand_Secondary.HeldObject;
        this.Hand_Secondary.DropHeldObject();
        this.Inventory.PutObjectInMe(heldObject);
      }
      else
      {
        if (!this.Hand_Secondary.IsHoldingObject && this.Priority.HasFreshTarget() && (this.Priority.IsTargetEntity() && this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Grenade)))
        {
          SosigWeapon bestWeaponOut = this.Inventory.GetBestWeaponOut(SosigWeapon.SosigWeaponType.Grenade);
          float distanceToTarget = this.Priority.GetDistanceToTarget(this.transform);
          if ((double) distanceToTarget > (double) bestWeaponOut.PreferredRange.x && (double) distanceToTarget < (double) bestWeaponOut.PreferredRange.y && ((UnityEngine.Object) bestWeaponOut != (UnityEngine.Object) null && bestWeaponOut.InventorySlotWithThis != null))
          {
            bestWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestWeaponOut.InventorySlotWithThis);
            this.Hand_Secondary.PickUp(bestWeaponOut);
            return;
          }
        }
        if (this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade)
        {
          bool flag = false;
          if (!this.Priority.HasFreshTarget() || !this.Priority.IsTargetEntity())
          {
            flag = true;
          }
          else
          {
            float distanceToTarget = this.Priority.GetDistanceToTarget(this.transform);
            if ((double) distanceToTarget <= (double) this.Hand_Secondary.HeldObject.PreferredRange.x || (double) distanceToTarget >= (double) this.Hand_Secondary.HeldObject.PreferredRange.y)
              flag = true;
          }
          if (flag)
          {
            SosigWeapon heldObject = this.Hand_Secondary.HeldObject;
            this.Hand_Secondary.DropHeldObject();
            this.Inventory.PutObjectInMe(heldObject);
          }
        }
        if (!this.Hand_Primary.IsHoldingObject && this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Melee) && !this.Hand_Secondary.IsHoldingObject)
        {
          SosigWeapon bestMeleeWeaponOut = this.Inventory.GetBestMeleeWeaponOut();
          if ((UnityEngine.Object) bestMeleeWeaponOut != (UnityEngine.Object) null && bestMeleeWeaponOut.InventorySlotWithThis != null)
          {
            bestMeleeWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestMeleeWeaponOut.InventorySlotWithThis);
            this.Hand_Secondary.PickUp(bestMeleeWeaponOut);
            return;
          }
        }
        if (this.Hand_Primary.IsHoldingObject && this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Shield) && !this.Hand_Secondary.IsHoldingObject)
        {
          SosigWeapon bestShieldWeaponOut = this.Inventory.GetBestShieldWeaponOut();
          if ((UnityEngine.Object) bestShieldWeaponOut != (UnityEngine.Object) null && bestShieldWeaponOut.InventorySlotWithThis != null)
          {
            bestShieldWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestShieldWeaponOut.InventorySlotWithThis);
            this.Hand_Secondary.PickUp(bestShieldWeaponOut);
            return;
          }
        }
        if (!this.Hand_Primary.IsHoldingObject)
          return;
        float num = 100f;
        if (this.Priority.HasFreshTarget() && this.Priority.IsTargetEntity())
          num = Vector3.Distance(this.transform.position, this.Priority.GetTargetPoint());
        if ((double) num < 5.0 || this.Hand_Primary.HeldObject.Type == SosigWeapon.SosigWeaponType.Melee)
        {
          if (this.Hand_Secondary.IsHoldingObject || !this.Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Melee))
            return;
          SosigWeapon bestMeleeWeaponOut = this.Inventory.GetBestMeleeWeaponOut();
          if (!((UnityEngine.Object) bestMeleeWeaponOut != (UnityEngine.Object) null) || bestMeleeWeaponOut.InventorySlotWithThis == null)
            return;
          bestMeleeWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestMeleeWeaponOut.InventorySlotWithThis);
          this.Hand_Secondary.PickUp(bestMeleeWeaponOut);
        }
        else
        {
          if (!this.Hand_Secondary.IsHoldingObject || this.Hand_Secondary.HeldObject.Type != SosigWeapon.SosigWeaponType.Melee || !this.Inventory.IsThereAFreeSlot())
            return;
          SosigWeapon heldObject = this.Hand_Secondary.HeldObject;
          this.Hand_Secondary.DropHeldObject();
          this.Inventory.PutObjectInMe(heldObject);
        }
      }
    }

    private bool GetEmptyHand(out SosigHand h)
    {
      if (!this.Hand_Primary.IsHoldingObject)
      {
        h = this.Hand_Primary;
        return true;
      }
      if (!this.Hand_Secondary.IsHoldingObject)
      {
        h = this.Hand_Secondary;
        return true;
      }
      h = (SosigHand) null;
      return false;
    }

    private bool DoIHaveAWeaponAtAll() => this.DoIHaveAWeaponInMyHand() || this.DoIHaveAWeaponInMyInventory();

    private bool DoIHaveAWeaponInMyHand() => this.Hand_Primary.IsHoldingObject && (this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun || this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee) || this.Hand_Secondary.IsHoldingObject && (this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun || this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee);

    private bool DoIHaveAShieldInMyHand() => this.Hand_Primary.IsHoldingObject && this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Shield || this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Shield;

    private bool DoIHaveAWeaponInMyInventory() => this.Inventory.DoIHaveAnyWeaponry();

    private SosigWeapon GetHeldMeleeWeapon()
    {
      if (this.Hand_Primary.IsHoldingObject && this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee)
        return this.Hand_Primary.HeldObject;
      return this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee ? this.Hand_Secondary.HeldObject : (SosigWeapon) null;
    }

    public bool DoIHaveAGun() => this.Hand_Primary.IsHoldingObject && this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun || this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun;

    private Vector2 GetPreferedEngagementDistance()
    {
      Vector2 vector2 = new Vector2(0.1f, 10f);
      if (this.DoIHaveAGun())
      {
        if (this.Hand_Primary.IsHoldingObject && this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun)
          return this.Hand_Primary.HeldObject.PreferredRange;
        if (this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun)
          return this.Hand_Secondary.HeldObject.PreferredRange;
      }
      return vector2;
    }

    private bool AmIReloading() => this.DoIHaveAGun() && (this.Hand_Primary.IsHoldingObject && this.Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun && this.Hand_Primary.HeldObject.UsageState == SosigWeapon.SosigWeaponUsageState.Reloading || this.Hand_Secondary.IsHoldingObject && this.Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun && this.Hand_Secondary.HeldObject.UsageState == SosigWeapon.SosigWeaponUsageState.Reloading);

    private bool AreMyHandsFull() => this.Hand_Primary.IsHoldingObject && this.Hand_Secondary.IsHoldingObject;

    private bool IsMyInventoryFull() => this.Inventory.Slots.Count == 0 || !this.Inventory.IsThereAFreeSlot();

    private int GetBestItemQualityBeingHeld()
    {
      int a = -1;
      if (this.Hand_Primary.IsHoldingObject)
        a = Mathf.Max(a, this.Hand_Primary.HeldObject.Quality);
      if (this.Hand_Secondary.IsHoldingObject)
        a = Mathf.Max(a, this.Hand_Secondary.HeldObject.Quality);
      return a;
    }

    private void HandPhysUpdate()
    {
      for (int index = 0; index < this.Hands.Count; ++index)
        this.Hands[index].Hold();
    }

    private void SetBodyPose(Sosig.SosigBodyPose p)
    {
      if (p == this.BodyPose)
        return;
      if (this.DoIHaveAShieldInMyHand() && p == Sosig.SosigBodyPose.Prone)
        p = Sosig.SosigBodyPose.Crouching;
      if (this.m_isHobbled && p == Sosig.SosigBodyPose.Standing)
        p = Sosig.SosigBodyPose.Crouching;
      if (this.m_isBackBroken && (p == Sosig.SosigBodyPose.Standing || p == Sosig.SosigBodyPose.Crouching))
        p = Sosig.SosigBodyPose.Prone;
      this.BodyPose = p;
      switch (this.BodyPose)
      {
        case Sosig.SosigBodyPose.Standing:
          this.m_targetPose = this.Pose_Standing;
          break;
        case Sosig.SosigBodyPose.Crouching:
          this.m_targetPose = this.Pose_Crouching;
          break;
        case Sosig.SosigBodyPose.Prone:
          this.m_targetPose = this.Pose_Prone;
          break;
      }
    }

    private void SetMovementState(Sosig.SosigMovementState s)
    {
      if (s == this.MovementState)
        return;
      this.MovementState = s;
    }

    private void SetMovementSpeed(Sosig.SosigMoveSpeed m) => this.MoveSpeed = m;

    private void SetMovementSpeedBasedOnDistance()
    {
      if ((double) Vector3.Distance(this.m_navToPoint, this.Agent.transform.position) > 20.0)
        this.MoveSpeed = Sosig.SosigMoveSpeed.Running;
      else
        this.MoveSpeed = Sosig.SosigMoveSpeed.Walking;
    }

    private void LegsUpdate()
    {
      switch (this.MovementState)
      {
        case Sosig.SosigMovementState.Idle:
          this.LegsUpdate_Idle();
          break;
        case Sosig.SosigMovementState.HoldFast:
          this.LegsUpdate_HoldFast();
          break;
        case Sosig.SosigMovementState.MoveToPoint:
          this.LegsUpdate_MoveToPoint();
          break;
        case Sosig.SosigMovementState.DiveToPoint:
          this.LegsUpdate_DiveToPoint();
          break;
      }
    }

    private void LegsUpdate_Idle()
    {
    }

    private void LegsUpdate_HoldFast()
    {
      if (!this.Agent.enabled)
        return;
      this.TurnTowardsFacingDir();
    }

    private void InitiateLink(NavMeshLinkExtension ex)
    {
      this.linkExtensions = ex;
      this.extensionSpeed = this.Agent.speed;
      this.targetSpeed = ex.GetXYSpeed();
    }

    private void EndLink()
    {
      this.extensionSpeed = 0.0f;
      this.targetSpeed = 0.0f;
      this.m_isOnOffMeshLink = false;
      this.linkExtensions = (NavMeshLinkExtension) null;
    }

    private void LegsUpdate_MoveToPoint()
    {
      if (!this.Agent.enabled)
        return;
      this.Agent.speed = this.GetLinearSpeed(this.Agent.velocity);
      this.Agent.autoTraverseOffMeshLink = true;
      if (this.Agent.isOnOffMeshLink)
      {
        if (!this.m_isOnOffMeshLink)
        {
          NavMeshLinkExtension component = this.Agent.currentOffMeshLinkData.offMeshLink.gameObject.GetComponent<NavMeshLinkExtension>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            this.m_isOnOffMeshLink = true;
            this.InitiateLink(component);
          }
        }
      }
      else if (!this.Agent.isOnOffMeshLink && this.m_isOnOffMeshLink)
        this.EndLink();
      if ((UnityEngine.Object) this.linkExtensions != (UnityEngine.Object) null)
      {
        if ((double) this.Agent.currentOffMeshLinkData.endPos.y < (double) this.Agent.transform.position.y)
        {
          this.targetSpeed += Time.deltaTime * 5f;
          this.targetSpeed = Mathf.Clamp(this.targetSpeed, 0.0f, 10f);
        }
        this.extensionSpeed = Mathf.MoveTowards(this.extensionSpeed, this.targetSpeed, Time.deltaTime * 5f);
        this.Agent.speed = this.extensionSpeed;
      }
      this.TurnTowardsFacingDir();
    }

    private void LegsUpdate_DiveToPoint()
    {
    }

    private void TurnTowardsFacingDir()
    {
      Vector3 forward = Vector3.RotateTowards(this.Agent.transform.forward, this.m_faceTowards.normalized, this.GetAngularSpeed() * Time.deltaTime, 1f);
      forward.y = 0.0f;
      if ((double) forward.x == 0.0 && (double) forward.z == 0.0 || (double) forward.y == 1.0)
        forward.z = 1E-06f;
      forward.z += 1f / 1000f;
      this.Agent.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    private void TryToGetTo(Vector3 v)
    {
      if (!this.Agent.enabled)
        return;
      float num = Vector3.Distance(v, this.lastDest);
      this.m_navToPoint = v;
      this.debug_haspath = this.Agent.hasPath;
      if (this.debug_haspath && (double) Vector3.Distance(this.m_navToPoint, this.Agent.transform.position) < 0.00999999977648258)
        this.Agent.ResetPath();
      this.debug_pathpending = this.Agent.pathPending;
      if (!this.debug_pathpending && (double) num > 0.0500000007450581)
      {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(v, out hit, 1f, -1))
          v = hit.position;
        this.Agent.SetDestination(v);
        this.lastDest = v;
      }
      Vector3 vector3 = this.transform.InverseTransformDirection(this.Agent.velocity);
      this.curEuluer = Vector3.Lerp(this.curEuluer, new Vector3(Mathf.Clamp(-vector3.z, -3f, 3f), 0.0f, Mathf.Clamp(vector3.x, -3f, 3f)) * this.MovementRotMagnitude, Time.deltaTime * 1f);
      this.Pose_Standing.localEulerAngles = this.m_poseLocalEulers_Standing + this.curEuluer;
      this.Pose_Crouching.localEulerAngles = this.m_poseLocalEulers_Crouching + this.curEuluer;
      this.Pose_Prone.localEulerAngles = this.m_poseLocalEulers_Prone + this.curEuluer;
    }

    private float GetLinearSpeed(Vector3 moveDir)
    {
      if (this.m_isChillOut)
        return 0.0f;
      if (moveDir == Vector3.zero)
        moveDir = Vector3.up;
      float a = this.Speed_Walk;
      Sosig.SosigMoveSpeed sosigMoveSpeed = this.MoveSpeed;
      if (this.m_isBackBroken)
        sosigMoveSpeed = Sosig.SosigMoveSpeed.Crawling;
      else if (this.m_isHobbled)
        sosigMoveSpeed = Sosig.SosigMoveSpeed.Crawling;
      else if (this.m_isStunned)
        sosigMoveSpeed = Sosig.SosigMoveSpeed.Sneaking;
      if (this.m_isFrozen)
        sosigMoveSpeed = Sosig.SosigMoveSpeed.Crawling;
      switch (sosigMoveSpeed)
      {
        case Sosig.SosigMoveSpeed.Crawling:
          a = this.Speed_Crawl;
          break;
        case Sosig.SosigMoveSpeed.Sneaking:
          a = this.Speed_Sneak;
          break;
        case Sosig.SosigMoveSpeed.Walking:
          a = this.Speed_Walk;
          break;
        case Sosig.SosigMoveSpeed.Running:
          a = this.Speed_Run;
          break;
      }
      if (this.m_isFrozen)
        a = Mathf.Min(this.Speed_Crawl, 0.5f);
      if (this.m_isSpeedup)
        a = this.Speed_Run * 2f;
      float speedSneak = this.Speed_Sneak;
      float num = Vector3.Angle(this.Agent.transform.forward, moveDir);
      return Mathf.Lerp(a, speedSneak, num / 180f);
    }

    private float GetAngularSpeed()
    {
      if (this.m_isFrozen)
        return this.Speed_Turning * 0.15f;
      if (this.m_isSpeedup)
        return this.Speed_Turning * 2f;
      return this.m_isStunned ? this.Speed_Turning * 0.25f : this.Speed_Turning;
    }

    private Vector3 GetRandomLookDir()
    {
      Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      return onUnitSphere.normalized;
    }

    private Vector3 GetNewRandomWanderPoint(Vector3 startPos)
    {
      Vector3 position = this.Agent.transform.position;
      Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      float num = UnityEngine.Random.Range(6f, 10f);
      return NavMesh.Raycast(position, position + onUnitSphere.normalized * num, out this.m_navMeshHit, -1) ? position + onUnitSphere.normalized * this.m_navMeshHit.distance * 0.5f : position + onUnitSphere.normalized * num * UnityEngine.Random.Range(0.6f, 1f);
    }

    private void BodyUpdate()
    {
      if (this.BodyState != Sosig.SosigBodyState.InControl)
        this.ClearCoverPoint();
      if (this.BodyState != Sosig.SosigBodyState.Dead)
      {
        if (this.m_isStunned)
        {
          this.m_stunTimeLeft = Mathf.Clamp(this.m_stunTimeLeft, -1f, this.m_maxStunTime);
          this.m_stunTimeLeft -= Time.deltaTime;
          if ((double) this.m_stunTimeLeft <= 0.0)
            this.m_isStunned = false;
        }
        if (this.m_isConfused)
        {
          this.m_confusedTime = Mathf.Clamp(this.m_confusedTime, -1f, this.m_maxConfusedTime);
          this.m_confusedTime -= Time.deltaTime;
          if ((double) this.m_confusedTime <= 0.0)
            this.m_isConfused = false;
        }
        if (this.m_isBlinded)
        {
          this.m_blindTime = Mathf.Clamp(this.m_blindTime, -1f, this.m_maxBlindTime);
          this.m_blindTime -= Time.deltaTime;
          if ((double) this.m_blindTime <= 0.0)
            this.m_isBlinded = false;
        }
        ++this.m_linkIndex;
        if (this.m_linkIndex >= this.Links.Count)
          this.m_linkIndex = 0;
        if ((UnityEngine.Object) this.Links[this.m_linkIndex] != (UnityEngine.Object) null)
          this.fakeEntityPos = this.Links[this.m_linkIndex].transform.position + UnityEngine.Random.onUnitSphere * 0.2f + this.Links[this.m_linkIndex].transform.up * 0.25f;
        this.E.FakePos = this.fakeEntityPos;
      }
      if (!this.m_isHobbled)
      {
        for (int index = 2; index < this.Links.Count; ++index)
        {
          if (!this.m_linksDestroyed[index] && (double) this.Links[index].GetIntegrityRatio() < 0.25)
          {
            this.m_isHobbled = true;
            break;
          }
        }
      }
      if (this.BodyState == Sosig.SosigBodyState.Ballistic)
      {
        this.m_recoveryFromBallisticTick = Mathf.Clamp(this.m_recoveryFromBallisticTick, -1f, 4f);
        this.m_recoveryFromBallisticTick -= Time.deltaTime;
        if ((double) this.m_recoveryFromBallisticTick <= 0.0)
        {
          this.AttemptToRecoverFromBallistic();
        }
        else
        {
          this.m_tickDownToWrithe -= Time.deltaTime;
          if ((double) this.m_tickDownToWrithe > 0.0)
            return;
          this.m_tickDownToWrithe = UnityEngine.Random.Range(this.m_writheTickRange.x, this.m_writheTickRange.y);
          this.Writhe();
        }
      }
      else
      {
        if (this.BodyState != Sosig.SosigBodyState.InControl)
          return;
        if (this.m_isCountingDownToStagger)
        {
          if ((double) this.m_staggerAmountToApply > 1.0)
          {
            this.Stagger(this.m_staggerAmountToApply);
            this.m_isCountingDownToStagger = false;
          }
          else
          {
            this.m_tickDownToStagger -= Time.deltaTime * 2f;
            if ((double) this.m_tickDownToStagger <= 0.0)
            {
              this.Stagger(this.m_staggerAmountToApply);
              this.m_isCountingDownToStagger = false;
            }
          }
        }
        float num1 = 4f;
        if (this.IsFrozen)
          num1 = 0.25f;
        if (this.IsSpeedUp)
          num1 = 8f;
        this.m_targetLocalPos = Vector3.Lerp(this.m_targetLocalPos, this.m_targetPose.localPosition, Time.deltaTime * num1);
        this.m_targetLocalRot = Quaternion.Slerp(this.m_targetLocalRot, this.m_targetPose.localRotation, Time.deltaTime * num1);
        if (this.m_recoveringFromBallisticState)
        {
          this.CoreTarget.position = Vector3.Lerp(this.m_recoveryFromBallisticStartPos, this.m_targetPose.position, (float) ((double) this.m_recoveryFromBallisticLerp * (double) num1 * 0.5));
          this.CoreTarget.rotation = Quaternion.Slerp(this.m_recoveryFromBallisticStartRot, this.m_targetPose.rotation, (float) ((double) this.m_recoveryFromBallisticLerp * (double) num1 * 0.5));
          this.UpdateJoints(this.m_recoveryFromBallisticLerp);
          if ((double) this.m_recoveryFromBallisticLerp >= 1.0)
            this.m_recoveringFromBallisticState = false;
          this.m_recoveryFromBallisticLerp += Time.deltaTime * 0.5f;
        }
        else
        {
          float num2 = this.Agent.velocity.magnitude - 0.05f;
          this.m_bobTick = Mathf.Repeat(this.m_bobTick += Time.deltaTime * this.BobSpeedMult, 1f);
          float num3 = Mathf.Clamp(num2 * 0.5f, 0.0f, 1f);
          float num4 = Mathf.Clamp(num2 * 0.5f, 0.0f, 2f);
          float num5 = this.BodyBobCurve_Vertical.Evaluate((float) ((double) this.m_bobTick * (double) num4 * 2.0));
          if ((double) num5 > 0.0)
            this.m_hasFootStepDown = false;
          if ((double) num5 <= -0.800000011920929 && !this.m_hasFootStepDown)
          {
            float num6 = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position);
            float delay = num6 / 343f;
            this.m_hasFootStepDown = true;
            if ((double) num6 < 10.0)
              SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.GenericClose, this.AudEvent_FootSteps, this.transform.position, new Vector2(num3 * 0.35f, num3 * 0.4f), new Vector2(0.95f, 1.05f), delay);
          }
          this.CoreTarget.position = this.transform.TransformPoint(this.m_targetLocalPos) + this.m_targetPose.up * num5 * this.MaxBobIntensityVertical * num3 + this.m_targetPose.right * this.BodyBobCurve_Horizontal.Evaluate(this.m_bobTick * num4) * this.MaxBobIntensityHorizontal * num3;
          this.CoreTarget.localRotation = this.m_targetLocalRot;
        }
      }
    }

    private void AttemptToRecoverFromBallistic()
    {
      this.m_ballisticRecoveryAttemptTick -= Time.deltaTime;
      if ((double) this.m_ballisticRecoveryAttemptTick >= 0.0)
        return;
      this.m_recoveryFromBallisticLerp = 0.0f;
      if ((UnityEngine.Object) this.CoreRB == (UnityEngine.Object) null)
      {
        this.SetBodyState(Sosig.SosigBodyState.Dead);
      }
      else
      {
        this.m_recoveryFromBallisticStartPos = this.CoreRB.transform.position;
        this.m_recoveryFromBallisticStartRot = this.CoreRB.transform.rotation;
        this.m_recoveringFromBallisticState = true;
        this.Agent.enabled = true;
        if (this.Agent.Warp(this.CoreRB.transform.position))
        {
          this.m_ballisticRecoveryAttemptTick = 0.0f;
          this.SetBodyState(Sosig.SosigBodyState.InControl);
          this.m_wanderPoint = this.CoreRB.transform.position;
          this.m_skirmishPoint = this.CoreRB.transform.position;
        }
        else
        {
          this.Agent.enabled = false;
          this.m_ballisticRecoveryAttemptTick = UnityEngine.Random.Range(this.m_ballsticRecoveryAttemptRange.x, this.m_ballsticRecoveryAttemptRange.y);
        }
      }
    }

    public void SpawnLargeMustardBurst(Vector3 point, Vector3 dir) => UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_LargeMustardBurst, point, Quaternion.LookRotation(dir));

    public bool IsHealing => this.m_isHealing;

    public bool IsDamResist => this.m_isDamResist;

    public bool IsInvuln => this.m_isInvuln;

    public bool isDamPowerUp => this.m_isDamPowerUp;

    public bool IsInfiniteAmmo => this.m_isInfiniteAmmo;

    public bool IsGhosted => this.m_isGhosted;

    public bool IsMuscleMeat => this.m_isMuscleMeat;

    public bool IsCyclops => this.m_isCyclops;

    public bool IsSpeedUp => this.m_isSpeedup;

    public bool IsHurting => this.m_isHurting;

    public bool IsDamMult => this.m_isDamMult;

    public bool IsFragile => this.m_isFragile;

    public bool IsDamPowerDown => this.m_isDamPowerDown;

    public bool IsAmmoDrain => this.m_isAmmoDrain;

    public bool IsSuperVisible => this.m_isSuperVisible;

    public bool IsWeakMeat => this.m_isWeakMeat;

    public bool IsBiClops => this.m_isBiClops;

    public bool IsBlort => this.m_isBlort;

    public bool IsDlort => this.m_isDlort;

    public bool IsFrozen => this.m_isFrozen;

    public bool IsDebuff => this.m_isDebuff;

    public float BuffIntensity_HealHarm => this.m_buffIntensity_HealHarm;

    public float BuffIntensity_DamResistHarm => this.m_buffIntensity_DamResistHarm;

    public float BuffIntensity_DamPowerUpDown => this.m_buffIntensity_DamPowerUpDown;

    public float BuffIntensity_MuscleMeatWeak => this.m_buffIntensity_MuscleMeatWeak;

    public float BuffIntensity_CyclopsPower => this.m_buffIntensity_CyclopsPower;

    private void ActivateBuff(int i, bool isInverted)
    {
      if (!PUM.HasEffectBot(i, isInverted))
        return;
      if (!isInverted)
      {
        if (i >= this.BuffSystems.Length || !((UnityEngine.Object) this.BuffSystems[i] == (UnityEngine.Object) null))
          return;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PUM.GetEffect(i, isInverted), this.Links[0].transform.position, this.Links[0].transform.rotation);
        gameObject.transform.SetParent(this.Links[0].transform);
        this.BuffSystems[i] = gameObject;
      }
      else
      {
        if (i >= this.DeBuffSystems.Length || !((UnityEngine.Object) this.DeBuffSystems[i] == (UnityEngine.Object) null))
          return;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PUM.GetEffect(i, isInverted), this.Links[0].transform.position, this.Links[0].transform.rotation);
        gameObject.transform.SetParent(this.Links[0].transform);
        this.DeBuffSystems[i] = gameObject;
      }
    }

    private void DeActivateBuff(int i)
    {
      if (this.BuffSystems.Length <= i || !((UnityEngine.Object) this.BuffSystems[i] != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.BuffSystems[i]);
      this.BuffSystems[i] = (GameObject) null;
    }

    private void DeActivateDeBuff(int i)
    {
      if (this.DeBuffSystems.Length <= i || !((UnityEngine.Object) this.DeBuffSystems[i] != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.DeBuffSystems[i]);
      this.DeBuffSystems[i] = (GameObject) null;
    }

    private void DeActivateAllBuffSystems()
    {
      for (int i = 0; i < 13; ++i)
      {
        this.DeActivateBuff(i);
        this.DeActivateDeBuff(i);
      }
    }

    public void ActivatePower(
      PowerupType type,
      PowerUpIntensity intensity,
      PowerUpDuration d,
      bool isPuke,
      bool isInverted)
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead || this.IsDebuff && type != PowerupType.Debuff && !isInverted)
        return;
      float b = 1f;
      switch (d)
      {
        case PowerUpDuration.Full:
          b = 30f;
          break;
        case PowerUpDuration.Short:
          b = 20f;
          break;
        case PowerUpDuration.VeryShort:
          b = 10f;
          break;
        case PowerUpDuration.Blip:
          b = 2f;
          break;
        case PowerUpDuration.SuperLong:
          b = 40f;
          break;
      }
      switch (type)
      {
        case PowerupType.Health:
          float amount = 0.0f;
          switch (intensity)
          {
            case PowerUpIntensity.High:
              amount = 1f;
              break;
            case PowerUpIntensity.Medium:
              amount = 0.5f;
              break;
            case PowerUpIntensity.Low:
              amount = 0.25f;
              break;
          }
          if (!isInverted)
          {
            this.HealSosig(amount);
            break;
          }
          this.HarmSosig(amount);
          break;
        case PowerupType.QuadDamage:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_buffIntensity_DamPowerUpDown = 4f;
                break;
              case PowerUpIntensity.Medium:
                this.m_buffIntensity_DamPowerUpDown = 3f;
                break;
              case PowerUpIntensity.Low:
                this.m_buffIntensity_DamPowerUpDown = 2f;
                break;
            }
            this.m_isDamPowerUp = true;
            this.m_isDamPowerDown = false;
            this.DeActivateDeBuff(1);
            this.m_buffTime_DamPowerUp = Mathf.Max(this.m_buffTime_DamPowerUp, b);
            break;
          }
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_buffIntensity_DamPowerUpDown = 0.125f;
              break;
            case PowerUpIntensity.Medium:
              this.m_buffIntensity_DamPowerUpDown = 0.25f;
              break;
            case PowerUpIntensity.Low:
              this.m_buffIntensity_DamPowerUpDown = 0.5f;
              break;
          }
          this.m_isDamPowerDown = true;
          this.m_isDamPowerUp = false;
          this.DeActivateBuff(1);
          this.m_debuffTime_DamPowerDown = Mathf.Max(this.m_debuffTime_DamPowerDown, b);
          break;
        case PowerupType.InfiniteAmmo:
          if (!isInverted)
          {
            this.m_isInfiniteAmmo = true;
            this.m_isAmmoDrain = false;
            this.DeActivateDeBuff(2);
            this.m_buffTime_InfiniteAmmo = Mathf.Max(this.m_buffTime_InfiniteAmmo, b);
            break;
          }
          this.m_isAmmoDrain = true;
          this.m_isInfiniteAmmo = false;
          this.DeActivateBuff(2);
          this.m_debuffTime_AmmoDrain = Mathf.Max(this.m_debuffTime_AmmoDrain, b);
          break;
        case PowerupType.Invincibility:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_isInvuln = true;
                this.m_isFragile = false;
                this.DeActivateDeBuff(3);
                this.m_isDamResist = false;
                this.m_isDamMult = false;
                this.m_buffTime_Invuln = Mathf.Max(this.m_buffTime_Invuln, b);
                break;
              case PowerUpIntensity.Medium:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateDeBuff(3);
                this.m_buffIntensity_DamResistHarm = 0.5f;
                this.m_buffTime_DamResist = Mathf.Max(this.m_buffTime_DamResist, b);
                break;
              case PowerUpIntensity.Low:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateDeBuff(3);
                this.m_buffIntensity_DamResistHarm = 0.75f;
                this.m_buffTime_DamResist = Mathf.Max(this.m_buffTime_DamResist, b);
                break;
            }
          }
          else
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_isFragile = true;
                this.m_isInvuln = false;
                this.DeActivateBuff(3);
                this.m_isDamResist = false;
                this.m_isDamMult = false;
                this.m_debuffTime_Fragile = Mathf.Max(this.m_debuffTime_Fragile, b);
                break;
              case PowerUpIntensity.Medium:
                this.m_isDamMult = true;
                this.m_isDamResist = false;
                this.DeActivateBuff(3);
                this.m_buffIntensity_DamResistHarm = 3f;
                this.m_debuffTime_DamMult = Mathf.Max(this.m_debuffTime_DamMult, b);
                break;
              case PowerUpIntensity.Low:
                this.m_isDamResist = true;
                this.m_isDamMult = false;
                this.DeActivateBuff(3);
                this.m_buffIntensity_DamResistHarm = 2f;
                this.m_debuffTime_DamMult = Mathf.Max(this.m_debuffTime_DamMult, b);
                break;
            }
          }
          break;
        case PowerupType.Ghosted:
          if (!isInverted)
          {
            this.m_isGhosted = true;
            this.m_isSuperVisible = false;
            this.DeActivateDeBuff(4);
            this.m_buffTime_Ghosted = Mathf.Max(this.m_buffTime_Ghosted, b);
            break;
          }
          this.m_isSuperVisible = true;
          this.m_isGhosted = false;
          this.DeActivateBuff(4);
          this.m_debuffTime_SuperVisible = Mathf.Max(this.m_debuffTime_SuperVisible, b);
          break;
        case PowerupType.MuscleMeat:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_buffIntensity_MuscleMeatWeak = 4f;
                break;
              case PowerUpIntensity.Medium:
                this.m_buffIntensity_MuscleMeatWeak = 3f;
                break;
              case PowerUpIntensity.Low:
                this.m_buffIntensity_MuscleMeatWeak = 2f;
                break;
            }
            this.m_isMuscleMeat = true;
            this.m_isWeakMeat = false;
            this.DeActivateDeBuff(6);
            this.m_buffTime_MuscleMeat = Mathf.Max(this.m_buffTime_MuscleMeat, b);
            break;
          }
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_buffIntensity_MuscleMeatWeak = 0.125f;
              break;
            case PowerUpIntensity.Medium:
              this.m_buffIntensity_MuscleMeatWeak = 0.25f;
              break;
            case PowerUpIntensity.Low:
              this.m_buffIntensity_MuscleMeatWeak = 0.5f;
              break;
          }
          this.m_isWeakMeat = true;
          this.m_isMuscleMeat = false;
          this.DeActivateBuff(6);
          this.m_debuffTime_WeakMeat = Mathf.Max(this.m_debuffTime_WeakMeat, b);
          break;
        case PowerupType.HomeTown:
          if (!isInverted)
          {
            this.TeleportSosig(GM.CurrentSceneSettings.PowerupPoint_HomeTown.position);
            break;
          }
          this.TeleportSosig(GM.CurrentSceneSettings.PowerupPoint_InverseHomeTown.position);
          break;
        case PowerupType.Blort:
          if (!isInverted)
          {
            this.m_isBlort = true;
            this.m_isDlort = false;
            this.m_buffTime_Blort = Mathf.Max(this.m_buffTime_Blort, b);
            break;
          }
          this.m_isDlort = true;
          this.m_isBlort = false;
          this.m_debuffTime_Dlort = Mathf.Max(this.m_debuffTime_Dlort, b);
          break;
        case PowerupType.Regen:
          if (!isInverted)
          {
            switch (intensity)
            {
              case PowerUpIntensity.High:
                this.m_buffIntensity_HealHarm = 20f;
                break;
              case PowerUpIntensity.Medium:
                this.m_buffIntensity_HealHarm = 10f;
                break;
              case PowerUpIntensity.Low:
                this.m_buffIntensity_HealHarm = 5f;
                break;
            }
            this.m_isHealing = true;
            this.m_isHurting = false;
            this.DeActivateDeBuff(10);
            this.m_buffTime_Heal = Mathf.Max(this.m_buffTime_Heal, b);
            break;
          }
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_buffIntensity_HealHarm = 20f;
              break;
            case PowerUpIntensity.Medium:
              this.m_buffIntensity_HealHarm = 10f;
              break;
            case PowerUpIntensity.Low:
              this.m_buffIntensity_HealHarm = 5f;
              break;
          }
          this.m_isHurting = true;
          this.m_isHealing = false;
          this.DeActivateBuff(10);
          this.m_debuffTime_Hurt = Mathf.Max(this.m_debuffTime_Hurt, b);
          break;
        case PowerupType.Cyclops:
          switch (intensity)
          {
            case PowerUpIntensity.High:
              this.m_buffIntensity_CyclopsPower = 4f;
              break;
            case PowerUpIntensity.Medium:
              this.m_buffIntensity_CyclopsPower = 3f;
              break;
            case PowerUpIntensity.Low:
              this.m_buffIntensity_CyclopsPower = 2f;
              break;
          }
          if (!isInverted)
          {
            this.m_isCyclops = true;
            this.m_isBiClops = false;
            this.DeActivateDeBuff(11);
            this.m_buffTime_Cyclops = Mathf.Max(this.m_buffTime_Cyclops, b);
            if ((UnityEngine.Object) this.m_vfx_cyclops == (UnityEngine.Object) null)
              this.m_vfx_cyclops = UnityEngine.Object.Instantiate<GameObject>(ManagerSingleton<PUM>.Instance.Sosig_Cyclops, this.Links[0].transform.position, this.Links[0].transform.rotation, this.Links[0].transform);
            if ((UnityEngine.Object) this.m_vfx_bicyclops != (UnityEngine.Object) null)
            {
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_vfx_bicyclops);
              break;
            }
            break;
          }
          this.m_isBiClops = true;
          this.m_isCyclops = false;
          this.DeActivateBuff(11);
          this.m_debuffTime_BiClops = Mathf.Max(this.m_debuffTime_BiClops, b);
          if ((UnityEngine.Object) this.m_vfx_bicyclops == (UnityEngine.Object) null)
            this.m_vfx_bicyclops = UnityEngine.Object.Instantiate<GameObject>(ManagerSingleton<PUM>.Instance.Sosig_Biclops, this.Links[0].transform.position, this.Links[0].transform.rotation, this.Links[0].transform);
          if ((UnityEngine.Object) this.m_vfx_cyclops != (UnityEngine.Object) null)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_vfx_cyclops);
            break;
          }
          break;
        case PowerupType.WheredIGo:
          this.TeleportSosig(GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav[UnityEngine.Random.Range(0, GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav.Count)].position);
          break;
        case PowerupType.ChillOut:
          this.m_debuffTime_ChillOut = b;
          this.m_isChillOut = true;
          break;
        case PowerupType.Debuff:
          this.m_debuffTime_Debuff = b;
          this.m_isDebuff = true;
          break;
        case PowerupType.Unfreeze:
          if (!isInverted)
          {
            this.m_isFrozen = false;
            this.m_debuffTime_Freeze = 0.0f;
            break;
          }
          this.m_isFrozen = true;
          this.m_debuffTime_Freeze = Mathf.Max(this.m_debuffTime_Freeze, b);
          break;
        case PowerupType.SpeedUp:
          if (!isInverted)
          {
            this.m_isSpeedup = true;
            this.m_buffTime_Speedup = Mathf.Max(this.m_debuffTime_Freeze, b);
            break;
          }
          break;
      }
      if (isPuke)
      {
        this.m_isVomitting = true;
        this.m_debuffTime_Vomit = b;
        if ((UnityEngine.Object) this.m_vfx_vomit == (UnityEngine.Object) null)
          this.m_vfx_vomit = UnityEngine.Object.Instantiate<GameObject>(ManagerSingleton<PUM>.Instance.Sosig_Barfer, this.Links[0].transform.position, this.Links[0].transform.rotation, this.Links[0].transform);
      }
      this.ActivateBuff((int) type, isInverted);
    }

    public void BuffHealing_Engage(float minTime, float healharm)
    {
      if (this.IsDebuff)
        return;
      this.m_buffTime_Heal = Mathf.Max(minTime, this.m_buffTime_Heal);
      this.m_buffIntensity_HealHarm = healharm;
      this.m_isHealing = true;
    }

    public void BuffHealing_Invis(float minTime)
    {
      if (this.IsDebuff)
        return;
      this.m_buffTime_Ghosted = Mathf.Max(this.m_buffTime_Ghosted, minTime);
      this.m_isGhosted = true;
    }

    public void BuffDamResist_Engage(float minTime, float resistHarm)
    {
      if (this.IsDebuff)
        return;
      this.m_buffTime_DamResist = Mathf.Max(minTime, this.m_buffTime_DamResist);
      this.m_buffIntensity_DamResistHarm = resistHarm;
      this.m_isDamResist = true;
    }

    public void BuffInvuln_Engage(float minTime)
    {
      if (this.IsDebuff)
        return;
      this.m_buffTime_Invuln = Mathf.Max(minTime, this.m_buffTime_Invuln);
      this.m_isInvuln = true;
    }

    private void HealSosig(float amount)
    {
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].HealIntegrity(amount);
        float num = this.Mustard + this.m_maxMustard * amount;
        if ((double) this.Mustard < (double) this.m_maxMustard)
          this.Mustard = Mathf.Clamp(num, this.Mustard, this.m_maxMustard);
      }
    }

    private void HarmSosig(float amount)
    {
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].RemoveIntegrity(amount, Damage.DamageClass.Abstract);
        this.Mustard -= this.m_maxMustard * amount;
      }
    }

    public void TeleportSosig(Vector3 point)
    {
    }

    private void BuffUpdate()
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead)
      {
        this.m_isHealing = false;
        this.m_isDamResist = false;
        this.m_isInvuln = false;
        this.m_isInfiniteAmmo = false;
        this.m_isGhosted = false;
        this.m_isMuscleMeat = false;
        this.m_isCyclops = false;
        this.m_isHurting = false;
        this.m_isDamMult = false;
        this.m_isFragile = false;
        this.m_isAmmoDrain = false;
        this.m_isSuperVisible = false;
        this.m_isWeakMeat = false;
        this.m_isBiClops = false;
      }
      if (this.m_isHealing && (double) this.m_buffTime_Heal > 0.0)
      {
        this.m_buffTime_Heal -= Time.deltaTime;
        if ((double) this.m_buffTime_Heal <= 0.0)
        {
          this.DeActivateBuff(10);
          this.m_isHealing = false;
        }
      }
      if (this.m_isDamResist && (double) this.m_buffTime_DamResist > 0.0)
      {
        this.m_buffTime_DamResist -= Time.deltaTime;
        if ((double) this.m_buffTime_DamResist <= 0.0)
        {
          this.DeActivateBuff(3);
          this.m_isDamResist = false;
        }
      }
      if (this.m_isInvuln && (double) this.m_buffTime_Invuln > 0.0)
      {
        this.m_buffTime_Invuln -= Time.deltaTime;
        if ((double) this.m_buffTime_Invuln <= 0.0)
        {
          this.DeActivateBuff(3);
          this.m_isInvuln = false;
        }
      }
      if (this.m_isDamPowerUp && (double) this.m_buffTime_DamPowerUp > 0.0)
      {
        this.m_buffTime_DamPowerUp -= Time.deltaTime;
        if ((double) this.m_buffTime_DamPowerUp <= 0.0)
        {
          this.DeActivateBuff(1);
          this.m_isDamPowerUp = false;
        }
      }
      if (this.m_isInfiniteAmmo && (double) this.m_buffTime_InfiniteAmmo > 0.0)
      {
        this.m_buffTime_InfiniteAmmo -= Time.deltaTime;
        if ((double) this.m_buffTime_InfiniteAmmo <= 0.0)
        {
          this.DeActivateBuff(2);
          this.m_isInfiniteAmmo = false;
        }
      }
      if (this.m_isGhosted && (double) this.m_buffTime_Ghosted > 0.0)
      {
        this.m_buffTime_Ghosted -= Time.deltaTime;
        if ((double) this.m_buffTime_Ghosted <= 0.0)
        {
          this.DeActivateBuff(4);
          this.m_isGhosted = false;
        }
      }
      if (this.m_isMuscleMeat && (double) this.m_buffTime_MuscleMeat > 0.0)
      {
        this.m_buffTime_MuscleMeat -= Time.deltaTime;
        if ((double) this.m_buffTime_MuscleMeat <= 0.0)
        {
          this.DeActivateBuff(6);
          this.m_isMuscleMeat = false;
        }
      }
      if (this.m_isCyclops && (double) this.m_buffTime_Cyclops > 0.0)
      {
        this.m_buffTime_Cyclops -= Time.deltaTime;
        if ((double) this.m_buffTime_Cyclops <= 0.0)
        {
          this.DeActivateBuff(11);
          this.m_isCyclops = false;
          if ((UnityEngine.Object) this.m_vfx_cyclops != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_vfx_cyclops);
        }
      }
      if (this.m_isBlort && (double) this.m_buffTime_Blort > 0.0)
      {
        this.m_buffTime_Blort -= Time.deltaTime;
        if ((double) this.m_buffTime_Blort <= 0.0)
        {
          this.DeActivateBuff(9);
          this.m_isBlort = false;
        }
      }
      if (this.m_isSpeedup && (double) this.m_buffTime_Speedup > 0.0)
      {
        this.m_buffTime_Speedup -= Time.deltaTime;
        if ((double) this.m_buffTime_Speedup <= 0.0)
          this.m_isSpeedup = false;
      }
      if (this.m_isDlort && (double) this.m_debuffTime_Dlort > 0.0)
      {
        this.m_debuffTime_Dlort -= Time.deltaTime;
        if ((double) this.m_debuffTime_Dlort <= 0.0)
        {
          this.DeActivateBuff(9);
          this.m_isDlort = false;
        }
      }
      if (this.m_isChillOut && (double) this.m_debuffTime_ChillOut > 0.0)
      {
        this.m_debuffTime_ChillOut -= Time.deltaTime;
        if ((double) this.m_debuffTime_ChillOut <= 0.0)
        {
          this.m_isChillOut = false;
          for (int index = 0; index < this.Links.Count; ++index)
          {
            if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
              this.Links[index].R.isKinematic = false;
          }
        }
      }
      if (this.m_isVomitting && (double) this.m_debuffTime_Vomit > 0.0)
      {
        this.m_debuffTime_Vomit -= Time.deltaTime;
        if ((double) this.m_debuffTime_Vomit <= 0.0)
        {
          this.m_isVomitting = false;
          if ((UnityEngine.Object) this.m_vfx_vomit != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_vfx_vomit);
        }
      }
      if (this.m_isHurting && (double) this.m_debuffTime_Hurt > 0.0)
      {
        this.m_debuffTime_Hurt -= Time.deltaTime;
        if ((double) this.m_debuffTime_Hurt <= 0.0)
        {
          this.DeActivateDeBuff(10);
          this.m_isHurting = false;
        }
      }
      if (this.m_isDamMult && (double) this.m_debuffTime_DamMult > 0.0)
      {
        this.m_debuffTime_DamMult -= Time.deltaTime;
        if ((double) this.m_debuffTime_DamMult <= 0.0)
        {
          this.DeActivateDeBuff(3);
          this.m_isDamMult = false;
        }
      }
      if (this.m_isFragile && (double) this.m_debuffTime_Fragile > 0.0)
      {
        this.m_debuffTime_Fragile -= Time.deltaTime;
        if ((double) this.m_debuffTime_Fragile <= 0.0)
        {
          this.DeActivateDeBuff(3);
          this.m_isFragile = false;
        }
      }
      if (this.m_isDamPowerDown && (double) this.m_debuffTime_DamPowerDown > 0.0)
      {
        this.m_debuffTime_DamPowerDown -= Time.deltaTime;
        if ((double) this.m_debuffTime_DamPowerDown <= 0.0)
        {
          this.DeActivateDeBuff(1);
          this.m_isDamPowerDown = false;
        }
      }
      if (this.m_isAmmoDrain && (double) this.m_debuffTime_AmmoDrain > 0.0)
      {
        this.m_debuffTime_AmmoDrain -= Time.deltaTime;
        if ((double) this.m_debuffTime_AmmoDrain <= 0.0)
        {
          this.DeActivateDeBuff(2);
          this.m_isAmmoDrain = false;
        }
      }
      if (this.m_isSuperVisible && (double) this.m_debuffTime_SuperVisible > 0.0)
      {
        this.m_debuffTime_SuperVisible -= Time.deltaTime;
        if ((double) this.m_debuffTime_SuperVisible <= 0.0)
        {
          this.DeActivateDeBuff(4);
          this.m_isSuperVisible = false;
        }
      }
      if (this.m_isWeakMeat && (double) this.m_debuffTime_WeakMeat > 0.0)
      {
        this.m_debuffTime_WeakMeat -= Time.deltaTime;
        if ((double) this.m_debuffTime_WeakMeat <= 0.0)
        {
          this.DeActivateDeBuff(6);
          this.m_isWeakMeat = false;
        }
      }
      if (this.m_isBiClops && (double) this.m_debuffTime_BiClops > 0.0)
      {
        this.m_debuffTime_BiClops -= Time.deltaTime;
        if ((double) this.m_debuffTime_BiClops <= 0.0)
        {
          this.DeActivateDeBuff(11);
          this.m_isBiClops = false;
          if ((UnityEngine.Object) this.m_vfx_bicyclops != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_vfx_bicyclops);
        }
      }
      if (this.m_isFrozen && (double) this.m_debuffTime_Freeze > 0.0)
      {
        this.m_debuffTime_Freeze -= Time.deltaTime;
        if ((double) this.m_debuffTime_Freeze <= 0.0)
          this.m_isFrozen = false;
      }
      if (this.m_isDebuff && (double) this.m_debuffTime_Debuff > 0.0)
      {
        this.m_debuffTime_Debuff -= Time.deltaTime;
        if ((double) this.m_debuffTime_Debuff <= 0.0)
        {
          this.DeActivateBuff(14);
          this.m_isDebuff = false;
        }
      }
      if (this.m_isChillOut)
      {
        for (int index = 0; index < this.Links.Count; ++index)
        {
          if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
            this.Links[index].R.isKinematic = true;
        }
      }
      Sosig.MaterialType curMat = this.CurMat;
      if (this.CurMat == Sosig.MaterialType.Vaporize)
        return;
      if (this.m_isGhosted && this.m_hasInvisMaterial)
      {
        if (this.CurMat != Sosig.MaterialType.Invis)
        {
          this.CurMat = Sosig.MaterialType.Invis;
          for (int index = 0; index < this.Links.Count; ++index)
          {
            if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
              this.Renderers[index].material = this.InvisMaterial;
          }
        }
      }
      else if (this.m_isFrozen && this.m_hasFrozenMaterial)
      {
        if (this.CurMat != Sosig.MaterialType.Frozen)
        {
          this.CurMat = Sosig.MaterialType.Frozen;
          for (int index = 0; index < this.Links.Count; ++index)
          {
            if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
              this.Renderers[index].material = this.FrozenMaterial;
          }
        }
      }
      else if (this.m_isInvuln && this.m_hasInvulnMaterial)
      {
        if (this.CurMat != Sosig.MaterialType.Invuln)
        {
          this.CurMat = Sosig.MaterialType.Invuln;
          for (int index = 0; index < this.Links.Count; ++index)
          {
            if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
              this.Renderers[index].material = this.InvulnMaterial;
          }
        }
      }
      else if (this.CurMat != Sosig.MaterialType.Standard)
      {
        this.CurMat = Sosig.MaterialType.Standard;
        for (int index = 0; index < this.Links.Count; ++index)
        {
          if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
            this.Renderers[index].material = this.GibMaterial;
        }
      }
      if (this.CurMat == Sosig.MaterialType.Invis && curMat != Sosig.MaterialType.Invis)
      {
        for (int index = 0; index < this.Links.Count; ++index)
        {
          if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
            this.Links[index].DisableWearables();
        }
      }
      if (this.CurMat == Sosig.MaterialType.Invis || curMat != Sosig.MaterialType.Invis)
        return;
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].EnableWearables();
      }
    }

    public void Vaporize(GameObject PSystemPrefab_Lightning, int iff)
    {
      if (this.m_isVaporizing)
        return;
      this.m_isVaporizing = true;
      this.SosigDies(Damage.DamageClass.Projectile, Sosig.SosigDeathType.JointExplosion);
      this.m_lastIFFDamageSource = iff;
      this.m_vaporizeSystems = new List<Transform>();
      if (this.CurMat == Sosig.MaterialType.Vaporize)
        return;
      this.CurMat = Sosig.MaterialType.Vaporize;
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
        {
          this.Renderers[index].material = this.VaporizeMaterial;
          UnityEngine.Object.Instantiate<GameObject>(PSystemPrefab_Lightning, this.Links[index].transform.position, this.Links[index].transform.rotation).transform.SetParent(this.Links[index].transform);
          this.Links[index].R.useGravity = false;
          this.Links[index].R.drag = 4.5f;
          this.Links[index].R.angularDrag = 3f;
          this.Links[index].C.enabled = false;
        }
      }
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].DisableWearables();
      }
    }

    private void VaporizeUpdate()
    {
      if (!this.m_isVaporizing)
        return;
      this.m_vaporizeTime -= Time.deltaTime;
      if ((double) this.m_vaporizeTime >= 0.0 || this.m_isVaporized)
        return;
      this.m_isVaporized = true;
      for (int index = 0; index < this.m_vaporizeSystems.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_vaporizeSystems[index] != (UnityEngine.Object) null)
          this.m_vaporizeSystems[index].SetParent((Transform) null);
      }
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
          this.Links[index].LinkExplodes(Damage.DamageClass.Projectile);
      }
      for (int index = 0; index < this.Hands.Count; ++index)
        this.Hands[index].DropHeldObject();
      this.Inventory.DropAllObjects();
      this.ClearCoverPoint();
    }

    private void BleedingUpdate()
    {
      if ((double) this.m_storedShudder > 0.0)
        this.m_storedShudder -= Time.deltaTime;
      else
        this.m_storedShudder = 0.0f;
      if (this.m_needsToSpawnBleedEvent && (UnityEngine.Object) this.m_linkToBleed != (UnityEngine.Object) null && (double) this.Mustard > 0.0)
      {
        this.m_needsToSpawnBleedEvent = false;
        if ((double) this.m_bloodLossForVFX >= 10.0)
        {
          UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_LargeMustardBurst, this.m_bloodLossPoint, Quaternion.LookRotation(this.m_bloodLossDir));
          this.m_bleedingEvents.Add(new Sosig.BleedingEvent(this.DamageFX_MustardSpoutLarge, this.m_linkToBleed, this.m_bloodLossForVFX, this.m_bloodLossPoint, -this.m_bloodLossDir, this.m_bloodLossForVFX * 0.25f, this.BleedVFXIntensity));
        }
        if ((double) this.m_bloodLossForVFX >= 1.0)
        {
          UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_SmallMustardBurst, this.m_bloodLossPoint, Quaternion.LookRotation(-this.m_bloodLossDir));
          this.m_bleedingEvents.Add(new Sosig.BleedingEvent(this.DamageFX_MustardSpoutSmall, this.m_linkToBleed, this.m_bloodLossForVFX, this.m_bloodLossPoint, -this.m_bloodLossDir, this.m_bloodLossForVFX * 0.25f, this.BleedVFXIntensity));
        }
      }
      this.m_bleedRate = 0.0f;
      if (this.m_bleedingEvents.Count > 0)
      {
        float deltaTime = Time.deltaTime;
        for (int index = this.m_bleedingEvents.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) this.m_bleedingEvents[index].l == (UnityEngine.Object) null || this.m_bleedingEvents[index].IsDone())
          {
            if ((UnityEngine.Object) this.m_bleedingEvents[index].m_system != (UnityEngine.Object) null)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bleedingEvents[index].m_system);
            this.m_bleedingEvents.RemoveAt(index);
          }
          else
            this.m_bleedRate += this.m_bleedingEvents[index].Update(deltaTime, this.Mustard);
        }
      }
      if ((double) this.m_bleedRate <= 0.0 && this.BodyState != Sosig.SosigBodyState.Dead && this.m_receivedHeadShot)
        this.m_receivedHeadShot = false;
      if (this.m_isHealing)
      {
        this.Mustard += this.BuffIntensity_HealHarm * Time.deltaTime;
        this.Mustard = Mathf.Clamp(this.Mustard, this.Mustard, this.m_maxMustard * 150f);
        for (int index = 0; index < this.Links.Count; ++index)
        {
          if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
            this.Links[index].HealIntegrity(this.BuffIntensity_HealHarm * Time.deltaTime);
        }
      }
      else if (this.m_isHurting)
      {
        this.Mustard -= this.BuffIntensity_HealHarm * Time.deltaTime;
        for (int index = 0; index < this.Links.Count; ++index)
        {
          if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null)
            this.Links[index].RemoveIntegrity(this.BuffIntensity_HealHarm * Time.deltaTime, Damage.DamageClass.Abstract);
        }
      }
      else if ((double) this.Mustard > (double) this.m_maxMustard)
        this.Mustard -= Time.deltaTime * 2f;
      if ((double) this.Mustard > 0.0 && (double) this.m_bleedRate > 0.0)
      {
        this.Mustard -= this.m_bleedRate * this.BleedRateMult;
        if ((double) this.Mustard <= 0.0)
          this.SosigDies(Damage.DamageClass.Abstract, Sosig.SosigDeathType.BleedOut);
      }
      this.m_hitDecalsThisFrameSoFar = 0;
      this.m_linkToBleed = (SosigLink) null;
      this.m_bloodLossForVFX = 0.0f;
    }

    public float DistanceFromCoreTarget() => Vector3.Distance(this.CoreRB.position, this.CoreTarget.position);

    public void RequestHitDecal(Vector3 point, Vector3 normal, float scale, SosigLink l)
    {
      this.CleanUpDecals();
      if (this.m_hitDecalsThisFrameSoFar >= this.MaxDecalsPerFrame)
        return;
      ++this.m_hitDecalsThisFrameSoFar;
      point = l.C.ClosestPoint(point);
      if ((double) Mathf.Abs(l.transform.InverseTransformPoint(point).y) > 0.230000004172325)
        return;
      if (this.m_spawnedHitDecals.Count < this.MaxTotalDecals)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.HitDecalPrefabs[UnityEngine.Random.Range(0, this.HitDecalPrefabs.Length)], point, Quaternion.LookRotation(normal, UnityEngine.Random.onUnitSphere));
        this.m_spawnedHitDecals.Add(gameObject.transform);
        scale = Mathf.Clamp(scale, this.HitDecalSizeRange.x, this.HitDecalSizeRange.y);
        gameObject.transform.localScale = new Vector3(scale, scale, scale * 0.5f);
        gameObject.transform.SetParent(l.transform);
      }
      else
      {
        if (this.m_nextDecalToMoveIndex >= this.m_spawnedHitDecals.Count)
          this.m_nextDecalToMoveIndex = 0;
        if ((UnityEngine.Object) this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex] == (UnityEngine.Object) null)
        {
          this.m_spawnedHitDecals.RemoveAt(this.m_nextDecalToMoveIndex);
        }
        else
        {
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].position = point;
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].rotation = Quaternion.LookRotation(normal, UnityEngine.Random.onUnitSphere);
          scale = Mathf.Clamp(scale, this.HitDecalSizeRange.x, this.HitDecalSizeRange.y);
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].transform.localScale = new Vector3(scale, scale, scale * 0.5f);
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].SetParent(l.transform);
          ++this.m_nextDecalToMoveIndex;
          if (this.m_nextDecalToMoveIndex < this.m_spawnedHitDecals.Count)
            return;
          this.m_nextDecalToMoveIndex = 0;
        }
      }
    }

    public void RequestHitDecal(
      Vector3 point,
      Vector3 normal,
      Vector3 edgeNormal,
      float scale,
      SosigLink l)
    {
      this.CleanUpDecals();
      if (this.m_hitDecalsThisFrameSoFar >= this.MaxDecalsPerFrame)
        return;
      ++this.m_hitDecalsThisFrameSoFar;
      point = l.C.ClosestPoint(point);
      if (this.m_spawnedHitDecals.Count < this.MaxTotalDecals)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.HitDecalPrefabs[UnityEngine.Random.Range(0, this.HitDecalPrefabs.Length)], point, Quaternion.LookRotation(normal, edgeNormal));
        this.m_spawnedHitDecals.Add(gameObject.transform);
        scale = Mathf.Clamp(scale, this.HitDecalSizeRange.x, this.HitDecalSizeRange.y);
        gameObject.transform.localScale = new Vector3(scale * 0.2f, scale, scale * 0.5f);
        gameObject.transform.SetParent(l.transform);
      }
      else
      {
        if (this.m_nextDecalToMoveIndex >= this.m_spawnedHitDecals.Count)
          this.m_nextDecalToMoveIndex = 0;
        if ((UnityEngine.Object) this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex] == (UnityEngine.Object) null)
        {
          this.m_spawnedHitDecals.RemoveAt(this.m_nextDecalToMoveIndex);
        }
        else
        {
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].position = point;
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].rotation = Quaternion.LookRotation(normal, UnityEngine.Random.onUnitSphere);
          scale = Mathf.Clamp(scale, this.HitDecalSizeRange.x, this.HitDecalSizeRange.y);
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].transform.localScale = new Vector3(scale * 0.2f, scale, scale * 0.5f);
          this.m_spawnedHitDecals[this.m_nextDecalToMoveIndex].SetParent(l.transform);
          ++this.m_nextDecalToMoveIndex;
          if (this.m_nextDecalToMoveIndex < this.m_spawnedHitDecals.Count)
            return;
          this.m_nextDecalToMoveIndex = 0;
        }
      }
    }

    private void CleanUpDecals()
    {
      if (this.m_spawnedHitDecals.Count <= 0)
        return;
      for (int index = this.m_spawnedHitDecals.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_spawnedHitDecals[index] == (UnityEngine.Object) null)
          this.m_spawnedHitDecals.RemoveAt(index);
      }
      if (this.m_nextDecalToMoveIndex < this.m_spawnedHitDecals.Count)
        return;
      this.m_nextDecalToMoveIndex = this.m_spawnedHitDecals.Count - 1;
      if (this.m_nextDecalToMoveIndex >= 0)
        return;
      this.m_nextDecalToMoveIndex = 0;
    }

    public void AccurueBleedingHit(SosigLink l, Vector3 point, Vector3 dir, float bloodAmount)
    {
      this.m_needsToSpawnBleedEvent = true;
      this.m_linkToBleed = l;
      this.m_bloodLossPoint = l.C.ClosestPoint(point);
      this.m_bloodLossDir = dir;
      this.m_bloodLossForVFX += bloodAmount * this.BleedDamageMult;
      if (this.BodyState == Sosig.SosigBodyState.Dead || l.BodyPart != SosigLink.SosigBodyPart.Head)
        return;
      this.m_receivedHeadShot = true;
    }

    public void ProcessDamage(Damage d, SosigLink link)
    {
      this.Stun(d.Dam_Stunning);
      if ((double) d.Dam_Stunning > 1.0)
        this.Shudder(d.Dam_Stunning);
      this.SetLastIFFDamageSource(d.Source_IFF);
      if (d.Class != Damage.DamageClass.Abstract && d.Source_IFF != this.E.IFFCode && this.BodyState != Sosig.SosigBodyState.Dead)
        this.m_diedFromClass = d.Class;
      this.Blind(d.Dam_Blinding);
      this.ProcessDamage(d.Dam_Piercing, d.Dam_Cutting, d.Dam_Blunt, d.Dam_Thermal, d.point, link);
    }

    public void ProcessDamage(
      float damage_p,
      float damage_c,
      float damage_b,
      float damage_t,
      Vector3 point,
      SosigLink link)
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead || this.m_isInvuln)
        return;
      damage_p *= this.DamMult_Piercing;
      damage_c *= this.DamMult_Cutting;
      damage_b *= this.DamMult_Blunt;
      damage_t *= this.DamMult_Thermal;
      if ((double) damage_t > 0.0 && this.m_isFrozen)
      {
        this.m_isFrozen = false;
        this.m_debuffTime_Freeze = 0.0f;
      }
      if (this.m_isBlort)
        damage_t = 0.0f;
      else if (this.m_isDlort)
        damage_t *= 10f;
      if (this.BodyState != Sosig.SosigBodyState.Dead && link.BodyPart == SosigLink.SosigBodyPart.Head)
        this.m_receivedHeadShot = true;
      float amount = Mathf.Lerp(0.01f, 5f, (float) (((double) damage_p + (double) damage_c + (double) damage_b + (double) damage_t * 5.0) / 2000.0)) * link.StaggerMagnitude;
      if (link.BodyPart == SosigLink.SosigBodyPart.Head && (double) amount > (double) this.StunThreshold)
      {
        this.m_isStunned = true;
        this.m_stunTimeLeft = Mathf.Max(this.m_stunTimeLeft, amount * this.StunMultiplier);
      }
      if ((double) amount >= 0.100000001490116)
      {
        this.m_timeSinceLastDamage = 0.0f;
        if ((double) UnityEngine.Random.value > 0.300000011920929)
          this.Invoke("DelayedSpeakPain", 0.01f);
      }
      if ((double) amount > (double) this.ConfusionThreshold)
      {
        this.m_isConfused = true;
        this.m_confusedTime = Mathf.Max(this.m_confusedTime, amount * this.ConfusionMultiplier);
      }
      if ((double) amount + (double) this.m_storedShudder > (double) this.ShudderThreshold)
      {
        this.m_storedShudder = 0.0f;
        this.Shudder(amount);
      }
      else if ((double) amount > (double) this.ShudderThreshold * 0.200000002980232)
        this.m_storedShudder += amount;
      if ((double) amount < 0.100000001490116 || this.m_isStunned && this.m_isConfused)
        return;
      float num = Mathf.Clamp(amount, 0.0f, 1f);
      this.EventReceive(new AIEvent(point, AIEvent.AIEType.Damage, 1f - num));
    }

    public void Shudder(float amount)
    {
      if (this.IsInvuln)
        return;
      this.SetBodyState(Sosig.SosigBodyState.Ballistic);
      this.m_recoveryFromBallisticTick = Mathf.Max(amount, this.m_recoveryFromBallisticTick) + amount * 0.5f;
      if (this.DoesDropWeaponsOnBallistic)
      {
        for (int index = 0; index < this.Hands.Count; ++index)
          this.Hands[index].DropHeldObject();
      }
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null && !this.m_jointsSevered[index])
        {
          amount = Mathf.Clamp(amount, 0.0f, 0.4f);
          this.Links[index].R.AddForce(UnityEngine.Random.onUnitSphere * amount * 5f, ForceMode.VelocityChange);
        }
      }
    }

    public void BreakJoint(SosigLink link, bool isStart, Damage.DamageClass damClass)
    {
      if (this.m_jointsBroken[(int) link.BodyPart])
        return;
      this.m_jointsBroken[(int) link.BodyPart] = true;
      if (link.BodyPart == SosigLink.SosigBodyPart.Head)
      {
        this.NeckBreak(isStart);
        if (!this.m_doesJointBreakKill_Head)
          return;
        this.SosigDies(damClass, Sosig.SosigDeathType.JointBreak);
      }
      else if (link.BodyPart == SosigLink.SosigBodyPart.UpperLink)
      {
        this.BreakBack(isStart);
        if (!this.m_doesJointBreakKill_Upper)
          return;
        this.SosigDies(damClass, Sosig.SosigDeathType.JointBreak);
      }
      else
      {
        if (link.BodyPart != SosigLink.SosigBodyPart.LowerLink)
          return;
        this.Hobble(isStart);
        if (!this.m_doesJointBreakKill_Lower)
          return;
        this.SosigDies(damClass, Sosig.SosigDeathType.JointBreak);
      }
    }

    public void SeverJoint(
      SosigLink link,
      bool isJointSlice,
      Damage.DamageClass damClass,
      bool isPullApart)
    {
      if (this.m_jointsSevered[(int) link.BodyPart])
        return;
      this.m_jointsSevered[(int) link.BodyPart] = true;
      UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_LargeMustardBurst, link.transform.position + Vector3.up * link.J.anchor.y, Quaternion.LookRotation(link.transform.up));
      UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_LargeMustardBurst, link.transform.position + Vector3.up * link.J.anchor.y, Quaternion.LookRotation(-link.transform.up));
      this.UpdateRenderers();
      UnityEngine.Object.Destroy((UnityEngine.Object) link.J);
      if ((double) UnityEngine.Random.value > 0.5)
      {
        if (isJointSlice)
          this.Speak_Pain(this.Speech.OnJointSlice);
        else
          this.Speak_Pain(this.Speech.OnJointSever);
      }
      else
        this.Invoke("DelayedSpeakPain", 0.01f);
      Sosig.SosigDeathType deathType = Sosig.SosigDeathType.JointSever;
      if (isPullApart)
        deathType = Sosig.SosigDeathType.JointPullApart;
      switch (link.BodyPart)
      {
        case SosigLink.SosigBodyPart.Head:
          if (!this.m_doesSeverKill_Head)
            break;
          this.SosigDies(damClass, deathType);
          break;
        case SosigLink.SosigBodyPart.UpperLink:
          if (this.m_doesSeverKill_Upper)
          {
            this.SosigDies(damClass, deathType);
            break;
          }
          this.BreakBack(false);
          break;
        case SosigLink.SosigBodyPart.LowerLink:
          if (this.m_doesSeverKill_Lower)
          {
            this.SosigDies(damClass, deathType);
            break;
          }
          this.Hobble(false);
          break;
      }
    }

    public void DestroyLink(SosigLink link, Damage.DamageClass damClass)
    {
      if ((UnityEngine.Object) link.J != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) link.J);
      for (int index = this.Links.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null && (UnityEngine.Object) this.Links[index].J != (UnityEngine.Object) null && (UnityEngine.Object) this.Links[index].J.connectedBody == (UnityEngine.Object) link.R)
        {
          this.Links[index].transform.SetParent((Transform) null);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.Links[index].J);
        }
      }
      this.m_linksDestroyed[(int) link.BodyPart] = true;
      this.Mustard *= 0.75f;
      switch (link.BodyPart)
      {
        case SosigLink.SosigBodyPart.Head:
          this.KillSpeech();
          if (this.m_doesExplodeKill_Head)
          {
            this.SosigDies(damClass, Sosig.SosigDeathType.JointExplosion);
            break;
          }
          this.Shudder(3f);
          break;
        case SosigLink.SosigBodyPart.Torso:
          this.SosigDies(damClass, Sosig.SosigDeathType.JointExplosion);
          break;
        case SosigLink.SosigBodyPart.UpperLink:
          if (this.m_doesExplodeKill_Upper)
          {
            this.SosigDies(damClass, Sosig.SosigDeathType.JointExplosion);
            break;
          }
          this.BreakBack(false);
          this.Shudder(2f);
          break;
        case SosigLink.SosigBodyPart.LowerLink:
          if (this.m_doesExplodeKill_Lower)
          {
            this.SosigDies(damClass, Sosig.SosigDeathType.JointExplosion);
            break;
          }
          this.Hobble(false);
          this.Shudder(2f);
          break;
      }
      UnityEngine.Object.Instantiate<GameObject>(this.DamageFX_Explosion, link.transform.position, UnityEngine.Random.rotation);
      if (link.HasSpawnOnDestroy)
        UnityEngine.Object.Instantiate<GameObject>(link.SpawnOnDestroy.GetGameObject(), link.transform.position, Quaternion.identity);
      if (this.UsesGibs)
      {
        Vector3 position1 = link.transform.position;
        Quaternion rotation = link.transform.rotation;
        for (int index = 0; index < this.GibLocalPoses.Length; ++index)
        {
          Vector3 position2 = link.transform.position + link.transform.right * this.GibLocalPoses[index].x + link.transform.up * this.GibLocalPoses[index].y + link.transform.forward * this.GibLocalPoses[index].z;
          UnityEngine.Object.Instantiate<GameObject>(this.GibPrefabs[index], position2, rotation);
        }
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) link.gameObject);
      this.CleanUpDecals();
      this.UpdateRenderers();
    }

    public void NeckBreak(bool isStarting)
    {
      this.BreakJointLimit(0);
      if (isStarting)
        return;
      if ((double) UnityEngine.Random.value > 0.5)
        this.Speak_Pain(this.Speech.OnJointBreak);
      else
        this.Speak_Pain(this.Speech.OnNeckBreak);
    }

    public void Hobble(bool isStarting)
    {
      this.m_isHobbled = true;
      this.SetBodyPose(Sosig.SosigBodyPose.Crouching);
      this.BreakJointLimit(3);
      if (isStarting)
        return;
      this.Shudder(UnityEngine.Random.Range(1.5f, 3f));
      this.Confuse(10f);
      if ((double) UnityEngine.Random.value > 0.5)
        this.Speak_Pain(this.Speech.OnJointBreak);
      else
        this.Invoke("DelayedSpeakPain", 0.01f);
    }

    public void BreakBack(bool isStarting)
    {
      this.m_isBackBroken = true;
      this.SetBodyPose(Sosig.SosigBodyPose.Prone);
      this.BreakJointLimit(2);
      if (isStarting)
        return;
      this.Shudder(UnityEngine.Random.Range(2f, 5f));
      this.Confuse(20f);
      if ((double) UnityEngine.Random.value > 0.5)
        this.Speak_Pain(this.Speech.OnJointBreak);
      else
        this.Speak_Pain(this.Speech.OnBackBreak);
    }

    public void Confuse(float f)
    {
      this.m_isConfused = true;
      this.m_confusedTime = Mathf.Max(this.m_confusedTime, f);
    }

    public void Stun(float f)
    {
      this.m_isStunned = true;
      this.m_stunTimeLeft = Mathf.Max(this.m_stunTimeLeft, f);
    }

    public void Blind(float f)
    {
      this.m_isBlinded = true;
      this.m_blindTime = Mathf.Max(this.m_blindTime, f);
    }

    private void UpdateRenderers()
    {
      for (int i = 0; i < this.Links.Count; ++i)
      {
        if (!this.m_linksDestroyed[i])
          this.UpdateRendererOnLink(i);
      }
    }

    public void UpdateRendererOnLink(int i)
    {
      if (this.m_linksDestroyed[i])
        return;
      if (this.UsesLinkMeshOverride)
      {
        switch (i)
        {
          case 0:
            int damageMeshIndex1 = this.GetDamageMeshIndex(this.Links[0]);
            if (this.m_linksDestroyed[1] || this.m_jointsSevered[0] && !this.m_linksDestroyed[0])
            {
              this.Meshes[0].mesh = this.Links[0].Meshes_Severed_Top[damageMeshIndex1];
              break;
            }
            this.Meshes[0].mesh = this.Links[0].Meshes_Whole[damageMeshIndex1];
            break;
          case 1:
            int damageMeshIndex2 = this.GetDamageMeshIndex(this.Links[1]);
            if ((this.m_linksDestroyed[0] || this.m_jointsSevered[0]) && (this.m_linksDestroyed[2] || this.m_jointsSevered[2]))
            {
              this.Meshes[1].mesh = this.Links[1].Meshes_Severed_Both[damageMeshIndex2];
              break;
            }
            if (this.m_linksDestroyed[2] || this.m_jointsSevered[2])
            {
              this.Meshes[1].mesh = this.Links[1].Meshes_Severed_Top[damageMeshIndex2];
              break;
            }
            if (this.m_linksDestroyed[0] || this.m_jointsSevered[0])
            {
              this.Meshes[1].mesh = this.Links[1].Meshes_Severed_Bottom[damageMeshIndex2];
              break;
            }
            this.Meshes[1].mesh = this.Links[1].Meshes_Whole[damageMeshIndex2];
            break;
          case 2:
            int damageMeshIndex3 = this.GetDamageMeshIndex(this.Links[2]);
            if ((this.m_linksDestroyed[1] || this.m_jointsSevered[2]) && (this.m_linksDestroyed[3] || this.m_jointsSevered[3]))
            {
              this.Meshes[2].mesh = this.Links[2].Meshes_Severed_Both[damageMeshIndex3];
              break;
            }
            if (this.m_linksDestroyed[3] || this.m_jointsSevered[3])
            {
              this.Meshes[2].mesh = this.Links[2].Meshes_Severed_Top[damageMeshIndex3];
              break;
            }
            if (this.m_linksDestroyed[1] || this.m_jointsSevered[2])
            {
              this.Meshes[2].mesh = this.Links[2].Meshes_Severed_Bottom[damageMeshIndex3];
              break;
            }
            this.Meshes[2].mesh = this.Links[2].Meshes_Whole[damageMeshIndex3];
            break;
          case 3:
            int damageMeshIndex4 = this.GetDamageMeshIndex(this.Links[3]);
            if (this.m_linksDestroyed[2] || this.m_jointsSevered[3])
            {
              this.Meshes[3].mesh = this.Links[3].Meshes_Severed_Bottom[damageMeshIndex4];
              break;
            }
            this.Meshes[3].mesh = this.Links[3].Meshes_Whole[damageMeshIndex4];
            break;
        }
      }
      else
      {
        switch (i)
        {
          case 0:
            int damageMeshIndex5 = this.GetDamageMeshIndex(this.Links[0]);
            if (this.m_linksDestroyed[1] || this.m_jointsSevered[0] && !this.m_linksDestroyed[0])
            {
              this.Meshes[0].mesh = this.Meshes_Severed_Top[damageMeshIndex5];
              break;
            }
            this.Meshes[0].mesh = this.Meshes_Whole[damageMeshIndex5];
            break;
          case 1:
            int damageMeshIndex6 = this.GetDamageMeshIndex(this.Links[1]);
            if ((this.m_linksDestroyed[0] || this.m_jointsSevered[0]) && (this.m_linksDestroyed[2] || this.m_jointsSevered[2]))
            {
              this.Meshes[1].mesh = this.Meshes_Severed_Both[damageMeshIndex6];
              break;
            }
            if (this.m_linksDestroyed[2] || this.m_jointsSevered[2])
            {
              this.Meshes[1].mesh = this.Meshes_Severed_Top[damageMeshIndex6];
              break;
            }
            if (this.m_linksDestroyed[0] || this.m_jointsSevered[0])
            {
              this.Meshes[1].mesh = this.Meshes_Severed_Bottom[damageMeshIndex6];
              break;
            }
            this.Meshes[1].mesh = this.Meshes_Whole[damageMeshIndex6];
            break;
          case 2:
            int damageMeshIndex7 = this.GetDamageMeshIndex(this.Links[2]);
            if ((this.m_linksDestroyed[1] || this.m_jointsSevered[2]) && (this.m_linksDestroyed[3] || this.m_jointsSevered[3]))
            {
              this.Meshes[2].mesh = this.Meshes_Severed_Both[damageMeshIndex7];
              break;
            }
            if (this.m_linksDestroyed[3] || this.m_jointsSevered[3])
            {
              this.Meshes[2].mesh = this.Meshes_Severed_Top[damageMeshIndex7];
              break;
            }
            if (this.m_linksDestroyed[1] || this.m_jointsSevered[2])
            {
              this.Meshes[2].mesh = this.Meshes_Severed_Bottom[damageMeshIndex7];
              break;
            }
            this.Meshes[2].mesh = this.Meshes_Whole[damageMeshIndex7];
            break;
          case 3:
            int damageMeshIndex8 = this.GetDamageMeshIndex(this.Links[3]);
            if (this.m_linksDestroyed[2] || this.m_jointsSevered[3])
            {
              this.Meshes[3].mesh = this.Meshes_Severed_Bottom[damageMeshIndex8];
              break;
            }
            this.Meshes[3].mesh = this.Meshes_Whole[damageMeshIndex8];
            break;
        }
      }
    }

    private int GetDamageMeshIndex(SosigLink l) => (UnityEngine.Object) l == (UnityEngine.Object) null ? 0 : l.GetDamageStateIndex();

    public void Writhe()
    {
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null && !this.m_jointsSevered[index])
          this.Links[index].R.AddForce(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0.1f, 1f), ForceMode.VelocityChange);
      }
    }

    public void SetLastIFFDamageSource(int iff)
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead || iff <= -1)
        return;
      this.m_lastIFFDamageSource = iff;
    }

    public bool GetDiedFromHeadShot() => this.m_receivedHeadShot;

    public int GetDiedFromIFF() => this.m_lastIFFDamageSource;

    public Damage.DamageClass GetDiedFromClass() => this.m_diedFromClass;

    public Sosig.SosigDeathType GetDiedFromType() => this.m_diedFromType;

    public void SosigDies(Damage.DamageClass damClass, Sosig.SosigDeathType deathType)
    {
      if (this.BodyState == Sosig.SosigBodyState.Dead)
        return;
      this.DeActivateAllBuffSystems();
      if (damClass != Damage.DamageClass.Abstract)
        this.m_diedFromClass = damClass;
      this.m_diedFromType = deathType;
      if (!this.m_linksDestroyed[0])
      {
        this.Speak_Pain(this.Speech.OnDeath);
      }
      else
      {
        this.KillSpeech();
        if (this.Speech.UseAltDeathOnHeadExplode)
          this.Speak_Pain(this.Speech.OnDeathAlt);
      }
      this.SetBodyState(Sosig.SosigBodyState.Dead);
      this.CurrentOrder = Sosig.SosigOrder.Disabled;
      this.FallbackOrder = Sosig.SosigOrder.Disabled;
      this.SetHandObjectUsage(Sosig.SosigObjectUsageFocus.EmptyHands);
      this.SetMovementState(Sosig.SosigMovementState.Idle);
      this.E.IFFCode = -3;
      for (int index = 0; index < this.Hands.Count; ++index)
        this.Hands[index].DropHeldObject();
      this.Inventory.DropAllObjects();
      for (int index = 0; index < this.Links.Count; ++index)
      {
        if ((UnityEngine.Object) this.Links[index] != (UnityEngine.Object) null && !this.m_jointsSevered[index])
        {
          this.Links[index].R.AddForce(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 5f), ForceMode.VelocityChange);
          this.Links[index].O.DistantGrabbable = true;
        }
      }
      GM.CurrentSceneSettings.OnSosigKill(this);
    }

    public void ProcessCollision(SosigLink l, Collision col)
    {
      if (this.IgnoreRBs.Contains(col.collider.attachedRigidbody))
        return;
      float magnitude = col.relativeVelocity.magnitude;
      bool flag1 = false;
      if ((UnityEngine.Object) col.collider.attachedRigidbody != (UnityEngine.Object) null && !col.collider.attachedRigidbody.isKinematic)
      {
        flag1 = true;
        if ((UnityEngine.Object) col.collider.attachedRigidbody.gameObject.GetComponent<FVRMeleeWeapon>() != (UnityEngine.Object) null)
          return;
        SosigWeapon component = col.collider.attachedRigidbody.gameObject.GetComponent<SosigWeapon>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && (component.Type == SosigWeapon.SosigWeaponType.Melee || component.IsHeldByBot || component.IsInBotInventory))
          return;
      }
      if (!flag1 && this.BodyState == Sosig.SosigBodyState.InControl)
        return;
      bool flag2 = false;
      if ((UnityEngine.Object) col.contacts[0].thisCollider.transform.parent != (UnityEngine.Object) l.C.transform)
        flag2 = true;
      if (this.m_isDamResist)
        magnitude *= 0.6f;
      if (this.m_isInvuln)
        return;
      if ((double) magnitude > 1.0)
      {
        float num = 1f;
        if (flag1)
          num = 4f;
        if (l.O.IsHeld)
          num = 8f;
        if (flag2)
          num *= l.CollisionBluntDamageMultiplier;
        if (flag1 || l.O.IsHeld)
          l.DamageIntegrity(magnitude * num, 0.0f, 0.0f, 0.0f, col.relativeVelocity, col.contacts[0].point, Damage.DamageClass.Melee, -3);
      }
      if ((double) magnitude > 6.0)
      {
        float num = 1f;
        if (flag1)
          num = 2f;
        if (l.O.IsHeld)
          num = 3f;
        if (flag2)
          num *= l.CollisionBluntDamageMultiplier;
        if (flag1 || l.O.IsHeld)
          this.ProcessDamage(0.0f, 0.0f, magnitude * magnitude * num, 0.0f, col.contacts[0].point, l);
      }
      if ((double) magnitude > 20.0 && !flag1)
      {
        switch (l.BodyPart)
        {
          case SosigLink.SosigBodyPart.Head:
            l.BreakJoint(false, Damage.DamageClass.Environment);
            break;
          case SosigLink.SosigBodyPart.UpperLink:
            l.BreakJoint(false, Damage.DamageClass.Environment);
            break;
          case SosigLink.SosigBodyPart.LowerLink:
            l.BreakJoint(false, Damage.DamageClass.Environment);
            break;
        }
      }
      if ((double) magnitude <= 30.0 || !flag1 || (double) col.collider.attachedRigidbody.mass <= 1.0)
        return;
      switch (l.BodyPart)
      {
        case SosigLink.SosigBodyPart.Head:
          l.BreakJoint(false, Damage.DamageClass.Environment);
          break;
        case SosigLink.SosigBodyPart.UpperLink:
          l.BreakJoint(false, Damage.DamageClass.Environment);
          break;
        case SosigLink.SosigBodyPart.LowerLink:
          l.BreakJoint(false, Damage.DamageClass.Environment);
          break;
      }
    }

    public void Stagger(float f)
    {
      this.SetBodyState(Sosig.SosigBodyState.Ballistic);
      this.m_recoveryFromBallisticTick = Mathf.Max(f, this.m_recoveryFromBallisticTick);
    }

    private void SetBodyState(Sosig.SosigBodyState s)
    {
      if (this.BodyState == s || this.BodyState == Sosig.SosigBodyState.Dead)
        return;
      this.BodyState = s;
      switch (this.BodyState)
      {
        case Sosig.SosigBodyState.InControl:
          this.m_isCountingDownToStagger = false;
          this.m_staggerAmountToApply = 0.0f;
          break;
        case Sosig.SosigBodyState.Ballistic:
          this.EndLink();
          this.Agent.enabled = false;
          this.m_recoveringFromBallisticState = false;
          this.m_recoveryFromBallisticLerp = 0.0f;
          this.m_tickDownToWrithe = UnityEngine.Random.Range(this.m_writheTickRange.x, this.m_writheTickRange.y);
          this.LoosenJoints();
          break;
        case Sosig.SosigBodyState.Dead:
          this.EndLink();
          this.Agent.enabled = false;
          this.LoosenJoints();
          break;
      }
    }

    private void SosigPhys()
    {
      if (this.BodyState != Sosig.SosigBodyState.InControl)
        return;
      Vector3 position1 = this.CoreRB.position;
      Quaternion rotation1 = this.CoreRB.rotation;
      Vector3 position2 = this.CoreTarget.position;
      Quaternion rotation2 = this.CoreTarget.rotation;
      Vector3 vector3 = position2 - position1;
      Quaternion quaternion = rotation2 * Quaternion.Inverse(rotation1);
      float deltaTime = Time.deltaTime;
      float angle;
      Vector3 axis;
      quaternion.ToAngleAxis(out angle, out axis);
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0)
        this.CoreRB.angularVelocity = Vector3.MoveTowards(this.CoreRB.angularVelocity, deltaTime * angle * axis * this.AttachedRotationMultiplier, this.AttachedRotationFudge * Time.fixedDeltaTime);
      this.CoreRB.velocity = Vector3.MoveTowards(this.CoreRB.velocity, vector3 * this.AttachedPositionMultiplier * deltaTime, this.AttachedPositionFudge * deltaTime);
    }

    private void UpdateJoints(float l)
    {
      float num = Mathf.Lerp(60f, this.m_maxJointLimit, l);
      for (int index = 0; index < this.m_joints.Count; ++index)
      {
        if ((!this.m_jointsBroken[0] || index != 0) && (!this.m_isHobbled || index < 3) && ((!this.m_isBackBroken || index < 2) && (UnityEngine.Object) this.m_joints[index] != (UnityEngine.Object) null))
        {
          SoftJointLimit softJointLimit = this.m_joints[index].lowTwistLimit;
          softJointLimit.limit = -num;
          this.m_joints[index].lowTwistLimit = softJointLimit;
          softJointLimit = this.m_joints[index].highTwistLimit;
          softJointLimit.limit = num;
          this.m_joints[index].highTwistLimit = softJointLimit;
          softJointLimit = this.m_joints[index].swing1Limit;
          softJointLimit.limit = num;
          this.m_joints[index].swing1Limit = softJointLimit;
          softJointLimit = this.m_joints[index].swing2Limit;
          softJointLimit.limit = num;
          this.m_joints[index].swing2Limit = softJointLimit;
        }
      }
    }

    private void LoosenJoints()
    {
      for (int index = 0; index < this.m_joints.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_joints[index] != (UnityEngine.Object) null)
        {
          SoftJointLimit lowTwistLimit = this.m_joints[index].lowTwistLimit;
          lowTwistLimit.limit = -60f;
          this.m_joints[index].lowTwistLimit = lowTwistLimit;
          SoftJointLimit highTwistLimit = this.m_joints[index].highTwistLimit;
          highTwistLimit.limit = 60f;
          this.m_joints[index].highTwistLimit = highTwistLimit;
          SoftJointLimit swing1Limit = this.m_joints[index].swing1Limit;
          swing1Limit.limit = 60f;
          this.m_joints[index].swing1Limit = swing1Limit;
          SoftJointLimit swing2Limit = this.m_joints[index].swing2Limit;
          swing2Limit.limit = 60f;
          this.m_joints[index].swing2Limit = swing2Limit;
        }
      }
    }

    private void BreakJointLimit(int i)
    {
      if (this.m_linksDestroyed[i] || !((UnityEngine.Object) this.Links[i].J != (UnityEngine.Object) null))
        return;
      CharacterJoint j = this.Links[i].J;
      SoftJointLimit lowTwistLimit = j.lowTwistLimit;
      lowTwistLimit.limit = -60f;
      j.lowTwistLimit = lowTwistLimit;
      SoftJointLimit softJointLimit = j.highTwistLimit;
      softJointLimit.limit = 60f;
      j.highTwistLimit = softJointLimit;
      softJointLimit = j.swing1Limit;
      softJointLimit.limit = 35f;
      j.swing1Limit = softJointLimit;
      softJointLimit = j.swing2Limit;
      softJointLimit.limit = 35f;
      j.swing2Limit = softJointLimit;
      SoftJointLimitSpring twistLimitSpring = j.twistLimitSpring;
      twistLimitSpring.damper = 100f;
      j.twistLimitSpring = twistLimitSpring;
    }

    private void TightenJoints()
    {
      for (int index = 0; index < this.m_joints.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_joints[index] != (UnityEngine.Object) null)
        {
          SoftJointLimit lowTwistLimit = this.m_joints[index].lowTwistLimit;
          lowTwistLimit.limit = -this.m_maxJointLimit;
          this.m_joints[index].lowTwistLimit = lowTwistLimit;
          SoftJointLimit highTwistLimit = this.m_joints[index].highTwistLimit;
          highTwistLimit.limit = this.m_maxJointLimit;
          this.m_joints[index].highTwistLimit = highTwistLimit;
          SoftJointLimit swing1Limit = this.m_joints[index].swing1Limit;
          swing1Limit.limit = this.m_maxJointLimit;
          this.m_joints[index].swing1Limit = swing1Limit;
          SoftJointLimit swing2Limit = this.m_joints[index].swing2Limit;
          swing2Limit.limit = this.m_maxJointLimit;
          this.m_joints[index].swing2Limit = swing2Limit;
        }
      }
    }

    public enum SosigType
    {
      Default = 0,
      NPC = 1,
      Zosig_Basic = 10, // 0x0000000A
      Zosig_Blut = 11, // 0x0000000B
      Zosig_Spitter = 12, // 0x0000000C
      Zosig_Runner = 13, // 0x0000000D
      Zosig_Armored = 14, // 0x0000000E
      Zosig_Exploding = 15, // 0x0000000F
      Zosig_Warper = 16, // 0x00000010
    }

    public enum SosigBodyState
    {
      InControl,
      Ballistic,
      Dead,
    }

    public enum SosigHeadIconState
    {
      None,
      Exclamation,
      Confused,
      Investigating,
      Blinded,
      Suppressed,
    }

    public enum MaterialType
    {
      Standard,
      Invuln,
      Invis,
      Vaporize,
      Frozen,
    }

    [Serializable]
    public class BleedingEvent
    {
      public ParticleSystem m_system;
      public SosigLink l;
      public float mustardLeftToBleed;
      public float BleedIntensity;
      public float BleedVFXIntensity;

      public BleedingEvent(
        GameObject PrefabToSpawn,
        SosigLink L,
        float bloodAmount,
        Vector3 pos,
        Vector3 dir,
        float bleedIntensity,
        float vfxIntensity)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PrefabToSpawn, pos, Quaternion.LookRotation(dir));
        this.m_system = gameObject.GetComponent<ParticleSystem>();
        this.mustardLeftToBleed = bloodAmount;
        this.l = L;
        gameObject.transform.SetParent(this.l.transform);
        this.BleedIntensity = bleedIntensity;
        this.BleedVFXIntensity = vfxIntensity;
      }

      public float Update(float t, float totalMustardLeft)
      {
        float num;
        if ((double) this.mustardLeftToBleed > 0.0 && (double) totalMustardLeft > 0.0)
        {
          num = Mathf.Clamp(this.BleedIntensity * t, 0.0f, this.mustardLeftToBleed);
          this.mustardLeftToBleed -= num;
        }
        else
        {
          this.BleedIntensity = 0.0f;
          num = 0.0f;
        }
        if ((UnityEngine.Object) this.m_system != (UnityEngine.Object) null)
        {
          ParticleSystem.EmissionModule emission = this.m_system.emission;
          ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
          rateOverTime.mode = ParticleSystemCurveMode.Constant;
          float max = 10f * this.BleedVFXIntensity;
          rateOverTime.constant = Mathf.Clamp(this.BleedIntensity, 0.0f, max);
          emission.rateOverTime = rateOverTime;
        }
        return num;
      }

      public bool IsDone() => (double) this.mustardLeftToBleed <= 0.0 && this.m_system.particleCount <= 0;

      public void EndBleedEvent()
      {
        this.mustardLeftToBleed = 0.0f;
        if (!((UnityEngine.Object) this.m_system != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_system);
      }

      public void Dispose()
      {
        if ((UnityEngine.Object) this.m_system != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_system);
        this.l = (SosigLink) null;
      }
    }

    public enum SosigOrder
    {
      Disabled,
      GuardPoint,
      Skirmish,
      Investigate,
      SearchForEquipment,
      TakeCover,
      Wander,
      Assault,
    }

    public enum SosigObjectUsageFocus
    {
      EmptyHands,
      MaintainHeldAtRest,
      AttackTarget,
      AimAtReady,
    }

    public enum SosigMovementState
    {
      Idle,
      HoldFast,
      MoveToPoint,
      DiveToPoint,
    }

    public enum SosigMoveSpeed
    {
      Crawling,
      Sneaking,
      Walking,
      Running,
    }

    public enum SosigBodyPose
    {
      Standing,
      Crouching,
      Prone,
    }

    public enum SosigDeathType
    {
      Unknown,
      BleedOut,
      JointSever,
      JointExplosion,
      JointBreak,
      JointPullApart,
    }
  }
}
