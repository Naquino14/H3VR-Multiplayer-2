// Decompiled with JetBrains decompiler
// Type: FistVR.RessemblerCore
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RessemblerCore : FVRPhysicalObject, IFVRDamageable
  {
    public GameObject ExplosionPrefab;
    private bool m_isExploded;

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isExploded || (double) d.Dam_TotalKinetic <= 100.0)
        return;
      this.m_isExploded = true;
      if ((Object) this.ExplosionPrefab != (Object) null)
        Object.Instantiate<GameObject>(this.ExplosionPrefab, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
