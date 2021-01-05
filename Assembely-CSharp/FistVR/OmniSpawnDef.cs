using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniSpawnDef : ScriptableObject
	{
		public GameObject SpawnerPrefab;

		public virtual List<int> CalculateSpawnerScoreThresholds()
		{
			return null;
		}
	}
}
