using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RonchCockpit : MonoBehaviour, IFVRDamageable
	{
		public float LifeRemaining = 100000f;

		private float m_startingLife;

		public List<GameObject> SpawnOnDestruction;

		private bool m_isDestroyed;

		private bool m_canTakeDamage;

		private void Start()
		{
			m_startingLife = LifeRemaining;
		}

		public float GetLifeRatio()
		{
			return Mathf.Clamp(LifeRemaining / m_startingLife, 0f, 1f);
		}

		public void SetCanTakeDamage(bool b)
		{
			m_canTakeDamage = b;
		}

		public void Damage(Damage d)
		{
			if (m_canTakeDamage)
			{
				float dam_Blunt = d.Dam_Blunt;
				dam_Blunt += d.Dam_Cutting;
				dam_Blunt += d.Dam_Piercing;
				dam_Blunt += d.Dam_Thermal;
				if (d.Class == FistVR.Damage.DamageClass.Explosive)
				{
					dam_Blunt *= 1.25f;
				}
				dam_Blunt = ((!(dam_Blunt > 4000f)) ? 0f : (dam_Blunt - 4000f));
				if (d.Class == FistVR.Damage.DamageClass.Projectile)
				{
					dam_Blunt *= 0.1f;
				}
				LifeRemaining -= dam_Blunt;
				if (LifeRemaining <= 0f)
				{
					Explode();
				}
			}
		}

		private void Explode()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				for (int i = 0; i < SpawnOnDestruction.Count; i++)
				{
					Object.Instantiate(SpawnOnDestruction[i], base.transform.position, Random.rotation);
				}
			}
		}
	}
}
