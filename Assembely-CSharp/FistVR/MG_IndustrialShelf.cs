using UnityEngine;

namespace FistVR
{
	public class MG_IndustrialShelf : MonoBehaviour
	{
		public Transform[] BoxSpawnPoints;

		public float Incidence = 0.5f;

		public GameObject[] BoxPrefabs;

		public void Init()
		{
			for (int i = 0; i < BoxSpawnPoints.Length; i++)
			{
				float num = Random.Range(0f, 1f);
				if (num <= Incidence)
				{
					Object.Instantiate(BoxPrefabs[Random.Range(0, BoxPrefabs.Length)], BoxSpawnPoints[i].position, BoxSpawnPoints[i].rotation);
				}
			}
		}
	}
}
