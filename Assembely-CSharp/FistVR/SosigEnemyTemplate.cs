using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New EnemyTemplate", menuName = "Sosig/EnemyTemplate", order = 0)]
	public class SosigEnemyTemplate : ScriptableObject
	{
		public string DisplayName;

		[SearchableEnum]
		public SosigEnemyCategory SosigEnemyCategory;

		[SearchableEnum]
		public SosigEnemyID SosigEnemyID = SosigEnemyID.None;

		public List<FVRObject> SosigPrefabs;

		public List<SosigConfigTemplate> ConfigTemplates;

		public List<SosigConfigTemplate> ConfigTemplates_Easy;

		public List<SosigOutfitConfig> OutfitConfig;

		public List<FVRObject> WeaponOptions;

		public List<FVRObject> WeaponOptions_Secondary;

		public float SecondaryChance;

		public List<FVRObject> WeaponOptions_Tertiary;

		public float TertiaryChance;

		[SearchableEnum]
		public TNH_EnemyType EnemyType = TNH_EnemyType.None;

		[ContextMenu("Autoname")]
		public void Autoname()
		{
		}
	}
}
