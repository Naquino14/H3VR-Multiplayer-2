// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skeleton_HandMask
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Skeleton_HandMask
  {
    public bool palm;
    public bool thumb;
    public bool index;
    public bool middle;
    public bool ring;
    public bool pinky;
    public bool[] values = new bool[6];
    public static readonly SteamVR_Skeleton_HandMask fullMask = new SteamVR_Skeleton_HandMask();

    public SteamVR_Skeleton_HandMask()
    {
      this.values = new bool[6];
      this.Reset();
    }

    public void SetFinger(int i, bool value)
    {
      this.values[i] = value;
      this.Apply();
    }

    public bool GetFinger(int i) => this.values[i];

    public void Reset()
    {
      this.values = new bool[6];
      for (int index = 0; index < 6; ++index)
        this.values[index] = true;
      this.Apply();
    }

    protected void Apply()
    {
      this.palm = this.values[0];
      this.thumb = this.values[1];
      this.index = this.values[2];
      this.middle = this.values[3];
      this.ring = this.values[4];
      this.pinky = this.values[5];
    }
  }
}
