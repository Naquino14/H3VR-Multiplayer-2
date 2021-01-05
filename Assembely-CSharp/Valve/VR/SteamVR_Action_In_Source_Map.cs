using System.Collections.Generic;

namespace Valve.VR
{
	public class SteamVR_Action_In_Source_Map<SourceElement> : SteamVR_Action_Source_Map<SourceElement> where SourceElement : SteamVR_Action_In_Source, new()
	{
		protected List<SteamVR_Input_Sources> updatingSources = new List<SteamVR_Input_Sources>();

		public bool IsUpdating(SteamVR_Input_Sources inputSource)
		{
			for (int i = 0; i < updatingSources.Count; i++)
			{
				if (inputSource == updatingSources[i])
				{
					return true;
				}
			}
			return false;
		}

		protected override void OnAccessSource(SteamVR_Input_Sources inputSource)
		{
			if (SteamVR_Action.startUpdatingSourceOnAccess)
			{
				ForceAddSourceToUpdateList(inputSource);
			}
		}

		public void ForceAddSourceToUpdateList(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sources[inputSource];
			if (!val.isUpdating)
			{
				updatingSources.Add(inputSource);
				SourceElement val2 = sources[inputSource];
				val2.isUpdating = true;
				if (!SteamVR_Input.isStartupFrame)
				{
					SourceElement val3 = sources[inputSource];
					val3.UpdateValue();
				}
			}
		}

		public void UpdateValues()
		{
			for (int i = 0; i < updatingSources.Count; i++)
			{
				SourceElement val = sources[updatingSources[i]];
				val.UpdateValue();
			}
		}
	}
}
