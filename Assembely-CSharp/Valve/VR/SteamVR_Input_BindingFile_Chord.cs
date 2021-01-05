using System;
using System.Collections.Generic;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Chord
	{
		public string output;

		public List<List<string>> inputs = new List<List<string>>();

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Chord)
			{
				SteamVR_Input_BindingFile_Chord steamVR_Input_BindingFile_Chord = (SteamVR_Input_BindingFile_Chord)obj;
				if (output == steamVR_Input_BindingFile_Chord.output && inputs != null && steamVR_Input_BindingFile_Chord.inputs != null && inputs.Count == steamVR_Input_BindingFile_Chord.inputs.Count)
				{
					for (int i = 0; i < inputs.Count; i++)
					{
						if (inputs[i] == null || steamVR_Input_BindingFile_Chord.inputs[i] == null || inputs[i].Count != steamVR_Input_BindingFile_Chord.inputs[i].Count)
						{
							continue;
						}
						for (int j = 0; j < inputs[i].Count; j++)
						{
							if (inputs[i][j] != steamVR_Input_BindingFile_Chord.inputs[i][j])
							{
								return false;
							}
						}
						return true;
					}
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
