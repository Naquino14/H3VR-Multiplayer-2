using System.Collections.Generic;
using RUST.Steamworks;
using Steamworks;
using UnityEngine;

public class ScoreHelper : MonoBehaviour
{
	public HighScoreManager hsm;

	public int score = 500;

	private void Start()
	{
	}

	public void AddScore()
	{
		hsm.UpdateScore("uniqueHighScoreTableName", score, HandleAddScore);
	}

	public void HandleAddScore(int previousRank, int newRank)
	{
		Debug.Log(previousRank + "/" + newRank);
	}

	public void GetScores()
	{
		hsm.GetLeaderboards("testHighScore", 1, 10, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, HandleGetScores);
	}

	public void HandleGetScores(List<HighScoreManager.HighScore> scores)
	{
		foreach (HighScoreManager.HighScore score2 in scores)
		{
			Debug.Log(score2.rank + "/" + score2.name + "/" + score2.score);
		}
	}
}
