using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New EnemyTemplate", menuName = "TNH/TNH_EnemyTemplate", order = 0)]
	public class TNH_EnemyTemplate : ScriptableObject
	{
		public TNH_EnemyType Type;

		public List<FVRObject> SosigPrefabs;

		public SosigConfigTemplate ConfigTemplate_Standard;

		public SosigConfigTemplate ConfigTemplate_Easy;

		public List<SosigOutfitConfig> OutfitConfig;

		public List<FVRObject> WeaponOptions;

		public List<FVRObject> WeaponOptions_Secondary;

		public float SecondaryChance;
	}
}
