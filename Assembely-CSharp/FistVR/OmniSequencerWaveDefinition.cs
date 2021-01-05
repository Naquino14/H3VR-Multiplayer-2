using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Wave Def", menuName = "OmniSequencer/WaveDefinition", order = 0)]
	public class OmniSequencerWaveDefinition : ScriptableObject
	{
		[Serializable]
		public class SpawnerAtRange
		{
			public OmniSpawnDef Def;

			public OmniWaveEngagementRange Range;
		}

		public float TimeForWarmup;

		public float TimeForWarmupRandomAddition;

		public float TimeForWave;

		public float ScoreMultiplier_Points = 1f;

		public float ScoreMultiplier_Time = 1f;

		public float ScoreMultiplier_Range = 1f;

		[Space(10f)]
		public bool UsesQuickDraw;

		public bool UsesReflex;

		[Space(10f)]
		public List<SpawnerAtRange> Spawners;

		public List<int> CalculateScoreThresholdsForWave()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Spawners.Count; j++)
			{
				List<int> list2 = Spawners[j].Def.CalculateSpawnerScoreThresholds();
				for (int k = 0; k < list2.Count; k++)
				{
					float num = (float)list2[k] * ScoreMultiplier_Points;
					num += num * ScoreMultiplier_Range * RangeToScoreMultiplier(Spawners[j].Range);
					list[k] += (int)num;
				}
			}
			int num2 = (int)(TimeForWave * 0.2f * 100f * ScoreMultiplier_Time);
			list[0] = (int)((float)list[0] * 0.9f);
			list[1] = (int)((float)list[1] * 0.9f);
			list[2] = (int)((float)list[2] * 0.9f);
			return list;
		}

		public float RangeToScoreMultiplier(OmniWaveEngagementRange range)
		{
			return range switch
			{
				OmniWaveEngagementRange.m5 => 0f, 
				OmniWaveEngagementRange.m10 => 0.1f, 
				OmniWaveEngagementRange.m15 => 0.25f, 
				OmniWaveEngagementRange.m20 => 0.5f, 
				OmniWaveEngagementRange.m25 => 1f, 
				OmniWaveEngagementRange.m50 => 2f, 
				OmniWaveEngagementRange.m100 => 3f, 
				OmniWaveEngagementRange.m150 => 4f, 
				OmniWaveEngagementRange.m200 => 5f, 
				_ => 1f, 
			};
		}
	}
}
