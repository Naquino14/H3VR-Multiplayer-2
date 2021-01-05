using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class MeatmasFlags
	{
		public bool[] MMMisKnown = new bool[18];

		public int[] MMMTCs = new int[18];

		public bool[] MMMiners = new bool[6];

		public bool[] EAPAUnlocks = new bool[16];

		public Dictionary<string, int> Hats = new Dictionary<string, int>();

		public int GB = 250;

		public List<int> WPH = new List<int>();

		public bool hasGenWPH;

		public void UnlockEAPA(int i)
		{
			EAPAUnlocks[i] = true;
		}

		public bool IsEAPAUnlocked(int i)
		{
			return EAPAUnlocks[i];
		}

		public void CollectCurrency(MMCurrency c, int amount)
		{
			MMMisKnown[(int)c] = true;
			int num = MMMTCs[(int)c];
			num += amount;
			MMMTCs[(int)c] = num;
		}

		public void RemoveCurrency(MMCurrency c, int i)
		{
			int num = MMMTCs[(int)c];
			num -= i;
			MMMTCs[(int)c] = num;
		}

		public bool HasCurrency(MMCurrency c)
		{
			int num = MMMTCs[(int)c];
			if (num > 0)
			{
				return true;
			}
			return false;
		}

		public bool HasCurrency(MMCurrency c, int amount)
		{
			int num = MMMTCs[(int)c];
			Debug.Log("Have:" + num + " Amount:" + amount);
			if (num >= amount)
			{
				return true;
			}
			return false;
		}

		public void LearnCurrency(MMCurrency c)
		{
			MMMisKnown[(int)c] = true;
		}

		public bool IsCurrencyKnown(MMCurrency c)
		{
			if (MMMisKnown[(int)c])
			{
				return true;
			}
			return false;
		}

		public void AddHat(string h)
		{
			if (Hats.ContainsKey(h))
			{
				Hats[h] += 1;
			}
			else
			{
				Hats.Add(h, 1);
			}
		}

		public void RemoveHat(string h)
		{
			if (Hats.ContainsKey(h))
			{
				Hats[h] -= 1;
				if (Hats[h] <= 0)
				{
					Hats.Remove(h);
				}
			}
		}

		public bool HasHat(string h)
		{
			if (Hats.ContainsKey(h) && Hats[h] > 0)
			{
				return true;
			}
			return false;
		}

		public int NumHat(string h)
		{
			if (!Hats.ContainsKey(h))
			{
				return 0;
			}
			return Hats[h];
		}

		public void AGB(int i)
		{
			GB += i;
		}

		public void SGB(int i)
		{
			GB -= i;
		}

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("MMF.txt"))
			{
				Debug.Log("MMF.txt exists, initializing from it");
				using ES2Reader eS2Reader = ES2Reader.Create("MMF.txt");
				if (eS2Reader.TagExists("MMMisKnown"))
				{
					MMMisKnown = eS2Reader.ReadArray<bool>("MMMisKnown");
				}
				if (eS2Reader.TagExists("MMMTCs"))
				{
					MMMTCs = eS2Reader.ReadArray<int>("MMMTCs");
				}
				if (eS2Reader.TagExists("MMMiners"))
				{
					MMMiners = eS2Reader.ReadArray<bool>("MMMiners");
				}
				if (eS2Reader.TagExists("EAPAUnlocks"))
				{
					EAPAUnlocks = eS2Reader.ReadArray<bool>("EAPAUnlocks");
				}
				if (eS2Reader.TagExists("Hats"))
				{
					Hats = eS2Reader.ReadDictionary<string, int>("Hats");
				}
				if (eS2Reader.TagExists("GB"))
				{
					GB = eS2Reader.Read<int>("GB");
				}
			}
			else
			{
				Debug.Log("MMF.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("MMF.txt");
			eS2Writer.Write(MMMTCs, "MMMTCs");
			eS2Writer.Write(MMMisKnown, "MMMisKnown");
			eS2Writer.Write(MMMiners, "MMMiners");
			eS2Writer.Write(EAPAUnlocks, "EAPAUnlocks");
			eS2Writer.Write(Hats, "Hats");
			eS2Writer.Write(GB, "GB");
			eS2Writer.Save();
		}
	}
}
