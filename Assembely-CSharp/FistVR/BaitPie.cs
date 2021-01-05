using UnityEngine;

namespace FistVR
{
	public class BaitPie : FVRPhysicalObject, IFVRDamageable
	{
		public GameObject CloudPrefab;

		private bool m_isExploded;

		public void Damage(Damage d)
		{
			if (!m_isExploded && d.Dam_TotalKinetic > 100f)
			{
				m_isExploded = true;
				Object.Instantiate(CloudPrefab, base.transform.position + Vector3.up * 0.25f, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		public override void OnCollisionEnter(Collision c)
		{
			base.OnCollisionEnter(c);
			if (!m_isExploded && c.relativeVelocity.magnitude > 3f)
			{
				m_isExploded = true;
				Object.Instantiate(CloudPrefab, base.transform.position + Vector3.up * 0.25f, Quaternion.identity);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
