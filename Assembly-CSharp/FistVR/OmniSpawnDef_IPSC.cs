// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawnDef_IPSC
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New IPSC Spawn Def", menuName = "OmniSequencer/SpawnDef/IPSC Spawn Definition", order = 0)]
  public class OmniSpawnDef_IPSC : OmniSpawnDef
  {
    public List<OmniSpawnDef_IPSC.IPSCType> TargetList;
    public Vector2 SpawnCadence = new Vector2(0.25f, 0.25f);
    public Vector2 TimeActivated = new Vector2(1f, 1f);

    public override List<int> CalculateSpawnerScoreThresholds()
    {
      List<int> intList1 = new List<int>(3);
      for (int index = 0; index < 3; ++index)
        intList1.Add(0);
      for (int index = 0; index < this.TargetList.Count; ++index)
      {
        if (this.TargetList[index] == OmniSpawnDef_IPSC.IPSCType.Standard)
        {
          List<int> intList2;
          (intList2 = intList1)[0] = intList2[0] + 100;
          List<int> intList3;
          (intList3 = intList1)[1] = intList3[1] + 70;
          List<int> intList4;
          (intList4 = intList1)[2] = intList4[2] + 30;
        }
      }
      return intList1;
    }

    public enum IPSCType
    {
      Standard,
      NoShoot,
    }
  }
}
