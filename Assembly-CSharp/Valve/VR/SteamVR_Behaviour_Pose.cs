// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Pose
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_Pose : MonoBehaviour
  {
    public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");
    [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
    public SteamVR_Input_Sources inputSource;
    [Tooltip("If not set, relative to parent")]
    public Transform origin;
    public SteamVR_Behaviour_PoseEvent onTransformUpdated;
    public SteamVR_Behaviour_PoseEvent onTransformChanged;
    public SteamVR_Behaviour_Pose_ConnectedChangedEvent onConnectedChanged;
    public SteamVR_Behaviour_Pose_TrackingChangedEvent onTrackingChanged;
    public SteamVR_Behaviour_Pose_DeviceIndexChangedEvent onDeviceIndexChanged;
    public SteamVR_Behaviour_Pose.UpdateHandler onTransformUpdatedEvent;
    public SteamVR_Behaviour_Pose.ChangeHandler onTransformChangedEvent;
    public SteamVR_Behaviour_Pose.DeviceConnectedChangeHandler onConnectedChangedEvent;
    public SteamVR_Behaviour_Pose.TrackingChangeHandler onTrackingChangedEvent;
    public SteamVR_Behaviour_Pose.DeviceIndexChangedHandler onDeviceIndexChangedEvent;
    [Tooltip("Can be disabled to stop broadcasting bound device status changes")]
    public bool broadcastDeviceChanges = true;
    protected int deviceIndex = -1;
    protected SteamVR_HistoryBuffer historyBuffer = new SteamVR_HistoryBuffer(30);
    protected int lastFrameUpdated;

    public bool isValid => this.poseAction[this.inputSource].poseIsValid;

    public bool isActive => this.poseAction[this.inputSource].active;

    protected virtual void Start()
    {
      if ((SteamVR_Action) this.poseAction == (SteamVR_Action) null)
      {
        Debug.LogError((object) "<b>[SteamVR]</b> No pose action set for this component");
      }
      else
      {
        this.CheckDeviceIndex();
        if (!((Object) this.origin == (Object) null))
          return;
        this.origin = this.transform.parent;
      }
    }

    protected virtual void OnEnable()
    {
      SteamVR.Initialize();
      if (!((SteamVR_Action) this.poseAction != (SteamVR_Action) null))
        return;
      this.poseAction[this.inputSource].onUpdate += new SteamVR_Action_Pose.UpdateHandler(this.SteamVR_Behaviour_Pose_OnUpdate);
      this.poseAction[this.inputSource].onDeviceConnectedChanged += new SteamVR_Action_Pose.DeviceConnectedChangeHandler(this.OnDeviceConnectedChanged);
      this.poseAction[this.inputSource].onTrackingChanged += new SteamVR_Action_Pose.TrackingChangeHandler(this.OnTrackingChanged);
      this.poseAction[this.inputSource].onChange += new SteamVR_Action_Pose.ChangeHandler(this.SteamVR_Behaviour_Pose_OnChange);
    }

    protected virtual void OnDisable()
    {
      if ((SteamVR_Action) this.poseAction != (SteamVR_Action) null)
      {
        this.poseAction[this.inputSource].onUpdate -= new SteamVR_Action_Pose.UpdateHandler(this.SteamVR_Behaviour_Pose_OnUpdate);
        this.poseAction[this.inputSource].onDeviceConnectedChanged -= new SteamVR_Action_Pose.DeviceConnectedChangeHandler(this.OnDeviceConnectedChanged);
        this.poseAction[this.inputSource].onTrackingChanged -= new SteamVR_Action_Pose.TrackingChangeHandler(this.OnTrackingChanged);
        this.poseAction[this.inputSource].onChange -= new SteamVR_Action_Pose.ChangeHandler(this.SteamVR_Behaviour_Pose_OnChange);
      }
      this.historyBuffer.Clear();
    }

    private void SteamVR_Behaviour_Pose_OnUpdate(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource)
    {
      this.UpdateHistoryBuffer();
      this.UpdateTransform();
      if (this.onTransformUpdated != null)
        this.onTransformUpdated.Invoke(this, this.inputSource);
      if (this.onTransformUpdatedEvent == null)
        return;
      this.onTransformUpdatedEvent(this, this.inputSource);
    }

    protected virtual void UpdateTransform()
    {
      this.CheckDeviceIndex();
      if ((SteamVR_Action) this.poseAction == (SteamVR_Action) null || !this.poseAction.active)
        return;
      if ((Object) this.origin != (Object) null)
      {
        this.transform.position = this.origin.transform.TransformPoint(this.poseAction[this.inputSource].localPosition);
        this.transform.rotation = this.origin.rotation * this.poseAction[this.inputSource].localRotation;
      }
      else
      {
        this.transform.localPosition = this.poseAction[this.inputSource].localPosition;
        this.transform.localRotation = this.poseAction[this.inputSource].localRotation;
      }
    }

    private void SteamVR_Behaviour_Pose_OnChange(
      SteamVR_Action_Pose fromAction,
      SteamVR_Input_Sources fromSource)
    {
      if (this.onTransformChanged != null)
        this.onTransformChanged.Invoke(this, fromSource);
      if (this.onTransformChangedEvent == null)
        return;
      this.onTransformChangedEvent(this, fromSource);
    }

    protected virtual void OnDeviceConnectedChanged(
      SteamVR_Action_Pose changedAction,
      SteamVR_Input_Sources changedSource,
      bool connected)
    {
      this.CheckDeviceIndex();
      if (this.onConnectedChanged != null)
        this.onConnectedChanged.Invoke(this, this.inputSource, connected);
      if (this.onConnectedChangedEvent == null)
        return;
      this.onConnectedChangedEvent(this, this.inputSource, connected);
    }

    protected virtual void OnTrackingChanged(
      SteamVR_Action_Pose changedAction,
      SteamVR_Input_Sources changedSource,
      ETrackingResult trackingChanged)
    {
      if (this.onTrackingChanged != null)
        this.onTrackingChanged.Invoke(this, this.inputSource, trackingChanged);
      if (this.onTrackingChangedEvent == null)
        return;
      this.onTrackingChangedEvent(this, this.inputSource, trackingChanged);
    }

    protected virtual void CheckDeviceIndex()
    {
      if (!this.poseAction[this.inputSource].active || !this.poseAction[this.inputSource].deviceIsConnected)
        return;
      int trackedDeviceIndex = (int) this.poseAction[this.inputSource].trackedDeviceIndex;
      if (this.deviceIndex == trackedDeviceIndex)
        return;
      this.deviceIndex = trackedDeviceIndex;
      if (this.broadcastDeviceChanges)
      {
        this.gameObject.BroadcastMessage("SetInputSource", (object) this.inputSource, SendMessageOptions.DontRequireReceiver);
        this.gameObject.BroadcastMessage("SetDeviceIndex", (object) this.deviceIndex, SendMessageOptions.DontRequireReceiver);
      }
      if (this.onDeviceIndexChanged != null)
        this.onDeviceIndexChanged.Invoke(this, this.inputSource, this.deviceIndex);
      if (this.onDeviceIndexChangedEvent == null)
        return;
      this.onDeviceIndexChangedEvent(this, this.inputSource, this.deviceIndex);
    }

    public int GetDeviceIndex()
    {
      if (this.deviceIndex == -1)
        this.CheckDeviceIndex();
      return this.deviceIndex;
    }

    public Vector3 GetVelocity() => this.poseAction[this.inputSource].velocity;

    public Vector3 GetAngularVelocity() => this.poseAction[this.inputSource].angularVelocity;

    public bool GetVelocitiesAtTimeOffset(
      float secondsFromNow,
      out Vector3 velocity,
      out Vector3 angularVelocity)
    {
      return this.poseAction[this.inputSource].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
    }

    public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
    {
      int topVelocity = this.historyBuffer.GetTopVelocity(10, 1);
      this.historyBuffer.GetAverageVelocities(out velocity, out angularVelocity, 2, topVelocity);
    }

    protected void UpdateHistoryBuffer()
    {
      int frameCount = Time.frameCount;
      if (this.lastFrameUpdated == frameCount)
        return;
      this.historyBuffer.Update(this.poseAction[this.inputSource].localPosition, this.poseAction[this.inputSource].localRotation, this.poseAction[this.inputSource].velocity, this.poseAction[this.inputSource].angularVelocity);
      this.lastFrameUpdated = frameCount;
    }

    public string GetLocalizedName(params EVRInputStringBits[] localizedParts) => (SteamVR_Action) this.poseAction != (SteamVR_Action) null ? this.poseAction.GetLocalizedOriginPart(this.inputSource, localizedParts) : (string) null;

    public delegate void ActiveChangeHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      bool active);

    public delegate void ChangeHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void UpdateHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource);

    public delegate void TrackingChangeHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      ETrackingResult trackingState);

    public delegate void ValidPoseChangeHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      bool validPose);

    public delegate void DeviceConnectedChangeHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      bool deviceConnected);

    public delegate void DeviceIndexChangedHandler(
      SteamVR_Behaviour_Pose fromAction,
      SteamVR_Input_Sources fromSource,
      int newDeviceIndex);
  }
}
