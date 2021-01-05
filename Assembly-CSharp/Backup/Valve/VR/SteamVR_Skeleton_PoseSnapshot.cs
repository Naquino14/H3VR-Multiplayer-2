// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skeleton_PoseSnapshot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Skeleton_PoseSnapshot
  {
    public SteamVR_Input_Sources inputSource;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3[] bonePositions;
    public Quaternion[] boneRotations;

    public SteamVR_Skeleton_PoseSnapshot(int boneCount, SteamVR_Input_Sources source)
    {
      this.inputSource = source;
      this.bonePositions = new Vector3[boneCount];
      this.boneRotations = new Quaternion[boneCount];
      this.position = Vector3.zero;
      this.rotation = Quaternion.identity;
    }

    public void CopyFrom(SteamVR_Skeleton_PoseSnapshot source)
    {
      this.inputSource = source.inputSource;
      this.position = source.position;
      this.rotation = source.rotation;
      for (int index = 0; index < this.bonePositions.Length; ++index)
      {
        this.bonePositions[index] = source.bonePositions[index];
        this.boneRotations[index] = source.boneRotations[index];
      }
    }
  }
}
