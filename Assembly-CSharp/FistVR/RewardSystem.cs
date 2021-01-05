// Decompiled with JetBrains decompiler
// Type: FistVR.RewardSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
        Debug.Log((object) "Rewards.txt exists, initializing from it");
        using (ES2Reader reader = ES2Reader.Create("Rewards.txt"))
          this.RewardUnlocks.InitializeFromSaveFile(reader);
      }
      else
      {
        Debug.Log((object) "Rewards.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer writer = ES2Writer.Create("Rewards.txt"))
      {
        this.RewardUnlocks.SaveToFile(writer);
        writer.Save();
      }
    }
  }
}
