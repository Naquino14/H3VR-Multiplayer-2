// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_Skeleton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_Skeleton : MonoBehaviour
  {
    [Tooltip("If not set, will try to auto assign this based on 'Skeleton' + inputSource")]
    public SteamVR_Action_Skeleton skeletonAction;
    [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
    public SteamVR_Input_Sources inputSource;
    [Tooltip("The range of motion you'd like the hand to move in. With controller is the best estimate of the fingers wrapped around a controller. Without is from a flat hand to a fist.")]
    public EVRSkeletalMotionRange rangeOfMotion = EVRSkeletalMotionRange.WithoutController;
    [Tooltip("This needs to be in the order of: root -> wrist -> thumb, index, middle, ring, pinky")]
    public Transform skeletonRoot;
    [Tooltip("If not set, relative to parent")]
    public Transform origin;
    [Tooltip("Set to true if you want this script to update its position and rotation. False if this will be handled elsewhere")]
    public bool updatePose = true;
    [Tooltip("Check this to not set the positions of the bones. This is helpful for differently scaled skeletons.")]
    public bool onlySetRotations;
    [Range(0.0f, 1f)]
    [Tooltip("Modify this to blend between animations setup on the hand")]
    public float skeletonBlend = 1f;
    public SteamVR_Behaviour_SkeletonEvent onBoneTransformsUpdated;
    public SteamVR_Behaviour_SkeletonEvent onTransformUpdated;
    public SteamVR_Behaviour_SkeletonEvent onTransformChanged;
    public SteamVR_Behaviour_Skeleton_ConnectedChangedEvent onConnectedChanged;
    public SteamVR_Behaviour_Skeleton_TrackingChangedEvent onTrackingChanged;
    public SteamVR_Behaviour_Skeleton.UpdateHandler onBoneTransformsUpdatedEvent;
    public SteamVR_Behaviour_Skeleton.UpdateHandler onTransformUpdatedEvent;
    public SteamVR_Behaviour_Skeleton.ChangeHandler onTransformChangedEvent;
    public SteamVR_Behaviour_Skeleton.DeviceConnectedChangeHandler onConnectedChangedEvent;
    public SteamVR_Behaviour_Skeleton.TrackingChangeHandler onTrackingChangedEvent;
    protected SteamVR_Skeleton_Poser blendPoser;
    protected SteamVR_Skeleton_PoseSnapshot blendSnapshot;
    [Tooltip("Is this rendermodel a mirror of another one?")]
    public SteamVR_Behaviour_Skeleton.MirrorType mirroring;
    protected Coroutine blendRoutine;
    protected Coroutine rangeOfMotionBlendRoutine;
    protected Coroutine attachRoutine;
    protected Transform[] bones;
    protected EVRSkeletalMotionRange? temporaryRangeOfMotion;
    private Vector3[] oldROMPositionBuffer = new Vector3[31];
    private Vector3[] newROMPositionBuffer = new Vector3[31];
    private Quaternion[] oldROMRotationBuffer = new Quaternion[31];
    private Quaternion[] newROMRotationBuffer = new Quaternion[31];
    private Vector3[] bonePositionBuffer = new Vector3[31];
    private Quaternion[] boneRotationBuffer = new Quaternion[31];
    private static readonly Quaternion rightFlipAngle = Quaternion.AngleAxis(180f, Vector3.right);

    public bool isActive => this.skeletonAction.GetActive();

    public float[] fingerCurls => this.skeletonAction.GetFingerCurls();

    public float thumbCurl => this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.thumb);

    public float indexCurl => this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.index);

    public float middleCurl => this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.middle);

    public float ringCurl => this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.ring);

    public float pinkyCurl => this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.pinky);

    public Transform root => this.bones[0];

    public Transform wrist => this.bones[1];

    public Transform indexMetacarpal => this.bones[6];

    public Transform indexProximal => this.bones[7];

    public Transform indexMiddle => this.bones[8];

    public Transform indexDistal => this.bones[9];

    public Transform indexTip => this.bones[10];

    public Transform middleMetacarpal => this.bones[11];

    public Transform middleProximal => this.bones[12];

    public Transform middleMiddle => this.bones[13];

    public Transform middleDistal => this.bones[14];

    public Transform middleTip => this.bones[15];

    public Transform pinkyMetacarpal => this.bones[21];

    public Transform pinkyProximal => this.bones[22];

    public Transform pinkyMiddle => this.bones[23];

    public Transform pinkyDistal => this.bones[24];

    public Transform pinkyTip => this.bones[25];

    public Transform ringMetacarpal => this.bones[16];

    public Transform ringProximal => this.bones[17];

    public Transform ringMiddle => this.bones[18];

    public Transform ringDistal => this.bones[19];

    public Transform ringTip => this.bones[20];

    public Transform thumbMetacarpal => this.bones[2];

    public Transform thumbProximal => this.bones[2];

    public Transform thumbMiddle => this.bones[3];

    public Transform thumbDistal => this.bones[4];

    public Transform thumbTip => this.bones[5];

    public Transform thumbAux => this.bones[26];

    public Transform indexAux => this.bones[27];

    public Transform middleAux => this.bones[28];

    public Transform ringAux => this.bones[29];

    public Transform pinkyAux => this.bones[30];

    public Transform[] proximals { get; protected set; }

    public Transform[] middles { get; protected set; }

    public Transform[] distals { get; protected set; }

    public Transform[] tips { get; protected set; }

    public Transform[] auxs { get; protected set; }

    public EVRSkeletalTrackingLevel skeletalTrackingLevel => this.skeletonAction.skeletalTrackingLevel;

    public bool isBlending => this.blendRoutine != null;

    public SteamVR_ActionSet actionSet => this.skeletonAction.actionSet;

    public SteamVR_ActionDirections direction => this.skeletonAction.direction;

    protected virtual void Awake()
    {
      this.AssignBonesArray();
      this.proximals = new Transform[5]
      {
        this.thumbProximal,
        this.indexProximal,
        this.middleProximal,
        this.ringProximal,
        this.pinkyProximal
      };
      this.middles = new Transform[5]
      {
        this.thumbMiddle,
        this.indexMiddle,
        this.middleMiddle,
        this.ringMiddle,
        this.pinkyMiddle
      };
      this.distals = new Transform[5]
      {
        this.thumbDistal,
        this.indexDistal,
        this.middleDistal,
        this.ringDistal,
        this.pinkyDistal
      };
      this.tips = new Transform[5]
      {
        this.thumbTip,
        this.indexTip,
        this.middleTip,
        this.ringTip,
        this.pinkyTip
      };
      this.auxs = new Transform[5]
      {
        this.thumbAux,
        this.indexAux,
        this.middleAux,
        this.ringAux,
        this.pinkyAux
      };
      this.CheckSkeletonAction();
    }

    protected virtual void CheckSkeletonAction()
    {
      if (!((SteamVR_Action) this.skeletonAction == (SteamVR_Action) null))
        return;
      this.skeletonAction = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>("Skeleton" + this.inputSource.ToString());
    }

    protected virtual void AssignBonesArray() => this.bones = this.skeletonRoot.GetComponentsInChildren<Transform>();

    protected virtual void OnEnable()
    {
      this.CheckSkeletonAction();
      SteamVR_Input.onSkeletonsUpdated += new SteamVR_Input.SkeletonsUpdatedHandler(this.SteamVR_Input_OnSkeletonsUpdated);
      if (!((SteamVR_Action) this.skeletonAction != (SteamVR_Action) null))
        return;
      this.skeletonAction.onDeviceConnectedChanged += new SteamVR_Action_Skeleton.DeviceConnectedChangeHandler(this.OnDeviceConnectedChanged);
      this.skeletonAction.onTrackingChanged += new SteamVR_Action_Skeleton.TrackingChangeHandler(this.OnTrackingChanged);
    }

    protected virtual void OnDisable()
    {
      SteamVR_Input.onSkeletonsUpdated -= new SteamVR_Input.SkeletonsUpdatedHandler(this.SteamVR_Input_OnSkeletonsUpdated);
      if (!((SteamVR_Action) this.skeletonAction != (SteamVR_Action) null))
        return;
      this.skeletonAction.onDeviceConnectedChanged -= new SteamVR_Action_Skeleton.DeviceConnectedChangeHandler(this.OnDeviceConnectedChanged);
      this.skeletonAction.onTrackingChanged -= new SteamVR_Action_Skeleton.TrackingChangeHandler(this.OnTrackingChanged);
    }

    private void OnDeviceConnectedChanged(SteamVR_Action_Skeleton fromAction, bool deviceConnected)
    {
      if (this.onConnectedChanged != null)
        this.onConnectedChanged.Invoke(this, this.inputSource, deviceConnected);
      if (this.onConnectedChangedEvent == null)
        return;
      this.onConnectedChangedEvent(this, this.inputSource, deviceConnected);
    }

    private void OnTrackingChanged(
      SteamVR_Action_Skeleton fromAction,
      ETrackingResult trackingState)
    {
      if (this.onTrackingChanged != null)
        this.onTrackingChanged.Invoke(this, this.inputSource, trackingState);
      if (this.onTrackingChangedEvent == null)
        return;
      this.onTrackingChangedEvent(this, this.inputSource, trackingState);
    }

    protected virtual void SteamVR_Input_OnSkeletonsUpdated(bool skipSendingEvents) => this.UpdateSkeleton();

    protected virtual void UpdateSkeleton()
    {
      if ((SteamVR_Action) this.skeletonAction == (SteamVR_Action) null || !this.skeletonAction.active)
        return;
      if (this.updatePose)
        this.UpdatePose();
      if ((UnityEngine.Object) this.blendPoser != (UnityEngine.Object) null && (double) this.skeletonBlend < 1.0)
      {
        if (this.blendSnapshot == null)
          this.blendSnapshot = this.blendPoser.GetBlendedPose(this);
        this.blendSnapshot.CopyFrom(this.blendPoser.GetBlendedPose(this));
      }
      if (this.rangeOfMotionBlendRoutine != null)
        return;
      if (this.temporaryRangeOfMotion.HasValue)
        this.skeletonAction.SetRangeOfMotion(this.temporaryRangeOfMotion.Value);
      else
        this.skeletonAction.SetRangeOfMotion(this.rangeOfMotion);
      this.UpdateSkeletonTransforms();
    }

    public void SetTemporaryRangeOfMotion(
      EVRSkeletalMotionRange newRangeOfMotion,
      float blendOverSeconds = 0.1f)
    {
      if (this.rangeOfMotion == newRangeOfMotion)
      {
        EVRSkeletalMotionRange? temporaryRangeOfMotion = this.temporaryRangeOfMotion;
        if ((temporaryRangeOfMotion.GetValueOrDefault() != newRangeOfMotion ? 1 : (!temporaryRangeOfMotion.HasValue ? 1 : 0)) == 0)
          return;
      }
      this.TemporaryRangeOfMotionBlend(newRangeOfMotion, blendOverSeconds);
    }

    public void ResetTemporaryRangeOfMotion(float blendOverSeconds = 0.1f) => this.ResetTemporaryRangeOfMotionBlend(blendOverSeconds);

    public void SetRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
    {
      if (this.rangeOfMotion == newRangeOfMotion)
        return;
      this.RangeOfMotionBlend(newRangeOfMotion, blendOverSeconds);
    }

    public void BlendToSkeleton(float overTime = 0.1f)
    {
      this.blendSnapshot = this.blendPoser.GetBlendedPose(this);
      this.blendPoser = (SteamVR_Skeleton_Poser) null;
      this.BlendTo(1f, overTime);
    }

    public void BlendToPoser(SteamVR_Skeleton_Poser poser, float overTime = 0.1f)
    {
      if ((UnityEngine.Object) poser == (UnityEngine.Object) null)
        return;
      this.blendPoser = poser;
      this.BlendTo(0.0f, overTime);
    }

    public void BlendToAnimation(float overTime = 0.1f) => this.BlendTo(0.0f, overTime);

    public void BlendTo(float blendToAmount, float overTime)
    {
      if (this.blendRoutine != null)
        this.StopCoroutine(this.blendRoutine);
      if (!this.gameObject.activeInHierarchy)
        return;
      this.blendRoutine = this.StartCoroutine(this.DoBlendRoutine(blendToAmount, overTime));
    }

    [DebuggerHidden]
    protected IEnumerator DoBlendRoutine(float blendToAmount, float overTime) => (IEnumerator) new SteamVR_Behaviour_Skeleton.\u003CDoBlendRoutine\u003Ec__Iterator0()
    {
      overTime = overTime,
      blendToAmount = blendToAmount,
      \u0024this = this
    };

    protected void RangeOfMotionBlend(
      EVRSkeletalMotionRange newRangeOfMotion,
      float blendOverSeconds)
    {
      if (this.rangeOfMotionBlendRoutine != null)
        this.StopCoroutine(this.rangeOfMotionBlendRoutine);
      EVRSkeletalMotionRange rangeOfMotion = this.rangeOfMotion;
      this.rangeOfMotion = newRangeOfMotion;
      if (!this.gameObject.activeInHierarchy)
        return;
      this.rangeOfMotionBlendRoutine = this.StartCoroutine(this.DoRangeOfMotionBlend(rangeOfMotion, newRangeOfMotion, blendOverSeconds));
    }

    protected void TemporaryRangeOfMotionBlend(
      EVRSkeletalMotionRange newRangeOfMotion,
      float blendOverSeconds)
    {
      if (this.rangeOfMotionBlendRoutine != null)
        this.StopCoroutine(this.rangeOfMotionBlendRoutine);
      EVRSkeletalMotionRange rangeOfMotion = this.rangeOfMotion;
      if (this.temporaryRangeOfMotion.HasValue)
        rangeOfMotion = this.temporaryRangeOfMotion.Value;
      this.temporaryRangeOfMotion = new EVRSkeletalMotionRange?(newRangeOfMotion);
      if (!this.gameObject.activeInHierarchy)
        return;
      this.rangeOfMotionBlendRoutine = this.StartCoroutine(this.DoRangeOfMotionBlend(rangeOfMotion, newRangeOfMotion, blendOverSeconds));
    }

    protected void ResetTemporaryRangeOfMotionBlend(float blendOverSeconds)
    {
      if (!this.temporaryRangeOfMotion.HasValue)
        return;
      if (this.rangeOfMotionBlendRoutine != null)
        this.StopCoroutine(this.rangeOfMotionBlendRoutine);
      EVRSkeletalMotionRange oldRangeOfMotion = this.temporaryRangeOfMotion.Value;
      EVRSkeletalMotionRange rangeOfMotion = this.rangeOfMotion;
      this.temporaryRangeOfMotion = new EVRSkeletalMotionRange?();
      if (!this.gameObject.activeInHierarchy)
        return;
      this.rangeOfMotionBlendRoutine = this.StartCoroutine(this.DoRangeOfMotionBlend(oldRangeOfMotion, rangeOfMotion, blendOverSeconds));
    }

    [DebuggerHidden]
    protected IEnumerator DoRangeOfMotionBlend(
      EVRSkeletalMotionRange oldRangeOfMotion,
      EVRSkeletalMotionRange newRangeOfMotion,
      float overTime)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SteamVR_Behaviour_Skeleton.\u003CDoRangeOfMotionBlend\u003Ec__Iterator1()
      {
        overTime = overTime,
        oldRangeOfMotion = oldRangeOfMotion,
        newRangeOfMotion = newRangeOfMotion,
        \u0024this = this
      };
    }

    protected virtual void UpdateSkeletonTransforms()
    {
      this.CopyBonePositions(this.bonePositionBuffer);
      this.CopyBoneRotations(this.boneRotationBuffer);
      if ((double) this.skeletonBlend <= 0.0)
      {
        if ((UnityEngine.Object) this.blendPoser != (UnityEngine.Object) null)
        {
          SteamVR_Skeleton_Pose_Hand hand = this.blendPoser.skeletonMainPose.GetHand(this.inputSource);
          for (int boneIndex = 0; boneIndex < this.bones.Length; ++boneIndex)
          {
            if (!((UnityEngine.Object) this.bones[boneIndex] == (UnityEngine.Object) null))
            {
              if (boneIndex == 1 && hand.ignoreWristPoseData || boneIndex == 0 && hand.ignoreRootPoseData)
              {
                this.SetBonePosition(boneIndex, this.bonePositionBuffer[boneIndex]);
                this.SetBoneRotation(boneIndex, this.boneRotationBuffer[boneIndex]);
              }
              else
              {
                this.SetBonePosition(boneIndex, this.blendSnapshot.bonePositions[boneIndex]);
                this.SetBoneRotation(boneIndex, this.blendSnapshot.boneRotations[boneIndex]);
              }
            }
          }
        }
        else
        {
          for (int boneIndex = 0; boneIndex < this.bones.Length; ++boneIndex)
          {
            this.SetBonePosition(boneIndex, this.blendSnapshot.bonePositions[boneIndex]);
            this.SetBoneRotation(boneIndex, this.blendSnapshot.boneRotations[boneIndex]);
          }
        }
      }
      else if ((double) this.skeletonBlend >= 1.0)
      {
        for (int boneIndex = 0; boneIndex < this.bones.Length; ++boneIndex)
        {
          if (!((UnityEngine.Object) this.bones[boneIndex] == (UnityEngine.Object) null))
          {
            this.SetBonePosition(boneIndex, this.bonePositionBuffer[boneIndex]);
            this.SetBoneRotation(boneIndex, this.boneRotationBuffer[boneIndex]);
          }
        }
      }
      else
      {
        for (int boneIndex = 0; boneIndex < this.bones.Length; ++boneIndex)
        {
          if (!((UnityEngine.Object) this.bones[boneIndex] == (UnityEngine.Object) null))
          {
            if ((UnityEngine.Object) this.blendPoser != (UnityEngine.Object) null)
            {
              SteamVR_Skeleton_Pose_Hand hand = this.blendPoser.skeletonMainPose.GetHand(this.inputSource);
              if (boneIndex == 1 && hand.ignoreWristPoseData || boneIndex == 0 && hand.ignoreRootPoseData)
              {
                this.SetBonePosition(boneIndex, this.bonePositionBuffer[boneIndex]);
                this.SetBoneRotation(boneIndex, this.boneRotationBuffer[boneIndex]);
              }
              else
              {
                this.SetBonePosition(boneIndex, Vector3.Lerp(this.blendSnapshot.bonePositions[boneIndex], this.bonePositionBuffer[boneIndex], this.skeletonBlend));
                this.SetBoneRotation(boneIndex, Quaternion.Lerp(this.blendSnapshot.boneRotations[boneIndex], this.boneRotationBuffer[boneIndex], this.skeletonBlend));
              }
            }
            else if (this.blendSnapshot == null)
            {
              this.SetBonePosition(boneIndex, Vector3.Lerp(this.bones[boneIndex].localPosition, this.bonePositionBuffer[boneIndex], this.skeletonBlend));
              this.SetBoneRotation(boneIndex, Quaternion.Lerp(this.bones[boneIndex].localRotation, this.boneRotationBuffer[boneIndex], this.skeletonBlend));
            }
            else
            {
              this.SetBonePosition(boneIndex, Vector3.Lerp(this.blendSnapshot.bonePositions[boneIndex], this.bonePositionBuffer[boneIndex], this.skeletonBlend));
              this.SetBoneRotation(boneIndex, Quaternion.Lerp(this.blendSnapshot.boneRotations[boneIndex], this.boneRotationBuffer[boneIndex], this.skeletonBlend));
            }
          }
        }
      }
      if (this.onBoneTransformsUpdated != null)
        this.onBoneTransformsUpdated.Invoke(this, this.inputSource);
      if (this.onBoneTransformsUpdatedEvent == null)
        return;
      this.onBoneTransformsUpdatedEvent(this, this.inputSource);
    }

    protected virtual void SetBonePosition(int boneIndex, Vector3 localPosition)
    {
      if (this.onlySetRotations)
        return;
      this.bones[boneIndex].localPosition = localPosition;
    }

    protected virtual void SetBoneRotation(int boneIndex, Quaternion localRotation) => this.bones[boneIndex].localRotation = localRotation;

    public virtual Transform GetBone(int joint)
    {
      if (this.bones == null || this.bones.Length == 0)
        this.Awake();
      return this.bones[joint];
    }

    public Vector3 GetBonePosition(int joint, bool local = false) => local ? this.bones[joint].localPosition : this.bones[joint].position;

    public Quaternion GetBoneRotation(int joint, bool local = false) => local ? this.bones[joint].localRotation : this.bones[joint].rotation;

    protected void CopyBonePositions(Vector3[] positionBuffer)
    {
      Vector3[] bonePositions = this.skeletonAction.GetBonePositions();
      if (this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.LeftToRight || this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
      {
        for (int boneIndex = 0; boneIndex < positionBuffer.Length; ++boneIndex)
          SteamVR_Behaviour_Skeleton.MirrorBonePosition(ref bonePositions[boneIndex], ref positionBuffer[boneIndex], boneIndex);
      }
      else
        bonePositions.CopyTo((Array) positionBuffer, 0);
    }

    protected void CopyBoneRotations(Quaternion[] rotationBuffer)
    {
      Quaternion[] boneRotations = this.skeletonAction.GetBoneRotations();
      if (this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.LeftToRight || this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
      {
        for (int boneIndex = 0; boneIndex < rotationBuffer.Length; ++boneIndex)
          SteamVR_Behaviour_Skeleton.MirrorBoneRotation(ref boneRotations[boneIndex], ref rotationBuffer[boneIndex], boneIndex);
      }
      else
        boneRotations.CopyTo((Array) rotationBuffer, 0);
    }

    public static void MirrorBonePosition(ref Vector3 source, ref Vector3 dest, int boneIndex)
    {
      if (boneIndex == 1 || SteamVR_Behaviour_Skeleton.IsMetacarpal(boneIndex))
      {
        dest.x = -source.x;
        dest.y = source.y;
        dest.z = source.z;
      }
      else if (boneIndex != 0)
      {
        dest.x = -source.x;
        dest.y = -source.y;
        dest.z = -source.z;
      }
      else
        dest = source;
    }

    public static void MirrorBoneRotation(
      ref Quaternion source,
      ref Quaternion dest,
      int boneIndex)
    {
      if (boneIndex == 1)
      {
        dest.x = source.x;
        dest.y = source.y * -1f;
        dest.z = source.z * -1f;
        dest.w = source.w;
      }
      else if (SteamVR_Behaviour_Skeleton.IsMetacarpal(boneIndex))
        dest = SteamVR_Behaviour_Skeleton.rightFlipAngle * source;
      else
        dest = source;
    }

    protected virtual void UpdatePose()
    {
      if ((SteamVR_Action) this.skeletonAction == (SteamVR_Action) null)
        return;
      Vector3 position = this.skeletonAction.GetLocalPosition();
      Quaternion quaternion = this.skeletonAction.GetLocalRotation();
      if ((UnityEngine.Object) this.origin == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null)
        {
          position = this.transform.parent.TransformPoint(position);
          quaternion = this.transform.parent.rotation * quaternion;
        }
      }
      else
      {
        position = this.origin.TransformPoint(position);
        quaternion = this.origin.rotation * quaternion;
      }
      if (this.skeletonAction.poseChanged)
      {
        if (this.onTransformChanged != null)
          this.onTransformChanged.Invoke(this, this.inputSource);
        if (this.onTransformChangedEvent != null)
          this.onTransformChangedEvent(this, this.inputSource);
      }
      this.transform.position = position;
      this.transform.rotation = quaternion;
      if (this.onTransformUpdated == null)
        return;
      this.onTransformUpdated.Invoke(this, this.inputSource);
    }

    public void ForceToReferencePose(EVRSkeletalReferencePose referencePose)
    {
      bool flag = false;
      if (Application.isEditor && !Application.isPlaying)
      {
        flag = SteamVR.InitializeTemporarySession(true);
        this.Awake();
        this.skeletonAction.actionSet.Activate(SteamVR_Input_Sources.Any, 0, false);
        SteamVR_ActionSet_Manager.UpdateActionStates(true);
        this.skeletonAction.UpdateValueWithoutEvents();
      }
      if (!this.skeletonAction.active)
      {
        UnityEngine.Debug.LogError((object) ("<b>[SteamVR Input]</b> Please turn on your " + this.inputSource.ToString() + " controller and ensure SteamVR is open."));
      }
      else
      {
        SteamVR_Utils.RigidTransform[] referenceTransforms = this.skeletonAction.GetReferenceTransforms(EVRSkeletalTransformSpace.Parent, referencePose);
        if (referenceTransforms == null || referenceTransforms.Length == 0)
          UnityEngine.Debug.LogError((object) ("<b>[SteamVR Input]</b> Unable to get the reference transform for " + this.inputSource.ToString() + ". Please make sure SteamVR is open and both controllers are connected."));
        for (int index = 0; index < referenceTransforms.Length; ++index)
        {
          this.bones[index].localPosition = referenceTransforms[index].pos;
          this.bones[index].localRotation = referenceTransforms[index].rot;
        }
        if (!flag)
          return;
        SteamVR.ExitTemporarySession();
      }
    }

    protected static bool IsMetacarpal(int boneIndex) => boneIndex == 6 || boneIndex == 11 || (boneIndex == 16 || boneIndex == 21) || boneIndex == 2;

    public enum MirrorType
    {
      None,
      LeftToRight,
      RightToLeft,
    }

    public delegate void ActiveChangeHandler(
      SteamVR_Behaviour_Skeleton fromAction,
      SteamVR_Input_Sources inputSource,
      bool active);

    public delegate void ChangeHandler(
      SteamVR_Behaviour_Skeleton fromAction,
      SteamVR_Input_Sources inputSource);

    public delegate void UpdateHandler(
      SteamVR_Behaviour_Skeleton fromAction,
      SteamVR_Input_Sources inputSource);

    public delegate void TrackingChangeHandler(
      SteamVR_Behaviour_Skeleton fromAction,
      SteamVR_Input_Sources inputSource,
      ETrackingResult trackingState);

    public delegate void ValidPoseChangeHandler(
      SteamVR_Behaviour_Skeleton fromAction,
      SteamVR_Input_Sources inputSource,
      bool validPose);

    public delegate void DeviceConnectedChangeHandler(
      SteamVR_Behaviour_Skeleton fromAction,
      SteamVR_Input_Sources inputSource,
      bool deviceConnected);
  }
}
