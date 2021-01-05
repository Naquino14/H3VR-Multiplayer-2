using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class wwBotWurstModernGun : MonoBehaviour
	{
		public enum FiringState
		{
			ReadyToFire,
			Firing,
			EjectionBack,
			EjectionForward,
			GoingToReload,
			Reloading,
			RecoveringFromReload
		}

		public wwBotWurst Bot;

		public FiringState FireState;

		[Header("Pose Vars")]
		public List<Transform> FiringPoses = new List<Transform>();

		private int m_curFiringPose;

		private int m_prevFiringPose;

		public Transform ReloadingPose;

		public Transform Rig_Gun;

		public Transform Rig_ReciprocatingPiece;

		private Vector3 gunPos;

		private Quaternion gunRot = Quaternion.identity;

		public bool UsesReciprocatingPiece = true;

		public float EjectionReciprocatingZ;

		[Header("Timing Vars")]
		public Vector2 Timer_RateLimiter;

		public Vector2 Timer_EjectionDelay;

		public float Timer_EjectionBack;

		public float Timer_EjectionForward;

		public float Timer_GoingToReload;

		public Vector2 Timer_ReloadTime;

		public float Timer_RecoveringFromReload;

		public bool UsesBurst;

		public int BurstCountMin = 3;

		public int BurstCountMax = 3;

		private int m_burstLimit;

		public Vector2 BurstAddDelay;

		private int m_burstCount;

		private float m_tick;

		private bool m_fireAtWill;

		[Header("Gun Vars")]
		public int AmmoCapacity;

		public bool CyclesOnLastRound = true;

		private int m_shotsLeft;

		[Header("AudioEvents")]
		public wwBotWurstGunSoundConfig GunShotProfile;

		private bool m_hasProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		public float MinHandlingDistance = 15f;

		[Header("Firing Vars")]
		public Transform Muzzle;

		public float AccuracyRange = 1f;

		public GameObject Projectile;

		public int NumProjectiles = 1;

		private bool m_usesFastProjectile;

		public bool UsesMuzzleFire;

		public ParticleSystem[] PSystemsMuzzle;

		public int MuzzlePAmount;

		public bool DoesFlashOnFire;

		[Header("PoseTest Vars")]
		private float m_timeSincePoseChange;

		private int m_nextPoseToTestToTarget;

		public LayerMask LM_VisCheck;

		private RaycastHit m_hit;

		private List<bool> m_validSightPoses = new List<bool>();

		private float angToTarget;

		public void SetUseFastProjectile(bool b)
		{
			m_usesFastProjectile = b;
		}

		private void Awake()
		{
			m_tick = Random.Range(Timer_RateLimiter.x, Timer_RateLimiter.y);
			m_shotsLeft = AmmoCapacity;
			gunPos = FiringPoses[0].localPosition;
			m_burstLimit = Random.Range(BurstCountMin, BurstCountMax);
			for (int i = 0; i < FiringPoses.Count; i++)
			{
				m_validSightPoses.Add(item: false);
			}
			if (GunShotProfile != null)
			{
				m_hasProfile = true;
			}
			if (m_hasProfile)
			{
				PrimeDics();
			}
		}

		private void Start()
		{
			Rig_Gun.position = FiringPoses[m_curFiringPose].position;
			Rig_Gun.rotation = FiringPoses[m_curFiringPose].rotation;
		}

		private void Update()
		{
			UpdateGunHandlingPose();
			UpdateGun();
		}

		public void SetFireAtWill(bool b)
		{
			m_fireAtWill = b;
		}

		public void UpdateGunHandlingPose()
		{
			if (Bot.State != wwBotWurst.BotState.Fighting && Bot.State != wwBotWurst.BotState.Searching)
			{
				return;
			}
			if (m_timeSincePoseChange <= 1f)
			{
				gunPos = Vector3.Lerp(FiringPoses[m_prevFiringPose].localPosition, FiringPoses[m_curFiringPose].localPosition, m_timeSincePoseChange);
				if (FireState != FiringState.GoingToReload && FireState != FiringState.Reloading && FireState != FiringState.RecoveringFromReload)
				{
					Rig_Gun.localPosition = gunPos;
				}
			}
			Vector3 position = FiringPoses[m_nextPoseToTestToTarget].position;
			Vector3 vector = Bot.LastPlaceTargetSeen + Random.onUnitSphere * 0.1f;
			Vector3 vector2 = vector - position;
			if (!Physics.Raycast(position, vector2.normalized, out m_hit, Mathf.Min(vector2.magnitude, Bot.MaxViewDistance), LM_VisCheck, QueryTriggerInteraction.Ignore))
			{
				m_validSightPoses[m_nextPoseToTestToTarget] = true;
			}
			else
			{
				m_validSightPoses[m_nextPoseToTestToTarget] = false;
			}
			m_nextPoseToTestToTarget++;
			if (m_nextPoseToTestToTarget >= m_validSightPoses.Count)
			{
				m_nextPoseToTestToTarget = 0;
			}
			if (m_timeSincePoseChange < 1f)
			{
				m_timeSincePoseChange += Time.deltaTime * 3f;
			}
			else
			{
				if (m_validSightPoses[m_curFiringPose])
				{
					return;
				}
				int num = -1;
				for (int i = 0; i < m_validSightPoses.Count; i++)
				{
					if (m_validSightPoses[i])
					{
						num = i;
						break;
					}
				}
				if (num > -1)
				{
					m_prevFiringPose = m_curFiringPose;
					m_curFiringPose = num;
					m_timeSincePoseChange = 0f;
				}
			}
		}

		public void UpdateGun()
		{
			if (Bot == null || Bot.RB_Bottom == null || Bot.RB_Torso == null)
			{
				return;
			}
			if (Bot.State == wwBotWurst.BotState.Fighting)
			{
				Vector3 vector = Bot.LastPlaceTargetSeen - Rig_Gun.position;
				Vector3 vector2 = Vector3.ProjectOnPlane(vector, Bot.RB_Bottom.transform.right);
				Vector3 normalized = Vector3.RotateTowards(Bot.RB_Torso.transform.forward, vector2, 1.396264f, 0f).normalized;
				Vector3 normalized2 = Vector3.RotateTowards(normalized, vector, Bot.MaxFiringAngle * 0.0174533f, 0f).normalized;
				Vector3 vector3 = Vector3.ProjectOnPlane(vector, Bot.RB_Bottom.transform.up);
				Vector3 normalized3 = Vector3.RotateTowards(Bot.RB_Torso.transform.forward, vector3, Bot.MaxFiringAngle * 0.0174533f, 0f).normalized;
				Debug.DrawLine(base.transform.position, base.transform.position + vector.normalized * 20f, Color.red);
				Debug.DrawLine(base.transform.position, base.transform.position + vector2 * 20f, Color.blue);
				Debug.DrawLine(base.transform.position, base.transform.position + normalized * 20f, Color.green);
				Vector3 vector4 = Vector3.Cross(normalized3, normalized);
				Debug.DrawLine(base.transform.position, base.transform.position + normalized2 * 20f, Color.yellow);
				Quaternion b = Quaternion.LookRotation(normalized2, Bot.RB_Torso.transform.up);
				gunRot = Quaternion.Slerp(gunRot, b, Time.deltaTime * 2f);
				angToTarget = Vector3.Angle(vector3, Vector3.ProjectOnPlane(base.transform.forward, Vector3.up));
			}
			else
			{
				gunRot = Quaternion.Slerp(gunRot, base.transform.rotation, Time.deltaTime * 2f);
			}
			if (FireState != FiringState.GoingToReload && FireState != FiringState.RecoveringFromReload && FireState != FiringState.Reloading)
			{
				Rig_Gun.rotation = gunRot;
			}
			switch (FireState)
			{
			case FiringState.ReadyToFire:
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
				}
				else if (m_fireAtWill && angToTarget < Bot.Config.AngularFiringRange && Bot.GetFireClear(Muzzle.position, Muzzle.position + base.transform.forward * 30f))
				{
					Fire();
					if (UsesBurst)
					{
						m_burstCount++;
					}
					m_shotsLeft--;
					FireState = FiringState.Firing;
					m_tick = Random.Range(Timer_EjectionDelay.x, Timer_EjectionDelay.y);
				}
				break;
			case FiringState.Firing:
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
					break;
				}
				if (m_shotsLeft <= 0 && !CyclesOnLastRound)
				{
					FireState = FiringState.GoingToReload;
					m_tick = Timer_GoingToReload;
					if (m_hasProfile && GunShotProfile.GoingToReload.Clips.Count > 0)
					{
						float num2 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
						if (num2 < MinHandlingDistance)
						{
							SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.GoingToReload, base.transform.position);
						}
					}
					break;
				}
				FireState = FiringState.EjectionBack;
				m_tick = Timer_EjectionBack;
				if (m_hasProfile && GunShotProfile.EjectionBack.Clips.Count > 0)
				{
					float num3 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
					if (num3 < MinHandlingDistance)
					{
						SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.EjectionBack, base.transform.position);
					}
				}
				break;
			case FiringState.EjectionBack:
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
				}
				if (m_tick < 0f)
				{
					m_tick = 0f;
				}
				if (UsesReciprocatingPiece)
				{
					float t4 = 1f - m_tick / Timer_EjectionBack;
					float z2 = Mathf.Lerp(0f, EjectionReciprocatingZ, t4);
					Rig_ReciprocatingPiece.localPosition = new Vector3(0f, 0f, z2);
				}
				if (!(m_tick <= 0f))
				{
					break;
				}
				m_tick = Timer_EjectionForward;
				FireState = FiringState.EjectionForward;
				if (m_hasProfile && GunShotProfile.EjectionForward.Clips.Count > 0)
				{
					float num7 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
					if (num7 < MinHandlingDistance)
					{
						SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.EjectionForward, base.transform.position);
					}
				}
				break;
			case FiringState.EjectionForward:
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
				}
				if (m_tick < 0f)
				{
					m_tick = 0f;
				}
				if (UsesReciprocatingPiece)
				{
					float t3 = 1f - m_tick / Timer_EjectionBack;
					float z = Mathf.Lerp(EjectionReciprocatingZ, 0f, t3);
					Rig_ReciprocatingPiece.localPosition = new Vector3(0f, 0f, z);
				}
				if (!(m_tick <= 0f))
				{
					break;
				}
				if (m_shotsLeft <= 0)
				{
					m_tick = Timer_GoingToReload;
					FireState = FiringState.GoingToReload;
					if (m_hasProfile && GunShotProfile.GoingToReload.Clips.Count > 0)
					{
						float num5 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
						if (num5 < MinHandlingDistance)
						{
							SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.GoingToReload, base.transform.position);
						}
					}
				}
				else
				{
					float num6 = 0f;
					if (UsesBurst && m_burstCount > m_burstLimit)
					{
						num6 = Random.Range(BurstAddDelay.x, BurstAddDelay.y);
						m_burstLimit = Random.Range(BurstCountMin, BurstCountMax);
						m_burstCount = 0;
					}
					m_tick = Random.Range(Timer_RateLimiter.x, Timer_RateLimiter.y) + num6;
					FireState = FiringState.ReadyToFire;
				}
				break;
			case FiringState.GoingToReload:
			{
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
				}
				if (m_tick < 0f)
				{
					m_tick = 0f;
				}
				float t2 = 1f - m_tick / Timer_GoingToReload;
				Vector3 localPosition2 = Vector3.Lerp(gunPos, ReloadingPose.localPosition, t2);
				Rig_Gun.localPosition = localPosition2;
				Quaternion rotation2 = Quaternion.Slerp(gunRot, ReloadingPose.rotation, t2);
				Rig_Gun.rotation = rotation2;
				if (!(m_tick <= 0f))
				{
					break;
				}
				m_tick = Random.Range(Timer_ReloadTime.x, Timer_ReloadTime.y);
				FireState = FiringState.Reloading;
				if (m_hasProfile && GunShotProfile.Reloading.Clips.Count > 0)
				{
					float num4 = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
					if (num4 < MinHandlingDistance)
					{
						SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.Reloading, base.transform.position);
					}
				}
				break;
			}
			case FiringState.Reloading:
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
					break;
				}
				m_tick = Timer_RecoveringFromReload;
				FireState = FiringState.RecoveringFromReload;
				m_shotsLeft = AmmoCapacity;
				if (m_hasProfile && GunShotProfile.RecoveringFromReload.Clips.Count > 0)
				{
					float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
					if (num < MinHandlingDistance)
					{
						SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.RecoveringFromReload, base.transform.position);
					}
				}
				break;
			case FiringState.RecoveringFromReload:
			{
				if (m_tick > 0f)
				{
					m_tick -= Time.deltaTime;
				}
				if (m_tick < 0f)
				{
					m_tick = 0f;
				}
				float t = 1f - m_tick / Timer_RecoveringFromReload;
				Vector3 localPosition = Vector3.Lerp(ReloadingPose.localPosition, gunPos, t);
				Rig_Gun.localPosition = localPosition;
				Quaternion rotation = Quaternion.Slerp(ReloadingPose.rotation, gunRot, t);
				Rig_Gun.rotation = rotation;
				if (m_tick <= 0f)
				{
					if (UsesBurst)
					{
						m_burstCount = 0;
					}
					m_tick = Random.Range(Timer_RateLimiter.x, Timer_RateLimiter.y);
					FireState = FiringState.ReadyToFire;
				}
				break;
			}
			}
		}

		private void Fire()
		{
			if (m_hasProfile)
			{
				PlayShotEvent(Muzzle.position);
			}
			GM.CurrentSceneSettings.OnBotShotFired(this);
			GameObject projectile = Projectile;
			for (int i = 0; i < NumProjectiles; i++)
			{
				Muzzle.localEulerAngles = new Vector3(Random.Range(0f - AccuracyRange, AccuracyRange), Random.Range(0f - AccuracyRange, AccuracyRange), 0f);
				GameObject gameObject = Object.Instantiate(projectile, Muzzle.position, Muzzle.rotation);
				if (!m_usesFastProjectile)
				{
					gameObject.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = 0.05f;
				}
				gameObject.GetComponent<BallisticProjectile>().Fire(Muzzle.forward, null);
			}
			if (UsesMuzzleFire)
			{
				for (int j = 0; j < PSystemsMuzzle.Length; j++)
				{
					PSystemsMuzzle[j].Emit(MuzzlePAmount);
				}
				if (DoesFlashOnFire)
				{
					FXM.InitiateMuzzleFlash(Muzzle.position, Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
				}
			}
		}

		private void PlayShotEvent(Vector3 source)
		{
			float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
			float delay = num / 343f;
			wwBotWurstGunSoundConfig.BotGunShotSet shotSet = GetShotSet(SM.GetSoundEnvironment(base.transform.position));
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

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
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
}
