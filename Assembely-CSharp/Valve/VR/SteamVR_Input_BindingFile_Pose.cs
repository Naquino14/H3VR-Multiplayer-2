using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Pose
	{
		public string output;

		public string path;

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Pose)
			{
				SteamVR_Input_BindingFile_Pose steamVR_Input_BindingFile_Pose = (SteamVR_Input_BindingFile_Pose)obj;
				if (steamVR_Input_BindingFile_Pose.output == output && steamVR_Input_BindingFile_Pose.path == path)
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
