using System;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "ItemSpawner/CategoryDefinition", order = 0)]
	public class ItemSpawnerCategoryDefinitions : ScriptableObject
	{
		[Serializable]
		public class Category
		{
			public ItemSpawnerID.EItemCategory Cat;

			public string DisplayName;

			public Sprite Sprite;

			public SubCategory[] Subcats;

			[Header("DisplayToggle")]
			public bool DoesDisplay_Sandbox = true;

			public bool DoesDisplay_Unlocks = true;
		}

		[Serializable]
		public class SubCategory
		{
			public ItemSpawnerID.ESubCategory Subcat;

			public string DisplayName;

			public Sprite Sprite;

			[Header("DisplayToggle")]
			public bool DoesDisplay_Sandbox = true;

			public bool DoesDisplay_Unlocks = true;
		}

		public Category[] Categories;
	}
}
