using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class F18Manager : MonoBehaviour
	{
		public List<Transform> SpawnPoints;

		public GameObject PlanePrefab;

		public int MaxPlanes = 4;

		private float m_tickTilSpawn = 30f;

		private List<GameObject> Planes = new List<GameObject>();

		private void Start()
		{
			SpawnAttempt();
			SpawnAttempt();
			SpawnAttempt();
			SpawnAttempt();
		}

		private void Update()
		{
			m_tickTilSpawn -= Time.deltaTime;
			if (m_tickTilSpawn < 0f)
			{
				m_tickTilSpawn = Random.Range(15f, 30f);
				SpawnAttempt();
			}
			if (Planes.Count <= 0)
			{
				return;
			}
			for (int num = Planes.Count - 1; num >= 0; num--)
			{
				if (Planes[num] == null)
				{
					Planes.RemoveAt(num);
				}
			}
		}

		private void SpawnAttempt()
		{
			if (Planes.Count < MaxPlanes)
			{
				Transform transform = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
				GameObject item = Object.Instantiate(PlanePrefab, transform.position, transform.rotation);
				Planes.Add(item);
			}
		}
	}
}
