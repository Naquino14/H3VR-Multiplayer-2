using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	public class SteamVR_Input
	{
		public delegate void PosesUpdatedHandler(bool skipSendingEvents);

		public delegate void SkeletonsUpdatedHandler(bool skipSendingEvents);

		public const string defaultInputGameObjectName = "[SteamVR Input]";

		private const string localizationKeyName = "localization";

		public static string actionsFilePath;

		public static bool fileInitialized;

		public static bool initialized;

		public static bool preInitialized;

		public static SteamVR_Input_ActionFile actionFile;

		public static string actionFileHash;

		protected static bool initializing;

		protected static int startupFrame;

		public static SteamVR_ActionSet[] actionSets;

		public static SteamVR_Action[] actions;

		public static ISteamVR_Action_In[] actionsIn;

		public static ISteamVR_Action_Out[] actionsOut;

		public static SteamVR_Action_Boolean[] actionsBoolean;

		public static SteamVR_Action_Single[] actionsSingle;

		public static SteamVR_Action_Vector2[] actionsVector2;

		public static SteamVR_Action_Vector3[] actionsVector3;

		public static SteamVR_Action_Pose[] actionsPose;

		public static SteamVR_Action_Skeleton[] actionsSkeleton;

		public static SteamVR_Action_Vibration[] actionsVibration;

		public static ISteamVR_Action_In[] actionsNonPoseNonSkeletonIn;

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPath;

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathLowered;

		protected static Dictionary<string, SteamVR_Action> actionsByPath;

		protected static Dictionary<string, SteamVR_Action> actionsByPathLowered;

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathCache;

		protected static Dictionary<string, SteamVR_Action> actionsByPathCache;

		protected static Dictionary<string, SteamVR_Action> actionsByNameCache;

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByNameCache;

		public static bool isStartupFrame => Time.frameCount >= startupFrame - 1 && Time.frameCount <= startupFrame + 1;

		public static event Action onNonVisualActionsUpdated;

		public static event PosesUpdatedHandler onPosesUpdated;

		public static event SkeletonsUpdatedHandler onSkeletonsUpdated;

		static SteamVR_Input()
		{
			fileInitialized = false;
			initialized = false;
			preInitialized = false;
			initializing = false;
			startupFrame = 0;
			actionSetsByPath = new Dictionary<string, SteamVR_ActionSet>();
			actionSetsByPathLowered = new Dictionary<string, SteamVR_ActionSet>();
			actionsByPath = new Dictionary<string, SteamVR_Action>();
			actionsByPathLowered = new Dictionary<string, SteamVR_Action>();
			actionSetsByPathCache = new Dictionary<string, SteamVR_ActionSet>();
			actionsByPathCache = new Dictionary<string, SteamVR_Action>();
			actionsByNameCache = new Dictionary<string, SteamVR_Action>();
			actionSetsByNameCache = new Dictionary<string, SteamVR_ActionSet>();
			FindPreinitializeMethod();
		}

		public static void ForcePreinitialize()
		{
			FindPreinitializeMethod();
		}

		private static void FindPreinitializeMethod()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type type = assembly.GetType("Valve.VR.SteamVR_Actions");
				if (type != null)
				{
					MethodInfo method = type.GetMethod("PreInitialize");
					if (method != null)
					{
						method.Invoke(null, null);
						break;
					}
				}
			}
		}

		public static void Initialize(bool force = false)
		{
			if (initialized && !force)
			{
				return;
			}
			initializing = true;
			startupFrame = Time.frameCount;
			SteamVR_ActionSet_Manager.Initialize();
			SteamVR_Input_Source.Initialize();
			for (int i = 0; i < actions.Length; i++)
			{
				SteamVR_Action steamVR_Action = actions[i];
				steamVR_Action.Initialize(createNew: true);
			}
			for (int j = 0; j < actionSets.Length; j++)
			{
				SteamVR_ActionSet steamVR_ActionSet = actionSets[j];
				steamVR_ActionSet.Initialize(createNew: true);
			}
			if (SteamVR_Settings.instance.activateFirstActionSetOnStart)
			{
				if (actionSets.Length > 0)
				{
					actionSets[0].Activate();
				}
				else
				{
					Debug.LogError("<b>[SteamVR]</b> No action sets to activate.");
				}
			}
			SteamVR_Action_Pose.SetTrackingUniverseOrigin(SteamVR_Settings.instance.trackingSpace);
			initialized = true;
			initializing = false;
		}

		public static void PreinitializeFinishActionSets()
		{
			for (int i = 0; i < actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = actionSets[i];
				steamVR_ActionSet.FinishPreInitialize();
			}
		}

		public static void PreinitializeActionSetDictionaries()
		{
			actionSetsByPath.Clear();
			actionSetsByPathLowered.Clear();
			actionSetsByPathCache.Clear();
			for (int i = 0; i < actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = actionSets[i];
				actionSetsByPath.Add(steamVR_ActionSet.fullPath, steamVR_ActionSet);
				actionSetsByPathLowered.Add(steamVR_ActionSet.fullPath.ToLower(), steamVR_ActionSet);
			}
		}

		public static void PreinitializeActionDictionaries()
		{
			actionsByPath.Clear();
			actionsByPathLowered.Clear();
			actionsByPathCache.Clear();
			for (int i = 0; i < actions.Length; i++)
			{
				SteamVR_Action steamVR_Action = actions[i];
				actionsByPath.Add(steamVR_Action.fullPath, steamVR_Action);
				actionsByPathLowered.Add(steamVR_Action.fullPath.ToLower(), steamVR_Action);
			}
		}

		public static void Update()
		{
			if (initialized && !isStartupFrame)
			{
				if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnUpdate))
				{
					UpdateNonVisualActions();
				}
				if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnUpdate))
				{
					UpdateVisualActions();
				}
			}
		}

		public static void LateUpdate()
		{
			if (initialized && !isStartupFrame)
			{
				if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
				{
					UpdateNonVisualActions();
				}
				if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
				{
					UpdateVisualActions();
				}
				else
				{
					UpdateSkeletonActions(skipSendingEvents: true);
				}
			}
		}

		public static void FixedUpdate()
		{
			if (initialized && !isStartupFrame)
			{
				if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
				{
					UpdateNonVisualActions();
				}
				if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
				{
					UpdateVisualActions();
				}
			}
		}

		public static void OnPreCull()
		{
			if (initialized && !isStartupFrame)
			{
				if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnPreCull))
				{
					UpdateNonVisualActions();
				}
				if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
				{
					UpdateVisualActions();
				}
			}
		}

		public static void UpdateVisualActions(bool skipStateAndEventUpdates = false)
		{
			if (initialized)
			{
				SteamVR_ActionSet_Manager.UpdateActionStates();
				UpdatePoseActions(skipStateAndEventUpdates);
				UpdateSkeletonActions(skipStateAndEventUpdates);
			}
		}

		public static void UpdatePoseActions(bool skipSendingEvents = false)
		{
			if (initialized)
			{
				for (int i = 0; i < actionsPose.Length; i++)
				{
					SteamVR_Action_Pose steamVR_Action_Pose = actionsPose[i];
					steamVR_Action_Pose.UpdateValues(skipSendingEvents);
				}
				if (SteamVR_Input.onPosesUpdated != null)
				{
					SteamVR_Input.onPosesUpdated(skipSendingEvents: false);
				}
			}
		}

		public static void UpdateSkeletonActions(bool skipSendingEvents = false)
		{
			if (initialized)
			{
				for (int i = 0; i < actionsSkeleton.Length; i++)
				{
					SteamVR_Action_Skeleton steamVR_Action_Skeleton = actionsSkeleton[i];
					steamVR_Action_Skeleton.UpdateValue(skipSendingEvents);
				}
				if (SteamVR_Input.onSkeletonsUpdated != null)
				{
					SteamVR_Input.onSkeletonsUpdated(skipSendingEvents);
				}
			}
		}

		public static void UpdateNonVisualActions()
		{
			if (initialized)
			{
				SteamVR_ActionSet_Manager.UpdateActionStates();
				for (int i = 0; i < actionsNonPoseNonSkeletonIn.Length; i++)
				{
					ISteamVR_Action_In steamVR_Action_In = actionsNonPoseNonSkeletonIn[i];
					steamVR_Action_In.UpdateValues();
				}
				if (SteamVR_Input.onNonVisualActionsUpdated != null)
				{
					SteamVR_Input.onNonVisualActionsUpdated();
				}
			}
		}

		public static T GetActionDataFromPath<T>(string path, bool caseSensitive = false) where T : SteamVR_Action_Source_Map
		{
			SteamVR_Action baseActionFromPath = GetBaseActionFromPath(path, caseSensitive);
			if (baseActionFromPath != null)
			{
				return (T)baseActionFromPath.GetSourceMap();
			}
			return (T)null;
		}

		public static SteamVR_ActionSet_Data GetActionSetDataFromPath(string path, bool caseSensitive = false)
		{
			SteamVR_ActionSet actionSetFromPath = GetActionSetFromPath(path, caseSensitive);
			if (actionSetFromPath != null)
			{
				return actionSetFromPath.GetActionSetData();
			}
			return null;
		}

		public static T GetActionFromPath<T>(string path, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_Action, new()
		{
			SteamVR_Action baseActionFromPath = GetBaseActionFromPath(path, caseSensitive);
			if (baseActionFromPath != null)
			{
				return baseActionFromPath.GetCopy<T>();
			}
			if (returnNulls)
			{
				return (T)null;
			}
			return CreateFakeAction<T>(path, caseSensitive);
		}

		public static SteamVR_Action GetBaseActionFromPath(string path, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			if (caseSensitive)
			{
				if (actionsByPath.ContainsKey(path))
				{
					return actionsByPath[path];
				}
			}
			else
			{
				if (actionsByPathCache.ContainsKey(path))
				{
					return actionsByPathCache[path];
				}
				if (actionsByPath.ContainsKey(path))
				{
					actionsByPathCache.Add(path, actionsByPath[path]);
					return actionsByPath[path];
				}
				string key = path.ToLower();
				if (actionsByPathLowered.ContainsKey(key))
				{
					actionsByPathCache.Add(path, actionsByPathLowered[key]);
					return actionsByPath[key];
				}
				actionsByPathCache.Add(path, null);
			}
			return null;
		}

		public static bool HasActionPath(string path, bool caseSensitive = false)
		{
			SteamVR_Action baseActionFromPath = GetBaseActionFromPath(path, caseSensitive);
			return baseActionFromPath != null;
		}

		public static bool HasAction(string actionName, bool caseSensitive = false)
		{
			SteamVR_Action baseAction = GetBaseAction(null, actionName, caseSensitive);
			return baseAction != null;
		}

		public static bool HasAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			SteamVR_Action baseAction = GetBaseAction(actionSetName, actionName, caseSensitive);
			return baseAction != null;
		}

		public static SteamVR_Action_Boolean GetBooleanActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Boolean>(path, caseSensitive);
		}

		public static SteamVR_Action_Single GetSingleActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Single>(path, caseSensitive);
		}

		public static SteamVR_Action_Vector2 GetVector2ActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Vector2>(path, caseSensitive);
		}

		public static SteamVR_Action_Vector3 GetVector3ActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Vector3>(path, caseSensitive);
		}

		public static SteamVR_Action_Vibration GetVibrationActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Vibration>(path, caseSensitive);
		}

		public static SteamVR_Action_Pose GetPoseActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Pose>(path, caseSensitive);
		}

		public static SteamVR_Action_Skeleton GetSkeletonActionFromPath(string path, bool caseSensitive = false)
		{
			return GetActionFromPath<SteamVR_Action_Skeleton>(path, caseSensitive);
		}

		public static T GetAction<T>(string actionSetName, string actionName, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_Action, new()
		{
			SteamVR_Action baseAction = GetBaseAction(actionSetName, actionName, caseSensitive);
			if (baseAction != null)
			{
				return baseAction.GetCopy<T>();
			}
			if (returnNulls)
			{
				return (T)null;
			}
			return CreateFakeAction<T>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action GetBaseAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			if (actions == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(actionSetName))
			{
				for (int i = 0; i < actions.Length; i++)
				{
					if (caseSensitive)
					{
						if (actions[i].GetShortName() == actionName)
						{
							return actions[i];
						}
					}
					else if (string.Equals(actions[i].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
					{
						return actions[i];
					}
				}
			}
			else
			{
				SteamVR_ActionSet actionSet = GetActionSet(actionSetName, caseSensitive, returnsNulls: true);
				if (actionSet != null)
				{
					for (int j = 0; j < actionSet.allActions.Length; j++)
					{
						if (caseSensitive)
						{
							if (actionSet.allActions[j].GetShortName() == actionName)
							{
								return actionSet.allActions[j];
							}
						}
						else if (string.Equals(actionSet.allActions[j].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
						{
							return actionSet.allActions[j];
						}
					}
				}
			}
			return null;
		}

		private static T CreateFakeAction<T>(string actionSetName, string actionName, bool caseSensitive) where T : SteamVR_Action, new()
		{
			if (typeof(T) == typeof(SteamVR_Action_Vibration))
			{
				return SteamVR_Action.CreateUninitialized<T>(actionSetName, SteamVR_ActionDirections.Out, actionName, caseSensitive);
			}
			return SteamVR_Action.CreateUninitialized<T>(actionSetName, SteamVR_ActionDirections.In, actionName, caseSensitive);
		}

		private static T CreateFakeAction<T>(string actionPath, bool caseSensitive) where T : SteamVR_Action, new()
		{
			return SteamVR_Action.CreateUninitialized<T>(actionPath, caseSensitive);
		}

		public static T GetAction<T>(string actionName, bool caseSensitive = false) where T : SteamVR_Action, new()
		{
			return GetAction<T>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Boolean GetBooleanAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Boolean>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Boolean GetBooleanAction(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Boolean>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Single GetSingleAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Single>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Single GetSingleAction(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Single>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Vector2 GetVector2Action(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Vector2>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Vector2 GetVector2Action(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Vector2>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Vector3 GetVector3Action(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Vector3>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Vector3 GetVector3Action(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Vector3>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Pose GetPoseAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Pose>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Pose GetPoseAction(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Pose>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Skeleton GetSkeletonAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Skeleton>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Skeleton GetSkeletonAction(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Skeleton>(null, actionName, caseSensitive);
		}

		public static SteamVR_Action_Vibration GetVibrationAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Vibration>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action_Vibration GetVibrationAction(string actionName, bool caseSensitive = false)
		{
			return GetAction<SteamVR_Action_Vibration>(null, actionName, caseSensitive);
		}

		public static T GetActionSet<T>(string actionSetName, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_ActionSet, new()
		{
			if (actionSets == null)
			{
				if (returnNulls)
				{
					return (T)null;
				}
				return SteamVR_ActionSet.CreateFromName<T>(actionSetName);
			}
			for (int i = 0; i < actionSets.Length; i++)
			{
				if (caseSensitive)
				{
					if (actionSets[i].GetShortName() == actionSetName)
					{
						return actionSets[i].GetCopy<T>();
					}
				}
				else if (string.Equals(actionSets[i].GetShortName(), actionSetName, StringComparison.CurrentCultureIgnoreCase))
				{
					return actionSets[i].GetCopy<T>();
				}
			}
			if (returnNulls)
			{
				return (T)null;
			}
			return SteamVR_ActionSet.CreateFromName<T>(actionSetName);
		}

		public static SteamVR_ActionSet GetActionSet(string actionSetName, bool caseSensitive = false, bool returnsNulls = false)
		{
			return GetActionSet<SteamVR_ActionSet>(actionSetName, caseSensitive, returnsNulls);
		}

		protected static bool HasActionSet(string name, bool caseSensitive = false)
		{
			SteamVR_ActionSet actionSet = GetActionSet(name, caseSensitive, returnsNulls: true);
			return actionSet != null;
		}

		public static T GetActionSetFromPath<T>(string path, bool caseSensitive = false, bool returnsNulls = false) where T : SteamVR_ActionSet, new()
		{
			if (actionSets == null || actionSets[0] == null || string.IsNullOrEmpty(path))
			{
				if (returnsNulls)
				{
					return (T)null;
				}
				return SteamVR_ActionSet.Create<T>(path);
			}
			if (caseSensitive)
			{
				if (actionSetsByPath.ContainsKey(path))
				{
					return actionSetsByPath[path].GetCopy<T>();
				}
			}
			else
			{
				if (actionSetsByPathCache.ContainsKey(path))
				{
					SteamVR_ActionSet steamVR_ActionSet = actionSetsByPathCache[path];
					if (steamVR_ActionSet == null)
					{
						return (T)null;
					}
					return steamVR_ActionSet.GetCopy<T>();
				}
				if (actionSetsByPath.ContainsKey(path))
				{
					actionSetsByPathCache.Add(path, actionSetsByPath[path]);
					return actionSetsByPath[path].GetCopy<T>();
				}
				string key = path.ToLower();
				if (actionSetsByPathLowered.ContainsKey(key))
				{
					actionSetsByPathCache.Add(path, actionSetsByPathLowered[key]);
					return actionSetsByPath[key].GetCopy<T>();
				}
				actionSetsByPathCache.Add(path, null);
			}
			if (returnsNulls)
			{
				return (T)null;
			}
			return SteamVR_ActionSet.Create<T>(path);
		}

		public static SteamVR_ActionSet GetActionSetFromPath(string path, bool caseSensitive = false)
		{
			return GetActionSetFromPath<SteamVR_ActionSet>(path, caseSensitive);
		}

		public static bool GetState(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Boolean action2 = GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetState(inputSource);
			}
			return false;
		}

		public static bool GetState(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetState(null, action, inputSource, caseSensitive);
		}

		public static bool GetStateDown(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Boolean action2 = GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetStateDown(inputSource);
			}
			return false;
		}

		public static bool GetStateDown(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetStateDown(null, action, inputSource, caseSensitive);
		}

		public static bool GetStateUp(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Boolean action2 = GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetStateUp(inputSource);
			}
			return false;
		}

		public static bool GetStateUp(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetStateUp(null, action, inputSource, caseSensitive);
		}

		public static float GetFloat(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Single action2 = GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return 0f;
		}

		public static float GetFloat(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetFloat(null, action, inputSource, caseSensitive);
		}

		public static float GetSingle(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Single action2 = GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return 0f;
		}

		public static float GetSingle(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetFloat(null, action, inputSource, caseSensitive);
		}

		public static Vector2 GetVector2(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Vector2 action2 = GetAction<SteamVR_Action_Vector2>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return Vector2.zero;
		}

		public static Vector2 GetVector2(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetVector2(null, action, inputSource, caseSensitive);
		}

		public static Vector3 GetVector3(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Vector3 action2 = GetAction<SteamVR_Action_Vector3>(actionSet, action, caseSensitive);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return Vector3.zero;
		}

		public static Vector3 GetVector3(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return GetVector3(null, action, inputSource, caseSensitive);
		}

		public static SteamVR_ActionSet[] GetActionSets()
		{
			return actionSets;
		}

		public static T[] GetActions<T>() where T : SteamVR_Action
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(SteamVR_Action))
			{
				return actions as T[];
			}
			if (typeFromHandle == typeof(ISteamVR_Action_In))
			{
				return actionsIn as T[];
			}
			if (typeFromHandle == typeof(ISteamVR_Action_Out))
			{
				return actionsOut as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Boolean))
			{
				return actionsBoolean as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Single))
			{
				return actionsSingle as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Vector2))
			{
				return actionsVector2 as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Vector3))
			{
				return actionsVector3 as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Pose))
			{
				return actionsPose as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Skeleton))
			{
				return actionsSkeleton as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Vibration))
			{
				return actionsVibration as T[];
			}
			Debug.Log("<b>[SteamVR]</b> Wrong type.");
			return null;
		}

		internal static bool ShouldMakeCopy()
		{
			return !SteamVR_Behaviour.isPlaying;
		}

		public static string GetLocalizedName(ulong originHandle, params EVRInputStringBits[] localizedParts)
		{
			int num = 0;
			for (int i = 0; i < localizedParts.Length; i++)
			{
				num |= (int)localizedParts[i];
			}
			StringBuilder stringBuilder = new StringBuilder(500);
			OpenVR.Input.GetOriginLocalizedName(originHandle, stringBuilder, 500u, num);
			return stringBuilder.ToString();
		}

		public static void IdentifyActionsFile(bool showLogs = true)
		{
			string dataPath = Application.dataPath;
			int num = dataPath.LastIndexOf('/');
			dataPath = dataPath.Remove(num, dataPath.Length - num);
			string text = Path.Combine(dataPath, SteamVR_Settings.instance.actionsFilePath);
			text = text.Replace("\\", "/");
			if (File.Exists(text))
			{
				if (OpenVR.Input == null)
				{
					Debug.LogError("<b>[SteamVR]</b> Could not instantiate OpenVR Input interface.");
					return;
				}
				EVRInputError eVRInputError = OpenVR.Input.SetActionManifestPath(text);
				if (eVRInputError != 0)
				{
					Debug.LogError("<b>[SteamVR]</b> Error loading action manifest into SteamVR: " + eVRInputError);
					return;
				}
				int num2 = 0;
				if (actions != null)
				{
					num2 = actions.Length;
					if (showLogs)
					{
						Debug.Log($"<b>[SteamVR]</b> Successfully loaded {num2} actions from action manifest into SteamVR ({text})");
					}
				}
				else if (showLogs)
				{
					Debug.LogWarning("<b>[SteamVR]</b> No actions found, but the action manifest was loaded. This usually means you haven't generated actions. Window -> SteamVR Input -> Save and Generate.");
				}
			}
			else if (showLogs)
			{
				Debug.LogError("<b>[SteamVR]</b> Could not find actions file at: " + text);
			}
		}

		public static bool HasFileInMemoryBeenModified()
		{
			string dataPath = Application.dataPath;
			int num = dataPath.LastIndexOf("/");
			dataPath = dataPath.Remove(num, dataPath.Length - num);
			actionsFilePath = Path.Combine(dataPath, SteamVR_Settings.instance.actionsFilePath);
			string text = null;
			if (File.Exists(actionsFilePath))
			{
				text = File.ReadAllText(actionsFilePath);
				string badMD5Hash = SteamVR_Utils.GetBadMD5Hash(text);
				string usedString = JsonConvert.SerializeObject(actionFile, Formatting.Indented, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				string badMD5Hash2 = SteamVR_Utils.GetBadMD5Hash(usedString);
				return badMD5Hash != badMD5Hash2;
			}
			return true;
		}

		public static bool CreateEmptyActionsFile(bool completelyEmpty = false)
		{
			string dataPath = Application.dataPath;
			int num = dataPath.LastIndexOf("/");
			dataPath = dataPath.Remove(num, dataPath.Length - num);
			actionsFilePath = Path.Combine(dataPath, SteamVR_Settings.instance.actionsFilePath);
			if (File.Exists(actionsFilePath))
			{
				Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file already exists in project root: {0}", actionsFilePath);
				return false;
			}
			actionFile = new SteamVR_Input_ActionFile();
			if (!completelyEmpty)
			{
				actionFile.action_sets.Add(SteamVR_Input_ActionFile_ActionSet.CreateNew());
				actionFile.actions.Add(SteamVR_Input_ActionFile_Action.CreateNew(actionFile.action_sets[0].shortName, SteamVR_ActionDirections.In, SteamVR_Input_ActionFile_ActionTypes.boolean));
			}
			string contents = JsonConvert.SerializeObject(actionFile, Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
			File.WriteAllText(actionsFilePath, contents);
			actionFile.InitializeHelperLists();
			fileInitialized = true;
			return true;
		}

		public static bool DoesActionsFileExist()
		{
			string dataPath = Application.dataPath;
			int num = dataPath.LastIndexOf("/");
			dataPath = dataPath.Remove(num, dataPath.Length - num);
			actionsFilePath = Path.Combine(dataPath, SteamVR_Settings.instance.actionsFilePath);
			return File.Exists(actionsFilePath);
		}

		public static bool InitializeFile(bool force = false, bool showErrors = true)
		{
			bool flag = DoesActionsFileExist();
			string text = null;
			if (flag)
			{
				text = File.ReadAllText(actionsFilePath);
				if (fileInitialized || (fileInitialized && !force))
				{
					string badMD5Hash = SteamVR_Utils.GetBadMD5Hash(text);
					if (badMD5Hash == actionFileHash)
					{
						return true;
					}
					actionFileHash = badMD5Hash;
				}
				actionFile = JsonConvert.DeserializeObject<SteamVR_Input_ActionFile>(text);
				actionFile.InitializeHelperLists();
				fileInitialized = true;
				return true;
			}
			if (showErrors)
			{
				Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file does not exist in project root: {0}", actionsFilePath);
			}
			return false;
		}

		public static bool DeleteManifestAndBindings()
		{
			if (!DoesActionsFileExist())
			{
				return false;
			}
			InitializeFile();
			string[] filesToCopy = actionFile.GetFilesToCopy();
			string[] array = filesToCopy;
			foreach (string text in array)
			{
				FileInfo fileInfo = new FileInfo(text);
				fileInfo.IsReadOnly = false;
				File.Delete(text);
			}
			if (File.Exists(actionsFilePath))
			{
				FileInfo fileInfo2 = new FileInfo(actionsFilePath);
				fileInfo2.IsReadOnly = false;
				File.Delete(actionsFilePath);
				actionFile = null;
				fileInitialized = false;
				return true;
			}
			return false;
		}
	}
}
