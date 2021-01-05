using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniClericRing : MonoBehaviour
	{
		public GameObject Map;

		public List<Transform> SpawnPoints;

		public List<Transform> Blockers;

		public List<Renderer> Indicators;

		public float BlockerMinHeight = 0.5f;

		public float BlockerMaxHeight = 1.9f;
	}
}
