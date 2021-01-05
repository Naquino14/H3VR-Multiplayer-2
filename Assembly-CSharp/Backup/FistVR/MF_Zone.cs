// Decompiled with JetBrains decompiler
// Type: FistVR.MF_Zone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF_Zone : MonoBehaviour
  {
    public MF_ZoneCategory Cat;
    public bool DrawGiz;
    public Transform Point_CapturePointPlinth;
    public List<MF_ZonePoint> TargetPoints_Assault;
    public List<MF_ZonePoint> TargetPoints_Support;
    public List<MF_ZonePoint> TargetPoints_Sniping;
    private MF_CapturePoint m_capturePoint;

    public MF_CapturePoint GetCapturePoint() => this.m_capturePoint;

    public MF_CapturePoint SpawnCapturePoint(GameObject prefab)
    {
      this.m_capturePoint = Object.Instantiate<GameObject>(prefab, this.Point_CapturePointPlinth.position, this.Point_CapturePointPlinth.rotation).GetComponent<MF_CapturePoint>();
      this.m_capturePoint.SetZone(this);
      return this.m_capturePoint;
    }

    public Transform GetTargetPointByClass(MF_Class c)
    {
      switch (c)
      {
        case MF_Class.Scout:
        case MF_Class.Spy:
        case MF_Class.Engineer:
        case MF_Class.Pyro:
          return this.TargetPoints_Assault[Random.Range(0, this.TargetPoints_Assault.Count)].transform;
        case MF_Class.Sniper:
          return this.TargetPoints_Sniping[Random.Range(0, this.TargetPoints_Sniping.Count)].transform;
        case MF_Class.Medic:
        case MF_Class.Demoman:
        case MF_Class.Soldier:
        case MF_Class.Heavy:
          return this.TargetPoints_Support[Random.Range(0, this.TargetPoints_Support.Count)].transform;
        default:
          return this.TargetPoints_Assault[Random.Range(0, this.TargetPoints_Assault.Count)].transform;
      }
    }

    private void OnDrawGizmos()
    {
      if (!this.DrawGiz)
        return;
      Gizmos.color = new Color(1f, 1f, 1f, 1f);
      Gizmos.DrawSphere(this.transform.position, 0.25f);
      Gizmos.DrawWireSphere(this.transform.position, 0.25f);
    }
  }
}
