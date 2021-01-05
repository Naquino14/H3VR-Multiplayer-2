// Decompiled with JetBrains decompiler
// Type: FistVR.ReturnRotwienersSaveGames
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      if (i != 1 && i != 2 && i != 3)
        return;
      this.CurrentSaveGame = i;
    }

    public void DeleteSaveGame(int i)
    {
      switch (i)
      {
        case 0:
          this.SaveGame1.Clear();
          break;
        case 1:
          this.SaveGame2.Clear();
          break;
        case 2:
          this.SaveGame3.Clear();
          break;
      }
    }

    public void WriteToSaveFromFlagM(Dictionary<string, int> flags)
    {
      switch (this.CurrentSaveGame)
      {
        case 1:
          using (Dictionary<string, int>.Enumerator enumerator = flags.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<string, int> current = enumerator.Current;
              if (this.SaveGame1.ContainsKey(current.Key))
                this.SaveGame1[current.Key] = current.Value;
              else
                this.SaveGame1.Add(current.Key, current.Value);
            }
            break;
          }
        case 2:
          using (Dictionary<string, int>.Enumerator enumerator = flags.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<string, int> current = enumerator.Current;
              if (this.SaveGame2.ContainsKey(current.Key))
                this.SaveGame2[current.Key] = current.Value;
              else
                this.SaveGame2.Add(current.Key, current.Value);
            }
            break;
          }
        case 3:
          using (Dictionary<string, int>.Enumerator enumerator = flags.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<string, int> current = enumerator.Current;
              if (this.SaveGame3.ContainsKey(current.Key))
                this.SaveGame3[current.Key] = current.Value;
              else
                this.SaveGame3.Add(current.Key, current.Value);
            }
            break;
          }
      }
    }

    public void InitializeFromSaveFile()
    {
      if (ES2.Exists("ROTRWv2.txt"))
      {
        Debug.Log((object) "ROTRWv2.txt exists, initializing from it");
        using (ES2Reader es2Reader = ES2Reader.Create("ROTRWv2.txt"))
        {
          if (es2Reader.TagExists("SaveGame1"))
            this.SaveGame1 = es2Reader.ReadDictionary<string, int>("SaveGame1");
          if (es2Reader.TagExists("SaveGame2"))
            this.SaveGame2 = es2Reader.ReadDictionary<string, int>("SaveGame2");
          if (es2Reader.TagExists("SaveGame3"))
            this.SaveGame3 = es2Reader.ReadDictionary<string, int>("SaveGame3");
          if (es2Reader.TagExists("HBC"))
            this.HBC = es2Reader.Read<bool>("HBC");
          if (es2Reader.TagExists("HBA"))
            this.HBA = es2Reader.Read<bool>("HBA");
          if (!es2Reader.TagExists("HBH"))
            return;
          this.HBH = es2Reader.Read<bool>("HBH");
        }
      }
      else
      {
        Debug.Log((object) "ROTRWv2.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer es2Writer = ES2Writer.Create("ROTRWv2.txt"))
      {
        es2Writer.Write<string, int>(this.SaveGame1, "SaveGame1");
        es2Writer.Write<string, int>(this.SaveGame2, "SaveGame2");
        es2Writer.Write<string, int>(this.SaveGame3, "SaveGame3");
        es2Writer.Write<bool>(this.HBC, "HBC");
        es2Writer.Write<bool>(this.HBA, "HBA");
        es2Writer.Write<bool>(this.HBH, "HBH");
        es2Writer.Save();
      }
    }
  }
}
