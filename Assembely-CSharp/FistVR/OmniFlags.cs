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
			if (!ScoreDic.ContainsKey(def.SequenceID))
			{
				List<OmniScore> scores = new List<OmniScore>();
				OmniScoreList omniScoreList = new OmniScoreList();
				omniScoreList.SequenceID = def.SequenceID;
				omniScoreList.Scores = scores;
				ScoreDic.Add(def.SequenceID, omniScoreList);
			}
			List<OmniScore> scores2 = ScoreDic[def.SequenceID].Scores;
			OmniScore omniScore = new OmniScore();
			omniScore.Score = score;
			omniScore.Name = StoredPlayerName;
			scores2.Add(omniScore);
			scores2.Sort();
			if (scores2.Count > 6)
			{
				scores2.RemoveRange(6, scores2.Count - 6);
			}
			for (int i = 0; i < def.ScoreThresholds.Count; i++)
			{
				if (score >= def.ScoreThresholds[i] && i < ScoreDic[def.SequenceID].Trophy)
				{
					ScoreDic[def.SequenceID].Trophy = i;
					break;
				}
			}
			ScoreDic[def.SequenceID].Scores = scores2;
		}

		public void AddScore(string defID, int score)
		{
			if (!ScoreDic.ContainsKey(defID))
			{
				List<OmniScore> scores = new List<OmniScore>();
				OmniScoreList omniScoreList = new OmniScoreList();
				omniScoreList.SequenceID = defID;
				omniScoreList.Scores = scores;
				ScoreDic.Add(defID, omniScoreList);
			}
			List<OmniScore> scores2 = ScoreDic[defID].Scores;
			OmniScore omniScore = new OmniScore();
			omniScore.Score = score;
			omniScore.Name = StoredPlayerName;
			scores2.Add(omniScore);
			scores2.Sort();
			if (scores2.Count > 6)
			{
				scores2.RemoveRange(6, scores2.Count - 6);
			}
			ScoreDic[defID].Scores = scores2;
		}

		public List<OmniScore> GetScoreList(string ID)
		{
			if (!ScoreDic.ContainsKey(ID))
			{
				return new List<OmniScore>();
			}
			return ScoreDic[ID].Scores;
		}

		public int GetRank(string ID)
		{
			if (!ScoreDic.ContainsKey(ID))
			{
				return 3;
			}
			return ScoreDic[ID].Trophy;
		}

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("ScoreDic"))
			{
				ScoreDic = reader.ReadDictionary<string, OmniScoreList>("ScoreDic");
			}
			if (reader.TagExists("StoredPlayerName"))
			{
				StoredPlayerName = reader.Read<string>("StoredPlayerName");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(ScoreDic, "ScoreDic");
			writer.Write(StoredPlayerName, "StoredPlayerName");
		}
	}
}
