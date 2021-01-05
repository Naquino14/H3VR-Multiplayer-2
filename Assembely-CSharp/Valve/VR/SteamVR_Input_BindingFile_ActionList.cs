using System;
using System.Collections.Generic;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_ActionList
	{
		public List<SteamVR_Input_BindingFile_Chord> chords = new List<SteamVR_Input_BindingFile_Chord>();

		public List<SteamVR_Input_BindingFile_Pose> poses = new List<SteamVR_Input_BindingFile_Pose>();

		public List<SteamVR_Input_BindingFile_Haptic> haptics = new List<SteamVR_Input_BindingFile_Haptic>();

		public List<SteamVR_Input_BindingFile_Source> sources = new List<SteamVR_Input_BindingFile_Source>();

		public List<SteamVR_Input_BindingFile_Skeleton> skeleton = new List<SteamVR_Input_BindingFile_Skeleton>();
	}
}
