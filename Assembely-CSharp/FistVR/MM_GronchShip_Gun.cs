using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MM_GronchShip_Gun : MonoBehaviour
	{
		public enum MMGronchGunType
		{
			Gatling,
			Plasma,
			Mortar,
			Rockets
		}

		[Header("Main Info")]
		public MM_GronchShip Ship;

		[Header("AudioEvents")]
		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		public MM_GronchShip_DamagePiece[] DamageZones;

		public MMGronchGunType Type;

		private bool m_isFireAtWill;

		private bool m_isDestroyed;

		public Transform YRotPiece;

		public Transform XRotPiece;

		public float YRotThreshold = 45f;

		public GameObject VFXPrefab;

		public Transform VFXSpawnPoint;

		[Header("Gun Details")]
		public GameObject ProjectilePrefab;

		public ParticleSystem MuzzleFlash;

		public Transform[] Muzzles;

		public Transform Aimer;

		public Vector2 FireTickRange = new Vector2(0.2f, 0.4f);

		public Vector2 BurstTickRange = new Vector2(0.2f, 0.4f);

		public float InaccuracyRange = 1f;

		public int MinShotsInBurst = 1;

		public int MaxShotsInBurst = 1;

		public int MinBurstsInSequence = 1;

		public int MaxBurstsInSequence = 1;

		public int MuzzleFlashParticleAmount = 2;

		public Vector2 VelocityRange = new Vector2(10f, 20f);

		private int m_currentMuzzle;

		private float m_fireTick;

		private float m_burstTick;

		private int m_shotsLeftInBurst;

		private int m_burstsLeftInSequence;

		private bool m_isFiringSequenceCompleted = true;

		private float angleToTarget = 20f;

		private void Start()
		{
			PrimeDics();
		}

		public bool IsFiringSequenceCompleted()
		{
			return m_isFiringSequenceCompleted;
		}

		public bool IsDestroyed()
		{
			return m_isDestroyed;
		}

		public void InitiateFiringSequence()
		{
			m_isFiringSequenceCompleted = false;
			m_currentMuzzle = 0;
			m_fireTick = Random.Range(FireTickRange.x, FireTickRange.y);
			m_burstTick = Random.Range(BurstTickRange.x, BurstTickRange.y);
			m_shotsLeftInBurst = Random.Range(MinShotsInBurst, MaxShotsInBurst + 1);
			m_burstsLeftInSequence = Random.Range(MinBurstsInSequence, MaxBurstsInSequence + 1);
			SetFireAtWill(f: true);
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

		public void SetFireAtWill(bool f)
		{
			m_isFireAtWill = f;
		}

		private void Update()
		{
			DamageCheck();
			RotationSystem();
			FireControl();
			if (m_isDestroyed)
			{
				m_isFiringSequenceCompleted = true;
			}
		}

		private void RotationSystem()
		{
			if (m_isFireAtWill)
			{
				Vector3 vector = GM.CurrentPlayerBody.transform.position - YRotPiece.transform.position;
				Vector3 vector2 = Vector3.ProjectOnPlane(vector, base.transform.up);
				YRotPiece.rotation = Quaternion.Slerp(YRotPiece.rotation, Quaternion.LookRotation(vector2, base.transform.up), Time.deltaTime * 4f);
				YRotPiece.localEulerAngles = new Vector3(0f, YRotPiece.localEulerAngles.y, 0f);
				angleToTarget = Vector3.Angle(vector2, YRotPiece.forward);
				if (angleToTarget < YRotThreshold)
				{
					Vector3 vector3 = GM.CurrentPlayerBody.Torso.position + Vector3.up * 0.25f - Aimer.position;
					Vector3 forward = Vector3.ProjectOnPlane(vector3, YRotPiece.right);
					XRotPiece.rotation = Quaternion.Slerp(XRotPiece.rotation, Quaternion.LookRotation(forward, base.transform.up), Time.deltaTime * 4f);
					XRotPiece.localEulerAngles = new Vector3(XRotPiece.localEulerAngles.x, 0f, 0f);
				}
			}
		}

		private void FireControl()
		{
			if (!m_isDestroyed && m_isFireAtWill)
			{
				if (m_burstsLeftInSequence <= 0)
				{
					m_isFiringSequenceCompleted = true;
					SetFireAtWill(f: false);
				}
				else if (m_shotsLeftInBurst <= 0)
				{
					m_burstsLeftInSequence--;
					m_burstTick = Random.Range(BurstTickRange.x, BurstTickRange.y);
					m_shotsLeftInBurst = Random.Range(MinShotsInBurst, MaxShotsInBurst + 1);
				}
				else if (m_burstTick > 0f)
				{
					m_burstTick -= Time.deltaTime;
				}
				else if (m_fireTick > 0f)
				{
					m_fireTick -= Time.deltaTime;
				}
				else if (angleToTarget < 30f)
				{
					m_fireTick = Random.Range(FireTickRange.x, FireTickRange.y);
					Fire();
					m_shotsLeftInBurst--;
				}
			}
		}

		private void Fire()
		{
			PlayShotEvent(Muzzles[m_currentMuzzle].position);
			Muzzles[m_currentMuzzle].localEulerAngles = new Vector3(Random.Range(0f - InaccuracyRange, InaccuracyRange), 0f, Random.Range(0f - InaccuracyRange, InaccuracyRange));
			GameObject gameObject = Object.Instantiate(ProjectilePrefab, Muzzles[m_currentMuzzle].position, Muzzles[m_currentMuzzle].rotation);
			BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
			if (component != null)
			{
				component.Fire(Random.Range(VelocityRange.x, VelocityRange.y), Muzzles[m_currentMuzzle].forward, null);
			}
			else
			{
				Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
				component2.velocity = Random.Range(VelocityRange.x, VelocityRange.y) * Muzzles[m_currentMuzzle].forward;
			}
			if (MuzzleFlash != null)
			{
				MuzzleFlash.transform.position = Muzzles[m_currentMuzzle].position;
				MuzzleFlash.Emit(MuzzleFlashParticleAmount);
			}
			m_currentMuzzle++;
			if (m_currentMuzzle >= Muzzles.Length)
			{
				m_currentMuzzle = 0;
			}
		}

		private void DamageCheck()
		{
			if (m_isDestroyed)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < DamageZones.Length; i++)
			{
				if (!DamageZones[i].IsDestroyed())
				{
					flag = false;
				}
			}
			if (flag)
			{
				SetFireAtWill(f: false);
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				Object.Instantiate(VFXPrefab, VFXSpawnPoint.position, VFXSpawnPoint.rotation);
				Ship.GunDestroyed();
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
