using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class ReturnRotwienersSaveGames
	{
		public Dictionary<string, int> SaveGame1 = new Dictionary<string, int>();

		public Dictionary<string, int> SaveGame2 = new Dictionary<string, int>();

		public Dictionary<string, int> SaveGame3 = new Dictionary<string, int>();

		public int CurrentSaveGame;

		public bool HBC;

		public bool HBA;

		public bool HBH;

		public void SetCurrentSaveGame(int i)
		{
			if (i == 1 || i == 2 || i == 3)
			{
				CurrentSaveGame = i;
			}
		}

		public void DeleteSaveGame(int i)
		{
			switch (i)
			{
			case 0:
				SaveGame1.Clear();
				break;
			case 1:
				SaveGame2.Clear();
				break;
			case 2:
				SaveGame3.Clear();
				break;
			}
		}

		public void WriteToSaveFromFlagM(Dictionary<string, int> flags)
		{
			switch (CurrentSaveGame)
			{
			case 1:
				foreach (KeyValuePair<string, int> flag in flags)
				{
					if (SaveGame1.ContainsKey(flag.Key))
					{
						SaveGame1[flag.Key] = flag.Value;
					}
					else
					{
						SaveGame1.Add(flag.Key, flag.Value);
					}
				}
				break;
			case 2:
				foreach (KeyValuePair<string, int> flag2 in flags)
				{
					if (SaveGame2.ContainsKey(flag2.Key))
					{
						SaveGame2[flag2.Key] = flag2.Value;
					}
					else
					{
						SaveGame2.Add(flag2.Key, flag2.Value);
					}
				}
				break;
			case 3:
				foreach (KeyValuePair<string, int> flag3 in flags)
				{
					if (SaveGame3.ContainsKey(flag3.Key))
					{
						SaveGame3[flag3.Key] = flag3.Value;
					}
					else
					{
						SaveGame3.Add(flag3.Key, flag3.Value);
					}
				}
				break;
			}
		}

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("ROTRWv2.txt"))
			{
				Debug.Log("ROTRWv2.txt exists, initializing from it");
				using ES2Reader eS2Reader = ES2Reader.Create("ROTRWv2.txt");
				if (eS2Reader.TagExists("SaveGame1"))
				{
					SaveGame1 = eS2Reader.ReadDictionary<string, int>("SaveGame1");
				}
				if (eS2Reader.TagExists("SaveGame2"))
				{
					SaveGame2 = eS2Reader.ReadDictionary<string, int>("SaveGame2");
				}
				if (eS2Reader.TagExists("SaveGame3"))
				{
					SaveGame3 = eS2Reader.ReadDictionary<string, int>("SaveGame3");
				}
				if (eS2Reader.TagExists("HBC"))
				{
					HBC = eS2Reader.Read<bool>("HBC");
				}
				if (eS2Reader.TagExists("HBA"))
				{
					HBA = eS2Reader.Read<bool>("HBA");
				}
				if (eS2Reader.TagExists("HBH"))
				{
					HBH = eS2Reader.Read<bool>("HBH");
				}
			}
			else
			{
				Debug.Log("ROTRWv2.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("ROTRWv2.txt");
			eS2Writer.Write(SaveGame1, "SaveGame1");
			eS2Writer.Write(SaveGame2, "SaveGame2");
			eS2Writer.Write(SaveGame3, "SaveGame3");
			eS2Writer.Write(HBC, "HBC");
			eS2Writer.Write(HBA, "HBA");
			eS2Writer.Write(HBH, "HBH");
			eS2Writer.Save();
		}
	}
}
