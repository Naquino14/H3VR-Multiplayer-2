using System;
using System.Linq;

namespace Valve.VR
{
	public class SteamVR_Skeleton_FingerExtensionTypeLists
	{
		private SteamVR_Skeleton_FingerExtensionTypes[] _enumList;

		private string[] _stringList;

		public SteamVR_Skeleton_FingerExtensionTypes[] enumList
		{
			get
			{
				if (_enumList == null)
				{
					_enumList = (SteamVR_Skeleton_FingerExtensionTypes[])Enum.GetValues(typeof(SteamVR_Skeleton_FingerExtensionTypes));
				}
				return _enumList;
			}
		}

		public string[] stringList
		{
			get
			{
				if (_stringList == null)
				{
					_stringList = enumList.Select((SteamVR_Skeleton_FingerExtensionTypes element) => element.ToString()).ToArray();
				}
				return _stringList;
			}
		}
	}
}
