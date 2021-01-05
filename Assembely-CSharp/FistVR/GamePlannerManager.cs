using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GamePlannerManager : MonoBehaviour
	{
		public delegate void GPEventSignal();

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

		public List<GPEventSignal>[] GPEventSignalDelegates = new List<GPEventSignal>[99];

		public bool CanSpawnSosigs()
		{
			return SosigManager != null;
		}

		public void FireEventSignal(int channel)
		{
			if (channel < 99 && channel >= 0 && GPEventSignalDelegates[channel] != null)
			{
				for (int i = 0; i < GPEventSignalDelegates[channel].Count; i++)
				{
					GPEventSignalDelegates[channel][i]();
				}
			}
		}

		private void InitEventSignalCollections()
		{
			for (int i = 0; i < GPEventSignalDelegates.Length; i++)
			{
				GPEventSignalDelegates[i] = new List<GPEventSignal>();
			}
		}

		private void FlushSignalCollections()
		{
			for (int i = 0; i < GPEventSignalDelegates.Length; i++)
			{
				GPEventSignalDelegates[i].Clear();
			}
		}

		public void AddReceiver(int channel, GPEventSignal del)
		{
			if (!GPEventSignalDelegates[channel].Contains(del))
			{
				GPEventSignalDelegates[channel].Add(del);
			}
		}

		public void RemoveReceiver(int channel, GPEventSignal del)
		{
			if (!GPEventSignalDelegates[channel].Contains(del))
			{
				GPEventSignalDelegates[channel].Remove(del);
			}
		}

		public void Init()
		{
			InitEventSignalCollections();
			m_currentScenarioDic = new Dictionary<int, GPPlaceable>();
			SMode = GPSceneMode.Design;
		}

		public void ToggleSceneMode()
		{
			if (SMode == GPSceneMode.Play)
			{
				SetSceneMode(GPSceneMode.Design);
			}
			if (SMode == GPSceneMode.Design)
			{
				SetSceneMode(GPSceneMode.Play);
			}
		}

		private void SetSceneMode(GPSceneMode m)
		{
			CleanSceneForLoad();
			SMode = m;
			if (m_currentScenario != string.Empty)
			{
				LoadScenario(m_currentScenario);
			}
		}

		public bool DeleteScenario(string scenarioName)
		{
			string errorMessage;
			return CynJson.Delete("Gameplanner", "Scenarios", levelName, scenarioName + "_Scenario.json", out errorMessage);
		}

		public void SaveActiveScenario(string scenarioName, string description)
		{
			GPScenario gPScenario = new GPScenario();
			gPScenario.levelname = levelName;
			gPScenario.versionNumber = 0;
			gPScenario.SavedPlaceables = new List<GPSavedPlaceable>();
			gPScenario.description = description;
			foreach (KeyValuePair<int, GPPlaceable> item in m_currentScenarioDic)
			{
				gPScenario.SavedPlaceables.Add(item.Value.GetSavedForm());
			}
			CynJson.Save("Gameplanner", "Scenarios", levelName, scenarioName + "_Scenario.json", gPScenario, out var _);
		}

		public string GetDescription(string scenarioName)
		{
			return string.Empty;
		}

		public bool LoadScenario(string scenarioName)
		{
			m_currentScenario = scenarioName;
			CleanSceneForLoad();
			FlushSignalCollections();
			GC.Collect();
			m_currentScenarioDic.Clear();
			GPScenario gPScenario = new GPScenario();
			CynJson.Load("Gameplanner", "Scenarios", levelName, scenarioName + "_Scenario.json", gPScenario, out var _);
			for (int i = 0; i < gPScenario.SavedPlaceables.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(IM.OD[gPScenario.SavedPlaceables[i].ObjectID].GetGameObject(), gPScenario.SavedPlaceables[i].Position, gPScenario.SavedPlaceables[i].Rotation);
				GPPlaceable component = gameObject.GetComponent<GPPlaceable>();
				component.ConfigureFromSavedPlaceable(gPScenario.SavedPlaceables[i]);
				m_nextUniqueID = Mathf.Max(component.UniqueID + 1, m_nextUniqueID);
				m_currentScenarioDic.Add(component.UniqueID, component);
			}
			if (SMode == GPSceneMode.Play)
			{
				InitializeCurrentScenarioInPlayMode();
			}
			else
			{
				InitializeCurrentScenarioInDesignMode();
			}
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
			p.Init(SMode);
			p.UniqueID = m_nextUniqueID;
			m_nextUniqueID++;
			m_currentScenarioDic.Add(p.UniqueID, p);
		}

		public void DeregisterAndDeletePlaceable(GPPlaceable p)
		{
			m_currentScenarioDic.Remove(p.UniqueID);
			UnityEngine.Object.Destroy(p.transform.root.gameObject);
		}

		public void ResetPlaceable(GPPlaceable p)
		{
			p.Reset();
		}

		private void CleanSceneForLoad()
		{
			foreach (KeyValuePair<int, GPPlaceable> item in m_currentScenarioDic)
			{
				UnityEngine.Object.Destroy(item.Value.gameObject);
			}
			m_currentScenarioDic.Clear();
		}

		public Vector3 GetSnappedGuidedPoint(Vector3 startPoint, Vector3 hitPoint, bool guides, bool increments)
		{
			return hitPoint;
		}

		public string[] GetFileListForCurrentScene()
		{
			return CynJson.GetFiles("Gameplanner", "Scenarios", levelName, "_Scenario.json");
		}
	}
}
