// Decompiled with JetBrains decompiler
// Type: FistVR.OmniUnlocks
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class OmniUnlocks
  {
    public int SaucePackets = 1500;
    public HashSet<string> UnlockedEquipment_Unlocks = new HashSet<string>();

    public bool IsEquipmentUnlocked(ItemSpawnerID ID, bool SandboxMode) => SandboxMode || (UnityEngine.Object) ID == (UnityEngine.Object) null || ID.IsUnlockedByDefault || this.UnlockedEquipment_Unlocks.Contains(ID.ItemID);

    public bool IsEquipmentUnlocked(string ID, bool SandboxMode) => SandboxMode || ID == string.Empty || (!IM.HasSpawnedID(ID) || IM.GetSpawnerID(ID).IsUnlockedByDefault) || this.UnlockedEquipment_Unlocks.Contains(ID);

    public void UnlockEquipment(ItemSpawnerID ID, bool SandboxMode)
    {
      if (SandboxMode)
        return;
      this.UnlockedEquipment_Unlocks.Add(ID.ItemID);
    }

    public void GainCurrency(int c) => this.SaucePackets += c;

    public void SpendCurrency(int c)
    {
      this.SaucePackets -= c;
      if (this.SaucePackets >= 0)
        return;
      this.SaucePackets = 0;
    }

    public bool HasCurrencyForPurchase(int c) => this.SaucePackets >= c;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("SaucePackets"))
        this.SaucePackets = reader.Read<int>("SaucePackets");
      if (!reader.TagExists("UnlockedEquipment_Unlocks"))
        return;
      this.UnlockedEquipment_Unlocks = reader.ReadHashSet<string>("UnlockedEquipment_Unlocks");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<int>(this.SaucePackets, "SaucePackets");
      writer.Write<string>(this.UnlockedEquipment_Unlocks, "UnlockedEquipment_Unlocks");
    }
  }
}
