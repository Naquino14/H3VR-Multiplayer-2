using UnityEngine;

namespace FistVR
{
	public class AIWeinerBot : MonoBehaviour
	{
		public enum WeinerBotState
		{
			None = -1,
			Guard,
			Patrol,
			Engage,
			Evade,
			Retreat,
			Hunt
		}

		private bool IsActivated = true;

		public AINavigator Navigator;

		public AISensorSystem SensorSystem;

		public AIWeinerBotBodyController BodyController;

		private AudioSource m_aud;

		public AudioClip AudClip_Die;

		public GameObject[] ExplosionPrefabs;

		public WeinerBotState State = WeinerBotState.None;

		public float ExplosionDelay = 3f;

		private float m_guardNewFacingTick;

		private float m_guardSwitchToPatrolTick = 10f;

		private float m_patrolNewFacingTick;

		private float m_patrolNewDestinationTick;

		private float m_patrolSwitchToGuardTick = 120f;

		public float MinEngagementRange = 10f;

		public float MaxEngagementRange = 30f;

		private bool m_evadeNextDirIsRandom;

		private float m_evadeNextDirTickDown = 1f;

		private float m_evadeNextMoveTickDown = 1f;

		public float EvadeCooldown = 20f;

		private float m_evadeGlobalCooldown = 20f;

		private float m_retreatNextDirTickDown = 1f;

		public float HuntingCooldown = 20f;

		private float m_huntTickDown = 20f;

		private void Awake()
		{
			m_aud = GetComponent<AudioSource>();
			SetState(WeinerBotState.Guard);
			BodyController.SensorSystem = SensorSystem;
			m_huntTickDown = HuntingCooldown;
			m_evadeGlobalCooldown = EvadeCooldown;
		}

		private void Update()
		{
			UpdateWeinerBot();
		}

		private void UpdateWeinerBot()
		{
			if (!IsActivated)
			{
				return;
			}
			if (BodyController == null && IsActivated)
			{
				Die();
				if (Navigator != null)
				{
					Object.Destroy(Navigator.Agent);
					Object.Destroy(Navigator);
				}
				GetComponent<Rigidbody>().isKinematic = false;
				GetComponent<Rigidbody>().useGravity = true;
			}
			else
			{
				StateControl();
				SensorControl();
				NavigatorControl();
			}
		}

		public void Die()
		{
			if (IsActivated)
			{
				Navigator.SetPlateDisabled(b: true);
				Navigator.enabled = false;
				IsActivated = false;
				m_aud.PlayOneShot(AudClip_Die, 1f);
				Invoke("Explode", ExplosionDelay);
			}
		}

		private void Explode()
		{
			for (int i = 0; i < ExplosionPrefabs.Length; i++)
			{
				Object.Instantiate(ExplosionPrefabs[i], base.transform.position + Vector3.up * 0.3f, Quaternion.identity);
			}
			Object.Destroy(base.gameObject);
		}

		private void SetState(WeinerBotState newState)
		{
			if (newState != State)
			{
				State = newState;
				EnterState();
			}
		}

		private void EnterState()
		{
			switch (State)
			{
			case WeinerBotState.Guard:
				EnterState_Guard();
				break;
			case WeinerBotState.Patrol:
				EnterState_Patrol();
				break;
			case WeinerBotState.Engage:
				EnterState_Engage();
				break;
			case WeinerBotState.Evade:
				EnterState_Evade();
				break;
			case WeinerBotState.Retreat:
				EnterState_Retreat();
				break;
			case WeinerBotState.Hunt:
				EnterState_Hunt();
				break;
			}
		}

		private void StateControl()
		{
			switch (State)
			{
			case WeinerBotState.Guard:
				State_Guard();
				break;
			case WeinerBotState.Patrol:
				State_Patrol();
				break;
			case WeinerBotState.Engage:
				State_Engage();
				break;
			case WeinerBotState.Evade:
				State_Evade();
				break;
			case WeinerBotState.Retreat:
				State_Retreat();
				break;
			case WeinerBotState.Hunt:
				State_Hunt();
				break;
			}
		}

		private void SensorControl()
		{
			if (SensorSystem != null)
			{
				SensorSystem.UpdateSensorSystem();
			}
		}

		private void NavigatorControl()
		{
			if (Navigator != null)
			{
				Navigator.UpdateNavigationSystem();
			}
		}

		private Vector3 GetRandomTurretFacing()
		{
			Vector3 result = Vector3.zero;
			if (BodyController != null)
			{
				result = BodyController.AimingTransform.position;
				Vector3 vector = BodyController.AimingTransform.forward * Random.Range(-1f, 1f);
				vector += BodyController.AimingTransform.right * Random.Range(-1f, 1f);
				vector.Normalize();
				vector *= 6f;
				result += vector;
			}
			return result;
		}

		private void EnterState_Guard()
		{
			m_guardNewFacingTick = 0f;
			m_guardSwitchToPatrolTick = 10f;
			if (BodyController != null)
			{
				BodyController.SetFireAtWill(b: false);
			}
		}

		private void State_Guard()
		{
			if (BodyController != null)
			{
				if (m_guardNewFacingTick <= 0f)
				{
					m_guardNewFacingTick = Random.Range(3f, 6f);
					BodyController.SetTargetPoint(GetRandomTurretFacing());
				}
				else
				{
					m_guardNewFacingTick -= Time.deltaTime;
				}
				if (m_guardSwitchToPatrolTick <= 0f)
				{
					SetState(WeinerBotState.Patrol);
					return;
				}
				m_guardSwitchToPatrolTick -= Time.deltaTime;
				if (SensorSystem != null && SensorSystem.PriorityTarget != null)
				{
					Debug.Log(SensorSystem.PriorityTarget.Tr.gameObject.name);
					SetState(WeinerBotState.Engage);
				}
			}
			else
			{
				SetState(WeinerBotState.Retreat);
			}
		}

		private void EnterState_Patrol()
		{
			m_patrolNewFacingTick = 0f;
			m_patrolNewDestinationTick = 0f;
			m_patrolSwitchToGuardTick = 120f;
			if (BodyController != null)
			{
				BodyController.SetFireAtWill(b: false);
			}
		}

		private void State_Patrol()
		{
			if (SensorSystem != null && SensorSystem.PriorityTarget != null)
			{
				SetState(WeinerBotState.Engage);
			}
			else if (BodyController != null)
			{
				if (m_patrolNewFacingTick <= 0f)
				{
					m_patrolNewFacingTick = Random.Range(2f, 4f);
					BodyController.SetTargetPoint(GetRandomTurretFacing());
				}
				else
				{
					m_patrolNewFacingTick -= Time.deltaTime;
				}
				if (Navigator != null)
				{
					if (m_patrolNewDestinationTick <= 0f || Navigator.IsAtDestination)
					{
						m_patrolNewDestinationTick = Random.Range(18f, 30f);
						Navigator.SetMovementIntensity(0.15f);
						Navigator.SetNewNavDestination(Navigator.GetRandomNearDestination());
					}
					else
					{
						m_patrolNewDestinationTick -= Time.deltaTime;
					}
				}
				if (m_patrolSwitchToGuardTick <= 0f)
				{
					SetState(WeinerBotState.Guard);
				}
				else
				{
					m_patrolSwitchToGuardTick -= Time.deltaTime;
				}
			}
			else
			{
				SetState(WeinerBotState.Retreat);
			}
		}

		private void EnterState_Engage()
		{
			if (BodyController != null)
			{
				BodyController.SetFireAtWill(b: true);
			}
		}

		private void State_Engage()
		{
			if (SensorSystem == null || BodyController == null)
			{
				SetState(WeinerBotState.Evade);
				return;
			}
			if (SensorSystem.PriorityTarget == null)
			{
				if (SensorSystem.LastPriorityTarget != null && SensorSystem.LastPriorityTarget.Tr != null)
				{
					SetState(WeinerBotState.Hunt);
				}
				else
				{
					SetState(WeinerBotState.Evade);
				}
				return;
			}
			BodyController.SetTargetPoint(SensorSystem.PriorityTarget.LastKnownPosition);
			if (Navigator != null)
			{
				float a = Vector3.Distance(new Vector3(base.transform.position.x, 0f, base.transform.position.z), new Vector3(SensorSystem.PriorityTarget.LastKnownPosition.x, 0f, SensorSystem.PriorityTarget.LastKnownPosition.z));
				float b = Vector3.Distance(new Vector3(Navigator.GetDestination().x, 0f, Navigator.GetDestination().z), new Vector3(SensorSystem.PriorityTarget.LastKnownPosition.x, 0f, SensorSystem.PriorityTarget.LastKnownPosition.z));
				float num = Mathf.Max(a, b);
				if (num > MaxEngagementRange)
				{
					Vector3 vector = base.transform.position - SensorSystem.PriorityTarget.LastKnownPosition;
					vector.y = 0f;
					vector = Vector3.ClampMagnitude(vector, 0.3f);
					vector += SensorSystem.PriorityTarget.LastKnownPosition;
					Debug.DrawLine(vector, SensorSystem.PriorityTarget.LastKnownPosition, Color.cyan);
					Navigator.TryToSetDestinationTo(vector);
					Navigator.SetMovementIntensity(1f);
				}
				else
				{
					Navigator.RotateTowards(SensorSystem.PriorityTarget.LastKnownPosition);
					Navigator.Agent.nextPosition = base.transform.position;
				}
			}
		}

		private void EnterState_Evade()
		{
			if (BodyController != null)
			{
				BodyController.SetFireAtWill(b: true);
			}
			m_evadeNextDirTickDown = 1f;
			m_evadeNextMoveTickDown = 1f;
			m_evadeGlobalCooldown = EvadeCooldown;
		}

		private void State_Evade()
		{
			if (BodyController == null)
			{
				SetState(WeinerBotState.Retreat);
				return;
			}
			if (Navigator == null || m_evadeGlobalCooldown <= 0f)
			{
				SetState(WeinerBotState.Guard);
				return;
			}
			if (SensorSystem != null && SensorSystem.PriorityTarget != null)
			{
				SetState(WeinerBotState.Engage);
				return;
			}
			if (m_evadeGlobalCooldown > 0f)
			{
				m_evadeGlobalCooldown -= Time.deltaTime;
			}
			if (m_evadeNextDirTickDown <= 0f)
			{
				m_evadeNextDirTickDown = Random.Range(1f, 3f);
				if (m_evadeNextDirIsRandom || SensorSystem == null || SensorSystem.LastPriorityTarget == null)
				{
					Vector3 onUnitSphere = Random.onUnitSphere;
					onUnitSphere.y = 0f;
					onUnitSphere.Normalize();
					onUnitSphere *= 8f;
					onUnitSphere.y = Random.Range(-2f, 2f);
					onUnitSphere += base.transform.position;
					BodyController.SetTargetPoint(onUnitSphere);
				}
				else
				{
					BodyController.SetTargetPoint(SensorSystem.LastPriorityTarget.LastKnownPosition);
				}
				m_evadeNextDirIsRandom = !m_evadeNextDirIsRandom;
			}
			else
			{
				m_evadeNextDirTickDown -= Time.deltaTime;
			}
			if (m_evadeNextMoveTickDown <= 0f || Navigator.IsAtDestination)
			{
				m_evadeNextMoveTickDown = Random.Range(3f, 5f);
				Navigator.SetMovementIntensity(1f);
			}
			else
			{
				m_evadeNextMoveTickDown -= Time.deltaTime;
			}
		}

		private void EnterState_Retreat()
		{
			if (BodyController != null)
			{
				BodyController.SetFireAtWill(b: false);
			}
			m_retreatNextDirTickDown = 1f;
		}

		private void State_Retreat()
		{
			if (BodyController != null)
			{
				m_retreatNextDirTickDown -= Time.deltaTime;
				if (m_retreatNextDirTickDown <= 0f)
				{
					m_retreatNextDirTickDown = Random.Range(3f, 5f);
					Vector3 onUnitSphere = Random.onUnitSphere;
					onUnitSphere.y = 0f;
					onUnitSphere.Normalize();
					onUnitSphere *= 8f;
					onUnitSphere.y = Random.Range(-2f, 2f);
					onUnitSphere += base.transform.position;
					BodyController.SetTargetPoint(onUnitSphere);
				}
			}
			if (Navigator != null && Navigator.IsAtDestination && SensorSystem != null && SensorSystem.LastPriorityTarget != null)
			{
				Navigator.TryToSetDestinationTo(SensorSystem.LastPriorityTarget.LastKnownPosition);
			}
		}

		private void EnterState_Hunt()
		{
			if (BodyController != null)
			{
				BodyController.SetFireAtWill(b: false);
			}
			m_huntTickDown = HuntingCooldown;
		}

		private void State_Hunt()
		{
			if (SensorSystem == null || BodyController == null)
			{
				SetState(WeinerBotState.Evade);
				return;
			}
			if (Navigator == null || SensorSystem.LastPriorityTarget == null)
			{
				SetState(WeinerBotState.Guard);
				return;
			}
			if (SensorSystem.PriorityTarget != null)
			{
				SetState(WeinerBotState.Engage);
				return;
			}
			BodyController.SetTargetPoint(SensorSystem.LastPriorityTarget.LastKnownPosition);
			if (Vector3.Distance(Navigator.GetDestination(), SensorSystem.LastPriorityTarget.LastKnownPosition) > 3f)
			{
				Navigator.TryToSetDestinationTo(SensorSystem.LastPriorityTarget.LastKnownPosition);
			}
			m_huntTickDown -= Time.deltaTime;
			if (Navigator.IsAtDestination || m_huntTickDown <= 0f)
			{
				SetState(WeinerBotState.Guard);
			}
		}
	}
}
