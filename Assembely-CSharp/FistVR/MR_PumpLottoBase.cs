using UnityEngine;

namespace FistVR
{
	public class MR_PumpLottoBase : MonoBehaviour, IMG_HandlePumpable
	{
		public enum PumpLottoType
		{
			OpenDoor,
			CloseDoor,
			Goof,
			WeinerBot,
			Slicer,
			Loot
		}

		public PumpLottoRoom Room;

		public PumpLottoType Type;

		public Transform SlicerSpawn;

		public Transform WeinerBotSpawn;

		public Transform[] LootSpawnPoint;

		private bool m_hasFired;

		public GameObject SlicerPrefab;

		public GameObject WeinerBotPrefab;

		public GameObject WeinerArriveExplosion;

		public GameObject FlamingMeatPrefab;

		public void SetPLType(PumpLottoType t)
		{
			Type = t;
		}

		public void Pump(float delta)
		{
			switch (Type)
			{
			case PumpLottoType.OpenDoor:
				Room.m_room.OpenDoors(playSound: true);
				break;
			case PumpLottoType.CloseDoor:
				Room.m_room.CloseDoors(playSound: true);
				break;
			case PumpLottoType.Goof:
				if (!m_hasFired)
				{
					SpawnGoof();
				}
				break;
			case PumpLottoType.WeinerBot:
				if (!m_hasFired)
				{
					SpawnSlicer();
				}
				break;
			case PumpLottoType.Slicer:
				if (!m_hasFired)
				{
					SpawnSlicer();
				}
				break;
			case PumpLottoType.Loot:
				if (!m_hasFired)
				{
					SpawnLoot();
				}
				break;
			}
			m_hasFired = true;
		}

		private void SpawnWeinerBot()
		{
			Object.Instantiate(WeinerBotPrefab, WeinerBotSpawn.position, WeinerBotSpawn.rotation);
			Object.Instantiate(WeinerArriveExplosion, WeinerBotSpawn.position, WeinerBotSpawn.rotation);
			GM.MGMaster.Narrator.PlayMonsterCloset();
		}

		private void SpawnSlicer()
		{
			Object.Instantiate(SlicerPrefab, SlicerSpawn.position, SlicerSpawn.rotation);
			GM.MGMaster.Narrator.PlayMonsterRebuilt();
		}

		private void SpawnGoof()
		{
			Object.Instantiate(FlamingMeatPrefab, LootSpawnPoint[0].position, Random.rotation);
			Object.Instantiate(FlamingMeatPrefab, LootSpawnPoint[2].position, Random.rotation);
			Object.Instantiate(FlamingMeatPrefab, LootSpawnPoint[4].position, Random.rotation);
		}

		private void SpawnLoot()
		{
			float num = Random.Range(0f, 1f);
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			GameObject gameObject4 = null;
			GameObject gameObject5 = null;
			if (num > 0.95f)
			{
				GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_RareGun2, LootSpawnPoint);
				return;
			}
			if (num > 0.9f)
			{
				GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_RareGun3, LootSpawnPoint);
				return;
			}
			if (num > 0.8f)
			{
				GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Shotgun3, LootSpawnPoint);
				return;
			}
			if (num > 0.6f)
			{
				GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Shotgun2, LootSpawnPoint);
				return;
			}
			if (num > 0.4f)
			{
				GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Handgun3, LootSpawnPoint);
				return;
			}
			if (num > 0.2f)
			{
				GM.MGMaster.SpawnGunAmmoPairToTransformList(GM.MGMaster.LTEntry_Handgun2, LootSpawnPoint);
				return;
			}
			if (!GM.MGMaster.m_hasSpawnedSeconaryLight)
			{
				GameObject gameObject6 = GM.MGMaster.SpawnLight(LootSpawnPoint[1].position, Random.rotation, isSecondary: true, GM.Options.MeatGrinderFlags.SecondaryLight);
			}
			GM.MGMaster.SpawnObjectAtPlace(GM.MGMaster.LT_Melee.GetRandomObject(), LootSpawnPoint[0]);
		}
	}
}
