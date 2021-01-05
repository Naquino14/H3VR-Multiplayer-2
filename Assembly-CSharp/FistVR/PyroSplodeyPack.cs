// Decompiled with JetBrains decompiler
// Type: FistVR.PyroSplodeyPack
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PyroSplodeyPack : MonoBehaviour, IFVRDamageable
  {
    public SosigWearable W;
    private bool m_isDestroyed;
    private float m_life = 1500f;
    public Transform SpawnPoint;
    public List<GameObject> SpawnOnBoom;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      this.m_life -= d.Dam_TotalKinetic;
      if ((double) this.m_life >= 0.0)
        return;
      this.Boom();
    }

    private void Boom()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.W.S.KillSosig();
      for (int index = 0; index < this.SpawnOnBoom.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnBoom[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
    }
  }
}
