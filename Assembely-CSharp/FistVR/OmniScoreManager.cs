using System.Collections.Generic;
using RUST.Steamworks;
using Steamworks;
using UnityEngine;

namespace FistVR
{
	public class OmniScoreManager : MonoBehaviour
	{
		public HighScoreManager HSM;

		public OmniSequencer Seq;

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

		private void Start()
		{
			if (SteamManager.Initialized)
			{
				m_name = SteamFriends.GetPersonaName();
				m_ID = SteamUser.GetSteamID();
			}
		}

		public void SwitchToSequenceID(string id)
		{
			m_curSequenceID = id;
			m_hasPreviousScore = false;
			m_previousScore = 0;
			m_hasRankInCurrentSequence = false;
			m_rankInCurrentSequence = 0;
			RequestHighScoreChart();
		}

		public void ProcessHighScore(int score)
		{
			m_hasCurrentScore = true;
			m_currentScore = score;
			HSM.UpdateScore(m_curSequenceID, score, HandleProcessHighScore);
		}

		public void HandleProcessHighScore(int prevRank, int newRank)
		{
			m_currentRank = newRank;
			m_doRequestScoresTop = true;
		}

		private void RequestHighScoreChart()
		{
			Seq.ClearGlobalHighScoreDisplay();
			Debug.Log("Requesting High Score Chart");
			m_hasScoresTop = false;
			m_hasScoresPlayer = false;
			m_scoresTop = null;
			m_scoresPlayer = null;
			m_doRequestScoresTop = true;
		}

		public void HandleHighScoreChartTop(List<HighScoreManager.HighScore> scores)
		{
			Debug.Log("HighScore Top Chart Received");
			m_hasScoresTop = true;
			m_scoresTop = scores;
			m_doRequestScoresPlayer = true;
		}

		public void HandleHighScoreChartPlayer(List<HighScoreManager.HighScore> scores)
		{
			Debug.Log("HighScore Player Chart Received");
			m_hasScoresPlayer = true;
			m_scoresPlayer = scores;
			RedrawHighScoreChart();
		}

		private void Update()
		{
			if (m_doRequestScoresTop)
			{
				m_doRequestScoresTop = false;
				HSM.GetLeaderboards(m_curSequenceID, 1, 6, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, HandleHighScoreChartTop);
			}
			if (m_doRequestScoresPlayer)
			{
				m_doRequestScoresPlayer = false;
				HSM.GetLeaderboards(m_curSequenceID, -1, 1, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, HandleHighScoreChartPlayer);
			}
		}

		private void RedrawHighScoreChart()
		{
			Debug.Log("m_hasScoresTop:" + m_hasScoresTop + " m_hasScoresPlayer:" + m_hasScoresPlayer);
			if (!m_hasScoresTop || !m_hasScoresPlayer)
			{
				return;
			}
			List<HighScoreManager.HighScore> list = new List<HighScoreManager.HighScore>();
			for (int i = 0; i < m_scoresTop.Count; i++)
			{
				list.Add(m_scoresTop[i]);
			}
			if (m_scoresPlayer.Count > 0)
			{
				int rank = m_scoresPlayer[0].rank;
				int num = 0;
				int num2 = 0;
				for (int j = 0; j < m_scoresPlayer.Count; j++)
				{
					if (m_scoresPlayer[j].steamID == m_ID)
					{
						num = j;
						num2 = m_scoresPlayer[j].rank;
						if (!m_hasCurrentScore)
						{
							m_currentScore = m_scoresPlayer[j].score;
							m_currentRank = m_scoresPlayer[j].rank;
							m_hasCurrentScore = true;
						}
						m_hasPreviousScore = true;
						m_hasRankInCurrentSequence = true;
						m_previousScore = m_scoresPlayer[j].score;
						m_rankInCurrentSequence = m_scoresPlayer[j].rank;
					}
				}
				if (num2 > 6)
				{
					int num3 = 0;
					for (int k = 6 - m_scoresPlayer.Count; k < 6; k++)
					{
						list[k] = m_scoresPlayer[num3];
						num3++;
					}
				}
			}
			Seq.SetGlobalHighScoreDisplay(list);
		}
	}
}
