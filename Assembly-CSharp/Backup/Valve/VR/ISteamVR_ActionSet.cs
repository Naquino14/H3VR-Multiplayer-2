// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_ActionSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public interface ISteamVR_ActionSet
  {
    SteamVR_Action[] allActions { get; }

    ISteamVR_Action_In[] nonVisualInActions { get; }

    ISteamVR_Action_In[] visualActions { get; }

    SteamVR_Action_Pose[] poseActions { get; }

    SteamVR_Action_Skeleton[] skeletonActions { get; }

    ISteamVR_Action_Out[] outActionArray { get; }

    string fullPath { get; }

    string usage { get; }

    ulong handle { get; }

    bool ReadRawSetActive(SteamVR_Input_Sources inputSource);

    float ReadRawSetLastChanged(SteamVR_Input_Sources inputSource);

    int ReadRawSetPriority(SteamVR_Input_Sources inputSource);

    bool IsActive(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any);

    float GetTimeLastChanged(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any);

    void Activate(
      SteamVR_Input_Sources activateForSource = SteamVR_Input_Sources.Any,
      int priority = 0,
      bool disableAllOtherActionSets = false);

    void Deactivate(SteamVR_Input_Sources forSource = SteamVR_Input_Sources.Any);

    string GetShortName();
  }
}
