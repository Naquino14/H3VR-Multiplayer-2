using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Disc Spawn Def", menuName = "OmniSequencer/SpawnDef/Disc Spawn Definition", order = 0)]
	public class OmniSpawnDef_Discs : OmniSpawnDef
	{
		public enum DiscType
		{
			Normal,
			NoShoot,
			Armored,
			RedRing,
			Bullseye
		}

		public enum DiscSpawnStyle
		{
			AllAtOnce = -1,
			Sequential,
			OnHit
		}

		public enum DiscSpawnPattern
		{
			Circle,
			Square,
			LineXCentered,
			LineYCentered,
			LineXUp,
			LineXDown,
			LineYLeft,
			LineYRight,
			DiagonalLeftUp,
			DiagonalRightUp,
			DiagonalLeftDown,
			DiagonalRightDown,
			SpiralCounterclockwise,
			SpiralClockwise
		}

		public enum DiscZConfig
		{
			Homogenous,
			Incremented
		}

		public enum DiscSpawnOrdering
		{
			InOrder,
			Random
		}

		public enum DiscMovementPattern
		{
			Static,
			OscillateX,
			OscillateY,
			OscillateZ,
			OscillateXY,
			ClockwiseRot,
			CounterClockwiseRot
		}

		public enum DiscMovementStyle
		{
			Linear,
			Sinusoidal,
			Rotational,
			RotationalSwell
		}

		public List<DiscType> Discs;

		public DiscSpawnStyle SpawnStyle;

		public DiscSpawnPattern SpawnPattern;

		public DiscZConfig ZConfig;

		public DiscSpawnOrdering SpawnOrdering;

		public DiscMovementPattern MovementPattern;

		public DiscMovementStyle MovementStyle;

		public float TimeBetweenDiscSpawns = 0.1f;

		public float DiscMovementSpeed = 1f;

		public override List<int> CalculateSpawnerScoreThresholds()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Discs.Count; j++)
			{
				switch (Discs[j])
				{
				case DiscType.Normal:
					list[0] += 100;
					list[1] += 75;
					list[2] += 50;
					break;
				case DiscType.Armored:
					list[0] += 1000;
					list[1] += 750;
					list[2] += 500;
					break;
				case DiscType.RedRing:
					list[0] += 100;
					list[1] += 75;
					list[2] += 50;
					break;
				case DiscType.Bullseye:
					list[0] += 200;
					list[1] += 100;
					list[2] += 50;
					break;
				}
			}
			return list;
		}
	}
}
