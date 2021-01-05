// Decompiled with JetBrains decompiler
// Type: FistVR.MF_ZoneMeta
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class MF_ZoneMeta : IComparable<MF_ZoneMeta>
  {
    public MF_Zone Zone;
    public MF_ZoneType Type;
    public float Neccesity;
    private List<MF_Squad> m_assignedSquads = new List<MF_Squad>();

    public void AssignSquad(MF_Squad s) => this.m_assignedSquads.Add(s);

    public void DeAssignSquad(MF_Squad s)
    {
      if (!this.m_assignedSquads.Contains(s))
        return;
      this.m_assignedSquads.Remove(s);
    }

    public int GetNumAssignedSquads() => this.m_assignedSquads.Count;

    public int CompareTo(MF_ZoneMeta other) => other == null ? 1 : this.Neccesity.CompareTo(other.Neccesity);
  }
}
