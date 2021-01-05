using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class Sosig : MonoBehaviour
	{
		public enum SosigType
		{
			Default = 0,
			NPC = 1,
			Zosig_Basic = 10,
			Zosig_Blut = 11,
			Zosig_Spitter = 12,
			Zosig_Runner = 13,
			Zosig_Armored = 14,
			Zosig_Exploding = 0xF,
			Zosig_Warper = 0x10
		}

		public enum SosigBodyState
		{
			InControl,
			Ballistic,
			Dead
		}

		public enum SosigHeadIconState
		{
			None,
			Exclamation,
			Confused,
			Investigating,
			Blinded,
			Suppressed
		}

		public enum MaterialType
		{
			Standard,
			Invuln,
			Invis,
			Vaporize,
			Frozen
		}

		[Serializable]
		public class BleedingEvent
		{
			public ParticleSystem m_system;

			public SosigLink l;

			public float mustardLeftToBleed;

			public float BleedIntensity;

			public float BleedVFXIntensity;

			public BleedingEvent(GameObject PrefabToSpawn, SosigLink L, float bloodAmount, Vector3 pos, Vector3 dir, float bleedIntensity, float vfxIntensity)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(PrefabToSpawn, pos, Quaternion.LookRotation(dir));
				m_system = gameObject.GetComponent<ParticleSystem>();
				mustardLeftToBleed = bloodAmount;
				l = L;
				gameObject.transform.SetParent(l.transform);
				BleedIntensity = bleedIntensity;
				BleedVFXIntensity = vfxIntensity;
			}

			public float Update(float t, float totalMustardLeft)
			{
				float num;
				if (mustardLeftToBleed > 0f && totalMustardLeft > 0f)
				{
					float value = BleedIntensity * t;
					value = Mathf.Clamp(value, 0f, mustardLeftToBleed);
					num = value;
					mustardLeftToBleed -= num;
				}
				else
				{
					BleedIntensity = 0f;
					num = 0f;
				}
				if (m_system != null)
				{
					ParticleSystem.EmissionModule emission = m_system.emission;
					ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
					rateOverTime.mode = ParticleSystemCurveMode.Constant;
					float max = 10f * BleedVFXIntensity;
					rateOverTime.constant = Mathf.Clamp(BleedIntensity, 0f, max);
					emission.rateOverTime = rateOverTime;
				}
				return num;
			}

			public bool IsDone()
			{
				if (mustardLeftToBleed <= 0f && m_system.particleCount <= 0)
				{
					return true;
				}
				return false;
			}

			public void EndBleedEvent()
			{
				mustardLeftToBleed = 0f;
				if (m_system != null)
				{
					UnityEngine.Object.Destroy(m_system);
				}
			}

			public void Dispose()
			{
				if (m_system != null)
				{
					UnityEngine.Object.Destroy(m_system);
				}
				l = null;
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
			Assault
		}

		public enum SosigObjectUsageFocus
		{
			EmptyHands,
			MaintainHeldAtRest,
			AttackTarget,
			AimAtReady
		}

		public enum SosigMovementState
		{
			Idle,
			HoldFast,
			MoveToPoint,
			DiveToPoint
		}

		public enum SosigMoveSpeed
		{
			Crawling,
			Sneaking,
			Walking,
			Running
		}

		public enum SosigBodyPose
		{
			Standing,
			Crouching,
			Prone
		}

		public enum SosigDeathType
		{
			Unknown,
			BleedOut,
			JointSever,
			JointExplosion,
			JointBreak,
			JointPullApart
		}

		public SosigType Type;

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
		public SosigBodyState BodyState;

		private float m_recoveryFromBallisticTick;

		private float m_recoveryFromBallisticLerp;

		private bool m_recoveringFromBallisticState;

		private Vector3 m_recoveryFromBallisticStartPos;

		private Quaternion m_recoveryFromBallisticStartRot;

		private float m_maxJointLimit = 6f;

		[Header("HeadIconLogic")]
		public SosigHeadIconState HeadIconState;

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
			new Vector3
			{
				x = -0.0644829f,
				y = 0.0619502f,
				z = 0.05735424f
			},
			new Vector3
			{
				x = -0.06146608f,
				y = 0.06169732f,
				z = -0.05414369f
			},
			new Vector3
			{
				x = 0.0427421f,
				y = 0.0777971f,
				z = 0.05667716f
			},
			new Vector3
			{
				x = 0.04047735f,
				y = 0.08263668f,
				z = -0.04825908f
			},
			new Vector3
			{
				x = 0.04711252f,
				y = -0.08044733f,
				z = 0.06141138f
			},
			new Vector3
			{
				x = -0.04509961f,
				y = -0.05805374f,
				z = -0.05172355f
			},
			new Vector3
			{
				x = 0.0655114f,
				y = -0.06081913f,
				z = -0.036587f
			},
			new Vector3
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

		public MaterialType CurMat;

		private bool m_hasInvulnMaterial;

		private bool m_hasInvisMaterial;

		private bool m_hasFrozenMaterial;

		private bool m_hasVaporizeMaterial;

		private List<BleedingEvent> m_bleedingEvents = new List<BleedingEvent>();

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
		public SosigOrder CurrentOrder;

		public SosigOrder FallbackOrder;

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

		private SosigMoveSpeed m_assaultSpeed = SosigMoveSpeed.Running;

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

		public SosigObjectUsageFocus ObjectUsageFocus = SosigObjectUsageFocus.MaintainHeldAtRest;

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
		public SosigMovementState MovementState;

		public float Speed_Crawl = 0.3f;

		public float Speed_Sneak = 0.6f;

		public float Speed_Walk = 3.5f;

		public float Speed_Run = 1.4f;

		public float Speed_Turning = 2f;

		public float MovementRotMagnitude = 10f;

		public SosigMoveSpeed MoveSpeed = SosigMoveSpeed.Walking;

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

		public SosigBodyPose BodyPose;

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

		private SosigDeathType m_diedFromType;

		private bool m_receivedHeadShot;

		public bool IsStunned => m_isStunned;

		public bool IsConfused => m_isConfused;

		public bool IsBlinded => m_isBlinded;

		public float SuppressionLevel => m_suppressionLevel;

		private Vector3 SuppressionDir => m_suppressionDir;

		public float AlertnessLevel => m_alertnessLevel;

		public bool IsHealing => m_isHealing;

		public bool IsDamResist => m_isDamResist;

		public bool IsInvuln => m_isInvuln;

		public bool isDamPowerUp => m_isDamPowerUp;

		public bool IsInfiniteAmmo => m_isInfiniteAmmo;

		public bool IsGhosted => m_isGhosted;

		public bool IsMuscleMeat => m_isMuscleMeat;

		public bool IsCyclops => m_isCyclops;

		public bool IsSpeedUp => m_isSpeedup;

		public bool IsHurting => m_isHurting;

		public bool IsDamMult => m_isDamMult;

		public bool IsFragile => m_isFragile;

		public bool IsDamPowerDown => m_isDamPowerDown;

		public bool IsAmmoDrain => m_isAmmoDrain;

		public bool IsSuperVisible => m_isSuperVisible;

		public bool IsWeakMeat => m_isWeakMeat;

		public bool IsBiClops => m_isBiClops;

		public bool IsBlort => m_isBlort;

		public bool IsDlort => m_isDlort;

		public bool IsFrozen => m_isFrozen;

		public bool IsDebuff => m_isDebuff;

		public float BuffIntensity_HealHarm => m_buffIntensity_HealHarm;

		public float BuffIntensity_DamResistHarm => m_buffIntensity_DamResistHarm;

		public float BuffIntensity_DamPowerUpDown => m_buffIntensity_DamPowerUpDown;

		public float BuffIntensity_MuscleMeatWeak => m_buffIntensity_MuscleMeatWeak;

		public float BuffIntensity_CyclopsPower => m_buffIntensity_CyclopsPower;

		public void SetOriginalIFFTeam(int i)
		{
			m_originalIFFTeam = i;
		}

		public int GetOriginalIFFTeam()
		{
			return m_originalIFFTeam;
		}

		[ContextMenu("SetGibPoses")]
		public void SetGibPoses()
		{
			for (int i = 0; i < GibPrefabs.Count; i++)
			{
				ref Vector3 reference = ref GibLocalPoses[i];
				reference = GibPrefabs[i].transform.position;
			}
		}

		public void SetInvulnMaterial(Material m)
		{
			InvulnMaterial = m;
			m_hasInvulnMaterial = true;
		}

		public void SetInvisMaterial(Material m)
		{
			InvisMaterial = m;
			m_hasInvisMaterial = true;
		}

		public void SetFrozenMaterial(Material m)
		{
			FrozenMaterial = m;
			m_hasFrozenMaterial = true;
		}

		public void SetVaporizeMaterial(Material m)
		{
			VaporizeMaterial = m;
			m_hasVaporizeMaterial = true;
		}

		public void Configure(SosigConfigTemplate t)
		{
			AppliesDamageResistToIntegrityLoss = t.AppliesDamageResistToIntegrityLoss;
			Mustard = t.TotalMustard;
			m_maxMustard = t.TotalMustard;
			BleedDamageMult = t.BleedDamageMult;
			BleedRateMult = t.BleedRateMultiplier;
			BleedVFXIntensity = t.BleedVFXIntensity;
			SearchExtentsModifier = t.SearchExtentsModifier;
			ShudderThreshold = t.ShudderThreshold;
			ConfusionThreshold = t.ConfusionThreshold;
			ConfusionMultiplier = t.ConfusionMultiplier;
			m_maxConfusedTime = t.ConfusionTimeMax;
			StunThreshold = t.StunThreshold;
			StunMultiplier = t.StunMultiplier;
			m_maxStunTime = t.StunTimeMax;
			HasABrain = t.HasABrain;
			DoesDropWeaponsOnBallistic = t.HasABrain;
			m_assaultPointOverridesSkirmishPointWhenFurtherThan = t.AssaultPointOverridesSkirmishPointWhenFurtherThan;
			E.MaximumSightRange = t.ViewDistance;
			E.MaxHearingDistance = t.HearingDistance;
			CanPickup_Ranged = t.CanPickup_Ranged;
			CanPickup_Melee = t.CanPickup_Melee;
			CanPickup_Other = t.CanPickup_Other;
			m_doesJointBreakKill_Head = t.DoesJointBreakKill_Head;
			m_doesJointBreakKill_Upper = t.DoesJointBreakKill_Upper;
			m_doesJointBreakKill_Lower = t.DoesJointBreakKill_Lower;
			m_doesSeverKill_Head = t.DoesSeverKill_Head;
			m_doesSeverKill_Upper = t.DoesSeverKill_Upper;
			m_doesSeverKill_Lower = t.DoesSeverKill_Lower;
			m_doesExplodeKill_Head = t.DoesExplodeKill_Head;
			m_doesExplodeKill_Upper = t.DoesExplodeKill_Upper;
			m_doesExplodeKill_Lower = t.DoesExplodeKill_Lower;
			Speed_Crawl = t.CrawlSpeed;
			Speed_Sneak = t.SneakSpeed;
			Speed_Walk = t.WalkSpeed;
			Speed_Run = t.RunSpeed;
			Speed_Turning = t.TurnSpeed;
			MovementRotMagnitude = t.MovementRotMagnitude;
			DamMult_Projectile = t.DamMult_Projectile;
			DamMult_Explosive = t.DamMult_Explosive;
			DamMult_Melee = t.DamMult_Melee;
			DamMult_Piercing = t.DamMult_Piercing;
			DamMult_Blunt = t.DamMult_Blunt;
			DamMult_Cutting = t.DamMult_Cutting;
			DamMult_Thermal = t.DamMult_Thermal;
			DamMult_Chilling = t.DamMult_Chilling;
			DamMult_EMP = t.DamMult_EMP;
			CanBeSuppresed = t.CanBeSurpressed;
			SuppressionMult = t.SuppressionMult;
			CanBeGrabbed = t.CanBeGrabbed;
			CanBeSevered = t.CanBeSevered;
			CanBeStabbed = t.CanBeStabbed;
			m_maxJointLimit = t.MaxJointLimit;
			if (t.OverrideSpeech)
			{
				Speech = t.OverrideSpeechSet;
			}
			for (int i = 0; i < Links.Count; i++)
			{
				Links[i].DamMult = t.LinkDamageMultipliers[i];
				Links[i].StaggerMagnitude = t.LinkStaggerMultipliers[i];
				Links[i].SetIntegrity(UnityEngine.Random.Range(t.StartingLinkIntegrity[i].x, t.StartingLinkIntegrity[i].y));
				float num = UnityEngine.Random.Range(0f, 1f);
				if (num < t.StartingChanceBrokenJoint[i])
				{
					Links[i].BreakJoint(isStart: true, Damage.DamageClass.Abstract);
				}
			}
			if (Priority != null)
			{
				m_hasPriority = true;
				m_hasConfiguredPriority = true;
				Priority.Init(E, t.TargetCapacity, t.TargetTrackingTime, t.NoFreshTargetTime);
			}
			UpdateRenderers();
			if (InvulnMaterial != null)
			{
				m_hasInvulnMaterial = true;
			}
			if (InvisMaterial != null)
			{
				m_hasInvisMaterial = true;
			}
			if (VaporizeMaterial != null)
			{
				m_hasVaporizeMaterial = true;
			}
			if (FrozenMaterial != null)
			{
				m_hasFrozenMaterial = true;
			}
		}

		private void Start()
		{
			Init();
			m_tickDownToNextStateSpeech = UnityEngine.Random.Range(5f, 20f);
			m_tickDownToPainSpeechAvailability = UnityEngine.Random.Range(1f, 6f);
			DeParentOnSpawn.SetParent(null);
			if (TestingSosigTemplate != null)
			{
				Configure(TestingSosigTemplate);
			}
			E.UsesFakedPosition = true;
			SetDominantGuardDirection(base.transform.forward);
			if (GM.Options.SimulationOptions.SosigClownMode)
			{
				DamageFX_SmallMustardBurst = FXM.GetClownFX(0);
				DamageFX_LargeMustardBurst = FXM.GetClownFX(1);
				DamageFX_MustardSpoutSmall = FXM.GetClownFX(2);
				DamageFX_MustardSpoutLarge = FXM.GetClownFX(3);
				DamageFX_Explosion = FXM.GetClownFX(4);
				GameObject gameObject = UnityEngine.Object.Instantiate(FXM.GetClownFX(5), Links[0].transform.position, Links[0].transform.rotation);
				gameObject.transform.position = Links[0].transform.position + Links[0].transform.forward * 0.12f - Links[0].transform.up * 0.12f;
				gameObject.transform.SetParent(Links[0].transform);
			}
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null)
				{
					Links[i].O.DistantGrabbable = false;
				}
			}
		}

		public bool IsTickingDownToClear()
		{
			return m_isTickingDownToClear;
		}

		public void TickDownToClear(float f)
		{
			m_isTickingDownToClear = true;
			m_tickDownToClear = Mathf.Min(m_tickDownToClear, f);
		}

		public void DestroyAllHeldObjects()
		{
			if (Inventory.Slots.Count > 0)
			{
				for (int num = Inventory.Slots.Count - 1; num >= 0; num--)
				{
					if (Inventory.Slots[num].HeldObject != null)
					{
						UnityEngine.Object.Destroy(Inventory.Slots[num].HeldObject.gameObject);
					}
				}
			}
			if (Hand_Primary.HeldObject != null)
			{
				UnityEngine.Object.Destroy(Hand_Primary.HeldObject.gameObject);
			}
			if (Hand_Secondary.HeldObject != null)
			{
				UnityEngine.Object.Destroy(Hand_Secondary.HeldObject.gameObject);
			}
		}

		public void KillSosig()
		{
			SosigDies(Damage.DamageClass.Abstract, SosigDeathType.Unknown);
		}

		public void ClearSosig()
		{
			if (DeParentOnSpawn != null)
			{
				UnityEngine.Object.Destroy(DeParentOnSpawn.gameObject);
			}
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null)
				{
					Links[i].LinkExplodes(Damage.DamageClass.Abstract);
				}
			}
			for (int j = 0; j < Hands.Count; j++)
			{
				Hands[j].DropHeldObject();
			}
			Inventory.DropAllObjects();
			ClearCoverPoint();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void DeSpawnSosig()
		{
			ClearCoverPoint();
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null)
				{
					UnityEngine.Object.Destroy(Links[i].gameObject);
				}
				DestroyAllHeldObjects();
				if (this != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void Init()
		{
			for (int i = 0; i < Links.Count; i++)
			{
				IgnoreRBs.Add(Links[i].R);
			}
			for (int j = 0; j < Links.Count; j++)
			{
				if (Links[j].J != null)
				{
					m_joints.Add(Links[j].J);
				}
			}
			if (E != null)
			{
				E.AIEventReceiveEvent += EventReceive;
				E.AIReceiveSuppressionEvent += SuppresionEvent;
			}
			Agent.Warp(base.transform.position);
			Agent.enabled = true;
			m_cachedPath = new NavMeshPath();
			if (Priority != null && !m_hasConfiguredPriority)
			{
				m_hasPriority = true;
				Priority.Init(E, 5, 2f, 1.5f);
			}
			InitHands();
			Inventory.Init();
			if (GM.CurrentAIManager.HasCPM)
			{
				CoverSearchRange = GM.CurrentAIManager.CPM.DefaultSearchRange;
			}
			m_targetPose = Pose_Standing;
			m_targetLocalPos = Pose_Standing.localPosition;
			m_targetLocalRot = Pose_Standing.localRotation;
			m_poseLocalEulers_Standing = Pose_Standing.localEulerAngles;
			m_poseLocalEulers_Crouching = Pose_Crouching.localEulerAngles;
			m_poseLocalEulers_Prone = Pose_Prone.localEulerAngles;
			UpdateJoints(1f);
		}

		private void OnDestroy()
		{
			if (E != null)
			{
				E.AIEventReceiveEvent -= EventReceive;
				E.AIReceiveSuppressionEvent -= SuppresionEvent;
			}
		}

		private bool ShouldContinueTickDown()
		{
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null && Links[i].O.IsHeld)
				{
					return false;
				}
			}
			return true;
		}

		public bool CanCurrentlyBeHeld()
		{
			if (!CanBeGrabbed)
			{
				return false;
			}
			if (m_isStunned)
			{
				return true;
			}
			if (BodyState != 0)
			{
				return true;
			}
			if (CurrentOrder == SosigOrder.Skirmish)
			{
				return false;
			}
			return true;
		}

		private void Update()
		{
			if (m_isTickingDownToClear)
			{
				if (ShouldContinueTickDown())
				{
					m_tickDownToClear -= Time.deltaTime;
				}
				if (m_tickDownToClear <= 0f)
				{
					ClearSosig();
				}
			}
			if (m_timeSinceLastDamage < 10f)
			{
				m_timeSinceLastDamage += Time.deltaTime;
			}
			if (m_isInvuln || m_isDamResist)
			{
				m_suppressionLevel = 0f;
			}
			if (m_suppressionLevel > 0f && BodyState == SosigBodyState.InControl)
			{
				m_suppressionLevel -= Time.deltaTime * 0.25f;
			}
			BrainUpdate();
			SpeechUpdate();
			HeadIconUpdate();
			BodyUpdate();
			BleedingUpdate();
			VaporizeUpdate();
			BuffUpdate();
			HandUpdate();
			LegsUpdate();
			EntityUpdate();
			if (IsDebuggingPriority)
			{
				if (Priority.HasFreshTarget() && DoIHaveAWeaponInMyHand())
				{
					PriorityTesterRay.gameObject.SetActive(value: true);
					PriorityTesterRay.position = base.transform.position + Vector3.up * 1.5f;
					Vector3 forward = Priority.GetTargetPoint() - PriorityTesterRay.position;
					PriorityTesterRay.rotation = Quaternion.LookRotation(forward);
					PriorityTesterRay.localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
				}
				else
				{
					PriorityTesterRay.gameObject.SetActive(value: false);
				}
			}
		}

		private void FixedUpdate()
		{
			SosigPhys();
			HandPhysUpdate();
			Inventory.PhysHold();
		}

		public void EventReceive(AIEvent e)
		{
			if (CurrentOrder == SosigOrder.Disabled || (e.IsEntity && e.Entity.IFFCode == E.IFFCode) || m_isConfused || !m_hasPriority)
			{
				return;
			}
			if (e.Type == AIEvent.AIEType.Visual)
			{
				Priority.ProcessEvent(e);
			}
			else if (e.Type == AIEvent.AIEType.Damage)
			{
				if (!Priority.HasFreshTarget() || !Priority.IsTargetEntity())
				{
					Priority.ProcessEvent(e);
				}
			}
			else if (e.Type == AIEvent.AIEType.Sonic)
			{
				float num = Mathf.Clamp(1f - e.Value, 0f, 1.1f);
				m_alertnessLevel += num;
				if (!Priority.HasFreshTarget() || !Priority.IsTargetEntity())
				{
					Priority.ProcessEvent(e);
				}
			}
		}

		public void SuppresionEvent(Vector3 pos, Vector3 dir, int IFF, float intensity, float range)
		{
			if (CurrentOrder != 0 && HasABrain && CanBeSuppresed && !m_isInvuln && !m_isDamResist && !DoIHaveAShieldInMyHand() && !(m_suppressionLevel >= 1f) && IFF >= 0 && IFF != E.IFFCode)
			{
				float num = Vector3.Distance(pos, base.transform.position);
				num = Mathf.Clamp(num - 1f, 0f, range);
				float num2 = intensity * ((range - num) / range);
				if (num2 > 0f)
				{
					dir.y = 0f;
					dir.z += 0.0001f;
					dir.Normalize();
					m_suppressionDir = Vector3.Lerp(m_suppressionDir, dir, 0.5f);
					m_suppressionDir.Normalize();
					m_suppressionLevel += Mathf.Clamp(num2, 0f, 1f) * SuppressionMult;
				}
			}
		}

		private void SpeechUpdate()
		{
			if (m_tickDownToNextStateSpeech > 0f)
			{
				m_tickDownToNextStateSpeech -= Time.deltaTime;
			}
			else
			{
				SpeechUpdate_State();
			}
			if (m_tickDownToPainSpeechAvailability > 0f)
			{
				m_tickDownToPainSpeechAvailability -= Time.deltaTime;
			}
			if (m_speakingSource != null)
			{
				isSpeaking = true;
				if (!m_speakingSource.Source.isPlaying)
				{
					m_speakingSource = null;
					isSpeaking = false;
				}
			}
			else
			{
				isSpeaking = false;
			}
		}

		private void SpeechUpdate_State()
		{
			if (CanSpeakState())
			{
				switch (CurrentOrder)
				{
				case SosigOrder.GuardPoint:
					Speak_State(Speech.OnWander);
					break;
				case SosigOrder.Wander:
					Speak_State(Speech.OnWander);
					break;
				case SosigOrder.Skirmish:
					break;
				case SosigOrder.SearchForEquipment:
					Speak_State(Speech.OnSearchingForGuns);
					break;
				case SosigOrder.Investigate:
					Speak_State(Speech.OnInvestigate);
					break;
				case SosigOrder.TakeCover:
					Speak_State(Speech.OnTakingCover);
					break;
				case SosigOrder.Assault:
					Speak_State(Speech.OnAssault);
					break;
				}
			}
		}

		private float GetSpeakDelay()
		{
			if (Speech.LessTalkativeSkirmish && CurrentOrder == SosigOrder.Skirmish)
			{
				return UnityEngine.Random.Range(2f, 6f);
			}
			if (CurrentOrder == SosigOrder.Investigate)
			{
				return UnityEngine.Random.Range(3f, 10f);
			}
			return UnityEngine.Random.Range(8f, 45f);
		}

		public void KillSpeech()
		{
			if (m_speakingSource != null)
			{
				m_speakingSource.Source.Stop();
				m_speakingSource = null;
				isSpeaking = false;
			}
		}

		public bool CanSpeakState()
		{
			if (m_tickDownToNextStateSpeech > 0f || !IsAllowedToSpeak || IsConfused || IsStunned || m_recoveringFromBallisticState)
			{
				return false;
			}
			if (isSpeaking)
			{
				return false;
			}
			return true;
		}

		private void Speak_State(List<AudioClip> clips)
		{
			if (BodyState != SosigBodyState.Dead && clips.Count > 0)
			{
				AudioClip audioClip = clips[UnityEngine.Random.Range(0, clips.Count)];
				m_tickDownToNextStateSpeech = audioClip.length + GetSpeakDelay();
				Vector3 position = base.transform.position;
				bool flag = true;
				if (Links[0] != null)
				{
					position = Links[0].transform.position;
					flag = false;
				}
				float num = 1f;
				if (IsFrozen)
				{
					num = 0.8f;
				}
				if (IsSpeedUp)
				{
					num = 1.8f;
				}
				m_speakingSource = GM.CurrentAIManager.Speak(audioClip, Speech.BaseVolume, Speech.BasePitch * num, position, AIManager.SpeakType.chat);
				if (m_speakingSource != null && !flag)
				{
					m_speakingSource.FollowThisTransform(Links[0].transform);
				}
			}
		}

		private void DelayedSpeakPain()
		{
			Speak_Pain(Speech.OnPain);
		}

		private bool CanSpeakPain()
		{
			if (m_tickDownToPainSpeechAvailability > 0f)
			{
				return false;
			}
			if (isSpeaking)
			{
				return false;
			}
			return true;
		}

		private void Speak_Pain(List<AudioClip> clips)
		{
			bool flag = false;
			if (Speech.ForceDeathSpeech && (clips == Speech.OnDeath || clips == Speech.OnDeathAlt))
			{
				flag = true;
			}
			if ((BodyState != SosigBodyState.Dead || flag) && clips.Count > 0 && (CanSpeakPain() || flag))
			{
				if (flag)
				{
					KillSpeech();
				}
				AudioClip audioClip = clips[UnityEngine.Random.Range(0, clips.Count)];
				m_tickDownToPainSpeechAvailability = audioClip.length + UnityEngine.Random.Range(1.1f, 1.2f);
				Vector3 position = base.transform.position;
				bool flag2 = true;
				if (Links[0] != null)
				{
					position = Links[0].transform.position;
					flag2 = false;
				}
				float num = 1f;
				if (IsFrozen)
				{
					num = 0.8f;
				}
				if (IsSpeedUp)
				{
					num = 1.8f;
				}
				if (flag)
				{
					m_speakingSource = GM.CurrentAIManager.Speak(audioClip, Speech.BaseVolume, Speech.BasePitch * num, position, AIManager.SpeakType.death);
				}
				else
				{
					m_speakingSource = GM.CurrentAIManager.Speak(audioClip, Speech.BaseVolume, Speech.BasePitch * num, position, AIManager.SpeakType.pain);
				}
				if (m_speakingSource != null && !flag2)
				{
					m_speakingSource.FollowThisTransform(Links[0].transform);
				}
			}
		}

		private void EntityUpdate()
		{
			if (m_entityJiggleTick > 0f)
			{
				m_entityJiggleTick -= Time.deltaTime;
			}
			else
			{
				m_entityJiggleTick = UnityEngine.Random.Range(0.1f, 1f);
			}
			if (m_linksDestroyed[0])
			{
				E.MaxHearingDistance = 3f;
			}
			if (m_isBlinded || BodyState == SosigBodyState.Dead)
			{
				E.ReceivesEvent_Visual = false;
				E.ReceivesEvent_Sonic = false;
				E.ReceivesEvent_Damage = false;
			}
			else
			{
				E.ReceivesEvent_Visual = true;
				E.ReceivesEvent_Sonic = true;
				E.ReceivesEvent_Damage = true;
			}
			switch (BodyPose)
			{
			case SosigBodyPose.Standing:
				E.VisibilityMultiplier = 1f;
				break;
			case SosigBodyPose.Crouching:
				E.VisibilityMultiplier = 1f;
				break;
			case SosigBodyPose.Prone:
				E.VisibilityMultiplier = 1.5f;
				break;
			}
			if (m_isSuperVisible)
			{
				E.VisibilityMultiplier = 0.3f;
			}
			if (m_isGhosted)
			{
				E.VisibilityMultiplier = 2.5f;
			}
			float num = 0.1f;
			if (!DoIHaveAGun())
			{
				num += 0.03f;
			}
			if (!DoIHaveAWeaponInMyHand())
			{
				num += 0.1f;
			}
			if (m_isHobbled)
			{
				num += 0.1f;
			}
			if (m_isBackBroken)
			{
				num += 0.2f;
			}
			if (m_isBlinded)
			{
				num += 0.3f;
			}
			if (m_isConfused)
			{
				num += 0.2f;
			}
			switch (BodyState)
			{
			case SosigBodyState.Ballistic:
				num += 0.8f;
				break;
			case SosigBodyState.Dead:
				num += 5f;
				break;
			}
			E.DangerMultiplier = num;
		}

		public void HeadIconUpdate()
		{
			if (BodyState == SosigBodyState.Dead)
			{
				SetHeadIcon(SosigHeadIconState.None);
			}
			else if (m_isBlinded)
			{
				SetHeadIcon(SosigHeadIconState.Blinded);
			}
			else if (m_isConfused)
			{
				SetHeadIcon(SosigHeadIconState.Confused);
			}
			else if (m_suppressionLevel > 0.2f)
			{
				SetHeadIcon(SosigHeadIconState.Suppressed);
			}
			else if (CurrentOrder == SosigOrder.Skirmish)
			{
				SetHeadIcon(SosigHeadIconState.Exclamation);
			}
			else if (CurrentOrder == SosigOrder.Investigate)
			{
				SetHeadIcon(SosigHeadIconState.Investigating);
			}
			else
			{
				SetHeadIcon(SosigHeadIconState.None);
			}
			if (!m_linksDestroyed[0] && HeadIconState == SosigHeadIconState.Confused)
			{
				m_rotato += Time.deltaTime * 180f;
				if (m_rotato > 360f)
				{
					m_rotato -= 360f;
				}
				HeadIcons[1].transform.localEulerAngles = new Vector3(0f, m_rotato, 0f);
			}
		}

		public void SetHeadIcon(SosigHeadIconState state)
		{
			if (HeadIconState == state)
			{
				return;
			}
			HeadIconState = state;
			if (m_linksDestroyed[0] || !(Links[0] != null))
			{
				return;
			}
			for (int i = 0; i < HeadIcons.Count; i++)
			{
				if (i == (int)(state - 1))
				{
					if (!HeadIcons[i].activeSelf)
					{
						HeadIcons[i].SetActive(value: true);
					}
				}
				else if (HeadIcons[i].activeSelf)
				{
					HeadIcons[i].SetActive(value: false);
				}
			}
		}

		public void SetGuardInvestigateDistanceThreshold(float d)
		{
			m_guardInvestigateDistanceThreshold = d;
		}

		public Vector3 GetGuardPoint()
		{
			return m_guardPoint;
		}

		public Vector3 GetGuardDir()
		{
			return m_guardDominantDirection;
		}

		public void SetAssaultPointOverrideDistance(float f)
		{
			m_assaultPointOverridesSkirmishPointWhenFurtherThan = f;
		}

		public Vector3 GetAssaultPoint()
		{
			return m_assaultPoint;
		}

		private void BrainUpdate()
		{
			switch (CurrentOrder)
			{
			case SosigOrder.Disabled:
				BrainUpdate_Disabled();
				break;
			case SosigOrder.GuardPoint:
				BrainUpdate_GuardPoint();
				break;
			case SosigOrder.Wander:
				BrainUpdate_Wander();
				break;
			case SosigOrder.SearchForEquipment:
				BrainUpdate_SearchForEquipment();
				break;
			case SosigOrder.Skirmish:
				BrainUpdate_Skirmish();
				break;
			case SosigOrder.Investigate:
				BrainUpdate_Investigate();
				break;
			case SosigOrder.TakeCover:
				BrainUpdate_TakeCover();
				break;
			case SosigOrder.Assault:
				BrainUpdate_Assault();
				break;
			}
			AlertnessUpdate();
		}

		public void SetCurrentOrder(SosigOrder o)
		{
			if (CurrentOrder != o)
			{
				CurrentOrder = o;
				if (o != SosigOrder.Skirmish)
				{
					ClearCoverPoint();
				}
				switch (CurrentOrder)
				{
				case SosigOrder.Disabled:
					break;
				case SosigOrder.GuardPoint:
					break;
				case SosigOrder.Wander:
					m_tickDownToNextStateSpeech = UnityEngine.Random.Range(15f, 25f);
					break;
				case SosigOrder.SearchForEquipment:
					break;
				case SosigOrder.Skirmish:
				{
					m_tickDownToNextStateSpeech = Mathf.Max(m_tickDownToNextStateSpeech, UnityEngine.Random.Range(0.4f, 5f));
					FVRSoundEnvironment environment = SM.GetReverbEnvironment(base.transform.position).Environment;
					float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(environment);
					GM.CurrentSceneSettings.OnPerceiveableSound(30f, 20f * soundTravelDistanceMultByEnvironment, base.transform.position, 0);
					break;
				}
				case SosigOrder.Investigate:
					m_tickDownToNextStateSpeech = UnityEngine.Random.Range(0.1f, 0.25f);
					m_investigateCooldown = UnityEngine.Random.Range(8f, 11f);
					break;
				case SosigOrder.TakeCover:
					break;
				case SosigOrder.Assault:
					break;
				}
			}
		}

		public void CommandAssaultPoint(Vector3 point)
		{
			SetCurrentOrder(SosigOrder.Assault);
			FallbackOrder = SosigOrder.Assault;
			m_assaultPoint = point;
		}

		public void UpdateAssaultPoint(Vector3 point)
		{
			m_assaultPoint = point;
		}

		public void SetAssaultSpeed(SosigMoveSpeed speed)
		{
			m_assaultSpeed = speed;
		}

		public void UpdateGuardPoint(Vector3 point)
		{
			m_guardPoint = point;
		}

		public void CommandGuardPoint(Vector3 point, bool hardguard)
		{
			SetCurrentOrder(SosigOrder.GuardPoint);
			FallbackOrder = SosigOrder.GuardPoint;
			m_guardPoint = point;
			m_hardGuard = hardguard;
		}

		private void AlertnessUpdate()
		{
			if (CurrentOrder == SosigOrder.Assault || CurrentOrder == SosigOrder.Skirmish || FallbackOrder == SosigOrder.Assault || FallbackOrder == SosigOrder.Skirmish)
			{
				m_alertnessLevel = 1f;
			}
			else if (CurrentOrder != SosigOrder.SearchForEquipment && CurrentOrder != SosigOrder.TakeCover)
			{
				if (Priority.HasFreshTarget() && Priority.IsTargetEntity())
				{
					float num = Vector3.Distance(Priority.GetTargetPoint(), base.transform.position);
					float num2 = Mathf.Lerp(2f, 0.2f, num * 0.01f);
					m_alertnessLevel += Time.deltaTime * 1f;
				}
				else if (Priority.HasFreshTarget())
				{
					m_alertnessLevel += Time.deltaTime * 0.01f;
				}
				else
				{
					m_alertnessLevel -= Time.deltaTime * 0.025f;
				}
			}
			m_alertnessLevel = Mathf.Clamp(m_alertnessLevel, 0f, 1.25f);
		}

		private bool StateBailCheck_Equipment()
		{
			if (m_isBlinded)
			{
				return false;
			}
			if (m_searchForEquipmentCoolDown > 0f)
			{
				m_searchForEquipmentCoolDown -= Time.deltaTime;
			}
			else if (!DoIHaveAWeaponAtAll())
			{
				m_searchForEquipmentTickDown = UnityEngine.Random.Range(5f, 10f);
				SetCurrentOrder(SosigOrder.SearchForEquipment);
				return true;
			}
			return false;
		}

		private bool StateBailCheck_ShouldISkirmish()
		{
			if (m_isBlinded)
			{
				return false;
			}
			if (Priority.HasFreshTarget())
			{
				if (Priority.IsTargetEntity())
				{
					float num = 0.5f;
					if (CurrentOrder == SosigOrder.Investigate)
					{
						num = 1f;
					}
					if (m_entityRecognition < 1f)
					{
						m_entityRecognition += Time.deltaTime * num;
					}
					if (m_entityRecognition >= 1f)
					{
						SetCurrentOrder(SosigOrder.Skirmish);
						return true;
					}
				}
				else if (m_entityRecognition > 0f)
				{
					m_entityRecognition -= Time.deltaTime;
				}
				if (CurrentOrder != SosigOrder.Investigate && m_alertnessLevel >= 1f)
				{
					SetCurrentOrder(SosigOrder.Investigate);
					m_investigateCooldown = UnityEngine.Random.Range(8f, 11f);
					return true;
				}
			}
			else if (m_entityRecognition > 0f)
			{
				m_entityRecognition -= Time.deltaTime;
			}
			return false;
		}

		private bool StateBailCheck_ShouldITakeCover()
		{
			if (m_isBlinded)
			{
				return false;
			}
			return false;
		}

		private void WeaponEquipCycle()
		{
			m_weaponSwapCheckTick -= Time.deltaTime;
			if (m_weaponSwapCheckTick <= 0f)
			{
				m_weaponSwapCheckTick = UnityEngine.Random.Range(m_weaponSwapTickRange.x, m_weaponSwapTickRange.y);
				if (EquipBestPrimary())
				{
					EquipSecondaryCycle();
				}
			}
		}

		public void SetDominantGuardDirection(Vector3 v)
		{
			m_guardDominantDirection = v;
		}

		private void RandomLookCycle(float TickDownSpeedMult, float TickRangeMult)
		{
			float num = 1f;
			if (m_isStunned)
			{
				num = 0.1f;
			}
			if (m_isConfused)
			{
				num = 6f;
			}
			m_tickDownToRandomLook -= Time.deltaTime * TickDownSpeedMult * num;
			if (m_tickDownToRandomLook <= 0f)
			{
				m_tickDownToRandomLook = UnityEngine.Random.Range(m_randomLookTickRange.x, m_randomLookTickRange.y) * TickRangeMult;
				m_faceTowards = GetRandomLookDir();
			}
		}

		private void RandomLookCycleDominantDirection(float TickDownSpeedMult, float TickRangeMult)
		{
			float num = 1f;
			if (m_isStunned)
			{
				num = 0.1f;
			}
			if (m_isConfused)
			{
				num = 6f;
			}
			m_tickDownToRandomLook -= Time.deltaTime * TickDownSpeedMult * num;
			if (m_tickDownToRandomLook <= 0f)
			{
				m_tickDownToRandomLook = UnityEngine.Random.Range(m_randomLookTickRange.x, m_randomLookTickRange.y) * TickRangeMult;
				m_faceTowards = Vector3.Lerp(m_guardDominantDirection, GetRandomLookDir(), 0.2f);
				m_faceTowards.y = 0f;
				m_faceTowards.Normalize();
			}
		}

		private void RandomWanderCycle(float TickDownSpeedMult, float TickRangeMult)
		{
			m_tickDownToRandomWanderPoint -= Time.deltaTime * TickDownSpeedMult;
			if (m_tickDownToRandomWanderPoint <= 0f)
			{
				m_tickDownToRandomWanderPoint = UnityEngine.Random.Range(m_randomWanderPointRange.x, m_randomWanderPointRange.y) * TickRangeMult;
				m_wanderPoint = GetNewRandomWanderPoint(Agent.transform.position);
			}
		}

		private void BrainUpdate_Disabled()
		{
			SetBodyPose(SosigBodyPose.Standing);
			SetHandObjectUsage(SosigObjectUsageFocus.MaintainHeldAtRest);
			SetMovementState(SosigMovementState.Idle);
		}

		private void BrainUpdate_GuardPoint()
		{
			if (m_hasPriority)
			{
				Priority.Compute();
			}
			if (!StateBailCheck_ShouldITakeCover() && !StateBailCheck_ShouldISkirmish())
			{
				WeaponEquipCycle();
				EquipmentScanCycle(new Vector3(3f, 3f, 3f), 1f);
				if (Priority.HasFreshTarget())
				{
					m_faceTowards = Priority.GetTargetPoint() - Agent.transform.position;
					m_faceTowards.y = 0.001f;
				}
				else
				{
					RandomLookCycleDominantDirection(1f, 1f);
				}
				TryToGetTo(m_guardPoint);
				SetMovementSpeedBasedOnDistance();
				SetMovementState(SosigMovementState.MoveToPoint);
				SetBodyPose(SosigBodyPose.Standing);
				SetHandObjectUsage(SosigObjectUsageFocus.MaintainHeldAtRest);
			}
		}

		private void BrainUpdate_Wander()
		{
			if (m_hasPriority)
			{
				Priority.Compute();
			}
			if (StateBailCheck_Equipment() || StateBailCheck_ShouldITakeCover() || StateBailCheck_ShouldISkirmish())
			{
				return;
			}
			WeaponEquipCycle();
			EquipmentScanCycle(new Vector3(10f, 3f, 10f), 0.2f);
			SetHandObjectUsage(SosigObjectUsageFocus.MaintainHeldAtRest);
			if (m_hasTargetEquipment && m_targetEquipmentToPickup != null)
			{
				RandomLookCycle(1f, 1f);
				if (m_isConfused || m_isBlinded)
				{
					TryToGetTo(m_wanderPoint);
				}
				else
				{
					TryToGetTo(m_targetEquipmentToPickup.transform.position);
				}
			}
			else
			{
				RandomLookCycle(1f, 0.2f);
				RandomWanderCycle(1f, 1f);
				TryToGetTo(m_wanderPoint);
			}
			SetMovementSpeed(SosigMoveSpeed.Walking);
			SetMovementState(SosigMovementState.MoveToPoint);
			SetBodyPose(SosigBodyPose.Standing);
		}

		private void ClearCoverPoint()
		{
			if (m_curCoverPoint != null)
			{
				m_curCoverPoint.IsClaimed = false;
				m_curCoverPoint = null;
			}
		}

		private void ShouldIPickANewSkirmishPoint(bool hasAGun, bool isReloading)
		{
			if (hasAGun || !DoIHaveAWeaponInMyHand())
			{
				m_tickDownToPickNewSkirmishPoint -= Time.deltaTime;
				if (m_tickDownToPickNewSkirmishPoint <= 0f)
				{
					if (GM.CurrentAIManager.HasCPM)
					{
						m_tickDownToPickNewSkirmishPoint = 0.1f;
					}
					else
					{
						m_tickDownToPickNewSkirmishPoint = UnityEngine.Random.Range(0.1f, 1f);
					}
					if (!AmIReloading() && (!(m_curCoverPoint != null) || !(m_suppressionLevel > 0.5f)))
					{
						m_skirmishPoint = GetNewRandomSkirmishPoint(Agent.transform.position);
					}
				}
			}
			else
			{
				if (hasAGun || !DoIHaveAWeaponInMyHand() || !Priority.HasFreshTarget() || !Priority.IsTargetEntity())
				{
					return;
				}
				Vector3 vector = Priority.GetTargetGroundPoint() + CoreTarget.right * horizOffset;
				Vector3 vector2 = Agent.transform.position - vector;
				vector2.y = 0f;
				SosigWeapon heldMeleeWeapon = GetHeldMeleeWeapon();
				if (heldMeleeWeapon.MeleeState == SosigWeapon.SosigMeleeState.Attacking)
				{
					if (!m_wasAttacking)
					{
						horizOffset = UnityEngine.Random.Range(-0.2f, 0.2f);
						DesiredMeleeDistance = UnityEngine.Random.Range(heldMeleeWeapon.CloseAttackRange.x, heldMeleeWeapon.CloseAttackRange.y);
					}
					m_wasAttacking = true;
				}
				else
				{
					if (m_wasAttacking)
					{
						horizOffset = UnityEngine.Random.Range(-0.8f, 0.8f);
						DesiredMeleeDistance = UnityEngine.Random.Range(heldMeleeWeapon.DistantAttackRange.x, heldMeleeWeapon.DistantAttackRange.x);
					}
					m_wasAttacking = false;
				}
				float desiredMeleeDistance = DesiredMeleeDistance;
				if (NavMesh.Raycast(vector, vector + vector2.normalized * desiredMeleeDistance, out m_navMeshHit, -1))
				{
					m_skirmishPoint = vector + vector2.normalized * m_navMeshHit.distance * 0.8f;
				}
				else
				{
					m_skirmishPoint = vector + vector2.normalized * desiredMeleeDistance;
				}
			}
		}

		private Vector3 GetNewRandomSkirmishPoint(Vector3 startPos)
		{
			Vector2 preferedEngagementDistance = GetPreferedEngagementDistance();
			Vector3 targetPoint = Priority.GetTargetPoint();
			if (GM.CurrentAIManager.HasCPM && DoIHaveAGun())
			{
				Vector3 goalPos = Vector3.zero;
				Vector3 suppressionDir = SuppressionDir;
				bool usesTargetPoint = true;
				bool usesGoalPoint = false;
				if (FallbackOrder == SosigOrder.Assault)
				{
					usesGoalPoint = true;
					goalPos = m_assaultPoint;
				}
				bool usesTakeCoverFromDir = m_suppressionLevel > 0.2f;
				float nextSearchRange = 0f;
				if (GM.CurrentAIManager.CPM.GetBestTacticalPoint(CoverSearchRange, base.transform.position, targetPoint, goalPos, suppressionDir, preferedEngagementDistance, usesTargetPoint, usesGoalPoint, usesTakeCoverFromDir, out var cp, m_curCoverPoint, out nextSearchRange))
				{
					ClearCoverPoint();
					m_curCoverPoint = cp;
					cp.IsClaimed = true;
					return m_curCoverPoint.Pos;
				}
				CoverSearchRange = nextSearchRange;
			}
			Vector3 vector = Agent.transform.position;
			Vector3 vector2 = targetPoint - base.transform.position;
			float magnitude = vector2.magnitude;
			Vector3 vector3 = Vector3.zero;
			if (magnitude < preferedEngagementDistance.x)
			{
				vector3 = vector2.normalized * -2f;
			}
			else if (magnitude > preferedEngagementDistance.y)
			{
				vector3 = vector2.normalized * 2f;
			}
			Vector3 vector4 = UnityEngine.Random.onUnitSphere + vector3;
			vector4.y = 0f;
			float num = UnityEngine.Random.Range(0.5f, 3f);
			if (!DoIHaveAGun() && Priority.HasFreshTarget() && Priority.IsTargetEntity())
			{
				vector = Priority.GetTargetGroundPoint();
				num = UnityEngine.Random.Range(0.7f, 1.1f);
			}
			if (NavMesh.Raycast(vector, vector + vector4.normalized * num, out m_navMeshHit, -1))
			{
				return vector + vector4.normalized * m_navMeshHit.distance * 0.5f;
			}
			return vector + vector4.normalized * num * UnityEngine.Random.Range(0.6f, 1f);
		}

		private void BrainUpdate_Skirmish()
		{
			if (m_hasPriority)
			{
				Priority.Compute();
			}
			if (StateBailCheck_Equipment() || StateBailCheck_ShouldITakeCover())
			{
				return;
			}
			WeaponEquipCycle();
			EquipmentScanCycle(new Vector3(EquipmentPickupDistance, 3f, EquipmentPickupDistance), 1.5f);
			if (!Priority.HasFreshTarget())
			{
				SetCurrentOrder(SosigOrder.Investigate);
				return;
			}
			bool flag = DoIHaveAGun();
			bool flag2 = DoIHaveAWeaponInMyHand();
			bool flag3 = AmIReloading();
			bool flag4 = true;
			Vector3 v = m_skirmishPoint;
			if (FallbackOrder == SosigOrder.Assault)
			{
				float num = Vector3.Distance(Priority.GetTargetPoint(), Agent.transform.position);
				if (num > m_assaultPointOverridesSkirmishPointWhenFurtherThan)
				{
					v = m_assaultPoint;
					flag4 = false;
				}
			}
			if (flag4)
			{
				ShouldIPickANewSkirmishPoint(flag, flag3);
				v = m_skirmishPoint;
			}
			TryToGetTo(v);
			m_faceTowards = Priority.GetTargetPoint() - Agent.transform.position;
			m_faceTowards.y = 0f;
			SetHandObjectUsage(SosigObjectUsageFocus.AttackTarget);
			if (flag)
			{
				if (m_suppressionLevel > 0.2f && !Agent.isOnOffMeshLink)
				{
					if (m_curCoverPoint != null)
					{
						float num2 = Vector3.Distance(m_curCoverPoint.Pos, base.transform.position);
						if (num2 > 2f)
						{
							SetMovementSpeed(SosigMoveSpeed.Running);
						}
						else
						{
							SetMovementSpeed(SosigMoveSpeed.Walking);
						}
					}
					else
					{
						SetMovementSpeed(SosigMoveSpeed.Sneaking);
					}
				}
				else if (flag3)
				{
					SetMovementSpeed(SosigMoveSpeed.Walking);
				}
				else
				{
					SetMovementSpeed(SosigMoveSpeed.Running);
				}
			}
			else
			{
				SetMovementSpeed(SosigMoveSpeed.Running);
			}
			if (CanSpeakState())
			{
				if (flag3 && Speech.OnReloading.Count > 0)
				{
					Speak_State(Speech.OnReloading);
				}
				else if ((IsHealing || IsInvuln) && Speech.OnMedic.Count > 0)
				{
					Speak_State(Speech.OnMedic);
				}
				else
				{
					Speak_State(Speech.OnSkirmish);
				}
			}
			m_timeTilReloadShout -= Time.deltaTime;
			if (m_timeTilReloadShout <= 0f)
			{
				m_timeTilReloadShout = UnityEngine.Random.Range(8f, 20f);
				if (CanSpeakState() && Speech.OnReloading.Count > 0)
				{
					Speak_State(Speech.OnReloading);
				}
			}
			if (HasABrain && !Agent.isOnOffMeshLink && (m_suppressionLevel > 0.2f || !flag2 || flag3))
			{
				if (flag3 || !flag2 || m_suppressionLevel > 0.8f)
				{
					SetBodyPose(SosigBodyPose.Prone);
				}
				else
				{
					SetBodyPose(SosigBodyPose.Crouching);
				}
			}
			else if (Agent.velocity.magnitude > 0.4f)
			{
				SetBodyPose(SosigBodyPose.Standing);
			}
			else
			{
				SetBodyPose(SosigBodyPose.Crouching);
			}
		}

		private void BrainUpdate_Investigate()
		{
			if (m_hasPriority)
			{
				Priority.Compute();
			}
			if (StateBailCheck_Equipment() || StateBailCheck_ShouldITakeCover() || StateBailCheck_ShouldISkirmish())
			{
				return;
			}
			if (!Priority.HasFreshTarget())
			{
				m_investigateCooldown -= Time.deltaTime;
				if (FallbackOrder == SosigOrder.Assault && m_assaultSpeed == SosigMoveSpeed.Running)
				{
					m_investigateCooldown -= Time.deltaTime * 2f;
				}
				if (m_investigateCooldown <= 0f)
				{
					SetCurrentOrder(FallbackOrder);
					return;
				}
			}
			WeaponEquipCycle();
			EquipmentScanCycle(new Vector3(EquipmentPickupDistance, 3f, EquipmentPickupDistance), 0.2f);
			m_investigateNoiseTick -= Time.deltaTime;
			if (m_investigateNoiseTick <= 0f)
			{
				m_investigateNoiseTick = UnityEngine.Random.Range(0.5f, 2f);
				m_investigateNoiseDir = UnityEngine.Random.onUnitSphere;
				m_investigateNoiseDir.y = 0f;
				m_investigateNoiseDir.Normalize();
				m_investigateNoiseDir *= UnityEngine.Random.Range(0.5f, 3f);
			}
			bool flag = true;
			Vector3 targetPoint = Priority.GetTargetPoint();
			Vector3 position = base.transform.position;
			if (FallbackOrder == SosigOrder.GuardPoint)
			{
				position = m_guardPoint;
				float num = Vector3.Distance(targetPoint, position);
				float num2 = m_guardInvestigateDistanceThreshold;
				if (m_hardGuard)
				{
					num2 = m_guardInvestigateDistanceThreshold * 0.5f;
				}
				if (num > num2)
				{
					flag = false;
				}
			}
			if (m_alertnessLevel >= 0.5f)
			{
				if (flag)
				{
					TryToGetTo(targetPoint + m_investigateNoiseDir);
					m_wanderPoint = targetPoint;
				}
				if (DoIHaveAGun())
				{
					SetHandObjectUsage(SosigObjectUsageFocus.AimAtReady);
				}
				else
				{
					SetHandObjectUsage(SosigObjectUsageFocus.MaintainHeldAtRest);
				}
			}
			else
			{
				SetHandObjectUsage(SosigObjectUsageFocus.MaintainHeldAtRest);
			}
			Vector3 faceTowards = targetPoint - Agent.transform.position;
			faceTowards.y = 0f;
			float magnitude = faceTowards.magnitude;
			if (m_suppressionLevel > 0.2f && HasABrain)
			{
				SetMovementSpeed(SosigMoveSpeed.Sneaking);
				SetBodyPose(SosigBodyPose.Prone);
				m_faceTowards = -SuppressionDir;
			}
			else if (magnitude > 20f)
			{
				SetMovementSpeed(SosigMoveSpeed.Running);
				SetBodyPose(SosigBodyPose.Standing);
				m_faceTowards = faceTowards;
			}
			else if (magnitude > 6f)
			{
				SetMovementSpeed(SosigMoveSpeed.Walking);
				SetBodyPose(SosigBodyPose.Crouching);
				m_faceTowards = faceTowards;
			}
			else if (magnitude > 3f)
			{
				SetMovementSpeed(SosigMoveSpeed.Sneaking);
				SetBodyPose(SosigBodyPose.Crouching);
				RandomLookCycle(5f, 0.2f);
			}
			else
			{
				SetMovementSpeed(SosigMoveSpeed.Running);
				SetBodyPose(SosigBodyPose.Crouching);
				m_faceTowards = faceTowards;
			}
		}

		private void BrainUpdate_TakeCover()
		{
			if (BodyState == SosigBodyState.InControl && !m_hasDoveYet)
			{
				m_hasDoveYet = true;
				SetBodyState(SosigBodyState.Ballistic);
				m_recoveryFromBallisticTick = UnityEngine.Random.Range(1f, 1.5f);
				for (int i = 0; i < Links.Count; i++)
				{
					if (!m_linksDestroyed[i] && !m_jointsSevered[i])
					{
						Links[i].R.velocity = m_takeCoverDiveDir * UnityEngine.Random.Range(4f, 6f);
					}
				}
			}
			else if (BodyState == SosigBodyState.InControl && m_hasDoveYet)
			{
				SetCurrentOrder(SosigOrder.Skirmish);
			}
		}

		private void BrainUpdate_SearchForEquipment()
		{
			if (m_hasPriority)
			{
				Priority.Compute();
			}
			if (DoIHaveAWeaponAtAll())
			{
				SetCurrentOrder(FallbackOrder);
				return;
			}
			EquipmentScanCycle(new Vector3(10f, 3f, 10f), 2f);
			SetHandObjectUsage(SosigObjectUsageFocus.MaintainHeldAtRest);
			if (m_targetEquipmentToPickup == null || m_targetEquipmentToPickup.IFFCode != -2)
			{
				m_hasTargetEquipment = false;
			}
			if (m_hasTargetEquipment)
			{
				RandomLookCycle(1f, 1f);
				if (m_isConfused || m_isBlinded)
				{
					TryToGetTo(m_wanderPoint);
				}
				else
				{
					TryToGetTo(m_targetEquipmentToPickup.transform.position);
				}
			}
			else
			{
				RandomLookCycle(3f, 1f);
				RandomWanderCycle(3f, 1f);
				TryToGetTo(m_wanderPoint);
			}
			SetMovementSpeed(SosigMoveSpeed.Running);
			SetMovementState(SosigMovementState.MoveToPoint);
			if (Agent.velocity.magnitude > 0.4f)
			{
				SetBodyPose(SosigBodyPose.Standing);
			}
			else
			{
				SetBodyPose(SosigBodyPose.Crouching);
			}
			WeaponEquipCycle();
			if (!Priority.HasFreshTarget() || !Priority.IsTargetEntity())
			{
				return;
			}
			float num = Vector3.Angle(-Vector3.ProjectOnPlane(m_faceTowards, Vector3.up), Priority.RecentEvents[0].Entity.SensoryFrame.forward);
			if (num < 15f)
			{
				SetCurrentOrder(SosigOrder.TakeCover);
				m_hasDoveYet = false;
				Vector3 vector = Priority.RecentEvents[0].Entity.SensoryFrame.right;
				if (UnityEngine.Random.value > 0.5f)
				{
					vector = -vector;
				}
				vector = Vector3.Slerp(vector, UnityEngine.Random.onUnitSphere, 0.3f);
				vector.y = 0f;
				m_takeCoverDiveDir = vector;
			}
		}

		private void BrainUpdate_Assault()
		{
			if (m_hasPriority)
			{
				Priority.Compute();
			}
			if (StateBailCheck_Equipment() || StateBailCheck_ShouldITakeCover() || StateBailCheck_ShouldISkirmish())
			{
				return;
			}
			WeaponEquipCycle();
			EquipmentScanCycle(new Vector3(EquipmentPickupDistance, 3f, EquipmentPickupDistance), 0.2f);
			SetHandObjectUsage(SosigObjectUsageFocus.AimAtReady);
			Vector3 vector = m_assaultPoint - Agent.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			TryToGetTo(m_assaultPoint);
			m_timeTilAssaultDirChange -= Time.deltaTime;
			if (m_timeTilAssaultDirChange <= 0f)
			{
				if (m_assaultSpeed == SosigMoveSpeed.Crawling || m_assaultSpeed == SosigMoveSpeed.Sneaking)
				{
					m_timeTilAssaultDirChange = UnityEngine.Random.Range(3f, 10f);
					m_assaultCross = UnityEngine.Random.Range(0.8f, -0.8f);
				}
				else if (m_assaultSpeed == SosigMoveSpeed.Walking)
				{
					m_timeTilAssaultDirChange = UnityEngine.Random.Range(2f, 7f);
					m_assaultCross = UnityEngine.Random.Range(0.5f, -0.5f);
				}
				else if (m_assaultSpeed == SosigMoveSpeed.Running)
				{
					m_timeTilAssaultDirChange = UnityEngine.Random.Range(1f, 5f);
					m_assaultCross = UnityEngine.Random.Range(0.2f, -0.2f);
				}
			}
			Vector3 velocity = Agent.velocity;
			if (velocity.magnitude < 0.01f)
			{
				velocity += base.transform.forward;
			}
			Vector3 b = Vector3.Cross(velocity.normalized, Vector3.up);
			if (m_assaultCross < 0f)
			{
				b = Vector3.Cross(velocity.normalized, -Vector3.up);
			}
			Vector3 faceTowards = Vector3.Slerp(b: Vector3.Slerp(velocity, b, Mathf.Abs(m_assaultCross)).normalized, a: m_faceTowards, t: Time.deltaTime);
			faceTowards.y = 0f;
			int assaultSpeed = (int)m_assaultSpeed;
			if (magnitude > 10f)
			{
				SetMovementSpeed((SosigMoveSpeed)Mathf.Min(assaultSpeed, 3));
				SetBodyPose(SosigBodyPose.Standing);
				m_faceTowards = faceTowards;
			}
			else if (magnitude > 2f)
			{
				SetMovementSpeed((SosigMoveSpeed)Mathf.Min(assaultSpeed, 3));
				SetBodyPose(SosigBodyPose.Crouching);
				m_faceTowards = faceTowards;
			}
			else
			{
				SetMovementSpeed((SosigMoveSpeed)Mathf.Min(assaultSpeed, 1));
				SetBodyPose(SosigBodyPose.Crouching);
				RandomLookCycle(2f, 0.2f);
			}
			SetMovementState(SosigMovementState.MoveToPoint);
		}

		private void BrainUpdate_DefendPoint()
		{
		}

		public void InitHands()
		{
			if (!m_hasHandsInit)
			{
				m_hasHandsInit = true;
				float num = UnityEngine.Random.Range(0f, 1f);
				if (num > 0.1f)
				{
					Hand_Primary = Hands[1];
					Hand_Secondary = Hands[0];
				}
				else
				{
					Hand_Primary = Hands[0];
					Hand_Secondary = Hands[1];
				}
				if (Hands[0].Point_ShieldHold == null)
				{
					Hands[0].Point_ShieldHold = Hands[0].Point_Aimed;
				}
				if (Hands[1].Point_ShieldHold == null)
				{
					Hands[1].Point_ShieldHold = Hands[1].Point_Aimed;
				}
				m_searchForWeaponsTick = UnityEngine.Random.Range(m_searchForWeaponsRefire.x, m_searchForWeaponsRefire.y);
			}
		}

		private void SetHandObjectUsage(SosigObjectUsageFocus o)
		{
			if (o != ObjectUsageFocus)
			{
				ObjectUsageFocus = o;
			}
		}

		public bool ForceEquip(SosigWeapon w)
		{
			if (GetEmptyHand(out var h))
			{
				w.transform.position = h.Point_HipFire.position + h.Point_HipFire.forward * 0.3f;
				w.transform.rotation = h.Point_HipFire.rotation;
				h.PickUp(w);
				return true;
			}
			if (Inventory.IsThereAFreeSlot())
			{
				SosigInventory.Slot freeSlot = Inventory.GetFreeSlot();
				freeSlot.PlaceObjectIn(w);
				w.transform.position = freeSlot.Target.transform.position;
				w.transform.rotation = freeSlot.Target.transform.rotation;
				return true;
			}
			return false;
		}

		private void HandUpdate()
		{
			if (m_timeSinceGrenadeThrow < 15f)
			{
				m_timeSinceGrenadeThrow += Time.deltaTime;
			}
			switch (ObjectUsageFocus)
			{
			case SosigObjectUsageFocus.EmptyHands:
				HandUpdate_EmptyHands();
				break;
			case SosigObjectUsageFocus.MaintainHeldAtRest:
				HandUpdate_MaintainHeldAtRest();
				break;
			case SosigObjectUsageFocus.AttackTarget:
				HandUpdate_AttackTarget();
				break;
			case SosigObjectUsageFocus.AimAtReady:
				HandUpdate_AimAtReady();
				break;
			}
			EquipmentPickupCycle();
		}

		private void HandUpdate_EmptyHands()
		{
			if (Hand_Primary.IsHoldingObject)
			{
				Hand_Primary.PutAwayHeldObject();
			}
			if (Hand_Secondary.IsHoldingObject)
			{
				Hand_Secondary.PutAwayHeldObject();
			}
		}

		private void HandUpdate_MaintainHeldAtRest()
		{
			for (int i = 0; i < Hands.Count; i++)
			{
				if (Hands[i].IsHoldingObject && (Hands[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee || Hands[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Shield))
				{
					Hands[i].SetHandPose(SosigHand.SosigHandPose.Melee);
					Hands[i].HeldObject.UseMelee(SosigObjectUsageFocus.MaintainHeldAtRest, CurrentOrder != SosigOrder.Disabled, Hands[i].Target.position);
				}
				else
				{
					Hands[i].SetHandPose(SosigHand.SosigHandPose.AtRest);
				}
			}
		}

		private void HandUpdate_AimAtReady()
		{
			for (int i = 0; i < Hands.Count; i++)
			{
				if (Hands[i].IsHoldingObject && (Hands[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee || Hands[i].HeldObject.Type == SosigWeapon.SosigWeaponType.Shield))
				{
					Hands[i].SetHandPose(SosigHand.SosigHandPose.Melee);
					Hands[i].HeldObject.UseMelee(SosigObjectUsageFocus.AimAtReady, CurrentOrder != SosigOrder.Disabled, Hands[i].Target.position);
				}
				else
				{
					Hands[i].SetHandPose(SosigHand.SosigHandPose.Aimed);
				}
			}
		}

		private void HandUpdate_AttackTarget()
		{
			Vector3 targetPoint = Priority.GetTargetPoint();
			if (m_isBlinded)
			{
				targetPoint += UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 5f) * Time.deltaTime;
			}
			if (Priority.HasFreshTarget())
			{
				for (int i = 0; i < Hands.Count; i++)
				{
					Hands[i].SetAimTowardPoint(targetPoint);
				}
			}
			else
			{
				for (int j = 0; j < Hands.Count; j++)
				{
					Hands[j].ClearAimPoint();
				}
			}
			bool flag = false;
			if (DoIHaveAShieldInMyHand())
			{
				flag = true;
			}
			if (Agent.velocity.magnitude > HipFiringVelocityThreshold || flag)
			{
				for (int k = 0; k < Hands.Count; k++)
				{
					Hands[k].SetHandPose(SosigHand.SosigHandPose.HipFire);
				}
			}
			else
			{
				for (int l = 0; l < Hands.Count; l++)
				{
					Hands[l].SetHandPose(SosigHand.SosigHandPose.Aimed);
				}
			}
			for (int m = 0; m < Hands.Count; m++)
			{
				if (!Hands[m].IsHoldingObject)
				{
					continue;
				}
				if (Hands[m].HeldObject.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					if (Hands[m].HeldObject.UsageState == SosigWeapon.SosigWeaponUsageState.Reloading)
					{
						Hands[m].SetHandPose(SosigHand.SosigHandPose.AtRest);
					}
					float timeTargetSeen = Priority.GetTimeTargetSeen();
					float num = Vector3.Distance(targetPoint, base.transform.position);
					float num2 = Mathf.Lerp(0f, 3f, num / 500f);
					float num3 = Mathf.Lerp(1f, 2f, m_suppressionLevel);
					bool targetPointIdentified = false;
					if (timeTargetSeen >= num2 * num3)
					{
						targetPointIdentified = true;
					}
					float num4 = 1f;
					bool flag2 = false;
					if (Hands[m].Pose == SosigHand.SosigHandPose.HipFire)
					{
						num4 += 0.8f;
						flag2 = true;
					}
					num4 += num3;
					bool isClutching = false;
					if (m_suppressionLevel > 0.7f)
					{
						isClutching = true;
					}
					m_aimWanderTick -= Time.deltaTime;
					if (m_aimWanderTick <= 0f)
					{
						m_aimWanderTick = 0.15f;
						m_aimWanderRandom = UnityEngine.Random.onUnitSphere;
					}
					m_aimWander = Vector3.SmoothDamp(m_aimWander, m_aimWanderRandom, ref m_aimWanderVel, 0.15f);
					float num5 = Mathf.Lerp(1f, 10f, num / 300f);
					float num6 = Mathf.Lerp(0.2f, 0.5f, num / 300f);
					float num7 = Mathf.Lerp(1f, 0f, timeTargetSeen * 0.2f);
					float num8 = 0f;
					if (flag2)
					{
						num8 = 0.6f;
					}
					float num9 = Mathf.Lerp(0f, 0.8f, m_suppressionLevel);
					float num10 = Mathf.Max(num7, num8, num9);
					targetPoint += m_aimWander * num10;
					if (AimTester != null)
					{
						if (aimTester != null)
						{
							aimTester.transform.position = targetPoint;
						}
						else
						{
							GameObject gameObject = UnityEngine.Object.Instantiate(AimTester, targetPoint, Quaternion.identity);
							aimTester = gameObject.transform;
						}
					}
					Hands[m].HeldObject.TryToFireGun(targetPoint, IsPanicFiring(), targetPointIdentified, isClutching, num4, flag2);
				}
				else if (Hands[m].HeldObject.Type == SosigWeapon.SosigWeaponType.Melee)
				{
					Hands[m].SetHandPose(SosigHand.SosigHandPose.Melee);
					Hands[m].HeldObject.UseMelee(SosigObjectUsageFocus.AttackTarget, CurrentOrder != SosigOrder.Disabled, targetPoint);
				}
				else if (Hands[m].HeldObject.Type == SosigWeapon.SosigWeaponType.Shield)
				{
					Hands[m].SetHandPose(SosigHand.SosigHandPose.Melee);
					Hands[m].HeldObject.UseMelee(SosigObjectUsageFocus.AttackTarget, CurrentOrder != SosigOrder.Disabled, targetPoint);
				}
				else if (Hands[m].HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade)
				{
					float timeTargetSeen2 = Priority.GetTimeTargetSeen();
					float num11 = Vector3.Distance(targetPoint, base.transform.position);
					float num12 = Mathf.Lerp(0f, 2f, num11 / 500f);
					float num13 = Mathf.Lerp(1f, 2f, m_suppressionLevel);
					bool flag3 = false;
					if (timeTargetSeen2 >= num12 * num13)
					{
						flag3 = true;
					}
					bool flag4 = false;
					float timeSinceTopTargetSeen = Priority.GetTimeSinceTopTargetSeen();
					if (timeSinceTopTargetSeen < 0.1f)
					{
						flag4 = true;
					}
					bool isReadyToThrow = false;
					if (flag4 && flag3)
					{
						isReadyToThrow = true;
					}
					if (m_timeSinceGrenadeThrow < GrenadeThrowLag)
					{
						isReadyToThrow = false;
					}
					if (flag3)
					{
						Hands[m].SetHandPose(SosigHand.SosigHandPose.Aimed);
						Hands[m].HeldObject.TryToThrowAt(targetPoint, isReadyToThrow);
					}
					else
					{
						Hands[m].SetHandPose(SosigHand.SosigHandPose.Aimed);
					}
				}
			}
		}

		public bool IsPanicFiring()
		{
			if (!HasABrain)
			{
				return true;
			}
			if (m_isConfused)
			{
				return true;
			}
			if (m_isBlinded && m_timeSinceLastDamage < 2f)
			{
				return true;
			}
			return false;
		}

		private void EquipmentPickupCycle()
		{
			if (m_hasTargetEquipment && m_targetEquipmentToPickup != null && m_targetEquipmentToPickup.IFFCode != -2)
			{
				m_hasTargetEquipment = false;
				m_targetEquipmentToPickup = null;
			}
			if (m_isStunned || m_isBlinded)
			{
				return;
			}
			if (m_targetEquipmentToPickup == null)
			{
				m_hasTargetEquipment = false;
			}
			if (m_hasTargetEquipment)
			{
				float num = Vector3.Distance(m_targetEquipmentToPickup.transform.position, Agent.transform.position);
				if (num < EquipmentPickupDistance && PickUpEquipment(m_targetEquipmentToPickup))
				{
					m_targetEquipmentToPickup = null;
					m_hasTargetEquipment = false;
				}
			}
		}

		private void EquipmentScanCycle(Vector3 extents, float tickDownSpeed)
		{
			if (AreMyHandsFull() && IsMyInventoryFull())
			{
				m_hasTargetEquipment = false;
				m_targetEquipmentToPickup = null;
				return;
			}
			if (m_searchForWeaponsTick > 0f)
			{
				m_searchForWeaponsTick -= Time.deltaTime * tickDownSpeed;
				return;
			}
			m_searchForWeaponsTick = UnityEngine.Random.Range(m_searchForWeaponsRefire.x, m_searchForWeaponsRefire.y);
			if (m_isBlinded)
			{
				return;
			}
			Vector3 center = Agent.transform.position + Vector3.up * 1f;
			Collider[] array = Physics.OverlapBox(center, extents * SearchExtentsModifier, Quaternion.identity, LM_SearchForWeapons, QueryTriggerInteraction.Collide);
			float num = 20f;
			AIEntity targetEquipmentToPickup = null;
			bool flag = false;
			SosigWeapon sosigWeapon = null;
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					AIEntity component = array[i].GetComponent<AIEntity>();
					if (component.IFFCode != -2)
					{
						continue;
					}
					float magnitude = (component.GetPos() - Agent.transform.position).magnitude;
					if (magnitude < num)
					{
						SosigWeapon component2 = component.FacingTransform.GetComponent<SosigWeapon>();
						if (Inventory.DoINeed(component2) && (component2.IsUsable() || Inventory.HasAmmoFor(component2)) && (component2.Type != 0 || CanPickup_Ranged) && (component2.Type != SosigWeapon.SosigWeaponType.Melee || CanPickup_Melee) && (component2.Type == SosigWeapon.SosigWeaponType.Gun || component2.Type == SosigWeapon.SosigWeaponType.Melee || CanPickup_Other) && (!flag || sosigWeapon.Type != 0 || (component2.Type != SosigWeapon.SosigWeaponType.Melee && component2.Type != SosigWeapon.SosigWeaponType.Grenade)))
						{
							num = magnitude;
							targetEquipmentToPickup = component;
							flag = true;
							sosigWeapon = component2;
						}
					}
				}
			}
			if (flag)
			{
				m_hasTargetEquipment = true;
				m_targetEquipmentToPickup = targetEquipmentToPickup;
			}
		}

		private bool PickUpEquipment(AIEntity e)
		{
			if (GetEmptyHand(out var h))
			{
				h.PickUp(e.FacingTransform.GetComponent<SosigWeapon>());
				return true;
			}
			if (Inventory.IsThereAFreeSlot() && Inventory.PutObjectInMe(e.FacingTransform.GetComponent<SosigWeapon>()))
			{
				return true;
			}
			return false;
		}

		private bool EquipBestPrimary()
		{
			int bestItemQuality = Inventory.GetBestItemQuality(SosigWeapon.SosigWeaponType.Gun);
			if (Hand_Primary.IsHoldingObject)
			{
				if (Hand_Primary.HeldObject.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					int num = -1;
					num = Hand_Primary.HeldObject.Quality;
					if (bestItemQuality > 0 && bestItemQuality > num)
					{
						Inventory.SwapObjectFromHandToObjectInInventory(Hand_Primary.HeldObject, Inventory.GetBestGunOut());
						return false;
					}
				}
				else if (Inventory.IsThereAFreeSlot() && Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Gun))
				{
					SosigWeapon heldObject = Hand_Primary.HeldObject;
					Hand_Primary.DropHeldObject();
					Inventory.PutObjectInMe(heldObject);
					return false;
				}
			}
			else
			{
				if (Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Gun))
				{
					SosigWeapon bestGunOut = Inventory.GetBestGunOut();
					bestGunOut.InventorySlotWithThis.I.DropObjectInSlot(bestGunOut.InventorySlotWithThis);
					Hand_Primary.PickUp(bestGunOut);
					return false;
				}
				if (Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Melee))
				{
					SosigWeapon bestMeleeWeaponOut = Inventory.GetBestMeleeWeaponOut();
					bestMeleeWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestMeleeWeaponOut.InventorySlotWithThis);
					Hand_Primary.PickUp(bestMeleeWeaponOut);
					return false;
				}
			}
			return true;
		}

		private void EquipSecondaryCycle()
		{
			if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.Type == SosigWeapon.SosigWeaponType.Gun && Inventory.IsThereAFreeSlot())
			{
				SosigWeapon heldObject = Hand_Secondary.HeldObject;
				Hand_Secondary.DropHeldObject();
				Inventory.PutObjectInMe(heldObject);
				return;
			}
			if (!Hand_Secondary.IsHoldingObject && Priority.HasFreshTarget() && Priority.IsTargetEntity() && Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Grenade))
			{
				SosigWeapon bestWeaponOut = Inventory.GetBestWeaponOut(SosigWeapon.SosigWeaponType.Grenade);
				float distanceToTarget = Priority.GetDistanceToTarget(base.transform);
				if (distanceToTarget > bestWeaponOut.PreferredRange.x && distanceToTarget < bestWeaponOut.PreferredRange.y && bestWeaponOut != null && bestWeaponOut.InventorySlotWithThis != null)
				{
					bestWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestWeaponOut.InventorySlotWithThis);
					Hand_Secondary.PickUp(bestWeaponOut);
					return;
				}
			}
			if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade)
			{
				bool flag = false;
				if (!Priority.HasFreshTarget() || !Priority.IsTargetEntity())
				{
					flag = true;
				}
				else
				{
					float distanceToTarget2 = Priority.GetDistanceToTarget(base.transform);
					if (distanceToTarget2 <= Hand_Secondary.HeldObject.PreferredRange.x || distanceToTarget2 >= Hand_Secondary.HeldObject.PreferredRange.y)
					{
						flag = true;
					}
				}
				if (flag)
				{
					SosigWeapon heldObject2 = Hand_Secondary.HeldObject;
					Hand_Secondary.DropHeldObject();
					Inventory.PutObjectInMe(heldObject2);
				}
			}
			if (!Hand_Primary.IsHoldingObject && Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Melee) && !Hand_Secondary.IsHoldingObject)
			{
				SosigWeapon bestMeleeWeaponOut = Inventory.GetBestMeleeWeaponOut();
				if (bestMeleeWeaponOut != null && bestMeleeWeaponOut.InventorySlotWithThis != null)
				{
					bestMeleeWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestMeleeWeaponOut.InventorySlotWithThis);
					Hand_Secondary.PickUp(bestMeleeWeaponOut);
					return;
				}
			}
			if (Hand_Primary.IsHoldingObject && Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Shield) && !Hand_Secondary.IsHoldingObject)
			{
				SosigWeapon bestShieldWeaponOut = Inventory.GetBestShieldWeaponOut();
				if (bestShieldWeaponOut != null && bestShieldWeaponOut.InventorySlotWithThis != null)
				{
					bestShieldWeaponOut.InventorySlotWithThis.I.DropObjectInSlot(bestShieldWeaponOut.InventorySlotWithThis);
					Hand_Secondary.PickUp(bestShieldWeaponOut);
					return;
				}
			}
			if (!Hand_Primary.IsHoldingObject)
			{
				return;
			}
			float num = 100f;
			if (Priority.HasFreshTarget() && Priority.IsTargetEntity())
			{
				num = Vector3.Distance(base.transform.position, Priority.GetTargetPoint());
			}
			if (num < 5f || Hand_Primary.HeldObject.Type == SosigWeapon.SosigWeaponType.Melee)
			{
				if (!Hand_Secondary.IsHoldingObject && Inventory.DoIHaveAnObjectOfType(SosigWeapon.SosigWeaponType.Melee))
				{
					SosigWeapon bestMeleeWeaponOut2 = Inventory.GetBestMeleeWeaponOut();
					if (bestMeleeWeaponOut2 != null && bestMeleeWeaponOut2.InventorySlotWithThis != null)
					{
						bestMeleeWeaponOut2.InventorySlotWithThis.I.DropObjectInSlot(bestMeleeWeaponOut2.InventorySlotWithThis);
						Hand_Secondary.PickUp(bestMeleeWeaponOut2);
					}
				}
			}
			else if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.Type == SosigWeapon.SosigWeaponType.Melee && Inventory.IsThereAFreeSlot())
			{
				SosigWeapon heldObject3 = Hand_Secondary.HeldObject;
				Hand_Secondary.DropHeldObject();
				Inventory.PutObjectInMe(heldObject3);
			}
		}

		private bool GetEmptyHand(out SosigHand h)
		{
			if (!Hand_Primary.IsHoldingObject)
			{
				h = Hand_Primary;
				return true;
			}
			if (!Hand_Secondary.IsHoldingObject)
			{
				h = Hand_Secondary;
				return true;
			}
			h = null;
			return false;
		}

		private bool DoIHaveAWeaponAtAll()
		{
			if (DoIHaveAWeaponInMyHand())
			{
				return true;
			}
			if (DoIHaveAWeaponInMyInventory())
			{
				return true;
			}
			return false;
		}

		private bool DoIHaveAWeaponInMyHand()
		{
			if (Hand_Primary.IsHoldingObject && (Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun || Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee))
			{
				return true;
			}
			if (Hand_Secondary.IsHoldingObject && (Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun || Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee))
			{
				return true;
			}
			return false;
		}

		private bool DoIHaveAShieldInMyHand()
		{
			if (Hand_Primary.IsHoldingObject && Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Shield)
			{
				return true;
			}
			if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Shield)
			{
				return true;
			}
			return false;
		}

		private bool DoIHaveAWeaponInMyInventory()
		{
			return Inventory.DoIHaveAnyWeaponry();
		}

		private SosigWeapon GetHeldMeleeWeapon()
		{
			if (Hand_Primary.IsHoldingObject && Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee)
			{
				return Hand_Primary.HeldObject;
			}
			if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Melee)
			{
				return Hand_Secondary.HeldObject;
			}
			return null;
		}

		public bool DoIHaveAGun()
		{
			if (Hand_Primary.IsHoldingObject && Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun)
			{
				return true;
			}
			if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun)
			{
				return true;
			}
			return false;
		}

		private Vector2 GetPreferedEngagementDistance()
		{
			Vector2 result = new Vector2(0.1f, 10f);
			if (DoIHaveAGun())
			{
				if (Hand_Primary.IsHoldingObject && Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun)
				{
					return Hand_Primary.HeldObject.PreferredRange;
				}
				if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun)
				{
					return Hand_Secondary.HeldObject.PreferredRange;
				}
			}
			return result;
		}

		private bool AmIReloading()
		{
			if (DoIHaveAGun())
			{
				if (Hand_Primary.IsHoldingObject && Hand_Primary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun && Hand_Primary.HeldObject.UsageState == SosigWeapon.SosigWeaponUsageState.Reloading)
				{
					return true;
				}
				if (Hand_Secondary.IsHoldingObject && Hand_Secondary.HeldObject.GetMyType() == SosigWeapon.SosigWeaponType.Gun && Hand_Secondary.HeldObject.UsageState == SosigWeapon.SosigWeaponUsageState.Reloading)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private bool AreMyHandsFull()
		{
			if (Hand_Primary.IsHoldingObject && Hand_Secondary.IsHoldingObject)
			{
				return true;
			}
			return false;
		}

		private bool IsMyInventoryFull()
		{
			if (Inventory.Slots.Count == 0)
			{
				return true;
			}
			if (Inventory.IsThereAFreeSlot())
			{
				return false;
			}
			return true;
		}

		private int GetBestItemQualityBeingHeld()
		{
			int num = -1;
			if (Hand_Primary.IsHoldingObject)
			{
				num = Mathf.Max(num, Hand_Primary.HeldObject.Quality);
			}
			if (Hand_Secondary.IsHoldingObject)
			{
				num = Mathf.Max(num, Hand_Secondary.HeldObject.Quality);
			}
			return num;
		}

		private void HandPhysUpdate()
		{
			for (int i = 0; i < Hands.Count; i++)
			{
				Hands[i].Hold();
			}
		}

		private void SetBodyPose(SosigBodyPose p)
		{
			if (p != BodyPose)
			{
				if (DoIHaveAShieldInMyHand() && p == SosigBodyPose.Prone)
				{
					p = SosigBodyPose.Crouching;
				}
				if (m_isHobbled && p == SosigBodyPose.Standing)
				{
					p = SosigBodyPose.Crouching;
				}
				if (m_isBackBroken && (p == SosigBodyPose.Standing || p == SosigBodyPose.Crouching))
				{
					p = SosigBodyPose.Prone;
				}
				BodyPose = p;
				switch (BodyPose)
				{
				case SosigBodyPose.Standing:
					m_targetPose = Pose_Standing;
					break;
				case SosigBodyPose.Crouching:
					m_targetPose = Pose_Crouching;
					break;
				case SosigBodyPose.Prone:
					m_targetPose = Pose_Prone;
					break;
				}
			}
		}

		private void SetMovementState(SosigMovementState s)
		{
			if (s != MovementState)
			{
				MovementState = s;
			}
		}

		private void SetMovementSpeed(SosigMoveSpeed m)
		{
			MoveSpeed = m;
		}

		private void SetMovementSpeedBasedOnDistance()
		{
			float num = Vector3.Distance(m_navToPoint, Agent.transform.position);
			if (num > 20f)
			{
				MoveSpeed = SosigMoveSpeed.Running;
			}
			else
			{
				MoveSpeed = SosigMoveSpeed.Walking;
			}
		}

		private void LegsUpdate()
		{
			switch (MovementState)
			{
			case SosigMovementState.Idle:
				LegsUpdate_Idle();
				break;
			case SosigMovementState.HoldFast:
				LegsUpdate_HoldFast();
				break;
			case SosigMovementState.MoveToPoint:
				LegsUpdate_MoveToPoint();
				break;
			case SosigMovementState.DiveToPoint:
				LegsUpdate_DiveToPoint();
				break;
			}
		}

		private void LegsUpdate_Idle()
		{
		}

		private void LegsUpdate_HoldFast()
		{
			if (Agent.enabled)
			{
				TurnTowardsFacingDir();
			}
		}

		private void InitiateLink(NavMeshLinkExtension ex)
		{
			linkExtensions = ex;
			extensionSpeed = Agent.speed;
			targetSpeed = ex.GetXYSpeed();
		}

		private void EndLink()
		{
			extensionSpeed = 0f;
			targetSpeed = 0f;
			m_isOnOffMeshLink = false;
			linkExtensions = null;
		}

		private void LegsUpdate_MoveToPoint()
		{
			if (!Agent.enabled)
			{
				return;
			}
			Agent.speed = GetLinearSpeed(Agent.velocity);
			Agent.autoTraverseOffMeshLink = true;
			if (Agent.isOnOffMeshLink)
			{
				if (!m_isOnOffMeshLink)
				{
					NavMeshLinkExtension component = Agent.currentOffMeshLinkData.offMeshLink.gameObject.GetComponent<NavMeshLinkExtension>();
					if (component != null)
					{
						m_isOnOffMeshLink = true;
						InitiateLink(component);
					}
				}
			}
			else if (!Agent.isOnOffMeshLink && m_isOnOffMeshLink)
			{
				EndLink();
			}
			if (linkExtensions != null)
			{
				if (Agent.currentOffMeshLinkData.endPos.y < Agent.transform.position.y)
				{
					targetSpeed += Time.deltaTime * 5f;
					targetSpeed = Mathf.Clamp(targetSpeed, 0f, 10f);
				}
				extensionSpeed = Mathf.MoveTowards(extensionSpeed, targetSpeed, Time.deltaTime * 5f);
				Agent.speed = extensionSpeed;
			}
			TurnTowardsFacingDir();
		}

		private void LegsUpdate_DiveToPoint()
		{
		}

		private void TurnTowardsFacingDir()
		{
			Vector3 normalized = m_faceTowards.normalized;
			Vector3 forward = Vector3.RotateTowards(Agent.transform.forward, normalized, GetAngularSpeed() * Time.deltaTime, 1f);
			forward.y = 0f;
			if ((forward.x == 0f && forward.z == 0f) || forward.y == 1f)
			{
				forward.z = 1E-06f;
			}
			forward.z += 0.001f;
			Agent.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		}

		private void TryToGetTo(Vector3 v)
		{
			if (!Agent.enabled)
			{
				return;
			}
			float num = Vector3.Distance(v, lastDest);
			m_navToPoint = v;
			debug_haspath = Agent.hasPath;
			if (debug_haspath)
			{
				float num2 = Vector3.Distance(m_navToPoint, Agent.transform.position);
				if (num2 < 0.01f)
				{
					Agent.ResetPath();
				}
			}
			debug_pathpending = Agent.pathPending;
			if (!debug_pathpending && num > 0.05f)
			{
				if (NavMesh.SamplePosition(v, out var hit, 1f, -1))
				{
					v = hit.position;
				}
				bool flag = Agent.SetDestination(v);
				lastDest = v;
			}
			Vector3 velocity = Agent.velocity;
			Vector3 vector = base.transform.InverseTransformDirection(velocity);
			Vector3 b = new Vector3(Mathf.Clamp(0f - vector.z, -3f, 3f), 0f, Mathf.Clamp(vector.x, -3f, 3f)) * MovementRotMagnitude;
			curEuluer = Vector3.Lerp(curEuluer, b, Time.deltaTime * 1f);
			Pose_Standing.localEulerAngles = m_poseLocalEulers_Standing + curEuluer;
			Pose_Crouching.localEulerAngles = m_poseLocalEulers_Crouching + curEuluer;
			Pose_Prone.localEulerAngles = m_poseLocalEulers_Prone + curEuluer;
		}

		private float GetLinearSpeed(Vector3 moveDir)
		{
			if (m_isChillOut)
			{
				return 0f;
			}
			if (moveDir == Vector3.zero)
			{
				moveDir = Vector3.up;
			}
			float a = Speed_Walk;
			SosigMoveSpeed sosigMoveSpeed = MoveSpeed;
			if (m_isBackBroken)
			{
				sosigMoveSpeed = SosigMoveSpeed.Crawling;
			}
			else if (m_isHobbled)
			{
				sosigMoveSpeed = SosigMoveSpeed.Crawling;
			}
			else if (m_isStunned)
			{
				sosigMoveSpeed = SosigMoveSpeed.Sneaking;
			}
			if (m_isFrozen)
			{
				sosigMoveSpeed = SosigMoveSpeed.Crawling;
			}
			switch (sosigMoveSpeed)
			{
			case SosigMoveSpeed.Crawling:
				a = Speed_Crawl;
				break;
			case SosigMoveSpeed.Sneaking:
				a = Speed_Sneak;
				break;
			case SosigMoveSpeed.Walking:
				a = Speed_Walk;
				break;
			case SosigMoveSpeed.Running:
				a = Speed_Run;
				break;
			}
			if (m_isFrozen)
			{
				a = Mathf.Min(Speed_Crawl, 0.5f);
			}
			if (m_isSpeedup)
			{
				a = Speed_Run * 2f;
			}
			float speed_Sneak = Speed_Sneak;
			float num = Vector3.Angle(Agent.transform.forward, moveDir);
			return Mathf.Lerp(a, speed_Sneak, num / 180f);
		}

		private float GetAngularSpeed()
		{
			if (m_isFrozen)
			{
				return Speed_Turning * 0.15f;
			}
			if (m_isSpeedup)
			{
				return Speed_Turning * 2f;
			}
			if (m_isStunned)
			{
				return Speed_Turning * 0.25f;
			}
			return Speed_Turning;
		}

		private Vector3 GetRandomLookDir()
		{
			Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
			onUnitSphere.y = 0f;
			return onUnitSphere.normalized;
		}

		private Vector3 GetNewRandomWanderPoint(Vector3 startPos)
		{
			Vector3 position = Agent.transform.position;
			Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
			onUnitSphere.y = 0f;
			float num = UnityEngine.Random.Range(6f, 10f);
			if (NavMesh.Raycast(position, position + onUnitSphere.normalized * num, out m_navMeshHit, -1))
			{
				return position + onUnitSphere.normalized * m_navMeshHit.distance * 0.5f;
			}
			return position + onUnitSphere.normalized * num * UnityEngine.Random.Range(0.6f, 1f);
		}

		private void BodyUpdate()
		{
			if (BodyState != 0)
			{
				ClearCoverPoint();
			}
			if (BodyState != SosigBodyState.Dead)
			{
				if (m_isStunned)
				{
					m_stunTimeLeft = Mathf.Clamp(m_stunTimeLeft, -1f, m_maxStunTime);
					m_stunTimeLeft -= Time.deltaTime;
					if (m_stunTimeLeft <= 0f)
					{
						m_isStunned = false;
					}
				}
				if (m_isConfused)
				{
					m_confusedTime = Mathf.Clamp(m_confusedTime, -1f, m_maxConfusedTime);
					m_confusedTime -= Time.deltaTime;
					if (m_confusedTime <= 0f)
					{
						m_isConfused = false;
					}
				}
				if (m_isBlinded)
				{
					m_blindTime = Mathf.Clamp(m_blindTime, -1f, m_maxBlindTime);
					m_blindTime -= Time.deltaTime;
					if (m_blindTime <= 0f)
					{
						m_isBlinded = false;
					}
				}
				m_linkIndex++;
				if (m_linkIndex >= Links.Count)
				{
					m_linkIndex = 0;
				}
				if (Links[m_linkIndex] != null)
				{
					fakeEntityPos = Links[m_linkIndex].transform.position + UnityEngine.Random.onUnitSphere * 0.2f + Links[m_linkIndex].transform.up * 0.25f;
				}
				E.FakePos = fakeEntityPos;
			}
			if (!m_isHobbled)
			{
				for (int i = 2; i < Links.Count; i++)
				{
					if (!m_linksDestroyed[i] && Links[i].GetIntegrityRatio() < 0.25f)
					{
						m_isHobbled = true;
						break;
					}
				}
			}
			if (BodyState == SosigBodyState.Ballistic)
			{
				m_recoveryFromBallisticTick = Mathf.Clamp(m_recoveryFromBallisticTick, -1f, 4f);
				m_recoveryFromBallisticTick -= Time.deltaTime;
				if (m_recoveryFromBallisticTick <= 0f)
				{
					AttemptToRecoverFromBallistic();
					return;
				}
				m_tickDownToWrithe -= Time.deltaTime;
				if (m_tickDownToWrithe <= 0f)
				{
					m_tickDownToWrithe = UnityEngine.Random.Range(m_writheTickRange.x, m_writheTickRange.y);
					Writhe();
				}
			}
			else
			{
				if (BodyState != 0)
				{
					return;
				}
				if (m_isCountingDownToStagger)
				{
					if (m_staggerAmountToApply > 1f)
					{
						Stagger(m_staggerAmountToApply);
						m_isCountingDownToStagger = false;
					}
					else
					{
						m_tickDownToStagger -= Time.deltaTime * 2f;
						if (m_tickDownToStagger <= 0f)
						{
							Stagger(m_staggerAmountToApply);
							m_isCountingDownToStagger = false;
						}
					}
				}
				float num = 4f;
				if (IsFrozen)
				{
					num = 0.25f;
				}
				if (IsSpeedUp)
				{
					num = 8f;
				}
				m_targetLocalPos = Vector3.Lerp(m_targetLocalPos, m_targetPose.localPosition, Time.deltaTime * num);
				m_targetLocalRot = Quaternion.Slerp(m_targetLocalRot, m_targetPose.localRotation, Time.deltaTime * num);
				if (m_recoveringFromBallisticState)
				{
					CoreTarget.position = Vector3.Lerp(m_recoveryFromBallisticStartPos, m_targetPose.position, m_recoveryFromBallisticLerp * num * 0.5f);
					CoreTarget.rotation = Quaternion.Slerp(m_recoveryFromBallisticStartRot, m_targetPose.rotation, m_recoveryFromBallisticLerp * num * 0.5f);
					UpdateJoints(m_recoveryFromBallisticLerp);
					if (m_recoveryFromBallisticLerp >= 1f)
					{
						m_recoveringFromBallisticState = false;
					}
					m_recoveryFromBallisticLerp += Time.deltaTime * 0.5f;
					return;
				}
				float num2 = Agent.velocity.magnitude - 0.05f;
				m_bobTick = Mathf.Repeat(m_bobTick += Time.deltaTime * BobSpeedMult, 1f);
				float num3 = Mathf.Clamp(num2 * 0.5f, 0f, 1f);
				float num4 = Mathf.Clamp(num2 * 0.5f, 0f, 2f);
				float num5 = BodyBobCurve_Vertical.Evaluate(m_bobTick * num4 * 2f);
				if (num5 > 0f)
				{
					m_hasFootStepDown = false;
				}
				if (num5 <= -0.8f && !m_hasFootStepDown)
				{
					float num6 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
					float delay = num6 / 343f;
					m_hasFootStepDown = true;
					if (num6 < 10f)
					{
						SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.GenericClose, AudEvent_FootSteps, base.transform.position, new Vector2(num3 * 0.35f, num3 * 0.4f), new Vector2(0.95f, 1.05f), delay);
					}
				}
				Vector3 vector = base.transform.TransformPoint(m_targetLocalPos);
				CoreTarget.position = vector + m_targetPose.up * num5 * MaxBobIntensityVertical * num3 + m_targetPose.right * BodyBobCurve_Horizontal.Evaluate(m_bobTick * num4) * MaxBobIntensityHorizontal * num3;
				CoreTarget.localRotation = m_targetLocalRot;
			}
		}

		private void AttemptToRecoverFromBallistic()
		{
			m_ballisticRecoveryAttemptTick -= Time.deltaTime;
			if (!(m_ballisticRecoveryAttemptTick < 0f))
			{
				return;
			}
			m_recoveryFromBallisticLerp = 0f;
			if (CoreRB == null)
			{
				SetBodyState(SosigBodyState.Dead);
				return;
			}
			m_recoveryFromBallisticStartPos = CoreRB.transform.position;
			m_recoveryFromBallisticStartRot = CoreRB.transform.rotation;
			m_recoveringFromBallisticState = true;
			Agent.enabled = true;
			if (Agent.Warp(CoreRB.transform.position))
			{
				m_ballisticRecoveryAttemptTick = 0f;
				SetBodyState(SosigBodyState.InControl);
				m_wanderPoint = CoreRB.transform.position;
				m_skirmishPoint = CoreRB.transform.position;
			}
			else
			{
				Agent.enabled = false;
				m_ballisticRecoveryAttemptTick = UnityEngine.Random.Range(m_ballsticRecoveryAttemptRange.x, m_ballsticRecoveryAttemptRange.y);
			}
		}

		public void SpawnLargeMustardBurst(Vector3 point, Vector3 dir)
		{
			UnityEngine.Object.Instantiate(DamageFX_LargeMustardBurst, point, Quaternion.LookRotation(dir));
		}

		private void ActivateBuff(int i, bool isInverted)
		{
			if (!PUM.HasEffectBot(i, isInverted))
			{
				return;
			}
			if (!isInverted)
			{
				if (i < BuffSystems.Length && BuffSystems[i] == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(PUM.GetEffect(i, isInverted), Links[0].transform.position, Links[0].transform.rotation);
					gameObject.transform.SetParent(Links[0].transform);
					BuffSystems[i] = gameObject;
				}
			}
			else if (i < DeBuffSystems.Length && DeBuffSystems[i] == null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(PUM.GetEffect(i, isInverted), Links[0].transform.position, Links[0].transform.rotation);
				gameObject2.transform.SetParent(Links[0].transform);
				DeBuffSystems[i] = gameObject2;
			}
		}

		private void DeActivateBuff(int i)
		{
			if (BuffSystems.Length > i && BuffSystems[i] != null)
			{
				UnityEngine.Object.Destroy(BuffSystems[i]);
				BuffSystems[i] = null;
			}
		}

		private void DeActivateDeBuff(int i)
		{
			if (DeBuffSystems.Length > i && DeBuffSystems[i] != null)
			{
				UnityEngine.Object.Destroy(DeBuffSystems[i]);
				DeBuffSystems[i] = null;
			}
		}

		private void DeActivateAllBuffSystems()
		{
			for (int i = 0; i < 13; i++)
			{
				DeActivateBuff(i);
				DeActivateDeBuff(i);
			}
		}

		public void ActivatePower(PowerupType type, PowerUpIntensity intensity, PowerUpDuration d, bool isPuke, bool isInverted)
		{
			if (BodyState == SosigBodyState.Dead || (IsDebuff && type != PowerupType.Debuff && !isInverted))
			{
				return;
			}
			float num = 1f;
			switch (d)
			{
			case PowerUpDuration.Full:
				num = 30f;
				break;
			case PowerUpDuration.SuperLong:
				num = 40f;
				break;
			case PowerUpDuration.Short:
				num = 20f;
				break;
			case PowerUpDuration.VeryShort:
				num = 10f;
				break;
			case PowerUpDuration.Blip:
				num = 2f;
				break;
			}
			switch (type)
			{
			case PowerupType.Unfreeze:
				if (!isInverted)
				{
					m_isFrozen = false;
					m_debuffTime_Freeze = 0f;
				}
				else
				{
					m_isFrozen = true;
					m_debuffTime_Freeze = Mathf.Max(m_debuffTime_Freeze, num);
				}
				break;
			case PowerupType.Health:
			{
				float amount = 0f;
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
					HealSosig(amount);
				}
				else
				{
					HarmSosig(amount);
				}
				break;
			}
			case PowerupType.QuadDamage:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_DamPowerUpDown = 4f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_DamPowerUpDown = 3f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_DamPowerUpDown = 2f;
						break;
					}
					m_isDamPowerUp = true;
					m_isDamPowerDown = false;
					DeActivateDeBuff(1);
					m_buffTime_DamPowerUp = Mathf.Max(m_buffTime_DamPowerUp, num);
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_DamPowerUpDown = 0.125f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_DamPowerUpDown = 0.25f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_DamPowerUpDown = 0.5f;
						break;
					}
					m_isDamPowerDown = true;
					m_isDamPowerUp = false;
					DeActivateBuff(1);
					m_debuffTime_DamPowerDown = Mathf.Max(m_debuffTime_DamPowerDown, num);
				}
				break;
			case PowerupType.InfiniteAmmo:
				if (!isInverted)
				{
					m_isInfiniteAmmo = true;
					m_isAmmoDrain = false;
					DeActivateDeBuff(2);
					m_buffTime_InfiniteAmmo = Mathf.Max(m_buffTime_InfiniteAmmo, num);
				}
				else
				{
					m_isAmmoDrain = true;
					m_isInfiniteAmmo = false;
					DeActivateBuff(2);
					m_debuffTime_AmmoDrain = Mathf.Max(m_debuffTime_AmmoDrain, num);
				}
				break;
			case PowerupType.Invincibility:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_isInvuln = true;
						m_isFragile = false;
						DeActivateDeBuff(3);
						m_isDamResist = false;
						m_isDamMult = false;
						m_buffTime_Invuln = Mathf.Max(m_buffTime_Invuln, num);
						break;
					case PowerUpIntensity.Medium:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateDeBuff(3);
						m_buffIntensity_DamResistHarm = 0.5f;
						m_buffTime_DamResist = Mathf.Max(m_buffTime_DamResist, num);
						break;
					case PowerUpIntensity.Low:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateDeBuff(3);
						m_buffIntensity_DamResistHarm = 0.75f;
						m_buffTime_DamResist = Mathf.Max(m_buffTime_DamResist, num);
						break;
					}
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_isFragile = true;
						m_isInvuln = false;
						DeActivateBuff(3);
						m_isDamResist = false;
						m_isDamMult = false;
						m_debuffTime_Fragile = Mathf.Max(m_debuffTime_Fragile, num);
						break;
					case PowerUpIntensity.Medium:
						m_isDamMult = true;
						m_isDamResist = false;
						DeActivateBuff(3);
						m_buffIntensity_DamResistHarm = 3f;
						m_debuffTime_DamMult = Mathf.Max(m_debuffTime_DamMult, num);
						break;
					case PowerUpIntensity.Low:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateBuff(3);
						m_buffIntensity_DamResistHarm = 2f;
						m_debuffTime_DamMult = Mathf.Max(m_debuffTime_DamMult, num);
						break;
					}
				}
				break;
			case PowerupType.Ghosted:
				if (!isInverted)
				{
					m_isGhosted = true;
					m_isSuperVisible = false;
					DeActivateDeBuff(4);
					m_buffTime_Ghosted = Mathf.Max(m_buffTime_Ghosted, num);
				}
				else
				{
					m_isSuperVisible = true;
					m_isGhosted = false;
					DeActivateBuff(4);
					m_debuffTime_SuperVisible = Mathf.Max(m_debuffTime_SuperVisible, num);
				}
				break;
			case PowerupType.MuscleMeat:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_MuscleMeatWeak = 4f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_MuscleMeatWeak = 3f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_MuscleMeatWeak = 2f;
						break;
					}
					m_isMuscleMeat = true;
					m_isWeakMeat = false;
					DeActivateDeBuff(6);
					m_buffTime_MuscleMeat = Mathf.Max(m_buffTime_MuscleMeat, num);
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_MuscleMeatWeak = 0.125f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_MuscleMeatWeak = 0.25f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_MuscleMeatWeak = 0.5f;
						break;
					}
					m_isWeakMeat = true;
					m_isMuscleMeat = false;
					DeActivateBuff(6);
					m_debuffTime_WeakMeat = Mathf.Max(m_debuffTime_WeakMeat, num);
				}
				break;
			case PowerupType.Regen:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_HealHarm = 20f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_HealHarm = 10f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_HealHarm = 5f;
						break;
					}
					m_isHealing = true;
					m_isHurting = false;
					DeActivateDeBuff(10);
					m_buffTime_Heal = Mathf.Max(m_buffTime_Heal, num);
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_HealHarm = 20f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_HealHarm = 10f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_HealHarm = 5f;
						break;
					}
					m_isHurting = true;
					m_isHealing = false;
					DeActivateBuff(10);
					m_debuffTime_Hurt = Mathf.Max(m_debuffTime_Hurt, num);
				}
				break;
			case PowerupType.Cyclops:
				switch (intensity)
				{
				case PowerUpIntensity.High:
					m_buffIntensity_CyclopsPower = 4f;
					break;
				case PowerUpIntensity.Medium:
					m_buffIntensity_CyclopsPower = 3f;
					break;
				case PowerUpIntensity.Low:
					m_buffIntensity_CyclopsPower = 2f;
					break;
				}
				if (!isInverted)
				{
					m_isCyclops = true;
					m_isBiClops = false;
					DeActivateDeBuff(11);
					m_buffTime_Cyclops = Mathf.Max(m_buffTime_Cyclops, num);
					if (m_vfx_cyclops == null)
					{
						m_vfx_cyclops = UnityEngine.Object.Instantiate(ManagerSingleton<PUM>.Instance.Sosig_Cyclops, Links[0].transform.position, Links[0].transform.rotation, Links[0].transform);
					}
					if (m_vfx_bicyclops != null)
					{
						UnityEngine.Object.Destroy(m_vfx_bicyclops);
					}
				}
				else
				{
					m_isBiClops = true;
					m_isCyclops = false;
					DeActivateBuff(11);
					m_debuffTime_BiClops = Mathf.Max(m_debuffTime_BiClops, num);
					if (m_vfx_bicyclops == null)
					{
						m_vfx_bicyclops = UnityEngine.Object.Instantiate(ManagerSingleton<PUM>.Instance.Sosig_Biclops, Links[0].transform.position, Links[0].transform.rotation, Links[0].transform);
					}
					if (m_vfx_cyclops != null)
					{
						UnityEngine.Object.Destroy(m_vfx_cyclops);
					}
				}
				break;
			case PowerupType.HomeTown:
				if (!isInverted)
				{
					TeleportSosig(GM.CurrentSceneSettings.PowerupPoint_HomeTown.position);
				}
				else
				{
					TeleportSosig(GM.CurrentSceneSettings.PowerupPoint_InverseHomeTown.position);
				}
				break;
			case PowerupType.WheredIGo:
				TeleportSosig(GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav[UnityEngine.Random.Range(0, GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav.Count)].position);
				break;
			case PowerupType.Blort:
				if (!isInverted)
				{
					m_isBlort = true;
					m_isDlort = false;
					m_buffTime_Blort = Mathf.Max(m_buffTime_Blort, num);
				}
				else
				{
					m_isDlort = true;
					m_isBlort = false;
					m_debuffTime_Dlort = Mathf.Max(m_debuffTime_Dlort, num);
				}
				break;
			case PowerupType.ChillOut:
				m_debuffTime_ChillOut = num;
				m_isChillOut = true;
				break;
			case PowerupType.Debuff:
				m_debuffTime_Debuff = num;
				m_isDebuff = true;
				break;
			case PowerupType.SpeedUp:
				if (!isInverted)
				{
					m_isSpeedup = true;
					m_buffTime_Speedup = Mathf.Max(m_debuffTime_Freeze, num);
				}
				break;
			}
			if (isPuke)
			{
				m_isVomitting = true;
				m_debuffTime_Vomit = num;
				if (m_vfx_vomit == null)
				{
					m_vfx_vomit = UnityEngine.Object.Instantiate(ManagerSingleton<PUM>.Instance.Sosig_Barfer, Links[0].transform.position, Links[0].transform.rotation, Links[0].transform);
				}
			}
			ActivateBuff((int)type, isInverted);
		}

		public void BuffHealing_Engage(float minTime, float healharm)
		{
			if (!IsDebuff)
			{
				m_buffTime_Heal = Mathf.Max(minTime, m_buffTime_Heal);
				m_buffIntensity_HealHarm = healharm;
				m_isHealing = true;
			}
		}

		public void BuffHealing_Invis(float minTime)
		{
			if (!IsDebuff)
			{
				m_buffTime_Ghosted = Mathf.Max(m_buffTime_Ghosted, minTime);
				m_isGhosted = true;
			}
		}

		public void BuffDamResist_Engage(float minTime, float resistHarm)
		{
			if (!IsDebuff)
			{
				m_buffTime_DamResist = Mathf.Max(minTime, m_buffTime_DamResist);
				m_buffIntensity_DamResistHarm = resistHarm;
				m_isDamResist = true;
			}
		}

		public void BuffInvuln_Engage(float minTime)
		{
			if (!IsDebuff)
			{
				m_buffTime_Invuln = Mathf.Max(minTime, m_buffTime_Invuln);
				m_isInvuln = true;
			}
		}

		private void HealSosig(float amount)
		{
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null)
				{
					Links[i].HealIntegrity(amount);
				}
				float value = Mustard + m_maxMustard * amount;
				if (Mustard < m_maxMustard)
				{
					Mustard = Mathf.Clamp(value, Mustard, m_maxMustard);
				}
			}
		}

		private void HarmSosig(float amount)
		{
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null)
				{
					Links[i].RemoveIntegrity(amount, Damage.DamageClass.Abstract);
				}
				Mustard -= m_maxMustard * amount;
			}
		}

		public void TeleportSosig(Vector3 point)
		{
		}

		private void BuffUpdate()
		{
			if (BodyState == SosigBodyState.Dead)
			{
				m_isHealing = false;
				m_isDamResist = false;
				m_isInvuln = false;
				m_isInfiniteAmmo = false;
				m_isGhosted = false;
				m_isMuscleMeat = false;
				m_isCyclops = false;
				m_isHurting = false;
				m_isDamMult = false;
				m_isFragile = false;
				m_isAmmoDrain = false;
				m_isSuperVisible = false;
				m_isWeakMeat = false;
				m_isBiClops = false;
			}
			if (m_isHealing && m_buffTime_Heal > 0f)
			{
				m_buffTime_Heal -= Time.deltaTime;
				if (m_buffTime_Heal <= 0f)
				{
					DeActivateBuff(10);
					m_isHealing = false;
				}
			}
			if (m_isDamResist && m_buffTime_DamResist > 0f)
			{
				m_buffTime_DamResist -= Time.deltaTime;
				if (m_buffTime_DamResist <= 0f)
				{
					DeActivateBuff(3);
					m_isDamResist = false;
				}
			}
			if (m_isInvuln && m_buffTime_Invuln > 0f)
			{
				m_buffTime_Invuln -= Time.deltaTime;
				if (m_buffTime_Invuln <= 0f)
				{
					DeActivateBuff(3);
					m_isInvuln = false;
				}
			}
			if (m_isDamPowerUp && m_buffTime_DamPowerUp > 0f)
			{
				m_buffTime_DamPowerUp -= Time.deltaTime;
				if (m_buffTime_DamPowerUp <= 0f)
				{
					DeActivateBuff(1);
					m_isDamPowerUp = false;
				}
			}
			if (m_isInfiniteAmmo && m_buffTime_InfiniteAmmo > 0f)
			{
				m_buffTime_InfiniteAmmo -= Time.deltaTime;
				if (m_buffTime_InfiniteAmmo <= 0f)
				{
					DeActivateBuff(2);
					m_isInfiniteAmmo = false;
				}
			}
			if (m_isGhosted && m_buffTime_Ghosted > 0f)
			{
				m_buffTime_Ghosted -= Time.deltaTime;
				if (m_buffTime_Ghosted <= 0f)
				{
					DeActivateBuff(4);
					m_isGhosted = false;
				}
			}
			if (m_isMuscleMeat && m_buffTime_MuscleMeat > 0f)
			{
				m_buffTime_MuscleMeat -= Time.deltaTime;
				if (m_buffTime_MuscleMeat <= 0f)
				{
					DeActivateBuff(6);
					m_isMuscleMeat = false;
				}
			}
			if (m_isCyclops && m_buffTime_Cyclops > 0f)
			{
				m_buffTime_Cyclops -= Time.deltaTime;
				if (m_buffTime_Cyclops <= 0f)
				{
					DeActivateBuff(11);
					m_isCyclops = false;
					if (m_vfx_cyclops != null)
					{
						UnityEngine.Object.Destroy(m_vfx_cyclops);
					}
				}
			}
			if (m_isBlort && m_buffTime_Blort > 0f)
			{
				m_buffTime_Blort -= Time.deltaTime;
				if (m_buffTime_Blort <= 0f)
				{
					DeActivateBuff(9);
					m_isBlort = false;
				}
			}
			if (m_isSpeedup && m_buffTime_Speedup > 0f)
			{
				m_buffTime_Speedup -= Time.deltaTime;
				if (m_buffTime_Speedup <= 0f)
				{
					m_isSpeedup = false;
				}
			}
			if (m_isDlort && m_debuffTime_Dlort > 0f)
			{
				m_debuffTime_Dlort -= Time.deltaTime;
				if (m_debuffTime_Dlort <= 0f)
				{
					DeActivateBuff(9);
					m_isDlort = false;
				}
			}
			if (m_isChillOut && m_debuffTime_ChillOut > 0f)
			{
				m_debuffTime_ChillOut -= Time.deltaTime;
				if (m_debuffTime_ChillOut <= 0f)
				{
					m_isChillOut = false;
					for (int i = 0; i < Links.Count; i++)
					{
						if (Links[i] != null)
						{
							Links[i].R.isKinematic = false;
						}
					}
				}
			}
			if (m_isVomitting && m_debuffTime_Vomit > 0f)
			{
				m_debuffTime_Vomit -= Time.deltaTime;
				if (m_debuffTime_Vomit <= 0f)
				{
					m_isVomitting = false;
					if (m_vfx_vomit != null)
					{
						UnityEngine.Object.Destroy(m_vfx_vomit);
					}
				}
			}
			if (m_isHurting && m_debuffTime_Hurt > 0f)
			{
				m_debuffTime_Hurt -= Time.deltaTime;
				if (m_debuffTime_Hurt <= 0f)
				{
					DeActivateDeBuff(10);
					m_isHurting = false;
				}
			}
			if (m_isDamMult && m_debuffTime_DamMult > 0f)
			{
				m_debuffTime_DamMult -= Time.deltaTime;
				if (m_debuffTime_DamMult <= 0f)
				{
					DeActivateDeBuff(3);
					m_isDamMult = false;
				}
			}
			if (m_isFragile && m_debuffTime_Fragile > 0f)
			{
				m_debuffTime_Fragile -= Time.deltaTime;
				if (m_debuffTime_Fragile <= 0f)
				{
					DeActivateDeBuff(3);
					m_isFragile = false;
				}
			}
			if (m_isDamPowerDown && m_debuffTime_DamPowerDown > 0f)
			{
				m_debuffTime_DamPowerDown -= Time.deltaTime;
				if (m_debuffTime_DamPowerDown <= 0f)
				{
					DeActivateDeBuff(1);
					m_isDamPowerDown = false;
				}
			}
			if (m_isAmmoDrain && m_debuffTime_AmmoDrain > 0f)
			{
				m_debuffTime_AmmoDrain -= Time.deltaTime;
				if (m_debuffTime_AmmoDrain <= 0f)
				{
					DeActivateDeBuff(2);
					m_isAmmoDrain = false;
				}
			}
			if (m_isSuperVisible && m_debuffTime_SuperVisible > 0f)
			{
				m_debuffTime_SuperVisible -= Time.deltaTime;
				if (m_debuffTime_SuperVisible <= 0f)
				{
					DeActivateDeBuff(4);
					m_isSuperVisible = false;
				}
			}
			if (m_isWeakMeat && m_debuffTime_WeakMeat > 0f)
			{
				m_debuffTime_WeakMeat -= Time.deltaTime;
				if (m_debuffTime_WeakMeat <= 0f)
				{
					DeActivateDeBuff(6);
					m_isWeakMeat = false;
				}
			}
			if (m_isBiClops && m_debuffTime_BiClops > 0f)
			{
				m_debuffTime_BiClops -= Time.deltaTime;
				if (m_debuffTime_BiClops <= 0f)
				{
					DeActivateDeBuff(11);
					m_isBiClops = false;
					if (m_vfx_bicyclops != null)
					{
						UnityEngine.Object.Destroy(m_vfx_bicyclops);
					}
				}
			}
			if (m_isFrozen && m_debuffTime_Freeze > 0f)
			{
				m_debuffTime_Freeze -= Time.deltaTime;
				if (m_debuffTime_Freeze <= 0f)
				{
					m_isFrozen = false;
				}
			}
			if (m_isDebuff && m_debuffTime_Debuff > 0f)
			{
				m_debuffTime_Debuff -= Time.deltaTime;
				if (m_debuffTime_Debuff <= 0f)
				{
					DeActivateBuff(14);
					m_isDebuff = false;
				}
			}
			if (m_isChillOut)
			{
				for (int j = 0; j < Links.Count; j++)
				{
					if (Links[j] != null)
					{
						Links[j].R.isKinematic = true;
					}
				}
			}
			MaterialType curMat = CurMat;
			if (CurMat == MaterialType.Vaporize)
			{
				return;
			}
			if (m_isGhosted && m_hasInvisMaterial)
			{
				if (CurMat != MaterialType.Invis)
				{
					CurMat = MaterialType.Invis;
					for (int k = 0; k < Links.Count; k++)
					{
						if (Links[k] != null)
						{
							Renderers[k].material = InvisMaterial;
						}
					}
				}
			}
			else if (m_isFrozen && m_hasFrozenMaterial)
			{
				if (CurMat != MaterialType.Frozen)
				{
					CurMat = MaterialType.Frozen;
					for (int l = 0; l < Links.Count; l++)
					{
						if (Links[l] != null)
						{
							Renderers[l].material = FrozenMaterial;
						}
					}
				}
			}
			else if (m_isInvuln && m_hasInvulnMaterial)
			{
				if (CurMat != MaterialType.Invuln)
				{
					CurMat = MaterialType.Invuln;
					for (int m = 0; m < Links.Count; m++)
					{
						if (Links[m] != null)
						{
							Renderers[m].material = InvulnMaterial;
						}
					}
				}
			}
			else if (CurMat != 0)
			{
				CurMat = MaterialType.Standard;
				for (int n = 0; n < Links.Count; n++)
				{
					if (Links[n] != null)
					{
						Renderers[n].material = GibMaterial;
					}
				}
			}
			if (CurMat == MaterialType.Invis && curMat != MaterialType.Invis)
			{
				for (int num = 0; num < Links.Count; num++)
				{
					if (Links[num] != null)
					{
						Links[num].DisableWearables();
					}
				}
			}
			if (CurMat == MaterialType.Invis || curMat != MaterialType.Invis)
			{
				return;
			}
			for (int num2 = 0; num2 < Links.Count; num2++)
			{
				if (Links[num2] != null)
				{
					Links[num2].EnableWearables();
				}
			}
		}

		public void Vaporize(GameObject PSystemPrefab_Lightning, int iff)
		{
			if (m_isVaporizing)
			{
				return;
			}
			m_isVaporizing = true;
			SosigDies(Damage.DamageClass.Projectile, SosigDeathType.JointExplosion);
			m_lastIFFDamageSource = iff;
			m_vaporizeSystems = new List<Transform>();
			if (CurMat == MaterialType.Vaporize)
			{
				return;
			}
			CurMat = MaterialType.Vaporize;
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null)
				{
					Renderers[i].material = VaporizeMaterial;
					GameObject gameObject = UnityEngine.Object.Instantiate(PSystemPrefab_Lightning, Links[i].transform.position, Links[i].transform.rotation);
					gameObject.transform.SetParent(Links[i].transform);
					Links[i].R.useGravity = false;
					Links[i].R.drag = 4.5f;
					Links[i].R.angularDrag = 3f;
					Links[i].C.enabled = false;
				}
			}
			for (int j = 0; j < Links.Count; j++)
			{
				if (Links[j] != null)
				{
					Links[j].DisableWearables();
				}
			}
		}

		private void VaporizeUpdate()
		{
			if (!m_isVaporizing)
			{
				return;
			}
			m_vaporizeTime -= Time.deltaTime;
			if (!(m_vaporizeTime < 0f) || m_isVaporized)
			{
				return;
			}
			m_isVaporized = true;
			for (int i = 0; i < m_vaporizeSystems.Count; i++)
			{
				if (m_vaporizeSystems[i] != null)
				{
					m_vaporizeSystems[i].SetParent(null);
				}
			}
			for (int j = 0; j < Links.Count; j++)
			{
				if (Links[j] != null)
				{
					Links[j].LinkExplodes(Damage.DamageClass.Projectile);
				}
			}
			for (int k = 0; k < Hands.Count; k++)
			{
				Hands[k].DropHeldObject();
			}
			Inventory.DropAllObjects();
			ClearCoverPoint();
		}

		private void BleedingUpdate()
		{
			if (m_storedShudder > 0f)
			{
				m_storedShudder -= Time.deltaTime;
			}
			else
			{
				m_storedShudder = 0f;
			}
			if (m_needsToSpawnBleedEvent && m_linkToBleed != null && Mustard > 0f)
			{
				m_needsToSpawnBleedEvent = false;
				if (m_bloodLossForVFX >= 10f)
				{
					UnityEngine.Object.Instantiate(DamageFX_LargeMustardBurst, m_bloodLossPoint, Quaternion.LookRotation(m_bloodLossDir));
					BleedingEvent item = new BleedingEvent(DamageFX_MustardSpoutLarge, m_linkToBleed, m_bloodLossForVFX, m_bloodLossPoint, -m_bloodLossDir, m_bloodLossForVFX * 0.25f, BleedVFXIntensity);
					m_bleedingEvents.Add(item);
				}
				if (m_bloodLossForVFX >= 1f)
				{
					UnityEngine.Object.Instantiate(DamageFX_SmallMustardBurst, m_bloodLossPoint, Quaternion.LookRotation(-m_bloodLossDir));
					BleedingEvent item2 = new BleedingEvent(DamageFX_MustardSpoutSmall, m_linkToBleed, m_bloodLossForVFX, m_bloodLossPoint, -m_bloodLossDir, m_bloodLossForVFX * 0.25f, BleedVFXIntensity);
					m_bleedingEvents.Add(item2);
				}
			}
			m_bleedRate = 0f;
			if (m_bleedingEvents.Count > 0)
			{
				float deltaTime = Time.deltaTime;
				for (int num = m_bleedingEvents.Count - 1; num >= 0; num--)
				{
					if (m_bleedingEvents[num].l == null || m_bleedingEvents[num].IsDone())
					{
						if (m_bleedingEvents[num].m_system != null)
						{
							UnityEngine.Object.Destroy(m_bleedingEvents[num].m_system);
						}
						m_bleedingEvents.RemoveAt(num);
					}
					else
					{
						float num2 = m_bleedingEvents[num].Update(deltaTime, Mustard);
						m_bleedRate += num2;
					}
				}
			}
			if (m_bleedRate <= 0f && BodyState != SosigBodyState.Dead && m_receivedHeadShot)
			{
				m_receivedHeadShot = false;
			}
			if (m_isHealing)
			{
				Mustard += BuffIntensity_HealHarm * Time.deltaTime;
				Mustard = Mathf.Clamp(Mustard, Mustard, m_maxMustard * 150f);
				for (int i = 0; i < Links.Count; i++)
				{
					if (Links[i] != null)
					{
						Links[i].HealIntegrity(BuffIntensity_HealHarm * Time.deltaTime);
					}
				}
			}
			else if (m_isHurting)
			{
				Mustard -= BuffIntensity_HealHarm * Time.deltaTime;
				for (int j = 0; j < Links.Count; j++)
				{
					if (Links[j] != null)
					{
						Links[j].RemoveIntegrity(BuffIntensity_HealHarm * Time.deltaTime, Damage.DamageClass.Abstract);
					}
				}
			}
			else if (Mustard > m_maxMustard)
			{
				Mustard -= Time.deltaTime * 2f;
			}
			if (Mustard > 0f && m_bleedRate > 0f)
			{
				Mustard -= m_bleedRate * BleedRateMult;
				if (Mustard <= 0f)
				{
					SosigDies(Damage.DamageClass.Abstract, SosigDeathType.BleedOut);
				}
			}
			m_hitDecalsThisFrameSoFar = 0;
			m_linkToBleed = null;
			m_bloodLossForVFX = 0f;
		}

		public float DistanceFromCoreTarget()
		{
			return Vector3.Distance(CoreRB.position, CoreTarget.position);
		}

		public void RequestHitDecal(Vector3 point, Vector3 normal, float scale, SosigLink l)
		{
			CleanUpDecals();
			if (m_hitDecalsThisFrameSoFar >= MaxDecalsPerFrame)
			{
				return;
			}
			m_hitDecalsThisFrameSoFar++;
			point = l.C.ClosestPoint(point);
			if (Mathf.Abs(l.transform.InverseTransformPoint(point).y) > 0.23f)
			{
				return;
			}
			if (m_spawnedHitDecals.Count < MaxTotalDecals)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(HitDecalPrefabs[UnityEngine.Random.Range(0, HitDecalPrefabs.Length)], point, Quaternion.LookRotation(normal, UnityEngine.Random.onUnitSphere));
				m_spawnedHitDecals.Add(gameObject.transform);
				scale = Mathf.Clamp(scale, HitDecalSizeRange.x, HitDecalSizeRange.y);
				gameObject.transform.localScale = new Vector3(scale, scale, scale * 0.5f);
				gameObject.transform.SetParent(l.transform);
				return;
			}
			if (m_nextDecalToMoveIndex >= m_spawnedHitDecals.Count)
			{
				m_nextDecalToMoveIndex = 0;
			}
			if (m_spawnedHitDecals[m_nextDecalToMoveIndex] == null)
			{
				m_spawnedHitDecals.RemoveAt(m_nextDecalToMoveIndex);
				return;
			}
			m_spawnedHitDecals[m_nextDecalToMoveIndex].position = point;
			m_spawnedHitDecals[m_nextDecalToMoveIndex].rotation = Quaternion.LookRotation(normal, UnityEngine.Random.onUnitSphere);
			scale = Mathf.Clamp(scale, HitDecalSizeRange.x, HitDecalSizeRange.y);
			m_spawnedHitDecals[m_nextDecalToMoveIndex].transform.localScale = new Vector3(scale, scale, scale * 0.5f);
			m_spawnedHitDecals[m_nextDecalToMoveIndex].SetParent(l.transform);
			m_nextDecalToMoveIndex++;
			if (m_nextDecalToMoveIndex >= m_spawnedHitDecals.Count)
			{
				m_nextDecalToMoveIndex = 0;
			}
		}

		public void RequestHitDecal(Vector3 point, Vector3 normal, Vector3 edgeNormal, float scale, SosigLink l)
		{
			CleanUpDecals();
			if (m_hitDecalsThisFrameSoFar >= MaxDecalsPerFrame)
			{
				return;
			}
			m_hitDecalsThisFrameSoFar++;
			point = l.C.ClosestPoint(point);
			if (m_spawnedHitDecals.Count < MaxTotalDecals)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(HitDecalPrefabs[UnityEngine.Random.Range(0, HitDecalPrefabs.Length)], point, Quaternion.LookRotation(normal, edgeNormal));
				m_spawnedHitDecals.Add(gameObject.transform);
				scale = Mathf.Clamp(scale, HitDecalSizeRange.x, HitDecalSizeRange.y);
				gameObject.transform.localScale = new Vector3(scale * 0.2f, scale, scale * 0.5f);
				gameObject.transform.SetParent(l.transform);
				return;
			}
			if (m_nextDecalToMoveIndex >= m_spawnedHitDecals.Count)
			{
				m_nextDecalToMoveIndex = 0;
			}
			if (m_spawnedHitDecals[m_nextDecalToMoveIndex] == null)
			{
				m_spawnedHitDecals.RemoveAt(m_nextDecalToMoveIndex);
				return;
			}
			m_spawnedHitDecals[m_nextDecalToMoveIndex].position = point;
			m_spawnedHitDecals[m_nextDecalToMoveIndex].rotation = Quaternion.LookRotation(normal, UnityEngine.Random.onUnitSphere);
			scale = Mathf.Clamp(scale, HitDecalSizeRange.x, HitDecalSizeRange.y);
			m_spawnedHitDecals[m_nextDecalToMoveIndex].transform.localScale = new Vector3(scale * 0.2f, scale, scale * 0.5f);
			m_spawnedHitDecals[m_nextDecalToMoveIndex].SetParent(l.transform);
			m_nextDecalToMoveIndex++;
			if (m_nextDecalToMoveIndex >= m_spawnedHitDecals.Count)
			{
				m_nextDecalToMoveIndex = 0;
			}
		}

		private void CleanUpDecals()
		{
			if (m_spawnedHitDecals.Count <= 0)
			{
				return;
			}
			for (int num = m_spawnedHitDecals.Count - 1; num >= 0; num--)
			{
				if (m_spawnedHitDecals[num] == null)
				{
					m_spawnedHitDecals.RemoveAt(num);
				}
			}
			if (m_nextDecalToMoveIndex >= m_spawnedHitDecals.Count)
			{
				m_nextDecalToMoveIndex = m_spawnedHitDecals.Count - 1;
				if (m_nextDecalToMoveIndex < 0)
				{
					m_nextDecalToMoveIndex = 0;
				}
			}
		}

		public void AccurueBleedingHit(SosigLink l, Vector3 point, Vector3 dir, float bloodAmount)
		{
			m_needsToSpawnBleedEvent = true;
			m_linkToBleed = l;
			m_bloodLossPoint = l.C.ClosestPoint(point);
			m_bloodLossDir = dir;
			m_bloodLossForVFX += bloodAmount * BleedDamageMult;
			if (BodyState != SosigBodyState.Dead && l.BodyPart == SosigLink.SosigBodyPart.Head)
			{
				m_receivedHeadShot = true;
			}
		}

		public void ProcessDamage(Damage d, SosigLink link)
		{
			Stun(d.Dam_Stunning);
			if (d.Dam_Stunning > 1f)
			{
				Shudder(d.Dam_Stunning);
			}
			SetLastIFFDamageSource(d.Source_IFF);
			if (d.Class != 0 && d.Source_IFF != E.IFFCode && BodyState != SosigBodyState.Dead)
			{
				m_diedFromClass = d.Class;
			}
			Blind(d.Dam_Blinding);
			ProcessDamage(d.Dam_Piercing, d.Dam_Cutting, d.Dam_Blunt, d.Dam_Thermal, d.point, link);
		}

		public void ProcessDamage(float damage_p, float damage_c, float damage_b, float damage_t, Vector3 point, SosigLink link)
		{
			if (BodyState == SosigBodyState.Dead || m_isInvuln)
			{
				return;
			}
			damage_p *= DamMult_Piercing;
			damage_c *= DamMult_Cutting;
			damage_b *= DamMult_Blunt;
			damage_t *= DamMult_Thermal;
			if (damage_t > 0f && m_isFrozen)
			{
				m_isFrozen = false;
				m_debuffTime_Freeze = 0f;
			}
			if (m_isBlort)
			{
				damage_t = 0f;
			}
			else if (m_isDlort)
			{
				damage_t *= 10f;
			}
			if (BodyState != SosigBodyState.Dead && link.BodyPart == SosigLink.SosigBodyPart.Head)
			{
				m_receivedHeadShot = true;
			}
			float num = Mathf.Lerp(0.01f, 5f, (damage_p + damage_c + damage_b + damage_t * 5f) / 2000f) * link.StaggerMagnitude;
			if (link.BodyPart == SosigLink.SosigBodyPart.Head && num > StunThreshold)
			{
				m_isStunned = true;
				m_stunTimeLeft = Mathf.Max(m_stunTimeLeft, num * StunMultiplier);
			}
			if (num >= 0.1f)
			{
				m_timeSinceLastDamage = 0f;
				if (UnityEngine.Random.value > 0.3f)
				{
					Invoke("DelayedSpeakPain", 0.01f);
				}
			}
			if (num > ConfusionThreshold)
			{
				m_isConfused = true;
				m_confusedTime = Mathf.Max(m_confusedTime, num * ConfusionMultiplier);
			}
			if (num + m_storedShudder > ShudderThreshold)
			{
				m_storedShudder = 0f;
				Shudder(num);
			}
			else if (num > ShudderThreshold * 0.2f)
			{
				m_storedShudder += num;
			}
			if (num >= 0.1f && (!m_isStunned || !m_isConfused))
			{
				float num2 = Mathf.Clamp(num, 0f, 1f);
				AIEvent e = new AIEvent(point, AIEvent.AIEType.Damage, 1f - num2);
				EventReceive(e);
			}
		}

		public void Shudder(float amount)
		{
			if (IsInvuln)
			{
				return;
			}
			SetBodyState(SosigBodyState.Ballistic);
			m_recoveryFromBallisticTick = Mathf.Max(amount, m_recoveryFromBallisticTick) + amount * 0.5f;
			if (DoesDropWeaponsOnBallistic)
			{
				for (int i = 0; i < Hands.Count; i++)
				{
					Hands[i].DropHeldObject();
				}
			}
			for (int j = 0; j < Links.Count; j++)
			{
				if (Links[j] != null && !m_jointsSevered[j])
				{
					amount = Mathf.Clamp(amount, 0f, 0.4f);
					Links[j].R.AddForce(UnityEngine.Random.onUnitSphere * amount * 5f, ForceMode.VelocityChange);
				}
			}
		}

		public void BreakJoint(SosigLink link, bool isStart, Damage.DamageClass damClass)
		{
			if (m_jointsBroken[(int)link.BodyPart])
			{
				return;
			}
			m_jointsBroken[(int)link.BodyPart] = true;
			if (link.BodyPart == SosigLink.SosigBodyPart.Head)
			{
				NeckBreak(isStart);
				if (m_doesJointBreakKill_Head)
				{
					SosigDies(damClass, SosigDeathType.JointBreak);
				}
			}
			else if (link.BodyPart == SosigLink.SosigBodyPart.UpperLink)
			{
				BreakBack(isStart);
				if (m_doesJointBreakKill_Upper)
				{
					SosigDies(damClass, SosigDeathType.JointBreak);
				}
			}
			else if (link.BodyPart == SosigLink.SosigBodyPart.LowerLink)
			{
				Hobble(isStart);
				if (m_doesJointBreakKill_Lower)
				{
					SosigDies(damClass, SosigDeathType.JointBreak);
				}
			}
		}

		public void SeverJoint(SosigLink link, bool isJointSlice, Damage.DamageClass damClass, bool isPullApart)
		{
			if (m_jointsSevered[(int)link.BodyPart])
			{
				return;
			}
			m_jointsSevered[(int)link.BodyPart] = true;
			UnityEngine.Object.Instantiate(DamageFX_LargeMustardBurst, link.transform.position + Vector3.up * link.J.anchor.y, Quaternion.LookRotation(link.transform.up));
			UnityEngine.Object.Instantiate(DamageFX_LargeMustardBurst, link.transform.position + Vector3.up * link.J.anchor.y, Quaternion.LookRotation(-link.transform.up));
			UpdateRenderers();
			UnityEngine.Object.Destroy(link.J);
			if (UnityEngine.Random.value > 0.5f)
			{
				if (isJointSlice)
				{
					Speak_Pain(Speech.OnJointSlice);
				}
				else
				{
					Speak_Pain(Speech.OnJointSever);
				}
			}
			else
			{
				Invoke("DelayedSpeakPain", 0.01f);
			}
			SosigDeathType deathType = SosigDeathType.JointSever;
			if (isPullApart)
			{
				deathType = SosigDeathType.JointPullApart;
			}
			switch (link.BodyPart)
			{
			case SosigLink.SosigBodyPart.Head:
				if (m_doesSeverKill_Head)
				{
					SosigDies(damClass, deathType);
				}
				break;
			case SosigLink.SosigBodyPart.UpperLink:
				if (m_doesSeverKill_Upper)
				{
					SosigDies(damClass, deathType);
				}
				else
				{
					BreakBack(isStarting: false);
				}
				break;
			case SosigLink.SosigBodyPart.LowerLink:
				if (m_doesSeverKill_Lower)
				{
					SosigDies(damClass, deathType);
				}
				else
				{
					Hobble(isStarting: false);
				}
				break;
			}
		}

		public void DestroyLink(SosigLink link, Damage.DamageClass damClass)
		{
			if (link.J != null)
			{
				UnityEngine.Object.Destroy(link.J);
			}
			for (int num = Links.Count - 1; num >= 0; num--)
			{
				if (Links[num] != null && Links[num].J != null && Links[num].J.connectedBody == link.R)
				{
					Links[num].transform.SetParent(null);
					UnityEngine.Object.Destroy(Links[num].J);
				}
			}
			m_linksDestroyed[(int)link.BodyPart] = true;
			Mustard *= 0.75f;
			switch (link.BodyPart)
			{
			case SosigLink.SosigBodyPart.Head:
				KillSpeech();
				if (m_doesExplodeKill_Head)
				{
					SosigDies(damClass, SosigDeathType.JointExplosion);
				}
				else
				{
					Shudder(3f);
				}
				break;
			case SosigLink.SosigBodyPart.Torso:
				SosigDies(damClass, SosigDeathType.JointExplosion);
				break;
			case SosigLink.SosigBodyPart.UpperLink:
				if (m_doesExplodeKill_Upper)
				{
					SosigDies(damClass, SosigDeathType.JointExplosion);
					break;
				}
				BreakBack(isStarting: false);
				Shudder(2f);
				break;
			case SosigLink.SosigBodyPart.LowerLink:
				if (m_doesExplodeKill_Lower)
				{
					SosigDies(damClass, SosigDeathType.JointExplosion);
					break;
				}
				Hobble(isStarting: false);
				Shudder(2f);
				break;
			}
			UnityEngine.Object.Instantiate(DamageFX_Explosion, link.transform.position, UnityEngine.Random.rotation);
			if (link.HasSpawnOnDestroy)
			{
				UnityEngine.Object.Instantiate(link.SpawnOnDestroy.GetGameObject(), link.transform.position, Quaternion.identity);
			}
			if (UsesGibs)
			{
				Vector3 position = link.transform.position;
				Quaternion rotation = link.transform.rotation;
				for (int i = 0; i < GibLocalPoses.Length; i++)
				{
					Vector3 position2 = link.transform.position + link.transform.right * GibLocalPoses[i].x + link.transform.up * GibLocalPoses[i].y + link.transform.forward * GibLocalPoses[i].z;
					UnityEngine.Object.Instantiate(GibPrefabs[i], position2, rotation);
				}
			}
			UnityEngine.Object.Destroy(link.gameObject);
			CleanUpDecals();
			UpdateRenderers();
		}

		public void NeckBreak(bool isStarting)
		{
			BreakJointLimit(0);
			if (!isStarting)
			{
				if (UnityEngine.Random.value > 0.5f)
				{
					Speak_Pain(Speech.OnJointBreak);
				}
				else
				{
					Speak_Pain(Speech.OnNeckBreak);
				}
			}
		}

		public void Hobble(bool isStarting)
		{
			m_isHobbled = true;
			SetBodyPose(SosigBodyPose.Crouching);
			BreakJointLimit(3);
			if (!isStarting)
			{
				Shudder(UnityEngine.Random.Range(1.5f, 3f));
				Confuse(10f);
				if (UnityEngine.Random.value > 0.5f)
				{
					Speak_Pain(Speech.OnJointBreak);
				}
				else
				{
					Invoke("DelayedSpeakPain", 0.01f);
				}
			}
		}

		public void BreakBack(bool isStarting)
		{
			m_isBackBroken = true;
			SetBodyPose(SosigBodyPose.Prone);
			BreakJointLimit(2);
			if (!isStarting)
			{
				Shudder(UnityEngine.Random.Range(2f, 5f));
				Confuse(20f);
				if (UnityEngine.Random.value > 0.5f)
				{
					Speak_Pain(Speech.OnJointBreak);
				}
				else
				{
					Speak_Pain(Speech.OnBackBreak);
				}
			}
		}

		public void Confuse(float f)
		{
			m_isConfused = true;
			m_confusedTime = Mathf.Max(m_confusedTime, f);
		}

		public void Stun(float f)
		{
			m_isStunned = true;
			m_stunTimeLeft = Mathf.Max(m_stunTimeLeft, f);
		}

		public void Blind(float f)
		{
			m_isBlinded = true;
			m_blindTime = Mathf.Max(m_blindTime, f);
		}

		private void UpdateRenderers()
		{
			for (int i = 0; i < Links.Count; i++)
			{
				if (!m_linksDestroyed[i])
				{
					UpdateRendererOnLink(i);
				}
			}
		}

		public void UpdateRendererOnLink(int i)
		{
			if (m_linksDestroyed[i])
			{
				return;
			}
			if (UsesLinkMeshOverride)
			{
				switch (i)
				{
				case 0:
				{
					int damageMeshIndex2 = GetDamageMeshIndex(Links[0]);
					if (m_linksDestroyed[1] || (m_jointsSevered[0] && !m_linksDestroyed[0]))
					{
						Meshes[0].mesh = Links[0].Meshes_Severed_Top[damageMeshIndex2];
					}
					else
					{
						Meshes[0].mesh = Links[0].Meshes_Whole[damageMeshIndex2];
					}
					break;
				}
				case 1:
				{
					int damageMeshIndex3 = GetDamageMeshIndex(Links[1]);
					if ((m_linksDestroyed[0] || m_jointsSevered[0]) && (m_linksDestroyed[2] || m_jointsSevered[2]))
					{
						Meshes[1].mesh = Links[1].Meshes_Severed_Both[damageMeshIndex3];
					}
					else if (m_linksDestroyed[2] || m_jointsSevered[2])
					{
						Meshes[1].mesh = Links[1].Meshes_Severed_Top[damageMeshIndex3];
					}
					else if (m_linksDestroyed[0] || m_jointsSevered[0])
					{
						Meshes[1].mesh = Links[1].Meshes_Severed_Bottom[damageMeshIndex3];
					}
					else
					{
						Meshes[1].mesh = Links[1].Meshes_Whole[damageMeshIndex3];
					}
					break;
				}
				case 2:
				{
					int damageMeshIndex4 = GetDamageMeshIndex(Links[2]);
					if ((m_linksDestroyed[1] || m_jointsSevered[2]) && (m_linksDestroyed[3] || m_jointsSevered[3]))
					{
						Meshes[2].mesh = Links[2].Meshes_Severed_Both[damageMeshIndex4];
					}
					else if (m_linksDestroyed[3] || m_jointsSevered[3])
					{
						Meshes[2].mesh = Links[2].Meshes_Severed_Top[damageMeshIndex4];
					}
					else if (m_linksDestroyed[1] || m_jointsSevered[2])
					{
						Meshes[2].mesh = Links[2].Meshes_Severed_Bottom[damageMeshIndex4];
					}
					else
					{
						Meshes[2].mesh = Links[2].Meshes_Whole[damageMeshIndex4];
					}
					break;
				}
				case 3:
				{
					int damageMeshIndex = GetDamageMeshIndex(Links[3]);
					if (m_linksDestroyed[2] || m_jointsSevered[3])
					{
						Meshes[3].mesh = Links[3].Meshes_Severed_Bottom[damageMeshIndex];
					}
					else
					{
						Meshes[3].mesh = Links[3].Meshes_Whole[damageMeshIndex];
					}
					break;
				}
				}
				return;
			}
			switch (i)
			{
			case 0:
			{
				int damageMeshIndex6 = GetDamageMeshIndex(Links[0]);
				if (m_linksDestroyed[1] || (m_jointsSevered[0] && !m_linksDestroyed[0]))
				{
					Meshes[0].mesh = Meshes_Severed_Top[damageMeshIndex6];
				}
				else
				{
					Meshes[0].mesh = Meshes_Whole[damageMeshIndex6];
				}
				break;
			}
			case 1:
			{
				int damageMeshIndex7 = GetDamageMeshIndex(Links[1]);
				if ((m_linksDestroyed[0] || m_jointsSevered[0]) && (m_linksDestroyed[2] || m_jointsSevered[2]))
				{
					Meshes[1].mesh = Meshes_Severed_Both[damageMeshIndex7];
				}
				else if (m_linksDestroyed[2] || m_jointsSevered[2])
				{
					Meshes[1].mesh = Meshes_Severed_Top[damageMeshIndex7];
				}
				else if (m_linksDestroyed[0] || m_jointsSevered[0])
				{
					Meshes[1].mesh = Meshes_Severed_Bottom[damageMeshIndex7];
				}
				else
				{
					Meshes[1].mesh = Meshes_Whole[damageMeshIndex7];
				}
				break;
			}
			case 2:
			{
				int damageMeshIndex8 = GetDamageMeshIndex(Links[2]);
				if ((m_linksDestroyed[1] || m_jointsSevered[2]) && (m_linksDestroyed[3] || m_jointsSevered[3]))
				{
					Meshes[2].mesh = Meshes_Severed_Both[damageMeshIndex8];
				}
				else if (m_linksDestroyed[3] || m_jointsSevered[3])
				{
					Meshes[2].mesh = Meshes_Severed_Top[damageMeshIndex8];
				}
				else if (m_linksDestroyed[1] || m_jointsSevered[2])
				{
					Meshes[2].mesh = Meshes_Severed_Bottom[damageMeshIndex8];
				}
				else
				{
					Meshes[2].mesh = Meshes_Whole[damageMeshIndex8];
				}
				break;
			}
			case 3:
			{
				int damageMeshIndex5 = GetDamageMeshIndex(Links[3]);
				if (m_linksDestroyed[2] || m_jointsSevered[3])
				{
					Meshes[3].mesh = Meshes_Severed_Bottom[damageMeshIndex5];
				}
				else
				{
					Meshes[3].mesh = Meshes_Whole[damageMeshIndex5];
				}
				break;
			}
			}
		}

		private int GetDamageMeshIndex(SosigLink l)
		{
			if (l == null)
			{
				return 0;
			}
			return l.GetDamageStateIndex();
		}

		public void Writhe()
		{
			for (int i = 0; i < Links.Count; i++)
			{
				if (Links[i] != null && !m_jointsSevered[i])
				{
					Links[i].R.AddForce(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0.1f, 1f), ForceMode.VelocityChange);
				}
			}
		}

		public void SetLastIFFDamageSource(int iff)
		{
			if (BodyState != SosigBodyState.Dead && iff > -1)
			{
				m_lastIFFDamageSource = iff;
			}
		}

		public bool GetDiedFromHeadShot()
		{
			return m_receivedHeadShot;
		}

		public int GetDiedFromIFF()
		{
			return m_lastIFFDamageSource;
		}

		public Damage.DamageClass GetDiedFromClass()
		{
			return m_diedFromClass;
		}

		public SosigDeathType GetDiedFromType()
		{
			return m_diedFromType;
		}

		public void SosigDies(Damage.DamageClass damClass, SosigDeathType deathType)
		{
			if (BodyState == SosigBodyState.Dead)
			{
				return;
			}
			DeActivateAllBuffSystems();
			if (damClass != 0)
			{
				m_diedFromClass = damClass;
			}
			m_diedFromType = deathType;
			if (!m_linksDestroyed[0])
			{
				Speak_Pain(Speech.OnDeath);
			}
			else
			{
				KillSpeech();
				if (Speech.UseAltDeathOnHeadExplode)
				{
					Speak_Pain(Speech.OnDeathAlt);
				}
			}
			SetBodyState(SosigBodyState.Dead);
			CurrentOrder = SosigOrder.Disabled;
			FallbackOrder = SosigOrder.Disabled;
			SetHandObjectUsage(SosigObjectUsageFocus.EmptyHands);
			SetMovementState(SosigMovementState.Idle);
			E.IFFCode = -3;
			for (int i = 0; i < Hands.Count; i++)
			{
				Hands[i].DropHeldObject();
			}
			Inventory.DropAllObjects();
			for (int j = 0; j < Links.Count; j++)
			{
				if (Links[j] != null && !m_jointsSevered[j])
				{
					Links[j].R.AddForce(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 5f), ForceMode.VelocityChange);
					Links[j].O.DistantGrabbable = true;
				}
			}
			GM.CurrentSceneSettings.OnSosigKill(this);
		}

		public void ProcessCollision(SosigLink l, Collision col)
		{
			if (IgnoreRBs.Contains(col.collider.attachedRigidbody))
			{
				return;
			}
			float num = col.relativeVelocity.magnitude;
			bool flag = false;
			if (col.collider.attachedRigidbody != null && !col.collider.attachedRigidbody.isKinematic)
			{
				flag = true;
				if (col.collider.attachedRigidbody.gameObject.GetComponent<FVRMeleeWeapon>() != null)
				{
					return;
				}
				SosigWeapon component = col.collider.attachedRigidbody.gameObject.GetComponent<SosigWeapon>();
				if (component != null && (component.Type == SosigWeapon.SosigWeaponType.Melee || component.IsHeldByBot || component.IsInBotInventory))
				{
					return;
				}
			}
			if (!flag && BodyState == SosigBodyState.InControl)
			{
				return;
			}
			bool flag2 = false;
			if (col.contacts[0].thisCollider.transform.parent != l.C.transform)
			{
				flag2 = true;
			}
			if (m_isDamResist)
			{
				num *= 0.6f;
			}
			if (m_isInvuln)
			{
				return;
			}
			if (num > 1f)
			{
				float num2 = 1f;
				if (flag)
				{
					num2 = 4f;
				}
				if (l.O.IsHeld)
				{
					num2 = 8f;
				}
				if (flag2)
				{
					num2 *= l.CollisionBluntDamageMultiplier;
				}
				if (flag || l.O.IsHeld)
				{
					l.DamageIntegrity(num * num2, 0f, 0f, 0f, col.relativeVelocity, col.contacts[0].point, Damage.DamageClass.Melee, -3);
				}
			}
			if (num > 6f)
			{
				float num3 = 1f;
				if (flag)
				{
					num3 = 2f;
				}
				if (l.O.IsHeld)
				{
					num3 = 3f;
				}
				if (flag2)
				{
					num3 *= l.CollisionBluntDamageMultiplier;
				}
				if (flag || l.O.IsHeld)
				{
					ProcessDamage(0f, 0f, num * num * num3, 0f, col.contacts[0].point, l);
				}
			}
			if (num > 20f && !flag)
			{
				switch (l.BodyPart)
				{
				case SosigLink.SosigBodyPart.Head:
					l.BreakJoint(isStart: false, Damage.DamageClass.Environment);
					break;
				case SosigLink.SosigBodyPart.UpperLink:
					l.BreakJoint(isStart: false, Damage.DamageClass.Environment);
					break;
				case SosigLink.SosigBodyPart.LowerLink:
					l.BreakJoint(isStart: false, Damage.DamageClass.Environment);
					break;
				}
			}
			if (num > 30f && flag && col.collider.attachedRigidbody.mass > 1f)
			{
				switch (l.BodyPart)
				{
				case SosigLink.SosigBodyPart.Head:
					l.BreakJoint(isStart: false, Damage.DamageClass.Environment);
					break;
				case SosigLink.SosigBodyPart.UpperLink:
					l.BreakJoint(isStart: false, Damage.DamageClass.Environment);
					break;
				case SosigLink.SosigBodyPart.LowerLink:
					l.BreakJoint(isStart: false, Damage.DamageClass.Environment);
					break;
				}
			}
		}

		public void Stagger(float f)
		{
			SetBodyState(SosigBodyState.Ballistic);
			m_recoveryFromBallisticTick = Mathf.Max(f, m_recoveryFromBallisticTick);
		}

		private void SetBodyState(SosigBodyState s)
		{
			if (BodyState != s && BodyState != SosigBodyState.Dead)
			{
				BodyState = s;
				switch (BodyState)
				{
				case SosigBodyState.Ballistic:
					EndLink();
					Agent.enabled = false;
					m_recoveringFromBallisticState = false;
					m_recoveryFromBallisticLerp = 0f;
					m_tickDownToWrithe = UnityEngine.Random.Range(m_writheTickRange.x, m_writheTickRange.y);
					LoosenJoints();
					break;
				case SosigBodyState.InControl:
					m_isCountingDownToStagger = false;
					m_staggerAmountToApply = 0f;
					break;
				case SosigBodyState.Dead:
					EndLink();
					Agent.enabled = false;
					LoosenJoints();
					break;
				}
			}
		}

		private void SosigPhys()
		{
			if (BodyState == SosigBodyState.InControl)
			{
				Vector3 position = CoreRB.position;
				Quaternion rotation = CoreRB.rotation;
				Vector3 position2 = CoreTarget.position;
				Quaternion rotation2 = CoreTarget.rotation;
				Vector3 vector = position2 - position;
				Quaternion quaternion = rotation2 * Quaternion.Inverse(rotation);
				float deltaTime = Time.deltaTime;
				quaternion.ToAngleAxis(out var angle, out var axis);
				if (angle > 180f)
				{
					angle -= 360f;
				}
				if (angle != 0f)
				{
					Vector3 target = deltaTime * angle * axis * AttachedRotationMultiplier;
					CoreRB.angularVelocity = Vector3.MoveTowards(CoreRB.angularVelocity, target, AttachedRotationFudge * Time.fixedDeltaTime);
				}
				Vector3 target2 = vector * AttachedPositionMultiplier * deltaTime;
				CoreRB.velocity = Vector3.MoveTowards(CoreRB.velocity, target2, AttachedPositionFudge * deltaTime);
			}
		}

		private void UpdateJoints(float l)
		{
			float num = Mathf.Lerp(60f, m_maxJointLimit, l);
			for (int i = 0; i < m_joints.Count; i++)
			{
				if ((!m_jointsBroken[0] || i != 0) && (!m_isHobbled || i < 3) && (!m_isBackBroken || i < 2) && m_joints[i] != null)
				{
					SoftJointLimit lowTwistLimit = m_joints[i].lowTwistLimit;
					lowTwistLimit.limit = 0f - num;
					m_joints[i].lowTwistLimit = lowTwistLimit;
					lowTwistLimit = m_joints[i].highTwistLimit;
					lowTwistLimit.limit = num;
					m_joints[i].highTwistLimit = lowTwistLimit;
					lowTwistLimit = m_joints[i].swing1Limit;
					lowTwistLimit.limit = num;
					m_joints[i].swing1Limit = lowTwistLimit;
					lowTwistLimit = m_joints[i].swing2Limit;
					lowTwistLimit.limit = num;
					m_joints[i].swing2Limit = lowTwistLimit;
				}
			}
		}

		private void LoosenJoints()
		{
			for (int i = 0; i < m_joints.Count; i++)
			{
				if (m_joints[i] != null)
				{
					SoftJointLimit lowTwistLimit = m_joints[i].lowTwistLimit;
					lowTwistLimit.limit = -60f;
					m_joints[i].lowTwistLimit = lowTwistLimit;
					lowTwistLimit = m_joints[i].highTwistLimit;
					lowTwistLimit.limit = 60f;
					m_joints[i].highTwistLimit = lowTwistLimit;
					lowTwistLimit = m_joints[i].swing1Limit;
					lowTwistLimit.limit = 60f;
					m_joints[i].swing1Limit = lowTwistLimit;
					lowTwistLimit = m_joints[i].swing2Limit;
					lowTwistLimit.limit = 60f;
					m_joints[i].swing2Limit = lowTwistLimit;
				}
			}
		}

		private void BreakJointLimit(int i)
		{
			if (!m_linksDestroyed[i] && Links[i].J != null)
			{
				CharacterJoint j = Links[i].J;
				SoftJointLimit lowTwistLimit = j.lowTwistLimit;
				lowTwistLimit.limit = -60f;
				j.lowTwistLimit = lowTwistLimit;
				lowTwistLimit = j.highTwistLimit;
				lowTwistLimit.limit = 60f;
				j.highTwistLimit = lowTwistLimit;
				lowTwistLimit = j.swing1Limit;
				lowTwistLimit.limit = 35f;
				j.swing1Limit = lowTwistLimit;
				lowTwistLimit = j.swing2Limit;
				lowTwistLimit.limit = 35f;
				j.swing2Limit = lowTwistLimit;
				SoftJointLimitSpring twistLimitSpring = j.twistLimitSpring;
				twistLimitSpring.damper = 100f;
				j.twistLimitSpring = twistLimitSpring;
			}
		}

		private void TightenJoints()
		{
			for (int i = 0; i < m_joints.Count; i++)
			{
				if (m_joints[i] != null)
				{
					SoftJointLimit lowTwistLimit = m_joints[i].lowTwistLimit;
					lowTwistLimit.limit = 0f - m_maxJointLimit;
					m_joints[i].lowTwistLimit = lowTwistLimit;
					lowTwistLimit = m_joints[i].highTwistLimit;
					lowTwistLimit.limit = m_maxJointLimit;
					m_joints[i].highTwistLimit = lowTwistLimit;
					lowTwistLimit = m_joints[i].swing1Limit;
					lowTwistLimit.limit = m_maxJointLimit;
					m_joints[i].swing1Limit = lowTwistLimit;
					lowTwistLimit = m_joints[i].swing2Limit;
					lowTwistLimit.limit = m_maxJointLimit;
					m_joints[i].swing2Limit = lowTwistLimit;
				}
			}
		}
	}
}
