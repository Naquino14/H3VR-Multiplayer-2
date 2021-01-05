using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Skeleton
	{
		public string output;

		public string path;

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Skeleton)
			{
				SteamVR_Input_BindingFile_Skeleton steamVR_Input_BindingFile_Skeleton = (SteamVR_Input_BindingFile_Skeleton)obj;
				if (steamVR_Input_BindingFile_Skeleton.output == output && steamVR_Input_BindingFile_Skeleton.path == path)
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
