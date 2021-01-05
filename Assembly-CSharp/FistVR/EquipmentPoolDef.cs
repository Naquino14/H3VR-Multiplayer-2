// Decompiled with JetBrains decompiler
// Type: FistVR.EquipmentPoolDef
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Equipment Pool Def", menuName = "EquipmentPoolDef", order = 0)]
  public class EquipmentPoolDef : ScriptableObject
  {
    public List<EquipmentPoolDef.PoolEntry> Entries = new List<EquipmentPoolDef.PoolEntry>();

    public int GetNumEntries(EquipmentPoolDef.PoolEntry.PoolEntryType pType, int atLevel)
    {
      int num = 0;
      for (int index = 0; index < this.Entries.Count; ++index)
      {
        if (this.Entries[index].Type == pType && this.Entries[index].MinLevelAppears <= atLevel && this.Entries[index].MaxLevelAppears >= atLevel)
          ++num;
      }
      return num;
    }

    [Serializable]
    public class PoolEntry
    {
      public ObjectTableDef TableDef;
      public EquipmentPoolDef.PoolEntry.PoolEntryType Type;
      public int TokenCost = 1;
      public int TokenCost_Limited = 1;
      public int MinLevelAppears = 1;
      public int MaxLevelAppears = 2;
      public float Rarity = 1f;

      public int GetCost(bool limited) => limited ? this.TokenCost_Limited : this.TokenCost;

      public int GetCost(TNHSetting_EquipmentMode m) => m == TNHSetting_EquipmentMode.LimitedAmmo ? this.TokenCost_Limited : this.TokenCost;

      public enum PoolEntryType
      {
        Firearm,
        Equipment,
        Consumable,
      }
    }
  }
}
