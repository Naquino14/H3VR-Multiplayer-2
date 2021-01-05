using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_ResourceLib", menuName = "TNH/TNH_ResourceLib", order = 0)]
	public class TNH_ResourceLib : ScriptableObject
	{
		public List<FVRObject> EncryptionObjects;

		public List<FVRObject> TurretObjects;

		public List<FVRObject> TrapObjects;
	}
}
