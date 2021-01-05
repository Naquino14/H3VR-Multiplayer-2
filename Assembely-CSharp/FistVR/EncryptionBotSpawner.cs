using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class EncryptionBotSpawner : MonoBehaviour
	{
		public GameObject BotToSpawn;

		public List<Transform> PossibleSpawns;

		public int NumToSpawn;

		public bool Respawns;

		public Vector2 RespawnTimes = new Vector2(180f, 360f);

		private List<GameObject> m_spawns = new List<GameObject>();

		private List<float> m_reSpawnTimes = new List<float>();

		private void Start()
		{
			PossibleSpawns.Shuffle();
			PossibleSpawns.Shuffle();
			for (int i = 0; i < NumToSpawn; i++)
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y = 0f;
				GameObject item = Object.Instantiate(BotToSpawn, PossibleSpawns[i].position, Quaternion.LookRotation(onUnitSphere, Vector3.up));
				m_spawns.Add(item);
				if (Respawns)
				{
					m_reSpawnTimes.Add(Random.Range(RespawnTimes.x, RespawnTimes.y));
				}
			}
		}

		public void ClearAll()
		{
			if (m_spawns.Count > 0)
			{
				for (int num = m_spawns.Count - 1; num >= 0; num--)
				{
					Object.Destroy(m_spawns[num]);
				}
			}
		}

		private void Update()
		{
			if (!Respawns)
			{
				return;
			}
			for (int i = 0; i < m_spawns.Count; i++)
			{
				if (m_spawns[i] == null)
				{
					m_reSpawnTimes[i] -= Time.deltaTime;
					if (m_reSpawnTimes[i] <= 0f)
					{
						PossibleSpawns.Shuffle();
						Vector3 onUnitSphere = Random.onUnitSphere;
						onUnitSphere.y = 0f;
						GameObject value = Object.Instantiate(BotToSpawn, PossibleSpawns[i].position, Quaternion.LookRotation(onUnitSphere, Vector3.up));
						m_spawns[i] = value;
						m_reSpawnTimes[i] = Random.Range(RespawnTimes.x, RespawnTimes.y);
					}
				}
			}
		}
	}
}
