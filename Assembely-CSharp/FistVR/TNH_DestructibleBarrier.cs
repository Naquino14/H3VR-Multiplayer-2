using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class TNH_DestructibleBarrier : MonoBehaviour, IFVRDamageable
	{
		public enum State
		{
			Lowered,
			Lowering,
			Raised,
			Raising
		}

		public State MyState;

		public float Height = 1.1f;

		private Vector3 m_lowerPos;

		private Vector3 m_upperPos;

		private float m_raiseLerp;

		public NavMeshObstacle Obstacle;

		public Rigidbody RB;

		public List<GameObject> Stages;

		public float Damage_PerStage;

		private int m_curStage;

		private float m_damageLeftInStage;

		private bool m_isDestroyed;

		public List<GameObject> SpawnOnDestroy;

		public List<Transform> SpawnOnDestroyPoints;

		private TNH_DestructibleBarrierPoint m_barrierPoint;

		private void Start()
		{
			m_curStage = 0;
			m_damageLeftInStage = Damage_PerStage;
		}

		public void SetBarrierPoint(TNH_DestructibleBarrierPoint b)
		{
			m_barrierPoint = b;
		}

		public void Lower()
		{
			MyState = State.Lowering;
			Obstacle.enabled = false;
		}

		private void FixedUpdate()
		{
			switch (MyState)
			{
			case State.Lowering:
				m_raiseLerp -= Time.deltaTime;
				RB.MovePosition(Vector3.Lerp(m_lowerPos, m_upperPos, m_raiseLerp));
				if (m_raiseLerp <= 0f)
				{
					m_raiseLerp = 0f;
					MyState = State.Lowered;
				}
				break;
			case State.Raising:
				m_raiseLerp += Time.deltaTime;
				RB.MovePosition(Vector3.Lerp(m_lowerPos, m_upperPos, m_raiseLerp));
				if (m_raiseLerp >= 1f)
				{
					m_raiseLerp = 1f;
					MyState = State.Raised;
					Obstacle.enabled = true;
				}
				break;
			}
		}

		private void Update()
		{
			if (MyState == State.Lowered)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public void InitToPlace(Vector3 pos, Vector3 forward)
		{
			m_upperPos = pos;
			m_lowerPos = pos - Vector3.up * Height;
			base.transform.position = m_lowerPos;
			base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			Obstacle.enabled = false;
			MyState = State.Raising;
		}

		public void Damage(Damage d)
		{
			if (MyState != State.Raised || m_isDestroyed)
			{
				return;
			}
			float num = d.Dam_TotalKinetic;
			if (d.Class == FistVR.Damage.DamageClass.Explosive)
			{
				num *= 0.1f;
			}
			else if (d.Class == FistVR.Damage.DamageClass.Melee)
			{
				num *= 0.01f;
			}
			m_damageLeftInStage -= num;
			if (!(m_damageLeftInStage <= 0f))
			{
				return;
			}
			m_damageLeftInStage = Damage_PerStage;
			m_curStage++;
			if (m_curStage < Stages.Count)
			{
				for (int i = 0; i < Stages.Count; i++)
				{
					if (i == m_curStage)
					{
						Stages[i].SetActive(value: true);
					}
					else
					{
						Stages[i].SetActive(value: false);
					}
				}
			}
			else
			{
				Destroy(d);
			}
		}

		private void Destroy(Damage dam)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				for (int i = 0; i < SpawnOnDestroy.Count; i++)
				{
					GameObject gameObject = Object.Instantiate(SpawnOnDestroy[i], SpawnOnDestroyPoints[i].position, SpawnOnDestroyPoints[i].rotation);
					Rigidbody component = gameObject.GetComponent<Rigidbody>();
					Vector3 force = Vector3.Lerp(gameObject.transform.position - dam.point, dam.strikeDir, 0.5f).normalized * Random.Range(1f, 10f);
					component.AddForceAtPosition(force, dam.point, ForceMode.Impulse);
				}
				m_barrierPoint.BarrierDestroyed();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
