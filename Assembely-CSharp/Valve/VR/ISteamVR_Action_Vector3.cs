using UnityEngine;

namespace Valve.VR
{
	public interface ISteamVR_Action_Vector3 : ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		Vector3 axis
		{
			get;
		}

		Vector3 lastAxis
		{
			get;
		}

		Vector3 delta
		{
			get;
		}

		Vector3 lastDelta
		{
			get;
		}
	}
}
