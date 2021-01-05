using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AutoMeater : MonoBehaviour, IFVRDamageable
	{
		public enum AutoMeaterState
		{
			Static,
			Idle,
			Alert,
			Engaging,
			Dead
		}

		public enum AMHitZoneType
		{
			FireControl,
			Magazine,
			Motor,
			TargetPriority,
			Generator
		}

		[Serializable]
		public class AutoMeaterFireControl
		{
			private AutoMeater M;

			public List<AutoMeaterFirearm> Firearms;

			private bool m_isDestroyed;

			private float m_disruptedTick;

			public void Init(AutoMeater m)
			{
				M = m;
				for (int i = 0; i < Firearms.Count; i++)
				{
					Firearms[i].Init(M, this);
				}
			}

			public void DisruptSystem(float f)
			{
				m_disruptedTick = Mathf.Max(m_disruptedTick, f);
			}

			public void SetUseFastProjectile(bool b)
			{
				for (int i = 0; i < Firearms.Count; i++)
				{
					Firearms[i].SetUseFastProjectile(b);
				}
			}

			public void Tick(float t, bool fireAtWill, float angleToTargetHoriz, float angleToTargetVertical, float distToTarget)
			{
				if (m_disruptedTick > 0f)
				{
					m_disruptedTick -= Time.deltaTime;
				}
				for (int i = 0; i < Firearms.Count; i++)
				{
					if (!m_isDestroyed)
					{
						if (m_disruptedTick > 0f)
						{
							int value = Mathf.FloorToInt(UnityEngine.Random.Range(0f, 1.9f));
							Firearms[i].SetFireAtWill(Convert.ToBoolean(value), UnityEngine.Random.Range(0f, 100f));
						}
						if (Firearms[i].FiringAngleLimit > angleToTargetHoriz && Firearms[i].FiringAngleLimitVertical > angleToTargetVertical && Firearms[i].RangeLimit >= distToTarget)
						{
							Firearms[i].SetFireAtWill(fireAtWill, distToTarget);
						}
						else
						{
							Firearms[i].SetFireAtWill(b: false, distToTarget);
						}
					}
					Firearms[i].Tick(t);
				}
			}

			public void DestroySystem()
			{
				for (int i = 0; i < Firearms.Count; i++)
				{
					Firearms[i].SetFireAtWill(b: false, 400f);
				}
				m_isDestroyed = true;
			}
		}

		[Serializable]
		public class AutoMeaterMotor
		{
			private AutoMeater M;

			private Transform m_base;

			private Transform m_sideToSideTransform;

			private Transform m_upAndDownMotor;

			private HingeJoint m_hingeJoint;

			private HingeJoint m_upDownJoint;

			private float m_sideMotorMaxSpeed;

			private float m_upDownMotorMaxSpeed;

			private float m_maxVerticalRot;

			private float m_currentSpeedMagnitude;

			private bool m_isDestroyed;

			private float m_disruptedTick;

			private bool usesUpDownHinger;

			public void Init(AutoMeater m, Transform baseTransform, HingeJoint sideToSideHinge, HingeJoint upDownHinge, Transform sideMotor, Transform upDownMotor, float sideMotorMaxSpeed, float upDownMotorMaxSpeed, float maxVerticalRot)
			{
				M = m;
				m_base = baseTransform;
				m_upAndDownMotor = upDownMotor;
				m_sideToSideTransform = sideMotor;
				m_hingeJoint = sideToSideHinge;
				m_upDownJoint = upDownHinge;
				if (m_upDownJoint != null)
				{
					usesUpDownHinger = true;
				}
				m_sideMotorMaxSpeed = sideMotorMaxSpeed;
				m_upDownMotorMaxSpeed = upDownMotorMaxSpeed;
				m_maxVerticalRot = maxVerticalRot;
			}

			public void DisruptSystem(float f)
			{
				m_disruptedTick = Mathf.Max(m_disruptedTick, f);
			}

			public void DestroySystem()
			{
				m_isDestroyed = true;
			}

			public void SetMaxSpeedMagnitude(float f)
			{
				m_currentSpeedMagnitude = Mathf.Clamp(f, 0f, 1f);
			}

			public void RotateToFacePoint(Vector3 p)
			{
				if (m_isDestroyed)
				{
					return;
				}
				if (m_disruptedTick > 0f)
				{
					m_disruptedTick -= Time.deltaTime;
				}
				Vector3 vector = ((!M.m_usesUpDownTransform) ? (p - M.transform.position) : (p - m_upAndDownMotor.position));
				Debug.DrawLine(m_sideToSideTransform.position, p, Color.yellow);
				if (m_disruptedTick > 0f)
				{
					vector = Vector3.Slerp(vector, UnityEngine.Random.onUnitSphere, 0.9f);
				}
				Vector3 vector2 = Vector3.ProjectOnPlane(vector, m_base.up);
				Vector3 forward = m_base.forward;
				float targetPosition = Mathf.Atan2(Vector3.Dot(m_base.up, Vector3.Cross(forward, vector2)), Vector3.Dot(forward, vector2)) * 57.29578f;
				JointSpring spring = m_hingeJoint.spring;
				spring.targetPosition = targetPosition;
				m_hingeJoint.spring = spring;
				m_sideToSideTransform.rotation = Quaternion.LookRotation(m_hingeJoint.transform.forward, m_base.up);
				Debug.DrawLine(m_sideToSideTransform.position, m_sideToSideTransform.position + vector2, Color.cyan);
				if (M.m_usesUpDownTransform)
				{
					Vector3 target = Vector3.ProjectOnPlane(vector, m_upAndDownMotor.right);
					Vector3 target2 = Vector3.RotateTowards(m_sideToSideTransform.forward, target, m_maxVerticalRot * ((float)Math.PI / 180f), 1f);
					Vector3 vector3 = ((!usesUpDownHinger) ? Vector3.RotateTowards(m_upAndDownMotor.forward, target2, (float)Math.PI / 180f * m_upDownMotorMaxSpeed * m_currentSpeedMagnitude * Time.deltaTime, 1f) : Vector3.RotateTowards(m_sideToSideTransform.forward, target2, (float)Math.PI / 180f * m_upDownMotorMaxSpeed * m_currentSpeedMagnitude * Time.deltaTime, 1f));
					Debug.DrawLine(m_sideToSideTransform.position, m_sideToSideTransform.position + vector3, Color.green);
					if (usesUpDownHinger)
					{
						Vector3 forward2 = m_sideToSideTransform.forward;
						float targetPosition2 = Mathf.Atan2(Vector3.Dot(m_sideToSideTransform.right, Vector3.Cross(forward2, vector3)), Vector3.Dot(forward2, vector3)) * 57.29578f;
						JointSpring spring2 = m_upDownJoint.spring;
						spring.targetPosition = targetPosition2;
						m_upDownJoint.spring = spring;
						m_upAndDownMotor.rotation = Quaternion.LookRotation(m_upDownJoint.transform.forward, m_base.up);
					}
					else
					{
						m_upAndDownMotor.rotation = Quaternion.LookRotation(vector3, m_base.up);
					}
				}
			}
		}

		[Serializable]
		public class AutoMeaterFirearm
		{
			public enum FiringState
			{
				FiringBurst,
				Cooldown
			}

			public AutoMeaterFirearmSoundProfile GunShotProfile;

			private Dictionary<FVRSoundEnvironment, AutoMeaterFirearmSoundProfile.GunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, AutoMeaterFirearmSoundProfile.GunShotSet>();

			private bool m_hasProfile;

			private AutoMeater M;

			private AutoMeaterFireControl FC;

			public FiringState State;

			private float m_refireTick;

			private float m_cooldownTick;

			private bool m_fireAtWill;

			private float m_distToTarget = 1f;

			public Transform Muzzle;

			public GameObject Projectile;

			public int NumProjectiles = 1;

			public Vector2 RefireCycle;

			public Vector2 BurstCooldownRange;

			public int BurstCountMin = 1;

			public int BurstCountMax = 1;

			private int m_burstsLeftToFire = 1;

			public float FiringAngleLimit = 5f;

			public float FiringAngleLimitVertical = 20f;

			public float AccuracyRange = 1f;

			public float RangeLimit = 150f;

			private bool m_usesFastProjectile;

			public bool UsesMuzzleFire;

			public ParticleSystem[] PSystemsMuzzle;

			public int MuzzlePAmount;

			public bool DoesFlashOnFire;

			public bool ExplodesOnEmpty;

			[Header("MagazineSystem")]
			public int Ammo = 1000;

			public int StartingAmmo = 1000;

			public bool UsesRefillMag;

			public bool HasMag = true;

			public GameObject EmptyMagazinePrefab;

			public Transform MagazineEjectPos;

			public List<GameObject> MagazineProxy;

			public AudioEvent AudEvent_Eject;

			[Header("FlameThrower Config")]
			public bool IsFlameThrower;

			public AudioEvent AudEvent_Ignite;

			public AudioEvent AudEvent_Extinguish;

			public AudioSource AudSource_FireLoop;

			private float m_hasBeenFiring;

			private bool m_hasFiredStartSound;

			private bool m_isFiring;

			public ParticleSystem FireParticles;

			public Vector2 FireWidthRange;

			public Vector2 SpeedRangeMin;

			public Vector2 SpeedRangeMax;

			public Vector2 SizeRangeMin;

			public Vector2 SizeRangeMax;

			public void SetUseFastProjectile(bool b)
			{
				m_usesFastProjectile = b;
			}

			public void Init(AutoMeater m, AutoMeaterFireControl f)
			{
				M = m;
				FC = f;
				if (GunShotProfile != null)
				{
					m_hasProfile = true;
				}
				if (m_hasProfile)
				{
					PrimeDics();
				}
			}

			public void Load()
			{
				HasMag = true;
				Ammo = StartingAmmo;
				if (MagazineProxy != null)
				{
					for (int i = 0; i < MagazineProxy.Count; i++)
					{
						MagazineProxy[i].SetActive(value: true);
					}
				}
			}

			public void EjectMag()
			{
				if (!UsesRefillMag || !HasMag || EmptyMagazinePrefab == null)
				{
					return;
				}
				if (MagazineProxy != null)
				{
					for (int i = 0; i < MagazineProxy.Count; i++)
					{
						MagazineProxy[i].SetActive(value: false);
					}
				}
				Ammo = 0;
				HasMag = false;
				GameObject gameObject = UnityEngine.Object.Instantiate(EmptyMagazinePrefab, MagazineEjectPos.position, MagazineEjectPos.rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				component.velocity = M.transform.up + UnityEngine.Random.onUnitSphere * 0.2f * 2.5f;
				component.angularVelocity = UnityEngine.Random.onUnitSphere * 3f;
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Eject, MagazineEjectPos.position);
			}

			public void SetFireAtWill(bool b, float d)
			{
				m_fireAtWill = b;
				m_distToTarget = d;
				if (!m_fireAtWill)
				{
					m_cooldownTick = BurstCooldownRange.x * 0.5f;
					State = FiringState.Cooldown;
				}
			}

			private void UpdateFlameThrower(float t)
			{
				if (m_fireAtWill && m_distToTarget <= RangeLimit)
				{
					m_isFiring = true;
					if (m_hasBeenFiring < 2f)
					{
						m_hasBeenFiring += Time.deltaTime;
					}
					if (!m_hasFiredStartSound)
					{
						m_hasFiredStartSound = true;
						SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Ignite, Muzzle.position);
					}
					float volume = Mathf.Clamp(m_hasBeenFiring * 2f, 0f, 0.4f);
					AudSource_FireLoop.volume = volume;
					float t2 = m_distToTarget / RangeLimit;
					AudSource_FireLoop.pitch = Mathf.Lerp(0.5f, 1.5f, t2);
					if (!AudSource_FireLoop.isPlaying)
					{
						AudSource_FireLoop.Play();
					}
				}
				else
				{
					m_hasFiredStartSound = false;
					m_hasBeenFiring = 0f;
					StopFiring();
				}
			}

			private void UpdateFire()
			{
				ParticleSystem.EmissionModule emission = FireParticles.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				if (m_isFiring)
				{
					rateOverTime.mode = ParticleSystemCurveMode.Constant;
					rateOverTime.constantMax = 40f;
					rateOverTime.constantMin = 40f;
					float num = Mathf.Clamp(m_distToTarget / RangeLimit, 0f, 1f);
					num = 1f - num;
					num *= num;
					num *= num;
					ParticleSystem.MainModule main = FireParticles.main;
					ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
					startSpeed.mode = ParticleSystemCurveMode.TwoConstants;
					startSpeed.constantMax = Mathf.Lerp(SpeedRangeMax.x, SpeedRangeMax.y, num);
					startSpeed.constantMin = Mathf.Lerp(SpeedRangeMin.x, SpeedRangeMin.y, num);
					main.startSpeed = startSpeed;
					ParticleSystem.MinMaxCurve startSize = main.startSize;
					startSize.mode = ParticleSystemCurveMode.TwoConstants;
					startSize.constantMax = Mathf.Lerp(SizeRangeMax.x, SizeRangeMax.y, num);
					startSize.constantMin = Mathf.Lerp(SizeRangeMin.x, SizeRangeMin.y, num);
					main.startSize = startSize;
					ParticleSystem.ShapeModule shape = FireParticles.shape;
					shape.angle = Mathf.Lerp(FireWidthRange.x, FireWidthRange.y, num);
				}
				else
				{
					rateOverTime.mode = ParticleSystemCurveMode.Constant;
					rateOverTime.constantMax = 0f;
					rateOverTime.constantMin = 0f;
				}
				emission.rateOverTime = rateOverTime;
			}

			private void StopFiring()
			{
				if (m_isFiring)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Extinguish, Muzzle.position);
					AudSource_FireLoop.Stop();
					AudSource_FireLoop.volume = 0f;
				}
				m_isFiring = false;
				m_hasFiredStartSound = false;
			}

			public void Tick(float t)
			{
				if (IsFlameThrower)
				{
					UpdateFlameThrower(t);
					UpdateFire();
					return;
				}
				switch (State)
				{
				case FiringState.FiringBurst:
					if (m_refireTick > 0f)
					{
						m_refireTick -= Time.deltaTime;
					}
					else if (m_burstsLeftToFire > 0)
					{
						if (Ammo < 1 && UsesRefillMag && HasMag)
						{
							EjectMag();
						}
						if (m_fireAtWill && Ammo > 0 && (HasMag || !UsesRefillMag))
						{
							FireShot();
							m_burstsLeftToFire--;
							m_refireTick = UnityEngine.Random.Range(RefireCycle.x, RefireCycle.y);
							Ammo--;
							if (Ammo <= 0 && ExplodesOnEmpty)
							{
								M.KillMe();
							}
						}
					}
					else
					{
						State = FiringState.Cooldown;
						m_cooldownTick = UnityEngine.Random.Range(BurstCooldownRange.x, BurstCooldownRange.y);
					}
					break;
				case FiringState.Cooldown:
					if (m_cooldownTick > 0f)
					{
						m_cooldownTick -= t;
						break;
					}
					State = FiringState.FiringBurst;
					m_burstsLeftToFire = UnityEngine.Random.Range(BurstCountMin, BurstCountMax + 1);
					m_refireTick = 0f;
					break;
				}
			}

			private void FireShot()
			{
				if (m_hasProfile)
				{
					PlayShotEvent(Muzzle.position);
				}
				GameObject projectile = Projectile;
				for (int i = 0; i < NumProjectiles; i++)
				{
					Muzzle.localEulerAngles = new Vector3(UnityEngine.Random.Range(0f - AccuracyRange, AccuracyRange), UnityEngine.Random.Range(0f - AccuracyRange, AccuracyRange), 0f);
					GameObject gameObject = UnityEngine.Object.Instantiate(projectile, Muzzle.position, Muzzle.rotation);
					if (!m_usesFastProjectile)
					{
						gameObject.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = 0.2f;
					}
					gameObject.GetComponent<BallisticProjectile>().Fire(Muzzle.forward, null);
					gameObject.GetComponent<BallisticProjectile>().Source_IFF = M.E.IFFCode;
				}
				if (UsesMuzzleFire)
				{
					for (int j = 0; j < PSystemsMuzzle.Length; j++)
					{
						PSystemsMuzzle[j].Emit(MuzzlePAmount);
					}
					if (DoesFlashOnFire)
					{
						FXM.InitiateMuzzleFlashLowPriority(Muzzle.position, Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
					}
				}
			}

			private void PlayShotEvent(Vector3 source)
			{
				float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
				float delay = num / 343f;
				AutoMeaterFirearmSoundProfile.GunShotSet shotSet = GetShotSet(SM.GetSoundEnvironment(M.transform.position));
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
			}

			private AutoMeaterFirearmSoundProfile.GunShotSet GetShotSet(FVRSoundEnvironment e)
			{
				return m_shotDic[e];
			}

			private void PrimeDics()
			{
				for (int i = 0; i < GunShotProfile.ShotSets.Count; i++)
				{
					for (int j = 0; j < GunShotProfile.ShotSets[i].EnvironmentsUsed.Count; j++)
					{
						m_shotDic.Add(GunShotProfile.ShotSets[i].EnvironmentsUsed[j], GunShotProfile.ShotSets[i]);
					}
				}
			}
		}

		[Serializable]
		public class AutoMeaterFlightSystem
		{
			private AutoMeater M;

			private Rigidbody RB;

			private bool m_isDestroyed;

			public void Init(AutoMeater m, Rigidbody rb)
			{
				M = m;
				RB = rb;
			}

			public void OrientToFacePoint(Vector3 p, float speedMultiplier)
			{
				if (!m_isDestroyed)
				{
					Quaternion to = Quaternion.LookRotation(p - M.transform.position, Vector3.Cross(M.transform.forward, Vector3.up));
					if (M.AttemptsToRam)
					{
						to = Quaternion.LookRotation(p - M.transform.position, Vector3.up);
					}
					M.m_targRot = Quaternion.RotateTowards(M.m_targRot, to, 180f * speedMultiplier * Time.deltaTime);
				}
			}

			public void FlyTowardsPoint(Vector3 p, float speedMultiplier, float distanceThreshold)
			{
				if (!m_isDestroyed)
				{
					bool flag = false;
					if (Physics.Raycast(M.transform.position, -Vector3.up, out M.m_hit, M.m_minHeight, M.LM_Flight, QueryTriggerInteraction.Ignore))
					{
						flag = true;
						p.y = M.m_hit.point.y + M.m_minHeight;
					}
					if (Vector3.Distance(p, M.m_targPos) > distanceThreshold)
					{
						M.m_targPos = Vector3.MoveTowards(M.m_targPos, p, M.MaxFlightSpeed * Time.deltaTime * speedMultiplier);
						return;
					}
					Vector3 vector = (p - M.transform.position).normalized * distanceThreshold;
					Vector3 target = p - vector;
					target.y = p.y;
					M.m_targPos = Vector3.MoveTowards(M.m_targPos, target, M.MaxFlightSpeed * Time.deltaTime * speedMultiplier);
				}
			}
		}

		private AutoMeaterState m_state;

		public AIEntity E;

		public FVRPhysicalObject PO;

		public Rigidbody RB;

		public float HeightOffGround = 0.185f;

		public float MaxBlindFireTime = 1f;

		public GameObject[] DisableOnDeathState;

		public bool SetGunsToFast;

		public float IdleRandomAngleRange = 45f;

		public Vector2 IdleNewLookTime = new Vector2(5f, 10f);

		public float AlertCoolDownTime = 30f;

		[Header("--Vocalizations System--")]
		public AudioEvent Vocal_Alerted;

		public AudioEvent Vocal_StandDownToIdle;

		public AudioEvent Vocal_StandDownToAlert;

		public AudioEvent Vocal_Scream;

		public float MaxVocalSpeed = 1f;

		private float m_timeSinceVocal = 10f;

		[Header("--Target Priority System--")]
		public AITargetPrioritySystem Priority;

		[Header("--Fire Control System--")]
		public AutoMeaterFireControl FireControl;

		[Header("--Motor System--")]
		public AutoMeaterMotor Motor;

		[Header("--Flight System--")]
		public AutoMeaterFlightSystem FlightSystem;

		public bool UsesFlightSystem;

		public Vector2 IdleNewDestinationTime = new Vector2(5f, 10f);

		public float MaxFlightSpeed = 4f;

		public float Radius = 0.5f;

		public LayerMask LM_Flight;

		private RaycastHit m_hit;

		public bool UsesBlades;

		public List<AutoMeaterBlade> Blades;

		public bool AttemptsToRam;

		private bool m_hasPriority;

		private bool m_hasFireControl;

		private bool m_hasMotor;

		[Header("--References--")]
		public Transform SideToSideTransform;

		public Transform UpDownTransform;

		public HingeJoint SideToSideHinge;

		public HingeJoint UpDownHinge;

		public GameObject TargetLaser;

		private bool m_usesUpDownTransform;

		private bool m_usesUpDownHinge;

		public LayerMask LM_FriendlyFireCheck;

		public AutoMeaterHitZone TerminalHitZone;

		[Header("Configparams")]
		public float sideMotorSpeed = 720f;

		public float updownMotorSpeed = 360f;

		public float updownRotClamp = 45f;

		[Header("DestructionShards")]
		public bool UsesDestructionShards;

		public List<Transform> ShardPoints;

		public List<GameObject> Shards;

		private bool m_hasSpawnedShards;

		private bool m_isTickingDownToRemove;

		private float m_removeTickDown = 5f;

		private Vector3 m_idleLookPoint;

		private float m_idleNewLookCountDown;

		private Vector3 m_idleDestination;

		private float m_idleDestinationCountDown;

		private float m_alertCountDown = 30f;

		private float m_dodgeTickDown = 0.2f;

		private Vector3 m_dodgeDir;

		private float m_minHeight = 0.5f;

		protected float AttachedRotationMultiplier = 60f;

		protected float AttachedPositionMultiplier = 9000f;

		protected float AttachedRotationFudge = 1000f;

		protected float AttachedPositionFudge = 1000f;

		private bool m_controlledMovement;

		public Vector3 m_targPos;

		public Quaternion m_targRot;

		private float m_flightRecoveryTime;

		private void Start()
		{
			E.AIEventReceiveEvent += EventReceive;
			if (Priority != null)
			{
				m_hasPriority = true;
				Priority.Init(E, 5, 5f, 3f);
			}
			if (FireControl != null)
			{
				m_hasFireControl = true;
				FireControl.Init(this);
			}
			if (Motor != null)
			{
				m_hasMotor = true;
				Motor.Init(this, base.transform, SideToSideHinge, UpDownHinge, SideToSideTransform, UpDownTransform, sideMotorSpeed, updownMotorSpeed, updownRotClamp);
			}
			if (UpDownTransform != null)
			{
				m_usesUpDownTransform = true;
			}
			if (UpDownHinge != null)
			{
				m_usesUpDownHinge = true;
			}
			if (UsesFlightSystem)
			{
				FlightSystem.Init(this, RB);
				m_targPos = base.transform.position;
				m_targRot = base.transform.rotation;
			}
			SetState(AutoMeaterState.Idle);
			if (UsesDestructionShards && E.IFFCode == 1 && (GM.CurrentPlayerBody.GetPlayerIFF() == 0 || GM.CurrentPlayerBody.GetPlayerIFF() == -3))
			{
				E.IFFCode = GM.CurrentPlayerBody.GetPlayerIFF();
			}
			if (SetGunsToFast)
			{
				for (int i = 0; i < FireControl.Firearms.Count; i++)
				{
					FireControl.Firearms[i].SetUseFastProjectile(b: true);
				}
			}
			if (GM.TNH_Manager != null)
			{
				GM.TNH_Manager.AddToMiscEnemies(base.gameObject);
			}
		}

		public void KillMe()
		{
			if (TerminalHitZone != null)
			{
				TerminalHitZone.BlowUp();
			}
		}

		public void TickDownToClear(float f)
		{
			m_isTickingDownToRemove = true;
			m_removeTickDown = f;
		}

		public void Vocalize(AudioEvent e, float rangeLimit)
		{
			if (!(m_timeSinceVocal < MaxVocalSpeed) && e.Clips.Count > 0)
			{
				m_timeSinceVocal = 0f;
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				float delay = num / 343f;
				if (num < rangeLimit)
				{
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.NPCBarks, e, base.transform.position, delay);
				}
			}
		}

		public void SetUseFastProjectile(bool b)
		{
			if (FireControl != null)
			{
				FireControl.SetUseFastProjectile(b);
			}
		}

		public void DamageEvent(Vector3 p, float value, AMHitZoneType type)
		{
			if (m_state != AutoMeaterState.Dead)
			{
				AIEvent e = new AIEvent(p, AIEvent.AIEType.Damage, value);
				EventReceive(e);
				if (type == AMHitZoneType.Motor)
				{
					Motor.DisruptSystem(1f - value);
				}
			}
		}

		public void DestroyComponent(AMHitZoneType Type, GameObject SpawnThis, GameObject AndThis, Transform SpawnThisHere, bool DestroysTurret)
		{
			UnityEngine.Object.Instantiate(SpawnThis, SpawnThisHere.position, SpawnThisHere.rotation);
			if (AndThis != null)
			{
				UnityEngine.Object.Instantiate(AndThis, SpawnThisHere.position, SpawnThisHere.rotation);
			}
			if (m_state == AutoMeaterState.Dead)
			{
				return;
			}
			if (DestroysTurret)
			{
				Type = AMHitZoneType.Generator;
			}
			if (DestroysTurret && UsesDestructionShards && !m_hasSpawnedShards)
			{
				m_hasSpawnedShards = true;
				for (int i = 0; i < ShardPoints.Count; i++)
				{
					UnityEngine.Object.Instantiate(Shards[i], ShardPoints[i].position, ShardPoints[i].rotation);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
			switch (Type)
			{
			case AMHitZoneType.Magazine:
				SetState(AutoMeaterState.Dead);
				if (TargetLaser != null)
				{
					TargetLaser.SetActive(value: false);
				}
				break;
			case AMHitZoneType.FireControl:
				FireControl.DestroySystem();
				if (TargetLaser != null)
				{
					TargetLaser.SetActive(value: false);
				}
				break;
			case AMHitZoneType.Motor:
				Motor.DestroySystem();
				break;
			case AMHitZoneType.TargetPriority:
				Priority.DestroySystem();
				break;
			case AMHitZoneType.Generator:
				SetState(AutoMeaterState.Dead);
				FireControl.DestroySystem();
				Motor.DestroySystem();
				Priority.DestroySystem();
				m_isTickingDownToRemove = true;
				if (TargetLaser != null)
				{
					TargetLaser.SetActive(value: false);
				}
				break;
			}
		}

		public void EventReceive(AIEvent e)
		{
			if (m_state == AutoMeaterState.Static || (e.IsEntity && e.Entity.IFFCode == E.IFFCode))
			{
				return;
			}
			if (e.Type == AIEvent.AIEType.Damage)
			{
				if (m_state == AutoMeaterState.Idle)
				{
					SetState(AutoMeaterState.Alert);
				}
			}
			else if (e.Type == AIEvent.AIEType.Visual && m_hasPriority)
			{
				Priority.ProcessEvent(e);
				if (m_state == AutoMeaterState.Idle)
				{
					Vocalize(Vocal_Alerted, 40f);
					SetState(AutoMeaterState.Alert);
				}
			}
		}

		private void SetState(AutoMeaterState s)
		{
			if (m_state == s || m_state == AutoMeaterState.Dead)
			{
				return;
			}
			m_state = s;
			switch (m_state)
			{
			case AutoMeaterState.Static:
				break;
			case AutoMeaterState.Idle:
				GenerateNewIdleLookPoint();
				m_idleNewLookCountDown = UnityEngine.Random.Range(IdleNewLookTime.x, IdleNewLookTime.y);
				if (UsesFlightSystem)
				{
					GenerateNewIdleDestination();
					m_idleDestinationCountDown = UnityEngine.Random.Range(IdleNewDestinationTime.x, IdleNewDestinationTime.y);
					m_targPos = base.transform.position;
					m_targRot = base.transform.rotation;
				}
				break;
			case AutoMeaterState.Alert:
				m_alertCountDown = AlertCoolDownTime;
				if (UsesFlightSystem)
				{
					m_targPos = base.transform.position;
					m_targRot = base.transform.rotation;
				}
				break;
			case AutoMeaterState.Engaging:
				m_dodgeDir = UnityEngine.Random.onUnitSphere;
				m_minHeight = UnityEngine.Random.Range(0.8f, 2f);
				m_dodgeTickDown = UnityEngine.Random.Range(0.1f, 0.4f);
				break;
			case AutoMeaterState.Dead:
			{
				Vocalize(Vocal_Scream, 40f);
				m_hasPriority = false;
				m_hasMotor = false;
				m_hasFireControl = false;
				if (RB != null)
				{
					RB.useGravity = true;
				}
				for (int i = 0; i < DisableOnDeathState.Length; i++)
				{
					if (DisableOnDeathState[i] != null)
					{
						DisableOnDeathState[i].SetActive(value: false);
					}
				}
				break;
			}
			}
		}

		private void Update()
		{
			if (m_timeSinceVocal < 10f)
			{
				m_timeSinceVocal += Time.deltaTime;
			}
			switch (m_state)
			{
			case AutoMeaterState.Static:
				UpdateState_Static();
				break;
			case AutoMeaterState.Idle:
				UpdateState_Idle();
				break;
			case AutoMeaterState.Alert:
				UpdateState_Alert();
				break;
			case AutoMeaterState.Engaging:
				UpdateState_Engaging();
				break;
			}
			if (m_isTickingDownToRemove)
			{
				m_removeTickDown -= Time.deltaTime;
				if (m_removeTickDown < 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void FixedUpdate()
		{
			if (UsesFlightSystem)
			{
				UpdateFlight();
			}
		}

		private void UpdateState_Static()
		{
		}

		private void GenerateNewIdleLookPoint()
		{
			Vector3 vector = base.transform.forward * 5f;
			vector = Quaternion.AngleAxis(UnityEngine.Random.Range(0f - IdleRandomAngleRange, IdleRandomAngleRange), Vector3.up) * vector;
			m_idleLookPoint = base.transform.position + vector;
		}

		private void GenerateNewIdleDestination()
		{
			Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
			if (Physics.SphereCast(base.transform.position, Radius, onUnitSphere, out m_hit, 10f, LM_Flight, QueryTriggerInteraction.Ignore))
			{
				m_idleDestination = base.transform.position + onUnitSphere * m_hit.distance;
			}
			else
			{
				m_idleDestination = base.transform.position + onUnitSphere * 10f;
			}
		}

		private void UpdateState_Idle()
		{
			if (m_hasMotor && m_hasPriority)
			{
				Priority.Compute();
				Motor.SetMaxSpeedMagnitude(0.2f);
				Motor.RotateToFacePoint(m_idleLookPoint);
			}
			if (m_hasFireControl)
			{
				FireControl.Tick(Time.deltaTime, fireAtWill: false, 180f, 180f, 400f);
			}
			if (m_idleNewLookCountDown > 0f)
			{
				m_idleNewLookCountDown -= Time.deltaTime;
			}
			else
			{
				GenerateNewIdleLookPoint();
				m_idleNewLookCountDown = UnityEngine.Random.Range(IdleNewLookTime.x, IdleNewLookTime.y);
			}
			if (UsesFlightSystem)
			{
				if (m_idleDestinationCountDown > 0f)
				{
					m_idleDestinationCountDown -= Time.deltaTime;
				}
				else
				{
					GenerateNewIdleDestination();
					m_idleDestinationCountDown = UnityEngine.Random.Range(IdleNewDestinationTime.x, IdleNewDestinationTime.y);
				}
				FlightSystem.OrientToFacePoint(m_idleDestination, 0.2f);
				FlightSystem.FlyTowardsPoint(m_idleDestination, 0.2f, 1f);
			}
		}

		private void UpdateState_Alert()
		{
			if (m_hasMotor && m_hasPriority)
			{
				Priority.Compute();
				Motor.SetMaxSpeedMagnitude(0.5f);
				Motor.RotateToFacePoint(Priority.GetTargetPoint());
			}
			if (m_hasFireControl)
			{
				FireControl.Tick(Time.deltaTime, fireAtWill: false, 180f, 180f, 400f);
			}
			if (m_hasPriority && Priority.HasFreshTarget() && Priority.GetTimeSinceTopTargetSeen() <= MaxBlindFireTime)
			{
				Vocalize(Vocal_Alerted, 40f);
				SetState(AutoMeaterState.Engaging);
			}
			if (m_alertCountDown > 0f)
			{
				m_alertCountDown -= Time.deltaTime;
			}
			else
			{
				Vocalize(Vocal_StandDownToIdle, 20f);
				SetState(AutoMeaterState.Idle);
			}
			if (UsesFlightSystem)
			{
				Vector3 p = ((!m_hasPriority) ? (base.transform.position + Vector3.Slerp(base.transform.forward * 5f, UnityEngine.Random.onUnitSphere * 5f, 0.3f)) : Priority.GetTargetPoint());
				FlightSystem.OrientToFacePoint(p, 1f);
				FlightSystem.FlyTowardsPoint(p, 0.5f, 1f);
			}
		}

		private void UpdateState_Engaging()
		{
			if (m_hasMotor && m_hasPriority)
			{
				Priority.Compute();
				Motor.SetMaxSpeedMagnitude(1f);
				Motor.RotateToFacePoint(Priority.GetTargetPoint());
			}
			if (m_hasFireControl)
			{
				float angleToTargetHoriz = 180f;
				float angleToTargetVertical = 180f;
				float distToTarget = 400f;
				if (m_hasPriority)
				{
					if (m_usesUpDownTransform)
					{
						angleToTargetHoriz = Priority.GetAngleToHorizontal(SideToSideTransform);
						angleToTargetVertical = Priority.GetAngleToVertical(FireControl.Firearms[0].Muzzle);
					}
					else
					{
						angleToTargetHoriz = Priority.GetAngleToHorizontal(base.transform);
						angleToTargetVertical = Priority.GetAngleToVertical(base.transform);
					}
					distToTarget = Priority.GetDistanceToTarget(base.transform);
				}
				FireControl.Tick(Time.deltaTime, fireAtWill: true, angleToTargetHoriz, angleToTargetVertical, distToTarget);
			}
			if (m_hasPriority && (!Priority.HasFreshTarget() || Priority.GetTimeSinceTopTargetSeen() > MaxBlindFireTime))
			{
				Vocalize(Vocal_StandDownToAlert, 20f);
				SetState(AutoMeaterState.Alert);
			}
			if (!UsesFlightSystem)
			{
				return;
			}
			Vector3 vector = ((!m_hasPriority) ? (base.transform.position + Vector3.Slerp(base.transform.forward * 5f, UnityEngine.Random.onUnitSphere * 5f, 0.3f)) : Priority.GetTargetPoint());
			FlightSystem.OrientToFacePoint(vector, 1f);
			float distanceThreshold = 2.5f;
			if (AttemptsToRam)
			{
				distanceThreshold = 0.3f;
			}
			FlightSystem.FlyTowardsPoint(vector, 0.5f, distanceThreshold);
			if (!m_hasPriority)
			{
				return;
			}
			if (m_dodgeTickDown > 0f)
			{
				m_dodgeTickDown -= Time.deltaTime;
			}
			else
			{
				m_dodgeDir = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 2f);
				m_minHeight = UnityEngine.Random.Range(0.8f, 2f);
				m_dodgeTickDown = UnityEngine.Random.Range(0.1f, 0.4f);
			}
			if (Priority.RecentEvents.Count <= 0 || !Priority.RecentEvents[0].IsEntity)
			{
				return;
			}
			Vector3 from = base.transform.position - vector;
			float num = Vector3.Angle(from, Priority.RecentEvents[0].Entity.GetThreatFacing());
			float num2 = 35f;
			if (AttemptsToRam)
			{
				num2 = 18f;
			}
			if (num < num2)
			{
				float distanceThreshold2 = 2.5f;
				if (AttemptsToRam)
				{
					distanceThreshold2 = 2.5f;
				}
				FlightSystem.FlyTowardsPoint(vector + m_dodgeDir, 1f, distanceThreshold2);
			}
		}

		private void UpdateFlight()
		{
			if (m_state == AutoMeaterState.Dead || m_state == AutoMeaterState.Static)
			{
				if (UsesBlades)
				{
					for (int i = 0; i < Blades.Count; i++)
					{
						Blades[i].ShutDown();
					}
				}
				return;
			}
			if (m_controlledMovement)
			{
				RB.useGravity = false;
				if (PO != null)
				{
					PO.DistantGrabbable = false;
				}
				if (UsesBlades)
				{
					for (int j = 0; j < Blades.Count; j++)
					{
						Blades[j].Reactivate();
					}
				}
			}
			else
			{
				RB.useGravity = true;
				if (PO != null)
				{
					PO.DistantGrabbable = true;
				}
				if (UsesBlades)
				{
					for (int k = 0; k < Blades.Count; k++)
					{
						Blades[k].ShutDown();
					}
				}
			}
			if (!m_controlledMovement)
			{
				if (m_flightRecoveryTime > 0f)
				{
					m_flightRecoveryTime -= Time.deltaTime;
					return;
				}
				m_targPos = base.transform.position;
				m_targRot = base.transform.rotation;
				m_controlledMovement = true;
				return;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Vector3 vector = m_targPos - position;
			Quaternion quaternion = m_targRot * Quaternion.Inverse(rotation);
			float deltaTime = Time.deltaTime;
			quaternion.ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = deltaTime * angle * axis * AttachedRotationMultiplier;
				RB.angularVelocity = Vector3.MoveTowards(RB.angularVelocity, target, AttachedRotationFudge * Time.fixedDeltaTime);
			}
			Vector3 target2 = vector * AttachedPositionMultiplier * deltaTime;
			RB.velocity = Vector3.MoveTowards(RB.velocity, target2, AttachedPositionFudge * deltaTime);
		}

		private void OnCollisionEnter(Collision col)
		{
			if (UsesFlightSystem && col.collider.attachedRigidbody != null)
			{
				float magnitude = col.relativeVelocity.magnitude;
				if (magnitude > 5f)
				{
					m_controlledMovement = false;
					m_flightRecoveryTime = Mathf.Max(m_flightRecoveryTime, Mathf.Clamp(magnitude * 0.25f, 0f, 2.5f));
				}
			}
		}

		public void Damage(Damage d)
		{
			m_controlledMovement = false;
			m_flightRecoveryTime = Mathf.Max(m_flightRecoveryTime, UnityEngine.Random.Range(0.2f, 1f));
			if (d.Dam_EMP > 0f)
			{
				Motor.DisruptSystem(d.Dam_EMP);
				FireControl.DisruptSystem(d.Dam_EMP);
			}
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}
	}
}
