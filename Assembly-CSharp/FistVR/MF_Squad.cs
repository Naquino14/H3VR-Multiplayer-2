// Decompiled with JetBrains decompiler
// Type: FistVR.MF_Squad
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class MF_Squad
  {
    private MF_Team team;
    private List<Sosig> members = new List<Sosig>();
    private Dictionary<Sosig, MF_Class> classDic = new Dictionary<Sosig, MF_Class>();
    private Sosig squadmedic;
    private bool m_hasMedic;
    private MF_ZoneMeta m_assignedZone;
    private Vector3 m_patrolToPoint = Vector3.zero;
    private float m_tickDownToRearmCheck = 1f;

    public void AssignZone(MF_ZoneMeta z, Sosig s)
    {
      this.m_assignedZone = z;
      this.m_assignedZone.AssignSquad(this);
      this.PickNewRandomPatrolPoint(s);
    }

    public void AddMember(Sosig s, MF_Class c, MF_Team t)
    {
      this.team = t;
      this.members.Add(s);
      this.classDic.Add(s, c);
      if (c != MF_Class.Medic)
        return;
      this.m_hasMedic = true;
      this.squadmedic = s;
    }

    public void Tick(float t)
    {
      this.m_tickDownToRearmCheck -= Time.deltaTime;
      if ((double) this.m_tickDownToRearmCheck <= 0.0)
      {
        this.m_tickDownToRearmCheck = UnityEngine.Random.Range(1f, 2f);
        this.RearmCheck();
      }
      if (this.members.Count <= 0)
        return;
      Sosig member = this.members[0];
      if ((UnityEngine.Object) member != (UnityEngine.Object) null)
      {
        float num = Vector3.Distance(member.transform.position, this.m_patrolToPoint);
        if (this.team.GetColor() == MF_TeamColor.Blue)
          Debug.DrawLine(member.transform.position, this.m_patrolToPoint, Color.blue);
        else
          Debug.DrawLine(member.transform.position, this.m_patrolToPoint, Color.red);
        if ((double) num < 2.0)
          this.PickNewRandomPatrolPoint(member);
      }
      if (this.members.Count <= 1)
        return;
      for (int index = 1; index < this.members.Count; ++index)
      {
        if ((UnityEngine.Object) this.members[index] != (UnityEngine.Object) null)
        {
          Vector3 vector3_1 = this.members[index].transform.position - member.transform.position;
          vector3_1.y = 0.0f;
          vector3_1.Normalize();
          Vector3 vector3_2 = member.transform.position + vector3_1;
          this.members[index].UpdateAssaultPoint(vector3_2);
          Debug.DrawLine(this.members[index].transform.position, vector3_2, Color.white);
        }
      }
    }

    private void PickNewRandomPatrolPoint(Sosig s)
    {
      this.m_patrolToPoint = this.m_assignedZone.Zone.GetTargetPointByClass(this.classDic[s]).position;
      s.UpdateAssaultPoint(this.m_patrolToPoint);
    }

    private void RearmCheck()
    {
      for (int index = 0; index < this.members.Count; ++index)
      {
        if ((UnityEngine.Object) this.members[index] != (UnityEngine.Object) null && !this.members[index].DoIHaveAGun() && (this.members[index].CurrentOrder == Sosig.SosigOrder.SearchForEquipment && this.members[index].BodyState == Sosig.SosigBodyState.InControl))
          this.team.GetManager().RearmSosig(this.members[index], this.classDic[this.members[index]]);
      }
    }

    public void Cleanup()
    {
      if (this.members.Count <= 0)
        return;
      for (int index = this.members.Count - 1; index >= 0; --index)
      {
        if (this.members[index].BodyState == Sosig.SosigBodyState.Dead)
        {
          this.members[index].TickDownToClear(5f);
          this.members.RemoveAt(index);
        }
      }
    }

    public void Flush()
    {
      if (this.members.Count > 0)
      {
        for (int index = this.members.Count - 1; index >= 0; --index)
        {
          if (this.members[index].BodyState == Sosig.SosigBodyState.Dead)
          {
            this.members[index].TickDownToClear(5f);
            this.members.RemoveAt(index);
          }
        }
      }
      this.classDic.Clear();
      this.squadmedic = (Sosig) null;
      this.m_hasMedic = false;
      this.team = (MF_Team) null;
      this.m_assignedZone.DeAssignSquad(this);
      this.m_assignedZone = (MF_ZoneMeta) null;
    }

    public int GetNumAlive() => this.members.Count;
  }
}
