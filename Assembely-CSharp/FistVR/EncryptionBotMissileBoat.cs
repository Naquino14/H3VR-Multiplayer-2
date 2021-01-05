using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotMissileBoat : MonoBehaviour
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

		public Rigidbody RB;

		public GameObject MissilePrefab;

		public List<Transform> LauncherMuzzles;

		private List<bool> m_isLoaded = new List<bool>();

		private List<float> m_reloadTime = new List<float>();

		private List<StingerMissile> m_missiles = new List<StingerMissile>();

		public List<GameObject> SpawnOnDestroy;

		public List<Transform> DestroyPoints;

		public BotState State;

		private float m_activateTick;

		public float ActivateSpeed = 1f;

		public float DeactivateSpeed = 1f;

		public float CooldownSpeed = 1f;

		private float m_cooldownTick = 1f;

		private float m_explodingTick;

		public float DetonationRange = 10f;

		[Header("Targetting")]
		public AITargetPrioritySystem Priority;

		[Header("Audio")]
		public AudioEvent AudEvent_Passive;

		public AudioEvent AudEvent_Activating;

		public AudioEvent AudEvent_Deactivating;

		public AudioEvent AudEvent_Scream;

		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		public ParticleSystem ExplodingParticles;

		public LayerMask LM_GroundCast;

		public Vector2 DesiredHeight = new Vector2(4f, 6f);

		private float m_desiredHeight = 4f;

		private float m_tickDownToSpeak = 1f;

		private bool m_hasPriority;

		private float m_refire = 0.5f;

		private bool canFire;

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
		{
			return m_shotDic[e];
		}

		private void Start()
		{
			PrimeDics();
			for (int i = 0; i < 3; i++)
			{
				m_isLoaded.Add(item: false);
				m_missiles.Add(null);
				m_reloadTime.Add(1f);
			}
			E.AIEventReceiveEvent += EventReceive;
			m_tickDownToSpeak = Random.Range(5f, 20f);
			m_desiredHeight = Random.Range(DesiredHeight.x, DesiredHeight.y);
			if (Priority != null)
			{
				m_hasPriority = true;
				Priority.Init(E, 5, 3f, 1.5f);
			}
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}

		public void EventReceive(AIEvent e)
		{
			if (e.IsEntity && e.Entity.IFFCode != E.IFFCode && e.Type == AIEvent.AIEType.Visual)
			{
				Priority.ProcessEvent(e);
				m_cooldownTick = 1f;
				if (State == BotState.Deactivated)
				{
					SetState(BotState.Activating);
				}
				if (!m_hasPriority)
				{
				}
			}
		}

		private void FireControl()
		{
			if (m_refire > 0f)
			{
				m_refire -= Time.deltaTime;
				canFire = false;
			}
			else
			{
				canFire = true;
			}
			if (canFire && Priority.HasFreshTarget())
			{
				for (int i = 0; i < m_isLoaded.Count; i++)
				{
					if (m_isLoaded[i])
					{
						Fire(i, Priority.GetTargetPoint());
						break;
					}
				}
			}
			for (int j = 0; j < m_isLoaded.Count; j++)
			{
				if (!m_isLoaded[j])
				{
					m_reloadTime[j] -= Time.deltaTime;
					if (m_reloadTime[j] < 0f)
					{
						m_isLoaded[j] = true;
					}
				}
			}
		}

		private void Fire(int whichMuzzle, Vector3 point)
		{
			m_isLoaded[whichMuzzle] = false;
			m_reloadTime[whichMuzzle] = Random.Range(8f, 15f);
			m_refire = Random.Range(1.5f, 3f);
			GameObject gameObject = Object.Instantiate(MissilePrefab, LauncherMuzzles[whichMuzzle].position, LauncherMuzzles[whichMuzzle].rotation);
			gameObject.transform.Rotate(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0f));
			StingerMissile component = gameObject.GetComponent<StingerMissile>();
			component.SetMotorPower(20f);
			component.SetMaxSpeed(20f);
			component.SetTurnSpeed(Random.Range(3.4f, 4f));
			if (Priority.IsTargetEntity())
			{
				component.Fire(point, 20f);
			}
			else
			{
				component.Fire(point, 20f);
			}
			m_missiles[whichMuzzle] = component;
		}

		private void Update()
		{
			if (State != BotState.Exploding)
			{
				Priority.Compute();
			}
			ParticleSystem.EmissionModule emission = ExplodingParticles.emission;
			switch (State)
			{
			case BotState.Deactivated:
				m_tickDownToSpeak -= Time.deltaTime;
				if (m_tickDownToSpeak <= 0f)
				{
					m_tickDownToSpeak = Random.Range(8f, 20f);
					if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 80f)
					{
						FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Passive, base.transform.position);
						fVRPooledAudioSource.FollowThisTransform(base.transform);
					}
					emission.rateOverTimeMultiplier = 0f;
				}
				if (Physics.Raycast(base.transform.position, -Vector3.up, m_desiredHeight, LM_GroundCast))
				{
					RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
				}
				break;
			case BotState.Activating:
				m_activateTick += Time.deltaTime * ActivateSpeed;
				if (m_activateTick >= 1f)
				{
					SetState(BotState.Activated);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Activated:
				m_cooldownTick -= Time.deltaTime * CooldownSpeed;
				FireControl();
				if (m_cooldownTick <= 0f)
				{
					SetState(BotState.Deactivating);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Deactivating:
				m_activateTick -= Time.deltaTime * ActivateSpeed;
				if (m_activateTick <= 0f)
				{
					m_activateTick = 0f;
					SetState(BotState.Deactivated);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Exploding:
				emission.rateOverTimeMultiplier = 80f;
				m_explodingTick += Time.deltaTime * 2f;
				if (m_explodingTick >= 1f)
				{
					Shatter();
				}
				break;
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
				break;
			case BotState.Activating:
				m_activateTick = 0f;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 250f)
				{
					FVRPooledAudioSource fVRPooledAudioSource2 = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Activating, base.transform.position);
					fVRPooledAudioSource2.FollowThisTransform(base.transform);
				}
				break;
			case BotState.Activated:
				m_cooldownTick = 1f;
				m_activateTick = 1f;
				break;
			case BotState.Deactivating:
				m_activateTick = 1f;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 250f)
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
				GameObject gameObject = Object.Instantiate(SpawnOnDestroy[i], DestroyPoints[i].position, DestroyPoints[i].rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(Random.Range(1, 10), base.transform.position + Random.onUnitSphere, 5f);
				}
			}
			Object.Destroy(base.gameObject);
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
	}
}
