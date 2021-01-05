using UnityEngine;

namespace Valve.VR
{
	public interface ISteamVR_Action_Vector2 : ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		Vector2 axis
		{
			get;
		}

		Vector2 lastAxis
		{
			get;
		}

		Vector2 delta
		{
			get;
		}

		Vector2 lastDelta
		{
			get;
		}
	}
}
