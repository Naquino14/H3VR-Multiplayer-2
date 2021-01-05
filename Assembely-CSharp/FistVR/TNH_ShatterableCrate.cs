using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_ShatterableCrate : MonoBehaviour, IFVRDamageable
	{
		public float DamTilDestroyed = 2500f;

		[Header("SubParts")]
		public List<GameObject> SpawnOnDestruction;

		public List<Transform> SpawnPoints;

		public GameObject ParticlePrefab_Full;

		public GameObject ParticlePrefab_Empty;

		[Header("Sound")]
		public bool SoundOnDamage;

		public AudioEvent AudEvent_SoundOnDamage;

		private bool m_isDestroyed;

		private float m_tickDownTilCanBeDamaged = 5f;

		private float damRefireLimited;

		private TNH_Manager m_m;

		private bool m_isHoldingHealth;

		private bool m_isHoldingToken;

		public GameObject OverrideToken;

		public GameObject HealthToken;

		public void Start()
		{
		}

		public void Update()
		{
			if (m_tickDownTilCanBeDamaged > 0f)
			{
				m_tickDownTilCanBeDamaged -= Time.deltaTime;
			}
			if (damRefireLimited > 0f)
			{
				damRefireLimited -= Time.deltaTime;
			}
		}

		public void SetHoldingHealth(TNH_Manager m)
		{
			m_m = m;
			m_isHoldingHealth = true;
		}

		public void SetHoldingToken(TNH_Manager m)
		{
			m_m = m;
			m_isHoldingToken = true;
		}

		public void Damage(Damage d)
		{
			if (!(m_tickDownTilCanBeDamaged > 0f) && !m_isDestroyed)
			{
				damRefireLimited = 0.05f;
				if (damRefireLimited <= 0f && SoundOnDamage)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_SoundOnDamage, base.transform.position);
				}
				float num = d.Dam_TotalKinetic;
				if (d.Class == FistVR.Damage.DamageClass.Explosive)
				{
					num *= 0.1f;
				}
				else if (d.Class == FistVR.Damage.DamageClass.Melee)
				{
					num *= 2f;
				}
				DamTilDestroyed -= num;
				if (DamTilDestroyed < 0f)
				{
					Destroy(d);
				}
			}
		}

		private void Destroy(Damage dam)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				for (int i = 0; i < SpawnOnDestruction.Count; i++)
				{
					GameObject gameObject = Object.Instantiate(SpawnOnDestruction[i], SpawnPoints[i].position, SpawnPoints[i].rotation);
					Rigidbody component = gameObject.GetComponent<Rigidbody>();
					Vector3 force = Vector3.Lerp(gameObject.transform.position - dam.point, dam.strikeDir, 0.5f).normalized * Random.Range(1f, 10f);
					component.AddForceAtPosition(force, dam.point, ForceMode.Impulse);
				}
				Vector3 forward = Vector3.up;
				if (dam.strikeDir.magnitude > 0f)
				{
					forward = dam.strikeDir;
				}
				if (m_isHoldingToken || m_isHoldingHealth)
				{
					GameObject gameObject2 = Object.Instantiate(ParticlePrefab_Full, base.transform.position, Quaternion.LookRotation(forward));
				}
				else
				{
					GameObject gameObject3 = Object.Instantiate(ParticlePrefab_Empty, base.transform.position, Quaternion.LookRotation(forward));
				}
				if (m_isHoldingToken)
				{
					GameObject gameObject4 = Object.Instantiate(OverrideToken, base.transform.position, Quaternion.identity);
					TNH_Token component2 = gameObject4.GetComponent<TNH_Token>();
					component2.M = m_m;
				}
				else if (m_isHoldingHealth)
				{
					GameObject gameObject5 = Object.Instantiate(HealthToken, base.transform.position, Quaternion.identity);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
