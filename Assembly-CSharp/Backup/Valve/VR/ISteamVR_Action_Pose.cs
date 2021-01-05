// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_Pose
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public interface ISteamVR_Action_Pose : ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    Vector3 localPosition { get; }

    Quaternion localRotation { get; }

    ETrackingResult trackingState { get; }

    Vector3 velocity { get; }

    Vector3 angularVelocity { get; }

    bool poseIsValid { get; }

    bool deviceIsConnected { get; }

    Vector3 lastLocalPosition { get; }

    Quaternion lastLocalRotation { get; }

    ETrackingResult lastTrackingState { get; }

    Vector3 lastVelocity { get; }

    Vector3 lastAngularVelocity { get; }

    bool lastPoseIsValid { get; }

    bool lastDeviceIsConnected { get; }
  }
}
