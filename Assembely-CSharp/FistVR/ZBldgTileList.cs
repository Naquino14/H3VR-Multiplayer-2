using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New zBldgTable", menuName = "Zosig/zBldgTable", order = 0)]
	public class ZBldgTileList : ScriptableObject
	{
		[Serializable]
		public class ZTileSet
		{
			public List<UnityEngine.Object> Tiles = new List<UnityEngine.Object>();
		}

		public List<ZTileSet> Tilesets = new List<ZTileSet>();
	}
}
