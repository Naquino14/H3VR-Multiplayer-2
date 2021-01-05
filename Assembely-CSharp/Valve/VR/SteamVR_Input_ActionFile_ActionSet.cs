using System;
using System.Collections.Generic;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_ActionFile_ActionSet
	{
		[JsonIgnore]
		private const string actionSetInstancePrefix = "instance_";

		public string name;

		public string usage;

		private const string nameTemplate = "/actions/{0}";

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_Action> actionsInList = new List<SteamVR_Input_ActionFile_Action>();

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_Action> actionsOutList = new List<SteamVR_Input_ActionFile_Action>();

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_Action> actionsList = new List<SteamVR_Input_ActionFile_Action>();

		[JsonIgnore]
		public string codeFriendlyName => SteamVR_Input_ActionFile.GetCodeFriendlyName(name);

		[JsonIgnore]
		public string shortName
		{
			get
			{
				int num = name.LastIndexOf('/');
				if (num == name.Length - 1)
				{
					return string.Empty;
				}
				return SteamVR_Input_ActionFile.GetShortName(name);
			}
		}

		public void SetNewShortName(string newShortName)
		{
			name = GetPathFromName(newShortName);
		}

		public static string CreateNewName()
		{
			return GetPathFromName("NewSet");
		}

		public static string GetPathFromName(string name)
		{
			return $"/actions/{name}";
		}

		public static SteamVR_Input_ActionFile_ActionSet CreateNew()
		{
			SteamVR_Input_ActionFile_ActionSet steamVR_Input_ActionFile_ActionSet = new SteamVR_Input_ActionFile_ActionSet();
			steamVR_Input_ActionFile_ActionSet.name = CreateNewName();
			return steamVR_Input_ActionFile_ActionSet;
		}

		public SteamVR_Input_ActionFile_ActionSet GetCopy()
		{
			SteamVR_Input_ActionFile_ActionSet steamVR_Input_ActionFile_ActionSet = new SteamVR_Input_ActionFile_ActionSet();
			steamVR_Input_ActionFile_ActionSet.name = name;
			steamVR_Input_ActionFile_ActionSet.usage = usage;
			return steamVR_Input_ActionFile_ActionSet;
		}

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_ActionFile_ActionSet)
			{
				SteamVR_Input_ActionFile_ActionSet steamVR_Input_ActionFile_ActionSet = (SteamVR_Input_ActionFile_ActionSet)obj;
				if (steamVR_Input_ActionFile_ActionSet == this)
				{
					return true;
				}
				if (steamVR_Input_ActionFile_ActionSet.name == name)
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
