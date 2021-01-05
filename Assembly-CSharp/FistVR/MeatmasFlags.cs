// Decompiled with JetBrains decompiler
// Type: FistVR.MeatmasFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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

    public void UnlockEAPA(int i) => this.EAPAUnlocks[i] = true;

    public bool IsEAPAUnlocked(int i) => this.EAPAUnlocks[i];

    public void CollectCurrency(MMCurrency c, int amount)
    {
      this.MMMisKnown[(int) c] = true;
      int num = this.MMMTCs[(int) c] + amount;
      this.MMMTCs[(int) c] = num;
    }

    public void RemoveCurrency(MMCurrency c, int i)
    {
      int num = this.MMMTCs[(int) c] - i;
      this.MMMTCs[(int) c] = num;
    }

    public bool HasCurrency(MMCurrency c) => this.MMMTCs[(int) c] > 0;

    public bool HasCurrency(MMCurrency c, int amount)
    {
      int mmmtC = this.MMMTCs[(int) c];
      Debug.Log((object) ("Have:" + (object) mmmtC + " Amount:" + (object) amount));
      return mmmtC >= amount;
    }

    public void LearnCurrency(MMCurrency c) => this.MMMisKnown[(int) c] = true;

    public bool IsCurrencyKnown(MMCurrency c) => this.MMMisKnown[(int) c];

    public void AddHat(string h)
    {
      if (this.Hats.ContainsKey(h))
        this.Hats[h] = this.Hats[h] + 1;
      else
        this.Hats.Add(h, 1);
    }

    public void RemoveHat(string h)
    {
      if (!this.Hats.ContainsKey(h))
        return;
      this.Hats[h] = this.Hats[h] - 1;
      if (this.Hats[h] > 0)
        return;
      this.Hats.Remove(h);
    }

    public bool HasHat(string h) => this.Hats.ContainsKey(h) && this.Hats[h] > 0;

    public int NumHat(string h) => !this.Hats.ContainsKey(h) ? 0 : this.Hats[h];

    public void AGB(int i) => this.GB += i;

    public void SGB(int i) => this.GB -= i;

    public void InitializeFromSaveFile()
    {
      if (ES2.Exists("MMF.txt"))
      {
        Debug.Log((object) "MMF.txt exists, initializing from it");
        using (ES2Reader es2Reader = ES2Reader.Create("MMF.txt"))
        {
          if (es2Reader.TagExists("MMMisKnown"))
            this.MMMisKnown = es2Reader.ReadArray<bool>("MMMisKnown");
          if (es2Reader.TagExists("MMMTCs"))
            this.MMMTCs = es2Reader.ReadArray<int>("MMMTCs");
          if (es2Reader.TagExists("MMMiners"))
            this.MMMiners = es2Reader.ReadArray<bool>("MMMiners");
          if (es2Reader.TagExists("EAPAUnlocks"))
            this.EAPAUnlocks = es2Reader.ReadArray<bool>("EAPAUnlocks");
          if (es2Reader.TagExists("Hats"))
            this.Hats = es2Reader.ReadDictionary<string, int>("Hats");
          if (!es2Reader.TagExists("GB"))
            return;
          this.GB = es2Reader.Read<int>("GB");
        }
      }
      else
      {
        Debug.Log((object) "MMF.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer es2Writer = ES2Writer.Create("MMF.txt"))
      {
        es2Writer.Write<int>(this.MMMTCs, "MMMTCs");
        es2Writer.Write<bool>(this.MMMisKnown, "MMMisKnown");
        es2Writer.Write<bool>(this.MMMiners, "MMMiners");
        es2Writer.Write<bool>(this.EAPAUnlocks, "EAPAUnlocks");
        es2Writer.Write<string, int>(this.Hats, "Hats");
        es2Writer.Write<int>(this.GB, "GB");
        es2Writer.Save();
      }
    }
  }
}
