namespace Valve.VR
{
	public interface ISteamVR_Action_Single : ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		float axis
		{
			get;
		}

		float lastAxis
		{
			get;
		}

		float delta
		{
			get;
		}

		float lastDelta
		{
			get;
		}
	}
}
