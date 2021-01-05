using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class TAHFlags
	{
		public int TAHOption_PlayerHealth = 1;

		public int TAHOption_DifficultyProgression;

		public int TAHOption_BotBullets;

		public int TAHOption_Music = 1;

		public int TAHOption_LootProgression;

		public int TAHOption_ItemSpawner;

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("TAH.txt"))
			{
				Debug.Log("TAH.txt exists, initializing from it");
				using ES2Reader eS2Reader = ES2Reader.Create("TAH.txt");
				if (eS2Reader.TagExists("TAHOption_PlayerHealth"))
				{
					TAHOption_PlayerHealth = eS2Reader.Read<int>("TAHOption_PlayerHealth");
				}
				if (eS2Reader.TagExists("TAHOption_DifficultyProgression"))
				{
					TAHOption_DifficultyProgression = eS2Reader.Read<int>("TAHOption_DifficultyProgression");
				}
				if (eS2Reader.TagExists("TAHOption_BotBullets"))
				{
					TAHOption_BotBullets = eS2Reader.Read<int>("TAHOption_BotBullets");
				}
				if (eS2Reader.TagExists("TAHOption_Music"))
				{
					TAHOption_Music = eS2Reader.Read<int>("TAHOption_Music");
				}
				if (eS2Reader.TagExists("TAHOption_LootProgression"))
				{
					TAHOption_LootProgression = eS2Reader.Read<int>("TAHOption_LootProgression");
				}
				if (eS2Reader.TagExists("TAHOption_ItemSpawner"))
				{
					TAHOption_ItemSpawner = eS2Reader.Read<int>("TAHOption_ItemSpawner");
				}
			}
			else
			{
				Debug.Log("TAH.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("TAH.txt");
			eS2Writer.Write(TAHOption_PlayerHealth, "TAHOption_PlayerHealth");
			eS2Writer.Write(TAHOption_DifficultyProgression, "TAHOption_DifficultyProgression");
			eS2Writer.Write(TAHOption_BotBullets, "TAHOption_BotBullets");
			eS2Writer.Write(TAHOption_Music, "TAHOption_Music");
			eS2Writer.Write(TAHOption_LootProgression, "TAHOption_LootProgression");
			eS2Writer.Write(TAHOption_ItemSpawner, "TAHOption_ItemSpawner");
			eS2Writer.Save();
		}
	}
}
