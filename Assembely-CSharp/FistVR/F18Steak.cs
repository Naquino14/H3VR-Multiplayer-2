using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class F18Steak : MonoBehaviour, IFVRDamageable
	{
		private Vector3 TargetPoint;

		private Vector3 m_velocity;

		private Vector3 m_forwardDir;

		private Vector3 m_upDir;

		private float m_thrustCapability = 100f;

		private float m_brakingCapability = 40f;

		private float targetPointRange = 2000f;

		private Vector2 targetPointHeightRange = new Vector2(300f, 500f);

		public List<GameObject> SpawnOnExplode;

		private Vector3 m_lastPos;

		private Vector3 m_newPos;

		private bool m_isDestroyed;

		public Rigidbody RB;

		private float m_timeTilNewPoint = 10f;

		private float velClamp = 100f;

		private Vector3 upcur = Vector3.up;

		public void Start()
		{
			m_lastPos = base.transform.position;
			m_newPos = base.transform.position;
			InitiateFlight(GenerateNewPoint());
		}

		public void Damage(Damage d)
		{
			Explode();
		}

		private Vector3 GenerateNewPoint()
		{
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			onUnitSphere *= Random.Range(targetPointRange * 0.8f, targetPointRange);
			onUnitSphere.y = Random.Range(targetPointHeightRange.x, targetPointHeightRange.y);
			m_timeTilNewPoint = 10f;
			return onUnitSphere;
		}

		public void InitiateFlight(Vector3 tPoint)
		{
			TargetPoint = tPoint;
			m_forwardDir = (TargetPoint - base.transform.position).normalized;
			m_upDir = Vector3.up;
			m_velocity = m_forwardDir * 200f;
		}

		private void Update()
		{
			if (m_timeTilNewPoint > 0f)
			{
				m_timeTilNewPoint -= Time.deltaTime;
			}
			else
			{
				TargetPoint = GenerateNewPoint();
			}
			if (!m_isDestroyed)
			{
				Vector3 vector = TargetPoint - base.transform.position;
				Vector3 vector2 = (m_forwardDir = Vector3.RotateTowards(m_forwardDir, vector.normalized, Time.deltaTime * 0.3f, 1f));
				if (m_forwardDir.y < -0.4f)
				{
					m_forwardDir.y = -0.4f;
					m_forwardDir.Normalize();
				}
				if (m_forwardDir.y > 0.4f)
				{
					m_forwardDir.y = 0.4f;
					m_forwardDir.Normalize();
				}
				Vector3 forwardDir = m_forwardDir;
				forwardDir.y = 0f;
				Vector3 vector3 = vector;
				vector3.y = 0f;
				Vector3 vector4 = Vector3.ProjectOnPlane(base.transform.right, Vector3.up);
				vector4.Normalize();
				float t = Mathf.Clamp(Vector3.Angle(forwardDir, vector3) / 90f, 0f, 0.9f);
				if (Vector3.Angle(vector3, vector4) <= 90f)
				{
					upcur = Vector3.Slerp(Vector3.up, vector4, t);
				}
				else
				{
					upcur = Vector3.Slerp(Vector3.up, -vector4, t);
				}
				m_upDir = Vector3.Slerp(m_upDir, upcur, Time.deltaTime);
				base.transform.rotation = Quaternion.LookRotation(m_forwardDir, m_upDir);
				float num = Vector3.Angle(m_forwardDir, vector);
				float t2 = num / 180f;
				float b = Mathf.Lerp(120f, 30f, t2);
				velClamp = Mathf.Lerp(velClamp, b, Time.deltaTime * 2f);
				m_velocity += m_forwardDir * Time.deltaTime * m_thrustCapability;
				m_velocity = Vector3.ClampMagnitude(m_velocity, velClamp);
				RB.AddForce(m_velocity, ForceMode.Acceleration);
				if (vector.magnitude < 100f)
				{
					TargetPoint = GenerateNewPoint();
				}
			}
			Debug.DrawLine(base.transform.position, TargetPoint, Color.red);
		}

		private void Explode()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				for (int i = 0; i < SpawnOnExplode.Count; i++)
				{
					Object.Instantiate(SpawnOnExplode[i], base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
