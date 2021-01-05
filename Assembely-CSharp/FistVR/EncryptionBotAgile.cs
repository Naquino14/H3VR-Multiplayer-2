using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotAgile : MonoBehaviour
	{
		public enum BotState
		{
			Patrol,
			Activating,
			Activated,
			Deactivating,
			Exploding,
			Evading
		}

		public AIEntity E;

		public Rigidbody RB;

		public List<GameObject> SpawnOnDestroy;

		public List<Transform> DestroyPoints;

		public float PatrolRange_Lateral;

		public float PatrolHeightAddMax;

		public List<Vector3> PatrolPoints = new List<Vector3>();

		private int m_curPatrolPoint;

		public BotState State;

		private float m_activateTick;

		public float ActivateSpeed = 1f;

		public float DeactivateSpeed = 1f;

		public float CooldownSpeed = 1f;

		private float m_cooldownTick = 1f;

		private float m_explodingTick;

		public float Speed_Patrol;

		[Header("Targetting")]
		public AITargetPrioritySystem Priority;

		public GameObject ScanBeamRoot;

		public List<Renderer> ScanBeams;

		public LayerMask LM_ScanBeam;

		private RaycastHit h;

		private Vector3 BeamForward = Vector3.zero;

		[Header("Audio")]
		public AudioEvent AudEvent_Passive;

		public AudioEvent AudEvent_Activating;

		public AudioEvent AudEvent_Deactivating;

		public AudioEvent AudEvent_Scream;

		public AudioEvent AudEvent_Evade;

		public List<Transform> Muzzles;

		public wwBotWurstGunSoundConfig GunShotProfile;

		private Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet> m_shotDic = new Dictionary<FVRSoundEnvironment, wwBotWurstGunSoundConfig.BotGunShotSet>();

		public GameObject ProjectilePrefab;

		public ParticleSystem ExplodingParticles;

		public FVRFireArmRoundDisplayData RoundData;

		private float m_tickDownToSpeak = 1f;

		private bool m_hasPriority;

		private float m_refire = 0.1f;

		private bool canFire;

		private float m_EvasionTick = 1f;

		private wwBotWurstGunSoundConfig.BotGunShotSet GetShotSet(FVRSoundEnvironment e)
		{
			return m_shotDic[e];
		}

		private void GeneratePatrolPoints()
		{
			for (int i = 0; i < 10; i++)
			{
				Vector3 position = base.transform.position;
				position.y += Random.Range(0f, PatrolHeightAddMax);
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y = 0f;
				onUnitSphere.Normalize();
				position += onUnitSphere * Random.Range(PatrolRange_Lateral * 0.5f, PatrolRange_Lateral);
				PatrolPoints.Add(position);
			}
		}

		private void Start()
		{
			PrimeDics();
			GeneratePatrolPoints();
			E.AIEventReceiveEvent += EventReceive;
			m_tickDownToSpeak = Random.Range(5f, 20f);
			if (Priority != null)
			{
				m_hasPriority = true;
				Priority.Init(E, 5, 3f, 1.5f);
			}
			BeamForward = ScanBeamRoot.transform.forward;
			base.transform.position += Vector3.up * Random.Range(300f, 500f);
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}

		public void EventReceive(AIEvent e)
		{
			if (e.IsEntity && e.Entity.IFFCode != E.IFFCode && !(e.Entity == E) && e.Type == AIEvent.AIEType.Visual)
			{
				Priority.ProcessEvent(e);
				m_cooldownTick = 1f;
				if (State == BotState.Patrol)
				{
					SetState(BotState.Activating);
				}
			}
		}

		public void Evade(Vector3 strikeDir)
		{
			if (State != BotState.Evading)
			{
				SetState(BotState.Evading);
				AIEvent e = new AIEvent(base.transform.position - strikeDir * 100f, AIEvent.AIEType.Visual, 1f);
				Priority.ProcessEvent(e);
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
				Fire(GetClosestMuzzle(), Priority.GetTargetPoint());
			}
		}

		private Transform GetClosestMuzzle()
		{
			float num = 3000f;
			Transform result = null;
			for (int i = 0; i < Muzzles.Count; i++)
			{
				float num2 = Vector3.Distance(Priority.GetTargetPoint(), Muzzles[i].position);
				if (num2 < num)
				{
					num = num2;
					result = Muzzles[i];
				}
			}
			return result;
		}

		private void Fire(Transform Muzzle, Vector3 point)
		{
			m_refire = Random.Range(0.1f, 0.3f);
			float num = Vector3.Distance(Muzzle.position, point);
			float num2 = Mathf.Abs(RoundData.BulletDropCurve.Evaluate(num * 0.001f));
			point += Vector3.up * num2;
			Vector3 forward = point - Muzzle.position;
			float num3 = 0.3f;
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject = Object.Instantiate(ProjectilePrefab, Muzzle.position, Quaternion.LookRotation(forward));
				gameObject.transform.Rotate(new Vector3(Random.Range(0f - num3, num3), Random.Range(0f - num3, num3), 0f));
				BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
				component.FlightVelocityMultiplier = Random.Range(0.4f, 0.5f);
				float muzzleVelocityBase = component.MuzzleVelocityBase;
				component.Fire(muzzleVelocityBase, gameObject.transform.forward, null);
				component.SetSource_IFF(E.IFFCode);
			}
		}

		private void FlyToCurrentPatrolPoint()
		{
			Vector3 forward = PatrolPoints[m_curPatrolPoint] - base.transform.position;
			RB.MoveRotation(Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 2f));
			Vector3 vector = forward.normalized * 10f;
			RB.MovePosition(RB.position + vector * Time.deltaTime);
			if (forward.magnitude < 20f)
			{
				m_curPatrolPoint++;
				if (m_curPatrolPoint >= PatrolPoints.Count)
				{
					m_curPatrolPoint = 0;
				}
			}
			UpdateScanBeams();
		}

		private void FlyToTarget()
		{
			Vector3 forward = Priority.GetTargetPoint() - base.transform.position;
			RB.MoveRotation(Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 2f));
			Vector3 vector = forward.normalized * 10f;
			if (forward.magnitude > 30f)
			{
				RB.MovePosition(RB.position + vector * 2f * Time.deltaTime);
			}
			UpdateScanBeams();
		}

		private void UpdateScanBeams()
		{
			for (int i = 0; i < ScanBeams.Count; i++)
			{
				float num = 350f;
				if (Physics.Raycast(ScanBeams[i].transform.position, ScanBeams[i].transform.forward, out h, 350f, LM_ScanBeam))
				{
					num = h.distance + 0.2f;
				}
				ScanBeams[i].transform.localScale = new Vector3(0.01f, 0.01f, num * 0.2f);
			}
		}

		private void UpdateScanBeamSystem()
		{
			Vector3 b = E.transform.forward;
			if (State == BotState.Activating || State == BotState.Activated)
			{
				b = Priority.GetTargetPoint() - base.transform.position;
			}
			BeamForward = Vector3.Slerp(BeamForward, b, Time.deltaTime * 4f);
			ScanBeamRoot.transform.rotation = Quaternion.LookRotation(BeamForward, Vector3.zero);
		}

		private void Update()
		{
			if (State != BotState.Exploding)
			{
				Priority.Compute();
			}
			UpdateScanBeamSystem();
			ParticleSystem.EmissionModule emission = ExplodingParticles.emission;
			switch (State)
			{
			case BotState.Patrol:
				FlyToCurrentPatrolPoint();
				m_tickDownToSpeak -= Time.deltaTime;
				if (m_tickDownToSpeak <= 0f)
				{
					m_tickDownToSpeak = Random.Range(8f, 20f);
					if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 300f)
					{
						FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Passive, base.transform.position);
						fVRPooledAudioSource.FollowThisTransform(base.transform);
					}
					emission.rateOverTimeMultiplier = 0f;
				}
				break;
			case BotState.Activating:
				m_activateTick += Time.deltaTime * ActivateSpeed;
				FlyToCurrentPatrolPoint();
				if (m_activateTick >= 1f)
				{
					SetState(BotState.Activated);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Activated:
				m_cooldownTick -= Time.deltaTime * CooldownSpeed;
				FlyToTarget();
				FireControl();
				if (m_cooldownTick <= 0f)
				{
					SetState(BotState.Deactivating);
				}
				emission.rateOverTimeMultiplier = 0f;
				break;
			case BotState.Deactivating:
				FlyToCurrentPatrolPoint();
				m_activateTick -= Time.deltaTime * ActivateSpeed;
				if (m_activateTick <= 0f)
				{
					m_activateTick = 0f;
					SetState(BotState.Patrol);
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
			case BotState.Evading:
				emission.rateOverTimeMultiplier = 0f;
				m_EvasionTick -= Time.deltaTime;
				if (m_EvasionTick < 0f)
				{
					if (Priority.HasFreshTarget())
					{
						m_refire = 0f;
						SetState(BotState.Activated);
					}
					else
					{
						SetState(BotState.Patrol);
					}
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
			case BotState.Patrol:
				ScanBeamRoot.SetActive(value: true);
				m_activateTick = 0f;
				RB.isKinematic = true;
				RB.useGravity = false;
				break;
			case BotState.Activating:
				ScanBeamRoot.SetActive(value: true);
				m_activateTick = 0f;
				RB.isKinematic = true;
				RB.useGravity = false;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 350f)
				{
					FVRPooledAudioSource fVRPooledAudioSource2 = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Activating, base.transform.position);
					fVRPooledAudioSource2.FollowThisTransform(base.transform);
				}
				break;
			case BotState.Activated:
				ScanBeamRoot.SetActive(value: true);
				RB.isKinematic = true;
				RB.useGravity = false;
				m_cooldownTick = 1f;
				m_activateTick = 1f;
				break;
			case BotState.Deactivating:
				ScanBeamRoot.SetActive(value: true);
				RB.isKinematic = true;
				RB.useGravity = false;
				m_activateTick = 1f;
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 350f)
				{
					FVRPooledAudioSource fVRPooledAudioSource3 = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Deactivating, base.transform.position);
					fVRPooledAudioSource3.FollowThisTransform(base.transform);
				}
				break;
			case BotState.Exploding:
				ScanBeamRoot.SetActive(value: false);
				RB.isKinematic = false;
				RB.useGravity = true;
				break;
			case BotState.Evading:
			{
				ScanBeamRoot.SetActive(value: false);
				RB.isKinematic = false;
				RB.useGravity = false;
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y = Mathf.Abs(onUnitSphere.y);
				onUnitSphere *= Random.Range(25f, 35f);
				RB.velocity = onUnitSphere;
				RB.angularVelocity = Random.onUnitSphere * Random.Range(7, 15);
				m_EvasionTick = Random.Range(0.4f, 1.2f);
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 350f)
				{
					FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Evade, base.transform.position);
					if (fVRPooledAudioSource != null)
					{
						fVRPooledAudioSource.FollowThisTransform(base.transform);
					}
				}
				break;
			}
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

		public void OnCollisionEnter()
		{
			Explode();
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
