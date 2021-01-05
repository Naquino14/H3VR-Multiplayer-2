using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Wearable Config", menuName = "Sosig/WearableConfig", order = 0)]
	public class SosigWearableConfig : ScriptableObject
	{
		public List<GameObject> Headwear;

		public float Chance_Headwear;

		[Space(5f)]
		public List<GameObject> Facewear;

		public float Chance_Facewear;

		[Space(5f)]
		public List<GameObject> Torsowear;

		public float Chance_Torsowear;

		[Space(5f)]
		public List<GameObject> Pantswear;

		public float Chance_Pantswear;

		[Space(5f)]
		public List<GameObject> Pantswear_Lower;

		public float Chance_Pantswear_Lower;

		[Space(5f)]
		public List<GameObject> Backpacks;

		public float Chance_Backpacks;
	}
}
