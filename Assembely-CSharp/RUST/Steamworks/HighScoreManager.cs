using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace RUST.Steamworks
{
	public class HighScoreManager : MonoBehaviour
	{
		public class HighScore
		{
			public string name;

			public CSteamID steamID;

			public int rank;

			public int score;
		}

		private const ELeaderboardUploadScoreMethod LeaderboardMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;

		private int _addScoreAmount;

		private string _addScoreLeaderboard;

		private Action<int, int> _addScoreCallback;

		private string _getScoresLeaderboard;

		private Action<List<HighScore>> _getScoresCallback;

		private ELeaderboardDataRequest _getScoresRequestType;

		private int[] _getScoresRange = new int[2];

		private static CallResult<LeaderboardFindResult_t> _findResult = new CallResult<LeaderboardFindResult_t>();

		private static CallResult<LeaderboardScoreUploaded_t> _uploadResult = new CallResult<LeaderboardScoreUploaded_t>();

		private static CallResult<LeaderboardScoresDownloaded_t> _downloadResult = new CallResult<LeaderboardScoresDownloaded_t>();

		private Dictionary<string, SteamLeaderboard_t> _leaderboards = new Dictionary<string, SteamLeaderboard_t>();

		private void Start()
		{
			if (SteamManager.Initialized)
			{
			}
		}

		public void GetRank(string boardName, Action<List<HighScore>> callback)
		{
			GetLeaderboards(boardName, 0, 0, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, callback);
		}

		public void UpdateScore(string boardName, int score, Action<int, int> callback)
		{
			Debug.Log("Updating score on " + boardName + " with score of:" + score);
			_addScoreAmount = score;
			_addScoreLeaderboard = boardName;
			_addScoreCallback = callback;
			if (_leaderboards.ContainsKey(_addScoreLeaderboard))
			{
				UpdateScore(_leaderboards[_addScoreLeaderboard], _addScoreAmount);
				return;
			}
			SteamAPICall_t hAPICall = SteamUserStats.FindOrCreateLeaderboard(boardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
			_findResult.Set(hAPICall, OnAddScoreLeaderboardFindResult);
		}

		private void UpdateScore(SteamLeaderboard_t board, int score)
		{
			SteamAPICall_t hAPICall = SteamUserStats.UploadLeaderboardScore(board, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);
			_uploadResult.Set(hAPICall, OnLeaderboardUploadResult);
		}

		public void GetLeaderboards(string boardName, int from, int to, ELeaderboardDataRequest type, Action<List<HighScore>> callback)
		{
			_getScoresLeaderboard = boardName;
			_getScoresCallback = callback;
			_getScoresRequestType = type;
			_getScoresRange[0] = from;
			_getScoresRange[1] = to;
			if (_leaderboards.ContainsKey(_getScoresLeaderboard))
			{
				GetLeaderboards(_leaderboards[_getScoresLeaderboard]);
				return;
			}
			SteamAPICall_t hAPICall = SteamUserStats.FindOrCreateLeaderboard(boardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
			_findResult.Set(hAPICall, OnGetScoresLeaderboardFindResult);
		}

		private void GetLeaderboards(SteamLeaderboard_t board)
		{
			SteamAPICall_t hAPICall = SteamUserStats.DownloadLeaderboardEntries(_leaderboards[_getScoresLeaderboard], _getScoresRequestType, _getScoresRange[0], _getScoresRange[1]);
			_downloadResult.Set(hAPICall, OnLeaderboardDownloadResult);
		}

		private void OnAddScoreLeaderboardFindResult(LeaderboardFindResult_t callback, bool failure)
		{
			if (!failure && _addScoreLeaderboard != null)
			{
				_leaderboards[_addScoreLeaderboard] = callback.m_hSteamLeaderboard;
				UpdateScore(callback.m_hSteamLeaderboard, _addScoreAmount);
			}
		}

		private void OnGetScoresLeaderboardFindResult(LeaderboardFindResult_t callback, bool failure)
		{
			if (!failure && _getScoresLeaderboard != null)
			{
				_leaderboards[_getScoresLeaderboard] = callback.m_hSteamLeaderboard;
				GetLeaderboards(callback.m_hSteamLeaderboard);
			}
		}

		private void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t callback, bool failure)
		{
			if (failure)
			{
				Debug.Log("Score failed to post due to lost connection to Steamworks?");
			}
			if (!failure && _addScoreCallback != null)
			{
				_addScoreCallback(callback.m_nGlobalRankPrevious, callback.m_nGlobalRankNew);
			}
		}

		private void OnLeaderboardDownloadResult(LeaderboardScoresDownloaded_t callback, bool failure)
		{
			if (failure)
			{
				return;
			}
			SteamLeaderboardEntries_t hSteamLeaderboardEntries = callback.m_hSteamLeaderboardEntries;
			List<HighScore> list = new List<HighScore>();
			for (int i = 0; i <= (int)hSteamLeaderboardEntries.m_SteamLeaderboardEntries; i++)
			{
				int num = 0;
				int[] pDetails = new int[num];
				SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, i, out var pLeaderboardEntry, pDetails, num);
				if (pLeaderboardEntry.m_steamIDUser.IsValid())
				{
					list.Add(new HighScore
					{
						name = SteamFriends.GetFriendPersonaName(pLeaderboardEntry.m_steamIDUser),
						rank = pLeaderboardEntry.m_nGlobalRank,
						score = pLeaderboardEntry.m_nScore,
						steamID = pLeaderboardEntry.m_steamIDUser
					});
				}
			}
			_getScoresCallback(list);
		}
	}
}
