using System.Collections.Generic;

namespace Valve.VR
{
	public class SteamVR_Action_Pose_Source_Map<Source> : SteamVR_Action_In_Source_Map<Source> where Source : SteamVR_Action_Pose_Source, new()
	{
		public void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newOrigin)
		{
			Dictionary<SteamVR_Input_Sources, Source>.Enumerator enumerator = sources.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.universeOrigin = newOrigin;
			}
		}

		public virtual void UpdateValues(bool skipStateAndEventUpdates)
		{
			for (int i = 0; i < updatingSources.Count; i++)
			{
				Source val = sources[updatingSources[i]];
				val.UpdateValue(skipStateAndEventUpdates);
			}
		}
	}
}
