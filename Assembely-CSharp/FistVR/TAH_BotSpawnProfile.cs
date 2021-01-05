using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Bot Spawn Profile", menuName = "TakeAndHold/TAHBotSpawnProfile", order = 0)]
	public class TAH_BotSpawnProfile : ScriptableObject
	{
		public GameObject[] Prefabs;

		public GameObject[] Weapons;

		public wwBotWurstConfig[] Configs;

		public GameObject[] SecondaryWeapons;

		public float ScaleFactor = 0.9f;

		public GameObject GetRandomPrefab()
		{
			return Prefabs[Random.Range(0, Prefabs.Length)];
		}

		public GameObject GetRandomWeapon()
		{
			return Weapons[Random.Range(0, Weapons.Length)];
		}

		public wwBotWurstConfig GetRandomConfig()
		{
			return Configs[Random.Range(0, Configs.Length)];
		}

		public GameObject GetRandomSecondaryWeapon()
		{
			if (SecondaryWeapons.Length > 0)
			{
				return SecondaryWeapons[Random.Range(0, Weapons.Length)];
			}
			return null;
		}
	}
}
