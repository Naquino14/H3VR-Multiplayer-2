using UnityEngine;

namespace FistVR
{
	public class FVRProxDetonate : MonoBehaviour, IFVRDamageable
	{
		private bool m_hasExploded;

		public GameObject[] Spawns;

		private void Explode()
		{
			if (!m_hasExploded)
			{
				m_hasExploded = true;
				for (int i = 0; i < Spawns.Length; i++)
				{
					Object.Instantiate(Spawns[i], base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.attachedRigidbody != null && col.attachedRigidbody.velocity.magnitude > 0.1f)
			{
				Explode();
			}
		}

		public void Damage(Damage dam)
		{
			Explode();
		}
	}
}
