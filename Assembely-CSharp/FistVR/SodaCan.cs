using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SodaCan : FVRPhysicalObject, IFVRDamageable
	{
		[Header("Sodacan Params")]
		public PMat Pmaterial;

		public GameObject SprayPSystemPrefab;

		private List<ParticleSystem> SpraySystems = new List<ParticleSystem>();

		public GameObject RupturePSystemPrefab;

		public GameObject ExplosionPSystemPrefab;

		private GameObject m_currentDisplayGo;

		public GameObject Can_Undamaged;

		public GameObject Can_Crumpled;

		public GameObject Can_RupturedTop;

		public GameObject Can_RupturedBottom;

		public GameObject Can_RupturedSideTop;

		public GameObject Can_RupturedSideBottom;

		public GameObject Can_RupturedSideGlance;

		public GameObject Can_RupturedSideCenter;

		private bool m_hasSploded;

		private bool m_hasRuptured;

		private bool m_isSpraying;

		private float m_sodaPressure = 1f;

		public float MaxSodaForce = 20f;

		public float MaxSprayForce = 1f;

		protected override void Awake()
		{
			base.Awake();
			m_currentDisplayGo = Can_Undamaged;
		}

		public void Damage(Damage dam)
		{
			if (!m_hasSploded && !m_hasRuptured && dam.Dam_TotalKinetic > 400f)
			{
				m_hasSploded = true;
				if (m_sodaPressure > 0.95f)
				{
				}
				Explode(dam.strikeDir);
			}
			else if (!m_hasSploded && !m_hasRuptured && dam.Dam_TotalKinetic > Random.Range(200f, 400f) && m_sodaPressure > 0.5f)
			{
				m_hasRuptured = true;
				Rupture(dam.hitNormal, dam.point, dam.strikeDir);
			}
			else if (!m_hasSploded && !m_hasRuptured && dam.Dam_TotalKinetic >= 50f && m_sodaPressure > 0.1f)
			{
				SetDisplayGo(Can_Crumpled);
				CreateSpewer(-dam.strikeDir, dam.point);
			}
			base.RootRigidbody.AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point, ForceMode.Impulse);
		}

		private void Explode(Vector3 hitNormal)
		{
			if (m_sodaPressure > 0f)
			{
				base.RootRigidbody.AddForceAtPosition(Vector3.up * m_sodaPressure * MaxSodaForce, base.transform.position, ForceMode.Impulse);
				Object.Instantiate(ExplosionPSystemPrefab, base.transform.position, Random.rotation);
			}
			m_sodaPressure = 0f;
			if (Vector3.Dot(hitNormal, base.transform.up) > 0.5f || Vector3.Dot(hitNormal, base.transform.up) < -0.5f)
			{
			}
			if (base.IsHeld)
			{
				m_hand.EndInteractionIfHeld(this);
			}
			Object.Destroy(base.gameObject);
		}

		private void Rupture(Vector3 hitNormal, Vector3 hitPoint, Vector3 force)
		{
			TurnSpewersOff();
			Debug.Log(hitNormal);
			if (m_sodaPressure > 0f)
			{
				base.RootRigidbody.AddForceAtPosition(force.normalized * m_sodaPressure * MaxSodaForce, hitPoint, ForceMode.Impulse);
				Object.Instantiate(RupturePSystemPrefab, hitPoint, Quaternion.LookRotation(-hitNormal, Random.onUnitSphere));
			}
			m_sodaPressure = 0f;
			if (Vector3.Dot(hitNormal.normalized, base.transform.up) > 0.5f)
			{
				SetDisplayGo(Can_RupturedBottom);
				return;
			}
			if (Vector3.Dot(hitNormal.normalized, -base.transform.up) > 0.5f)
			{
				SetDisplayGo(Can_RupturedTop);
				return;
			}
			Vector3 normalized = (hitPoint - base.transform.position).normalized;
			if (Vector3.Dot(normalized, base.transform.up) > 0.3f)
			{
				SetDisplayGo(Can_RupturedSideTop);
				return;
			}
			if (Vector3.Dot(normalized, -base.transform.up) > 0.3f)
			{
				SetDisplayGo(Can_RupturedSideBottom);
				return;
			}
			Debug.Log(Vector3.Dot(-hitNormal, force));
			if (Vector3.Dot(-hitNormal, force) > 0.5f)
			{
				SetDisplayGo(Can_RupturedSideCenter);
			}
			else
			{
				SetDisplayGo(Can_RupturedSideGlance);
			}
		}

		private void CreateSpewer(Vector3 facingDir, Vector3 pos)
		{
			GameObject gameObject = Object.Instantiate(SprayPSystemPrefab, pos, Quaternion.LookRotation(facingDir, Random.onUnitSphere));
			SpraySystems.Add(gameObject.GetComponent<ParticleSystem>());
			gameObject.transform.SetParent(base.transform);
			gameObject.GetComponent<AudioSource>().Play();
			m_isSpraying = true;
		}

		private void TurnSpewersOff()
		{
			m_isSpraying = false;
			for (int i = 0; i < SpraySystems.Count; i++)
			{
				ParticleSystem.EmissionModule emission = SpraySystems[i].emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.mode = ParticleSystemCurveMode.Constant;
				rate.constantMax = 0f;
				rate.constantMin = 0f;
				emission.rate = rate;
				SpraySystems[i].gameObject.GetComponent<AudioSource>().Stop();
			}
		}

		private void SetDisplayGo(GameObject go)
		{
			m_currentDisplayGo.SetActive(value: false);
			m_currentDisplayGo = go;
			m_currentDisplayGo.SetActive(value: true);
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_isSpraying)
			{
				base.RootRigidbody.mass = Mathf.Lerp(0.05f, 0.4f, m_sodaPressure * m_sodaPressure);
				m_sodaPressure -= Time.deltaTime * 0.1f * (float)SpraySystems.Count;
				if (m_sodaPressure <= 0f)
				{
					m_isSpraying = false;
					TurnSpewersOff();
				}
			}
			if (SpraySystems.Count > 0 && m_isSpraying)
			{
				for (int i = 0; i < SpraySystems.Count; i++)
				{
					ParticleSystem.EmissionModule emission = SpraySystems[i].emission;
					ParticleSystem.MinMaxCurve rate = emission.rate;
					rate.mode = ParticleSystemCurveMode.Constant;
					rate.constantMax = 15f * m_sodaPressure;
					rate.constantMin = 15f * m_sodaPressure;
					emission.rate = rate;
					SpraySystems[i].gameObject.GetComponent<AudioSource>().volume = m_sodaPressure;
					base.RootRigidbody.AddForceAtPosition(m_sodaPressure * MaxSprayForce * SpraySystems[i].transform.forward, SpraySystems[i].transform.position);
				}
			}
		}
	}
}
