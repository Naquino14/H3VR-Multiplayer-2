// Decompiled with JetBrains decompiler
// Type: ScoreHelper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using RUST.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHelper : MonoBehaviour
{
  public HighScoreManager hsm;
  public int score = 500;

  private void Start()
  {
  }

  public void AddScore() => this.hsm.UpdateScore("uniqueHighScoreTableName", this.score, new Action<int, int>(this.HandleAddScore));

  public void HandleAddScore(int previousRank, int newRank) => Debug.Log((object) (previousRank.ToString() + "/" + (object) newRank));

  public void GetScores() => this.hsm.GetLeaderboards("testHighScore", 1, 10, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, new Action<List<HighScoreManager.HighScore>>(this.HandleGetScores));

  public void HandleGetScores(List<HighScoreManager.HighScore> scores)
  {
    foreach (HighScoreManager.HighScore score in scores)
      Debug.Log((object) (score.rank.ToString() + "/" + score.name + "/" + (object) score.score));
  }
}
