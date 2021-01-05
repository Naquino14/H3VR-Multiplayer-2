namespace Valve.VR
{
	public interface ISteamVR_Action_Source
	{
		bool active
		{
			get;
		}

		bool activeBinding
		{
			get;
		}

		bool lastActive
		{
			get;
		}

		bool lastActiveBinding
		{
			get;
		}

		string fullPath
		{
			get;
		}

		ulong handle
		{
			get;
		}

		SteamVR_ActionSet actionSet
		{
			get;
		}

		SteamVR_ActionDirections direction
		{
			get;
		}
	}
}
