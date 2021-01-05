using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class WurstWorldSaveGame
	{
		public WWFlags Flags = new WWFlags();

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("WW.txt"))
			{
				Debug.Log("WW.txt exists, initializing from it");
				using ES2Reader reader = ES2Reader.Create("WW.txt");
				Flags.InitializeFromSaveFile(reader);
			}
			else
			{
				Debug.Log("WW.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("WW.txt");
			Flags.SaveToFile(eS2Writer);
			eS2Writer.Save();
		}
	}
}
