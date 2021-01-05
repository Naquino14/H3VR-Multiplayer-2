using UnityEngine;

namespace FistVR
{
	public class AIClunk : MonoBehaviour
	{
		public enum ClunkState
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

		public AIBallJointTurret Turret;

		public AINavigator Navigator;

		public AISensorSystem SensorSystem;

		private AudioSource m_aud;

		public AudioClip AudClip_Die;

		public GameObject[] ExplosionPrefabs;

		public ClunkState State = ClunkState.None;

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
			SetState(ClunkState.Guard);
			Turret.SensorSystem = SensorSystem;
			m_huntTickDown = HuntingCooldown;
			m_evadeGlobalCooldown = EvadeCooldown;
		}

		private void Update()
		{
			UpdateClunk();
		}

		private void UpdateClunk()
		{
			if (IsActivated)
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

		private void SetState(ClunkState newState)
		{
			if (newState != State)
			{
				Debug.Log("Entered State: " + newState);
				State = newState;
				EnterState();
			}
		}

		private void EnterState()
		{
			switch (State)
			{
			case ClunkState.Guard:
				EnterState_Guard();
				break;
			case ClunkState.Patrol:
				EnterState_Patrol();
				break;
			case ClunkState.Engage:
				EnterState_Engage();
				break;
			case ClunkState.Evade:
				EnterState_Evade();
				break;
			case ClunkState.Retreat:
				EnterState_Retreat();
				break;
			case ClunkState.Hunt:
				EnterState_Hunt();
				break;
			}
		}

		private void StateControl()
		{
			switch (State)
			{
			case ClunkState.Guard:
				State_Guard();
				break;
			case ClunkState.Patrol:
				State_Patrol();
				break;
			case ClunkState.Engage:
				State_Engage();
				break;
			case ClunkState.Evade:
				State_Evade();
				break;
			case ClunkState.Retreat:
				State_Retreat();
				break;
			case ClunkState.Hunt:
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
			if (Turret != null)
			{
				result = Turret.AimingTransform.position;
				Vector3 vector = Turret.AimingTransform.forward * Random.Range(-1f, 1f);
				vector += Turret.AimingTransform.right * Random.Range(-1f, 1f);
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
			if (Turret != null)
			{
				Turret.SetFireAtWill(b: false);
			}
		}

		private void State_Guard()
		{
			if (Turret != null)
			{
				if (m_guardNewFacingTick <= 0f)
				{
					m_guardNewFacingTick = Random.Range(3f, 6f);
					Turret.SetTargetPoint(GetRandomTurretFacing());
				}
				else
				{
					m_guardNewFacingTick -= Time.deltaTime;
				}
				if (m_guardSwitchToPatrolTick <= 0f)
				{
					SetState(ClunkState.Patrol);
					return;
				}
				m_guardSwitchToPatrolTick -= Time.deltaTime;
				if (SensorSystem != null && SensorSystem.PriorityTarget != null)
				{
					Debug.Log(SensorSystem.PriorityTarget.Tr.gameObject.name);
					SetState(ClunkState.Engage);
				}
			}
			else
			{
				SetState(ClunkState.Retreat);
			}
		}

		private void EnterState_Patrol()
		{
			m_patrolNewFacingTick = 0f;
			m_patrolNewDestinationTick = 0f;
			m_patrolSwitchToGuardTick = 120f;
			if (Turret != null)
			{
				Turret.SetFireAtWill(b: false);
			}
		}

		private void State_Patrol()
		{
			if (SensorSystem != null && SensorSystem.PriorityTarget != null)
			{
				SetState(ClunkState.Engage);
			}
			else if (Turret != null)
			{
				if (m_patrolNewFacingTick <= 0f)
				{
					m_patrolNewFacingTick = Random.Range(2f, 4f);
					Turret.SetTargetPoint(GetRandomTurretFacing());
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
					SetState(ClunkState.Guard);
				}
				else
				{
					m_patrolSwitchToGuardTick -= Time.deltaTime;
				}
			}
			else
			{
				SetState(ClunkState.Retreat);
			}
		}

		private void EnterState_Engage()
		{
			if (Turret != null)
			{
				Turret.SetFireAtWill(b: true);
			}
		}

		private void State_Engage()
		{
			if (SensorSystem == null || Turret == null)
			{
				SetState(ClunkState.Evade);
				return;
			}
			if (SensorSystem.PriorityTarget == null)
			{
				if (SensorSystem.LastPriorityTarget != null && SensorSystem.LastPriorityTarget.Tr != null)
				{
					SetState(ClunkState.Hunt);
				}
				else
				{
					SetState(ClunkState.Evade);
				}
				return;
			}
			Turret.SetTargetPoint(SensorSystem.PriorityTarget.LastKnownPosition);
			if (!(Navigator != null))
			{
				return;
			}
			float num = Vector3.Distance(base.transform.position, SensorSystem.PriorityTarget.LastKnownPosition);
			float b = Vector3.Distance(Navigator.GetDestination(), SensorSystem.PriorityTarget.LastKnownPosition);
			float num2 = Mathf.Min(num, b);
			if (num2 < MinEngagementRange)
			{
				Navigator.SetNewNavDestination(Navigator.GetFurthestNearPointFrom(SensorSystem.PriorityTarget.LastKnownPosition));
				Navigator.SetMovementIntensity(1f);
			}
			if (Navigator.IsAtDestination)
			{
				if (num2 < MinEngagementRange)
				{
					Navigator.SetNewNavDestination(Navigator.GetFurthestNearPointFrom(SensorSystem.PriorityTarget.LastKnownPosition));
					Navigator.SetMovementIntensity(1f);
				}
				else if (num > MaxEngagementRange)
				{
					Navigator.SetNewNavDestination(Navigator.GetNearestNearPointFrom(SensorSystem.PriorityTarget.LastKnownPosition));
					Navigator.SetMovementIntensity(1f);
				}
				else
				{
					Navigator.SetNewNavDestination(Navigator.GetRandomNearDestination());
					Navigator.SetMovementIntensity(0.3f);
				}
			}
		}

		private void EnterState_Evade()
		{
			if (Turret != null)
			{
				Turret.SetFireAtWill(b: true);
			}
			m_evadeNextDirTickDown = 1f;
			m_evadeNextMoveTickDown = 1f;
			m_evadeGlobalCooldown = EvadeCooldown;
		}

		private void State_Evade()
		{
			if (Turret == null)
			{
				SetState(ClunkState.Retreat);
				return;
			}
			if (Navigator == null || m_evadeGlobalCooldown <= 0f)
			{
				SetState(ClunkState.Guard);
				return;
			}
			if (SensorSystem != null && SensorSystem.PriorityTarget != null)
			{
				SetState(ClunkState.Engage);
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
					Turret.SetTargetPoint(onUnitSphere);
				}
				else
				{
					Turret.SetTargetPoint(SensorSystem.LastPriorityTarget.LastKnownPosition);
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
			if (Turret != null)
			{
				Turret.SetFireAtWill(b: false);
			}
			m_retreatNextDirTickDown = 1f;
		}

		private void State_Retreat()
		{
			if (Turret != null)
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
					Turret.SetTargetPoint(onUnitSphere);
				}
			}
			if (Navigator != null && Navigator.IsAtDestination && SensorSystem != null && SensorSystem.LastPriorityTarget != null)
			{
				Navigator.TryToSetDestinationTo(SensorSystem.LastPriorityTarget.LastKnownPosition);
			}
		}

		private void EnterState_Hunt()
		{
			if (Turret != null)
			{
				Turret.SetFireAtWill(b: false);
			}
			m_huntTickDown = HuntingCooldown;
		}

		private void State_Hunt()
		{
			if (SensorSystem == null || Turret == null)
			{
				SetState(ClunkState.Evade);
				return;
			}
			if (Navigator == null || SensorSystem.LastPriorityTarget == null)
			{
				SetState(ClunkState.Guard);
				return;
			}
			if (SensorSystem.PriorityTarget != null)
			{
				SetState(ClunkState.Engage);
				return;
			}
			Turret.SetTargetPoint(SensorSystem.LastPriorityTarget.LastKnownPosition);
			if (Vector3.Distance(Navigator.GetDestination(), SensorSystem.LastPriorityTarget.LastKnownPosition) > 3f)
			{
				Navigator.TryToSetDestinationTo(SensorSystem.LastPriorityTarget.LastKnownPosition);
			}
			m_huntTickDown -= Time.deltaTime;
			if (Navigator.IsAtDestination || m_huntTickDown <= 0f)
			{
				SetState(ClunkState.Guard);
			}
		}
	}
}
