// Decompiled with JetBrains decompiler
// Type: FistVR.Quest06_BearTrapManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Quest06_BearTrapManager : ZosigQuestManager
  {
    public List<BearTrap> Field1_HBH;
    public List<BearTrap> Field2_Crossroads;
    public List<BearTrap> Field3_Bend;
    public List<BearTrap> Field4_WoodedPass;
    private ZosigGameManager m;
    private bool m_isQuestCompleted;

    public override void Init(ZosigGameManager M)
    {
      this.m = M;
      this.InitializeTrapsFromFlagM();
    }

    private void InitializeTrapsFromFlagM()
    {
      if (this.m.FlagM.GetFlagValue("quest06_state") < 2)
      {
        this.CloseTraps(this.Field1_HBH);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "HBH Trap field not complete");
      }
      else
      {
        this.OpenTraps(this.Field1_HBH);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "HBH Trap field already complete");
      }
      if (this.m.FlagM.GetFlagValue("quest06a_state") < 1)
      {
        this.CloseTraps(this.Field2_Crossroads);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "Crossroads Trap field not complete");
      }
      else
      {
        this.OpenTraps(this.Field2_Crossroads);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "Crossroads Trap field already complete");
      }
      if (this.m.FlagM.GetFlagValue("quest06b_state") < 1)
      {
        this.CloseTraps(this.Field3_Bend);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "Bend field not complete");
      }
      else
      {
        this.OpenTraps(this.Field3_Bend);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "Bend field already complete");
      }
      if (this.m.FlagM.GetFlagValue("quest06c_state") < 1)
      {
        this.CloseTraps(this.Field4_WoodedPass);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "WoodedPass Trap field not complete");
      }
      else
      {
        this.OpenTraps(this.Field4_WoodedPass);
        if (this.m.IsVerboseDebug)
          Debug.Log((object) "WoodedPass Trap field already complete");
      }
      if (this.m.FlagM.GetFlagValue("quest06_state") <= 2)
        return;
      this.m_isQuestCompleted = true;
      if (!this.m.IsVerboseDebug)
        return;
      Debug.Log((object) "Quest Completed Trap field already complete");
    }

    private void OpenTraps(List<BearTrap> t)
    {
      for (int index = 0; index < t.Count; ++index)
        t[index].ForceOpen();
    }

    private void CloseTraps(List<BearTrap> t)
    {
      for (int index = 0; index < t.Count; ++index)
        t[index].ForceClose();
    }

    private void Update()
    {
      if (this.m_isQuestCompleted)
        return;
      if (this.m.FlagM.GetFlagValue("quest06_state") < 2)
        this.SetCompletedFlagIfAllOpen(this.Field1_HBH, "quest06_state", 2);
      if (this.m.FlagM.GetFlagValue("quest06a_state") < 1)
        this.SetCompletedFlagIfAllOpen(this.Field2_Crossroads, "quest06a_state", 1);
      if (this.m.FlagM.GetFlagValue("quest06b_state") < 1)
        this.SetCompletedFlagIfAllOpen(this.Field3_Bend, "quest06b_state", 1);
      if (this.m.FlagM.GetFlagValue("quest06c_state") < 1)
        this.SetCompletedFlagIfAllOpen(this.Field4_WoodedPass, "quest06c_state", 1);
      if (this.m.FlagM.GetFlagValue("quest06_state") != 2 || this.m.FlagM.GetFlagValue("quest06a_state") != 1 || (this.m.FlagM.GetFlagValue("quest06b_state") != 1 || this.m.FlagM.GetFlagValue("quest06c_state") != 1))
        return;
      this.m.FlagM.SetFlagMaxBlend("quest06_state", 3);
      this.m_isQuestCompleted = true;
    }

    private void SetCompletedFlagIfAllOpen(List<BearTrap> t, string flag, int successValue)
    {
      bool flag1 = true;
      for (int index = 0; index < t.Count; ++index)
      {
        if (!t[index].IsOpen())
        {
          flag1 = false;
          break;
        }
      }
      if (!flag1)
        return;
      this.m.FlagM.SetFlagMaxBlend(flag, successValue);
      if (!this.m.IsVerboseDebug)
        return;
      Debug.Log((object) (flag + "Set to 2 because testing list is all open"));
    }
  }
}
