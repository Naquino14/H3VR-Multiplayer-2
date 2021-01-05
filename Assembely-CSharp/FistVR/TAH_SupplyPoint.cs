using UnityEngine;

namespace FistVR
{
	public class TAH_SupplyPoint : MonoBehaviour
	{
		public Transform PlayerSpawnPoint;

		public Transform SpawnPos_CrateLarge;

		public Transform SpawnPos_CrateSmall;

		public TAH_WeaponCrate CrateSmall;

		public TAH_WeaponCrate CrateLarge;

		public Transform SpawnPoint_Large1;

		public Transform SpawnPoint_Large2;

		public Transform SpawnPoint_MeleeWeapon;

		public Transform SpawnPos_PowerUp;

		public Transform[] BotSpawnPoints;

		public Transform[] BotAttackSpawnPoints;

		public wwBotWurstNavPointGroup NavGroup;
	}
}
