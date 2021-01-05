// Decompiled with JetBrains decompiler
// Type: FistVR.BaitPie
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BaitPie : FVRPhysicalObject, IFVRDamageable
  {
    public GameObject CloudPrefab;
    private bool m_isExploded;

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isExploded || (double) d.Dam_TotalKinetic <= 100.0)
        return;
      this.m_isExploded = true;
      Object.Instantiate<GameObject>(this.CloudPrefab, this.transform.position + Vector3.up * 0.25f, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public override void OnCollisionEnter(Collision c)
    {
      base.OnCollisionEnter(c);
      if (this.m_isExploded || (double) c.relativeVelocity.magnitude <= 3.0)
        return;
      this.m_isExploded = true;
      Object.Instantiate<GameObject>(this.CloudPrefab, this.transform.position + Vector3.up * 0.25f, Quaternion.identity);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
