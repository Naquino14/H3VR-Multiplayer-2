using UnityEngine;

namespace FistVR
{
	public class MM_EAPACrate : MonoBehaviour, IFVRDamageable
	{
		public Transform[] SpawnPoints;

		public Rigidbody[] Shards;

		private bool m_isDestroyed;

		public GameObject ShatterFX_Prefab;

		public Transform ShatterFX_Point;

		private GameObject go1;

		private GameObject go2;

		private GameObject go3;

		private GameObject go4;

		private GameObject go5;

		public void Damage(Damage d)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				Destroy();
			}
		}

		public void SetGOs(GameObject g1, GameObject g2, GameObject g3, GameObject g4, GameObject g5)
		{
			go1 = g1;
			go2 = g2;
			go3 = g3;
			go4 = g4;
			go5 = g5;
		}

		private void Destroy()
		{
			Object.Instantiate(ShatterFX_Prefab, ShatterFX_Point.position, ShatterFX_Point.rotation);
			for (int i = 0; i < Shards.Length; i++)
			{
				Shards[i].transform.SetParent(null);
				Shards[i].gameObject.SetActive(value: true);
			}
			if (go1 != null)
			{
				Object.Instantiate(go1, SpawnPoints[0].position, SpawnPoints[0].rotation);
			}
			if (go2 != null)
			{
				Object.Instantiate(go2, SpawnPoints[0].position, SpawnPoints[0].rotation);
			}
			if (go3 != null)
			{
				Object.Instantiate(go3, SpawnPoints[0].position, SpawnPoints[0].rotation);
			}
			if (go4 != null)
			{
				Object.Instantiate(go4, SpawnPoints[0].position, SpawnPoints[0].rotation);
			}
			if (go5 != null)
			{
				Object.Instantiate(go5, SpawnPoints[0].position, SpawnPoints[0].rotation);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
