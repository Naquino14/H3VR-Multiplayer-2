// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_Skeleton_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public interface ISteamVR_Action_Skeleton_Source
  {
    EVRSkeletalTrackingLevel skeletalTrackingLevel { get; }

    Vector3[] bonePositions { get; }

    Quaternion[] boneRotations { get; }

    Vector3[] lastBonePositions { get; }

    Quaternion[] lastBoneRotations { get; }

    EVRSkeletalMotionRange rangeOfMotion { get; set; }

    EVRSkeletalTransformSpace skeletalTransformSpace { get; set; }

    bool onlyUpdateSummaryData { get; set; }

    float thumbCurl { get; }

    float indexCurl { get; }

    float middleCurl { get; }

    float ringCurl { get; }

    float pinkyCurl { get; }

    float thumbIndexSplay { get; }

    float indexMiddleSplay { get; }

    float middleRingSplay { get; }

    float ringPinkySplay { get; }

    float lastThumbCurl { get; }

    float lastIndexCurl { get; }

    float lastMiddleCurl { get; }

    float lastRingCurl { get; }

    float lastPinkyCurl { get; }

    float lastThumbIndexSplay { get; }

    float lastIndexMiddleSplay { get; }

    float lastMiddleRingSplay { get; }

    float lastRingPinkySplay { get; }

    float[] fingerCurls { get; }

    float[] fingerSplays { get; }

    float[] lastFingerCurls { get; }

    float[] lastFingerSplays { get; }
  }
}
