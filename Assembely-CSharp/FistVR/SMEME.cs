using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class SMEME : MonoBehaviour
	{
		public enum HatRarity
		{
			Case,
			Key,
			WellDone,
			MemeWell,
			Meme,
			MemeRare,
			Rare,
			BlackAndBlue
		}

		public Text LBL_Money;

		public List<SMEMErow> Rows_Inventory;

		public List<SMEMErow> Rows_MarketPlace;

		public List<ItemSpawnerID> DontUnlock;

		public List<ItemSpawnerID> IDs;

		public List<HatRarity> Rarities;

		private List<int> m_mp_quantities = new List<int>();

		private List<int> m_mp_buycosts = new List<int>();

		private List<int> m_mp_sellcosts = new List<int>();

		public List<ItemSpawnerID> H_BB;

		public List<ItemSpawnerID> H_RA;

		public List<ItemSpawnerID> H_MR;

		public List<ItemSpawnerID> H_ME;

		public List<ItemSpawnerID> H_MW;

		public List<ItemSpawnerID> H_WD;

		private List<float> m_c = new List<float>
		{
			-1f,
			-1f,
			0f,
			80f,
			90f,
			96f,
			99f,
			99.75f
		};

		public AudioEvent AudEvent_click;

		public Transform SpawnPoint;

		[Header("Inventory")]
		public Text PageNumberCurrent_Inventory;

		private int m_inventory_page;

		private List<ItemSpawnerID> m_inventory = new List<ItemSpawnerID>();

		private List<int> m_inventory_indicies = new List<int>();

		[Header("Marketplace")]
		public Text PageNumberCurrent_Marketplace;

		private int m_market_page;

		private void Start()
		{
			GenerateInitialMarketPlace();
			DrawGlobal();
			UpdateInventory();
			m_inventory_page = 0;
			DrawInventory();
			m_market_page = 0;
			DrawMarketPlace();
		}

		private void GenerateInitialMarketPlace()
		{
			for (int i = 0; i < IDs.Count; i++)
			{
				switch (Rarities[i])
				{
				case HatRarity.Case:
					m_mp_quantities.Add(Random.Range(5100, 87000));
					m_mp_buycosts.Add(Random.Range(119, 147));
					m_mp_sellcosts.Add(Random.Range(79, 94));
					break;
				case HatRarity.Key:
					m_mp_quantities.Add(Random.Range(15100, 54000));
					m_mp_buycosts.Add(Random.Range(249, 249));
					m_mp_sellcosts.Add(Random.Range(179, 239));
					break;
				case HatRarity.WellDone:
					m_mp_quantities.Add(Random.Range(87100, 230000));
					m_mp_buycosts.Add(Random.Range(170, 189));
					m_mp_sellcosts.Add(Random.Range(19, 35));
					break;
				case HatRarity.MemeWell:
					m_mp_quantities.Add(Random.Range(32100, 69000));
					m_mp_buycosts.Add(Random.Range(340, 490));
					m_mp_sellcosts.Add(Random.Range(125, 179));
					break;
				case HatRarity.Meme:
					m_mp_quantities.Add(Random.Range(15100, 34000));
					m_mp_buycosts.Add(Random.Range(890, 1530));
					m_mp_sellcosts.Add(Random.Range(289, 689));
					break;
				case HatRarity.MemeRare:
					m_mp_quantities.Add(Random.Range(7500, 17000));
					m_mp_buycosts.Add(Random.Range(2500, 4500));
					m_mp_sellcosts.Add(Random.Range(1200, 1900));
					break;
				case HatRarity.Rare:
					m_mp_quantities.Add(Random.Range(3500, 7000));
					m_mp_buycosts.Add(Random.Range(4500, 9500));
					m_mp_sellcosts.Add(Random.Range(2800, 3700));
					break;
				case HatRarity.BlackAndBlue:
					m_mp_quantities.Add(Random.Range(600, 1500));
					m_mp_buycosts.Add(Random.Range(13000, 37000));
					m_mp_sellcosts.Add(Random.Range(8500, 11000));
					break;
				}
			}
		}

		private void TickMarket()
		{
		}

		private bool CanBuy(int i)
		{
			if (GM.MMFlags.GB >= i)
			{
				return true;
			}
			return false;
		}

		public void BTN_Spawn(int i)
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
			ItemSpawnerID iD = Rows_Inventory[i].GetID();
			FVRObject mainObject = iD.MainObject;
			GameObject gameObject = Object.Instantiate(mainObject.GetGameObject(), SpawnPoint.position, SpawnPoint.rotation);
			if (DontUnlock.Contains(iD))
			{
				GM.MMFlags.RemoveHat(iD.ItemID);
				GM.MMFlags.SaveToFile();
				if (!GM.MMFlags.HasHat(iD.ItemID))
				{
					GM.Rewards.RewardUnlocks.LockReward(iD.ItemID);
					GM.Rewards.SaveToFile();
				}
				UpdateInventory();
				DrawInventory();
			}
			GronchHatCase component = gameObject.GetComponent<GronchHatCase>();
			if (component != null)
			{
				Generate(component);
			}
		}

		private void Generate(GronchHatCase c)
		{
			float num = Random.Range(0f, 100f);
			for (int num2 = m_c.Count - 1; num2 >= 2; num2--)
			{
				if (num >= m_c[num2])
				{
					Generate(c, num2);
					break;
				}
			}
		}

		private void Generate(GronchHatCase c, int i)
		{
			switch (i)
			{
			case 2:
				Generate(c, H_WD[Random.Range(0, H_WD.Count)]);
				break;
			case 3:
				Generate(c, H_MW[Random.Range(0, H_MW.Count)]);
				break;
			case 4:
				Generate(c, H_ME[Random.Range(0, H_ME.Count)]);
				break;
			case 5:
				Generate(c, H_MR[Random.Range(0, H_MR.Count)]);
				break;
			case 6:
				Generate(c, H_RA[Random.Range(0, H_RA.Count)]);
				break;
			case 7:
				Generate(c, H_BB[Random.Range(0, H_BB.Count)]);
				break;
			}
		}

		private void Generate(GronchHatCase c, ItemSpawnerID id)
		{
			c.HID = id.ItemID;
		}

		public void BTN_Buy(int i)
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
			ItemSpawnerID iD = Rows_MarketPlace[i].GetID();
			int price = Rows_MarketPlace[i].GetPrice();
			if (CanBuy(price))
			{
				GM.MMFlags.SGB(price);
				GM.MMFlags.AddHat(iD.ItemID);
				GM.MMFlags.SaveToFile();
				if (!DontUnlock.Contains(iD))
				{
					GM.Rewards.RewardUnlocks.UnlockReward(iD.ItemID);
					GM.Rewards.SaveToFile();
				}
				UpdateInventory();
				TickMarket();
				DrawGlobal();
				DrawInventory();
				DrawMarketPlace();
			}
		}

		public void BTN_Sell(int i)
		{
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
			ItemSpawnerID iD = Rows_Inventory[i].GetID();
			int price = Rows_Inventory[i].GetPrice();
			GM.MMFlags.AGB(price);
			GM.MMFlags.RemoveHat(iD.ItemID);
			GM.MMFlags.SaveToFile();
			if (!GM.MMFlags.HasHat(iD.ItemID))
			{
				GM.Rewards.RewardUnlocks.LockReward(iD.ItemID);
				GM.Rewards.SaveToFile();
			}
			UpdateInventory();
			TickMarket();
			DrawGlobal();
			DrawInventory();
			DrawMarketPlace();
		}

		public void DrawGlobal()
		{
			float num = (float)GM.MMFlags.GB * 0.01f;
			LBL_Money.text = "G" + num.ToString("C", new CultureInfo("en-US"));
		}

		public void NextPage_Inventory()
		{
			m_inventory_page++;
			DrawInventory();
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
		}

		public void PrevPage_Inventory()
		{
			m_inventory_page--;
			if (m_inventory_page < 0)
			{
				m_inventory_page = 0;
			}
			DrawInventory();
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
		}

		public void UpdateInventory()
		{
			m_inventory.Clear();
			m_inventory_indicies.Clear();
			for (int i = 0; i < IDs.Count; i++)
			{
				int num = GM.MMFlags.NumHat(IDs[i].ItemID);
				if (num > 0)
				{
					m_inventory.Add(IDs[i]);
					m_inventory_indicies.Add(i);
				}
			}
		}

		public void DrawInventory()
		{
			int num = Mathf.CeilToInt(m_inventory.Count / 7);
			if (m_inventory_page > num)
			{
				m_inventory_page = 9;
			}
			PageNumberCurrent_Inventory.text = (m_inventory_page + 1).ToString();
			int num2 = m_inventory_page * 7;
			int num3 = Mathf.Min(num2 + 7, m_inventory.Count);
			int num4 = num2;
			for (int i = 0; i < Rows_Inventory.Count; i++)
			{
				if (num4 < num3)
				{
					Rows_Inventory[i].gameObject.SetActive(value: true);
					Rows_Inventory[i].SetMarketPlaceListing(m_inventory[num4], Rarities[m_inventory_indicies[num4]], GM.MMFlags.NumHat(m_inventory[num4].ItemID), m_mp_sellcosts[m_inventory_indicies[num4]]);
				}
				else
				{
					Rows_Inventory[i].gameObject.SetActive(value: false);
				}
				num4++;
			}
		}

		public void NextPage_Marketplace()
		{
			m_market_page++;
			DrawMarketPlace();
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
		}

		public void PrevPage_Marketplace()
		{
			m_market_page--;
			if (m_market_page < 0)
			{
				m_market_page = 0;
			}
			DrawMarketPlace();
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_click, base.transform.position);
		}

		private void DrawMarketPlace()
		{
			int num = 9;
			if (m_market_page > num)
			{
				m_market_page = 9;
			}
			PageNumberCurrent_Marketplace.text = (m_market_page + 1).ToString();
			int num2 = m_market_page * 7;
			int num3 = Mathf.Min(num2 + 7, 69);
			int num4 = num2;
			for (int i = 0; i < Rows_MarketPlace.Count; i++)
			{
				if (num4 < num3)
				{
					Rows_MarketPlace[i].gameObject.SetActive(value: true);
					Rows_MarketPlace[i].SetMarketPlaceListing(IDs[num4], Rarities[num4], m_mp_quantities[num4], m_mp_buycosts[num4]);
				}
				else
				{
					Rows_MarketPlace[i].gameObject.SetActive(value: false);
				}
				num4++;
			}
		}
	}
}
