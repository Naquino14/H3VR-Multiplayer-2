using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Haptic
	{
		public string output;

		public string path;

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Haptic)
			{
				SteamVR_Input_BindingFile_Haptic steamVR_Input_BindingFile_Haptic = (SteamVR_Input_BindingFile_Haptic)obj;
				if (steamVR_Input_BindingFile_Haptic.output == output && steamVR_Input_BindingFile_Haptic.path == path)
				{
					return true;
				}
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
