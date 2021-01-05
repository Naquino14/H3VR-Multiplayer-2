using System;

namespace Anvil
{
	[Serializable]
	public struct AssetID : IEquatable<AssetID>
	{
		public string Guid;

		public string Bundle;

		public string AssetName;

		public bool Equals(AssetID other)
		{
			return string.Equals(Bundle, other.Bundle) && string.Equals(AssetName, other.AssetName);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is AssetID && Equals((AssetID)obj);
		}

		public override int GetHashCode()
		{
			return (((Bundle != null) ? Bundle.GetHashCode() : 0) * 397) ^ ((AssetName != null) ? AssetName.GetHashCode() : 0);
		}
	}
}
