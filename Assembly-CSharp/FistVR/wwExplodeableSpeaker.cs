// Decompiled with JetBrains decompiler
// Type: FistVR.wwExplodeableSpeaker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwExplodeableSpeaker : MonoBehaviour, IFVRDamageable
  {
    private bool m_isDestroyed;
    public GameObject Splode;
    public wwPASystem PASystem;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class != FistVR.Damage.DamageClass.Projectile || this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      Object.Instantiate<GameObject>(this.Splode, this.transform.position, this.transform.rotation);
      this.PASystem.DestroySpeaker(this);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
