using System;
using System.Collections.Generic;
using System.Linq;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Source_Input_StringDictionary : Dictionary<string, string>
	{
		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Source_Input_StringDictionary)
			{
				SteamVR_Input_BindingFile_Source_Input_StringDictionary steamVR_Input_BindingFile_Source_Input_StringDictionary = (SteamVR_Input_BindingFile_Source_Input_StringDictionary)obj;
				if (this == steamVR_Input_BindingFile_Source_Input_StringDictionary)
				{
					return true;
				}
				return base.Count == steamVR_Input_BindingFile_Source_Input_StringDictionary.Count && !this.Except(steamVR_Input_BindingFile_Source_Input_StringDictionary).Any();
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
