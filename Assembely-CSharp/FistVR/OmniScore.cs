using System;

namespace FistVR
{
	[Serializable]
	public class OmniScore : IComparable<OmniScore>
	{
		public int Score;

		public string Name = string.Empty;

		public int CompareTo(OmniScore that)
		{
			if (that == null)
			{
				return 1;
			}
			if (Score > that.Score)
			{
				return -1;
			}
			if (Score < that.Score)
			{
				return 1;
			}
			return 0;
		}
	}
}
