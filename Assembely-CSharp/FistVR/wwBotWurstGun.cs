using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class wwBotWurstGun : MonoBehaviour
	{
		public enum FiringState
		{
			firing,
			cycledown,
			cycleup,
			movingToLoad,
			reloading,
			hanging,
			recovery,
			movingToFire
		}

		public wwBotWurst Bot;

		public Transform Gun;

		public Transform Pose_Firing;

		public Transform Pose_Reloading;

		public Transform GunCyclePiece;

		public Transform CyclePose_Firing;

		public Transform CyclePose_Reloading;

		private bool m_isReloading;

		public float PoseChangeSpeed;

		private float m_poseChangeLerp;

		public bool DoesCycle;

		public float CycleSpeed;

		public float CycleStall;

		private float m_cycleStallTick = 1f;

		private float m_cycleLerp;

		private bool m_isCycleStalling;

		public float ReloadingSpeed;

		private float m_reloadingLerp;

		public float HangOnMidReloadingTime = 1f;

		private float m_hangTick = 1f;

		public FiringState FireState;

		public Vector2 Firing_RefireRange = new Vector2(0.25f, 0.5f);

		public int ShotsPerLoad = 1;

		private float m_firingTick = 0.5f;

		private int m_shotsLeft = 1;

		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		public float MinHandlingDistance = 15f;

		public Transform Muzzle;

		public float AccuracyRange = 1f;

		public GameObject Projectile;

		public int NumProjectiles = 1;

		private bool m_fireAtWill;

		public float RecoilAngle = -30f;

		private float angToTarget;

		public bool UsesMuzzleFire;

		public ParticleSystem[] PSystemsMuzzle;

		public int MuzzlePAmount;

		public bool DoesFlashOnFire;

		private void Awake()
		{
			m_shotsLeft = ShotsPerLoad;
			PrimeDics();
		}

		private void Update()
		{
			UpdateGun();
		}

		public void SetFireAtWill(bool b)
		{
			m_fireAtWill = b;
		}

		private void Fire()
		{
			PlayShotEvent(Muzzle.position);
			for (int i = 0; i < NumProjectiles; i++)
			{
				Muzzle.localEulerAngles = new Vector3(Random.Range(0f - AccuracyRange, AccuracyRange), Random.Range(0f - AccuracyRange, AccuracyRange), 0f);
				GameObject gameObject = Object.Instantiate(Projectile, Muzzle.position, Muzzle.rotation);
				gameObject.GetComponent<BallisticProjectile>().FlightVelocityMultiplier = 0.1f;
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
			Gun.localEulerAngles = new Vector3(RecoilAngle, Random.Range((0f - RecoilAngle) * 0.2f, RecoilAngle * 0.2f), 0f);
		}

		public void UpdateGun()
		{
			if (Bot == null || Bot.RB_Bottom == null || Bot.RB_Torso == null)
			{
				return;
			}
			if (Bot.State == wwBotWurst.BotState.Fighting)
			{
				Vector3 vector = Bot.LastPlaceTargetSeen - base.transform.position;
				Vector3 vector2 = Vector3.ProjectOnPlane(vector, Bot.RB_Bottom.transform.right);
				Vector3 normalized = Vector3.RotateTowards(Bot.RB_Torso.transform.forward, vector2, 1.396264f, 0f).normalized;
				Vector3 normalized2 = Vector3.RotateTowards(normalized, vector, Bot.MaxFiringAngle * 0.0174533f, 0f).normalized;
				Vector3 target = Vector3.ProjectOnPlane(vector, Bot.RB_Bottom.transform.up);
				Vector3 normalized3 = Vector3.RotateTowards(Bot.RB_Torso.transform.forward, target, Bot.MaxFiringAngle * 0.0174533f, 0f).normalized;
				Debug.DrawLine(base.transform.position, base.transform.position + vector.normalized * 20f, Color.red);
				Debug.DrawLine(base.transform.position, base.transform.position + vector2 * 20f, Color.blue);
				Debug.DrawLine(base.transform.position, base.transform.position + normalized * 20f, Color.green);
				Vector3 vector3 = Vector3.Cross(normalized3, normalized);
				Debug.DrawLine(base.transform.position, base.transform.position + normalized2 * 20f, Color.yellow);
				Quaternion b = Quaternion.LookRotation(normalized2, Bot.RB_Torso.transform.up);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 2f);
				angToTarget = Vector3.Angle(vector, base.transform.forward);
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.identity, Time.deltaTime * 2f);
			}
			switch (FireState)
			{
			case FiringState.firing:
				if (m_firingTick > 0f)
				{
					m_firingTick -= Time.deltaTime;
				}
				else if (m_fireAtWill && angToTarget < Bot.MaxFiringAngle)
				{
					Fire();
					m_shotsLeft--;
					if (m_shotsLeft > 0)
					{
						if (DoesCycle)
						{
							FireState = FiringState.cycledown;
							m_cycleStallTick = CycleStall;
							m_isCycleStalling = true;
						}
						m_firingTick = Random.Range(Firing_RefireRange.x, Firing_RefireRange.y);
					}
					else
					{
						FireState = FiringState.movingToLoad;
					}
				}
				Gun.localRotation = Quaternion.Slerp(Gun.localRotation, Pose_Firing.localRotation, Time.deltaTime * 15f);
				break;
			case FiringState.cycledown:
				if (m_isCycleStalling)
				{
					if (m_cycleStallTick > 0f)
					{
						m_cycleStallTick -= Time.deltaTime;
						break;
					}
					m_cycleStallTick = 0f;
					SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.EjectionBack, base.transform.position);
					m_isCycleStalling = false;
					break;
				}
				if (m_cycleLerp < 1f)
				{
					m_cycleLerp += Time.deltaTime * CycleSpeed;
				}
				else
				{
					m_cycleLerp = 1f;
					FireState = FiringState.cycleup;
					SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.EjectionForward, base.transform.position);
				}
				GunCyclePiece.localRotation = Quaternion.Slerp(CyclePose_Firing.localRotation, CyclePose_Reloading.localRotation, Mathf.Sqrt(m_cycleLerp));
				break;
			case FiringState.cycleup:
				if (m_cycleLerp > 0f)
				{
					m_cycleLerp -= Time.deltaTime * CycleSpeed;
				}
				else
				{
					m_cycleLerp = 0f;
					FireState = FiringState.firing;
				}
				GunCyclePiece.localRotation = Quaternion.Slerp(CyclePose_Firing.localRotation, CyclePose_Reloading.localRotation, Mathf.Pow(m_cycleLerp, 2f));
				break;
			case FiringState.movingToLoad:
				if (m_poseChangeLerp < 1f)
				{
					m_poseChangeLerp += Time.deltaTime * PoseChangeSpeed;
				}
				else
				{
					m_poseChangeLerp = 1f;
					FireState = FiringState.reloading;
					SM.PlayCoreSound(FVRPooledAudioType.NPCHandling, GunShotProfile.Reloading, base.transform.position);
				}
				Gun.localPosition = Vector3.Lerp(Pose_Firing.localPosition, Pose_Reloading.localPosition, Mathf.Pow(m_poseChangeLerp, 4f));
				Gun.localRotation = Quaternion.Slerp(Pose_Firing.localRotation, Pose_Reloading.localRotation, Mathf.Pow(m_poseChangeLerp, 4f));
				break;
			case FiringState.reloading:
				if (m_reloadingLerp < 1f)
				{
					m_reloadingLerp += Time.deltaTime * ReloadingSpeed;
				}
				else
				{
					m_reloadingLerp = 1f;
					m_shotsLeft = ShotsPerLoad;
					FireState = FiringState.hanging;
					m_hangTick = HangOnMidReloadingTime;
				}
				GunCyclePiece.localRotation = Quaternion.Slerp(CyclePose_Firing.localRotation, CyclePose_Reloading.localRotation, Mathf.Pow(m_reloadingLerp, 4f));
				break;
			case FiringState.hanging:
				if (m_hangTick > 0f)
				{
					m_hangTick -= Time.deltaTime;
				}
				else
				{
					FireState = FiringState.recovery;
				}
				break;
			case FiringState.recovery:
				if (m_reloadingLerp > 0f)
				{
					m_reloadingLerp -= Time.deltaTime * ReloadingSpeed;
				}
				else
				{
					m_reloadingLerp = 0f;
					FireState = FiringState.movingToFire;
				}
				GunCyclePiece.localRotation = Quaternion.Slerp(CyclePose_Firing.localRotation, CyclePose_Reloading.localRotation, Mathf.Pow(m_reloadingLerp, 4f));
				break;
			case FiringState.movingToFire:
				if (m_poseChangeLerp > 0f)
				{
					m_poseChangeLerp -= Time.deltaTime * PoseChangeSpeed;
				}
				else
				{
					m_poseChangeLerp = 0f;
					m_firingTick = Random.Range(Firing_RefireRange.x, Firing_RefireRange.y);
					FireState = FiringState.firing;
				}
				Gun.localPosition = Vector3.Lerp(Pose_Firing.localPosition, Pose_Reloading.localPosition, Mathf.Pow(m_poseChangeLerp, 4f));
				Gun.localRotation = Quaternion.Slerp(Pose_Firing.localRotation, Pose_Reloading.localRotation, Mathf.Pow(m_poseChangeLerp, 4f));
				break;
			}
		}

		private void PlayShotEvent(Vector3 source)
		{
			float num = Vector3.Distance(source, GM.CurrentPlayerBody.Head.position);
			float delay = num / 343f;
			wwBotWurstGunSoundConfig.BotGunShotSet shotSet = GetShotSet(SM.GetReverbEnvironment(base.transform.position).Environment);
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
