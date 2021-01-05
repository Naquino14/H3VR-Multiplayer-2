using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class OmniMaster
	{
		public OmniFlags OmniFlags = new OmniFlags();

		public OmniUnlocks OmniUnlocks = new OmniUnlocks();

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("OmniScores.txt"))
			{
				Debug.Log("OmniScores.txt exists, initializing from it");
				using ES2Reader reader = ES2Reader.Create("OmniScores.txt");
				OmniFlags.InitializeFromSaveFile(reader);
			}
			else
			{
				Debug.Log("OmniScores.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("OmniScores.txt");
			OmniFlags.SaveToFile(eS2Writer);
			eS2Writer.Save();
		}

		public void InitializeUnlocks()
		{
			if (ES2.Exists("OmniUnlocks.txt"))
			{
				Debug.Log("OmniUnlocks.txt exists, initializing from it");
				using ES2Reader reader = ES2Reader.Create("OmniUnlocks.txt");
				OmniUnlocks.InitializeFromSaveFile(reader);
			}
			else
			{
				Debug.Log("OmniUnlocks.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveUnlocksToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("OmniUnlocks.txt");
			OmniUnlocks.SaveToFile(eS2Writer);
			eS2Writer.Save();
		}
	}
}
