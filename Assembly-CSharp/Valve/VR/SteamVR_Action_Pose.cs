// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Pose
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Action_Pose : SteamVR_Action_Pose_Base<SteamVR_Action_Pose_Source_Map<SteamVR_Action_Pose_Source>, SteamVR_Action_Pose_Source>, ISerializationCallbackReceiver
  {
    public event SteamVR_Action_Pose.ActiveChangeHandler onActiveChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
    }

    public event SteamVR_Action_Pose.ActiveChangeHandler onActiveBindingChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
    }

    public event SteamVR_Action_Pose.ChangeHandler onChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
    }

    public event SteamVR_Action_Pose.UpdateHandler onUpdate
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
    }

    public event SteamVR_Action_Pose.TrackingChangeHandler onTrackingChanged
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= value;
    }

    public event SteamVR_Action_Pose.ValidPoseChangeHandler onValidPoseChanged
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= value;
    }

    public event SteamVR_Action_Pose.DeviceConnectedChangeHandler onDeviceConnectedChanged
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= value;
    }

    public void AddOnDeviceConnectedChanged(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.DeviceConnectedChangeHandler functionToCall)
    {
      this.sourceMap[inputSource].onDeviceConnectedChanged += functionToCall;
    }

    public void RemoveOnDeviceConnectedChanged(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.DeviceConnectedChangeHandler functionToStopCalling)
    {
      this.sourceMap[inputSource].onDeviceConnectedChanged -= functionToStopCalling;
    }

    public void AddOnTrackingChanged(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.TrackingChangeHandler functionToCall)
    {
      this.sourceMap[inputSource].onTrackingChanged += functionToCall;
    }

    public void RemoveOnTrackingChanged(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.TrackingChangeHandler functionToStopCalling)
    {
      this.sourceMap[inputSource].onTrackingChanged -= functionToStopCalling;
    }

    public void AddOnValidPoseChanged(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.ValidPoseChangeHandler functionToCall)
    {
      this.sourceMap[inputSource].onValidPoseChanged += functionToCall;
    }

    public void RemoveOnValidPoseChanged(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.ValidPoseChangeHandler functionToStopCalling)
    {
      this.sourceMap[inputSource].onValidPoseChanged -= functionToStopCalling;
    }

    public void AddOnActiveChangeListener(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.ActiveChangeHandler functionToCall)
    {
      this.sourceMap[inputSource].onActiveChange += functionToCall;
    }

    public void RemoveOnActiveChangeListener(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.ActiveChangeHandler functionToStopCalling)
    {
      this.sourceMap[inputSource].onActiveChange -= functionToStopCalling;
    }

    public void AddOnChangeListener(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.ChangeHandler functionToCall)
    {
      this.sourceMap[inputSource].onChange += functionToCall;
    }

    public void RemoveOnChangeListener(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.ChangeHandler functionToStopCalling)
    {
      this.sourceMap[inputSource].onChange -= functionToStopCalling;
    }

    public void AddOnUpdateListener(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.UpdateHandler functionToCall)
    {
      this.sourceMap[inputSource].onUpdate += functionToCall;
    }

    public void RemoveOnUpdateListener(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action_Pose.UpdateHandler functionToStopCalling)
    {
      this.sourceMap[inputSource].onUpdate -= functionToStopCalling;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() => this.InitAfterDeserialize();

    public static void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newOrigin)
    {
      SteamVR_Action_Pose_Base<SteamVR_Action_Pose_Source_Map<SteamVR_Action_Pose_Source>, SteamVR_Action_Pose_Source>.SetUniverseOrigin(newOrigin);
      OpenVR.Compositor.SetTrackingSpace(newOrigin);
    }

    public delegate void ActiveChangeHandler(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      bool active);

    public delegate void ChangeHandler(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void UpdateHandler(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void TrackingChangeHandler(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      ETrackingResult trackingState);

    public delegate void ValidPoseChangeHandler(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      bool validPose);

    public delegate void DeviceConnectedChangeHandler(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      bool deviceConnected);
  }
}
