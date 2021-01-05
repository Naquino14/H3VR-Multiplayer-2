// Decompiled with JetBrains decompiler
// Type: FistVR.HG_UIManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using RUST.Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class HG_UIManager : MonoBehaviour
  {
    [Header("Main Screen")]
    public Text LBL_SelectedMode;
    public Text LBL_ModeRules;
    public Text[] UIH_Scores_Global;
    public Text[] UIH_Scores_Local;
    public Text[] ScoringReadOuts;

    public void SetSelectedModeText(HG_ModeProfile p)
    {
      this.LBL_SelectedMode.text = "Selected Game: " + p.Title;
      this.LBL_ModeRules.text = p.DescriptionText;
    }

    public void SetScoringReadouts(List<string> readout)
    {
      if (readout == null)
        return;
      for (int index = 0; index < this.ScoringReadOuts.Length; ++index)
      {
        if (index < readout.Count)
        {
          this.ScoringReadOuts[index].gameObject.SetActive(true);
          this.ScoringReadOuts[index].text = readout[index];
        }
        else
          this.ScoringReadOuts[index].gameObject.SetActive(false);
      }
    }

    public void ClearGlobalHighScoreDisplay()
    {
      for (int index = 0; index < 6; ++index)
        this.UIH_Scores_Global[index].gameObject.SetActive(false);
    }

    public void SetGlobalHighScoreDisplay(List<HighScoreManager.HighScore> scores)
    {
      for (int index = 0; index < 6; ++index)
      {
        if (scores.Count > index)
        {
          this.UIH_Scores_Global[index].gameObject.SetActive(true);
          this.UIH_Scores_Global[index].text = scores[index].rank.ToString() + ". " + (object) scores[index].score + " - " + scores[index].name;
        }
        else
          this.UIH_Scores_Global[index].gameObject.SetActive(false);
      }
    }

    public void RedrawHighScoreDisplay(string SequenceID)
    {
      List<OmniScore> scoreList = GM.Omni.OmniFlags.GetScoreList(SequenceID);
      for (int index = 0; index < scoreList.Count; ++index)
      {
        this.UIH_Scores_Local[index].gameObject.SetActive(true);
        this.UIH_Scores_Local[index].text = (index + 1).ToString() + ": " + (object) scoreList[index].Score;
      }
      for (int count = scoreList.Count; count < this.UIH_Scores_Local.Length; ++count)
        this.UIH_Scores_Local[count].gameObject.SetActive(false);
    }
  }
}
