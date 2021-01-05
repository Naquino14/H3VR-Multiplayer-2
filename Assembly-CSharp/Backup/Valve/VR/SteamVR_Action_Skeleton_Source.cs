// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Skeleton_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Action_Skeleton_Source : SteamVR_Action_Pose_Source, ISteamVR_Action_Skeleton_Source
  {
    protected static uint skeletonActionData_size;
    protected VRSkeletalSummaryData_t skeletalSummaryData = new VRSkeletalSummaryData_t();
    protected VRSkeletalSummaryData_t lastSkeletalSummaryData = new VRSkeletalSummaryData_t();
    protected SteamVR_Action_Skeleton skeletonAction;
    protected VRBoneTransform_t[] tempBoneTransforms = new VRBoneTransform_t[31];
    protected InputSkeletalActionData_t skeletonActionData = new InputSkeletalActionData_t();
    protected InputSkeletalActionData_t lastSkeletonActionData = new InputSkeletalActionData_t();
    protected InputSkeletalActionData_t tempSkeletonActionData = new InputSkeletalActionData_t();

    public event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveChange;

    public event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveBindingChange;

    public event SteamVR_Action_Skeleton.ChangeHandler onChange;

    public event SteamVR_Action_Skeleton.UpdateHandler onUpdate;

    public event SteamVR_Action_Skeleton.TrackingChangeHandler onTrackingChanged;

    public event SteamVR_Action_Skeleton.ValidPoseChangeHandler onValidPoseChanged;

    public event SteamVR_Action_Skeleton.DeviceConnectedChangeHandler onDeviceConnectedChanged;

    public override bool activeBinding => this.skeletonActionData.bActive;

    public override bool lastActiveBinding => this.lastSkeletonActionData.bActive;

    public Vector3[] bonePositions { get; protected set; }

    public Quaternion[] boneRotations { get; protected set; }

    public Vector3[] lastBonePositions { get; protected set; }

    public Quaternion[] lastBoneRotations { get; protected set; }

    public EVRSkeletalMotionRange rangeOfMotion { get; set; }

    public EVRSkeletalTransformSpace skeletalTransformSpace { get; set; }

    public EVRSummaryType summaryDataType { get; set; }

    public float thumbCurl => this.fingerCurls[0];

    public float indexCurl => this.fingerCurls[1];

    public float middleCurl => this.fingerCurls[2];

    public float ringCurl => this.fingerCurls[3];

    public float pinkyCurl => this.fingerCurls[4];

    public float thumbIndexSplay => this.fingerSplays[0];

    public float indexMiddleSplay => this.fingerSplays[1];

    public float middleRingSplay => this.fingerSplays[2];

    public float ringPinkySplay => this.fingerSplays[3];

    public float lastThumbCurl => this.lastFingerCurls[0];

    public float lastIndexCurl => this.lastFingerCurls[1];

    public float lastMiddleCurl => this.lastFingerCurls[2];

    public float lastRingCurl => this.lastFingerCurls[3];

    public float lastPinkyCurl => this.lastFingerCurls[4];

    public float lastThumbIndexSplay => this.lastFingerSplays[0];

    public float lastIndexMiddleSplay => this.lastFingerSplays[1];

    public float lastMiddleRingSplay => this.lastFingerSplays[2];

    public float lastRingPinkySplay => this.lastFingerSplays[3];

    public float[] fingerCurls { get; protected set; }

    public float[] fingerSplays { get; protected set; }

    public float[] lastFingerCurls { get; protected set; }

    public float[] lastFingerSplays { get; protected set; }

    public bool poseChanged { get; protected set; }

    public bool onlyUpdateSummaryData { get; set; }

    public override void Preinitialize(
      SteamVR_Action wrappingAction,
      SteamVR_Input_Sources forInputSource)
    {
      base.Preinitialize(wrappingAction, forInputSource);
      this.skeletonAction = (SteamVR_Action_Skeleton) wrappingAction;
      this.bonePositions = new Vector3[31];
      this.lastBonePositions = new Vector3[31];
      this.boneRotations = new Quaternion[31];
      this.lastBoneRotations = new Quaternion[31];
      this.rangeOfMotion = EVRSkeletalMotionRange.WithController;
      this.skeletalTransformSpace = EVRSkeletalTransformSpace.Parent;
      this.fingerCurls = new float[SteamVR_Skeleton_FingerIndexes.enumArray.Length];
      this.fingerSplays = new float[SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length];
      this.lastFingerCurls = new float[SteamVR_Skeleton_FingerIndexes.enumArray.Length];
      this.lastFingerSplays = new float[SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length];
    }

    public override void Initialize()
    {
      base.Initialize();
      if (SteamVR_Action_Skeleton_Source.skeletonActionData_size != 0U)
        return;
      SteamVR_Action_Skeleton_Source.skeletonActionData_size = (uint) Marshal.SizeOf(typeof (InputSkeletalActionData_t));
    }

    public override void UpdateValue() => this.UpdateValue(false);

    public override void UpdateValue(bool skipStateAndEventUpdates)
    {
      this.lastActive = this.active;
      this.lastSkeletonActionData = this.skeletonActionData;
      this.lastSkeletalSummaryData = this.skeletalSummaryData;
      if (!this.onlyUpdateSummaryData)
      {
        for (int index = 0; index < 31; ++index)
        {
          this.lastBonePositions[index] = this.bonePositions[index];
          this.lastBoneRotations[index] = this.boneRotations[index];
        }
      }
      for (int index = 0; index < SteamVR_Skeleton_FingerIndexes.enumArray.Length; ++index)
        this.lastFingerCurls[index] = this.fingerCurls[index];
      for (int index = 0; index < SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length; ++index)
        this.lastFingerSplays[index] = this.fingerSplays[index];
      base.UpdateValue(true);
      this.poseChanged = this.changed;
      EVRInputError skeletalActionData = OpenVR.Input.GetSkeletalActionData(this.handle, ref this.skeletonActionData, SteamVR_Action_Skeleton_Source.skeletonActionData_size);
      if (skeletalActionData != EVRInputError.None)
      {
        Debug.LogError((object) ("<b>[SteamVR]</b> GetSkeletalActionData error (" + this.fullPath + "): " + skeletalActionData.ToString() + " handle: " + this.handle.ToString()));
      }
      else
      {
        if (this.active)
        {
          if (!this.onlyUpdateSummaryData)
          {
            EVRInputError skeletalBoneData = OpenVR.Input.GetSkeletalBoneData(this.handle, this.skeletalTransformSpace, this.rangeOfMotion, this.tempBoneTransforms);
            if (skeletalBoneData != EVRInputError.None)
              Debug.LogError((object) ("<b>[SteamVR]</b> GetSkeletalBoneData error (" + this.fullPath + "): " + skeletalBoneData.ToString() + " handle: " + this.handle.ToString()));
            for (int index = 0; index < this.tempBoneTransforms.Length; ++index)
            {
              this.bonePositions[index].x = -this.tempBoneTransforms[index].position.v0;
              this.bonePositions[index].y = this.tempBoneTransforms[index].position.v1;
              this.bonePositions[index].z = this.tempBoneTransforms[index].position.v2;
              this.boneRotations[index].x = this.tempBoneTransforms[index].orientation.x;
              this.boneRotations[index].y = -this.tempBoneTransforms[index].orientation.y;
              this.boneRotations[index].z = -this.tempBoneTransforms[index].orientation.z;
              this.boneRotations[index].w = this.tempBoneTransforms[index].orientation.w;
            }
            this.boneRotations[0] = SteamVR_Action_Skeleton.steamVRFixUpRotation * this.boneRotations[0];
          }
          this.UpdateSkeletalSummaryData(this.summaryDataType, true);
        }
        if (!this.changed)
        {
          for (int index = 0; index < this.tempBoneTransforms.Length; ++index)
          {
            if ((double) Vector3.Distance(this.lastBonePositions[index], this.bonePositions[index]) > (double) this.changeTolerance)
            {
              this.changed = true;
              break;
            }
            if ((double) Mathf.Abs(Quaternion.Angle(this.lastBoneRotations[index], this.boneRotations[index])) > (double) this.changeTolerance)
            {
              this.changed = true;
              break;
            }
          }
        }
        if (this.changed)
          this.changedTime = Time.realtimeSinceStartup;
        if (skipStateAndEventUpdates)
          return;
        this.CheckAndSendEvents();
      }
    }

    public int boneCount => (int) this.GetBoneCount();

    public uint GetBoneCount()
    {
      uint pBoneCount = 0;
      EVRInputError boneCount = OpenVR.Input.GetBoneCount(this.handle, ref pBoneCount);
      if (boneCount != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetBoneCount error (" + this.fullPath + "): " + boneCount.ToString() + " handle: " + this.handle.ToString()));
      return pBoneCount;
    }

    public int[] boneHierarchy => this.GetBoneHierarchy();

    public int[] GetBoneHierarchy()
    {
      int[] pParentIndices = new int[(int) this.GetBoneCount()];
      EVRInputError boneHierarchy = OpenVR.Input.GetBoneHierarchy(this.handle, pParentIndices);
      if (boneHierarchy != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetBoneHierarchy error (" + this.fullPath + "): " + boneHierarchy.ToString() + " handle: " + this.handle.ToString()));
      return pParentIndices;
    }

    public string GetBoneName(int boneIndex)
    {
      StringBuilder pchBoneName = new StringBuilder((int) byte.MaxValue);
      EVRInputError boneName = OpenVR.Input.GetBoneName(this.handle, boneIndex, pchBoneName, (uint) byte.MaxValue);
      if (boneName != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetBoneName error (" + this.fullPath + "): " + boneName.ToString() + " handle: " + this.handle.ToString()));
      return pchBoneName.ToString();
    }

    public SteamVR_Utils.RigidTransform[] GetReferenceTransforms(
      EVRSkeletalTransformSpace transformSpace,
      EVRSkeletalReferencePose referencePose)
    {
      SteamVR_Utils.RigidTransform[] rigidTransformArray = new SteamVR_Utils.RigidTransform[(IntPtr) this.GetBoneCount()];
      VRBoneTransform_t[] pTransformArray = new VRBoneTransform_t[rigidTransformArray.Length];
      EVRInputError referenceTransforms = OpenVR.Input.GetSkeletalReferenceTransforms(this.handle, transformSpace, referencePose, pTransformArray);
      if (referenceTransforms != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetSkeletalReferenceTransforms error (" + this.fullPath + "): " + referenceTransforms.ToString() + " handle: " + this.handle.ToString()));
      for (int index = 0; index < pTransformArray.Length; ++index)
      {
        Vector3 position = new Vector3(-pTransformArray[index].position.v0, pTransformArray[index].position.v1, pTransformArray[index].position.v2);
        Quaternion rotation = new Quaternion(pTransformArray[index].orientation.x, -pTransformArray[index].orientation.y, -pTransformArray[index].orientation.z, pTransformArray[index].orientation.w);
        rigidTransformArray[index] = new SteamVR_Utils.RigidTransform(position, rotation);
      }
      if (rigidTransformArray.Length > 0)
      {
        Quaternion quaternion = Quaternion.AngleAxis(180f, Vector3.up);
        rigidTransformArray[0].rot = quaternion * rigidTransformArray[0].rot;
      }
      return rigidTransformArray;
    }

    public EVRSkeletalTrackingLevel skeletalTrackingLevel => this.GetSkeletalTrackingLevel();

    public EVRSkeletalTrackingLevel GetSkeletalTrackingLevel()
    {
      EVRSkeletalTrackingLevel pSkeletalTrackingLevel = EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated;
      EVRInputError skeletalTrackingLevel = OpenVR.Input.GetSkeletalTrackingLevel(this.handle, ref pSkeletalTrackingLevel);
      if (skeletalTrackingLevel != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetSkeletalTrackingLevel error (" + this.fullPath + "): " + skeletalTrackingLevel.ToString() + " handle: " + this.handle.ToString()));
      return pSkeletalTrackingLevel;
    }

    protected VRSkeletalSummaryData_t GetSkeletalSummaryData(
      EVRSummaryType summaryType = EVRSummaryType.FromAnimation,
      bool force = false)
    {
      this.UpdateSkeletalSummaryData(summaryType, force);
      return this.skeletalSummaryData;
    }

    protected void UpdateSkeletalSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromAnimation, bool force = false)
    {
      if (!force && (this.summaryDataType == this.summaryDataType || !this.active))
        return;
      EVRInputError skeletalSummaryData = OpenVR.Input.GetSkeletalSummaryData(this.handle, summaryType, ref this.skeletalSummaryData);
      if (skeletalSummaryData != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetSkeletalSummaryData error (" + this.fullPath + "): " + skeletalSummaryData.ToString() + " handle: " + this.handle.ToString()));
      this.fingerCurls[0] = this.skeletalSummaryData.flFingerCurl0;
      this.fingerCurls[1] = this.skeletalSummaryData.flFingerCurl1;
      this.fingerCurls[2] = this.skeletalSummaryData.flFingerCurl2;
      this.fingerCurls[3] = this.skeletalSummaryData.flFingerCurl3;
      this.fingerCurls[4] = this.skeletalSummaryData.flFingerCurl4;
      this.fingerSplays[0] = this.skeletalSummaryData.flFingerSplay0;
      this.fingerSplays[1] = this.skeletalSummaryData.flFingerSplay1;
      this.fingerSplays[2] = this.skeletalSummaryData.flFingerSplay2;
      this.fingerSplays[3] = this.skeletalSummaryData.flFingerSplay3;
    }

    protected override void CheckAndSendEvents()
    {
      if (this.trackingState != this.lastTrackingState && this.onTrackingChanged != null)
        this.onTrackingChanged(this.skeletonAction, this.trackingState);
      if (this.poseIsValid != this.lastPoseIsValid && this.onValidPoseChanged != null)
        this.onValidPoseChanged(this.skeletonAction, this.poseIsValid);
      if (this.deviceIsConnected != this.lastDeviceIsConnected && this.onDeviceConnectedChanged != null)
        this.onDeviceConnectedChanged(this.skeletonAction, this.deviceIsConnected);
      if (this.changed && this.onChange != null)
        this.onChange(this.skeletonAction);
      if (this.active != this.lastActive && this.onActiveChange != null)
        this.onActiveChange(this.skeletonAction, this.active);
      if (this.activeBinding != this.lastActiveBinding && this.onActiveBindingChange != null)
        this.onActiveBindingChange(this.skeletonAction, this.activeBinding);
      if (this.onUpdate == null)
        return;
      this.onUpdate(this.skeletonAction);
    }
  }
}
