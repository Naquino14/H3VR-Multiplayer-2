// Decompiled with JetBrains decompiler
// Type: FistVR.GamePlannerManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GamePlannerManager : MonoBehaviour
  {
    private const string rootFolderName = "Gameplanner";
    private const string catFolderName_Scenarios = "Scenarios";
    private const string suffix = "_Scenario.json";
    public string levelName = "TestScene";
    private const int versionNumber = 0;
    public GPSceneMode SMode;
    private string m_currentScenario = string.Empty;
    public Dictionary<int, GPPlaceable> m_currentScenarioDic = new Dictionary<int, GPPlaceable>();
    private int m_nextUniqueID;
    private bool m_isLoadSaveRoutineGoing;
    public GPSosigManager SosigManager;
    public List<GamePlannerManager.GPEventSignal>[] GPEventSignalDelegates = new List<GamePlannerManager.GPEventSignal>[99];

    public bool CanSpawnSosigs() => (UnityEngine.Object) this.SosigManager != (UnityEngine.Object) null;

    public void FireEventSignal(int channel)
    {
      if (channel >= 99 || channel < 0 || this.GPEventSignalDelegates[channel] == null)
        return;
      for (int index = 0; index < this.GPEventSignalDelegates[channel].Count; ++index)
        this.GPEventSignalDelegates[channel][index]();
    }

    private void InitEventSignalCollections()
    {
      for (int index = 0; index < this.GPEventSignalDelegates.Length; ++index)
        this.GPEventSignalDelegates[index] = new List<GamePlannerManager.GPEventSignal>();
    }

    private void FlushSignalCollections()
    {
      for (int index = 0; index < this.GPEventSignalDelegates.Length; ++index)
        this.GPEventSignalDelegates[index].Clear();
    }

    public void AddReceiver(int channel, GamePlannerManager.GPEventSignal del)
    {
      if (this.GPEventSignalDelegates[channel].Contains(del))
        return;
      this.GPEventSignalDelegates[channel].Add(del);
    }

    public void RemoveReceiver(int channel, GamePlannerManager.GPEventSignal del)
    {
      if (this.GPEventSignalDelegates[channel].Contains(del))
        return;
      this.GPEventSignalDelegates[channel].Remove(del);
    }

    public void Init()
    {
      this.InitEventSignalCollections();
      this.m_currentScenarioDic = new Dictionary<int, GPPlaceable>();
      this.SMode = GPSceneMode.Design;
    }

    public void ToggleSceneMode()
    {
      if (this.SMode == GPSceneMode.Play)
        this.SetSceneMode(GPSceneMode.Design);
      if (this.SMode != GPSceneMode.Design)
        return;
      this.SetSceneMode(GPSceneMode.Play);
    }

    private void SetSceneMode(GPSceneMode m)
    {
      this.CleanSceneForLoad();
      this.SMode = m;
      if (!(this.m_currentScenario != string.Empty))
        return;
      this.LoadScenario(this.m_currentScenario);
    }

    public bool DeleteScenario(string scenarioName) => CynJson.Delete("Gameplanner", "Scenarios", this.levelName, scenarioName + "_Scenario.json", out string _);

    public void SaveActiveScenario(string scenarioName, string description)
    {
      GPScenario gpScenario = new GPScenario();
      gpScenario.levelname = this.levelName;
      gpScenario.versionNumber = 0;
      gpScenario.SavedPlaceables = new List<GPSavedPlaceable>();
      gpScenario.description = description;
      foreach (KeyValuePair<int, GPPlaceable> keyValuePair in this.m_currentScenarioDic)
        gpScenario.SavedPlaceables.Add(keyValuePair.Value.GetSavedForm());
      CynJson.Save("Gameplanner", "Scenarios", this.levelName, scenarioName + "_Scenario.json", (object) gpScenario, out string _);
    }

    public string GetDescription(string scenarioName) => string.Empty;

    public bool LoadScenario(string scenarioName)
    {
      this.m_currentScenario = scenarioName;
      this.CleanSceneForLoad();
      this.FlushSignalCollections();
      GC.Collect();
      this.m_currentScenarioDic.Clear();
      GPScenario objectToOverwrite = new GPScenario();
      CynJson.Load<GPScenario>("Gameplanner", "Scenarios", this.levelName, scenarioName + "_Scenario.json", objectToOverwrite, out string _);
      for (int index = 0; index < objectToOverwrite.SavedPlaceables.Count; ++index)
      {
        GPPlaceable component = UnityEngine.Object.Instantiate<GameObject>(IM.OD[objectToOverwrite.SavedPlaceables[index].ObjectID].GetGameObject(), objectToOverwrite.SavedPlaceables[index].Position, objectToOverwrite.SavedPlaceables[index].Rotation).GetComponent<GPPlaceable>();
        component.ConfigureFromSavedPlaceable(objectToOverwrite.SavedPlaceables[index]);
        this.m_nextUniqueID = Mathf.Max(component.UniqueID + 1, this.m_nextUniqueID);
        this.m_currentScenarioDic.Add(component.UniqueID, component);
      }
      if (this.SMode == GPSceneMode.Play)
        this.InitializeCurrentScenarioInPlayMode();
      else
        this.InitializeCurrentScenarioInDesignMode();
      return true;
    }

    public void InitializeCurrentScenarioInPlayMode()
    {
    }

    public void InitializeCurrentScenarioInDesignMode()
    {
    }

    public void RegisterAndInitSpawnedPlaceable(GPPlaceable p)
    {
      p.Init(this.SMode);
      p.UniqueID = this.m_nextUniqueID;
      ++this.m_nextUniqueID;
      this.m_currentScenarioDic.Add(p.UniqueID, p);
    }

    public void DeregisterAndDeletePlaceable(GPPlaceable p)
    {
      this.m_currentScenarioDic.Remove(p.UniqueID);
      UnityEngine.Object.Destroy((UnityEngine.Object) p.transform.root.gameObject);
    }

    public void ResetPlaceable(GPPlaceable p) => p.Reset();

    private void CleanSceneForLoad()
    {
      foreach (KeyValuePair<int, GPPlaceable> keyValuePair in this.m_currentScenarioDic)
        UnityEngine.Object.Destroy((UnityEngine.Object) keyValuePair.Value.gameObject);
      this.m_currentScenarioDic.Clear();
    }

    public Vector3 GetSnappedGuidedPoint(
      Vector3 startPoint,
      Vector3 hitPoint,
      bool guides,
      bool increments)
    {
      return hitPoint;
    }

    public string[] GetFileListForCurrentScene() => CynJson.GetFiles("Gameplanner", "Scenarios", this.levelName, "_Scenario.json");

    public delegate void GPEventSignal();
  }
}
