namespace Valve.VR
{
	public interface ISteamVR_Action_Boolean : ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		bool state
		{
			get;
		}

		bool stateDown
		{
			get;
		}

		bool stateUp
		{
			get;
		}

		bool lastState
		{
			get;
		}

		bool lastStateDown
		{
			get;
		}

		bool lastStateUp
		{
			get;
		}
	}
}
