// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawnDef_Shape
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Shape Spawn Def", menuName = "OmniSequencer/SpawnDef/Shape Spawn Definition", order = 0)]
  public class OmniSpawnDef_Shape : OmniSpawnDef
  {
    public int ShapeAmount = 3;
    public List<OmniSpawnDef_Shape.ShapeInstruction> Instructions;

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
          case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColor:
            num = (int) ((double) this.ShapeAmount * 0.5);
            break;
          case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheShape:
            num = (int) ((double) this.ShapeAmount * 0.5);
            break;
          case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColorShape:
            num = (int) ((double) this.ShapeAmount * 0.5);
            break;
          case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotColor:
            num = this.ShapeAmount - (int) ((double) this.ShapeAmount * 0.5);
            break;
          case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotShape:
            num = this.ShapeAmount - (int) ((double) this.ShapeAmount * 0.5);
            break;
          case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotColorShape:
            num = this.ShapeAmount - (int) ((double) this.ShapeAmount * 0.5);
            break;
        }
        List<int> intList2;
        (intList2 = intList1)[0] = intList2[0] + 100 * num;
        List<int> intList3;
        (intList3 = intList1)[1] = intList3[1] + 70 * num;
        List<int> intList4;
        (intList4 = intList1)[2] = intList4[2] + 40 * num;
      }
      return intList1;
    }

    public enum ShapeInstruction
    {
      ShootTheColor,
      ShootTheShape,
      ShootTheColorShape,
      ShootAllTheColor,
      ShootAllTheShape,
      ShootAllTheColorShape,
      ShootAllTheNotColor,
      ShootAllTheNotShape,
      ShootAllTheNotColorShape,
    }

    public enum OmniShapeType
    {
      Circle,
      Triangle,
      Square,
      Pentagon,
      Hexagon,
      Diamond,
      Heptagon,
      Octagon,
    }

    public enum OmniShapeColor
    {
      Red,
      Orange,
      Yellow,
      Green,
      Blue,
      Purple,
      Pink,
      Brown,
    }
  }
}
