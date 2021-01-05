using System;
using UnityEngine;

[Serializable]
public class SBPlaceableDef
{
	public string ObjectID;

	public Vector3 Position;

	public Quaternion Rotation;

	public SerializableStringDictionary Flags;
}
