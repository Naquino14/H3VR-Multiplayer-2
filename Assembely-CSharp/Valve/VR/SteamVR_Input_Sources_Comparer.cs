using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SteamVR_Input_Sources_Comparer : IEqualityComparer<SteamVR_Input_Sources>
	{
		public bool Equals(SteamVR_Input_Sources x, SteamVR_Input_Sources y)
		{
			return x == y;
		}

		public int GetHashCode(SteamVR_Input_Sources obj)
		{
			return (int)obj;
		}
	}
}
