// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skeleton_Poser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Skeleton_Poser : MonoBehaviour
  {
    public bool poseEditorExpanded = true;
    public bool blendEditorExpanded = true;
    public string[] poseNames;
    public GameObject previewLeftHandPrefab;
    public GameObject previewRightHandPrefab;
    public SteamVR_Skeleton_Pose skeletonMainPose;
    public List<SteamVR_Skeleton_Pose> skeletonAdditionalPoses = new List<SteamVR_Skeleton_Pose>();
    [SerializeField]
    protected bool showLeftPreview;
    [SerializeField]
    protected bool showRightPreview = true;
    [SerializeField]
    protected GameObject previewLeftInstance;
    [SerializeField]
    protected GameObject previewRightInstance;
    [SerializeField]
    protected int previewPoseSelection;
    public List<SteamVR_Skeleton_Poser.PoseBlendingBehaviour> blendingBehaviours = new List<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>();
    public SteamVR_Skeleton_PoseSnapshot blendedSnapshotL;
    public SteamVR_Skeleton_PoseSnapshot blendedSnapshotR;
    private SteamVR_Skeleton_Poser.SkeletonBlendablePose[] blendPoses;
    private int boneCount;
    private bool poseUpdatedThisFrame;
    public float scale;

    public int blendPoseCount => this.blendPoses.Length;

    protected void Awake()
    {
      if ((UnityEngine.Object) this.previewLeftInstance != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.previewLeftInstance);
      if ((UnityEngine.Object) this.previewRightInstance != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.previewRightInstance);
      this.blendPoses = new SteamVR_Skeleton_Poser.SkeletonBlendablePose[this.skeletonAdditionalPoses.Count + 1];
      for (int index = 0; index < this.blendPoseCount; ++index)
      {
        this.blendPoses[index] = new SteamVR_Skeleton_Poser.SkeletonBlendablePose(this.GetPoseByIndex(index));
        this.blendPoses[index].PoseToSnapshots();
      }
      this.boneCount = this.skeletonMainPose.leftHand.bonePositions.Length;
      this.blendedSnapshotL = new SteamVR_Skeleton_PoseSnapshot(this.boneCount, SteamVR_Input_Sources.LeftHand);
      this.blendedSnapshotR = new SteamVR_Skeleton_PoseSnapshot(this.boneCount, SteamVR_Input_Sources.RightHand);
    }

    public void SetBlendingBehaviourValue(string behaviourName, float value)
    {
      SteamVR_Skeleton_Poser.PoseBlendingBehaviour blendingBehaviour = this.blendingBehaviours.Find((Predicate<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>) (b => b.name == behaviourName));
      if (blendingBehaviour == null)
      {
        Debug.LogError((object) ("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + this.gameObject.name));
      }
      else
      {
        if (blendingBehaviour.type != SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes.Manual)
          Debug.LogWarning((object) ("[SteamVR] Blending Behaviour: " + behaviourName + " is not a manual behaviour. Its value will likely be overriden."));
        blendingBehaviour.value = value;
      }
    }

    public float GetBlendingBehaviourValue(string behaviourName)
    {
      SteamVR_Skeleton_Poser.PoseBlendingBehaviour blendingBehaviour = this.blendingBehaviours.Find((Predicate<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>) (b => b.name == behaviourName));
      if (blendingBehaviour != null)
        return blendingBehaviour.value;
      Debug.LogError((object) ("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + this.gameObject.name));
      return 0.0f;
    }

    public void SetBlendingBehaviourEnabled(string behaviourName, bool value)
    {
      SteamVR_Skeleton_Poser.PoseBlendingBehaviour blendingBehaviour = this.blendingBehaviours.Find((Predicate<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>) (b => b.name == behaviourName));
      if (blendingBehaviour == null)
        Debug.LogError((object) ("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + this.gameObject.name));
      else
        blendingBehaviour.enabled = value;
    }

    public bool GetBlendingBehaviourEnabled(string behaviourName)
    {
      SteamVR_Skeleton_Poser.PoseBlendingBehaviour blendingBehaviour = this.blendingBehaviours.Find((Predicate<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>) (b => b.name == behaviourName));
      if (blendingBehaviour != null)
        return blendingBehaviour.enabled;
      Debug.LogError((object) ("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + this.gameObject.name));
      return false;
    }

    public SteamVR_Skeleton_Poser.PoseBlendingBehaviour GetBlendingBehaviour(
      string behaviourName)
    {
      SteamVR_Skeleton_Poser.PoseBlendingBehaviour blendingBehaviour = this.blendingBehaviours.Find((Predicate<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>) (b => b.name == behaviourName));
      if (blendingBehaviour != null)
        return blendingBehaviour;
      Debug.LogError((object) ("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + this.gameObject.name));
      return (SteamVR_Skeleton_Poser.PoseBlendingBehaviour) null;
    }

    public SteamVR_Skeleton_Pose GetPoseByIndex(int index) => index == 0 ? this.skeletonMainPose : this.skeletonAdditionalPoses[index - 1];

    private SteamVR_Skeleton_PoseSnapshot GetHandSnapshot(
      SteamVR_Input_Sources inputSource)
    {
      return inputSource == SteamVR_Input_Sources.LeftHand ? this.blendedSnapshotL : this.blendedSnapshotR;
    }

    public SteamVR_Skeleton_PoseSnapshot GetBlendedPose(
      SteamVR_Action_Skeleton skeletonAction,
      SteamVR_Input_Sources handType)
    {
      this.UpdatePose(skeletonAction, handType);
      return this.GetHandSnapshot(handType);
    }

    public SteamVR_Skeleton_PoseSnapshot GetBlendedPose(
      SteamVR_Behaviour_Skeleton skeletonBehaviour)
    {
      return this.GetBlendedPose(skeletonBehaviour.skeletonAction, skeletonBehaviour.inputSource);
    }

    public void UpdatePose(
      SteamVR_Action_Skeleton skeletonAction,
      SteamVR_Input_Sources inputSource)
    {
      if (this.poseUpdatedThisFrame)
        return;
      this.poseUpdatedThisFrame = true;
      this.blendPoses[0].UpdateAdditiveAnimation(skeletonAction, inputSource);
      SteamVR_Skeleton_PoseSnapshot handSnapshot = this.GetHandSnapshot(inputSource);
      handSnapshot.CopyFrom(this.blendPoses[0].GetHandSnapshot(inputSource));
      this.ApplyBlenderBehaviours(skeletonAction, inputSource, handSnapshot);
      if (inputSource == SteamVR_Input_Sources.RightHand)
      {
        this.blendedSnapshotR = handSnapshot;
      }
      else
      {
        if (inputSource != SteamVR_Input_Sources.LeftHand)
          return;
        this.blendedSnapshotL = handSnapshot;
      }
    }

    protected void ApplyBlenderBehaviours(
      SteamVR_Action_Skeleton skeletonAction,
      SteamVR_Input_Sources inputSource,
      SteamVR_Skeleton_PoseSnapshot snapshot)
    {
      for (int index = 0; index < this.blendingBehaviours.Count; ++index)
      {
        this.blendingBehaviours[index].Update(Time.deltaTime, inputSource);
        if (this.blendingBehaviours[index].enabled && (double) this.blendingBehaviours[index].influence * (double) this.blendingBehaviours[index].value > 0.00999999977648258)
        {
          if (this.blendingBehaviours[index].pose != 0)
            this.blendPoses[this.blendingBehaviours[index].pose].UpdateAdditiveAnimation(skeletonAction, inputSource);
          this.blendingBehaviours[index].ApplyBlending(snapshot, this.blendPoses, inputSource);
        }
      }
    }

    protected void LateUpdate() => this.poseUpdatedThisFrame = false;

    protected Vector3 BlendVectors(Vector3[] vectors, float[] weights)
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < vectors.Length; ++index)
        zero += vectors[index] * weights[index];
      return zero;
    }

    protected Quaternion BlendQuaternions(Quaternion[] quaternions, float[] weights)
    {
      Quaternion identity = Quaternion.identity;
      for (int index = 0; index < quaternions.Length; ++index)
        identity *= Quaternion.Slerp(Quaternion.identity, quaternions[index], weights[index]);
      return identity;
    }

    public class SkeletonBlendablePose
    {
      public SteamVR_Skeleton_Pose pose;
      public SteamVR_Skeleton_PoseSnapshot snapshotR;
      public SteamVR_Skeleton_PoseSnapshot snapshotL;
      private Vector3[] additivePositionBuffer;
      private Quaternion[] additiveRotationBuffer;

      public SkeletonBlendablePose(SteamVR_Skeleton_Pose p)
      {
        this.pose = p;
        this.snapshotR = new SteamVR_Skeleton_PoseSnapshot(p.rightHand.bonePositions.Length, SteamVR_Input_Sources.RightHand);
        this.snapshotL = new SteamVR_Skeleton_PoseSnapshot(p.leftHand.bonePositions.Length, SteamVR_Input_Sources.LeftHand);
      }

      public SkeletonBlendablePose()
      {
      }

      public SteamVR_Skeleton_PoseSnapshot GetHandSnapshot(
        SteamVR_Input_Sources inputSource)
      {
        return inputSource == SteamVR_Input_Sources.LeftHand ? this.snapshotL : this.snapshotR;
      }

      public void UpdateAdditiveAnimation(
        SteamVR_Action_Skeleton skeletonAction,
        SteamVR_Input_Sources inputSource)
      {
        SteamVR_Skeleton_PoseSnapshot handSnapshot = this.GetHandSnapshot(inputSource);
        SteamVR_Skeleton_Pose_Hand hand = this.pose.GetHand(inputSource);
        if (this.additivePositionBuffer == null)
          this.additivePositionBuffer = new Vector3[skeletonAction.boneCount];
        if (this.additiveRotationBuffer == null)
          this.additiveRotationBuffer = new Quaternion[skeletonAction.boneCount];
        for (int boneIndex = 0; boneIndex < this.snapshotL.bonePositions.Length; ++boneIndex)
        {
          int fingerForBone = SteamVR_Skeleton_JointIndexes.GetFingerForBone(boneIndex);
          SteamVR_Skeleton_FingerExtensionTypes movementTypeForBone = hand.GetMovementTypeForBone(boneIndex);
          if (inputSource == SteamVR_Input_Sources.LeftHand)
          {
            SteamVR_Behaviour_Skeleton.MirrorBonePosition(ref skeletonAction.bonePositions[boneIndex], ref this.additivePositionBuffer[boneIndex], boneIndex);
            SteamVR_Behaviour_Skeleton.MirrorBoneRotation(ref skeletonAction.boneRotations[boneIndex], ref this.additiveRotationBuffer[boneIndex], boneIndex);
          }
          else
          {
            this.additivePositionBuffer[boneIndex] = skeletonAction.bonePositions[boneIndex];
            this.additiveRotationBuffer[boneIndex] = skeletonAction.boneRotations[boneIndex];
          }
          switch (movementTypeForBone)
          {
            case SteamVR_Skeleton_FingerExtensionTypes.Free:
              handSnapshot.bonePositions[boneIndex] = this.additivePositionBuffer[boneIndex];
              handSnapshot.boneRotations[boneIndex] = this.additiveRotationBuffer[boneIndex];
              break;
            case SteamVR_Skeleton_FingerExtensionTypes.Extend:
              handSnapshot.bonePositions[boneIndex] = Vector3.Lerp(hand.bonePositions[boneIndex], this.additivePositionBuffer[boneIndex], 1f - skeletonAction.fingerCurls[fingerForBone]);
              handSnapshot.boneRotations[boneIndex] = Quaternion.Lerp(hand.boneRotations[boneIndex], this.additiveRotationBuffer[boneIndex], 1f - skeletonAction.fingerCurls[fingerForBone]);
              break;
            case SteamVR_Skeleton_FingerExtensionTypes.Contract:
              handSnapshot.bonePositions[boneIndex] = Vector3.Lerp(hand.bonePositions[boneIndex], this.additivePositionBuffer[boneIndex], skeletonAction.fingerCurls[fingerForBone]);
              handSnapshot.boneRotations[boneIndex] = Quaternion.Lerp(hand.boneRotations[boneIndex], this.additiveRotationBuffer[boneIndex], skeletonAction.fingerCurls[fingerForBone]);
              break;
          }
        }
      }

      public void PoseToSnapshots()
      {
        this.snapshotR.position = this.pose.rightHand.position;
        this.snapshotR.rotation = this.pose.rightHand.rotation;
        this.pose.rightHand.bonePositions.CopyTo((Array) this.snapshotR.bonePositions, 0);
        this.pose.rightHand.boneRotations.CopyTo((Array) this.snapshotR.boneRotations, 0);
        this.snapshotL.position = this.pose.leftHand.position;
        this.snapshotL.rotation = this.pose.leftHand.rotation;
        this.pose.leftHand.bonePositions.CopyTo((Array) this.snapshotL.bonePositions, 0);
        this.pose.leftHand.boneRotations.CopyTo((Array) this.snapshotL.boneRotations, 0);
      }
    }

    [Serializable]
    public class PoseBlendingBehaviour
    {
      public string name;
      public bool enabled = true;
      public float influence = 1f;
      public int pose = 1;
      public float value;
      public SteamVR_Action_Single action_single;
      public SteamVR_Action_Boolean action_bool;
      public float smoothingSpeed;
      public SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes type;
      public bool useMask;
      public SteamVR_Skeleton_HandMask mask = new SteamVR_Skeleton_HandMask();
      public bool previewEnabled;

      public PoseBlendingBehaviour()
      {
        this.enabled = true;
        this.influence = 1f;
      }

      public void Update(float deltaTime, SteamVR_Input_Sources inputSource)
      {
        if (this.type == SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes.AnalogAction)
          this.value = (double) this.smoothingSpeed != 0.0 ? Mathf.Lerp(this.value, this.action_single.GetAxis(inputSource), deltaTime * this.smoothingSpeed) : this.action_single.GetAxis(inputSource);
        if (this.type != SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes.BooleanAction)
          return;
        if ((double) this.smoothingSpeed == 0.0)
          this.value = !this.action_bool.GetState(inputSource) ? 0.0f : 1f;
        else
          this.value = Mathf.Lerp(this.value, !this.action_bool.GetState(inputSource) ? 0.0f : 1f, deltaTime * this.smoothingSpeed);
      }

      public void ApplyBlending(
        SteamVR_Skeleton_PoseSnapshot snapshot,
        SteamVR_Skeleton_Poser.SkeletonBlendablePose[] blendPoses,
        SteamVR_Input_Sources inputSource)
      {
        SteamVR_Skeleton_PoseSnapshot handSnapshot = blendPoses[this.pose].GetHandSnapshot(inputSource);
        if (this.mask.GetFinger(0) || !this.useMask)
        {
          snapshot.position = Vector3.Lerp(snapshot.position, handSnapshot.position, this.influence * this.value);
          snapshot.rotation = Quaternion.Slerp(snapshot.rotation, handSnapshot.rotation, this.influence * this.value);
        }
        for (int boneIndex = 0; boneIndex < snapshot.bonePositions.Length; ++boneIndex)
        {
          if (this.mask.GetFinger(SteamVR_Skeleton_JointIndexes.GetFingerForBone(boneIndex) + 1) || !this.useMask)
          {
            snapshot.bonePositions[boneIndex] = Vector3.Lerp(snapshot.bonePositions[boneIndex], handSnapshot.bonePositions[boneIndex], this.influence * this.value);
            snapshot.boneRotations[boneIndex] = Quaternion.Slerp(snapshot.boneRotations[boneIndex], handSnapshot.boneRotations[boneIndex], this.influence * this.value);
          }
        }
      }

      public enum BlenderTypes
      {
        Manual,
        AnalogAction,
        BooleanAction,
      }
    }
  }
}
