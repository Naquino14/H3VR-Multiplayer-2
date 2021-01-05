// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Skeleton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Action_Skeleton : SteamVR_Action_Pose_Base<SteamVR_Action_Skeleton_Source_Map, SteamVR_Action_Skeleton_Source>, ISteamVR_Action_Skeleton_Source, ISerializationCallbackReceiver
  {
    public const int numBones = 31;
    public static Quaternion steamVRFixUpRotation = Quaternion.AngleAxis(180f, Vector3.up);

    public event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
    }

    public event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveBindingChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
    }

    public event SteamVR_Action_Skeleton.ChangeHandler onChange
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onChange += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
    }

    public event SteamVR_Action_Skeleton.UpdateHandler onUpdate
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
    }

    public event SteamVR_Action_Skeleton.TrackingChangeHandler onTrackingChanged
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= value;
    }

    public event SteamVR_Action_Skeleton.ValidPoseChangeHandler onValidPoseChanged
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= value;
    }

    public event SteamVR_Action_Skeleton.DeviceConnectedChangeHandler onDeviceConnectedChanged
    {
      add => this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += value;
      remove => this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= value;
    }

    public virtual void UpdateValue(bool skipStateAndEventUpdates) => this.sourceMap[SteamVR_Input_Sources.Any].UpdateValue(skipStateAndEventUpdates);

    public void UpdateValueWithoutEvents() => this.sourceMap[SteamVR_Input_Sources.Any].UpdateValue(true);

    public void UpdateTransform(Transform transformToUpdate) => this.UpdateTransform(SteamVR_Input_Sources.Any, transformToUpdate);

    public Vector3[] bonePositions => this.sourceMap[SteamVR_Input_Sources.Any].bonePositions;

    public Quaternion[] boneRotations => this.sourceMap[SteamVR_Input_Sources.Any].boneRotations;

    public Vector3[] lastBonePositions => this.sourceMap[SteamVR_Input_Sources.Any].lastBonePositions;

    public Quaternion[] lastBoneRotations => this.sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations;

    public EVRSkeletalMotionRange rangeOfMotion
    {
      get => this.sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion;
      set => this.sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion = value;
    }

    public EVRSkeletalTransformSpace skeletalTransformSpace
    {
      get => this.sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace;
      set => this.sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace = value;
    }

    public EVRSummaryType summaryDataType
    {
      get => this.sourceMap[SteamVR_Input_Sources.Any].summaryDataType;
      set => this.sourceMap[SteamVR_Input_Sources.Any].summaryDataType = value;
    }

    public EVRSkeletalTrackingLevel skeletalTrackingLevel => this.sourceMap[SteamVR_Input_Sources.Any].skeletalTrackingLevel;

    public float thumbCurl => this.sourceMap[SteamVR_Input_Sources.Any].thumbCurl;

    public float indexCurl => this.sourceMap[SteamVR_Input_Sources.Any].indexCurl;

    public float middleCurl => this.sourceMap[SteamVR_Input_Sources.Any].middleCurl;

    public float ringCurl => this.sourceMap[SteamVR_Input_Sources.Any].ringCurl;

    public float pinkyCurl => this.sourceMap[SteamVR_Input_Sources.Any].pinkyCurl;

    public float thumbIndexSplay => this.sourceMap[SteamVR_Input_Sources.Any].thumbIndexSplay;

    public float indexMiddleSplay => this.sourceMap[SteamVR_Input_Sources.Any].indexMiddleSplay;

    public float middleRingSplay => this.sourceMap[SteamVR_Input_Sources.Any].middleRingSplay;

    public float ringPinkySplay => this.sourceMap[SteamVR_Input_Sources.Any].ringPinkySplay;

    public float lastThumbCurl => this.sourceMap[SteamVR_Input_Sources.Any].lastThumbCurl;

    public float lastIndexCurl => this.sourceMap[SteamVR_Input_Sources.Any].lastIndexCurl;

    public float lastMiddleCurl => this.sourceMap[SteamVR_Input_Sources.Any].lastMiddleCurl;

    public float lastRingCurl => this.sourceMap[SteamVR_Input_Sources.Any].lastRingCurl;

    public float lastPinkyCurl => this.sourceMap[SteamVR_Input_Sources.Any].lastPinkyCurl;

    public float lastThumbIndexSplay => this.sourceMap[SteamVR_Input_Sources.Any].lastThumbIndexSplay;

    public float lastIndexMiddleSplay => this.sourceMap[SteamVR_Input_Sources.Any].lastIndexMiddleSplay;

    public float lastMiddleRingSplay => this.sourceMap[SteamVR_Input_Sources.Any].lastMiddleRingSplay;

    public float lastRingPinkySplay => this.sourceMap[SteamVR_Input_Sources.Any].lastRingPinkySplay;

    public float[] fingerCurls => this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls;

    public float[] fingerSplays => this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays;

    public float[] lastFingerCurls => this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls;

    public float[] lastFingerSplays => this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays;

    public bool poseChanged => this.sourceMap[SteamVR_Input_Sources.Any].poseChanged;

    public bool onlyUpdateSummaryData
    {
      get => this.sourceMap[SteamVR_Input_Sources.Any].onlyUpdateSummaryData;
      set => this.sourceMap[SteamVR_Input_Sources.Any].onlyUpdateSummaryData = value;
    }

    public bool GetActive() => this.sourceMap[SteamVR_Input_Sources.Any].active;

    public bool GetSetActive() => this.actionSet.IsActive(SteamVR_Input_Sources.Any);

    public bool GetVelocitiesAtTimeOffset(
      float secondsFromNow,
      out Vector3 velocity,
      out Vector3 angularVelocity)
    {
      return this.sourceMap[SteamVR_Input_Sources.Any].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
    }

    public bool GetPoseAtTimeOffset(
      float secondsFromNow,
      out Vector3 position,
      out Quaternion rotation,
      out Vector3 velocity,
      out Vector3 angularVelocity)
    {
      return this.sourceMap[SteamVR_Input_Sources.Any].GetPoseAtTimeOffset(secondsFromNow, out position, out rotation, out velocity, out angularVelocity);
    }

    public Vector3 GetLocalPosition() => this.sourceMap[SteamVR_Input_Sources.Any].localPosition;

    public Quaternion GetLocalRotation() => this.sourceMap[SteamVR_Input_Sources.Any].localRotation;

    public Vector3 GetVelocity() => this.sourceMap[SteamVR_Input_Sources.Any].velocity;

    public Vector3 GetAngularVelocity() => this.sourceMap[SteamVR_Input_Sources.Any].angularVelocity;

    public bool GetDeviceIsConnected() => this.sourceMap[SteamVR_Input_Sources.Any].deviceIsConnected;

    public bool GetPoseIsValid() => this.sourceMap[SteamVR_Input_Sources.Any].poseIsValid;

    public ETrackingResult GetTrackingResult() => this.sourceMap[SteamVR_Input_Sources.Any].trackingState;

    public Vector3 GetLastLocalPosition() => this.sourceMap[SteamVR_Input_Sources.Any].lastLocalPosition;

    public Quaternion GetLastLocalRotation() => this.sourceMap[SteamVR_Input_Sources.Any].lastLocalRotation;

    public Vector3 GetLastVelocity() => this.sourceMap[SteamVR_Input_Sources.Any].lastVelocity;

    public Vector3 GetLastAngularVelocity() => this.sourceMap[SteamVR_Input_Sources.Any].lastAngularVelocity;

    public bool GetLastDeviceIsConnected() => this.sourceMap[SteamVR_Input_Sources.Any].lastDeviceIsConnected;

    public bool GetLastPoseIsValid() => this.sourceMap[SteamVR_Input_Sources.Any].lastPoseIsValid;

    public ETrackingResult GetLastTrackingResult() => this.sourceMap[SteamVR_Input_Sources.Any].lastTrackingState;

    public int boneCount => (int) this.GetBoneCount();

    public Vector3[] GetBonePositions(bool copy = false) => copy ? (Vector3[]) this.sourceMap[SteamVR_Input_Sources.Any].bonePositions.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].bonePositions;

    public Quaternion[] GetBoneRotations(bool copy = false) => copy ? (Quaternion[]) this.sourceMap[SteamVR_Input_Sources.Any].boneRotations.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].boneRotations;

    public Vector3[] GetLastBonePositions(bool copy = false) => copy ? (Vector3[]) this.sourceMap[SteamVR_Input_Sources.Any].lastBonePositions.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].lastBonePositions;

    public Quaternion[] GetLastBoneRotations(bool copy = false) => copy ? (Quaternion[]) this.sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations;

    public void SetRangeOfMotion(EVRSkeletalMotionRange range) => this.sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion = range;

    public void SetSkeletalTransformSpace(EVRSkeletalTransformSpace space) => this.sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace = space;

    public uint GetBoneCount() => this.sourceMap[SteamVR_Input_Sources.Any].GetBoneCount();

    public int[] GetBoneHierarchy() => this.sourceMap[SteamVR_Input_Sources.Any].GetBoneHierarchy();

    public string GetBoneName(int boneIndex) => this.sourceMap[SteamVR_Input_Sources.Any].GetBoneName(boneIndex);

    public SteamVR_Utils.RigidTransform[] GetReferenceTransforms(
      EVRSkeletalTransformSpace transformSpace,
      EVRSkeletalReferencePose referencePose)
    {
      return this.sourceMap[SteamVR_Input_Sources.Any].GetReferenceTransforms(transformSpace, referencePose);
    }

    public EVRSkeletalTrackingLevel GetSkeletalTrackingLevel() => this.sourceMap[SteamVR_Input_Sources.Any].GetSkeletalTrackingLevel();

    public float[] GetFingerCurls(bool copy = false) => copy ? (float[]) this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls;

    public float[] GetLastFingerCurls(bool copy = false) => copy ? (float[]) this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls;

    public float[] GetFingerSplays(bool copy = false) => copy ? (float[]) this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays;

    public float[] GetLastFingerSplays(bool copy = false) => copy ? (float[]) this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays.Clone() : this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays;

    public float GetFingerCurl(int finger) => this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls[finger];

    public float GetSplay(int fingerGapIndex) => this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays[fingerGapIndex];

    public float GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger) => this.GetFingerCurl((int) finger);

    public float GetSplay(SteamVR_Skeleton_FingerSplayIndexEnum fingerSplay) => this.GetSplay((int) fingerSplay);

    public float GetLastFingerCurl(int finger) => this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls[finger];

    public float GetLastSplay(int fingerGapIndex) => this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays[fingerGapIndex];

    public float GetLastFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger) => this.GetLastFingerCurl((int) finger);

    public float GetLastSplay(SteamVR_Skeleton_FingerSplayIndexEnum fingerSplay) => this.GetLastSplay((int) fingerSplay);

    public string GetLocalizedName(params EVRInputStringBits[] localizedParts) => this.sourceMap[SteamVR_Input_Sources.Any].GetLocalizedOriginPart(localizedParts);

    public void AddOnDeviceConnectedChanged(
      SteamVR_Action_Skeleton.DeviceConnectedChangeHandler functionToCall)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += functionToCall;
    }

    public void RemoveOnDeviceConnectedChanged(
      SteamVR_Action_Skeleton.DeviceConnectedChangeHandler functionToStopCalling)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= functionToStopCalling;
    }

    public void AddOnTrackingChanged(
      SteamVR_Action_Skeleton.TrackingChangeHandler functionToCall)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += functionToCall;
    }

    public void RemoveOnTrackingChanged(
      SteamVR_Action_Skeleton.TrackingChangeHandler functionToStopCalling)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= functionToStopCalling;
    }

    public void AddOnValidPoseChanged(
      SteamVR_Action_Skeleton.ValidPoseChangeHandler functionToCall)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += functionToCall;
    }

    public void RemoveOnValidPoseChanged(
      SteamVR_Action_Skeleton.ValidPoseChangeHandler functionToStopCalling)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= functionToStopCalling;
    }

    public void AddOnActiveChangeListener(
      SteamVR_Action_Skeleton.ActiveChangeHandler functionToCall)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += functionToCall;
    }

    public void RemoveOnActiveChangeListener(
      SteamVR_Action_Skeleton.ActiveChangeHandler functionToStopCalling)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= functionToStopCalling;
    }

    public void AddOnChangeListener(
      SteamVR_Action_Skeleton.ChangeHandler functionToCall)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onChange += functionToCall;
    }

    public void RemoveOnChangeListener(
      SteamVR_Action_Skeleton.ChangeHandler functionToStopCalling)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onChange -= functionToStopCalling;
    }

    public void AddOnUpdateListener(
      SteamVR_Action_Skeleton.UpdateHandler functionToCall)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += functionToCall;
    }

    public void RemoveOnUpdateListener(
      SteamVR_Action_Skeleton.UpdateHandler functionToStopCalling)
    {
      this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= functionToStopCalling;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() => this.InitAfterDeserialize();

    public delegate void ActiveChangeHandler(SteamVR_Action_Skeleton fromAction, bool active);

    public delegate void ChangeHandler(SteamVR_Action_Skeleton fromAction);

    public delegate void UpdateHandler(SteamVR_Action_Skeleton fromAction);

    public delegate void TrackingChangeHandler(
      SteamVR_Action_Skeleton fromAction,
      ETrackingResult trackingState);

    public delegate void ValidPoseChangeHandler(SteamVR_Action_Skeleton fromAction, bool validPose);

    public delegate void DeviceConnectedChangeHandler(
      SteamVR_Action_Skeleton fromAction,
      bool deviceConnected);
  }
}
