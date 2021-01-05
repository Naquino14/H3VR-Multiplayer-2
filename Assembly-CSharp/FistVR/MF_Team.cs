// Decompiled with JetBrains decompiler
// Type: FistVR.MF_Team
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class MF_Team
  {
    private MF_TeamColor m_teamColor;
    private MF_TeamManager manager;
    private MF_Team opposingTeam;
    private List<MF_ZoneMeta> m_zones = new List<MF_ZoneMeta>();
    private List<MF_ZoneMeta> m_spawnZones = new List<MF_ZoneMeta>();
    private List<MF_Squad> m_squads = new List<MF_Squad>();
    private int maxSize;
    private float spawnCadence;
    private float m_tickDownToSpawn;

    public MF_TeamColor GetColor() => this.m_teamColor;

    public MF_TeamManager GetManager() => this.manager;

    public void Init(MF_TeamManager m, MF_TeamColor tc, MF_Team o, int max, float spawnspeed)
    {
      this.manager = m;
      this.opposingTeam = o;
      this.m_teamColor = tc;
      this.maxSize = max;
      this.spawnCadence = spawnspeed;
      this.m_tickDownToSpawn = this.spawnCadence * UnityEngine.Random.Range(1f, 1.2f);
      this.UpdateZoneMetaScoring();
      for (int spawnZoneIndex = 0; spawnZoneIndex < this.m_spawnZones.Count; ++spawnZoneIndex)
        this.SpawnSquad(spawnZoneIndex);
    }

    public void ResetZoneSet()
    {
      if (this.m_spawnZones.Count > 0)
      {
        for (int index = this.m_spawnZones.Count - 1; index >= 0; --index)
          this.m_spawnZones[index].Zone = (MF_Zone) null;
      }
      this.m_spawnZones.Clear();
      if (this.m_zones.Count > 0)
      {
        for (int index = this.m_zones.Count - 1; index >= 0; --index)
          this.m_zones[index].Zone = (MF_Zone) null;
      }
      this.m_zones.Clear();
    }

    public void LoadZoneList(List<MF_Zone> zones, MF_ZoneType type)
    {
      if (type == MF_ZoneType.Spawn)
      {
        for (int index = 0; index < zones.Count; ++index)
          this.m_spawnZones.Add(new MF_ZoneMeta()
          {
            Type = type,
            Zone = zones[index]
          });
      }
      else
      {
        for (int index = 0; index < zones.Count; ++index)
          this.m_zones.Add(new MF_ZoneMeta()
          {
            Type = type,
            Zone = zones[index]
          });
      }
    }

    public void Tick(float t)
    {
      if (this.m_squads.Count > 0)
      {
        for (int index = this.m_squads.Count - 1; index >= 0; --index)
          this.m_squads[index].Cleanup();
      }
      if (this.m_squads.Count > 0)
      {
        for (int index = this.m_squads.Count - 1; index >= 0; --index)
        {
          if (this.m_squads[index].GetNumAlive() < 1)
          {
            this.m_squads[index].Flush();
            this.m_squads.RemoveAt(index);
          }
        }
      }
      this.UpdateZoneMetaScoring();
      this.m_tickDownToSpawn -= t;
      if ((double) this.m_tickDownToSpawn <= 0.0)
      {
        this.m_tickDownToSpawn = this.spawnCadence;
        this.SpawnSquad(UnityEngine.Random.Range(0, this.m_spawnZones.Count));
      }
      for (int index = 0; index < this.m_squads.Count; ++index)
        this.m_squads[index].Tick(t);
    }

    private void UpdateZoneMetaScoring()
    {
      for (int index = 0; index < this.m_zones.Count; ++index)
        this.m_zones[index].Neccesity = UnityEngine.Random.Range(0.1f, 0.5f);
      for (int index = 0; index < this.m_zones.Count; ++index)
      {
        if (this.m_zones[index].Type == MF_ZoneType.EnemySide)
          this.m_zones[index].Neccesity += UnityEngine.Random.Range(1f, 1.6f);
        if (this.m_zones[index].Type == MF_ZoneType.Center)
          this.m_zones[index].Neccesity += UnityEngine.Random.Range(2f, 10f);
        if (this.m_zones[index].GetNumAssignedSquads() > 2)
          this.m_zones[index].Neccesity -= UnityEngine.Random.Range(-0.1f, -1.5f);
      }
      this.m_zones.Sort();
      this.m_zones.Reverse();
    }

    public void SpawnSquad(int spawnZoneIndex)
    {
      MF_ZoneMeta zone = this.m_zones[0];
      int a = this.maxSize - this.NumOnTeam();
      if (a < 1)
        return;
      MF_SquadDefinition squadDef = this.manager.SquadDefs[UnityEngine.Random.Range(0, this.manager.SquadDefs.Count)];
      int num = Mathf.Min(a, squadDef.MemberClasses.Count);
      MF_ZoneMeta spawnZone = this.m_spawnZones[spawnZoneIndex];
      MF_Squad mfSquad = new MF_Squad();
      Sosig s1 = (Sosig) null;
      for (int index = 0; index < num; ++index)
      {
        MF_Class memberClass = squadDef.MemberClasses[index];
        spawnZone.Zone.TargetPoints_Assault.Shuffle<MF_ZonePoint>();
        Transform transform = spawnZone.Zone.TargetPoints_Assault[index].transform;
        Transform targetPointByClass = zone.Zone.GetTargetPointByClass(memberClass);
        SosigEnemyTemplate t = this.manager.EnemyTemplatesByClassIndex_Red[(int) memberClass];
        int IFF = this.manager.IFF_Red;
        if (this.m_teamColor == MF_TeamColor.Blue)
        {
          IFF = this.manager.IFF_Blue;
          t = this.manager.EnemyTemplatesByClassIndex_Blue[(int) memberClass];
        }
        Sosig s2 = this.manager.SpawnEnemy(t, transform, IFF, true, targetPointByClass.position, true, this.m_teamColor);
        if (index == 0)
          s1 = s2;
        mfSquad.AddMember(s2, memberClass, this);
      }
      mfSquad.AssignZone(zone, s1);
      this.m_squads.Add(mfSquad);
    }

    private int NumOnTeam()
    {
      int num = 0;
      for (int index = 0; index < this.m_squads.Count; ++index)
        num += this.m_squads[index].GetNumAlive();
      return num;
    }
  }
}
