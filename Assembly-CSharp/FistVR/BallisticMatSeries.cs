// Decompiled with JetBrains decompiler
// Type: FistVR.BallisticMatSeries
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class BallisticMatSeries
  {
    public MatBallisticType MaterialType;
    public float PenThreshold;
    public float ShatterThreshold;
    public float Absorption;
    public float RicochetLimit;
    public float MinAngularAbsord;
    public float Roughness;
    public bool StopsOnPen;
    public bool DownGradesOnPen;
    public BallisticProjectileType DownGradesTo;
  }
}
