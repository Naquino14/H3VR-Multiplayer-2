// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
    public const string defaultInputGameObjectName = "[SteamVR Input]";
    private const string localizationKeyName = "localization";
    public static string actionsFilePath;
    public static bool fileInitialized = false;
    public static bool initialized = false;
    public static bool preInitialized = false;
    public static SteamVR_Input_ActionFile actionFile;
    public static string actionFileHash;
    protected static bool initializing = false;
    protected static int startupFrame = 0;
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
    protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPath = new Dictionary<string, SteamVR_ActionSet>();
    protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathLowered = new Dictionary<string, SteamVR_ActionSet>();
    protected static Dictionary<string, SteamVR_Action> actionsByPath = new Dictionary<string, SteamVR_Action>();
    protected static Dictionary<string, SteamVR_Action> actionsByPathLowered = new Dictionary<string, SteamVR_Action>();
    protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathCache = new Dictionary<string, SteamVR_ActionSet>();
    protected static Dictionary<string, SteamVR_Action> actionsByPathCache = new Dictionary<string, SteamVR_Action>();
    protected static Dictionary<string, SteamVR_Action> actionsByNameCache = new Dictionary<string, SteamVR_Action>();
    protected static Dictionary<string, SteamVR_ActionSet> actionSetsByNameCache = new Dictionary<string, SteamVR_ActionSet>();

    static SteamVR_Input() => SteamVR_Input.FindPreinitializeMethod();

    public static event System.Action onNonVisualActionsUpdated;

    public static event SteamVR_Input.PosesUpdatedHandler onPosesUpdated;

    public static event SteamVR_Input.SkeletonsUpdatedHandler onSkeletonsUpdated;

    public static bool isStartupFrame => Time.frameCount >= SteamVR_Input.startupFrame - 1 && Time.frameCount <= SteamVR_Input.startupFrame + 1;

    public static void ForcePreinitialize() => SteamVR_Input.FindPreinitializeMethod();

    private static void FindPreinitializeMethod()
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        System.Type type = assembly.GetType("Valve.VR.SteamVR_Actions");
        if (type != null)
        {
          MethodInfo method = type.GetMethod("PreInitialize");
          if (method != null)
          {
            method.Invoke((object) null, (object[]) null);
            break;
          }
        }
      }
    }

    public static void Initialize(bool force = false)
    {
      if (SteamVR_Input.initialized && !force)
        return;
      SteamVR_Input.initializing = true;
      SteamVR_Input.startupFrame = Time.frameCount;
      SteamVR_ActionSet_Manager.Initialize();
      SteamVR_Input_Source.Initialize();
      for (int index = 0; index < SteamVR_Input.actions.Length; ++index)
        SteamVR_Input.actions[index].Initialize(true);
      for (int index = 0; index < SteamVR_Input.actionSets.Length; ++index)
        SteamVR_Input.actionSets[index].Initialize(true);
      if (SteamVR_Settings.instance.activateFirstActionSetOnStart)
      {
        if (SteamVR_Input.actionSets.Length > 0)
          SteamVR_Input.actionSets[0].Activate(SteamVR_Input_Sources.Any, 0, false);
        else
          Debug.LogError((object) "<b>[SteamVR]</b> No action sets to activate.");
      }
      SteamVR_Action_Pose.SetTrackingUniverseOrigin(SteamVR_Settings.instance.trackingSpace);
      SteamVR_Input.initialized = true;
      SteamVR_Input.initializing = false;
    }

    public static void PreinitializeFinishActionSets()
    {
      for (int index = 0; index < SteamVR_Input.actionSets.Length; ++index)
        SteamVR_Input.actionSets[index].FinishPreInitialize();
    }

    public static void PreinitializeActionSetDictionaries()
    {
      SteamVR_Input.actionSetsByPath.Clear();
      SteamVR_Input.actionSetsByPathLowered.Clear();
      SteamVR_Input.actionSetsByPathCache.Clear();
      for (int index = 0; index < SteamVR_Input.actionSets.Length; ++index)
      {
        SteamVR_ActionSet actionSet = SteamVR_Input.actionSets[index];
        SteamVR_Input.actionSetsByPath.Add(actionSet.fullPath, actionSet);
        SteamVR_Input.actionSetsByPathLowered.Add(actionSet.fullPath.ToLower(), actionSet);
      }
    }

    public static void PreinitializeActionDictionaries()
    {
      SteamVR_Input.actionsByPath.Clear();
      SteamVR_Input.actionsByPathLowered.Clear();
      SteamVR_Input.actionsByPathCache.Clear();
      for (int index = 0; index < SteamVR_Input.actions.Length; ++index)
      {
        SteamVR_Action action = SteamVR_Input.actions[index];
        SteamVR_Input.actionsByPath.Add(action.fullPath, action);
        SteamVR_Input.actionsByPathLowered.Add(action.fullPath.ToLower(), action);
      }
    }

    public static void Update()
    {
      if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
        return;
      if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnUpdate))
        SteamVR_Input.UpdateNonVisualActions();
      if (!SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnUpdate))
        return;
      SteamVR_Input.UpdateVisualActions();
    }

    public static void LateUpdate()
    {
      if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
        return;
      if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
        SteamVR_Input.UpdateNonVisualActions();
      if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
        SteamVR_Input.UpdateVisualActions();
      else
        SteamVR_Input.UpdateSkeletonActions(true);
    }

    public static void FixedUpdate()
    {
      if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
        return;
      if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
        SteamVR_Input.UpdateNonVisualActions();
      if (!SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
        return;
      SteamVR_Input.UpdateVisualActions();
    }

    public static void OnPreCull()
    {
      if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
        return;
      if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnPreCull))
        SteamVR_Input.UpdateNonVisualActions();
      if (!SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
        return;
      SteamVR_Input.UpdateVisualActions();
    }

    public static void UpdateVisualActions(bool skipStateAndEventUpdates = false)
    {
      if (!SteamVR_Input.initialized)
        return;
      SteamVR_ActionSet_Manager.UpdateActionStates();
      SteamVR_Input.UpdatePoseActions(skipStateAndEventUpdates);
      SteamVR_Input.UpdateSkeletonActions(skipStateAndEventUpdates);
    }

    public static void UpdatePoseActions(bool skipSendingEvents = false)
    {
      if (!SteamVR_Input.initialized)
        return;
      for (int index = 0; index < SteamVR_Input.actionsPose.Length; ++index)
        SteamVR_Input.actionsPose[index].UpdateValues(skipSendingEvents);
      if (SteamVR_Input.onPosesUpdated == null)
        return;
      SteamVR_Input.onPosesUpdated(false);
    }

    public static void UpdateSkeletonActions(bool skipSendingEvents = false)
    {
      if (!SteamVR_Input.initialized)
        return;
      for (int index = 0; index < SteamVR_Input.actionsSkeleton.Length; ++index)
        SteamVR_Input.actionsSkeleton[index].UpdateValue(skipSendingEvents);
      if (SteamVR_Input.onSkeletonsUpdated == null)
        return;
      SteamVR_Input.onSkeletonsUpdated(skipSendingEvents);
    }

    public static void UpdateNonVisualActions()
    {
      if (!SteamVR_Input.initialized)
        return;
      SteamVR_ActionSet_Manager.UpdateActionStates();
      for (int index = 0; index < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; ++index)
        SteamVR_Input.actionsNonPoseNonSkeletonIn[index].UpdateValues();
      if (SteamVR_Input.onNonVisualActionsUpdated == null)
        return;
      SteamVR_Input.onNonVisualActionsUpdated();
    }

    public static T GetActionDataFromPath<T>(string path, bool caseSensitive = false) where T : SteamVR_Action_Source_Map
    {
      SteamVR_Action baseActionFromPath = SteamVR_Input.GetBaseActionFromPath(path, caseSensitive);
      return baseActionFromPath != (SteamVR_Action) null ? (T) baseActionFromPath.GetSourceMap() : (T) null;
    }

    public static SteamVR_ActionSet_Data GetActionSetDataFromPath(
      string path,
      bool caseSensitive = false)
    {
      SteamVR_ActionSet actionSetFromPath = SteamVR_Input.GetActionSetFromPath(path, caseSensitive);
      return actionSetFromPath != (SteamVR_ActionSet) null ? actionSetFromPath.GetActionSetData() : (SteamVR_ActionSet_Data) null;
    }

    public static T GetActionFromPath<T>(string path, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_Action, new()
    {
      SteamVR_Action baseActionFromPath = SteamVR_Input.GetBaseActionFromPath(path, caseSensitive);
      if (baseActionFromPath != (SteamVR_Action) null)
        return baseActionFromPath.GetCopy<T>();
      return returnNulls ? (T) null : SteamVR_Input.CreateFakeAction<T>(path, caseSensitive);
    }

    public static SteamVR_Action GetBaseActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      if (string.IsNullOrEmpty(path))
        return (SteamVR_Action) null;
      if (caseSensitive)
      {
        if (SteamVR_Input.actionsByPath.ContainsKey(path))
          return SteamVR_Input.actionsByPath[path];
      }
      else
      {
        if (SteamVR_Input.actionsByPathCache.ContainsKey(path))
          return SteamVR_Input.actionsByPathCache[path];
        if (SteamVR_Input.actionsByPath.ContainsKey(path))
        {
          SteamVR_Input.actionsByPathCache.Add(path, SteamVR_Input.actionsByPath[path]);
          return SteamVR_Input.actionsByPath[path];
        }
        string lower = path.ToLower();
        if (SteamVR_Input.actionsByPathLowered.ContainsKey(lower))
        {
          SteamVR_Input.actionsByPathCache.Add(path, SteamVR_Input.actionsByPathLowered[lower]);
          return SteamVR_Input.actionsByPath[lower];
        }
        SteamVR_Input.actionsByPathCache.Add(path, (SteamVR_Action) null);
      }
      return (SteamVR_Action) null;
    }

    public static bool HasActionPath(string path, bool caseSensitive = false) => SteamVR_Input.GetBaseActionFromPath(path, caseSensitive) != (SteamVR_Action) null;

    public static bool HasAction(string actionName, bool caseSensitive = false) => SteamVR_Input.GetBaseAction((string) null, actionName, caseSensitive) != (SteamVR_Action) null;

    public static bool HasAction(string actionSetName, string actionName, bool caseSensitive = false) => SteamVR_Input.GetBaseAction(actionSetName, actionName, caseSensitive) != (SteamVR_Action) null;

    public static SteamVR_Action_Boolean GetBooleanActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Boolean>(path, caseSensitive);
    }

    public static SteamVR_Action_Single GetSingleActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Single>(path, caseSensitive);
    }

    public static SteamVR_Action_Vector2 GetVector2ActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Vector2>(path, caseSensitive);
    }

    public static SteamVR_Action_Vector3 GetVector3ActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Vector3>(path, caseSensitive);
    }

    public static SteamVR_Action_Vibration GetVibrationActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Vibration>(path, caseSensitive);
    }

    public static SteamVR_Action_Pose GetPoseActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Pose>(path, caseSensitive);
    }

    public static SteamVR_Action_Skeleton GetSkeletonActionFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionFromPath<SteamVR_Action_Skeleton>(path, caseSensitive);
    }

    public static T GetAction<T>(
      string actionSetName,
      string actionName,
      bool caseSensitive = false,
      bool returnNulls = false)
      where T : SteamVR_Action, new()
    {
      SteamVR_Action baseAction = SteamVR_Input.GetBaseAction(actionSetName, actionName, caseSensitive);
      if (baseAction != (SteamVR_Action) null)
        return baseAction.GetCopy<T>();
      return returnNulls ? (T) null : SteamVR_Input.CreateFakeAction<T>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action GetBaseAction(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      if (SteamVR_Input.actions == null)
        return (SteamVR_Action) null;
      if (string.IsNullOrEmpty(actionSetName))
      {
        for (int index = 0; index < SteamVR_Input.actions.Length; ++index)
        {
          if (caseSensitive)
          {
            if (SteamVR_Input.actions[index].GetShortName() == actionName)
              return SteamVR_Input.actions[index];
          }
          else if (string.Equals(SteamVR_Input.actions[index].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
            return SteamVR_Input.actions[index];
        }
      }
      else
      {
        SteamVR_ActionSet actionSet = SteamVR_Input.GetActionSet(actionSetName, caseSensitive, true);
        if (actionSet != (SteamVR_ActionSet) null)
        {
          for (int index = 0; index < actionSet.allActions.Length; ++index)
          {
            if (caseSensitive)
            {
              if (actionSet.allActions[index].GetShortName() == actionName)
                return actionSet.allActions[index];
            }
            else if (string.Equals(actionSet.allActions[index].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
              return actionSet.allActions[index];
          }
        }
      }
      return (SteamVR_Action) null;
    }

    private static T CreateFakeAction<T>(
      string actionSetName,
      string actionName,
      bool caseSensitive)
      where T : SteamVR_Action, new()
    {
      return typeof (T) == typeof (SteamVR_Action_Vibration) ? SteamVR_Action.CreateUninitialized<T>(actionSetName, SteamVR_ActionDirections.Out, actionName, caseSensitive) : SteamVR_Action.CreateUninitialized<T>(actionSetName, SteamVR_ActionDirections.In, actionName, caseSensitive);
    }

    private static T CreateFakeAction<T>(string actionPath, bool caseSensitive) where T : SteamVR_Action, new() => SteamVR_Action.CreateUninitialized<T>(actionPath, caseSensitive);

    public static T GetAction<T>(string actionName, bool caseSensitive = false) where T : SteamVR_Action, new() => SteamVR_Input.GetAction<T>((string) null, actionName, caseSensitive);

    public static SteamVR_Action_Boolean GetBooleanAction(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Boolean GetBooleanAction(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Boolean>((string) null, actionName, caseSensitive);
    }

    public static SteamVR_Action_Single GetSingleAction(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Single>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Single GetSingleAction(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Single>((string) null, actionName, caseSensitive);
    }

    public static SteamVR_Action_Vector2 GetVector2Action(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Vector2>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Vector2 GetVector2Action(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Vector2>((string) null, actionName, caseSensitive);
    }

    public static SteamVR_Action_Vector3 GetVector3Action(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Vector3>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Vector3 GetVector3Action(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Vector3>((string) null, actionName, caseSensitive);
    }

    public static SteamVR_Action_Pose GetPoseAction(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Pose>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Pose GetPoseAction(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Pose>((string) null, actionName, caseSensitive);
    }

    public static SteamVR_Action_Skeleton GetSkeletonAction(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Skeleton>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Skeleton GetSkeletonAction(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Skeleton>((string) null, actionName, caseSensitive);
    }

    public static SteamVR_Action_Vibration GetVibrationAction(
      string actionSetName,
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Vibration>(actionSetName, actionName, caseSensitive);
    }

    public static SteamVR_Action_Vibration GetVibrationAction(
      string actionName,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetAction<SteamVR_Action_Vibration>((string) null, actionName, caseSensitive);
    }

    public static T GetActionSet<T>(string actionSetName, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_ActionSet, new()
    {
      if (SteamVR_Input.actionSets == null)
        return returnNulls ? (T) null : SteamVR_ActionSet.CreateFromName<T>(actionSetName);
      for (int index = 0; index < SteamVR_Input.actionSets.Length; ++index)
      {
        if (caseSensitive)
        {
          if (SteamVR_Input.actionSets[index].GetShortName() == actionSetName)
            return SteamVR_Input.actionSets[index].GetCopy<T>();
        }
        else if (string.Equals(SteamVR_Input.actionSets[index].GetShortName(), actionSetName, StringComparison.CurrentCultureIgnoreCase))
          return SteamVR_Input.actionSets[index].GetCopy<T>();
      }
      return returnNulls ? (T) null : SteamVR_ActionSet.CreateFromName<T>(actionSetName);
    }

    public static SteamVR_ActionSet GetActionSet(
      string actionSetName,
      bool caseSensitive = false,
      bool returnsNulls = false)
    {
      return SteamVR_Input.GetActionSet<SteamVR_ActionSet>(actionSetName, caseSensitive, returnsNulls);
    }

    protected static bool HasActionSet(string name, bool caseSensitive = false) => SteamVR_Input.GetActionSet(name, caseSensitive, true) != (SteamVR_ActionSet) null;

    public static T GetActionSetFromPath<T>(string path, bool caseSensitive = false, bool returnsNulls = false) where T : SteamVR_ActionSet, new()
    {
      if (SteamVR_Input.actionSets == null || SteamVR_Input.actionSets[0] == (SteamVR_ActionSet) null || string.IsNullOrEmpty(path))
        return returnsNulls ? (T) null : SteamVR_ActionSet.Create<T>(path);
      if (caseSensitive)
      {
        if (SteamVR_Input.actionSetsByPath.ContainsKey(path))
          return SteamVR_Input.actionSetsByPath[path].GetCopy<T>();
      }
      else
      {
        if (SteamVR_Input.actionSetsByPathCache.ContainsKey(path))
        {
          SteamVR_ActionSet steamVrActionSet = SteamVR_Input.actionSetsByPathCache[path];
          return steamVrActionSet == (SteamVR_ActionSet) null ? (T) null : steamVrActionSet.GetCopy<T>();
        }
        if (SteamVR_Input.actionSetsByPath.ContainsKey(path))
        {
          SteamVR_Input.actionSetsByPathCache.Add(path, SteamVR_Input.actionSetsByPath[path]);
          return SteamVR_Input.actionSetsByPath[path].GetCopy<T>();
        }
        string lower = path.ToLower();
        if (SteamVR_Input.actionSetsByPathLowered.ContainsKey(lower))
        {
          SteamVR_Input.actionSetsByPathCache.Add(path, SteamVR_Input.actionSetsByPathLowered[lower]);
          return SteamVR_Input.actionSetsByPath[lower].GetCopy<T>();
        }
        SteamVR_Input.actionSetsByPathCache.Add(path, (SteamVR_ActionSet) null);
      }
      return returnsNulls ? (T) null : SteamVR_ActionSet.Create<T>(path);
    }

    public static SteamVR_ActionSet GetActionSetFromPath(
      string path,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetActionSetFromPath<SteamVR_ActionSet>(path, caseSensitive);
    }

    public static bool GetState(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Boolean action1 = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null && action1.GetState(inputSource);
    }

    public static bool GetState(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetState((string) null, action, inputSource, caseSensitive);
    }

    public static bool GetStateDown(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Boolean action1 = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null && action1.GetStateDown(inputSource);
    }

    public static bool GetStateDown(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetStateDown((string) null, action, inputSource, caseSensitive);
    }

    public static bool GetStateUp(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Boolean action1 = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null && action1.GetStateUp(inputSource);
    }

    public static bool GetStateUp(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetStateUp((string) null, action, inputSource, caseSensitive);
    }

    public static float GetFloat(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Single action1 = SteamVR_Input.GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null ? action1.GetAxis(inputSource) : 0.0f;
    }

    public static float GetFloat(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetFloat((string) null, action, inputSource, caseSensitive);
    }

    public static float GetSingle(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Single action1 = SteamVR_Input.GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null ? action1.GetAxis(inputSource) : 0.0f;
    }

    public static float GetSingle(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetFloat((string) null, action, inputSource, caseSensitive);
    }

    public static Vector2 GetVector2(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Vector2 action1 = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null ? action1.GetAxis(inputSource) : Vector2.zero;
    }

    public static Vector2 GetVector2(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetVector2((string) null, action, inputSource, caseSensitive);
    }

    public static Vector3 GetVector3(
      string actionSet,
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      SteamVR_Action_Vector3 action1 = SteamVR_Input.GetAction<SteamVR_Action_Vector3>(actionSet, action, caseSensitive);
      return (SteamVR_Action) action1 != (SteamVR_Action) null ? action1.GetAxis(inputSource) : Vector3.zero;
    }

    public static Vector3 GetVector3(
      string action,
      SteamVR_Input_Sources inputSource,
      bool caseSensitive = false)
    {
      return SteamVR_Input.GetVector3((string) null, action, inputSource, caseSensitive);
    }

    public static SteamVR_ActionSet[] GetActionSets() => SteamVR_Input.actionSets;

    public static T[] GetActions<T>() where T : SteamVR_Action
    {
      System.Type type = typeof (T);
      if (type == typeof (SteamVR_Action))
        return SteamVR_Input.actions as T[];
      if (type == typeof (ISteamVR_Action_In))
        return SteamVR_Input.actionsIn as T[];
      if (type == typeof (ISteamVR_Action_Out))
        return SteamVR_Input.actionsOut as T[];
      if (type == typeof (SteamVR_Action_Boolean))
        return (object) SteamVR_Input.actionsBoolean as T[];
      if (type == typeof (SteamVR_Action_Single))
        return (object) SteamVR_Input.actionsSingle as T[];
      if (type == typeof (SteamVR_Action_Vector2))
        return (object) SteamVR_Input.actionsVector2 as T[];
      if (type == typeof (SteamVR_Action_Vector3))
        return (object) SteamVR_Input.actionsVector3 as T[];
      if (type == typeof (SteamVR_Action_Pose))
        return (object) SteamVR_Input.actionsPose as T[];
      if (type == typeof (SteamVR_Action_Skeleton))
        return (object) SteamVR_Input.actionsSkeleton as T[];
      if (type == typeof (SteamVR_Action_Vibration))
        return (object) SteamVR_Input.actionsVibration as T[];
      Debug.Log((object) "<b>[SteamVR]</b> Wrong type.");
      return (T[]) null;
    }

    internal static bool ShouldMakeCopy() => !SteamVR_Behaviour.isPlaying;

    public static string GetLocalizedName(
      ulong originHandle,
      params EVRInputStringBits[] localizedParts)
    {
      int unStringSectionsToInclude = 0;
      for (int index = 0; index < localizedParts.Length; ++index)
        unStringSectionsToInclude = (int) ((EVRInputStringBits) unStringSectionsToInclude | localizedParts[index]);
      StringBuilder pchNameArray = new StringBuilder(500);
      int originLocalizedName = (int) OpenVR.Input.GetOriginLocalizedName(originHandle, pchNameArray, 500U, unStringSectionsToInclude);
      return pchNameArray.ToString();
    }

    public static void IdentifyActionsFile(bool showLogs = true)
    {
      string dataPath = Application.dataPath;
      int startIndex = dataPath.LastIndexOf('/');
      string str = Path.Combine(dataPath.Remove(startIndex, dataPath.Length - startIndex), SteamVR_Settings.instance.actionsFilePath).Replace("\\", "/");
      if (File.Exists(str))
      {
        if (OpenVR.Input == null)
        {
          Debug.LogError((object) "<b>[SteamVR]</b> Could not instantiate OpenVR Input interface.");
        }
        else
        {
          EVRInputError evrInputError = OpenVR.Input.SetActionManifestPath(str);
          if (evrInputError != EVRInputError.None)
            Debug.LogError((object) ("<b>[SteamVR]</b> Error loading action manifest into SteamVR: " + evrInputError.ToString()));
          else if (SteamVR_Input.actions != null)
          {
            int length = SteamVR_Input.actions.Length;
            if (!showLogs)
              return;
            Debug.Log((object) string.Format("<b>[SteamVR]</b> Successfully loaded {0} actions from action manifest into SteamVR ({1})", (object) length, (object) str));
          }
          else
          {
            if (!showLogs)
              return;
            Debug.LogWarning((object) "<b>[SteamVR]</b> No actions found, but the action manifest was loaded. This usually means you haven't generated actions. Window -> SteamVR Input -> Save and Generate.");
          }
        }
      }
      else
      {
        if (!showLogs)
          return;
        Debug.LogError((object) ("<b>[SteamVR]</b> Could not find actions file at: " + str));
      }
    }

    public static bool HasFileInMemoryBeenModified()
    {
      string dataPath = Application.dataPath;
      int startIndex = dataPath.LastIndexOf("/");
      SteamVR_Input.actionsFilePath = Path.Combine(dataPath.Remove(startIndex, dataPath.Length - startIndex), SteamVR_Settings.instance.actionsFilePath);
      if (!File.Exists(SteamVR_Input.actionsFilePath))
        return true;
      return SteamVR_Utils.GetBadMD5Hash(File.ReadAllText(SteamVR_Input.actionsFilePath)) != SteamVR_Utils.GetBadMD5Hash(JsonConvert.SerializeObject((object) SteamVR_Input.actionFile, Formatting.Indented, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      }));
    }

    public static bool CreateEmptyActionsFile(bool completelyEmpty = false)
    {
      string dataPath = Application.dataPath;
      int startIndex = dataPath.LastIndexOf("/");
      SteamVR_Input.actionsFilePath = Path.Combine(dataPath.Remove(startIndex, dataPath.Length - startIndex), SteamVR_Settings.instance.actionsFilePath);
      if (File.Exists(SteamVR_Input.actionsFilePath))
      {
        Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file already exists in project root: {0}", (object) SteamVR_Input.actionsFilePath);
        return false;
      }
      SteamVR_Input.actionFile = new SteamVR_Input_ActionFile();
      if (!completelyEmpty)
      {
        SteamVR_Input.actionFile.action_sets.Add(SteamVR_Input_ActionFile_ActionSet.CreateNew());
        SteamVR_Input.actionFile.actions.Add(SteamVR_Input_ActionFile_Action.CreateNew(SteamVR_Input.actionFile.action_sets[0].shortName, SteamVR_ActionDirections.In, SteamVR_Input_ActionFile_ActionTypes.boolean));
      }
      string contents = JsonConvert.SerializeObject((object) SteamVR_Input.actionFile, Formatting.Indented, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
      File.WriteAllText(SteamVR_Input.actionsFilePath, contents);
      SteamVR_Input.actionFile.InitializeHelperLists();
      SteamVR_Input.fileInitialized = true;
      return true;
    }

    public static bool DoesActionsFileExist()
    {
      string dataPath = Application.dataPath;
      int startIndex = dataPath.LastIndexOf("/");
      SteamVR_Input.actionsFilePath = Path.Combine(dataPath.Remove(startIndex, dataPath.Length - startIndex), SteamVR_Settings.instance.actionsFilePath);
      return File.Exists(SteamVR_Input.actionsFilePath);
    }

    public static bool InitializeFile(bool force = false, bool showErrors = true)
    {
      if (SteamVR_Input.DoesActionsFileExist())
      {
        string usedString = File.ReadAllText(SteamVR_Input.actionsFilePath);
        if (SteamVR_Input.fileInitialized || SteamVR_Input.fileInitialized && !force)
        {
          string badMd5Hash = SteamVR_Utils.GetBadMD5Hash(usedString);
          if (badMd5Hash == SteamVR_Input.actionFileHash)
            return true;
          SteamVR_Input.actionFileHash = badMd5Hash;
        }
        SteamVR_Input.actionFile = JsonConvert.DeserializeObject<SteamVR_Input_ActionFile>(usedString);
        SteamVR_Input.actionFile.InitializeHelperLists();
        SteamVR_Input.fileInitialized = true;
        return true;
      }
      if (showErrors)
        Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file does not exist in project root: {0}", (object) SteamVR_Input.actionsFilePath);
      return false;
    }

    public static bool DeleteManifestAndBindings()
    {
      if (!SteamVR_Input.DoesActionsFileExist())
        return false;
      SteamVR_Input.InitializeFile();
      foreach (string str in SteamVR_Input.actionFile.GetFilesToCopy())
      {
        new FileInfo(str).IsReadOnly = false;
        File.Delete(str);
      }
      if (!File.Exists(SteamVR_Input.actionsFilePath))
        return false;
      new FileInfo(SteamVR_Input.actionsFilePath).IsReadOnly = false;
      File.Delete(SteamVR_Input.actionsFilePath);
      SteamVR_Input.actionFile = (SteamVR_Input_ActionFile) null;
      SteamVR_Input.fileInitialized = false;
      return true;
    }

    public delegate void PosesUpdatedHandler(bool skipSendingEvents);

    public delegate void SkeletonsUpdatedHandler(bool skipSendingEvents);
  }
}
