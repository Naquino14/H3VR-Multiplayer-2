using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotMine : MonoBehaviour
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

		public ParticleSystem ActivatedParticles;

		public ParticleSystem ExplodingParticles;

		public LayerMask LM_GroundCast;

		public Vector2 DesiredHeight = new Vector2(4f, 6f);

		private float m_desiredHeight = 4f;

		private float m_tickDownToSpeak = 1f;

		private Vector3 latestTargetPos = Vector3.zero;

		private float moveTowardTick;

		private float m_respondToEventCooldown = 0.1f;

		private void Start()
		{
			E.AIEventReceiveEvent += EventReceive;
			m_tickDownToSpeak = Random.Range(5f, 20f);
			m_desiredHeight = Random.Range(DesiredHeight.x, DesiredHeight.y);
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}

		private void TestMe()
		{
		}

		private void Update()
		{
			if (m_respondToEventCooldown > 0f)
			{
				m_respondToEventCooldown -= Time.deltaTime;
			}
			ParticleSystem.EmissionModule emission = ActivatedParticles.emission;
			ParticleSystem.EmissionModule emission2 = ExplodingParticles.emission;
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
				emission.rateOverTimeMultiplier = m_activateTick * 80f;
				break;
			case BotState.Activated:
				m_cooldownTick -= Time.deltaTime * CooldownSpeed;
				if (m_cooldownTick <= 0f)
				{
					SetState(BotState.Deactivating);
				}
				emission.rateOverTimeMultiplier = m_activateTick * 80f;
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
				emission.rateOverTimeMultiplier = 0f;
				emission2.rateOverTimeMultiplier = 80f;
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
				Vector3 normalized = (latestTargetPos - base.transform.position).normalized;
				RB.velocity = MoveSpeed * normalized;
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

		private void OnCollisionEnter(Collision collision)
		{
			if ((State == BotState.Activating || State == BotState.Activated) && collision.gameObject.layer != LayerMask.NameToLayer("Environment"))
			{
				Explode();
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
				if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) <= 50f)
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
