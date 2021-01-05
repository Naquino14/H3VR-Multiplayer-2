using UnityEngine;

namespace Valve.VR.Extras
{
	public struct PointerEventArgs
	{
		public SteamVR_Input_Sources fromInputSource;

		public uint flags;

		public float distance;

		public Transform target;
	}
}
