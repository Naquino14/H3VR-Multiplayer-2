using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "ItemSpawner/ObjectDefinition", order = 0)]
	public class ItemSpawnerObjectDefinition : ScriptableObject
	{
		public enum ItemSpawnerCategory
		{
			None = -1,
			Melee,
			Explosives,
			Handguns,
			Shotguns,
			Submachineguns,
			Rifles,
			Heavyweapons,
			Magazines,
			Ammunition,
			Attachments,
			Misc,
			Horseshoes
		}

		public string DisplayName;

		public ItemSpawnerCategory Category;

		public GameObject Prefab;

		public Sprite Sprite;

		public GameObject[] AdditionalPrefabs;
	}
}
