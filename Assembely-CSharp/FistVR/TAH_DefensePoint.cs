using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TAH_DefensePoint : MonoBehaviour
	{
		[Serializable]
		public class TAH_BotSpawner
		{
			public List<Transform> SpawnPoints = new List<Transform>();

			public TAH_BotSpawnProfile SpawnProfile;

			[HideInInspector]
			public int NumLeftToSpawn;

			[HideInInspector]
			public float TimeTilSpawnBot;

			[HideInInspector]
			public int NumSpawnPointsToUse = 1;

			[HideInInspector]
			public float SpawnCooldownTime = 1f;

			public void Init(TAH_WaveDefinition waveDef)
			{
				NumLeftToSpawn = waveDef.NumBots;
				TimeTilSpawnBot = waveDef.WarmUpToSpawnTime;
				NumSpawnPointsToUse = waveDef.NumSpawnPointsToUse;
				SpawnProfile = waveDef.BotSpawnProfiles[UnityEngine.Random.Range(0, waveDef.BotSpawnProfiles.Count)];
				SpawnCooldownTime = waveDef.SpawnCooldownTime;
			}

			public void End()
			{
				NumLeftToSpawn = 0;
				TimeTilSpawnBot = 0f;
				NumSpawnPointsToUse = 1;
			}

			public int GetSpawnPointIndex()
			{
				return UnityEngine.Random.Range(0, Mathf.Min(SpawnPoints.Count, NumSpawnPointsToUse));
			}
		}

		public TAH_BotSpawner Spawner;

		public GameObject NavBlock;

		public GameObject PointCircle;

		public Transform[] StaticBotSpawnPoints;

		public wwBotWurstNavPointGroup NavGroup;

		public GameObject BeginTouch;

		public void Awake()
		{
			PointCircle.SetActive(value: false);
			BeginTouch.SetActive(value: false);
		}

		public void InitiateDefense()
		{
			NavBlock.SetActive(value: true);
			BeginTouch.SetActive(value: false);
		}

		public void EndDefense()
		{
			NavBlock.SetActive(value: false);
			PointCircle.SetActive(value: false);
		}

		public void BeginWave(TAH_WaveDefinition waveDef)
		{
			Spawner.Init(waveDef);
		}

		public void EndWave()
		{
			Spawner.End();
		}
	}
}
