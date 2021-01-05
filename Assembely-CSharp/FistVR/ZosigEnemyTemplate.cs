using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New EnemyTemplate", menuName = "Zosig/EnemyTemplate", order = 0)]
	public class ZosigEnemyTemplate : ScriptableObject
	{
		public List<GameObject> SosigPrefabs;

		public List<SosigConfigTemplate> ConfigTemplates;

		public List<SosigWearableConfig> WearableTemplates;

		public List<FVRObject> WeaponOptions;

		public List<FVRObject> WeaponOptions_Secondary;
	}
}
