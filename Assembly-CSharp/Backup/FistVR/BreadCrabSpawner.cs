// Decompiled with JetBrains decompiler
// Type: FistVR.BreadCrabSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BreadCrabSpawner : SosigWearable, IFVRDamageable
  {
    public float m_lifeLeft = 2500f;
    private bool m_isDead;
    public FVRObject BreadCrab;
    public Transform BreadCrabSpawnPoint;
    private bool m_hasSpawned;

    public override void Damage(FistVR.Damage d)
    {
      this.m_lifeLeft -= d.Dam_TotalKinetic;
      if ((double) this.m_lifeLeft <= 0.0 && !this.m_isDead)
      {
        this.m_isDead = true;
        this.S.DestroyLink(this.S.Links[0], d.Class);
      }
      base.Damage(d);
    }

    private void Update() => this.SpawnCheck();

    private void SpawnCheck()
    {
      if (this.m_isDead || this.m_hasSpawned || ((Object) this.S == (Object) null || this.S.BodyState != Sosig.SosigBodyState.Dead) || !((Object) this.S.Links[0] != (Object) null))
        return;
      this.m_hasSpawned = true;
      this.L.DeRegisterWearable((SosigWearable) this);
      Object.Instantiate<GameObject>(this.BreadCrab.GetGameObject(), this.BreadCrabSpawnPoint.position, this.BreadCrabSpawnPoint.rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
