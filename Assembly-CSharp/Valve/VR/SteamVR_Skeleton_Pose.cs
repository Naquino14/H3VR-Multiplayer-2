// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skeleton_Pose
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Skeleton_Pose : ScriptableObject
  {
    public SteamVR_Skeleton_Pose_Hand leftHand = new SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources.LeftHand);
    public SteamVR_Skeleton_Pose_Hand rightHand = new SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources.RightHand);
    protected const int leftHandInputSource = 1;
    protected const int rightHandInputSource = 2;

    public SteamVR_Skeleton_Pose_Hand GetHand(int hand)
    {
      if (hand == 1)
        return this.leftHand;
      return hand == 2 ? this.rightHand : (SteamVR_Skeleton_Pose_Hand) null;
    }

    public SteamVR_Skeleton_Pose_Hand GetHand(SteamVR_Input_Sources hand)
    {
      if (hand == SteamVR_Input_Sources.LeftHand)
        return this.leftHand;
      return hand == SteamVR_Input_Sources.RightHand ? this.rightHand : (SteamVR_Skeleton_Pose_Hand) null;
    }
  }
}
