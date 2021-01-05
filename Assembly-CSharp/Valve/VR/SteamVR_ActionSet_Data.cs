// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_ActionSet_Data
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_ActionSet_Data : ISteamVR_ActionSet
  {
    protected Dictionary<SteamVR_Input_Sources, bool> rawSetActive = new Dictionary<SteamVR_Input_Sources, bool>((IEqualityComparer<SteamVR_Input_Sources>) new SteamVR_Input_Sources_Comparer());
    protected Dictionary<SteamVR_Input_Sources, float> rawSetLastChanged = new Dictionary<SteamVR_Input_Sources, float>((IEqualityComparer<SteamVR_Input_Sources>) new SteamVR_Input_Sources_Comparer());
    protected Dictionary<SteamVR_Input_Sources, int> rawSetPriority = new Dictionary<SteamVR_Input_Sources, int>((IEqualityComparer<SteamVR_Input_Sources>) new SteamVR_Input_Sources_Comparer());
    protected bool initialized;
    private string cachedShortName;

    public SteamVR_Action[] allActions { get; set; }

    public ISteamVR_Action_In[] nonVisualInActions { get; set; }

    public ISteamVR_Action_In[] visualActions { get; set; }

    public SteamVR_Action_Pose[] poseActions { get; set; }

    public SteamVR_Action_Skeleton[] skeletonActions { get; set; }

    public ISteamVR_Action_Out[] outActionArray { get; set; }

    public string fullPath { get; set; }

    public string usage { get; set; }

    public ulong handle { get; set; }

    public void PreInitialize()
    {
      foreach (SteamVR_Input_Sources allSource in SteamVR_Input_Source.GetAllSources())
      {
        this.rawSetActive.Add(allSource, false);
        this.rawSetLastChanged.Add(allSource, 0.0f);
        this.rawSetPriority.Add(allSource, 0);
      }
    }

    public void FinishPreInitialize()
    {
      List<SteamVR_Action> steamVrActionList = new List<SteamVR_Action>();
      List<ISteamVR_Action_In> steamVrActionInList1 = new List<ISteamVR_Action_In>();
      List<ISteamVR_Action_In> steamVrActionInList2 = new List<ISteamVR_Action_In>();
      List<SteamVR_Action_Pose> steamVrActionPoseList = new List<SteamVR_Action_Pose>();
      List<SteamVR_Action_Skeleton> vrActionSkeletonList = new List<SteamVR_Action_Skeleton>();
      List<ISteamVR_Action_Out> steamVrActionOutList = new List<ISteamVR_Action_Out>();
      if (SteamVR_Input.actions == null)
      {
        Debug.LogError((object) "<b>[SteamVR Input]</b> Actions not initialized!");
      }
      else
      {
        for (int index = 0; index < SteamVR_Input.actions.Length; ++index)
        {
          SteamVR_Action action = SteamVR_Input.actions[index];
          if (action.actionSet.GetActionSetData() == this)
          {
            steamVrActionList.Add(action);
            switch (action)
            {
              case ISteamVR_Action_Boolean _:
              case ISteamVR_Action_Single _:
              case ISteamVR_Action_Vector2 _:
              case ISteamVR_Action_Vector3 _:
                steamVrActionInList1.Add((ISteamVR_Action_In) action);
                continue;
              case SteamVR_Action_Pose _:
                steamVrActionInList2.Add((ISteamVR_Action_In) action);
                steamVrActionPoseList.Add((SteamVR_Action_Pose) action);
                continue;
              case SteamVR_Action_Skeleton _:
                steamVrActionInList2.Add((ISteamVR_Action_In) action);
                vrActionSkeletonList.Add((SteamVR_Action_Skeleton) action);
                continue;
              case ISteamVR_Action_Out _:
                steamVrActionOutList.Add((ISteamVR_Action_Out) action);
                continue;
              default:
                Debug.LogError((object) ("<b>[SteamVR Input]</b> Action doesn't implement known interface: " + action.fullPath));
                continue;
            }
          }
        }
        this.allActions = steamVrActionList.ToArray();
        this.nonVisualInActions = steamVrActionInList1.ToArray();
        this.visualActions = steamVrActionInList2.ToArray();
        this.poseActions = steamVrActionPoseList.ToArray();
        this.skeletonActions = vrActionSkeletonList.ToArray();
        this.outActionArray = steamVrActionOutList.ToArray();
      }
    }

    public void Initialize()
    {
      ulong pHandle = 0;
      EVRInputError actionSetHandle = OpenVR.Input.GetActionSetHandle(this.fullPath.ToLower(), ref pHandle);
      this.handle = pHandle;
      if (actionSetHandle != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetActionSetHandle (" + this.fullPath + ") error: " + actionSetHandle.ToString()));
      this.initialized = true;
    }

    public bool IsActive(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
    {
      if (!this.initialized)
        return false;
      return this.rawSetActive[source] || this.rawSetActive[SteamVR_Input_Sources.Any];
    }

    public float GetTimeLastChanged(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any) => this.initialized ? this.rawSetLastChanged[source] : 0.0f;

    public void Activate(
      SteamVR_Input_Sources activateForSource = SteamVR_Input_Sources.Any,
      int priority = 0,
      bool disableAllOtherActionSets = false)
    {
      if (disableAllOtherActionSets)
        SteamVR_ActionSet_Manager.DisableAllActionSets();
      if (!this.rawSetActive[activateForSource])
      {
        this.rawSetActive[activateForSource] = true;
        SteamVR_ActionSet_Manager.SetChanged();
        this.rawSetLastChanged[activateForSource] = Time.realtimeSinceStartup;
      }
      if (this.rawSetPriority[activateForSource] == priority)
        return;
      this.rawSetPriority[activateForSource] = priority;
      SteamVR_ActionSet_Manager.SetChanged();
      this.rawSetLastChanged[activateForSource] = Time.realtimeSinceStartup;
    }

    public void Deactivate(SteamVR_Input_Sources forSource = SteamVR_Input_Sources.Any)
    {
      if (this.rawSetActive[forSource])
      {
        this.rawSetLastChanged[forSource] = Time.realtimeSinceStartup;
        SteamVR_ActionSet_Manager.SetChanged();
      }
      this.rawSetActive[forSource] = false;
      this.rawSetPriority[forSource] = 0;
    }

    public string GetShortName()
    {
      if (this.cachedShortName == null)
        this.cachedShortName = SteamVR_Input_ActionFile.GetShortName(this.fullPath);
      return this.cachedShortName;
    }

    public bool ReadRawSetActive(SteamVR_Input_Sources inputSource) => this.rawSetActive[inputSource];

    public float ReadRawSetLastChanged(SteamVR_Input_Sources inputSource) => this.rawSetLastChanged[inputSource];

    public int ReadRawSetPriority(SteamVR_Input_Sources inputSource) => this.rawSetPriority[inputSource];
  }
}
