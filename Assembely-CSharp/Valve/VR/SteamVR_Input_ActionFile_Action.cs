using System;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_ActionFile_Action
	{
		[JsonIgnore]
		private static string[] _requirementValues;

		public string name;

		public string type;

		public string scope;

		public string skeleton;

		public string requirement;

		private const string nameTemplate = "/actions/{0}/{1}/{2}";

		protected const string prefix = "/actions/";

		[JsonIgnore]
		public static string[] requirementValues
		{
			get
			{
				if (_requirementValues == null)
				{
					_requirementValues = Enum.GetNames(typeof(SteamVR_Input_ActionFile_Action_Requirements));
				}
				return _requirementValues;
			}
		}

		[JsonIgnore]
		public SteamVR_Input_ActionFile_Action_Requirements requirementEnum
		{
			get
			{
				for (int i = 0; i < requirementValues.Length; i++)
				{
					if (string.Equals(requirementValues[i], requirement, StringComparison.CurrentCultureIgnoreCase))
					{
						return (SteamVR_Input_ActionFile_Action_Requirements)i;
					}
				}
				return SteamVR_Input_ActionFile_Action_Requirements.suggested;
			}
			set
			{
				requirement = value.ToString();
			}
		}

		[JsonIgnore]
		public string codeFriendlyName => SteamVR_Input_ActionFile.GetCodeFriendlyName(name);

		[JsonIgnore]
		public string shortName => SteamVR_Input_ActionFile.GetShortName(name);

		[JsonIgnore]
		public string path
		{
			get
			{
				int num = name.LastIndexOf('/');
				if (num != -1 && num + 1 < name.Length)
				{
					return name.Substring(0, num + 1);
				}
				return name;
			}
		}

		[JsonIgnore]
		public SteamVR_ActionDirections direction
		{
			get
			{
				if (type.ToLower() == SteamVR_Input_ActionFile_ActionTypes.vibration)
				{
					return SteamVR_ActionDirections.Out;
				}
				return SteamVR_ActionDirections.In;
			}
		}

		[JsonIgnore]
		public string actionSet
		{
			get
			{
				int num = name.IndexOf('/', "/actions/".Length);
				if (num == -1)
				{
					return string.Empty;
				}
				return name.Substring(0, num);
			}
		}

		public SteamVR_Input_ActionFile_Action GetCopy()
		{
			SteamVR_Input_ActionFile_Action steamVR_Input_ActionFile_Action = new SteamVR_Input_ActionFile_Action();
			steamVR_Input_ActionFile_Action.name = name;
			steamVR_Input_ActionFile_Action.type = type;
			steamVR_Input_ActionFile_Action.scope = scope;
			steamVR_Input_ActionFile_Action.skeleton = skeleton;
			steamVR_Input_ActionFile_Action.requirement = requirement;
			return steamVR_Input_ActionFile_Action;
		}

		public static string CreateNewName(string actionSet, string direction)
		{
			return string.Format("/actions/{0}/{1}/{2}", actionSet, direction, "NewAction");
		}

		public static string CreateNewName(string actionSet, SteamVR_ActionDirections direction, string actionName)
		{
			return $"/actions/{actionSet}/{direction.ToString().ToLower()}/{actionName}";
		}

		public static SteamVR_Input_ActionFile_Action CreateNew(string actionSet, SteamVR_ActionDirections direction, string actionType)
		{
			SteamVR_Input_ActionFile_Action steamVR_Input_ActionFile_Action = new SteamVR_Input_ActionFile_Action();
			steamVR_Input_ActionFile_Action.name = CreateNewName(actionSet, direction.ToString().ToLower());
			steamVR_Input_ActionFile_Action.type = actionType;
			return steamVR_Input_ActionFile_Action;
		}

		public void SetNewActionSet(string newSetName)
		{
			name = $"/actions/{newSetName}/{direction.ToString().ToLower()}/{shortName}";
		}

		public override string ToString()
		{
			return shortName;
		}

		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_ActionFile_Action)
			{
				SteamVR_Input_ActionFile_Action steamVR_Input_ActionFile_Action = (SteamVR_Input_ActionFile_Action)obj;
				if (this == obj)
				{
					return true;
				}
				if (name == steamVR_Input_ActionFile_Action.name && type == steamVR_Input_ActionFile_Action.type && skeleton == steamVR_Input_ActionFile_Action.skeleton && requirement == steamVR_Input_ActionFile_Action.requirement)
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
