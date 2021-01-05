using System;
using UnityEngine;

namespace FistVR
{
	public class OmniDisc : MonoBehaviour, IFVRDamageable
	{
		private bool m_isDestroyed;

		private OmniSpawner_Discs m_spawner;

		public OmniSpawnDef_Discs.DiscType Type;

		private OmniSpawnDef_Discs.DiscMovementPattern m_movePattern;

		private OmniSpawnDef_Discs.DiscMovementStyle m_moveStyle;

		private Vector3 m_startPos;

		private Vector3 m_endPos;

		private Quaternion m_startRot;

		private Quaternion m_endRot;

		private bool m_isMovingIntoPosition;

		private float m_moveLerp;

		private float m_moveSpeed = 1f;

		private int pointsForNormal = 100;

		private int pointsForNoShoot = -50;

		private int pointsForArmored = 1000;

		private int pointsForBullCenter = 200;

		private int pointsForBullInnerRing = 50;

		private int pointsForBullOuterRing = 10;

		private float m_startingArmorLife = 4000f;

		private float ArmoredPointsLife = 4000f;

		private Vector2 m_spawnBounds;

		private float m_tick;

		private Vector3 m_lerpPointA;

		private Vector3 m_lerpPointB;

		public Rigidbody[] Shards;

		private Renderer m_rend;

		public GameObject[] PFXPrefabs;

		public void Init(OmniSpawner_Discs spawner, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, OmniSpawnDef_Discs.DiscMovementPattern pattern, OmniSpawnDef_Discs.DiscMovementStyle moveStyle, Vector2 spawnbounds, float moveSpeed)
		{
			m_spawner = spawner;
			m_startPos = startPos;
			m_endPos = endPos;
			m_startRot = startRot;
			m_endRot = endRot;
			m_isMovingIntoPosition = true;
			m_movePattern = pattern;
			m_spawnBounds = spawnbounds;
			m_moveStyle = moveStyle;
			m_rend = GetComponent<Renderer>();
			m_moveSpeed = moveSpeed;
			InitTick();
		}

		private void InitTick()
		{
			m_lerpPointA = m_endPos;
			m_lerpPointB = m_endPos;
			switch (m_movePattern)
			{
			case OmniSpawnDef_Discs.DiscMovementPattern.Static:
				break;
			case OmniSpawnDef_Discs.DiscMovementPattern.OscillateX:
				m_lerpPointB.x = 0f - m_lerpPointB.x;
				break;
			case OmniSpawnDef_Discs.DiscMovementPattern.OscillateY:
				m_lerpPointB.y = 0f - (m_lerpPointB.y - 1.25f) + 1.25f;
				break;
			case OmniSpawnDef_Discs.DiscMovementPattern.OscillateZ:
				m_lerpPointB.z *= 2f;
				break;
			case OmniSpawnDef_Discs.DiscMovementPattern.OscillateXY:
				m_lerpPointB.x = 0f - m_lerpPointB.x;
				m_lerpPointB.y = 0f - (m_lerpPointB.y - 1.25f) + 1.25f;
				break;
			case OmniSpawnDef_Discs.DiscMovementPattern.ClockwiseRot:
				m_lerpPointB.y -= 1.25f;
				break;
			case OmniSpawnDef_Discs.DiscMovementPattern.CounterClockwiseRot:
				m_lerpPointB.y -= 1.25f;
				break;
			}
		}

		private void Update()
		{
			if (m_isMovingIntoPosition)
			{
				if (m_moveLerp < 1f)
				{
					m_moveLerp += Time.deltaTime * 5f;
				}
				else
				{
					m_moveLerp = 1f;
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_moveLerp);
				base.transform.rotation = Quaternion.Slerp(m_startRot, m_endRot, m_moveLerp);
				if (m_moveLerp >= 1f)
				{
					m_isMovingIntoPosition = false;
				}
				return;
			}
			float num = 0f;
			Vector3 position = Vector3.zero;
			float num2 = 1f;
			switch (m_moveStyle)
			{
			case OmniSpawnDef_Discs.DiscMovementStyle.Linear:
				m_tick += Time.deltaTime * m_moveSpeed;
				if (m_tick > 2f)
				{
					m_tick -= 2f;
				}
				position = Vector3.Lerp(t: (!(m_tick < 1f)) ? (2f - m_tick) : m_tick, a: m_lerpPointA, b: m_lerpPointB);
				break;
			case OmniSpawnDef_Discs.DiscMovementStyle.Sinusoidal:
				m_tick += Time.deltaTime * m_moveSpeed;
				num = Mathf.Sin(m_tick * (float)Math.PI * 0.5f - (float)Math.PI / 2f) * 0.5f + 0.5f;
				position = Vector3.Lerp(m_lerpPointA, m_lerpPointB, num);
				break;
			case OmniSpawnDef_Discs.DiscMovementStyle.Rotational:
			{
				m_tick += Time.deltaTime * 0.25f * m_moveSpeed;
				if (m_tick > 1f)
				{
					m_tick -= 1f;
				}
				num2 = 1f;
				num2 = ((m_movePattern != OmniSpawnDef_Discs.DiscMovementPattern.ClockwiseRot) ? (-1f) : 1f);
				Vector3 vector3 = Quaternion.Euler(0f, 0f, m_tick * 360f * num2) * m_lerpPointB;
				position = vector3 + Vector3.up * 1.25f;
				break;
			}
			case OmniSpawnDef_Discs.DiscMovementStyle.RotationalSwell:
			{
				Vector3 vector = new Vector3(m_lerpPointB.x, m_lerpPointB.y, 0f);
				float z = m_lerpPointB.z;
				float magnitude = vector.magnitude;
				Vector3 normalized = vector.normalized;
				m_tick += Time.deltaTime * 0.25f * m_moveSpeed;
				if (m_tick > 1f)
				{
					m_tick -= 1f;
				}
				num2 = 1f;
				num2 = ((m_movePattern != OmniSpawnDef_Discs.DiscMovementPattern.ClockwiseRot) ? (-1f) : 1f);
				Vector3 vector2 = Quaternion.Euler(0f, 0f, m_tick * 360f * num2) * normalized;
				vector2 = vector2 * ((Mathf.Sin(m_tick * (float)Math.PI * 2f) + 1f) * 0.25f + 0.5f) * magnitude;
				vector2.z = z;
				position = vector2 + Vector3.up * 1.25f;
				break;
			}
			}
			base.transform.position = position;
		}

		private void DeployShards(Vector3 point)
		{
			for (int i = 0; i < Shards.Length; i++)
			{
				Shards[i].gameObject.SetActive(value: true);
				Shards[i].transform.SetParent(null);
				Shards[i].AddExplosionForce(10f, point, 2f, 0.1f, ForceMode.Impulse);
			}
		}

		private void DeployPFX(int i)
		{
			UnityEngine.Object.Instantiate(PFXPrefabs[i], base.transform.position, base.transform.rotation);
		}

		public void Damage(Damage dam)
		{
			if (m_isMovingIntoPosition)
			{
				return;
			}
			switch (Type)
			{
			case OmniSpawnDef_Discs.DiscType.Normal:
				if (!m_isDestroyed)
				{
					m_isDestroyed = true;
					m_spawner.AddPoints(pointsForNormal);
					m_spawner.ClearDisc(this);
					m_spawner.Invoke("PlaySuccessSound", 0.15f);
					DeployShards(dam.point);
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			case OmniSpawnDef_Discs.DiscType.NoShoot:
				if (!m_isDestroyed)
				{
					m_isDestroyed = true;
					m_spawner.AddPoints(pointsForNoShoot);
					m_spawner.ClearDisc(this);
					m_spawner.Invoke("PlayFailureSound", 0.15f);
					DeployShards(dam.point);
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			case OmniSpawnDef_Discs.DiscType.Armored:
				if (dam.Class == FistVR.Damage.DamageClass.Projectile)
				{
					float num = dam.Dam_TotalKinetic;
					ArmoredPointsLife -= num;
					float t = ArmoredPointsLife / m_startingArmorLife;
					Color value = Color.Lerp(new Color(0.2f, 0.4f, 1f, 1f), Color.white, t);
					m_rend.material.SetColor("_Color", value);
					if (ArmoredPointsLife <= 0f && !m_isDestroyed)
					{
						m_isDestroyed = true;
						m_spawner.AddPoints(pointsForArmored);
						m_spawner.ClearDisc(this);
						m_spawner.Invoke("PlaySuccessSound", 0.15f);
						DeployShards(dam.point);
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}
				break;
			case OmniSpawnDef_Discs.DiscType.RedRing:
				if (!m_isDestroyed)
				{
					m_isDestroyed = true;
					if (Vector3.Distance(dam.point, base.transform.position) > 0.25f)
					{
						m_spawner.AddPoints(pointsForNoShoot);
						m_spawner.Invoke("PlayFailureSound", 0.15f);
						DeployPFX(0);
					}
					else
					{
						m_spawner.AddPoints(pointsForNormal);
						m_spawner.Invoke("PlaySuccessSound", 0.15f);
						DeployPFX(1);
					}
					m_spawner.ClearDisc(this);
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			case OmniSpawnDef_Discs.DiscType.Bullseye:
				if (!m_isDestroyed)
				{
					m_isDestroyed = true;
					if (Vector3.Distance(dam.point, base.transform.position) > 0.3f)
					{
						m_spawner.AddPoints(pointsForBullOuterRing);
						DeployPFX(2);
					}
					else if (Vector3.Distance(dam.point, base.transform.position) > 0.1f)
					{
						m_spawner.AddPoints(pointsForBullInnerRing);
						DeployPFX(1);
					}
					else
					{
						m_spawner.AddPoints(pointsForBullCenter);
						DeployPFX(0);
					}
					m_spawner.ClearDisc(this);
					m_spawner.Invoke("PlaySuccessSound", 0.15f);
					DeployShards(dam.point);
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			}
		}
	}
}
