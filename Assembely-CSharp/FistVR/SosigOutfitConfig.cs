using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Outfit Config", menuName = "Sosig/OutfitConfig", order = 0)]
	public class SosigOutfitConfig : ScriptableObject
	{
		public List<FVRObject> Headwear;

		public float Chance_Headwear;

		[Space(5f)]
		public List<FVRObject> Eyewear;

		public float Chance_Eyewear;

		[Space(5f)]
		public List<FVRObject> Facewear;

		public float Chance_Facewear;

		[Space(5f)]
		public List<FVRObject> Torsowear;

		public float Chance_Torsowear;

		[Space(5f)]
		public List<FVRObject> Pantswear;

		public float Chance_Pantswear;

		[Space(5f)]
		public List<FVRObject> Pantswear_Lower;

		public float Chance_Pantswear_Lower;

		[Space(5f)]
		public List<FVRObject> Backpacks;

		public float Chance_Backpacks;
	}
}
