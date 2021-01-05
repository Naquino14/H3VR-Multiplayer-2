using UnityEngine;

namespace FistVR
{
	public class MM_GronchShip_DamagePiece : MonoBehaviour, IFVRDamageable
	{
		private bool m_isDestroyed;

		public float Life = 1000f;

		private Renderer m_rend;

		private Collider m_col;

		public GameObject VFXPrefab;

		public bool IsDestroyed()
		{
			return m_isDestroyed;
		}

		public void Damage(Damage d)
		{
			if (!m_isDestroyed && d.Dam_TotalKinetic > 1f)
			{
				Life -= d.Dam_TotalKinetic;
				if (Life < 0f)
				{
					Explode();
				}
			}
		}

		private void Start()
		{
			m_rend = GetComponent<Renderer>();
			m_col = GetComponent<Collider>();
		}

		private void Explode()
		{
			m_isDestroyed = true;
			Object.Instantiate(VFXPrefab, base.transform.position, base.transform.rotation);
			m_rend.enabled = false;
			m_col.enabled = false;
		}
	}
}
