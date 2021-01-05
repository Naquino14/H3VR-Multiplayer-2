using UnityEngine;

namespace FistVR
{
	public class PowerUp_Cyclops : MonoBehaviour
	{
		public Transform Beam;

		public LayerMask CastMask;

		private RaycastHit m_hit;

		public ParticleSystem HitParticles;

		public float Thickness = 0.15f;

		private void Update()
		{
			if (Physics.Raycast(base.transform.position, base.transform.forward, out m_hit, 500f, CastMask, QueryTriggerInteraction.Ignore))
			{
				Vector3 point = m_hit.point;
				HitParticles.transform.position = m_hit.point;
				HitParticles.Emit(1);
				Beam.transform.localScale = new Vector3(0.15f, 0.15f, m_hit.distance);
				if (m_hit.collider.attachedRigidbody != null && m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>() != null)
				{
					if ((bool)m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>())
					{
						FXM.Ignite(m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>(), 1f);
					}
					Damage damage = new Damage();
					float cyclopsPower = GM.CurrentPlayerBody.GetCyclopsPower();
					damage.Class = Damage.DamageClass.Projectile;
					damage.Dam_Piercing = 1f * cyclopsPower;
					damage.Dam_TotalKinetic = 1f * cyclopsPower;
					damage.Dam_Thermal = 50f * cyclopsPower;
					damage.Dam_TotalEnergetic = 50f * cyclopsPower;
					damage.point = m_hit.point;
					damage.hitNormal = m_hit.normal;
					damage.strikeDir = base.transform.forward;
					m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>().Damage(damage);
				}
			}
			else
			{
				Vector3 point = base.transform.position + base.transform.forward * 500f;
				HitParticles.transform.position = point;
				Beam.transform.localScale = new Vector3(Thickness, Thickness, 501f);
			}
		}
	}
}
