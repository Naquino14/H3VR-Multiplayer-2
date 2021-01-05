// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawnDef_Discs
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Disc Spawn Def", menuName = "OmniSequencer/SpawnDef/Disc Spawn Definition", order = 0)]
  public class OmniSpawnDef_Discs : OmniSpawnDef
  {
    public List<OmniSpawnDef_Discs.DiscType> Discs;
    public OmniSpawnDef_Discs.DiscSpawnStyle SpawnStyle;
    public OmniSpawnDef_Discs.DiscSpawnPattern SpawnPattern;
    public OmniSpawnDef_Discs.DiscZConfig ZConfig;
    public OmniSpawnDef_Discs.DiscSpawnOrdering SpawnOrdering;
    public OmniSpawnDef_Discs.DiscMovementPattern MovementPattern;
    public OmniSpawnDef_Discs.DiscMovementStyle MovementStyle;
    public float TimeBetweenDiscSpawns = 0.1f;
    public float DiscMovementSpeed = 1f;

    public override List<int> CalculateSpawnerScoreThresholds()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index = 0; index < this.Discs.Count; ++index)
      {
        switch (this.Discs[index])
        {
          case OmniSpawnDef_Discs.DiscType.Normal:
            List<int> intList2;
            (intList2 = intList1)[0] = intList2[0] + 100;
            List<int> intList3;
            (intList3 = intList1)[1] = intList3[1] + 75;
            List<int> intList4;
            (intList4 = intList1)[2] = intList4[2] + 50;
            break;
          case OmniSpawnDef_Discs.DiscType.Armored:
            List<int> intList5;
            (intList5 = intList1)[0] = intList5[0] + 1000;
            List<int> intList6;
            (intList6 = intList1)[1] = intList6[1] + 750;
            List<int> intList7;
            (intList7 = intList1)[2] = intList7[2] + 500;
            break;
          case OmniSpawnDef_Discs.DiscType.RedRing:
            List<int> intList8;
            (intList8 = intList1)[0] = intList8[0] + 100;
            List<int> intList9;
            (intList9 = intList1)[1] = intList9[1] + 75;
            List<int> intList10;
            (intList10 = intList1)[2] = intList10[2] + 50;
            break;
          case OmniSpawnDef_Discs.DiscType.Bullseye:
            List<int> intList11;
            (intList11 = intList1)[0] = intList11[0] + 200;
            List<int> intList12;
            (intList12 = intList1)[1] = intList12[1] + 100;
            List<int> intList13;
            (intList13 = intList1)[2] = intList13[2] + 50;
            break;
        }
      }
      return intList1;
    }

    public enum DiscType
    {
      Normal,
      NoShoot,
      Armored,
      RedRing,
      Bullseye,
    }

    public enum DiscSpawnStyle
    {
      AllAtOnce = -1, // 0xFFFFFFFF
      Sequential = 0,
      OnHit = 1,
    }

    public enum DiscSpawnPattern
    {
      Circle,
      Square,
      LineXCentered,
      LineYCentered,
      LineXUp,
      LineXDown,
      LineYLeft,
      LineYRight,
      DiagonalLeftUp,
      DiagonalRightUp,
      DiagonalLeftDown,
      DiagonalRightDown,
      SpiralCounterclockwise,
      SpiralClockwise,
    }

    public enum DiscZConfig
    {
      Homogenous,
      Incremented,
    }

    public enum DiscSpawnOrdering
    {
      InOrder,
      Random,
    }

    public enum DiscMovementPattern
    {
      Static,
      OscillateX,
      OscillateY,
      OscillateZ,
      OscillateXY,
      ClockwiseRot,
      CounterClockwiseRot,
    }

    public enum DiscMovementStyle
    {
      Linear,
      Sinusoidal,
      Rotational,
      RotationalSwell,
    }
  }
}
