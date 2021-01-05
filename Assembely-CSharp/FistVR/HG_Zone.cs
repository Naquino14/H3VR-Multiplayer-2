using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_Zone : MonoBehaviour
	{
		public bool DebugView;

		public Transform PlayerSpawnPoint;

		public List<Transform> SpawnPoints_Offense;

		public List<Transform> SpawnPoints_Defense;

		public List<Transform> TargetPoints;

		public Transform Indicator;
	}
}
