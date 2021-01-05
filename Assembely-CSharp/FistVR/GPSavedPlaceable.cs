using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class GPSavedPlaceable
	{
		public string ObjectID;

		public int UniqueID;

		public Vector3 Position;

		public Quaternion Rotation;

		public List<string> InternalList;

		public SerializableStringDictionary Flags;
	}
}
