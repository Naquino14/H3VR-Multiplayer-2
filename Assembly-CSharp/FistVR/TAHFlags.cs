// Decompiled with JetBrains decompiler
// Type: FistVR.TAHFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
        Debug.Log((object) "TAH.txt exists, initializing from it");
        using (ES2Reader es2Reader = ES2Reader.Create("TAH.txt"))
        {
          if (es2Reader.TagExists("TAHOption_PlayerHealth"))
            this.TAHOption_PlayerHealth = es2Reader.Read<int>("TAHOption_PlayerHealth");
          if (es2Reader.TagExists("TAHOption_DifficultyProgression"))
            this.TAHOption_DifficultyProgression = es2Reader.Read<int>("TAHOption_DifficultyProgression");
          if (es2Reader.TagExists("TAHOption_BotBullets"))
            this.TAHOption_BotBullets = es2Reader.Read<int>("TAHOption_BotBullets");
          if (es2Reader.TagExists("TAHOption_Music"))
            this.TAHOption_Music = es2Reader.Read<int>("TAHOption_Music");
          if (es2Reader.TagExists("TAHOption_LootProgression"))
            this.TAHOption_LootProgression = es2Reader.Read<int>("TAHOption_LootProgression");
          if (!es2Reader.TagExists("TAHOption_ItemSpawner"))
            return;
          this.TAHOption_ItemSpawner = es2Reader.Read<int>("TAHOption_ItemSpawner");
        }
      }
      else
      {
        Debug.Log((object) "TAH.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer es2Writer = ES2Writer.Create("TAH.txt"))
      {
        es2Writer.Write<int>(this.TAHOption_PlayerHealth, "TAHOption_PlayerHealth");
        es2Writer.Write<int>(this.TAHOption_DifficultyProgression, "TAHOption_DifficultyProgression");
        es2Writer.Write<int>(this.TAHOption_BotBullets, "TAHOption_BotBullets");
        es2Writer.Write<int>(this.TAHOption_Music, "TAHOption_Music");
        es2Writer.Write<int>(this.TAHOption_LootProgression, "TAHOption_LootProgression");
        es2Writer.Write<int>(this.TAHOption_ItemSpawner, "TAHOption_ItemSpawner");
        es2Writer.Save();
      }
    }
  }
}
