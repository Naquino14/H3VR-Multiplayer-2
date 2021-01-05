using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Mortar Spawn Def", menuName = "OmniSequencer/SpawnDef/Mortar Spawn Definition", order = 0)]
	public class OmniSpawnDef_Mortar : OmniSpawnDef
	{
		public enum MortarAngle
		{
			DownRange,
			Centered,
			UpRange
		}

		public int NumShots = 1;

		public float MortarSize = 1f;

		public Vector2 SpawnCadence = new Vector2(0.25f, 0.25f);

		public Vector2 VelocityRange = new Vector2(10f, 10f);

		public MortarAngle Angle;

		public override List<int> CalculateSpawnerScoreThresholds()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < NumShots; j++)
			{
				list[0] += 100;
				list[1] += 75;
				list[2] += 50;
			}
			return list;
		}
	}
}
