// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawnDef_Grid
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Grid Spawn Def", menuName = "OmniSequencer/SpawnDef/Grid Spawn Definition", order = 0)]
  public class OmniSpawnDef_Grid : OmniSpawnDef
  {
    public OmniSpawnDef_Grid.GridSize Size;
    public OmniSpawnDef_Grid.GridConfiguration Configuration;
    public List<OmniSpawnDef_Grid.GridInstruction> Instructions;

    public override List<int> CalculateSpawnerScoreThresholds()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index = 0; index < this.Instructions.Count; ++index)
      {
        int num = 1;
        switch (this.Instructions[index])
        {
          case OmniSpawnDef_Grid.GridInstruction.CountUp:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 9;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 16;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 25;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.CountDown:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 9;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 16;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 25;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.Addition:
            num = 3;
            break;
          case OmniSpawnDef_Grid.GridInstruction.Subtraction:
            num = 3;
            break;
          case OmniSpawnDef_Grid.GridInstruction.Multiplication:
            num = 3;
            break;
          case OmniSpawnDef_Grid.GridInstruction.Division:
            num = 3;
            break;
          case OmniSpawnDef_Grid.GridInstruction.Odds:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 5;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 8;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 13;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.Evens:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 4;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 8;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 12;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.Multiples3:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 3;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 7;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 10;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.Multiples4:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 3;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 6;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 9;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.Primes:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 5;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 8;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 12;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 4;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 8;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 12;
                break;
            }
            break;
          case OmniSpawnDef_Grid.GridInstruction.LessThan:
            switch (this.Size)
            {
              case OmniSpawnDef_Grid.GridSize.To9:
                num = 4;
                break;
              case OmniSpawnDef_Grid.GridSize.To16:
                num = 8;
                break;
              case OmniSpawnDef_Grid.GridSize.To25:
                num = 12;
                break;
            }
            break;
        }
        List<int> intList2;
        (intList2 = intList1)[0] = intList2[0] + 10 * num;
        List<int> intList3;
        (intList3 = intList1)[1] = intList3[1] + 7 * num;
        List<int> intList4;
        (intList4 = intList1)[2] = intList4[2] + 4 * num;
      }
      return intList1;
    }

    public enum GridSize
    {
      To9,
      To16,
      To25,
    }

    public enum GridConfiguration
    {
      Ascending,
      Descending,
      Shuffled,
    }

    public enum GridInstruction
    {
      CountUp,
      CountDown,
      Addition,
      Subtraction,
      Multiplication,
      Division,
      Odds,
      Evens,
      Multiples3,
      Multiples4,
      Primes,
      GreaterThan,
      LessThan,
    }
  }
}
