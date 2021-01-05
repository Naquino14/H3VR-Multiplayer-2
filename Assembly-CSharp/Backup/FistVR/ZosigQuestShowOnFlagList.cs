// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigQuestShowOnFlagList
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigQuestShowOnFlagList : MonoBehaviour
  {
    public List<ZosigQuestShowOnFlagList.ShowByFlag> ShowByFlags;
    private float checkTick = 1f;

    private void Start()
    {
      for (int index = 0; index < this.ShowByFlags.Count; ++index)
        this.ShowByFlags[index].Show.SetActive(false);
    }

    private void Update()
    {
      if ((double) this.checkTick > 0.0)
      {
        this.checkTick -= Time.deltaTime;
      }
      else
      {
        this.checkTick = UnityEngine.Random.Range(1f, 1.4f);
        for (int index = 0; index < this.ShowByFlags.Count; ++index)
        {
          if (!this.ShowByFlags[index].IsRevealed && GM.ZMaster.FlagM.GetFlagValue(this.ShowByFlags[index].Flag) >= this.ShowByFlags[index].ValueEqualOrAbove)
          {
            this.ShowByFlags[index].IsRevealed = true;
            if (!this.ShowByFlags[index].Show.activeSelf)
              this.ShowByFlags[index].Show.SetActive(true);
          }
        }
      }
    }

    [Serializable]
    public class ShowByFlag
    {
      public GameObject Show;
      public string Flag;
      public int ValueEqualOrAbove;
      public bool IsRevealed;
    }
  }
}
