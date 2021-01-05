// Decompiled with JetBrains decompiler
// Type: FistVR.MG_EventAIConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Definition", menuName = "MeatGrinder/EventAIConfig", order = 0)]
  public class MG_EventAIConfig : ScriptableObject
  {
    public float TotalWeight;
    public MeatGrinderMaster.EventAI.EventAIMood Mood;
    public List<MeatGrinderMaster.EventAI.MGEvent> EventList;

    public MeatGrinderMaster.EventAI.MGEvent GetWeightedRandomEntry()
    {
      float num = Random.Range(0.0f, this.TotalWeight);
      int index1 = -1;
      for (int index2 = 0; index2 < this.EventList.Count; ++index2)
      {
        if ((double) num < (double) this.EventList[index2].FinalWeight)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 < 0)
        index1 = 0;
      return this.EventList[index1];
    }

    [ContextMenu("CalcWeights")]
    public void CalcWeights()
    {
      float num = 0.0f;
      this.TotalWeight = 0.0f;
      for (int index = 0; index < this.EventList.Count; ++index)
      {
        this.EventList[index].FinalWeight = this.EventList[index].Incidence + num;
        num += this.EventList[index].Incidence;
        this.TotalWeight += this.EventList[index].Incidence;
      }
    }
  }
}
