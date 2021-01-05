// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Pose_Base`2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public abstract class SteamVR_Action_Pose_Base<SourceMap, SourceElement> : SteamVR_Action_In<SourceMap, SourceElement>, ISteamVR_Action_Pose, ISteamVR_Action_In_Source, ISteamVR_Action_Source
    where SourceMap : SteamVR_Action_Pose_Source_Map<SourceElement>, new()
    where SourceElement : SteamVR_Action_Pose_Source, new()
  {
    protected static void SetUniverseOrigin(ETrackingUniverseOrigin newOrigin)
    {
      for (int index = 0; index < SteamVR_Input.actionsPose.Length; ++index)
        SteamVR_Input.actionsPose[index].sourceMap.SetTrackingUniverseOrigin(newOrigin);
      for (int index = 0; index < SteamVR_Input.actionsSkeleton.Length; ++index)
        SteamVR_Input.actionsSkeleton[index].sourceMap.SetTrackingUniverseOrigin(newOrigin);
    }

    public Vector3 localPosition => this.sourceMap[SteamVR_Input_Sources.Any].localPosition;

    public Quaternion localRotation => this.sourceMap[SteamVR_Input_Sources.Any].localRotation;

    public ETrackingResult trackingState => this.sourceMap[SteamVR_Input_Sources.Any].trackingState;

    public Vector3 velocity => this.sourceMap[SteamVR_Input_Sources.Any].velocity;

    public Vector3 angularVelocity => this.sourceMap[SteamVR_Input_Sources.Any].angularVelocity;

    public bool poseIsValid => this.sourceMap[SteamVR_Input_Sources.Any].poseIsValid;

    public bool deviceIsConnected => this.sourceMap[SteamVR_Input_Sources.Any].deviceIsConnected;

    public Vector3 lastLocalPosition => this.sourceMap[SteamVR_Input_Sources.Any].lastLocalPosition;

    public Quaternion lastLocalRotation => this.sourceMap[SteamVR_Input_Sources.Any].lastLocalRotation;

    public ETrackingResult lastTrackingState => this.sourceMap[SteamVR_Input_Sources.Any].lastTrackingState;

    public Vector3 lastVelocity => this.sourceMap[SteamVR_Input_Sources.Any].lastVelocity;

    public Vector3 lastAngularVelocity => this.sourceMap[SteamVR_Input_Sources.Any].lastAngularVelocity;

    public bool lastPoseIsValid => this.sourceMap[SteamVR_Input_Sources.Any].lastPoseIsValid;

    public bool lastDeviceIsConnected => this.sourceMap[SteamVR_Input_Sources.Any].lastDeviceIsConnected;

    public virtual void UpdateValues(bool skipStateAndEventUpdates) => this.sourceMap.UpdateValues(skipStateAndEventUpdates);

    public bool GetVelocitiesAtTimeOffset(
      SteamVR_Input_Sources inputSource,
      float secondsFromNow,
      out Vector3 velocity,
      out Vector3 angularVelocity)
    {
      return this.sourceMap[inputSource].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
    }

    public bool GetPoseAtTimeOffset(
      SteamVR_Input_Sources inputSource,
      float secondsFromNow,
      out Vector3 localPosition,
      out Quaternion localRotation,
      out Vector3 velocity,
      out Vector3 angularVelocity)
    {
      return this.sourceMap[inputSource].GetPoseAtTimeOffset(secondsFromNow, out localPosition, out localRotation, out velocity, out angularVelocity);
    }

    public virtual void UpdateTransform(
      SteamVR_Input_Sources inputSource,
      Transform transformToUpdate)
    {
      this.sourceMap[inputSource].UpdateTransform(transformToUpdate);
    }

    public Vector3 GetLocalPosition(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].localPosition;

    public Quaternion GetLocalRotation(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].localRotation;

    public Vector3 GetVelocity(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].velocity;

    public Vector3 GetAngularVelocity(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].angularVelocity;

    public bool GetDeviceIsConnected(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].deviceIsConnected;

    public bool GetPoseIsValid(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].poseIsValid;

    public ETrackingResult GetTrackingResult(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].trackingState;

    public Vector3 GetLastLocalPosition(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastLocalPosition;

    public Quaternion GetLastLocalRotation(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastLocalRotation;

    public Vector3 GetLastVelocity(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastVelocity;

    public Vector3 GetLastAngularVelocity(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastAngularVelocity;

    public bool GetLastDeviceIsConnected(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastDeviceIsConnected;

    public bool GetLastPoseIsValid(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastPoseIsValid;

    public ETrackingResult GetLastTrackingResult(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastTrackingState;
  }
}
