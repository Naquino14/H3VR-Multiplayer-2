using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PyroSplodeyPack : MonoBehaviour, IFVRDamageable
	{
		public SosigWearable W;

		private bool m_isDestroyed;

		private float m_life = 1500f;

		public Transform SpawnPoint;

		public List<GameObject> SpawnOnBoom;

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				m_life -= d.Dam_TotalKinetic;
				if (m_life < 0f)
				{
					Boom();
				}
			}
		}

		private void Boom()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				W.S.KillSosig();
				for (int i = 0; i < SpawnOnBoom.Count; i++)
				{
					Object.Instantiate(SpawnOnBoom[i], SpawnPoint.position, SpawnPoint.rotation);
				}
			}
		}
	}
}
