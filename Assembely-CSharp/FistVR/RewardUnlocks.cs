using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class RewardUnlocks
	{
		public HashSet<string> Rewards = new HashSet<string>();

		public bool IsRewardUnlocked(ItemSpawnerID ID)
		{
			if (!ID.IsReward || Rewards.Contains(ID.ItemID))
			{
				return true;
			}
			return false;
		}

		public bool IsRewardUnlocked(string ID)
		{
			if (Rewards.Contains(ID))
			{
				return true;
			}
			return false;
		}

		public void UnlockReward(ItemSpawnerID ID)
		{
			Rewards.Add(ID.ItemID);
		}

		public void UnlockReward(string ID)
		{
			Rewards.Add(ID);
		}

		public void LockReward(string ID)
		{
			if (Rewards.Contains(ID))
			{
				Rewards.Remove(ID);
			}
		}

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("Rewards"))
			{
				Rewards = reader.ReadHashSet<string>("Rewards");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(Rewards, "Rewards");
		}
	}
}
