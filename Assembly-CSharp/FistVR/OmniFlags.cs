// Decompiled with JetBrains decompiler
// Type: FistVR.OmniFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class OmniFlags
  {
    public Dictionary<string, OmniScoreList> ScoreDic = new Dictionary<string, OmniScoreList>();
    public OmniScoreList testlist = new OmniScoreList();
    public string StoredPlayerName = "Meat";

    public void AddScore(OmniSequencerSequenceDefinition def, int score)
    {
      if (!this.ScoreDic.ContainsKey(def.SequenceID))
      {
        List<OmniScore> omniScoreList = new List<OmniScore>();
        this.ScoreDic.Add(def.SequenceID, new OmniScoreList()
        {
          SequenceID = def.SequenceID,
          Scores = omniScoreList
        });
      }
      List<OmniScore> scores = this.ScoreDic[def.SequenceID].Scores;
      scores.Add(new OmniScore()
      {
        Score = score,
        Name = this.StoredPlayerName
      });
      scores.Sort();
      if (scores.Count > 6)
        scores.RemoveRange(6, scores.Count - 6);
      for (int index = 0; index < def.ScoreThresholds.Count; ++index)
      {
        if (score >= def.ScoreThresholds[index] && index < this.ScoreDic[def.SequenceID].Trophy)
        {
          this.ScoreDic[def.SequenceID].Trophy = index;
          break;
        }
      }
      this.ScoreDic[def.SequenceID].Scores = scores;
    }

    public void AddScore(string defID, int score)
    {
      if (!this.ScoreDic.ContainsKey(defID))
      {
        List<OmniScore> omniScoreList = new List<OmniScore>();
        this.ScoreDic.Add(defID, new OmniScoreList()
        {
          SequenceID = defID,
          Scores = omniScoreList
        });
      }
      List<OmniScore> scores = this.ScoreDic[defID].Scores;
      scores.Add(new OmniScore()
      {
        Score = score,
        Name = this.StoredPlayerName
      });
      scores.Sort();
      if (scores.Count > 6)
        scores.RemoveRange(6, scores.Count - 6);
      this.ScoreDic[defID].Scores = scores;
    }

    public List<OmniScore> GetScoreList(string ID) => !this.ScoreDic.ContainsKey(ID) ? new List<OmniScore>() : this.ScoreDic[ID].Scores;

    public int GetRank(string ID) => !this.ScoreDic.ContainsKey(ID) ? 3 : this.ScoreDic[ID].Trophy;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("ScoreDic"))
        this.ScoreDic = reader.ReadDictionary<string, OmniScoreList>("ScoreDic");
      if (!reader.TagExists("StoredPlayerName"))
        return;
      this.StoredPlayerName = reader.Read<string>("StoredPlayerName");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<string, OmniScoreList>(this.ScoreDic, "ScoreDic");
      writer.Write<string>(this.StoredPlayerName, "StoredPlayerName");
    }
  }
}
