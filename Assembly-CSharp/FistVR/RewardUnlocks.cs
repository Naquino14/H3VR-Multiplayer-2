// Decompiled with JetBrains decompiler
// Type: FistVR.RewardUnlocks
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class RewardUnlocks
  {
    public HashSet<string> Rewards = new HashSet<string>();

    public bool IsRewardUnlocked(ItemSpawnerID ID) => !ID.IsReward || this.Rewards.Contains(ID.ItemID);

    public bool IsRewardUnlocked(string ID) => this.Rewards.Contains(ID);

    public void UnlockReward(ItemSpawnerID ID) => this.Rewards.Add(ID.ItemID);

    public void UnlockReward(string ID) => this.Rewards.Add(ID);

    public void LockReward(string ID)
    {
      if (!this.Rewards.Contains(ID))
        return;
      this.Rewards.Remove(ID);
    }

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (!reader.TagExists("Rewards"))
        return;
      this.Rewards = reader.ReadHashSet<string>("Rewards");
    }

    public void SaveToFile(ES2Writer writer) => writer.Write<string>(this.Rewards, "Rewards");
  }
}
