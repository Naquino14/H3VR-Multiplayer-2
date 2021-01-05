using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ClunkSpawner : MonoBehaviour
	{
		public GameObject ClunkPrefabEasy;

		public GameObject ClunkMk3Prefab;

		public Transform[] ClunkSpawnPositions;

		private List<GameObject> Clunks = new List<GameObject>();

		private List<GameObject> Slicers = new List<GameObject>();

		public GameObject Slicer;

		public Transform[] SlicerSpawnPositions;

		private List<GameObject> SoldierBots = new List<GameObject>();

		public wwBotWurstNavPointGroup NavGroup;

		public GameObject[] SoldierBots_Slowbullets;

		public GameObject[] SoldierBots_Fastbullets;

		private int[] Choose3RandomIndicies(int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			for (int j = 0; j < array.Length; j++)
			{
				int num = array[j];
				int num2 = Random.Range(j, length);
				array[j] = array[num2];
				array[num2] = num;
			}
			return array;
		}

		public void SpawnSlicer()
		{
			int[] array = Choose3RandomIndicies(3);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject item = Object.Instantiate(Slicer, SlicerSpawnPositions[array[i]].position, SlicerSpawnPositions[array[i]].rotation);
				Clunks.Add(item);
			}
		}

		public void SpawnClunksEasy()
		{
			int[] array = Choose3RandomIndicies(3);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject item = Object.Instantiate(ClunkPrefabEasy, ClunkSpawnPositions[array[i]].position, ClunkSpawnPositions[array[i]].rotation);
				Clunks.Add(item);
			}
		}

		public void SpawnClunkMk3s()
		{
			int[] array = Choose3RandomIndicies(3);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject item = Object.Instantiate(ClunkMk3Prefab, ClunkSpawnPositions[array[i]].position, ClunkSpawnPositions[array[i]].rotation);
				Clunks.Add(item);
			}
		}

		public void SpawnSoldierBots_SlowBullets()
		{
			int length = Random.Range(3, 7);
			int[] array = Choose3RandomIndicies(length);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(SoldierBots_Slowbullets[Random.Range(0, SoldierBots_Slowbullets.Length)], ClunkSpawnPositions[array[i]].position, ClunkSpawnPositions[array[i]].rotation);
				gameObject.GetComponent<wwBotWurst>().NavPointGroup = NavGroup;
				GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
				SoldierBots.Add(gameObject);
			}
		}

		public void SpawnSoldierBots_FastBullets()
		{
			int length = Random.Range(3, 7);
			int[] array = Choose3RandomIndicies(length);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(SoldierBots_Fastbullets[Random.Range(0, SoldierBots_Fastbullets.Length)], ClunkSpawnPositions[array[i]].position, ClunkSpawnPositions[array[i]].rotation);
				gameObject.GetComponent<wwBotWurst>().NavPointGroup = NavGroup;
				GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
				SoldierBots.Add(gameObject);
			}
		}

		public void DeleteBots()
		{
			if (Clunks.Count > 0)
			{
				for (int num = Clunks.Count - 1; num >= 0; num--)
				{
					Object.Destroy(Clunks[num]);
				}
			}
			Clunks.Clear();
			if (Slicers.Count > 0)
			{
				for (int num2 = Slicers.Count - 1; num2 >= 0; num2--)
				{
					Object.Destroy(Slicers[num2]);
				}
			}
			Slicers.Clear();
			if (SoldierBots.Count > 0)
			{
				for (int num3 = SoldierBots.Count - 1; num3 >= 0; num3--)
				{
					Object.Destroy(SoldierBots[num3]);
				}
			}
			SoldierBots.Clear();
		}

		public void DeleteClunks()
		{
			if (Clunks.Count > 0)
			{
				for (int num = Clunks.Count - 1; num >= 0; num--)
				{
					Object.Destroy(Clunks[num]);
				}
			}
			Clunks.Clear();
		}

		public void ResetSlicers()
		{
			DeleteClunks();
			SpawnSlicer();
		}

		public void ResetClunksEasy()
		{
			DeleteClunks();
			SpawnClunksEasy();
		}

		public void ResetClunkMk3s()
		{
			DeleteClunks();
			SpawnClunkMk3s();
		}

		public void DeleteMagazines()
		{
			FVRFireArmMagazine[] array = Object.FindObjectsOfType<FVRFireArmMagazine>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (!array[num].IsHeld && array[num].QuickbeltSlot == null && array[num].FireArm == null)
				{
					Object.Destroy(array[num].gameObject);
				}
			}
			FVRFireArmRound[] array2 = Object.FindObjectsOfType<FVRFireArmRound>();
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				if (!array2[num2].IsHeld && array2[num2].QuickbeltSlot == null)
				{
					Object.Destroy(array2[num2].gameObject);
				}
			}
		}

		public void PlayerDied()
		{
			DeleteBots();
		}
	}
}
