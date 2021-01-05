using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Loot Table", menuName = "Zosig/LootTable", order = 0)]
	public class ZosigItemSpawnTable : ScriptableObject
	{
		public List<FVRObject> Objects;
	}
}
