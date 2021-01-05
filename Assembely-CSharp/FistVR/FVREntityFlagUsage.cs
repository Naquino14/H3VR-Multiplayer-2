using System;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "Entities/EntityFlagUsage", order = 0)]
	public class FVREntityFlagUsage : ScriptableObject
	{
		[Serializable]
		public class StoredBoolFlag
		{
			public string Name;

			public bool ExposedForUserEdit;

			public bool DefaultValue;
		}

		[Serializable]
		public class StoredIntFlag
		{
			public string Name;

			public bool ExposedForUserEdit;

			public int MinValue;

			public int MaxValue = 1;

			public int DefaultValue;
		}

		[Serializable]
		public class StoredFloatFlag
		{
			public string Name;

			public bool ExposedForUserEdit;

			public float MinValue;

			public float MaxValue = 1f;

			public float DefaultValue;
		}

		[Serializable]
		public class StoredVector4Flag
		{
			public string Name;

			public bool ExposedForUserEdit;

			public Vector4 MinValues = new Vector4(0f, 0f, 0f, 0f);

			public Vector4 MaxValues = new Vector4(1f, 1f, 1f, 1f);

			public EntityVectorFlagUsage Usage;

			public Vector4 DefaultValue;
		}

		[Serializable]
		public class StoredString
		{
			public string Name;

			public bool ExposedForUserEdit;

			public int MaxLength = 30;

			public string DefaultValue;
		}

		public enum EntityVectorFlagUsage
		{
			Default,
			Color
		}

		public bool IsSerialized = true;

		[Header("Bool Params")]
		public StoredBoolFlag[] BoolFlags;

		[Header("Int Params")]
		public StoredIntFlag[] IntFlags;

		[Header("Float Params")]
		public StoredFloatFlag[] FloatFlags;

		[Header("Vector4 Params")]
		public StoredVector4Flag[] Vector4Flags;

		[Header("String Params")]
		public StoredString[] StringFlags;
	}
}
