using UnityEngine;

namespace FistVR
{
	public class LaserFieldBox : MonoBehaviour, IFVRDamageable
	{
		public LaserField Field;

		public float Life;

		public GameObject SpawnOnDestroy;

		private bool m_isDestroyed;

		public void Damage(Damage d)
		{
			Life -= d.Dam_TotalKinetic;
			if (Life <= 0f)
			{
				Boom();
			}
		}

		private void Boom()
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				Object.Instantiate(SpawnOnDestroy, base.transform.position, base.transform.rotation);
				Field.ShutDown();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
