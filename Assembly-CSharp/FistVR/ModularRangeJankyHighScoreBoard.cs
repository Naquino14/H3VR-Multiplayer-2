// Decompiled with JetBrains decompiler
// Type: FistVR.ModularRangeJankyHighScoreBoard
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Assets.Rust.Lodestone;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ModularRangeJankyHighScoreBoard : MonoBehaviour, IRequester
  {
    public Text[] NameLists;
    public Text[] ScoreLists;
    private float m_update_tick = 1f;
    private int m_list_updating_index;

    public void Update()
    {
      if ((double) this.m_update_tick > 0.0)
        this.m_update_tick -= Time.deltaTime;
      else
        this.RefreshScoreBoard();
    }

    private void RefreshScoreBoard()
    {
      if ((double) Assets.Rust.Lodestone.Lodestone.GetLogs((IRequester) this, "ModularRange_Sequence" + this.m_list_updating_index.ToString(), "PlayerScore", 10, false) < 0.0)
        return;
      ++this.m_list_updating_index;
      if (this.m_list_updating_index >= this.NameLists.Length)
        this.m_list_updating_index = 0;
      this.m_update_tick = 1f;
    }

    public void HandleResponse(
      string endPointName,
      float startTime,
      List<KeyValuePair<string, string>> fieldTypes,
      List<Dictionary<string, object>> fieldValues)
    {
      string str1 = string.Empty;
      string str2 = string.Empty;
      for (int index = 0; index < fieldValues.Count; ++index)
      {
        str1 = str1 + (index + 1).ToString("##") + ". " + (string) fieldValues[index]["PlayerName"] + "\n";
        str2 = str2 + (object) (int) fieldValues[index]["PlayerScore"] + "\n";
      }
      int index1 = 0;
      switch (endPointName)
      {
        case "ModularRange_Sequence0":
          index1 = 0;
          break;
        case "ModularRange_Sequence1":
          index1 = 1;
          break;
        case "ModularRange_Sequence2":
          index1 = 2;
          break;
        case "ModularRange_Sequence3":
          index1 = 3;
          break;
      }
      this.NameLists[index1].text = str1;
      this.ScoreLists[index1].text = str2;
    }
  }
}
