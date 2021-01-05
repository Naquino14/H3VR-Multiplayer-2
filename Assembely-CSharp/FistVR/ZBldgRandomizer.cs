using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZBldgRandomizer : MonoBehaviour
	{
		[InspectorButton("RandoTime")]
		public bool Shuffle;

		[InspectorButton("PopulateSpawnedTiles")]
		public bool GrabTiles;

		public List<GameObject> SpawnedTiles = new List<GameObject>();

		public ZBldgTileList L;
	}
}
