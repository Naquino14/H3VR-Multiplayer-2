using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Skeleton_HandMask
	{
		public bool palm;

		public bool thumb;

		public bool index;

		public bool middle;

		public bool ring;

		public bool pinky;

		public bool[] values = new bool[6];

		public static readonly SteamVR_Skeleton_HandMask fullMask = new SteamVR_Skeleton_HandMask();

		public SteamVR_Skeleton_HandMask()
		{
			values = new bool[6];
			Reset();
		}

		public void SetFinger(int i, bool value)
		{
			values[i] = value;
			Apply();
		}

		public bool GetFinger(int i)
		{
			return values[i];
		}

		public void Reset()
		{
			values = new bool[6];
			for (int i = 0; i < 6; i++)
			{
				values[i] = true;
			}
			Apply();
		}

		protected void Apply()
		{
			palm = values[0];
			thumb = values[1];
			index = values[2];
			middle = values[3];
			ring = values[4];
			pinky = values[5];
		}
	}
}
