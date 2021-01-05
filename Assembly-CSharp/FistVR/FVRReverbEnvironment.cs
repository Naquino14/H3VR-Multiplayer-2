// Decompiled with JetBrains decompiler
// Type: FistVR.FVRReverbEnvironment
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRReverbEnvironment : MonoBehaviour
  {
    public FVRSoundEnvironment Environment;
    public Vector3 Size;
    public int Priority;

    public void Awake() => this.Size = this.transform.localScale * 2f;

    public void SetPriorityBasedOnType()
    {
      if (this.Environment == FVRSoundEnvironment.InsideNarrowSmall)
        this.Priority = 0;
      else if (this.Environment == FVRSoundEnvironment.InsideSmall)
        this.Priority = 1;
      else if (this.Environment == FVRSoundEnvironment.InsideNarrow)
        this.Priority = 2;
      else if (this.Environment == FVRSoundEnvironment.InsideMedium)
        this.Priority = 3;
      else if (this.Environment == FVRSoundEnvironment.InsideLarge)
        this.Priority = 4;
      else if (this.Environment == FVRSoundEnvironment.InsideLargeHighCeiling)
        this.Priority = 5;
      else if (this.Environment == FVRSoundEnvironment.ShootingRange)
        this.Priority = 6;
      else if (this.Environment == FVRSoundEnvironment.SniperRange)
        this.Priority = 7;
      else if (this.Environment == FVRSoundEnvironment.InsideWarehouseSmall)
        this.Priority = 8;
      else if (this.Environment == FVRSoundEnvironment.InsideWarehouse)
        this.Priority = 9;
      else if (this.Environment == FVRSoundEnvironment.OutsideEnclosedNarrow)
        this.Priority = 10;
      else if (this.Environment == FVRSoundEnvironment.OutsideEnclosed)
        this.Priority = 11;
      else if (this.Environment == FVRSoundEnvironment.Forest)
      {
        this.Priority = 12;
      }
      else
      {
        if (this.Environment != FVRSoundEnvironment.OutsideOpen)
          return;
        this.Priority = 13;
      }
    }
  }
}
