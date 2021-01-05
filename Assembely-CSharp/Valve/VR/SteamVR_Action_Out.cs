using System;

namespace Valve.VR
{
	[Serializable]
	public abstract class SteamVR_Action_Out<SourceMap, SourceElement> : SteamVR_Action<SourceMap, SourceElement>, ISteamVR_Action_Out, ISteamVR_Action, ISteamVR_Action_Out_Source, ISteamVR_Action_Source where SourceMap : SteamVR_Action_Source_Map<SourceElement>, new()where SourceElement : SteamVR_Action_Out_Source, new()
	{
	}
}
