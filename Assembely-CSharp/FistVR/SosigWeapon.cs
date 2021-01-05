using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class SosigWeapon : MonoBehaviour, IFVRDamageable
	{
		public enum SosigWeaponType
		{
			Gun,
			Melee,
			Grenade,
			Ammo,
			Healing,
			Shield
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
			Zosig
		}

		public enum SosigWeaponMechaState
		{
			ReadyToFire,
			CyclingBack,
			CyclingForward
		}

		public enum SosigMeleeWeaponType
		{
			Bladed,
			Blunt,
			Stabbing,
			Shield
		}

		public enum SosigMeleeState
		{
			Ready,
			Attacking,
			Recovery
		}

		public enum SosigWeaponUsageState
		{
			Firing,
			Reloading
		}

		public enum SosigWeaponLayerState
		{
			Agent,
			Player
		}

		public enum InterpStyle
		{
			Translate,
			Rotation
		}

		public enum Axis
		{
			X,
			Y,
			Z
		}

		[Header("Sosig Weapon Stuff")]
		public int Quality = 1;

		private int m_baseQuality = 1;

		public SosigWeaponType Type;

		public bool UsesFakeType;

		public SosigWeaponType FakeType;

		public bool IsAlsoThrowable;

		private SosigWeaponType m_backupType;

		public bool PrimesOnFirstSwing;

		public bool IsDroppedOnStrongHit = true;

		public SosiggunAmmoType AmmoType;

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

		public Vector2 PreferredRange = new Vector2(0f, 10f);

		[Header("Gun Mechanics Params")]
		public SosigWeaponMechaState MechaState;

		public bool UsesSustainedSound;

		public AudioEvent AudEvent_SustainedEmit;

		public AudioEvent AudEvent_SustainedTerminate;

		public AudioSource AudSource_SustainedLoop;

		public float SustainEnergyPerShot = 0.2f;

		private float m_sustainEnergy;

		public Transform Muzzle;

		public Transform CyclingPart;

		public bool HasCyclingPart = true;

		public Axis CycleAxis;

		public InterpStyle CycleInterpStyle;

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
		public SosigMeleeWeaponType MeleeType;

		public List<SosigMeleeWeaponType> MeleeTypeCycleList;

		public bool DoesMeleeTypeCycle;

		private int m_meleeTypeCycle;

		private float m_tickDownToTypeCycle = 4f;

		public bool DoesHeightAdjust = true;

		public SosigMeleeState MeleeState;

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
		public SosigWeaponUsageState UsageState;

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

		public SosigWeaponLayerState LayerState;

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

		public SosigWeaponType GetMyType()
		{
			if (UsesFakeType)
			{
				return FakeType;
			}
			return Type;
		}

		public void SetAmmoClamping(bool b)
		{
			m_ammoClampOnPlayerPickup = b;
		}

		protected void Start()
		{
			m_startingLife = Life;
			m_shotsLeft = ShotsPerLoad;
			PrimeDics();
			m_baseQuality = Quality;
			m_shotsLeftTilBurstLimit = BurstLimit;
			m_startFuseTime = GrenadeFuseTime;
			m_currentStateSpeeds = MeleeStateSpeeds;
			if (ImpactController != null)
			{
				m_hasImpactController = true;
			}
			if (Laser != null)
			{
				m_hasLaser = true;
			}
			if (AvailableIdleAnims.Count > 0)
			{
				m_currentIdleAnim = AvailableIdleAnims[Random.Range(0, AvailableIdleAnims.Count)];
			}
			if (AvailableReadyAnims.Count > 0)
			{
				m_currentReadyAnim = AvailableReadyAnims[Random.Range(0, AvailableReadyAnims.Count)];
			}
			if (AvailableAttackAnims.Count > 0)
			{
				m_currentAttackAnim = AvailableAttackAnims[Random.Range(0, AvailableAttackAnims.Count)];
			}
			m_collisionLife = Random.Range(CollisionLife.x, CollisionLife.y);
			m_backupType = Type;
			if (O.MP.IsMeleeWeapon && O.MP.CanNewStab)
			{
				m_shouldBeAbleToStab = true;
			}
		}

		private void UpdateDestructionRenderers()
		{
			float num = 1f - Life / m_startingLife;
			num *= (float)DestructionRenderers.Length;
			int value = Mathf.RoundToInt(num);
			value = Mathf.Clamp(value, 0, DestructionRenderers.Length - 1);
			if (m_currentDestructionRenderer == value)
			{
				return;
			}
			m_currentDestructionRenderer = value;
			for (int i = 0; i < DestructionRenderers.Length; i++)
			{
				if (i == m_currentDestructionRenderer)
				{
					DestructionRenderers[i].enabled = true;
				}
				else
				{
					DestructionRenderers[i].enabled = false;
				}
			}
		}

		public void BotPickup(Sosig S)
		{
			O.SetAllCollidersToLayer(triggersToo: false, "AgentMeleeWeapon");
			if (O.MP.IsMeleeWeapon)
			{
				for (int i = 0; i < S.Links.Count; i++)
				{
					O.MP.IgnoreRBs.Add(S.Links[i].R);
				}
			}
		}

		public void BotDrop()
		{
			if (O.MP.IsMeleeWeapon)
			{
				O.MP.IgnoreRBs.Clear();
			}
		}

		public void PlayerPickup()
		{
			if (!m_hasClamped)
			{
				if (m_ammoClampOnPlayerPickup)
				{
					m_hasClamped = true;
					int max = Mathf.RoundToInt(Random.Range(AmmoClampRange.x, AmmoClampRange.y));
					m_shotsLeft = Mathf.Clamp(m_shotsLeft, 0, max);
				}
				if (O.MP.IsMeleeWeapon)
				{
					O.MP.IgnoreRBs.Clear();
				}
			}
		}

		public bool IsUsable()
		{
			if (O.MP.IsJointedToObject)
			{
				return false;
			}
			if (Type != 0)
			{
				return true;
			}
			if (m_shotsLeft > 0)
			{
				return true;
			}
			return false;
		}

		public void SetIgnoreRBs(List<Rigidbody> rbs)
		{
			for (int i = 0; i < rbs.Count; i++)
			{
				m_ignoreRBs.Add(rbs[i]);
				if (m_hasImpactController)
				{
					ImpactController.IgnoreRBs.Add(rbs[i]);
				}
			}
		}

		public void ClearRBs()
		{
			m_ignoreRBs.Clear();
			if (m_hasImpactController)
			{
				ImpactController.IgnoreRBs.Clear();
			}
		}

		public void SetAutoDestroy(bool b)
		{
			m_setToAutoDestroy = b;
		}

		public void Update()
		{
			if (m_shouldShatter)
			{
				Shatter();
			}
			if (timeSinceFired < 10f)
			{
				timeSinceFired += 10f;
			}
			if (DamRefire > 0f)
			{
				DamRefire -= Time.deltaTime;
			}
			if (O.IsHeld)
			{
				m_autoDestroyTickDown = 30f;
			}
			if (DoesCastForPlayerHitBox && m_isThrownTagCastTag && Physics.Linecast(ThrownCastPos1.position, ThrownCastPos2.position, out m_thrownHit, LM_Thrown, QueryTriggerInteraction.Collide))
			{
				Debug.Log("yup");
				m_isThrownTagCastTag = false;
				IFVRDamageable component = m_thrownHit.collider.gameObject.GetComponent<IFVRDamageable>();
				Damage damage = new Damage();
				if (component != null)
				{
					damage.Class = FistVR.Damage.DamageClass.Melee;
					damage.point = m_hit.point;
					damage.hitNormal = m_hit.normal;
					damage.strikeDir = O.RootRigidbody.velocity.normalized;
					damage.damageSize = 0.02f;
					float num = Mathf.Clamp(O.RootRigidbody.velocity.magnitude, 2f, 10f);
					switch (MeleeType)
					{
					case SosigMeleeWeaponType.Stabbing:
						damage.Dam_Blunt = 5f * num;
						damage.Dam_Piercing = 40f * num;
						damage.Dam_Cutting = 25f * num;
						damage.Dam_TotalKinetic = 75f * num;
						break;
					case SosigMeleeWeaponType.Blunt:
						damage.Dam_Blunt = 35f * num;
						damage.Dam_Piercing = 10f * num;
						damage.Dam_TotalKinetic = 45f * num;
						break;
					case SosigMeleeWeaponType.Bladed:
						damage.Dam_Blunt = 5f * num;
						damage.Dam_Cutting = 75f * num;
						damage.Dam_Piercing = 5f * num;
						damage.Dam_TotalKinetic = 80f * num;
						break;
					case SosigMeleeWeaponType.Shield:
						damage.Dam_Blunt = 45f * num;
						damage.Dam_Cutting = 10f * num;
						damage.Dam_Piercing = 5f * num;
						damage.Dam_TotalKinetic = 60f * num;
						break;
					}
					if (component != null)
					{
						component.Damage(damage);
						DamRefire = 0.1f;
					}
				}
			}
			if (UsesSustainedSound)
			{
				float sustainEnergy = m_sustainEnergy;
				m_sustainEnergy -= Time.deltaTime;
				if (m_sustainEnergy <= 0f && sustainEnergy > 0f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_SustainedTerminate, base.transform.position);
					if (AudSource_SustainedLoop.isPlaying)
					{
						AudSource_SustainedLoop.Stop();
					}
				}
			}
			if (m_shouldBeAbleToStab)
			{
				if (O.IsHeld || IsHeldByBot)
				{
					O.MP.CanNewStab = true;
				}
				if (IsInBotInventory)
				{
					O.MP.CanNewStab = false;
					O.MP.DeJoint();
				}
			}
			if (m_setToAutoDestroy && !O.IsHeld && !IsHeldByBot && !IsInBotInventory && O.QuickbeltSlot == null)
			{
				m_autoDestroyTickDown -= Time.deltaTime;
				if (m_autoDestroyTickDown < 0f)
				{
					Object.Destroy(base.gameObject);
				}
			}
			float num2 = 1f;
			if (O.IsHeld)
			{
				num2 = 0.2f;
			}
			if (m_isFuseTickingDown)
			{
				GrenadeFuseTime -= Time.deltaTime * num2;
				if (UsesFusePulse)
				{
					float f = Mathf.Clamp(1f - GrenadeFuseTime / m_startFuseTime, 0f, 1f);
					f = Mathf.Pow(f, 2f);
					if (m_fuse_tick <= 0f)
					{
						m_fuse_tick = Mathf.Lerp(m_fuse_StartRefire, m_fuse_EndRefire, f);
						float num3 = Mathf.Lerp(m_fuse_PitchStart, m_fuse_PitchEnd, f);
						SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.Generic, AudEvent_FusePulse, base.transform.position, new Vector2(0.3f, 0.3f), new Vector2(num3, num3), Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
						FusePSystem.Emit(2);
					}
					else
					{
						m_fuse_tick -= Time.deltaTime;
					}
				}
				if (GrenadeFuseTime <= 0f)
				{
					Explode();
				}
			}
			switch (LayerState)
			{
			case SosigWeaponLayerState.Agent:
				if (O.IsHeld)
				{
					O.SetAllCollidersToLayer(triggersToo: false, "Default");
					LayerState = SosigWeaponLayerState.Player;
				}
				break;
			case SosigWeaponLayerState.Player:
				if (!O.IsHeld && O.QuickbeltSlot == null)
				{
					LayerState = SosigWeaponLayerState.Agent;
					O.SetAllCollidersToLayer(triggersToo: false, "AgentMeleeWeapon");
				}
				break;
			}
			if ((O.IsHeld || O.QuickbeltSlot != null) && IsHeldByBot && HandHoldingThis != null)
			{
				HandHoldingThis.DropHeldObject();
			}
			if (O.IsHeld || IsHeldByBot || IsInBotInventory || O.QuickbeltSlot != null || m_isFuseTickingDown)
			{
				E.IFFCode = -3;
			}
			else
			{
				E.IFFCode = -2;
			}
			if (Type == SosigWeaponType.Gun)
			{
				Update_Gun();
				UpdateRecoil();
			}
			if (IsHeldByBot || IsInBotInventory)
			{
				O.DistantGrabbable = false;
			}
			else
			{
				O.DistantGrabbable = true;
			}
			if (m_hasLaser)
			{
				if (IsHeldByBot || O.IsHeld)
				{
					if (!Laser.activeSelf)
					{
						Laser.SetActive(value: true);
					}
				}
				else if (Laser.activeSelf)
				{
					Laser.SetActive(value: false);
				}
			}
			lastPos = base.transform.position;
		}

		private void Explode()
		{
			if (m_hasExploded)
			{
				return;
			}
			m_hasExploded = true;
			for (int i = 0; i < SpawnOnExplode.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(SpawnOnExplode[i], base.transform.position + Vector3.up * 0.1f, Quaternion.identity);
				Explosion component = gameObject.GetComponent<Explosion>();
				if (component != null)
				{
					component.IFF = SourceIFF;
				}
				ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
				if (component2 != null)
				{
					component2.IFF = SourceIFF;
				}
				Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
				if (component3 != null)
				{
					component3.velocity = O.RootRigidbody.velocity;
				}
			}
			Object.Destroy(base.gameObject);
		}

		public void FuseGrenade()
		{
			if ((Type == SosigWeaponType.Grenade || PrimesOnFirstSwing) && !m_isFuseTickingDown)
			{
				Obstacle.enabled = true;
				m_isFuseTickingDown = true;
			}
		}

		private void Update_Gun()
		{
			switch (MechaState)
			{
			case SosigWeaponMechaState.CyclingBack:
				m_cycleLerp += Time.deltaTime * CycleSpeedBackward;
				if (HasCyclingPart)
				{
					SetAnimatedComponent(CyclingPart, Mathf.Lerp(CycleForwardBackValues.x, CycleForwardBackValues.y, m_cycleLerp), CycleInterpStyle, CycleAxis);
				}
				if (!(m_cycleLerp >= 1f))
				{
					break;
				}
				m_cycleLerp = 1f;
				if (m_shotsLeft <= 0)
				{
					if (!O.IsHeld && IsHeldByBot && UsageState != SosigWeaponUsageState.Reloading)
					{
						if (!SosigHoldingThis.Inventory.HasAmmo((int)AmmoType))
						{
							HandHoldingThis.DropHeldObject();
							return;
						}
						UsageState = SosigWeaponUsageState.Reloading;
						if (UsesReloadingSounds && GunShotProfile.Reloading.Clips.Count > 0 && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < MinHandlingDistance)
						{
							SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.Reloading, base.transform.position);
						}
					}
				}
				else
				{
					if (UsesCyclingSounds && GunShotProfile.EjectionBack.Clips.Count > 0 && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < MinHandlingDistance)
					{
						SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.EjectionBack, base.transform.position);
					}
					MechaState = SosigWeaponMechaState.CyclingForward;
				}
				break;
			case SosigWeaponMechaState.CyclingForward:
				m_cycleLerp -= Time.deltaTime * CycleSpeedForward;
				if (HasCyclingPart)
				{
					SetAnimatedComponent(CyclingPart, Mathf.Lerp(CycleForwardBackValues.x, CycleForwardBackValues.y, m_cycleLerp), CycleInterpStyle, CycleAxis);
				}
				if (m_cycleLerp <= 0f)
				{
					m_cycleLerp = 0f;
					MechaState = SosigWeaponMechaState.ReadyToFire;
					if (UsesCyclingSounds && GunShotProfile.EjectionForward.Clips.Count > 0 && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position) < MinHandlingDistance)
					{
						SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.EjectionForward, base.transform.position);
					}
				}
				break;
			}
			if (HasCyclingPart && MechaState != SosigWeaponMechaState.CyclingBack && MechaState != SosigWeaponMechaState.CyclingForward && m_shotsLeft <= 0)
			{
				SetAnimatedComponent(CyclingPart, CycleForwardBackValues.y, CycleInterpStyle, CycleAxis);
			}
			SosigWeaponUsageState usageState = UsageState;
			if (usageState != SosigWeaponUsageState.Reloading)
			{
				return;
			}
			m_reloadingLerp += Time.deltaTime;
			if (m_reloadingLerp > ReloadTime && IsHeldByBot)
			{
				MechaState = SosigWeaponMechaState.CyclingForward;
				m_reloadingLerp = 0f;
				UsageState = SosigWeaponUsageState.Firing;
				if (SosigHoldingThis.Inventory.HasAmmo((int)AmmoType))
				{
					m_shotsLeft = SosigHoldingThis.Inventory.ReloadFromType((int)AmmoType, ShotsPerLoad);
				}
				if (m_shotsLeft <= 0)
				{
					HandHoldingThis.DropHeldObject();
				}
			}
		}

		private float GetCurrentAngularLimit(float rangeToTarget)
		{
			float time = rangeToTarget / 100f;
			return AngularFiringLimitByRange.Evaluate(time) * MaxAngularFireRange;
		}

		public bool GetFireClear(Vector3 source, Vector3 dir, float range, float distToTarget)
		{
			bool flag = true;
			if (Physics.Raycast(source, dir, out m_hit, range, LM_IFFCheck, QueryTriggerInteraction.Ignore))
			{
				AIEntityIFFBeacon component = m_hit.collider.attachedRigidbody.GetComponent<AIEntityIFFBeacon>();
				if (SosigHoldingThis != null && component != null && component.IFF == SosigHoldingThis.E.IFFCode)
				{
					return false;
				}
			}
			float maxDistance = Mathf.Min(distToTarget, 4f);
			if (Physics.Raycast(source, dir, out m_hit, maxDistance, LM_EnvCheck, QueryTriggerInteraction.Ignore))
			{
				return false;
			}
			return true;
		}

		private void StartNewIdleAnim()
		{
			m_currentIdleAnim = AvailableIdleAnims[Random.Range(0, AvailableIdleAnims.Count)];
			m_idleTick = 0f;
			m_currentStateSpeeds.z = Random.Range(MeleeStateSpeeds.z * 0.9f, MeleeStateSpeeds.z * 1.1f);
		}

		private void StartNewReadyAnim()
		{
			m_currentReadyAnim = AvailableReadyAnims[Random.Range(0, AvailableReadyAnims.Count)];
			m_readyLoopTick = 0f;
			m_currentStateSpeeds.x = Random.Range(MeleeStateSpeeds.x * 0.9f, MeleeStateSpeeds.x * 1.4f);
		}

		private void StartNewAttackAnim()
		{
			m_currentAttackAnim = AvailableAttackAnims[Random.Range(0, AvailableAttackAnims.Count)];
			m_attackLerpTick = Random.Range(-2f, -0.8f);
			attackHoldLimit = Random.Range(1f, 1.8f);
			m_currentStateSpeeds.y = Random.Range(MeleeStateSpeeds.y * 0.9f, MeleeStateSpeeds.y * 1.1f);
			if (PrimesOnFirstSwing && !m_isFuseTickingDown)
			{
				Pin.ForceExpelPin();
			}
		}

		private void AbortAttack()
		{
			if (MeleeState == SosigMeleeState.Attacking)
			{
				MeleeState = SosigMeleeState.Ready;
				StartNewReadyAnim();
			}
		}

		public void UseMelee(Sosig.SosigObjectUsageFocus usage, bool isActive, Vector3 targetPoint)
		{
			if (IsAlsoThrowable && Vector3.Distance(base.transform.position, targetPoint) > 5f)
			{
				Type = SosigWeaponType.Grenade;
			}
			switch (usage)
			{
			case Sosig.SosigObjectUsageFocus.MaintainHeldAtRest:
				MeleeIdleCycle(isActive);
				break;
			case Sosig.SosigObjectUsageFocus.AimAtReady:
				MeleeIdleCycle(isActive);
				break;
			case Sosig.SosigObjectUsageFocus.AttackTarget:
				MeleeAttackCycle(targetPoint);
				break;
			}
		}

		public void MeleeIdleCycle(bool isActive)
		{
			if (IsHeldByBot)
			{
				if (m_currentIdleAnim == null)
				{
					StartNewIdleAnim();
				}
				MeleeState = SosigMeleeState.Ready;
				m_readyLoopTick = 0f;
				Vector3 zero = Vector3.zero;
				Vector3 forward = Vector3.forward;
				Vector3 up = Vector3.up;
				m_idleTick += Time.deltaTime * m_currentStateSpeeds.z;
				if (!isActive)
				{
					m_idleTick = 0f;
				}
				zero = m_currentIdleAnim.GetPos(m_idleTick, doEXP: false, loop: true);
				forward = m_currentIdleAnim.GetForward(m_idleTick, doEXP: false, loop: true);
				up = m_currentIdleAnim.GetUp(m_idleTick, doEXP: false, loop: true);
				if (!HandHoldingThis.IsRightHand)
				{
					zero = Vector3.Reflect(zero, Vector3.right);
					forward = Vector3.Reflect(forward, Vector3.right);
					up = Vector3.Reflect(up, Vector3.right);
				}
				if (m_idleTick >= 1f)
				{
					StartNewIdleAnim();
				}
				Vector3 pos = HandHoldingThis.Root.TransformPoint(zero);
				Vector3 forward2 = HandHoldingThis.Root.TransformDirection(forward);
				Vector3 upwards = HandHoldingThis.Root.TransformDirection(up);
				HandHoldingThis.SetPoseDirect(pos, Quaternion.LookRotation(forward2, upwards));
			}
		}

		public void MeleeAttackCycle(Vector3 targetPoint)
		{
			if (!IsHeldByBot)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.forward;
			Vector3 vector3 = Vector3.up;
			switch (MeleeState)
			{
			case SosigMeleeState.Ready:
				if (m_currentReadyAnim == null)
				{
					StartNewReadyAnim();
				}
				m_readyLoopTick += Time.deltaTime * m_currentStateSpeeds.x;
				vector = m_currentReadyAnim.GetPos(m_readyLoopTick, doEXP: false, loop: true);
				vector2 = m_currentReadyAnim.GetForward(m_readyLoopTick, doEXP: false, loop: true);
				vector3 = m_currentReadyAnim.GetUp(m_readyLoopTick, doEXP: false, loop: true);
				if (!HandHoldingThis.IsRightHand)
				{
					vector = Vector3.Reflect(vector, Vector3.right);
					vector2 = Vector3.Reflect(vector2, Vector3.right);
					vector3 = Vector3.Reflect(vector3, Vector3.right);
				}
				if (m_readyLoopTick >= readyAbort)
				{
					m_readyLoopTick = 0f;
					MeleeState = SosigMeleeState.Attacking;
					StartNewAttackAnim();
					float num2 = 1f - readyAbort;
					m_attackLerpTick += num2;
				}
				break;
			case SosigMeleeState.Attacking:
				if (m_currentAttackAnim == null)
				{
					StartNewAttackAnim();
				}
				m_attackLerpTick += Time.deltaTime * m_currentStateSpeeds.y;
				vector = m_currentAttackAnim.GetPos(m_attackLerpTick, doEXP: true, loop: false);
				vector2 = m_currentAttackAnim.GetForward(m_attackLerpTick, doEXP: true, loop: false);
				vector3 = m_currentAttackAnim.GetUp(m_attackLerpTick, doEXP: true, loop: false);
				if (!HandHoldingThis.IsRightHand)
				{
					vector = Vector3.Reflect(vector, Vector3.right);
					vector2 = Vector3.Reflect(vector2, Vector3.right);
					vector3 = Vector3.Reflect(vector3, Vector3.right);
				}
				if (m_attackLerpTick >= attackHoldLimit)
				{
					float num = Random.Range(0f, 1f);
					if (num > 0.9f)
					{
						MeleeState = SosigMeleeState.Attacking;
						StartNewAttackAnim();
						m_attackLerpTick = Random.Range(-0.8f, -0.3f);
					}
					else if (num > 0.6f)
					{
						readyAbort = Random.Range(0.2f, 0.8f);
						m_attackLerpTick = 0f;
						MeleeState = SosigMeleeState.Ready;
						StartNewReadyAnim();
					}
					else
					{
						readyAbort = 1f;
						m_attackLerpTick = 0f;
						MeleeState = SosigMeleeState.Ready;
						StartNewReadyAnim();
					}
				}
				break;
			}
			Vector3 pos = HandHoldingThis.Root.TransformPoint(vector);
			Vector3 forward = HandHoldingThis.Root.TransformDirection(vector2);
			Vector3 upwards = HandHoldingThis.Root.TransformDirection(vector3);
			checkHeightTick -= Time.deltaTime;
			if (checkHeightTick <= 0f)
			{
				checkHeightTick = Random.Range(0.25f, 0.7f);
				float num3 = Mathf.Clamp(Mathf.Abs(targetPoint.y - HandHoldingThis.Root.position.y), 0f, 0.2f);
				if (targetPoint.y < HandHoldingThis.Root.position.y && num3 > 0.1f)
				{
					heightOffset = 0f - num3;
				}
				else if (targetPoint.y > HandHoldingThis.Root.position.y && num3 > 0.1f)
				{
					heightOffset = num3 * 0.3f;
				}
				else
				{
					heightOffset = 0f;
				}
			}
			if (DoesHeightAdjust)
			{
				pos += Vector3.up * heightOffset;
			}
			HandHoldingThis.SetPoseDirect(pos, Quaternion.LookRotation(forward, upwards));
		}

		public bool TryToThrowAt(Vector3 targetPoint, bool isReadyToThrow)
		{
			if (IsAlsoThrowable && Vector3.Distance(base.transform.position, targetPoint) < 4f)
			{
				Type = m_backupType;
			}
			if (!isReadyToThrow)
			{
				return false;
			}
			float num = Vector3.Distance(HandHoldingThis.Target.position, base.transform.position);
			if (num > 0.15f)
			{
				return false;
			}
			Vector3 vector = targetPoint - base.transform.position;
			int num2 = 0;
			Vector3 s;
			Vector3 s2;
			if (vector.magnitude < 12f)
			{
				vector = vector.normalized * 14f;
				s = vector;
				s2 = vector;
				num2 = 1;
			}
			else
			{
				num2 = fts.solve_ballistic_arc(base.transform.position, Vector3.Distance(base.transform.position, targetPoint), targetPoint, Mathf.Abs(Physics.gravity.y), out s, out s2);
			}
			if (num2 > 0 && Pin != null)
			{
				Pin.ForceExpelPin();
			}
			bool flag = false;
			if (num2 > 0)
			{
				Vector3 vector2 = s;
				if (num2 > 1)
				{
					vector2 = s2;
				}
				if (IsHeldByBot)
				{
					SourceIFF = SosigHoldingThis.E.IFFCode;
				}
				else if (O.IsHeld)
				{
					SourceIFF = GM.CurrentPlayerBody.GetPlayerIFF();
				}
				HandHoldingThis.ThrowObject(s * 1f, targetPoint);
				if (DoesCastForPlayerHitBox)
				{
					m_isThrownTagCastTag = true;
				}
				return true;
			}
			return false;
		}

		public void TryToFireGun(Vector3 targetPos, bool isPanicFiring, bool targetPointIdentified, bool isClutching, float recoilMult, bool isHipfiring)
		{
			if (UsageState != 0)
			{
				return;
			}
			if (m_refireTick > 0f)
			{
				m_refireTick -= Time.deltaTime;
			}
			else
			{
				if (!targetPointIdentified && !isPanicFiring)
				{
					return;
				}
				Vector3 vector = targetPos - Muzzle.position;
				float magnitude = vector.magnitude;
				if (!(magnitude > RangeMinMax.y) || isPanicFiring)
				{
					float num = GetCurrentAngularLimit(magnitude);
					if (isHipfiring)
					{
						num *= 4f;
					}
					float num2 = Vector3.Angle(Vector3.ProjectOnPlane(vector, Vector3.up), Vector3.ProjectOnPlane(Muzzle.forward, Vector3.up));
					float num3 = Vector3.Angle(Vector3.ProjectOnPlane(vector, base.transform.right), Vector3.ProjectOnPlane(Muzzle.forward, base.transform.right));
					float b = 0.5f;
					if (UsesBurstLimit && m_shotsLeftTilBurstLimit < 2)
					{
						b = 5f;
					}
					if (magnitude < 4f)
					{
						b = 5f;
					}
					if (magnitude < 2f)
					{
						b = 10f;
					}
					if (magnitude < 1f)
					{
						b = num;
					}
					bool flag = false;
					if (isClutching && timeSinceFired < 0.25f)
					{
						flag = true;
					}
					if (((!(num2 > num) && !(num3 > Mathf.Min(num, b))) || isPanicFiring || flag) && (isPanicFiring || flag || GetFireClear(Muzzle.position, Muzzle.forward, RangeMinMax.y, magnitude)))
					{
						FireGun(recoilMult);
					}
				}
			}
		}

		private void UpdateRecoil()
		{
			bool flag = false;
			bool flag2 = false;
			if (m_recoilX > 0f)
			{
				m_recoilX = Mathf.MoveTowards(m_recoilX, 0f, Time.deltaTime * RecoilPerShot.y * 4f);
				flag = true;
			}
			if (Mathf.Abs(m_recoilY) > 0f)
			{
				m_recoilY = Mathf.MoveTowards(m_recoilY, 0f, Time.deltaTime * RecoilPerShot.y * 3f * 0.5f);
				flag = true;
			}
			if (m_recoilLinear > 0f)
			{
				m_recoilLinear = Mathf.MoveTowards(m_recoilLinear, 0f, Time.deltaTime * RecoilPerShot.w * 1f);
				flag2 = true;
			}
			if (flag2)
			{
				RecoilHolder.localPosition = new Vector3(0f, 0f, m_recoilLinear);
			}
			if (flag)
			{
				RecoilHolder.localEulerAngles = new Vector3(m_recoilX, m_recoilY, 0f);
			}
		}

		private void Recoil(float recoilMult)
		{
			m_recoilX += RecoilPerShot.x * recoilMult;
			m_recoilY += Random.Range(RecoilPerShot.x * 0.5f, (0f - RecoilPerShot.x) * 0.5f) * recoilMult;
			m_recoilLinear += RecoilPerShot.z;
			m_recoilX = Mathf.Clamp(m_recoilX, 0f, RecoilPerShot.y);
			m_recoilY = Mathf.Clamp(m_recoilY, 0f, RecoilPerShot.y * 0.2f);
			m_recoilLinear = Mathf.Clamp(m_recoilLinear, 0f, RecoilPerShot.w);
			RecoilHolder.localPosition = new Vector3(0f, 0f, m_recoilLinear);
			RecoilHolder.localEulerAngles = new Vector3(m_recoilX, m_recoilY, 0f);
		}

		public bool FireGun(float recoilMult)
		{
			if (m_shotsLeft <= 0)
			{
				return false;
			}
			if (MechaState != 0)
			{
				return false;
			}
			if (IsHeldByBot)
			{
				SosigHoldingThis.E.ProcessLoudness(ShotLoudness);
			}
			if (Muzzle == null)
			{
				return false;
			}
			timeSinceFired = 0f;
			if (UsesSustainedSound)
			{
				if (m_sustainEnergy <= 0f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_SustainedEmit, base.transform.position);
				}
				if (!AudSource_SustainedLoop.isPlaying)
				{
					AudSource_SustainedLoop.Play();
				}
				m_sustainEnergy = SustainEnergyPerShot;
			}
			for (int i = 0; i < ProjectilesPerShot; i++)
			{
				GameObject gameObject = Object.Instantiate(Projectile, Muzzle.position, Muzzle.rotation);
				gameObject.transform.Rotate(new Vector3(Random.Range(0f - ProjectileSpread, ProjectileSpread), Random.Range(0f - ProjectileSpread, ProjectileSpread), 0f));
				BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
				component.FlightVelocityMultiplier = FlightVelocityMultiplier;
				float num = component.MuzzleVelocityBase;
				if (IsHeldByBot && SosigHoldingThis.isDamPowerUp)
				{
					num *= SosigHoldingThis.BuffIntensity_DamPowerUpDown;
				}
				component.Fire(num, gameObject.transform.forward, null);
				if (IsHeldByBot)
				{
					component.SetSource_IFF(SosigHoldingThis.E.IFFCode);
				}
				else if (O.IsHeld)
				{
					component.SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
				}
			}
			Recoil(recoilMult);
			if (GunShotProfile != null)
			{
				FVRSoundEnvironment se = PlayShotEvent(Muzzle.position);
				float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(se);
				if (IsHeldByBot)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(ShotLoudness, ShotLoudness * soundTravelDistanceMultByEnvironment, base.transform.position, SosigHoldingThis.E.IFFCode);
				}
				else if (O.IsHeld)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(ShotLoudness, ShotLoudness * soundTravelDistanceMultByEnvironment, base.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
				}
			}
			if (UsesMuzzleFire)
			{
				for (int j = 0; j < PSystemsMuzzle.Length; j++)
				{
					PSystemsMuzzle[j].Emit(MuzzlePAmount);
				}
			}
			if (DoesFlashOnFire)
			{
				FXM.InitiateMuzzleFlash(Muzzle.position, Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
			}
			if (IsHeldByBot)
			{
				if (!SosigHoldingThis.IsInfiniteAmmo)
				{
					if (SosigHoldingThis.IsAmmoDrain)
					{
						m_shotsLeft = 0;
					}
					else
					{
						m_shotsLeft--;
					}
				}
			}
			else if (!GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				if (GM.CurrentPlayerBody.IsAmmoDrain)
				{
					m_shotsLeft = 0;
				}
				else
				{
					m_shotsLeft--;
				}
			}
			m_shotsSoFarThisCycle++;
			if (m_shotsSoFarThisCycle >= ShotsPerCycle)
			{
				MechaState = SosigWeaponMechaState.CyclingBack;
				m_cycleLerp = 0f;
				m_shotsSoFarThisCycle = 0;
			}
			if (UsesBurstLimit)
			{
				m_shotsLeftTilBurstLimit--;
			}
			m_refireTick = Random.Range(Usage_RefireRange.x, Usage_RefireRange.y);
			if (m_shotsLeftTilBurstLimit <= 0)
			{
				m_shotsLeftTilBurstLimit = Random.Range(BurstLimit, BurstLimit + 1 + BurstLimitRange);
				m_refireTick += Random.Range(BurstDelayRange.x, BurstDelayRange.y);
			}
			return true;
		}

		private FVRSoundEnvironment PlayShotEvent(Vector3 source)
		{
			float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
			float delay = num / 343f;
			FVRSoundEnvironment environment = SM.GetReverbEnvironment(base.transform.position).Environment;
			wwBotWurstGunSoundConfig.BotGunShotSet shotSet = GetShotSet(environment);
			if (num < 20f)
			{
				SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotNear, shotSet.ShotSet_Near, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
			}
			else if (num < 100f)
			{
				float num2 = Mathf.Lerp(0.4f, 0.2f, (num - 20f) / 80f);
				SM.PlayCoreSoundDelayedOverrides(vol: new Vector2(num2 * 0.95f, num2), type: FVRPooledAudioType.NPCShotFarDistant, ClipSet: shotSet.ShotSet_Far, pos: source, pitch: shotSet.ShotSet_Distant.PitchRange, delay: delay);
			}
			else
			{
				SM.PlayCoreSoundDelayedOverrides(FVRPooledAudioType.NPCShotFarDistant, shotSet.ShotSet_Distant, source, shotSet.ShotSet_Distant.VolumeRange, shotSet.ShotSet_Distant.PitchRange, delay);
			}
			return environment;
		}

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
		{
			return m_shotDic[e];
		}

		private void OnCollisionEnter(Collision col)
		{
			ProcessCollision(col);
			m_isThrownTagCastTag = false;
		}

		private void Shatter()
		{
			if (!m_isShattered)
			{
				m_isShattered = true;
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Shatter, base.transform.position);
				for (int i = 0; i < ShardPrefabs.Count; i++)
				{
					GameObject gameObject = Object.Instantiate(ShardPrefabs[i], ShardPositions[i].position, ShardPositions[i].rotation);
					Rigidbody component = gameObject.GetComponent<Rigidbody>();
					component.velocity = O.RootRigidbody.velocity + Random.onUnitSphere * 0.2f;
				}
				Object.Destroy(base.gameObject);
			}
		}

		private void ProcessCollision(Collision col)
		{
			bool flag = false;
			if (IsInBotInventory)
			{
				return;
			}
			if (col.collider.attachedRigidbody != null)
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			if (IsDestructible && O.IsHeld)
			{
				m_collisionLife -= col.relativeVelocity.magnitude;
				if (m_collisionLife <= 0f)
				{
					m_shouldShatter = true;
				}
			}
			if ((IsHeldByBot && SosigHoldingThis.IgnoreRBs.Contains(col.collider.attachedRigidbody)) || IsInBotInventory)
			{
				return;
			}
			if (Type == SosigWeaponType.Melee)
			{
				if (IsHeldByBot && MeleeState == SosigMeleeState.Attacking)
				{
					DoMeleeDamageInCollision(col);
					if (LayerMask.LayerToName(col.collider.gameObject.layer) != "AgentBody")
					{
						AbortAttack();
					}
				}
				else if (O.IsHeld)
				{
					DoMeleeDamageInCollision(col);
				}
				else if (!IsHeldByBot && !O.IsHeld && O.RootRigidbody.velocity.magnitude > 2f)
				{
					DoMeleeDamageInCollision(col);
				}
			}
			float magnitude = col.relativeVelocity.magnitude;
			float num = 6f;
			if (Type == SosigWeaponType.Melee)
			{
				num = 15f;
			}
			if (MeleeState == SosigMeleeState.Attacking)
			{
				num = 25f;
			}
			if (IsDroppedOnStrongHit && col.relativeVelocity.magnitude > num && IsHeldByBot && LayerMask.LayerToName(col.collider.gameObject.layer) != "AgentBody" && HandHoldingThis.S.DoesDropWeaponsOnBallistic)
			{
				HandHoldingThis.DropHeldObject();
			}
		}

		private void DoMeleeDamageInCollision(Collision col)
		{
			if (DamRefire > 0f)
			{
				return;
			}
			IFVRDamageable iFVRDamageable = null;
			iFVRDamageable = col.collider.transform.GetComponent<IFVRDamageable>();
			if (iFVRDamageable == null)
			{
				iFVRDamageable = col.collider.attachedRigidbody.GetComponent<IFVRDamageable>();
			}
			Damage damage = new Damage();
			damage.Class = FistVR.Damage.DamageClass.Melee;
			damage.point = col.contacts[0].point;
			damage.hitNormal = col.contacts[0].normal;
			damage.strikeDir = O.RootRigidbody.velocity.normalized;
			damage.damageSize = 0.02f;
			float num = Mathf.Clamp(col.relativeVelocity.magnitude, 2f, 10f);
			if (IsHeldByBot && (SosigHoldingThis.IsMuscleMeat || SosigHoldingThis.IsWeakMeat))
			{
				num *= SosigHoldingThis.BuffIntensity_MuscleMeatWeak;
			}
			if (O.IsHeld)
			{
				num = Mathf.Clamp(O.m_hand.Input.VelLinearWorld.magnitude, 0f, 10f);
				if (GM.CurrentPlayerBody.IsMuscleMeat || GM.CurrentPlayerBody.IsWeakMeat)
				{
					num *= GM.CurrentPlayerBody.GetMuscleMeatPower();
				}
			}
			switch (MeleeType)
			{
			case SosigMeleeWeaponType.Stabbing:
				damage.Dam_Blunt = 5f * num;
				damage.Dam_Piercing = 40f * num;
				damage.Dam_Cutting = 25f * num;
				damage.Dam_TotalKinetic = 75f * num;
				break;
			case SosigMeleeWeaponType.Blunt:
				damage.Dam_Blunt = 35f * num;
				damage.Dam_Piercing = 10f * num;
				damage.Dam_TotalKinetic = 45f * num;
				break;
			case SosigMeleeWeaponType.Bladed:
				damage.Dam_Blunt = 5f * num;
				damage.Dam_Cutting = 75f * num;
				damage.Dam_Piercing = 5f * num;
				damage.Dam_TotalKinetic = 80f * num;
				break;
			case SosigMeleeWeaponType.Shield:
				damage.Dam_Blunt = 45f * num;
				damage.Dam_Cutting = 10f * num;
				damage.Dam_Piercing = 5f * num;
				damage.Dam_TotalKinetic = 60f * num;
				break;
			}
			if (iFVRDamageable != null)
			{
				iFVRDamageable.Damage(damage);
				DamRefire = 0.1f;
			}
		}

		public void Damage(Damage d)
		{
			if (TakesDamage)
			{
				float num = d.Dam_Blunt * ResistMultBCP.x;
				float num2 = d.Dam_Cutting * ResistMultBCP.y;
				float num3 = d.Dam_Piercing * ResistMultBCP.z;
				float num4 = num + num2 + num3;
				Life -= num4;
				if (UsesDestructionStageRenderers)
				{
					UpdateDestructionRenderers();
				}
				if (Life < 0f)
				{
					Shatter();
				}
			}
			if (d.Class != FistVR.Damage.DamageClass.Melee || Type != SosigWeaponType.Melee)
			{
				if (IsDroppedOnStrongHit && IsHeldByBot && d.Dam_TotalKinetic > 600f && HandHoldingThis.S.DoesDropWeaponsOnBallistic)
				{
					HandHoldingThis.DropHeldObject();
				}
				if (IsDroppedOnStrongHit && IsInBotInventory && d.Dam_TotalKinetic > 1000f && InventorySlotWithThis.I.S.DoesDropWeaponsOnBallistic)
				{
					InventorySlotWithThis.DetachHeldObject();
				}
				if (Type == SosigWeaponType.Grenade && d.Dam_Thermal > 5f)
				{
					FuseGrenade();
					GrenadeFuseTime = Random.Range(GrenadeFuseTime * 0.1f, GrenadeFuseTime * 0.25f);
				}
			}
		}

		private void PrimeDics()
		{
			if (!(GunShotProfile != null))
			{
				return;
			}
			for (int i = 0; i < GunShotProfile.ShotSets.Count; i++)
			{
				for (int j = 0; j < GunShotProfile.ShotSets[i].EnvironmentsUsed.Count; j++)
				{
					m_shotDic.Add(GunShotProfile.ShotSets[i].EnvironmentsUsed[j], GunShotProfile.ShotSets[i]);
				}
			}
		}

		public void SetAnimatedComponent(Transform t, float val, InterpStyle interp, Axis axis)
		{
			switch (interp)
			{
			case InterpStyle.Rotation:
			{
				Vector3 zero = Vector3.zero;
				switch (axis)
				{
				case Axis.X:
					zero.x = val;
					break;
				case Axis.Y:
					zero.y = val;
					break;
				case Axis.Z:
					zero.z = val;
					break;
				}
				t.localEulerAngles = zero;
				break;
			}
			case InterpStyle.Translate:
			{
				Vector3 localPosition = t.localPosition;
				switch (axis)
				{
				case Axis.X:
					localPosition.x = val;
					break;
				case Axis.Y:
					localPosition.y = val;
					break;
				case Axis.Z:
					localPosition.z = val;
					break;
				}
				t.localPosition = localPosition;
				break;
			}
			}
		}
	}
}
