using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class StingerMissile : MonoBehaviour, IFVRDamageable
	{
		public ParticleSystem PFX_Fire;

		public ParticleSystem PFX_Smoke;

		private AIEntity m_targetEntity;

		private bool m_isFlying;

		private bool m_isMotorEngaged;

		private bool m_isDestroyedAndWaitingToCountDown;

		private float m_distanceTravelled;

		private Vector3 m_velocity;

		private Vector3 m_forwardDir;

		public LayerMask LM_Hitting;

		private RaycastHit m_hit;

		public GameObject TurnOffOnExplode;

		public GameObject DestroyOnExplode;

		public List<GameObject> SpawnOnExplode;

		public AudioSource RocketSound;

		public AudioEvent AudEvent_Launch;

		private bool m_hadPlayedLaunchEvent;

		private float m_motorPower = 200f;

		private float m_maxSpeed = 800f;

		private float m_turnSpeed = 1f;

		public GameObject PlaceMarker;

		private Vector3 m_lastPos;

		private Vector3 m_newPos;

		private Vector3 m_lastEntPos;

		private Vector3 m_curEntPos;

		private bool m_tickingDownToStopSound;

		private float m_tickDownToStopSound = 1f;

		public void Fire(AIEntity ent)
		{
			m_targetEntity = ent;
			m_isFlying = true;
			m_forwardDir = base.transform.forward;
			m_velocity = base.transform.forward * 12f;
			m_lastPos = base.transform.position;
			m_newPos = base.transform.position;
			m_curEntPos = ent.GetPos();
			m_lastEntPos = ent.GetPos();
		}

		public void Fire(Vector3 targPos, float InitialSpeed)
		{
			m_targetEntity = null;
			m_isFlying = true;
			m_forwardDir = base.transform.forward;
			m_velocity = base.transform.forward * InitialSpeed;
			m_lastPos = base.transform.position;
			m_newPos = base.transform.position;
			m_curEntPos = targPos;
			m_lastEntPos = targPos;
		}

		public void Fire(AIEntity ent, float InitialSpeed)
		{
			m_targetEntity = ent;
			m_isFlying = true;
			m_forwardDir = base.transform.forward;
			m_velocity = base.transform.forward * InitialSpeed;
			m_lastPos = base.transform.position;
			m_newPos = base.transform.position;
			m_curEntPos = ent.GetPos();
			m_lastEntPos = ent.GetPos();
		}

		public void SetMotorPower(float f)
		{
			m_motorPower = f;
		}

		public void SetMaxSpeed(float f)
		{
			m_maxSpeed = f;
		}

		public void SetTurnSpeed(float f)
		{
			m_turnSpeed = f;
		}

		public void Damage(Damage d)
		{
			Explode();
		}

		private void Update()
		{
			if (m_isFlying && !m_isDestroyedAndWaitingToCountDown)
			{
				float num = 9.81f;
				switch (GM.Options.SimulationOptions.BallisticGravityMode)
				{
				case SimulationOptions.GravityMode.Realistic:
					num = 9.81f;
					break;
				case SimulationOptions.GravityMode.Playful:
					num = 5f;
					break;
				case SimulationOptions.GravityMode.OnTheMoon:
					num = 1.622f;
					break;
				case SimulationOptions.GravityMode.None:
					num = 0f;
					break;
				}
				m_velocity += Vector3.down * num * Time.deltaTime;
				if (m_isMotorEngaged)
				{
					if (!RocketSound.isPlaying)
					{
						RocketSound.Play();
					}
					if (!m_hadPlayedLaunchEvent)
					{
						m_hadPlayedLaunchEvent = true;
						SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_Launch, base.transform.position);
					}
					m_velocity += m_forwardDir * Time.deltaTime * m_motorPower;
					m_velocity = Vector3.ClampMagnitude(m_velocity, m_maxSpeed);
					if (m_targetEntity != null)
					{
						m_curEntPos = m_targetEntity.GetPos();
					}
					Vector3 vector = m_curEntPos - m_lastEntPos;
					Vector3 normalized = vector.normalized;
					float num2 = vector.magnitude * (1f / Time.deltaTime);
					float num3 = Vector3.Distance(base.transform.position, m_curEntPos);
					float num4 = num3 / (m_velocity.magnitude + 0.001f);
					float num5 = num2 * num4;
					Vector3 vector2 = m_curEntPos + normalized * num5;
					Vector3 target = vector2 + num * Time.deltaTime * Vector3.up * 0.5f - base.transform.position;
					Vector3 vector3 = (m_forwardDir = Vector3.RotateTowards(m_forwardDir, target, Time.deltaTime * m_turnSpeed, 1f));
					base.transform.rotation = Quaternion.LookRotation(m_forwardDir);
					m_lastEntPos = m_curEntPos;
				}
				bool flag = false;
				if (m_distanceTravelled > 1f && Physics.SphereCast(m_lastPos, 0.1f, m_velocity.normalized, out m_hit, m_velocity.magnitude * Time.deltaTime, LM_Hitting, QueryTriggerInteraction.Collide))
				{
					m_newPos = m_hit.point;
					flag = true;
				}
				else
				{
					m_newPos = m_lastPos + m_velocity * Time.deltaTime;
				}
				base.transform.position = m_newPos;
				float num6 = Vector3.Distance(m_newPos, m_lastPos);
				m_distanceTravelled += num6;
				if (m_distanceTravelled > 8f && !m_isMotorEngaged)
				{
					m_isMotorEngaged = true;
				}
				if (flag || m_distanceTravelled > 4000f)
				{
					Explode();
				}
				m_lastPos = m_newPos;
			}
			if (m_isDestroyedAndWaitingToCountDown && m_tickingDownToStopSound)
			{
				m_tickDownToStopSound -= Time.deltaTime;
				if (m_tickDownToStopSound <= 0f)
				{
					RocketSound.Stop();
				}
				if (m_tickDownToStopSound <= -30f)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		private void Explode()
		{
			if (!m_isDestroyedAndWaitingToCountDown)
			{
				m_isDestroyedAndWaitingToCountDown = true;
				TurnOffOnExplode.SetActive(value: false);
				if (DestroyOnExplode != null)
				{
					Object.Destroy(DestroyOnExplode);
				}
				m_tickDownToStopSound = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f;
				m_tickingDownToStopSound = true;
				ParticleSystem.EmissionModule emission = PFX_Fire.emission;
				emission.rateOverTimeMultiplier = 0f;
				ParticleSystem.EmissionModule emission2 = PFX_Smoke.emission;
				emission2.rateOverTimeMultiplier = 0f;
				emission2.rateOverDistanceMultiplier = 0f;
				for (int i = 0; i < SpawnOnExplode.Count; i++)
				{
					Object.Instantiate(SpawnOnExplode[i], base.transform.position, Quaternion.identity);
				}
			}
		}
	}
}
