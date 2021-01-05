using System;
using System.Collections.Generic;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile
	{
		public string app_key;

		public Dictionary<string, SteamVR_Input_BindingFile_ActionList> bindings = new Dictionary<string, SteamVR_Input_BindingFile_ActionList>();

		public string controller_type;

		public string description;

		public string name;
	}
}
