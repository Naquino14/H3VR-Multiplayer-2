// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigQuestDisplay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigQuestDisplay : ZosigQuestManager
  {
    private ZosigGameManager m;
    public List<ZosigQuestDisplay.DisplayItem> Display;
    private float checkTick = 1f;

    public override void Init(ZosigGameManager M) => this.m = M;

    private void Update()
    {
      this.checkTick -= Time.deltaTime;
      if ((double) this.checkTick > 0.0)
        return;
      this.checkTick = 1f;
      this.UpdateList();
    }

    private void UpdateList()
    {
      for (int index = 0; index < this.Display.Count; ++index)
      {
        if (this.m.FlagM.GetFlagValue(this.Display[index].flagNeeded) >= this.Display[index].valueNeeded)
          this.Display[index].ObjectToShow.SetActive(true);
        else
          this.Display[index].ObjectToShow.SetActive(false);
      }
    }

    [Serializable]
    public class DisplayItem
    {
      public GameObject ObjectToShow;
      public string flagNeeded;
      public int valueNeeded;
    }
  }
}
