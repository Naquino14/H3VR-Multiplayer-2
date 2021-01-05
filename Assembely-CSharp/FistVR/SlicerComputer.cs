using UnityEngine;

namespace FistVR
{
	public class SlicerComputer : FVRDestroyableObject
	{
		[Header("Slicer Computer Params")]
		public Rigidbody BaseRB;

		public AIThrusterControlBox[] ControlBoxes;

		public AISensorSystem Sensors;

		private float m_desiredElevation = 1.3f;

		private bool m_needsElevationBoost;

		private Vector3 m_desiredDir = Vector3.zero;

		public Vector3 CurrentThrusterForce = Vector3.zero;

		public LayerMask LM_GroundCast;

		private RaycastHit m_hit;

		private bool m_hasBegunDestroy;

		private float m_TimeTilAgro = 3f;

		private bool m_isAgro;

		public override void Awake()
		{
			base.Awake();
		}

		public override void Update()
		{
			Sensors.UpdateSensorSystem();
			CombatComputer();
			FlightComputer();
			base.Update();
		}

		public void FixedUpdate()
		{
			ThrustMaster();
		}

		public override void DestroyEvent()
		{
			if (!m_hasBegunDestroy)
			{
				m_hasBegunDestroy = true;
				Invoke("BlowUpAControlBox", Random.Range(0.85f, 1.5f));
				Invoke("BlowUpAControlBox", Random.Range(1.6f, 2.1f));
				Invoke("BlowUpAControlBox", Random.Range(2.2f, 2.7f));
				Invoke("DelayedDestroy", Random.Range(3f, 3.2f));
			}
		}

		private void BlowUpAControlBox()
		{
			int num = Random.Range(0, ControlBoxes.Length);
			if (ControlBoxes[num] != null)
			{
				ControlBoxes[num].DestroyEvent();
			}
		}

		private void DelayedDestroy()
		{
			base.DestroyEvent();
		}

		private void CombatComputer()
		{
			if (Sensors.PriorityTarget != null)
			{
				m_TimeTilAgro -= Time.deltaTime;
				if (m_TimeTilAgro <= 0f)
				{
					m_isAgro = true;
				}
			}
			else
			{
				m_TimeTilAgro = 3f;
				m_isAgro = false;
			}
		}

		private void FlightComputer()
		{
			m_desiredDir = Vector3.zero;
			float b = base.transform.position.y + 50f;
			float num = base.transform.position.y - 50f;
			if (Physics.Raycast(base.transform.position, -Vector3.up, out m_hit, 50f, LM_GroundCast, QueryTriggerInteraction.Ignore))
			{
				num = m_hit.point.y;
			}
			if (Physics.Raycast(base.transform.position, Vector3.up, out m_hit, 50f, LM_GroundCast, QueryTriggerInteraction.Ignore))
			{
				b = m_hit.point.y;
			}
			float num2 = Mathf.Min(num + 1.4f, b);
			if (Sensors.PriorityTarget != null && m_isAgro && !m_hasBegunDestroy)
			{
				Vector3 vector = Sensors.PriorityTarget.LastKnownPosition - base.transform.position;
				vector.y = 0f;
				float maxLength = 0.05f;
				if (vector.magnitude > 12f)
				{
					maxLength = 0.12f;
				}
				m_desiredDir = Vector3.ClampMagnitude(vector, maxLength);
				float a = Mathf.Min(Sensors.PriorityTarget.LastKnownPosition.y, b);
				a = Mathf.Max(a, num2);
				num2 = a;
			}
			if (num2 > base.transform.position.y)
			{
				m_desiredDir += Vector3.up * Mathf.Abs(num2 - base.transform.position.y) * 0.5f;
				m_needsElevationBoost = true;
			}
			else
			{
				m_desiredDir += Vector3.down * Mathf.Abs(num2 - base.transform.position.y) * 0.3f;
				m_needsElevationBoost = true;
			}
		}

		private void ThrustMaster()
		{
			if (!m_needsElevationBoost)
			{
				return;
			}
			bool flag = false;
			float magnitude = 0f;
			Vector3 normalized = m_desiredDir.normalized;
			for (int i = 0; i < ControlBoxes.Length; i++)
			{
				if (ControlBoxes[i] != null && ControlBoxes[i].Thrust(normalized, ref magnitude))
				{
					flag = true;
				}
			}
			if (flag)
			{
				float num = Mathf.Clamp(30f * magnitude, 0f, 100f);
				CurrentThrusterForce = m_desiredDir * num;
				BaseRB.AddForce(CurrentThrusterForce, ForceMode.Acceleration);
				if (m_isAgro || m_hasBegunDestroy)
				{
					m_desiredDir.y = 0f;
					m_desiredDir = Vector3.Cross(m_desiredDir, Vector3.up);
					BaseRB.AddTorque(m_desiredDir * 20f);
				}
			}
		}
	}
}
