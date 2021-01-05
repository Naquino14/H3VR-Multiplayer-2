using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotHardened : MonoBehaviour
	{
		public enum BotState
		{
			Deactivated,
			Activating,
			Activated,
			Deactivating,
			Exploding
		}

		public AIEntity E;

		public List<GameObject> SpawnOnDestroy;

		public List<Transform> SpawnOnDestroyPoints;

		public Rigidbody RB;

		public BotState State;

		private float m_activateTick;

		public float ActivateSpeed = 1f;

		public float DeactivateSpeed = 1f;

		public float CooldownSpeed = 1f;

		private float m_cooldownTick = 1f;

		private float m_explodingTick;

		public float DetonationRange = 10f;

		public float MoveSpeed = 10f;

		[Header("Audio")]
		public AudioEvent AudEvent_Passive;

		public AudioEvent AudEvent_Activating;

		public AudioEvent AudEvent_Deactivating;

		public AudioEvent AudEvent_Scream;

		public AudioEvent AudEvent_Fire;

		public ParticleSystem ExplodingParticles;

		public LayerMask LM_GroundCast;

		private Vector2 DesiredHeight = new Vector2(3f, 4f);

		private float m_desiredHeight = 4f;

		private float m_curDesiredHeight = 4f;

		public List<Transform> OuterPieces = new List<Transform>();

		public Transform Muzzle;

		public GameObject Projectile;

		public float ProjectileSpread;

		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		private float m_tickDownToSpeak = 1f;

		private int m_queuedShots;

		private float m_shotRefire = 0.05f;

		public GameObject Defense;

		public ParticleSystem MuzzleFire;

		private Vector3 latestTargetPos = Vector3.zero;

		private float moveTowardTick;

		private float m_respondToEventCooldown = 0.1f;

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
		{
			return m_shotDic[e];
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

		private void Start()
		{
			PrimeDics();
			E.AIEventReceiveEvent += EventReceive;
			m_tickDownToSpeak = Random.Range(5f, 20f);
			m_desiredHeight = Random.Range(DesiredHeight.x, DesiredHeight.y);
		}

		private void TestMe()
		{
			TargetSighted(base.transform.position + Random.onUnitSphere * 8f);
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}

		private void Fire()
		{
			m_queuedShots--;
			Vector3 vector = latestTargetPos - Muzzle.position;
			if (!Physics.Raycast(Muzzle.position, vector.normalized, vector.magnitude, LM_GroundCast))
			{
				if (GunShotProfile != null)
				{
					FVRSoundEnvironment se = PlayShotEvent(Muzzle.position);
					float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(se);
				}
				for (int i = 0; i < 3; i++)
				{
					GameObject gameObject = Object.Instantiate(Projectile, Muzzle.position, Muzzle.rotation);
					gameObject.transform.Rotate(new Vector3(Random.Range(0f - ProjectileSpread, ProjectileSpread), Random.Range(0f - ProjectileSpread, ProjectileSpread), 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.FlightVelocityMultiplier = 0.2f;
					float muzzleVelocityBase = component.MuzzleVelocityBase;
					component.Fire(muzzleVelocityBase, gameObject.transform.forward, null);
					component.SetSource_IFF(E.IFFCode);
				}
				MuzzleFire.Emit(2);
				FXM.InitiateMuzzleFlash(Muzzle.position, Muzzle.forward, 4f, new Color(1f, 0.1f, 0.1f), 8f);
			}
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

		private void Update()
		{
			if (m_respondToEventCooldown > 0f)
			{
				m_respondToEventCooldown -= Time.deltaTime;
			}
			ParticleSystem.EmissionModule emission = ExplodingParticles.emission;
			if (Physics.Raycast(base.transform.position, -Vector3.up, m_curDesiredHeight, LM_GroundCast))
			{
				RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
			}
			switch (State)
			{
			case BotState.Deactivated:
				m_tickDownToSpeak -= Time.deltaTime;
				if (m_tickDownToSpeak <= 0f)
				{
					m_tickDownToSpeak = Random.Range(8f, 20f);
					if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 50f)
					{
						FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Passive, base.transform.position);
						fVRPooledAudioSource.FollowThisTransform(base.transform);
					}
					m_curDesiredHeight = m_desiredHeight;
				}
				break;
			case BotState.Activating:
			{
				m_activateTick += Time.deltaTime * ActivateSpeed;
				if (m_activateTick >= 1f)
				{
					m_activateTick = 1f;
					SetState(BotState.Activated);
				}
				for (int j = 0; j < OuterPieces.Count; j++)
				{
					float num2 = Mathf.Lerp(1.1f, 1f, m_activateTick);
					OuterPieces[j].localScale = new Vector3(num2, num2, num2);
				}
				m_curDesiredHeight = m_desiredHeight * m_desiredHeight;
				Vector3 normalized2 = (latestTargetPos - base.transform.position).normalized;
				RB.rotation = Quaternion.Slerp(RB.rotation, Quaternion.LookRotation(normalized2), Time.deltaTime * 8f);
				break;
			}
			case BotState.Activated:
			{
				m_cooldownTick -= Time.deltaTime * CooldownSpeed;
				if (m_cooldownTick <= 0f)
				{
					SetState(BotState.Deactivating);
				}
				if (m_queuedShots > 0)
				{
					m_shotRefire -= Time.deltaTime;
					if (m_shotRefire < 0f)
					{
						m_shotRefire = Random.Range(0.08f, 0.22f);
						Fire();
					}
				}
				Vector3 normalized = (latestTargetPos - base.transform.position).normalized;
				RB.rotation = Quaternion.Slerp(RB.rotation, Quaternion.LookRotation(normalized), Time.deltaTime * 8f);
				m_curDesiredHeight = m_desiredHeight * m_desiredHeight;
				break;
			}
			case BotState.Deactivating:
			{
				m_activateTick -= Time.deltaTime * ActivateSpeed;
				if (m_activateTick <= 0f)
				{
					m_activateTick = 0f;
					SetState(BotState.Deactivated);
				}
				for (int i = 0; i < OuterPieces.Count; i++)
				{
					float num = Mathf.Lerp(1.1f, 1f, m_activateTick);
					OuterPieces[i].localScale = new Vector3(num, num, num);
				}
				m_curDesiredHeight = m_desiredHeight;
				break;
			}
			case BotState.Exploding:
				emission.rateOverTimeMultiplier = 80f;
				m_explodingTick += Time.deltaTime * 2f;
				if (m_explodingTick >= 1f)
				{
					Shatter();
				}
				break;
			}
			if (moveTowardTick > 0f)
			{
				moveTowardTick -= Time.deltaTime;
			}
		}

		public void EventReceive(AIEvent e)
		{
			if (!(m_respondToEventCooldown >= 0.1f) && (!e.IsEntity || e.Entity.IFFCode != E.IFFCode))
			{
				TargetSighted(e.Pos);
			}
		}

		private void TargetSighted(Vector3 v)
		{
			if (State != BotState.Deactivating)
			{
				latestTargetPos = v;
				moveTowardTick = 1f;
				m_queuedShots = 3;
				if (State == BotState.Deactivated)
				{
					SetState(BotState.Activating);
				}
				else if ((State == BotState.Activated || State == BotState.Activating) && Vector3.Distance(v, base.transform.position) <= DetonationRange)
				{
					Explode();
				}
			}
		}

		private void SetState(BotState S)
		{
			if (State == BotState.Exploding || State == S)
			{
				return;
			}
			State = S;
			switch (State)
			{
			case BotState.Deactivated:
				m_activateTick = 0f;
				Defense.SetActive(value: true);
				break;
			case BotState.Activating:
				m_activateTick = 0f;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 50f)
				{
					FVRPooledAudioSource fVRPooledAudioSource2 = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Activating, base.transform.position);
					fVRPooledAudioSource2.FollowThisTransform(base.transform);
				}
				Defense.SetActive(value: false);
				break;
			case BotState.Activated:
				m_cooldownTick = 1f;
				m_activateTick = 1f;
				break;
			case BotState.Deactivating:
				m_activateTick = 1f;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 50f)
				{
					FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Deactivating, base.transform.position);
					fVRPooledAudioSource.FollowThisTransform(base.transform);
				}
				break;
			case BotState.Exploding:
				break;
			}
		}

		public void Explode()
		{
			if (State != BotState.Exploding)
			{
				SetState(BotState.Exploding);
				m_explodingTick = 0f;
				FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Scream, base.transform.position);
				fVRPooledAudioSource.FollowThisTransform(base.transform);
			}
		}

		private void Shatter()
		{
			for (int i = 0; i < SpawnOnDestroy.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(SpawnOnDestroy[i], SpawnOnDestroyPoints[i].position, SpawnOnDestroyPoints[i].rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(Random.Range(1, 10), base.transform.position + Random.onUnitSphere, 5f);
				}
			}
			Object.Destroy(base.gameObject);
		}
	}
}
