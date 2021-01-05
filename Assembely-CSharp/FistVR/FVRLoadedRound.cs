using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class FVRLoadedRound
	{
		public FireArmRoundClass LR_Class;

		public Mesh LR_Mesh;

		public Material LR_Material;

		public FVRObject LR_ObjectWrapper;

		public GameObject LR_ProjectilePrefab;
	}
}
