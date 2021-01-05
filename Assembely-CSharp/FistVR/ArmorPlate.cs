using UnityEngine;

namespace FistVR
{
	public class ArmorPlate : MonoBehaviour, IFVRDamageable
	{
		public IcoSphereTarget Core;

		public int Life = 1;

		private Rigidbody rb;

		public float Points;

		private bool m_isDetached;

		private bool m_isDestroyed;

		private bool isDieTicking;

		private float dieTick = 5f;

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				Life -= (int)d.Dam_TotalKinetic;
			}
			if (Life <= 0)
			{
				Boom(getPoints: true);
			}
		}

		public void Update()
		{
			if (isDieTicking)
			{
				dieTick -= Time.deltaTime;
			}
			if (dieTick <= 0f)
			{
				Boom(getPoints: false);
			}
		}

		public void Boom(bool getPoints)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				if (Core != null)
				{
					Core.DamageParticle(base.transform.position, 3);
				}
				if (getPoints && Core != null)
				{
					Core.Game.ScorePoints(Points);
				}
				Object.Destroy(base.gameObject);
			}
		}

		public void Detach(Vector3 vel)
		{
			if (!m_isDetached)
			{
				m_isDetached = true;
				base.transform.SetParent(null);
				rb = base.gameObject.AddComponent<Rigidbody>();
				rb.velocity = vel * Random.Range(1f, 20f);
				rb.angularVelocity = Random.onUnitSphere * 3f;
				rb.useGravity = false;
				base.gameObject.tag = "Harmless";
				isDieTicking = true;
				dieTick = Random.Range(3f, 5f);
			}
		}
	}
}
