using UnityEngine;

namespace FistVR
{
	public class OmniMortar : MonoBehaviour, IFVRDamageable
	{
		private OmniSpawner_Mortar m_spawner;

		public Rigidbody RB;

		public Rigidbody[] Shards;

		private bool m_isDestroyed;

		private void Awake()
		{
			RB.maxAngularVelocity = 25f;
		}

		public void Configure(OmniSpawner_Mortar spawner, Vector3 pos, Vector3 forward, float vel)
		{
			m_spawner = spawner;
			base.transform.position = pos;
			RB.angularVelocity = Random.Range(1f, 5f) * Random.onUnitSphere;
			RB.velocity = forward * vel;
		}

		private void FixedUpdate()
		{
			Vector3 vector = new Vector3(0f, -9.81f, 0f);
			RB.velocity += vector * Time.deltaTime;
			if (base.transform.position.y <= -100f && !m_isDestroyed)
			{
				m_isDestroyed = true;
				m_spawner.MortarExpired(this);
			}
		}

		public void Damage(Damage dam)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				m_spawner.AddPoints(100);
				for (int i = 0; i < Shards.Length; i++)
				{
					Shards[i].gameObject.SetActive(value: true);
					Shards[i].transform.SetParent(null);
					Shards[i].maxAngularVelocity = 25f;
					Shards[i].velocity = RB.velocity;
					Shards[i].angularVelocity = RB.angularVelocity;
					Shards[i].AddExplosionForce(Random.Range(5f, 20f), dam.point, 1f, 0.1f, ForceMode.Impulse);
				}
				m_spawner.MortarHit(this);
			}
		}
	}
}
