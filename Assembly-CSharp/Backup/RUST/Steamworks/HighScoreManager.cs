// Decompiled with JetBrains decompiler
// Type: RUST.Steamworks.HighScoreManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RUST.Steamworks
{
  public class HighScoreManager : MonoBehaviour
  {
    private const ELeaderboardUploadScoreMethod LeaderboardMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;
    private int _addScoreAmount;
    private string _addScoreLeaderboard;
    private Action<int, int> _addScoreCallback;
    private string _getScoresLeaderboard;
    private Action<List<HighScoreManager.HighScore>> _getScoresCallback;
    private ELeaderboardDataRequest _getScoresRequestType;
    private int[] _getScoresRange = new int[2];
    private static CallResult<LeaderboardFindResult_t> _findResult = new CallResult<LeaderboardFindResult_t>();
    private static CallResult<LeaderboardScoreUploaded_t> _uploadResult = new CallResult<LeaderboardScoreUploaded_t>();
    private static CallResult<LeaderboardScoresDownloaded_t> _downloadResult = new CallResult<LeaderboardScoresDownloaded_t>();
    private Dictionary<string, SteamLeaderboard_t> _leaderboards = new Dictionary<string, SteamLeaderboard_t>();

    private void Start()
    {
      if (SteamManager.Initialized)
        ;
    }

    public void GetRank(string boardName, Action<List<HighScoreManager.HighScore>> callback) => this.GetLeaderboards(boardName, 0, 0, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, callback);

    public void UpdateScore(string boardName, int score, Action<int, int> callback)
    {
      Debug.Log((object) ("Updating score on " + boardName + " with score of:" + (object) score));
      this._addScoreAmount = score;
      this._addScoreLeaderboard = boardName;
      this._addScoreCallback = callback;
      if (this._leaderboards.ContainsKey(this._addScoreLeaderboard))
      {
        this.UpdateScore(this._leaderboards[this._addScoreLeaderboard], this._addScoreAmount);
      }
      else
      {
        SteamAPICall_t createLeaderboard = SteamUserStats.FindOrCreateLeaderboard(boardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
        HighScoreManager._findResult.Set(createLeaderboard, new CallResult<LeaderboardFindResult_t>.APIDispatchDelegate(this.OnAddScoreLeaderboardFindResult));
      }
    }

    private void UpdateScore(SteamLeaderboard_t board, int score)
    {
      SteamAPICall_t hAPICall = SteamUserStats.UploadLeaderboardScore(board, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, (int[]) null, 0);
      HighScoreManager._uploadResult.Set(hAPICall, new CallResult<LeaderboardScoreUploaded_t>.APIDispatchDelegate(this.OnLeaderboardUploadResult));
    }

    public void GetLeaderboards(
      string boardName,
      int from,
      int to,
      ELeaderboardDataRequest type,
      Action<List<HighScoreManager.HighScore>> callback)
    {
      this._getScoresLeaderboard = boardName;
      this._getScoresCallback = callback;
      this._getScoresRequestType = type;
      this._getScoresRange[0] = from;
      this._getScoresRange[1] = to;
      if (this._leaderboards.ContainsKey(this._getScoresLeaderboard))
      {
        this.GetLeaderboards(this._leaderboards[this._getScoresLeaderboard]);
      }
      else
      {
        SteamAPICall_t createLeaderboard = SteamUserStats.FindOrCreateLeaderboard(boardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
        HighScoreManager._findResult.Set(createLeaderboard, new CallResult<LeaderboardFindResult_t>.APIDispatchDelegate(this.OnGetScoresLeaderboardFindResult));
      }
    }

    private void GetLeaderboards(SteamLeaderboard_t board)
    {
      SteamAPICall_t hAPICall = SteamUserStats.DownloadLeaderboardEntries(this._leaderboards[this._getScoresLeaderboard], this._getScoresRequestType, this._getScoresRange[0], this._getScoresRange[1]);
      HighScoreManager._downloadResult.Set(hAPICall, new CallResult<LeaderboardScoresDownloaded_t>.APIDispatchDelegate(this.OnLeaderboardDownloadResult));
    }

    private void OnAddScoreLeaderboardFindResult(LeaderboardFindResult_t callback, bool failure)
    {
      if (failure || this._addScoreLeaderboard == null)
        return;
      this._leaderboards[this._addScoreLeaderboard] = callback.m_hSteamLeaderboard;
      this.UpdateScore(callback.m_hSteamLeaderboard, this._addScoreAmount);
    }

    private void OnGetScoresLeaderboardFindResult(LeaderboardFindResult_t callback, bool failure)
    {
      if (failure || this._getScoresLeaderboard == null)
        return;
      this._leaderboards[this._getScoresLeaderboard] = callback.m_hSteamLeaderboard;
      this.GetLeaderboards(callback.m_hSteamLeaderboard);
    }

    private void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t callback, bool failure)
    {
      if (failure)
        Debug.Log((object) "Score failed to post due to lost connection to Steamworks?");
      if (failure || this._addScoreCallback == null)
        return;
      this._addScoreCallback(callback.m_nGlobalRankPrevious, callback.m_nGlobalRankNew);
    }

    private void OnLeaderboardDownloadResult(LeaderboardScoresDownloaded_t callback, bool failure)
    {
      if (failure)
        return;
      SteamLeaderboardEntries_t leaderboardEntries = callback.m_hSteamLeaderboardEntries;
      List<HighScoreManager.HighScore> highScoreList = new List<HighScoreManager.HighScore>();
      for (int index = 0; index <= (int) leaderboardEntries.m_SteamLeaderboardEntries; ++index)
      {
        int cDetailsMax = 0;
        int[] pDetails = new int[cDetailsMax];
        LeaderboardEntry_t pLeaderboardEntry;
        SteamUserStats.GetDownloadedLeaderboardEntry(leaderboardEntries, index, out pLeaderboardEntry, pDetails, cDetailsMax);
        if (pLeaderboardEntry.m_steamIDUser.IsValid())
          highScoreList.Add(new HighScoreManager.HighScore()
          {
            name = SteamFriends.GetFriendPersonaName(pLeaderboardEntry.m_steamIDUser),
            rank = pLeaderboardEntry.m_nGlobalRank,
            score = pLeaderboardEntry.m_nScore,
            steamID = pLeaderboardEntry.m_steamIDUser
          });
      }
      this._getScoresCallback(highScoreList);
    }

    public class HighScore
    {
      public string name;
      public CSteamID steamID;
      public int rank;
      public int score;
    }
  }
}
