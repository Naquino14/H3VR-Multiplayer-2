// Decompiled with JetBrains decompiler
// Type: FistVR.MF_PlayAreaConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class MF_PlayAreaConfig
  {
    public MF_PlayArea PlayArea;
    public MF_Zone PlayerSpawnZone_Red;
    public MF_Zone PlayerSpawnZone_Blue;
    public List<MF_Zone> SpawnZones_RedTeam;
    public List<MF_Zone> SpawnZones_BlueTeam;
    public List<MF_Zone> PlayZones_RedTeam;
    public List<MF_Zone> PlayZones_BlueTeam;
    public List<MF_Zone> PlayZones_Neutral;

    public List<MF_Zone> GetZoneSet(MF_ZoneCategory cat)
    {
      switch (cat)
      {
        case MF_ZoneCategory.RedSide:
          return this.PlayZones_RedTeam;
        case MF_ZoneCategory.BlueSide:
          return this.PlayZones_BlueTeam;
        case MF_ZoneCategory.Neutral:
          return this.PlayZones_Neutral;
        case MF_ZoneCategory.SpawnRed:
          return this.SpawnZones_RedTeam;
        case MF_ZoneCategory.SpawnBlue:
          return this.SpawnZones_BlueTeam;
        default:
          return (List<MF_Zone>) null;
      }
    }
  }
}
