// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.ExternalCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace LIV.SDK.Unity
{
  [AddComponentMenu("LIV/ExternalCamera")]
  public class ExternalCamera : MonoBehaviour
  {
    private TrackedDevicePose_t _trackedDevicePose;
    private TrackedDevicePose_t[] _devices = new TrackedDevicePose_t[new IntPtr(64)];
    private TrackedDevicePose_t[] _gameDevices = new TrackedDevicePose_t[new IntPtr(64)];

    public bool IsValid => this.OpenVRTrackedDeviceId != uint.MaxValue;

    public uint OpenVRTrackedDeviceId { get; protected set; }

    private void OnEnable()
    {
      this.InvokeRepeating("UpdateOpenVRDevice", 0.5f, 0.5f);
      this.UpdateOpenVRDevice();
    }

    private void OnDisable() => this.CancelInvoke("UpdateOpenVRDevice");

    private void LateUpdate() => this.UpdateCamera();

    private void OnPreCull() => this.UpdateCamera();

    private void UpdateCamera()
    {
      if (this.OpenVRTrackedDeviceId == uint.MaxValue)
        return;
      this.UpdateOpenVRPose();
      if (!this._trackedDevicePose.bDeviceIsConnected)
      {
        this.UpdateOpenVRDevice();
        if (this.OpenVRTrackedDeviceId == uint.MaxValue)
          return;
        this.UpdateOpenVRPose();
      }
      this.UpdateTransform(this._trackedDevicePose.mDeviceToAbsoluteTracking);
    }

    private void UpdateOpenVRPose()
    {
      if (OpenVR.Compositor.GetLastPoses(this._devices, this._gameDevices) != EVRCompositorError.None)
        return;
      this._trackedDevicePose = this._devices[(IntPtr) this.OpenVRTrackedDeviceId];
    }

    private void UpdateTransform(HmdMatrix34_t deviceToAbsolute)
    {
      Matrix4x4 identity = Matrix4x4.identity;
      identity[0, 0] = deviceToAbsolute.m0;
      identity[0, 1] = deviceToAbsolute.m1;
      identity[0, 2] = -deviceToAbsolute.m2;
      identity[0, 3] = deviceToAbsolute.m3;
      identity[1, 0] = deviceToAbsolute.m4;
      identity[1, 1] = deviceToAbsolute.m5;
      identity[1, 2] = -deviceToAbsolute.m6;
      identity[1, 3] = deviceToAbsolute.m7;
      identity[2, 0] = -deviceToAbsolute.m8;
      identity[2, 1] = -deviceToAbsolute.m9;
      identity[2, 2] = deviceToAbsolute.m10;
      identity[2, 3] = -deviceToAbsolute.m11;
      this.transform.localPosition = identity.GetPosition();
      this.transform.localRotation = identity.GetRotation();
    }

    private void UpdateOpenVRDevice() => this.OpenVRTrackedDeviceId = this.IdentifyExternalCameraDevice();

    private uint IdentifyExternalCameraDevice()
    {
      EVRCompositorError lastPoses = OpenVR.Compositor.GetLastPoses(this._devices, this._gameDevices);
      if (lastPoses != EVRCompositorError.None)
      {
        Debug.Log((object) ("Encountered an error whilst looking for the external camera: " + (object) lastPoses));
        return uint.MaxValue;
      }
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      \u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string> anonType2 = ((IEnumerable<TrackedDevicePose_t>) this._devices).Select<TrackedDevicePose_t, \u003C\u003E__AnonType0<TrackedDevicePose_t, uint>>((Func<TrackedDevicePose_t, int, \u003C\u003E__AnonType0<TrackedDevicePose_t, uint>>) ((pose, index) => new \u003C\u003E__AnonType0<TrackedDevicePose_t, uint>(pose, (uint) index))).Where<\u003C\u003E__AnonType0<TrackedDevicePose_t, uint>>((Func<\u003C\u003E__AnonType0<TrackedDevicePose_t, uint>, bool>) (candidate => candidate.pose.bDeviceIsConnected)).Select<\u003C\u003E__AnonType0<TrackedDevicePose_t, uint>, \u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>>((Func<\u003C\u003E__AnonType0<TrackedDevicePose_t, uint>, \u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>>) (candidate => new \u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>(candidate.pose, candidate.index, OpenVR.System.GetTrackedDeviceClass(candidate.index)))).Where<\u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>>((Func<\u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>, bool>) (candidate => candidate.deviceClass == ETrackedDeviceClass.Controller || candidate.deviceClass == ETrackedDeviceClass.GenericTracker)).Select<\u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>, \u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string>>((Func<\u003C\u003E__AnonType1<TrackedDevicePose_t, uint, ETrackedDeviceClass>, \u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string>>) (candidate => new \u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string>(candidate.pose, candidate.index, candidate.deviceClass, OpenVR.System.GetControllerRoleForTrackedDeviceIndex(candidate.index), ExternalCamera.GetStringTrackedDeviceProperty(candidate.index, ETrackedDeviceProperty.Prop_ModelNumber_String), ExternalCamera.GetStringTrackedDeviceProperty(candidate.index, ETrackedDeviceProperty.Prop_RenderModelName_String)))).OrderByDescending<\u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string>, int>((Func<\u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string>, int>) (candidate =>
      {
        if (candidate.modelNumber == "LIV Virtual Camera")
          return 10;
        if (candidate.modelNumber == "Virtual Controller Device")
          return 9;
        if (candidate.deviceClass == ETrackedDeviceClass.GenericTracker)
          return 5;
        if (candidate.deviceClass != ETrackedDeviceClass.Controller)
          return 0;
        if (candidate.renderModel == "{htc}vr_tracker_vive_1_0")
          return 8;
        if (candidate.deviceRole == ETrackedControllerRole.OptOut)
          return 7;
        return candidate.deviceRole == ETrackedControllerRole.Invalid ? 6 : 1;
      })).FirstOrDefault<\u003C\u003E__AnonType2<TrackedDevicePose_t, uint, ETrackedDeviceClass, ETrackedControllerRole, string, string>>();
      return anonType2 != null ? anonType2.index : uint.MaxValue;
    }

    private static string GetStringTrackedDeviceProperty(
      uint device,
      ETrackedDeviceProperty property)
    {
      StringBuilder pchValue = new StringBuilder(1024);
      ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
      int trackedDeviceProperty = (int) OpenVR.System.GetStringTrackedDeviceProperty(device, property, pchValue, 1024U, ref pError);
      if (pError == ETrackedPropertyError.TrackedProp_Success)
        return pchValue.ToString();
      if (pError == ETrackedPropertyError.TrackedProp_UnknownProperty)
        return string.Empty;
      Debug.Log((object) ("Encountered an error whilst reading a tracked device property: " + (object) pError));
      return (string) null;
    }
  }
}
