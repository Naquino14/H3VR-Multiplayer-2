using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New IPSC Spawn Def", menuName = "OmniSequencer/SpawnDef/IPSC Spawn Definition", order = 0)]
	public class OmniSpawnDef_IPSC : OmniSpawnDef
	{
		public enum IPSCType
		{
			Standard,
			NoShoot
		}

		public List<IPSCType> TargetList;

		public Vector2 SpawnCadence = new Vector2(0.25f, 0.25f);

		public Vector2 TimeActivated = new Vector2(1f, 1f);

		public override List<int> CalculateSpawnerScoreThresholds()
		{
			List<int> list = new List<int>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < TargetList.Count; j++)
			{
				if (TargetList[j] == IPSCType.Standard)
				{
					list[0] += 100;
					list[1] += 70;
					list[2] += 30;
				}
			}
			return list;
		}
	}
}
