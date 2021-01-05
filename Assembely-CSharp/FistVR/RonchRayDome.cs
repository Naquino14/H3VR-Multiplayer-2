using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RonchRayDome : MonoBehaviour, IFVRDamageable
	{
		public float LifeRemaining = 50000f;

		private float m_startingLife;

		public List<GameObject> SpawnOnDestruction;

		private bool m_isDestroyed;

		public Transform RayDomeSightPose;

		public void Start()
		{
			m_startingLife = LifeRemaining;
		}

		public float GetLifeRatio()
		{
			return Mathf.Clamp(LifeRemaining / m_startingLife, 0f, 1f);
		}

		public void Damage(Damage d)
		{
			float dam_Blunt = d.Dam_Blunt;
			dam_Blunt += d.Dam_Cutting;
			dam_Blunt += d.Dam_Piercing;
			dam_Blunt += d.Dam_Thermal;
			if (d.Class == FistVR.Damage.DamageClass.Explosive)
			{
				dam_Blunt *= 3.25f;
			}
			if (dam_Blunt > 10000f)
			{
				dam_Blunt -= 10000f;
				if (d.Class == FistVR.Damage.DamageClass.Projectile)
				{
					dam_Blunt *= 0.07f;
				}
			}
			else
			{
				dam_Blunt *= 0.025f;
			}
			LifeRemaining -= dam_Blunt;
			if (LifeRemaining <= 0f)
			{
				Explode();
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
				Object.Destroy(base.gameObject);
			}
		}
	}
}
