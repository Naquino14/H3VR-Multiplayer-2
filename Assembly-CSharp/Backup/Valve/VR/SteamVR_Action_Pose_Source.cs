// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Pose_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Action_Pose_Source : SteamVR_Action_In_Source, ISteamVR_Action_Pose, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    public ETrackingUniverseOrigin universeOrigin = ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated;
    protected static uint poseActionData_size;
    public float changeTolerance = Mathf.Epsilon;
    protected InputPoseActionData_t poseActionData = new InputPoseActionData_t();
    protected InputPoseActionData_t lastPoseActionData = new InputPoseActionData_t();
    protected InputPoseActionData_t tempPoseActionData = new InputPoseActionData_t();
    protected SteamVR_Action_Pose poseAction;

    public event SteamVR_Action_Pose.ActiveChangeHandler onActiveChange;

    public event SteamVR_Action_Pose.ActiveChangeHandler onActiveBindingChange;

    public event SteamVR_Action_Pose.ChangeHandler onChange;

    public event SteamVR_Action_Pose.UpdateHandler onUpdate;

    public event SteamVR_Action_Pose.TrackingChangeHandler onTrackingChanged;

    public event SteamVR_Action_Pose.ValidPoseChangeHandler onValidPoseChanged;

    public event SteamVR_Action_Pose.DeviceConnectedChangeHandler onDeviceConnectedChanged;

    public override bool changed { get; protected set; }

    public override bool lastChanged { get; protected set; }

    public override ulong activeOrigin => this.active ? this.poseActionData.activeOrigin : 0UL;

    public override ulong lastActiveOrigin => this.lastPoseActionData.activeOrigin;

    public override bool active => this.activeBinding && this.action.actionSet.IsActive(this.inputSource);

    public override bool activeBinding => this.poseActionData.bActive;

    public override bool lastActive { get; protected set; }

    public override bool lastActiveBinding => this.lastPoseActionData.bActive;

    public ETrackingResult trackingState => this.poseActionData.pose.eTrackingResult;

    public ETrackingResult lastTrackingState => this.lastPoseActionData.pose.eTrackingResult;

    public bool poseIsValid => this.poseActionData.pose.bPoseIsValid;

    public bool lastPoseIsValid => this.lastPoseActionData.pose.bPoseIsValid;

    public bool deviceIsConnected => this.poseActionData.pose.bDeviceIsConnected;

    public bool lastDeviceIsConnected => this.lastPoseActionData.pose.bDeviceIsConnected;

    public Vector3 localPosition { get; protected set; }

    public Quaternion localRotation { get; protected set; }

    public Vector3 lastLocalPosition { get; protected set; }

    public Quaternion lastLocalRotation { get; protected set; }

    public Vector3 velocity { get; protected set; }

    public Vector3 lastVelocity { get; protected set; }

    public Vector3 angularVelocity { get; protected set; }

    public Vector3 lastAngularVelocity { get; protected set; }

    public override void Preinitialize(
      SteamVR_Action wrappingAction,
      SteamVR_Input_Sources forInputSource)
    {
      base.Preinitialize(wrappingAction, forInputSource);
      this.poseAction = wrappingAction as SteamVR_Action_Pose;
    }

    public override void Initialize()
    {
      base.Initialize();
      if (SteamVR_Action_Pose_Source.poseActionData_size != 0U)
        return;
      SteamVR_Action_Pose_Source.poseActionData_size = (uint) Marshal.SizeOf(typeof (InputPoseActionData_t));
    }

    public override void UpdateValue() => this.UpdateValue(false);

    public virtual void UpdateValue(bool skipStateAndEventUpdates)
    {
      this.lastChanged = this.changed;
      this.lastPoseActionData = this.poseActionData;
      this.lastLocalPosition = this.localPosition;
      this.lastLocalRotation = this.localRotation;
      this.lastVelocity = this.velocity;
      this.lastAngularVelocity = this.angularVelocity;
      EVRInputError dataForNextFrame = OpenVR.Input.GetPoseActionDataForNextFrame(this.handle, this.universeOrigin, ref this.poseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
      if (dataForNextFrame != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetPoseActionData error (" + this.fullPath + "): " + dataForNextFrame.ToString() + " Handle: " + this.handle.ToString() + ". Input source: " + this.inputSource.ToString()));
      if (this.active)
        this.SetCacheVariables();
      this.changed = this.GetChanged();
      if (this.changed)
        this.changedTime = this.updateTime;
      if (skipStateAndEventUpdates)
        return;
      this.CheckAndSendEvents();
    }

    protected void SetCacheVariables()
    {
      this.localPosition = SteamVR_Utils.GetPosition(this.poseActionData.pose.mDeviceToAbsoluteTracking);
      this.localRotation = SteamVR_Utils.GetRotation(this.poseActionData.pose.mDeviceToAbsoluteTracking);
      this.velocity = this.GetUnityCoordinateVelocity(this.poseActionData.pose.vVelocity);
      this.angularVelocity = this.GetUnityCoordinateAngularVelocity(this.poseActionData.pose.vAngularVelocity);
      this.updateTime = Time.realtimeSinceStartup;
    }

    protected bool GetChanged() => (double) Vector3.Distance(this.localPosition, this.lastLocalPosition) > (double) this.changeTolerance || (double) Mathf.Abs(Quaternion.Angle(this.localRotation, this.lastLocalRotation)) > (double) this.changeTolerance || ((double) Vector3.Distance(this.velocity, this.lastVelocity) > (double) this.changeTolerance || (double) Vector3.Distance(this.angularVelocity, this.lastAngularVelocity) > (double) this.changeTolerance);

    public bool GetVelocitiesAtTimeOffset(
      float secondsFromNow,
      out Vector3 velocityAtTime,
      out Vector3 angularVelocityAtTime)
    {
      EVRInputError dataRelativeToNow = OpenVR.Input.GetPoseActionDataRelativeToNow(this.handle, this.universeOrigin, secondsFromNow, ref this.tempPoseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
      if (dataRelativeToNow != EVRInputError.None)
      {
        Debug.LogError((object) ("<b>[SteamVR]</b> GetPoseActionData error (" + this.fullPath + "): " + dataRelativeToNow.ToString() + " handle: " + this.handle.ToString()));
        velocityAtTime = Vector3.zero;
        angularVelocityAtTime = Vector3.zero;
        return false;
      }
      velocityAtTime = this.GetUnityCoordinateVelocity(this.tempPoseActionData.pose.vVelocity);
      angularVelocityAtTime = this.GetUnityCoordinateAngularVelocity(this.tempPoseActionData.pose.vAngularVelocity);
      return true;
    }

    public bool GetPoseAtTimeOffset(
      float secondsFromNow,
      out Vector3 positionAtTime,
      out Quaternion rotationAtTime,
      out Vector3 velocityAtTime,
      out Vector3 angularVelocityAtTime)
    {
      EVRInputError dataRelativeToNow = OpenVR.Input.GetPoseActionDataRelativeToNow(this.handle, this.universeOrigin, secondsFromNow, ref this.tempPoseActionData, SteamVR_Action_Pose_Source.poseActionData_size, this.inputSourceHandle);
      if (dataRelativeToNow != EVRInputError.None)
      {
        Debug.LogError((object) ("<b>[SteamVR]</b> GetPoseActionData error (" + this.fullPath + "): " + dataRelativeToNow.ToString() + " handle: " + this.handle.ToString()));
        velocityAtTime = Vector3.zero;
        angularVelocityAtTime = Vector3.zero;
        positionAtTime = Vector3.zero;
        rotationAtTime = Quaternion.identity;
        return false;
      }
      velocityAtTime = this.GetUnityCoordinateVelocity(this.tempPoseActionData.pose.vVelocity);
      angularVelocityAtTime = this.GetUnityCoordinateAngularVelocity(this.tempPoseActionData.pose.vAngularVelocity);
      positionAtTime = SteamVR_Utils.GetPosition(this.tempPoseActionData.pose.mDeviceToAbsoluteTracking);
      rotationAtTime = SteamVR_Utils.GetRotation(this.tempPoseActionData.pose.mDeviceToAbsoluteTracking);
      return true;
    }

    public void UpdateTransform(Transform transformToUpdate)
    {
      transformToUpdate.localPosition = this.localPosition;
      transformToUpdate.localRotation = this.localRotation;
    }

    protected virtual void CheckAndSendEvents()
    {
      if (this.trackingState != this.lastTrackingState && this.onTrackingChanged != null)
        this.onTrackingChanged(this.poseAction, this.inputSource, this.trackingState);
      if (this.poseIsValid != this.lastPoseIsValid && this.onValidPoseChanged != null)
        this.onValidPoseChanged(this.poseAction, this.inputSource, this.poseIsValid);
      if (this.deviceIsConnected != this.lastDeviceIsConnected && this.onDeviceConnectedChanged != null)
        this.onDeviceConnectedChanged(this.poseAction, this.inputSource, this.deviceIsConnected);
      if (this.changed && this.onChange != null)
        this.onChange(this.poseAction, this.inputSource);
      if (this.active != this.lastActive && this.onActiveChange != null)
        this.onActiveChange(this.poseAction, this.inputSource, this.active);
      if (this.activeBinding != this.lastActiveBinding && this.onActiveBindingChange != null)
        this.onActiveBindingChange(this.poseAction, this.inputSource, this.activeBinding);
      if (this.onUpdate == null)
        return;
      this.onUpdate(this.poseAction, this.inputSource);
    }

    protected Vector3 GetUnityCoordinateVelocity(HmdVector3_t vector) => this.GetUnityCoordinateVelocity(vector.v0, vector.v1, vector.v2);

    protected Vector3 GetUnityCoordinateAngularVelocity(HmdVector3_t vector) => this.GetUnityCoordinateAngularVelocity(vector.v0, vector.v1, vector.v2);

    protected Vector3 GetUnityCoordinateVelocity(float x, float y, float z) => new Vector3()
    {
      x = x,
      y = y,
      z = -z
    };

    protected Vector3 GetUnityCoordinateAngularVelocity(float x, float y, float z) => new Vector3()
    {
      x = -x,
      y = -y,
      z = z
    };
  }
}
