using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FistVR
{
	public class LM : ManagerSingleton<LM>
	{
		public GameObject entitySpawnerPrefab;

		private LevelManagerState m_state;

		private FVRLevelData m_currentLevel;

		private List<FVREntityDefinition> m_defs;

		private Dictionary<string, FVREntityDefinition> m_entitiesByID = new Dictionary<string, FVREntityDefinition>();

		private Dictionary<int, List<FVREntityDefinition>> m_entitiesByTagList = new Dictionary<int, List<FVREntityDefinition>>();

		private Dictionary<int, HashSet<FVREntityDefinition>> m_entitiesByTagHashset = new Dictionary<int, HashSet<FVREntityDefinition>>();

		public static GameObject EntitySpawnerPrefab => ManagerSingleton<LM>.Instance.entitySpawnerPrefab;

		public static LevelManagerState State
		{
			get
			{
				return ManagerSingleton<LM>.Instance.m_state;
			}
			set
			{
				ManagerSingleton<LM>.Instance.m_state = value;
			}
		}

		public static FVRLevelData CurrentLevel
		{
			get
			{
				return ManagerSingleton<LM>.Instance.m_currentLevel;
			}
			set
			{
				ManagerSingleton<LM>.Instance.m_currentLevel = value;
			}
		}

		public static List<FVREntityDefinition> Defs => ManagerSingleton<LM>.Instance.m_defs;

		public static Dictionary<string, FVREntityDefinition> EntitiesByID => ManagerSingleton<LM>.Instance.m_entitiesByID;

		public static Dictionary<int, List<FVREntityDefinition>> EntitiesByTagList => ManagerSingleton<LM>.Instance.m_entitiesByTagList;

		public static Dictionary<int, HashSet<FVREntityDefinition>> EntitiesByTagHashset => ManagerSingleton<LM>.Instance.m_entitiesByTagHashset;

		protected override void Awake()
		{
			base.Awake();
			PrimeEntityDictionaries();
		}

		public void OnLevelWasLoaded(int level)
		{
			if (m_currentLevel != null && SceneManager.GetActiveScene().name == m_currentLevel.SceneName)
			{
				switch (State)
				{
				case LevelManagerState.EditMode:
					BuildCurrentLevelForEdit();
					break;
				case LevelManagerState.PlayMode:
					BuildCurrentLevelForPlay();
					break;
				}
			}
		}

		public static void CreateNewLevelAndSetToCurrent(string ln, string sn)
		{
			ManagerSingleton<LM>.Instance.createNewLevelAndSetToCurrent(ln, sn);
		}

		private void createNewLevelAndSetToCurrent(string levelName, string sceneName)
		{
			FVRLevelData fVRLevelData2 = (CurrentLevel = new FVRLevelData(levelName, sceneName));
		}

		public static bool DoesLevelExist(string uniqueID)
		{
			return ManagerSingleton<LM>.Instance.doesLevelExist(uniqueID);
		}

		private bool doesLevelExist(string uniqueID)
		{
			string identifier = "Level_" + uniqueID + ".txt";
			if (ES2.Exists(identifier))
			{
				return true;
			}
			return false;
		}

		public static void SetCurrentLevelToFile(string fileName)
		{
			ManagerSingleton<LM>.Instance.setCurrentLevel(fileName);
		}

		private void setCurrentLevel(string fileName)
		{
			string text = string.Empty;
			using (ES2Reader eS2Reader = ES2Reader.Create(fileName))
			{
				if (eS2Reader.TagExists("levelData"))
				{
					text = eS2Reader.Read<string>("levelData");
				}
			}
			if (text != string.Empty)
			{
				m_currentLevel = FVRLevelData.Parse(text);
			}
		}

		public static void LoadCurrentLevelIntoEditMode()
		{
			ManagerSingleton<LM>.Instance.loadCurrentLevelIntoEditMode();
		}

		private void loadCurrentLevelIntoEditMode()
		{
			State = LevelManagerState.EditMode;
			SteamVR_LoadLevel.Begin(m_currentLevel.SceneName);
		}

		public static void LoadCurrentLevelIntoPlayMode()
		{
			ManagerSingleton<LM>.Instance.loadCurrentLevelIntoPlayMode();
		}

		private void loadCurrentLevelIntoPlayMode()
		{
			State = LevelManagerState.PlayMode;
			SteamVR_LoadLevel.Begin(m_currentLevel.SceneName);
		}

		public static void SaveCurrentLevel()
		{
			ManagerSingleton<LM>.Instance.saveCurrentLevel();
		}

		private void saveCurrentLevel()
		{
			if (CurrentLevel == null)
			{
				Debug.Log("THERE IS NO CURRENT LEVEL TO SAVE");
				return;
			}
			SaveEntityProxiesToCurrentLevel();
			string text = "Level_" + m_currentLevel.UniqueID + ".txt";
			string text2 = m_currentLevel.ToString();
			string levelName = m_currentLevel.LevelName;
			Debug.Log("Saving Scene:" + text + "with contents" + text2);
			using ES2Writer eS2Writer = ES2Writer.Create(text);
			eS2Writer.Write(text2, "levelData");
			eS2Writer.Write(levelName, "displayName");
			eS2Writer.Save();
		}

		private void SaveEntityProxiesToCurrentLevel()
		{
			if (m_currentLevel == null)
			{
				return;
			}
			m_currentLevel.ProxyDataList.Clear();
			FVREntityProxy[] array = UnityEngine.Object.FindObjectsOfType<FVREntityProxy>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Flags.IsSerialized)
				{
					array[i].SaveEntityProxyData();
					m_currentLevel.ProxyDataList.Add(array[i].Data);
				}
			}
		}

		private void BuildCurrentLevelForEdit()
		{
			if (m_currentLevel.ProxyDataList.Count > 0)
			{
				for (int i = 0; i < m_currentLevel.ProxyDataList.Count; i++)
				{
					FVREntityProxyData fVREntityProxyData = m_currentLevel.ProxyDataList[i];
					FVREntityDefinition fVREntityDefinition = m_entitiesByID[fVREntityProxyData.EntityID];
					GameObject gameObject = UnityEngine.Object.Instantiate(fVREntityDefinition.ProxyPrefab, Vector3.zero, Quaternion.identity);
					FVREntityProxy component = gameObject.GetComponent<FVREntityProxy>();
					component.DecodeFromProxyData(fVREntityProxyData);
					component.UpdateProxyState();
				}
			}
		}

		private void BuildCurrentLevelForPlay()
		{
			if (m_currentLevel.ProxyDataList.Count > 0)
			{
				for (int i = 0; i < m_currentLevel.ProxyDataList.Count; i++)
				{
					FVREntityProxyData fVREntityProxyData = m_currentLevel.ProxyDataList[i];
					FVREntityDefinition fVREntityDefinition = m_entitiesByID[fVREntityProxyData.EntityID];
					GameObject gameObject = UnityEngine.Object.Instantiate(fVREntityDefinition.MainPrefab, Vector3.zero, Quaternion.identity);
					IProxyConfigurable component = gameObject.GetComponent<IProxyConfigurable>();
					component.Configure(fVREntityProxyData);
				}
			}
		}

		public static int GetNumberOfDefsWithTag(FVREntityTag tag)
		{
			if (EntitiesByTagList.ContainsKey((int)tag))
			{
				return EntitiesByTagList[(int)tag].Count;
			}
			return 0;
		}

		public static List<FVREntityDefinition> GetDefsWithTags(List<FVREntityTag> tags)
		{
			if (tags.Count > 0)
			{
				List<FVREntityDefinition> list = EntitiesByTagList[(int)tags[0]];
				List<FVREntityDefinition> list2 = new List<FVREntityDefinition>();
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i]);
				}
				if (tags.Count > 1)
				{
					for (int num = list2.Count - 1; num >= 0; num--)
					{
						for (int j = 1; j < tags.Count; j++)
						{
							if (!list2[num].Tags.Contains(tags[j]))
							{
								list2.RemoveAt(num);
								break;
							}
						}
					}
				}
				return list2;
			}
			return Defs;
		}

		public static List<FVREntityTag> GetValidTags()
		{
			List<FVREntityTag> list = new List<FVREntityTag>((FVREntityTag[])Enum.GetValues(typeof(FVREntityTag)));
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (!EntitiesByTagHashset.ContainsKey((int)list[num]))
				{
					list.RemoveAt(num);
				}
			}
			return list;
		}

		public static List<FVREntityTag> GetValidIntersectingTags(List<FVREntityTag> existingTags)
		{
			HashSet<FVREntityTag> hashSet = new HashSet<FVREntityTag>();
			for (int i = 0; i < existingTags.Count; i++)
			{
				hashSet.Add(existingTags[i]);
			}
			List<FVREntityTag> list = new List<FVREntityTag>();
			List<FVREntityDefinition> defsWithTags = GetDefsWithTags(existingTags);
			for (int j = 0; j < defsWithTags.Count; j++)
			{
				FVREntityDefinition fVREntityDefinition = defsWithTags[j];
				for (int k = 0; k < fVREntityDefinition.Tags.Count; k++)
				{
					if (hashSet.Add(fVREntityDefinition.Tags[k]))
					{
						list.Add(fVREntityDefinition.Tags[k]);
					}
				}
			}
			return list;
		}

		private void PrimeEntityDictionaries()
		{
			m_defs = new List<FVREntityDefinition>(Resources.LoadAll<FVREntityDefinition>("FVREntities"));
			for (int i = 0; i < m_defs.Count; i++)
			{
				if (m_entitiesByID.ContainsKey(m_defs[i].EntityID))
				{
					Debug.Log("ENTITY ID COLLISION for " + m_defs[i].DisplayName + " and " + m_entitiesByID[m_defs[i].EntityID].DisplayName);
				}
				else
				{
					m_entitiesByID.Add(m_defs[i].EntityID, m_defs[i]);
				}
				for (int j = 0; j < m_defs[i].Tags.Count; j++)
				{
					if (!m_entitiesByTagList.ContainsKey((int)m_defs[i].Tags[j]))
					{
						List<FVREntityDefinition> value = new List<FVREntityDefinition>();
						m_entitiesByTagList.Add((int)m_defs[i].Tags[j], value);
						HashSet<FVREntityDefinition> value2 = new HashSet<FVREntityDefinition>();
						m_entitiesByTagHashset.Add((int)m_defs[i].Tags[j], value2);
					}
					m_entitiesByTagList[(int)m_defs[i].Tags[j]].Add(m_defs[i]);
					m_entitiesByTagHashset[(int)m_defs[i].Tags[j]].Add(m_defs[i]);
				}
			}
		}
	}
}
