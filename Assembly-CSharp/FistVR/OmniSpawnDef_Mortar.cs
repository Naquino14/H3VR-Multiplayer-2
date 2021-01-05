// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawnDef_Mortar
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Mortar Spawn Def", menuName = "OmniSequencer/SpawnDef/Mortar Spawn Definition", order = 0)]
  public class OmniSpawnDef_Mortar : OmniSpawnDef
  {
    public int NumShots = 1;
    public float MortarSize = 1f;
    public Vector2 SpawnCadence = new Vector2(0.25f, 0.25f);
    public Vector2 VelocityRange = new Vector2(10f, 10f);
    public OmniSpawnDef_Mortar.MortarAngle Angle;

    public override List<int> CalculateSpawnerScoreThresholds()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index = 0; index < this.NumShots; ++index)
      {
        List<int> intList2;
        (intList2 = intList1)[0] = intList2[0] + 100;
        List<int> intList3;
        (intList3 = intList1)[1] = intList3[1] + 75;
        List<int> intList4;
        (intList4 = intList1)[2] = intList4[2] + 50;
      }
      return intList1;
    }

    public enum MortarAngle
    {
      DownRange,
      Centered,
      UpRange,
    }
  }
}
