// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_ActionSet_Manager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
  public static class SteamVR_ActionSet_Manager
  {
    public static VRActiveActionSet_t[] rawActiveActionSetArray;
    [NonSerialized]
    private static uint activeActionSetSize;
    private static bool changed;
    private static int lastFrameUpdated;
    public static string debugActiveSetListText;
    public static bool updateDebugTextInBuilds;

    public static void Initialize() => SteamVR_ActionSet_Manager.activeActionSetSize = (uint) Marshal.SizeOf(typeof (VRActiveActionSet_t));

    public static void DisableAllActionSets()
    {
      for (int index = 0; index < SteamVR_Input.actionSets.Length; ++index)
      {
        SteamVR_Input.actionSets[index].Deactivate(SteamVR_Input_Sources.Any);
        SteamVR_Input.actionSets[index].Deactivate(SteamVR_Input_Sources.LeftHand);
        SteamVR_Input.actionSets[index].Deactivate(SteamVR_Input_Sources.RightHand);
      }
    }

    public static void UpdateActionStates(bool force = false)
    {
      if (!force && Time.frameCount == SteamVR_ActionSet_Manager.lastFrameUpdated)
        return;
      SteamVR_ActionSet_Manager.lastFrameUpdated = Time.frameCount;
      if (SteamVR_ActionSet_Manager.changed)
        SteamVR_ActionSet_Manager.UpdateActionSetsArray();
      if (SteamVR_ActionSet_Manager.rawActiveActionSetArray == null || SteamVR_ActionSet_Manager.rawActiveActionSetArray.Length <= 0)
        return;
      EVRInputError evrInputError = OpenVR.Input.UpdateActionState(SteamVR_ActionSet_Manager.rawActiveActionSetArray, SteamVR_ActionSet_Manager.activeActionSetSize);
      if (evrInputError == EVRInputError.None)
        return;
      Debug.LogError((object) ("<b>[SteamVR]</b> UpdateActionState error: " + evrInputError.ToString()));
    }

    public static void SetChanged() => SteamVR_ActionSet_Manager.changed = true;

    private static void UpdateActionSetsArray()
    {
      List<VRActiveActionSet_t> activeActionSetTList = new List<VRActiveActionSet_t>();
      SteamVR_Input_Sources[] allSources = SteamVR_Input_Source.GetAllSources();
      for (int index1 = 0; index1 < SteamVR_Input.actionSets.Length; ++index1)
      {
        SteamVR_ActionSet actionSet = SteamVR_Input.actionSets[index1];
        for (int index2 = 0; index2 < allSources.Length; ++index2)
        {
          SteamVR_Input_Sources inputSource = allSources[index2];
          if (actionSet.ReadRawSetActive(inputSource))
          {
            VRActiveActionSet_t activeActionSetT = new VRActiveActionSet_t();
            activeActionSetT.ulActionSet = actionSet.handle;
            activeActionSetT.nPriority = actionSet.ReadRawSetPriority(inputSource);
            activeActionSetT.ulRestrictedToDevice = SteamVR_Input_Source.GetHandle(inputSource);
            int index3 = 0;
            while (index3 < activeActionSetTList.Count && activeActionSetTList[index3].nPriority <= activeActionSetT.nPriority)
              ++index3;
            activeActionSetTList.Insert(index3, activeActionSetT);
          }
        }
      }
      SteamVR_ActionSet_Manager.changed = false;
      SteamVR_ActionSet_Manager.rawActiveActionSetArray = activeActionSetTList.ToArray();
      if (!Application.isEditor && !SteamVR_ActionSet_Manager.updateDebugTextInBuilds)
        return;
      SteamVR_ActionSet_Manager.UpdateDebugText();
    }

    public static SteamVR_ActionSet GetSetFromHandle(ulong handle)
    {
      for (int index = 0; index < SteamVR_Input.actionSets.Length; ++index)
      {
        SteamVR_ActionSet actionSet = SteamVR_Input.actionSets[index];
        if ((long) actionSet.handle == (long) handle)
          return actionSet;
      }
      return (SteamVR_ActionSet) null;
    }

    private static void UpdateDebugText()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < SteamVR_ActionSet_Manager.rawActiveActionSetArray.Length; ++index)
      {
        VRActiveActionSet_t rawActiveActionSet = SteamVR_ActionSet_Manager.rawActiveActionSetArray[index];
        stringBuilder.Append(rawActiveActionSet.nPriority);
        stringBuilder.Append("\t");
        stringBuilder.Append((object) SteamVR_Input_Source.GetSource(rawActiveActionSet.ulRestrictedToDevice));
        stringBuilder.Append("\t");
        stringBuilder.Append(SteamVR_ActionSet_Manager.GetSetFromHandle(rawActiveActionSet.ulActionSet).GetShortName());
        stringBuilder.Append("\n");
      }
      SteamVR_ActionSet_Manager.debugActiveSetListText = stringBuilder.ToString();
    }
  }
}
