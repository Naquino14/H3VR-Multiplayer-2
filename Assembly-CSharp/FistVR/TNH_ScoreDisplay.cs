// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_ScoreDisplay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using RUST.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FistVR
{
  public class TNH_ScoreDisplay : MonoBehaviour
  {
    public Text TableNameLabel;
    public Text[] UIH_Scores_Global;
    public Text[] UIH_Scores_Local;
    private bool m_usingSteamworks;
    [Header("Scoring System")]
    public HighScoreManager HSM;
    private string m_curSequenceID = string.Empty;
    private string m_name = string.Empty;
    private CSteamID m_ID;
    private bool m_hasCurrentScore;
    private int m_currentScore;
    private int m_currentRank;
    private bool m_hasPreviousScore;
    private int m_previousScore;
    private bool m_hasRankInCurrentSequence;
    private int m_rankInCurrentSequence;
    private bool m_hasScoresTop;
    private bool m_hasScoresPlayer;
    private bool m_doRequestScoresTop;
    private bool m_doRequestScoresPlayer;
    private List<HighScoreManager.HighScore> m_scoresTop;
    private List<HighScoreManager.HighScore> m_scoresPlayer;
    public bool UsesRunDisplay;

    private void Start()
    {
      this.InitScoring();
      if (!SteamManager.Initialized)
        return;
      this.m_usingSteamworks = true;
    }

    private void InitScoring()
    {
      if (!SteamManager.Initialized)
        return;
      this.m_name = SteamFriends.GetPersonaName();
      this.m_ID = SteamUser.GetSteamID();
    }

    public void ReloadLevel() => SteamVR_LoadLevel.Begin(SceneManager.GetActiveScene().name);

    public void ReturnToLobby() => SteamVR_LoadLevel.Begin("TakeAndHold_Lobby_2");

    public string GetTableID(
      string levelName,
      string charTableID,
      TNHSetting_ProgressionType p,
      TNHSetting_EquipmentMode e,
      TNHSetting_HealthMode h)
    {
      return string.Empty + levelName + charTableID + p.ToString() + e.ToString() + h.ToString();
    }

    private void Update() => this.UpdateHighScoreCallbacks();

    public void ForceSetSequenceID(string s) => this.m_curSequenceID = s;

    public void SwitchToModeID(string id)
    {
      this.TableNameLabel.text = id;
      this.m_curSequenceID = id;
      this.m_hasPreviousScore = false;
      this.m_previousScore = 0;
      this.m_hasRankInCurrentSequence = false;
      this.m_rankInCurrentSequence = 0;
      this.RedrawHighScoreDisplay(this.m_curSequenceID);
      this.RequestHighScoreChart();
    }

    public void SubmitScoreAndGoToBoard(int score)
    {
      GM.Omni.OmniFlags.AddScore(this.m_curSequenceID, score);
      Debug.Log((object) ("Preparing to Submit score of:" + (object) score + " to board:" + this.m_curSequenceID));
      if (this.m_usingSteamworks)
        this.ProcessHighScore(score);
      this.RedrawHighScoreDisplay(this.m_curSequenceID);
      GM.Omni.SaveToFile();
    }

    public void ProcessHighScore(int score)
    {
      this.m_hasCurrentScore = true;
      this.m_currentScore = score;
      this.HSM.UpdateScore(this.m_curSequenceID, score, new Action<int, int>(this.HandleProcessHighScore));
    }

    public void HandleProcessHighScore(int prevRank, int newRank)
    {
      this.m_currentRank = newRank;
      this.m_doRequestScoresTop = true;
    }

    private void RequestHighScoreChart()
    {
      this.ClearGlobalHighScoreDisplay();
      Debug.Log((object) "Requesting High Score Chart");
      this.m_hasScoresTop = false;
      this.m_hasScoresPlayer = false;
      this.m_scoresTop = (List<HighScoreManager.HighScore>) null;
      this.m_scoresPlayer = (List<HighScoreManager.HighScore>) null;
      this.m_doRequestScoresTop = true;
    }

    public void HandleHighScoreChartTop(List<HighScoreManager.HighScore> scores)
    {
      Debug.Log((object) "HighScore Top Chart Received");
      this.m_hasScoresTop = true;
      this.m_scoresTop = scores;
      this.m_doRequestScoresPlayer = true;
    }

    public void HandleHighScoreChartPlayer(List<HighScoreManager.HighScore> scores)
    {
      Debug.Log((object) "HighScore Player Chart Received");
      this.m_hasScoresPlayer = true;
      this.m_scoresPlayer = scores;
      this.RedrawHighScoreChart();
    }

    private void UpdateHighScoreCallbacks()
    {
      if (this.m_doRequestScoresTop)
      {
        this.m_doRequestScoresTop = false;
        this.HSM.GetLeaderboards(this.m_curSequenceID, 1, 6, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, new Action<List<HighScoreManager.HighScore>>(this.HandleHighScoreChartTop));
      }
      if (!this.m_doRequestScoresPlayer)
        return;
      this.m_doRequestScoresPlayer = false;
      this.HSM.GetLeaderboards(this.m_curSequenceID, -1, 1, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, new Action<List<HighScoreManager.HighScore>>(this.HandleHighScoreChartPlayer));
    }

    private void RedrawHighScoreChart()
    {
      Debug.Log((object) ("m_hasScoresTop:" + (object) this.m_hasScoresTop + " m_hasScoresPlayer:" + (object) this.m_hasScoresPlayer));
      if (!this.m_hasScoresTop || !this.m_hasScoresPlayer)
        return;
      List<HighScoreManager.HighScore> scores = new List<HighScoreManager.HighScore>();
      for (int index = 0; index < this.m_scoresTop.Count; ++index)
        scores.Add(this.m_scoresTop[index]);
      if (this.m_scoresPlayer.Count > 0)
      {
        int rank = this.m_scoresPlayer[0].rank;
        int num1 = 0;
        int num2 = 0;
        for (int index = 0; index < this.m_scoresPlayer.Count; ++index)
        {
          if (this.m_scoresPlayer[index].steamID == this.m_ID)
          {
            num1 = index;
            num2 = this.m_scoresPlayer[index].rank;
            if (!this.m_hasCurrentScore)
            {
              this.m_currentScore = this.m_scoresPlayer[index].score;
              this.m_currentRank = this.m_scoresPlayer[index].rank;
              this.m_hasCurrentScore = true;
            }
            this.m_hasPreviousScore = true;
            this.m_hasRankInCurrentSequence = true;
            this.m_previousScore = this.m_scoresPlayer[index].score;
            this.m_rankInCurrentSequence = this.m_scoresPlayer[index].rank;
          }
        }
        if (num2 > 6)
        {
          int index1 = 0;
          for (int index2 = 6 - this.m_scoresPlayer.Count; index2 < 6; ++index2)
          {
            scores[index2] = this.m_scoresPlayer[index1];
            ++index1;
          }
        }
      }
      this.SetGlobalHighScoreDisplay(scores);
    }

    public void ClearGlobalHighScoreDisplay()
    {
      for (int index = 0; index < 6; ++index)
        this.UIH_Scores_Global[index].gameObject.SetActive(false);
    }

    public void ClearLocalHighScoreDisplay()
    {
      for (int index = 0; index < 6; ++index)
        this.UIH_Scores_Local[index].gameObject.SetActive(false);
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
