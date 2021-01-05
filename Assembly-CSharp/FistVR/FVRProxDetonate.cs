// Decompiled with JetBrains decompiler
// Type: FistVR.FVRProxDetonate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRProxDetonate : MonoBehaviour, IFVRDamageable
  {
    private bool m_hasExploded;
    public GameObject[] Spawns;

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
      if (!((Object) col.attachedRigidbody != (Object) null) || (double) col.attachedRigidbody.velocity.magnitude <= 0.100000001490116)
        return;
      this.Explode();
    }

    public void Damage(FistVR.Damage dam) => this.Explode();
  }
}
