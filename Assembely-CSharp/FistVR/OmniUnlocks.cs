using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class OmniUnlocks
	{
		public int SaucePackets = 1500;

		public HashSet<string> UnlockedEquipment_Unlocks = new HashSet<string>();

		public bool IsEquipmentUnlocked(ItemSpawnerID ID, bool SandboxMode)
		{
			if (SandboxMode || ID == null)
			{
				return true;
			}
			if (ID.IsUnlockedByDefault)
			{
				return true;
			}
			return UnlockedEquipment_Unlocks.Contains(ID.ItemID);
		}

		public bool IsEquipmentUnlocked(string ID, bool SandboxMode)
		{
			if (SandboxMode || ID == string.Empty || !IM.HasSpawnedID(ID))
			{
				return true;
			}
			ItemSpawnerID spawnerID = IM.GetSpawnerID(ID);
			if (spawnerID.IsUnlockedByDefault)
			{
				return true;
			}
			return UnlockedEquipment_Unlocks.Contains(ID);
		}

		public void UnlockEquipment(ItemSpawnerID ID, bool SandboxMode)
		{
			if (!SandboxMode)
			{
				UnlockedEquipment_Unlocks.Add(ID.ItemID);
			}
		}

		public void GainCurrency(int c)
		{
			SaucePackets += c;
		}

		public void SpendCurrency(int c)
		{
			SaucePackets -= c;
			if (SaucePackets < 0)
			{
				SaucePackets = 0;
			}
		}

		public bool HasCurrencyForPurchase(int c)
		{
			if (SaucePackets >= c)
			{
				return true;
			}
			return false;
		}

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("SaucePackets"))
			{
				SaucePackets = reader.Read<int>("SaucePackets");
			}
			if (reader.TagExists("UnlockedEquipment_Unlocks"))
			{
				UnlockedEquipment_Unlocks = reader.ReadHashSet<string>("UnlockedEquipment_Unlocks");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(SaucePackets, "SaucePackets");
			writer.Write(UnlockedEquipment_Unlocks, "UnlockedEquipment_Unlocks");
		}
	}
}
