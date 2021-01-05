// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_LootTable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Loot Table", menuName = "TakeAndHold/LootTable", order = 0)]
  public class TAH_LootTable : ScriptableObject
  {
    public float TotalWeight;
    public List<TAH_LootTableEntry> Chart;

    public TAH_LootTableEntry GetWeightedRandomEntry()
    {
      float num = Random.Range(0.0f, this.TotalWeight);
      int index1 = -1;
      for (int index2 = 0; index2 < this.Chart.Count; ++index2)
      {
        if ((double) num < (double) this.Chart[index2].Nums.y)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 < 0)
        index1 = 0;
      return this.Chart[index1];
    }

    [ContextMenu("CalcWeights")]
    public void CalcWeights()
    {
      for (int index = 0; index < this.Chart.Count; ++index)
        this.Chart[index].Nums.y = 0.0f;
      float num = 0.0f;
      for (int index = 0; index < this.Chart.Count; ++index)
      {
        this.Chart[index].Nums.y = this.Chart[index].Nums.x + num;
        num += this.Chart[index].Nums.x;
        this.TotalWeight += this.Chart[index].Nums.x;
      }
    }
  }
}
