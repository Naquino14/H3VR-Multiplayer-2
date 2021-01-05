using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotStealthMine : MonoBehaviour, IFVRDamageable
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

		public float DetonationRange = 2f;

		public float MoveSpeed = 10f;

		[Header("Audio")]
		public AudioEvent AudEvent_Passive;

		public AudioEvent AudEvent_Activating;

		public AudioEvent AudEvent_Deactivating;

		public AudioEvent AudEvent_Scream;

		public AudioEvent AudEvent_TP;

		public ParticleSystem ActivatedParticles;

		public ParticleSystem ExplodingParticles;

		public LayerMask LM_GroundCast;

		public Vector2 DesiredHeight = new Vector2(2f, 2.2f);

		private float m_desiredHeight = 4f;

		[Header("StealthPart")]
		public Renderer Rend_Base;

		public Renderer Rend_Cloaked;

		public LayerMask LM_Stealth;

		private float m_timeTilTeleport = 10f;

		private Vector3 startPoint = Vector3.zero;

		private float m_tickDownToSpeak = 1f;

		private RaycastHit h;

		private Vector3 latestTargetPos = Vector3.zero;

		private float moveTowardTick;

		private float m_respondToEventCooldown = 0.1f;

		private float m_stun;

		public void Damage(Damage d)
		{
			m_stun = 0.4f;
		}

		private void Start()
		{
			E.AIEventReceiveEvent += EventReceive;
			m_tickDownToSpeak = Random.Range(5f, 20f);
			m_desiredHeight = Random.Range(DesiredHeight.x, DesiredHeight.y);
			m_timeTilTeleport = Random.Range(10f, 30f);
			startPoint = base.transform.position;
			Rend_Cloaked.material.SetVector("_MainTexVelocity", new Vector2(Random.Range(0.2104f, 0.241f), 1f));
		}

		private void OnDestroy()
		{
			E.AIEventReceiveEvent -= EventReceive;
		}

		private void Teleport()
		{
			m_timeTilTeleport = Random.Range(5f, 15f);
			Vector3 vector = startPoint + Random.onUnitSphere * 50f;
			Vector3 vector2 = new Vector3(GM.CurrentPlayerBody.Head.position.x, 0f, GM.CurrentPlayerBody.Head.position.z);
			Vector3 vector3 = new Vector3(vector.x, 0f, vector.z);
			if (Vector3.Distance(vector2, vector3) < 10f)
			{
				vector3 += (vector3 - vector2).normalized * 10f;
			}
			if (Physics.SphereCast(vector3 + Vector3.up * 500f, 1.5f, Vector3.down, out h, 1000f, LM_Stealth))
			{
				vector3.y = h.point.y + Random.Range(2f, 4f);
				ExplodingParticles.Emit(10);
				base.transform.position = vector3;
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				ExplodingParticles.Emit(10);
				if (num < 25f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, AudEvent_TP, base.transform.position);
				}
			}
		}

		private void Update()
		{
			if (m_stun > 0f)
			{
				m_stun -= Time.deltaTime;
			}
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
				if (Rend_Base.enabled)
				{
					Rend_Base.enabled = false;
				}
				if (!Rend_Cloaked.enabled)
				{
					Rend_Base.enabled = true;
				}
				m_timeTilTeleport -= Time.deltaTime;
				if (m_timeTilTeleport <= 0f)
				{
					Teleport();
				}
				break;
			case BotState.Activating:
				m_activateTick += Time.deltaTime * ActivateSpeed;
				if (m_activateTick >= 1f)
				{
					SetState(BotState.Activated);
				}
				emission.rateOverTimeMultiplier = m_activateTick * 10f;
				if (Rend_Base.enabled)
				{
					Rend_Base.enabled = false;
				}
				if (!Rend_Cloaked.enabled)
				{
					Rend_Base.enabled = true;
				}
				break;
			case BotState.Activated:
				m_cooldownTick -= Time.deltaTime * CooldownSpeed;
				if (m_cooldownTick <= 0f)
				{
					SetState(BotState.Deactivating);
				}
				emission.rateOverTimeMultiplier = m_activateTick * 10f;
				if (!Rend_Base.enabled)
				{
					Rend_Base.enabled = true;
				}
				if (Rend_Cloaked.enabled)
				{
					Rend_Base.enabled = false;
				}
				break;
			case BotState.Deactivating:
				m_activateTick -= Time.deltaTime * ActivateSpeed;
				if (m_activateTick <= 0f)
				{
					m_activateTick = 0f;
					SetState(BotState.Deactivated);
				}
				if (Rend_Base.enabled)
				{
					Rend_Base.enabled = false;
				}
				if (!Rend_Cloaked.enabled)
				{
					Rend_Base.enabled = true;
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
				if (!Rend_Base.enabled)
				{
					Rend_Base.enabled = true;
				}
				if (Rend_Cloaked.enabled)
				{
					Rend_Base.enabled = false;
				}
				break;
			}
			if (moveTowardTick > 0f && m_stun <= 0f)
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
			Explode();
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
