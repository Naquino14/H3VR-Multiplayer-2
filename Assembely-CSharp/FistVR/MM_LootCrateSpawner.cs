using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MM_LootCrateSpawner : MonoBehaviour
	{
		public GameObject Prefab_GunCrateSmall;

		public GameObject Prefab_Buy_GunCrateLarge;

		public GameObject Prefab_LootBox;

		public Transform SpawnPoint;

		public AudioSource Aud_Buy;

		public GameObject FXOnSpawnPrefab;

		public TAH_LootTable[] LT_SmallGunCases;

		public TAH_LootTable[] LT_LargeGunCases;

		public TAH_LootTable[] LT_Attachments;

		public Text SAUCEREADOUT;

		private float tick = 0.1f;

		public void Update()
		{
			tick -= Time.deltaTime;
			if (tick <= 0f)
			{
				tick = Random.Range(0.1f, 0.25f);
				SAUCEREADOUT.text = "You Have -- " + GM.Omni.OmniUnlocks.SaucePackets + " -- S.A.U.C.E.";
			}
		}

		public void Buy_GunCrateSmall()
		{
			if (!Aud_Buy.isPlaying && GM.Omni.OmniUnlocks.HasCurrencyForPurchase(100))
			{
				PlaySound();
				GM.Omni.OmniUnlocks.SpendCurrency(100);
				Object.Instantiate(FXOnSpawnPrefab, SpawnPoint.position, SpawnPoint.rotation);
				GameObject gameObject = Object.Instantiate(Prefab_GunCrateSmall, SpawnPoint.position, Random.rotation);
				MM_GunCase component = gameObject.GetComponent<MM_GunCase>();
				SpawnIntoCase(isSmall: true, component);
				GM.Omni.SaveUnlocksToFile();
			}
		}

		public void Buy_GunCrateLarge()
		{
			if (!Aud_Buy.isPlaying && GM.Omni.OmniUnlocks.HasCurrencyForPurchase(250))
			{
				PlaySound();
				GM.Omni.OmniUnlocks.SpendCurrency(250);
				Object.Instantiate(FXOnSpawnPrefab, SpawnPoint.position, SpawnPoint.rotation);
				GameObject gameObject = Object.Instantiate(Prefab_Buy_GunCrateLarge, SpawnPoint.position, Random.rotation);
				MM_GunCase component = gameObject.GetComponent<MM_GunCase>();
				SpawnIntoCase(isSmall: false, component);
				GM.Omni.SaveUnlocksToFile();
			}
		}

		public void Buy_LootBox()
		{
			if (!Aud_Buy.isPlaying && GM.Omni.OmniUnlocks.HasCurrencyForPurchase(1000))
			{
				PlaySound();
				GM.Omni.OmniUnlocks.SpendCurrency(1000);
				Object.Instantiate(FXOnSpawnPrefab, SpawnPoint.position, SpawnPoint.rotation);
				Object.Instantiate(Prefab_LootBox, SpawnPoint.position, SpawnPoint.rotation);
				GM.Omni.SaveUnlocksToFile();
			}
		}

		private void PlaySound()
		{
			if (!Aud_Buy.isPlaying)
			{
				Aud_Buy.pitch += 0.1f;
				if (Aud_Buy.pitch > 1.5f)
				{
					Aud_Buy.pitch = 0.8f;
				}
				Aud_Buy.Play();
			}
		}

		private void SpawnIntoCase(bool isSmall, MM_GunCase c)
		{
			TAH_LootTableEntry tAH_LootTableEntry = ((!isSmall) ? LT_LargeGunCases[Random.Range(0, LT_LargeGunCases.Length)].GetWeightedRandomEntry() : LT_SmallGunCases[Random.Range(0, LT_SmallGunCases.Length)].GetWeightedRandomEntry());
			int attachmentSpawn = tAH_LootTableEntry.AttachmentSpawn;
			GameObject gameObject = null;
			GameObject go_mag = null;
			GameObject go_attach = null;
			GameObject go_attach2 = null;
			GameObject go_attach3 = null;
			gameObject = tAH_LootTableEntry.MainObj.GetGameObject();
			if (tAH_LootTableEntry.SecondaryObj != null)
			{
				go_mag = tAH_LootTableEntry.SecondaryObj.GetGameObject();
			}
			if (attachmentSpawn == 3)
			{
				TAH_LootTableEntry weightedRandomEntry = LT_Attachments[3].GetWeightedRandomEntry();
				go_attach = weightedRandomEntry.MainObj.GetGameObject();
			}
			else if (attachmentSpawn > 0)
			{
				TAH_LootTableEntry weightedRandomEntry2 = LT_Attachments[attachmentSpawn].GetWeightedRandomEntry();
				go_attach2 = weightedRandomEntry2.MainObj.GetGameObject();
				if (weightedRandomEntry2.SecondaryObj != null)
				{
					go_attach3 = weightedRandomEntry2.SecondaryObj.GetGameObject();
				}
			}
			c.PlaceItemsInCrate(gameObject, go_mag, go_attach, go_attach2, go_attach3);
		}
	}
}
