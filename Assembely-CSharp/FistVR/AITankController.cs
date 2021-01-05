using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class AITankController : MonoBehaviour
	{
		private bool IsActivated;

		public AISensorSystem SensorSystem;

		public AITurretController TurretController;

		private NavMeshAgent m_agent;

		private Vector3 m_currentNavTarget;

		private NavMeshHit m_nHit;

		public AIStrikePlateSlideDown StrikePlate1;

		public AIStrikePlateRotateResetting StrikePlate2;

		private bool IsLocomotionDisrupted;

		public Transform BotChassis;

		public float curChassisXRot;

		public float tarChassisXRot;

		public float deadCharrisXRot = 30f;

		private AudioSource m_aud;

		public AudioClip AudClip_Die;

		private void Awake()
		{
			m_agent = GetComponent<NavMeshAgent>();
			m_currentNavTarget = base.transform.position;
			m_aud = GetComponent<AudioSource>();
			Invoke("Activate", 5f);
		}

		public void Disable()
		{
			if (IsActivated)
			{
				IsActivated = false;
				m_aud.PlayOneShot(AudClip_Die, 1f);
				m_agent.Stop();
				m_agent.ResetPath();
			}
		}

		public void Activate()
		{
			StrikePlate1.Reset();
			StrikePlate2.Reset();
			IsActivated = true;
		}

		public void UndisruptLocomotion()
		{
			IsLocomotionDisrupted = false;
			m_agent.Resume();
		}

		public void DisruptLocomotion()
		{
			IsLocomotionDisrupted = true;
			m_agent.Stop();
			m_agent.ResetPath();
		}

		private void Update()
		{
			if (IsActivated)
			{
				TurretController.UpdateTurretController();
				SensorSystem.UpdateSensorSystem();
				if (SensorSystem.PriorityTarget != null)
				{
					TurretController.SetTargetPoint(SensorSystem.PriorityTarget.LastKnownPosition);
					if (!IsLocomotionDisrupted)
					{
						CheckIfNeedNewPath(SensorSystem.PriorityTarget.LastKnownPosition);
					}
					TurretController.SetFireAtWill(b: true);
				}
				else
				{
					TurretController.SetTargetPoint(base.transform.position + base.transform.forward + base.transform.up);
					TurretController.SetFireAtWill(b: false);
				}
				tarChassisXRot = 0f;
			}
			else
			{
				tarChassisXRot = deadCharrisXRot;
				TurretController.SetFireAtWill(b: false);
			}
			if (curChassisXRot != tarChassisXRot)
			{
				curChassisXRot = Mathf.Lerp(curChassisXRot, tarChassisXRot, Time.deltaTime * 2f);
				BotChassis.localEulerAngles = new Vector3(curChassisXRot, 0f, 0f);
			}
		}

		private void CheckIfNeedNewPath(Vector3 target)
		{
			Vector3 vector = m_currentNavTarget;
			Vector3 vector2 = Random.onUnitSphere * 0.5f;
			vector2.y = 0f;
			if (NavMesh.SamplePosition(target + vector2 + Vector3.down * 0.5f, out m_nHit, 1.9f, -1))
			{
				vector = m_nHit.position;
			}
			if (Vector3.Distance(vector, m_currentNavTarget) > 2f)
			{
				m_currentNavTarget = vector;
				m_agent.SetDestination(vector);
			}
		}
	}
}
