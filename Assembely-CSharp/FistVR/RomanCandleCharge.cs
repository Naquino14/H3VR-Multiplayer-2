using UnityEngine;

namespace FistVR
{
	public class RomanCandleCharge : MonoBehaviour
	{
		public ParticleSystem[] PSystems;

		public GameObject ImpactEffect;

		private bool hasSploded;

		public float VelocityMin;

		public float VelocityMax;

		public Vector3 velocity;

		public float MinFuseTime;

		public float MaxFuseTime;

		private float m_fuse;

		public LayerMask CollisionLayerMask;

		private RaycastHit m_hit;

		public bool isNewType;

		public GameObject[] ExplosionEffects;

		private void Explode()
		{
			if (!hasSploded)
			{
				hasSploded = true;
				for (int i = 0; i < PSystems.Length; i++)
				{
					ParticleSystem.EmissionModule emission = PSystems[i].emission;
					ParticleSystem.MinMaxCurve rate = emission.rate;
					rate.mode = ParticleSystemCurveMode.Constant;
					rate.constantMax = 0f;
					rate.constantMin = 0f;
					emission.rate = rate;
					PSystems[i].transform.SetParent(null);
					KillAfter killAfter = PSystems[i].gameObject.AddComponent<KillAfter>();
					killAfter.DieTime = 10f;
				}
				Object.Instantiate(ExplosionEffects[Random.Range(0, ExplosionEffects.Length)], base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		public void Fire()
		{
			velocity = base.transform.forward * Random.Range(VelocityMin, VelocityMax);
			m_fuse = Random.Range(MinFuseTime, MaxFuseTime);
		}

		public void Update()
		{
			m_fuse -= Time.deltaTime;
			if (m_fuse <= 0f)
			{
				Explode();
			}
			velocity += Physics.gravity * Time.deltaTime;
			Vector3 worldPosition = base.transform.position + velocity * Time.deltaTime + base.transform.forward * Mathf.Epsilon;
			base.transform.LookAt(worldPosition);
			float magnitude = velocity.magnitude;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out m_hit, magnitude * Time.deltaTime, CollisionLayerMask, QueryTriggerInteraction.Collide))
			{
				if (!float.IsNaN(m_hit.point.x))
				{
					base.transform.position = m_hit.point;
				}
				velocity = Vector3.Reflect(velocity, m_hit.normal);
				m_fuse -= 0.5f;
				if (isNewType = false)
				{
					Object.Instantiate(ImpactEffect, m_hit.point, Quaternion.LookRotation(m_hit.normal));
				}
			}
			else
			{
				base.transform.position += base.transform.forward * magnitude * Time.deltaTime;
			}
		}
	}
}
