// Decompiled with JetBrains decompiler
// Type: FistVR.ModularRangeHighScoreBoard
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Assets.Rust.Lodestone;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ModularRangeHighScoreBoard : MonoBehaviour, IRequester
  {
    public Text HighScoreBoardTitle;
    public Text[] NameLists;
    public Text[] ScoreLists;
    private float m_update_tick = 0.1f;
    private int m_list_updating_index;
    public ModularRangeMaster Master;
    private bool IsUpdatingScore;
    private ModularRangeSequenceDefinition.SequenceCategory Category;
    private int SequenceIndex;

    public void Update()
    {
      if ((double) this.m_update_tick > 0.0)
      {
        this.m_update_tick -= Time.deltaTime;
      }
      else
      {
        if (this.IsUpdatingScore)
          return;
        this.RefreshScoreBoard();
      }
    }

    private void ClearScoreBoard()
    {
      string str = "Loading...";
      string empty = string.Empty;
      for (int index = 0; index < this.NameLists.Length; ++index)
      {
        this.NameLists[index].text = str;
        this.ScoreLists[index].text = empty;
      }
    }

    public void SetScoreBoardSequence(
      ModularRangeSequenceDefinition.SequenceCategory cat,
      int index)
    {
      this.Category = cat;
      this.SequenceIndex = index;
      this.ClearScoreBoard();
    }

    private void RefreshScoreBoard()
    {
      this.HighScoreBoardTitle.text = this.Master.SequenceDic[this.Category].Sequences[this.SequenceIndex].MetaData.Category.ToString() + " - " + this.Master.SequenceDic[this.Category].Sequences[this.SequenceIndex].MetaData.DisplayName;
      string endPointName = this.Master.SequenceDic[this.Category].Sequences[this.SequenceIndex].MetaData.EndPointName;
      float num = -1f;
      switch (this.m_list_updating_index)
      {
        case 0:
          num = Assets.Rust.Lodestone.Lodestone.GetLogs((IRequester) this, endPointName, "PlayerScore", 10, false);
          break;
        case 1:
          num = Assets.Rust.Lodestone.Lodestone.GetLogs((IRequester) this, endPointName, "PlayerScore", 10, false, filterFieldValues: new SortedList<string, string>()
          {
            {
              "PlayerName",
              "unique"
            }
          });
          break;
        case 2:
          num = Assets.Rust.Lodestone.Lodestone.GetLogs((IRequester) this, endPointName, "PlayerScore", 10, false, currentPlayerOnly: true);
          break;
      }
      if ((double) num < 0.0)
        return;
      this.IsUpdatingScore = true;
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
      this.NameLists[this.m_list_updating_index].text = str1;
      this.ScoreLists[this.m_list_updating_index].text = str2;
      this.IsUpdatingScore = false;
      this.m_update_tick = 0.1f;
      ++this.m_list_updating_index;
      if (this.m_list_updating_index < this.NameLists.Length)
        return;
      this.m_list_updating_index = 0;
    }
  }
}
