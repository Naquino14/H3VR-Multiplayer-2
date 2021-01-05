using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class RewardSystem
	{
		public RewardUnlocks RewardUnlocks = new RewardUnlocks();

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("Rewards.txt"))
			{
				Debug.Log("Rewards.txt exists, initializing from it");
				using ES2Reader reader = ES2Reader.Create("Rewards.txt");
				RewardUnlocks.InitializeFromSaveFile(reader);
			}
			else
			{
				Debug.Log("Rewards.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("Rewards.txt");
			RewardUnlocks.SaveToFile(eS2Writer);
			eS2Writer.Save();
		}
	}
}
