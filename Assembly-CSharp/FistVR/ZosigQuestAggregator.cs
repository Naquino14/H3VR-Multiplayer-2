// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigQuestAggregator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigQuestAggregator : ZosigQuestManager
  {
    public string QuestFlagToSet;
    public int QuestFlagValueBeneath = 2;
    public int QuestFlagToSetToWhenDone = 2;
    public List<ZosigQuestAggregator.ZQuestRequirement> RequiredQuests;
    private bool m_isQuestCompleted;
    private ZosigGameManager m;
    private float checkTick = 1f;

    public override void Init(ZosigGameManager M)
    {
      this.m = M;
      this.InitializeTrapsFromFlagM();
    }

    private void InitializeTrapsFromFlagM()
    {
      if (this.m.FlagM.GetFlagValue(this.QuestFlagToSet) >= this.QuestFlagToSetToWhenDone)
      {
        this.m_isQuestCompleted = true;
        if (!this.m.IsVerboseDebug)
          return;
        Debug.Log((object) (this.QuestFlagToSet + " already detected as being done, setting aggregator to finished"));
      }
      else
      {
        if (!this.m.IsVerboseDebug)
          return;
        Debug.Log((object) (this.QuestFlagToSet + " is not done yet, setting aggregator to not finished"));
      }
    }

    private void Update()
    {
      if (this.m_isQuestCompleted)
        return;
      this.checkTick -= Time.deltaTime;
      if ((double) this.checkTick > 0.0)
        return;
      this.checkTick = 1f;
      this.CheckQuestState();
    }

    private void CheckQuestState()
    {
      bool flag = true;
      for (int index = 0; index < this.RequiredQuests.Count; ++index)
      {
        if (this.m.FlagM.GetFlagValue(this.RequiredQuests[index].QuestFlag) < this.RequiredQuests[index].ValueNeeded)
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        return;
      this.m_isQuestCompleted = true;
      if (this.m.IsVerboseDebug)
        Debug.Log((object) (this.QuestFlagToSet + " has been completed! setting aggregator to finished"));
      this.m.FlagM.SetFlag(this.QuestFlagToSet, this.QuestFlagToSetToWhenDone);
      GM.ZMaster.FlagM.Save();
    }

    [Serializable]
    public class ZQuestRequirement
    {
      public string QuestFlag;
      public int ValueNeeded = 2;
    }
  }
}
