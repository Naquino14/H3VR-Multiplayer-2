// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawnDef_Cleric
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Cleric Spawn Def", menuName = "OmniSequencer/SpawnDef/Cleric Spawn Definition", order = 0)]
  public class OmniSpawnDef_Cleric : OmniSpawnDef
  {
    public float TimeBetweenSets = 1f;
    public List<OmniSpawnDef_Cleric.ClericSet> Sets;

    public override List<int> CalculateSpawnerScoreThresholds()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index1 = 0; index1 < this.Sets.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.Sets[index1].TargetSet.Count; ++index2)
        {
          List<int> intList2;
          (intList2 = intList1)[0] = intList2[0] + 200;
          List<int> intList3;
          (intList3 = intList1)[1] = intList3[1] + 100;
          List<int> intList4;
          (intList4 = intList1)[2] = intList4[2] + 50;
        }
      }
      return intList1;
    }

    [Serializable]
    public class ClericSet
    {
      public OmniSpawnDef_Cleric.TargetSpawnStyle SpawnStyle;
      public List<OmniSpawnDef_Cleric.TargetLocation> TargetSet;
      public List<OmniSpawnDef_Cleric.TargetLocation> BlockerSet;
      public bool LocationsRandomized;
      public float SequentialTiming = 0.2f;
      public bool FiresBack;
      public float FiringTime = 1f;
    }

    public enum TargetSpawnStyle
    {
      AllAtOnce = -1, // 0xFFFFFFFF
      Sequential = 0,
      OnHit = 1,
    }

    public enum TargetLocation
    {
      FrontLeft1,
      FrontLeft2,
      Left1,
      Left2,
      Left3,
      BackLeft1,
      BackLeft2,
      FrontRight1,
      FrontRight2,
      Right1,
      Right2,
      Right3,
      BackRight1,
      BackRight2,
    }
  }
}
