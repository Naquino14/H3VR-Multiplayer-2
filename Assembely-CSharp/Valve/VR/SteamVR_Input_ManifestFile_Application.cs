using System.Collections.Generic;

namespace Valve.VR
{
	public class SteamVR_Input_ManifestFile_Application
	{
		public string app_key;

		public string launch_type;

		public string url;

		public string binary_path_windows;

		public string binary_path_linux;

		public string binary_path_osx;

		public string action_manifest_path;

		public string image_path;

		public Dictionary<string, SteamVR_Input_ManifestFile_ApplicationString> strings = new Dictionary<string, SteamVR_Input_ManifestFile_ApplicationString>();
	}
}
