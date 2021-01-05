using UnityEngine;

namespace FistVR
{
	public class RessemblerCore : FVRPhysicalObject, IFVRDamageable
	{
		public GameObject ExplosionPrefab;

		private bool m_isExploded;

		public void Damage(Damage d)
		{
			if (!m_isExploded && d.Dam_TotalKinetic > 100f)
			{
				m_isExploded = true;
				if (ExplosionPrefab != null)
				{
					Object.Instantiate(ExplosionPrefab, base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
