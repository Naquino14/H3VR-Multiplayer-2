using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigTurretSpawn : MonoBehaviour
	{
		public List<FVRObject> PossibleTurrets;

		public int IFF;

		public float SpawnChance = 1f;

		private bool m_hasSpawned;

		public void SpawnKernel(float t)
		{
			if (!m_hasSpawned)
			{
				m_hasSpawned = true;
				float num = Random.Range(0f, 1f);
				if (num <= SpawnChance)
				{
					GameObject gameObject = Object.Instantiate(PossibleTurrets[Random.Range(0, PossibleTurrets.Count)].GetGameObject(), base.transform.position + Vector3.up * 0.2f, base.transform.rotation);
					AutoMeater component = gameObject.GetComponent<AutoMeater>();
					component.SetUseFastProjectile(b: true);
					component.E.IFFCode = IFF;
				}
			}
		}
	}
}
