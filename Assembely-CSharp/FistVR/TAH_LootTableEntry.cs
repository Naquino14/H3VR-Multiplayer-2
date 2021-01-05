using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class TAH_LootTableEntry
	{
		public FVRObject MainObj;

		public FVRObject SecondaryObj;

		public Vector2 Nums;

		public int AttachmentSpawn;

		public bool UsesLargeCase = true;
	}
}
