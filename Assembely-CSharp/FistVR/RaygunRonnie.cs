using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RaygunRonnie : MonoBehaviour
	{
		public Transform[] SpawnPoints;

		public GameObject Ronnie_Normal;

		public GameObject Ronnie_Boss;

		private List<GameObject> SpawnedEnemies = new List<GameObject>();

		private bool m_HotDogExists;

		public GameObject HotDog;

		public Transform HotDogSpawnPoint;

		private void Awake()
		{
			SpawnEdibleHotDog();
		}

		public void BeginSequence()
		{
			m_HotDogExists = false;
			for (int i = 0; i < SpawnPoints.Length; i++)
			{
				Vector3 position = SpawnPoints[i].position;
				GameObject item = Object.Instantiate(Ronnie_Boss, position, SpawnPoints[i].rotation);
				SpawnedEnemies.Add(item);
				for (int j = 0; j < 5; j++)
				{
					Vector3 vector = Random.onUnitSphere * 2.5f;
					vector.y = 0f;
					item = Object.Instantiate(Ronnie_Normal, SpawnPoints[i].position + vector, SpawnPoints[i].rotation);
					SpawnedEnemies.Add(item);
				}
			}
			SpawnEdibleHotDog();
		}

		public void PlayerDied()
		{
			if (SpawnedEnemies.Count <= 0)
			{
				return;
			}
			for (int num = SpawnedEnemies.Count - 1; num >= 0; num--)
			{
				if (SpawnedEnemies[num] != null)
				{
					Object.Destroy(SpawnedEnemies[num]);
				}
			}
			SpawnedEnemies.Clear();
		}

		public void SpawnEdibleHotDog()
		{
			if (!m_HotDogExists)
			{
				m_HotDogExists = true;
				GameObject gameObject = Object.Instantiate(HotDog, HotDogSpawnPoint.position, HotDogSpawnPoint.rotation);
			}
		}
	}
}
