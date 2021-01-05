// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skeleton_Pose_Hand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Skeleton_Pose_Hand
  {
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Skeleton_FingerExtensionTypes thumbFingerMovementType;
    public SteamVR_Skeleton_FingerExtensionTypes indexFingerMovementType;
    public SteamVR_Skeleton_FingerExtensionTypes middleFingerMovementType;
    public SteamVR_Skeleton_FingerExtensionTypes ringFingerMovementType;
    public SteamVR_Skeleton_FingerExtensionTypes pinkyFingerMovementType;
    public bool ignoreRootPoseData = true;
    public bool ignoreWristPoseData = true;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3[] bonePositions;
    public Quaternion[] boneRotations;

    public SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources source) => this.inputSource = source;

    public SteamVR_Skeleton_FingerExtensionTypes GetFingerExtensionType(
      int finger)
    {
      switch (finger)
      {
        case 0:
          return this.thumbFingerMovementType;
        case 1:
          return this.indexFingerMovementType;
        case 2:
          return this.middleFingerMovementType;
        case 3:
          return this.ringFingerMovementType;
        case 4:
          return this.pinkyFingerMovementType;
        default:
          Debug.LogWarning((object) "Finger not in range!");
          return SteamVR_Skeleton_FingerExtensionTypes.Static;
      }
    }

    public SteamVR_Skeleton_FingerExtensionTypes GetMovementTypeForBone(
      int boneIndex)
    {
      switch (SteamVR_Skeleton_JointIndexes.GetFingerForBone(boneIndex))
      {
        case 0:
          return this.thumbFingerMovementType;
        case 1:
          return this.indexFingerMovementType;
        case 2:
          return this.middleFingerMovementType;
        case 3:
          return this.ringFingerMovementType;
        case 4:
          return this.pinkyFingerMovementType;
        default:
          return SteamVR_Skeleton_FingerExtensionTypes.Static;
      }
    }
  }
}
