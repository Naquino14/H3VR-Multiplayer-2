using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Sequence Def", menuName = "OmniSequencer/SequenceDefinition", order = 0)]
	public class OmniSequencerSequenceDefinition : ScriptableObject
	{
		public enum OmniSequenceTheme
		{
			GettingStarted,
			CasualPlinking,
			OpenSeason,
			LightningReflexes,
			SixShooter,
			MentalGymnastics,
			SprayAndPray,
			DownRange
		}

		public enum OmniSequenceDifficulty
		{
			Beginner,
			Intermediate,
			Advanced,
			Master,
			Impossible
		}

		public enum OmniSequenceFirearmMode
		{
			Open,
			Category,
			Fixed
		}

		public enum OmniSequenceAmmoMode
		{
			Infinite,
			Spawnlockable,
			Fixed
		}

		public string SequenceName;

		[Space(10f)]
		public string SequenceID;

		[Space(10f)]
		[Multiline(8)]
		public string Description;

		[Space(10f)]
		public OmniSequenceTheme Theme;

		[Space(10f)]
		public OmniSequenceDifficulty Difficulty;

		[Space(10f)]
		public List<int> ScoreThresholds = new List<int>();

		[Space(10f)]
		public List<int> CurrencyForRanks = new List<int>();

		[Space(10f)]
		public List<OmniSequencerWaveDefinition> Waves;

		public OmniSequenceFirearmMode FirearmMode;

		public OmniSequenceAmmoMode AmmoMode;

		public List<ItemSpawnerID.ESubCategory> AllowedCats = new List<ItemSpawnerID.ESubCategory>();

		public int GetRankForScore(int score)
		{
			for (int i = 0; i < ScoreThresholds.Count; i++)
			{
				if (score >= ScoreThresholds[i])
				{
					return i;
				}
			}
			return 3;
		}

		[ContextMenu("CalculateScoreThreshold")]
		public void CalculateScoreThreshold()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Waves.Count; j++)
			{
				List<int> list2 = Waves[j].CalculateScoreThresholdsForWave();
				for (int k = 0; k < list2.Count; k++)
				{
					list[k] += list2[k];
				}
			}
			ScoreThresholds.Clear();
			ScoreThresholds.Add(list[0]);
			ScoreThresholds.Add(list[1]);
			ScoreThresholds.Add(list[2]);
			Debug.Log("Gold Score Needed:" + list[0]);
			Debug.Log("Silver Score Needed:" + list[1]);
			Debug.Log("Bronze Score Needed:" + list[2]);
		}
	}
}
