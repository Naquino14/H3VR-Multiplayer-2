using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TAH_MobSpawnGroup : MonoBehaviour
	{
		public AnimationCurve SpawnChanceByDifficulty;

		public int BaseWhenSpawning;

		public float DifficultyMultBonus;

		public int MinWhenSpawning;

		public int MaxWhenSpawning;

		public List<Transform> SpawnPoints;

		public List<GameObject> EnemyPrefab;

		public float MinDifficulty;

		public float MinDistance = 20f;

		public bool GetShouldSpawn(float difficulty, Vector3 playerPos)
		{
			if (MinDifficulty > difficulty)
			{
				return false;
			}
			float num = SpawnChanceByDifficulty.Evaluate(difficulty);
			float num2 = Random.Range(0f, 1f);
			if (num2 <= num)
			{
				float num3 = Vector3.Distance(base.transform.position, playerPos);
				if (num3 > MinDistance)
				{
					return true;
				}
			}
			return false;
		}

		public List<GameObject> SpawnMobs(float difficulty)
		{
			List<GameObject> list = new List<GameObject>();
			int num = MinWhenSpawning;
			if (DifficultyMultBonus > 0f)
			{
				num += Mathf.RoundToInt(DifficultyMultBonus * difficulty);
			}
			if (num > MaxWhenSpawning)
			{
				num = MaxWhenSpawning;
			}
			List<Transform> list2 = new List<Transform>();
			for (int i = 0; i < SpawnPoints.Count; i++)
			{
				list2.Add(SpawnPoints[i]);
			}
			for (int j = 0; j < num; j++)
			{
				int index = Random.Range(0, list2.Count);
				GameObject item = Object.Instantiate(EnemyPrefab[Random.Range(0, EnemyPrefab.Count)], list2[index].position, list2[index].rotation);
				list.Add(item);
				list2.RemoveAt(index);
			}
			list2.Clear();
			return list;
		}
	}
}
