// Decompiled with JetBrains decompiler
// Type: FistVR.LM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      get => ManagerSingleton<LM>.Instance.m_state;
      set => ManagerSingleton<LM>.Instance.m_state = value;
    }

    public static FVRLevelData CurrentLevel
    {
      get => ManagerSingleton<LM>.Instance.m_currentLevel;
      set => ManagerSingleton<LM>.Instance.m_currentLevel = value;
    }

    protected override void Awake()
    {
      base.Awake();
      this.PrimeEntityDictionaries();
    }

    public void OnLevelWasLoaded(int level)
    {
      if (this.m_currentLevel == null || !(SceneManager.GetActiveScene().name == this.m_currentLevel.SceneName))
        return;
      switch (LM.State)
      {
        case LevelManagerState.EditMode:
          this.BuildCurrentLevelForEdit();
          break;
        case LevelManagerState.PlayMode:
          this.BuildCurrentLevelForPlay();
          break;
      }
    }

    public static void CreateNewLevelAndSetToCurrent(string ln, string sn) => ManagerSingleton<LM>.Instance.createNewLevelAndSetToCurrent(ln, sn);

    private void createNewLevelAndSetToCurrent(string levelName, string sceneName) => LM.CurrentLevel = new FVRLevelData(levelName, sceneName);

    public static bool DoesLevelExist(string uniqueID) => ManagerSingleton<LM>.Instance.doesLevelExist(uniqueID);

    private bool doesLevelExist(string uniqueID) => ES2.Exists("Level_" + uniqueID + ".txt");

    public static void SetCurrentLevelToFile(string fileName) => ManagerSingleton<LM>.Instance.setCurrentLevel(fileName);

    private void setCurrentLevel(string fileName)
    {
      string s = string.Empty;
      using (ES2Reader es2Reader = ES2Reader.Create(fileName))
      {
        if (es2Reader.TagExists("levelData"))
          s = es2Reader.Read<string>("levelData");
      }
      if (!(s != string.Empty))
        return;
      this.m_currentLevel = FVRLevelData.Parse(s);
    }

    public static void LoadCurrentLevelIntoEditMode() => ManagerSingleton<LM>.Instance.loadCurrentLevelIntoEditMode();

    private void loadCurrentLevelIntoEditMode()
    {
      LM.State = LevelManagerState.EditMode;
      SteamVR_LoadLevel.Begin(this.m_currentLevel.SceneName);
    }

    public static void LoadCurrentLevelIntoPlayMode() => ManagerSingleton<LM>.Instance.loadCurrentLevelIntoPlayMode();

    private void loadCurrentLevelIntoPlayMode()
    {
      LM.State = LevelManagerState.PlayMode;
      SteamVR_LoadLevel.Begin(this.m_currentLevel.SceneName);
    }

    public static void SaveCurrentLevel() => ManagerSingleton<LM>.Instance.saveCurrentLevel();

    private void saveCurrentLevel()
    {
      if (LM.CurrentLevel == null)
      {
        Debug.Log((object) "THERE IS NO CURRENT LEVEL TO SAVE");
      }
      else
      {
        this.SaveEntityProxiesToCurrentLevel();
        string identifier = "Level_" + this.m_currentLevel.UniqueID + ".txt";
        string str = this.m_currentLevel.ToString();
        string levelName = this.m_currentLevel.LevelName;
        Debug.Log((object) ("Saving Scene:" + identifier + "with contents" + str));
        using (ES2Writer es2Writer = ES2Writer.Create(identifier))
        {
          es2Writer.Write<string>(str, "levelData");
          es2Writer.Write<string>(levelName, "displayName");
          es2Writer.Save();
        }
      }
    }

    private void SaveEntityProxiesToCurrentLevel()
    {
      if (this.m_currentLevel == null)
        return;
      this.m_currentLevel.ProxyDataList.Clear();
      FVREntityProxy[] objectsOfType = UnityEngine.Object.FindObjectsOfType<FVREntityProxy>();
      for (int index = 0; index < objectsOfType.Length; ++index)
      {
        if (objectsOfType[index].Flags.IsSerialized)
        {
          objectsOfType[index].SaveEntityProxyData();
          this.m_currentLevel.ProxyDataList.Add(objectsOfType[index].Data);
        }
      }
    }

    private void BuildCurrentLevelForEdit()
    {
      if (this.m_currentLevel.ProxyDataList.Count <= 0)
        return;
      for (int index = 0; index < this.m_currentLevel.ProxyDataList.Count; ++index)
      {
        FVREntityProxyData proxyData = this.m_currentLevel.ProxyDataList[index];
        FVREntityProxy component = UnityEngine.Object.Instantiate<GameObject>(this.m_entitiesByID[proxyData.EntityID].ProxyPrefab, Vector3.zero, Quaternion.identity).GetComponent<FVREntityProxy>();
        component.DecodeFromProxyData(proxyData);
        component.UpdateProxyState();
      }
    }

    private void BuildCurrentLevelForPlay()
    {
      if (this.m_currentLevel.ProxyDataList.Count <= 0)
        return;
      for (int index = 0; index < this.m_currentLevel.ProxyDataList.Count; ++index)
      {
        FVREntityProxyData proxyData = this.m_currentLevel.ProxyDataList[index];
        UnityEngine.Object.Instantiate<GameObject>(this.m_entitiesByID[proxyData.EntityID].MainPrefab, Vector3.zero, Quaternion.identity).GetComponent<IProxyConfigurable>().Configure(proxyData);
      }
    }

    public static List<FVREntityDefinition> Defs => ManagerSingleton<LM>.Instance.m_defs;

    public static Dictionary<string, FVREntityDefinition> EntitiesByID => ManagerSingleton<LM>.Instance.m_entitiesByID;

    public static Dictionary<int, List<FVREntityDefinition>> EntitiesByTagList => ManagerSingleton<LM>.Instance.m_entitiesByTagList;

    public static Dictionary<int, HashSet<FVREntityDefinition>> EntitiesByTagHashset => ManagerSingleton<LM>.Instance.m_entitiesByTagHashset;

    public static int GetNumberOfDefsWithTag(FVREntityTag tag) => LM.EntitiesByTagList.ContainsKey((int) tag) ? LM.EntitiesByTagList[(int) tag].Count : 0;

    public static List<FVREntityDefinition> GetDefsWithTags(
      List<FVREntityTag> tags)
    {
      if (tags.Count <= 0)
        return LM.Defs;
      List<FVREntityDefinition> entitiesByTag = LM.EntitiesByTagList[(int) tags[0]];
      List<FVREntityDefinition> entityDefinitionList = new List<FVREntityDefinition>();
      for (int index = 0; index < entitiesByTag.Count; ++index)
        entityDefinitionList.Add(entitiesByTag[index]);
      if (tags.Count > 1)
      {
        for (int index1 = entityDefinitionList.Count - 1; index1 >= 0; --index1)
        {
          for (int index2 = 1; index2 < tags.Count; ++index2)
          {
            if (!entityDefinitionList[index1].Tags.Contains(tags[index2]))
            {
              entityDefinitionList.RemoveAt(index1);
              break;
            }
          }
        }
      }
      return entityDefinitionList;
    }

    public static List<FVREntityTag> GetValidTags()
    {
      List<FVREntityTag> fvrEntityTagList = new List<FVREntityTag>((IEnumerable<FVREntityTag>) Enum.GetValues(typeof (FVREntityTag)));
      for (int index = fvrEntityTagList.Count - 1; index >= 0; --index)
      {
        if (!LM.EntitiesByTagHashset.ContainsKey((int) fvrEntityTagList[index]))
          fvrEntityTagList.RemoveAt(index);
      }
      return fvrEntityTagList;
    }

    public static List<FVREntityTag> GetValidIntersectingTags(
      List<FVREntityTag> existingTags)
    {
      HashSet<FVREntityTag> fvrEntityTagSet = new HashSet<FVREntityTag>();
      for (int index = 0; index < existingTags.Count; ++index)
        fvrEntityTagSet.Add(existingTags[index]);
      List<FVREntityTag> fvrEntityTagList = new List<FVREntityTag>();
      List<FVREntityDefinition> defsWithTags = LM.GetDefsWithTags(existingTags);
      for (int index1 = 0; index1 < defsWithTags.Count; ++index1)
      {
        FVREntityDefinition entityDefinition = defsWithTags[index1];
        for (int index2 = 0; index2 < entityDefinition.Tags.Count; ++index2)
        {
          if (fvrEntityTagSet.Add(entityDefinition.Tags[index2]))
            fvrEntityTagList.Add(entityDefinition.Tags[index2]);
        }
      }
      return fvrEntityTagList;
    }

    private void PrimeEntityDictionaries()
    {
      this.m_defs = new List<FVREntityDefinition>((IEnumerable<FVREntityDefinition>) Resources.LoadAll<FVREntityDefinition>("FVREntities"));
      for (int index1 = 0; index1 < this.m_defs.Count; ++index1)
      {
        if (this.m_entitiesByID.ContainsKey(this.m_defs[index1].EntityID))
          Debug.Log((object) ("ENTITY ID COLLISION for " + this.m_defs[index1].DisplayName + " and " + this.m_entitiesByID[this.m_defs[index1].EntityID].DisplayName));
        else
          this.m_entitiesByID.Add(this.m_defs[index1].EntityID, this.m_defs[index1]);
        for (int index2 = 0; index2 < this.m_defs[index1].Tags.Count; ++index2)
        {
          if (!this.m_entitiesByTagList.ContainsKey((int) this.m_defs[index1].Tags[index2]))
          {
            List<FVREntityDefinition> entityDefinitionList = new List<FVREntityDefinition>();
            this.m_entitiesByTagList.Add((int) this.m_defs[index1].Tags[index2], entityDefinitionList);
            HashSet<FVREntityDefinition> entityDefinitionSet = new HashSet<FVREntityDefinition>();
            this.m_entitiesByTagHashset.Add((int) this.m_defs[index1].Tags[index2], entityDefinitionSet);
          }
          this.m_entitiesByTagList[(int) this.m_defs[index1].Tags[index2]].Add(this.m_defs[index1]);
          this.m_entitiesByTagHashset[(int) this.m_defs[index1].Tags[index2]].Add(this.m_defs[index1]);
        }
      }
    }
  }
}
