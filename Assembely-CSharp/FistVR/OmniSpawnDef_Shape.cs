using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Shape Spawn Def", menuName = "OmniSequencer/SpawnDef/Shape Spawn Definition", order = 0)]
	public class OmniSpawnDef_Shape : OmniSpawnDef
	{
		public enum ShapeInstruction
		{
			ShootTheColor,
			ShootTheShape,
			ShootTheColorShape,
			ShootAllTheColor,
			ShootAllTheShape,
			ShootAllTheColorShape,
			ShootAllTheNotColor,
			ShootAllTheNotShape,
			ShootAllTheNotColorShape
		}

		public enum OmniShapeType
		{
			Circle,
			Triangle,
			Square,
			Pentagon,
			Hexagon,
			Diamond,
			Heptagon,
			Octagon
		}

		public enum OmniShapeColor
		{
			Red,
			Orange,
			Yellow,
			Green,
			Blue,
			Purple,
			Pink,
			Brown
		}

		public int ShapeAmount = 3;

		public List<ShapeInstruction> Instructions;

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
				case ShapeInstruction.ShootAllTheColor:
					num = (int)((float)ShapeAmount * 0.5f);
					break;
				case ShapeInstruction.ShootAllTheColorShape:
					num = (int)((float)ShapeAmount * 0.5f);
					break;
				case ShapeInstruction.ShootAllTheNotColor:
					num = ShapeAmount - (int)((float)ShapeAmount * 0.5f);
					break;
				case ShapeInstruction.ShootAllTheNotColorShape:
					num = ShapeAmount - (int)((float)ShapeAmount * 0.5f);
					break;
				case ShapeInstruction.ShootAllTheNotShape:
					num = ShapeAmount - (int)((float)ShapeAmount * 0.5f);
					break;
				case ShapeInstruction.ShootAllTheShape:
					num = (int)((float)ShapeAmount * 0.5f);
					break;
				}
				list[0] += 100 * num;
				list[1] += 70 * num;
				list[2] += 40 * num;
			}
			return list;
		}
	}
}
