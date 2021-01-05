using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class FVRLevelData
	{
		public string UniqueID = "unassigned";

		public string LevelName = "unnamed";

		public string SceneName = "unassigned";

		public string SaveSystemVersion = "v0.1";

		public string Description = "new user level";

		public List<FVREntityProxyData> ProxyDataList = new List<FVREntityProxyData>();

		public FVRLevelData(string levelName, string sceneName)
		{
			UniqueID = Guid.NewGuid().ToString();
			LevelName = levelName;
			SceneName = sceneName;
		}

		private bool IsUniqueIDValid()
		{
			if (UniqueID != "unassigned")
			{
				return true;
			}
			return false;
		}

		private bool IsLevelNameValid()
		{
			if (LevelName != string.Empty && LevelName != "unnamed")
			{
				return true;
			}
			return false;
		}

		private bool IsSceneNameValid()
		{
			if (SceneName != string.Empty && SceneName != "unassigned")
			{
				return true;
			}
			return false;
		}

		public static FVRLevelData Parse(string s)
		{
			return JsonUtility.FromJson<FVRLevelData>(s);
		}

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}
	}
}
