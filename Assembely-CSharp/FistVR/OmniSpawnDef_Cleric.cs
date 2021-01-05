using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Cleric Spawn Def", menuName = "OmniSequencer/SpawnDef/Cleric Spawn Definition", order = 0)]
	public class OmniSpawnDef_Cleric : OmniSpawnDef
	{
		[Serializable]
		public class ClericSet
		{
			public TargetSpawnStyle SpawnStyle;

			public List<TargetLocation> TargetSet;

			public List<TargetLocation> BlockerSet;

			public bool LocationsRandomized;

			public float SequentialTiming = 0.2f;

			public bool FiresBack;

			public float FiringTime = 1f;
		}

		public enum TargetSpawnStyle
		{
			AllAtOnce = -1,
			Sequential,
			OnHit
		}

		public enum TargetLocation
		{
			FrontLeft1,
			FrontLeft2,
			Left1,
			Left2,
			Left3,
			BackLeft1,
			BackLeft2,
			FrontRight1,
			FrontRight2,
			Right1,
			Right2,
			Right3,
			BackRight1,
			BackRight2
		}

		public float TimeBetweenSets = 1f;

		public List<ClericSet> Sets;

		public override List<int> CalculateSpawnerScoreThresholds()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Sets.Count; j++)
			{
				for (int k = 0; k < Sets[j].TargetSet.Count; k++)
				{
					list[0] += 200;
					list[1] += 100;
					list[2] += 50;
				}
			}
			return list;
		}
	}
}
