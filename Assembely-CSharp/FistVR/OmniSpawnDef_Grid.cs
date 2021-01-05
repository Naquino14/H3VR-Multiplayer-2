using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Grid Spawn Def", menuName = "OmniSequencer/SpawnDef/Grid Spawn Definition", order = 0)]
	public class OmniSpawnDef_Grid : OmniSpawnDef
	{
		public enum GridSize
		{
			To9,
			To16,
			To25
		}

		public enum GridConfiguration
		{
			Ascending,
			Descending,
			Shuffled
		}

		public enum GridInstruction
		{
			CountUp,
			CountDown,
			Addition,
			Subtraction,
			Multiplication,
			Division,
			Odds,
			Evens,
			Multiples3,
			Multiples4,
			Primes,
			GreaterThan,
			LessThan
		}

		public GridSize Size;

		public GridConfiguration Configuration;

		public List<GridInstruction> Instructions;

		public override List<int> CalculateSpawnerScoreThresholds()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < Instructions.Count; j++)
			{
				int num = 1;
				switch (Instructions[j])
				{
				case GridInstruction.Addition:
					num = 3;
					break;
				case GridInstruction.Subtraction:
					num = 3;
					break;
				case GridInstruction.Multiplication:
					num = 3;
					break;
				case GridInstruction.Division:
					num = 3;
					break;
				case GridInstruction.Odds:
					switch (Size)
					{
					case GridSize.To9:
						num = 5;
						break;
					case GridSize.To16:
						num = 8;
						break;
					case GridSize.To25:
						num = 13;
						break;
					}
					break;
				case GridInstruction.Evens:
					switch (Size)
					{
					case GridSize.To9:
						num = 4;
						break;
					case GridSize.To16:
						num = 8;
						break;
					case GridSize.To25:
						num = 12;
						break;
					}
					break;
				case GridInstruction.CountUp:
					switch (Size)
					{
					case GridSize.To9:
						num = 9;
						break;
					case GridSize.To16:
						num = 16;
						break;
					case GridSize.To25:
						num = 25;
						break;
					}
					break;
				case GridInstruction.CountDown:
					switch (Size)
					{
					case GridSize.To9:
						num = 9;
						break;
					case GridSize.To16:
						num = 16;
						break;
					case GridSize.To25:
						num = 25;
						break;
					}
					break;
				case GridInstruction.Multiples3:
					switch (Size)
					{
					case GridSize.To9:
						num = 3;
						break;
					case GridSize.To16:
						num = 7;
						break;
					case GridSize.To25:
						num = 10;
						break;
					}
					break;
				case GridInstruction.Multiples4:
					switch (Size)
					{
					case GridSize.To9:
						num = 3;
						break;
					case GridSize.To16:
						num = 6;
						break;
					case GridSize.To25:
						num = 9;
						break;
					}
					break;
				case GridInstruction.Primes:
					switch (Size)
					{
					case GridSize.To9:
						num = 5;
						break;
					case GridSize.To16:
						num = 8;
						break;
					case GridSize.To25:
						num = 12;
						break;
					}
					break;
				case GridInstruction.GreaterThan:
					switch (Size)
					{
					case GridSize.To9:
						num = 4;
						break;
					case GridSize.To16:
						num = 8;
						break;
					case GridSize.To25:
						num = 12;
						break;
					}
					break;
				case GridInstruction.LessThan:
					switch (Size)
					{
					case GridSize.To9:
						num = 4;
						break;
					case GridSize.To16:
						num = 8;
						break;
					case GridSize.To25:
						num = 12;
						break;
					}
					break;
				}
				list[0] += 10 * num;
				list[1] += 7 * num;
				list[2] += 4 * num;
			}
			return list;
		}
	}
}
