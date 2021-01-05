using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_Target : MonoBehaviour, IFVRDamageable
	{
		private bool m_isDestroyed;

		private HG_ModeManager m_manager;

		private HG_Zone m_zone;

		public List<GameObject> SpawnOnDestruction;

		public List<Rigidbody> ActivateOnDestruction;

		public float Life = 1f;

		public bool RequiresMeleeDamage;

		private Damage.DamageClass m_destroyedDamClass = FistVR.Damage.DamageClass.Projectile;

		public HG_Zone GetZone()
		{
			return m_zone;
		}

		public void Init(HG_ModeManager m, HG_Zone z)
		{
			m_manager = m;
			m_zone = z;
		}

		public Damage.DamageClass GetClassThatKilledMe()
		{
			return m_destroyedDamClass;
		}

		public void Damage(Damage dam)
		{
			if (m_isDestroyed || (RequiresMeleeDamage && dam.Class != FistVR.Damage.DamageClass.Melee))
			{
				return;
			}
			Life -= dam.Dam_TotalKinetic;
			Life -= dam.Dam_TotalEnergetic;
			if (Life <= 0f)
			{
				m_destroyedDamClass = dam.Class;
				for (int i = 0; i < SpawnOnDestruction.Count; i++)
				{
					Object.Instantiate(SpawnOnDestruction[i], base.transform.position, base.transform.rotation);
				}
				for (int j = 0; j < ActivateOnDestruction.Count; j++)
				{
					ActivateOnDestruction[j].transform.SetParent(null);
					ActivateOnDestruction[j].gameObject.SetActive(value: true);
				}
				m_isDestroyed = true;
				m_manager.TargetDestroyed(this);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
