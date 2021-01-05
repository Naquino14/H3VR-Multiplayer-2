// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_StickyBomb
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF2_StickyBomb : MonoBehaviour, IFVRDamageable
  {
    public bool m_hasExploded;
    public bool m_hasStuck;
    public List<GameObject> SpawnOnExplode;
    public Rigidbody RB;
    public int IFF;
    public List<Material> Mats;
    public Renderer Rend;
    public bool DelayFuse;
    public Vector2 FuseTime;
    private float fuse = 1f;
    private bool m_isFUsing;

    public void SetIFF(int i)
    {
      this.IFF = i;
      i = Mathf.Clamp(i, 0, 2);
      this.Rend.material = this.Mats[i];
    }

    private void Start()
    {
    }

    public void Detonate()
    {
      if (this.DelayFuse)
      {
        this.fuse = Random.Range(this.FuseTime.x, this.FuseTime.y);
        this.m_isFUsing = true;
      }
      else
        this.Explode();
    }

    private void Update()
    {
      if (!this.m_isFUsing)
        return;
      this.fuse -= Time.deltaTime;
      if ((double) this.fuse > 0.0)
        return;
      this.Explode();
    }

    private void Stick()
    {
      this.RB.isKinematic = true;
      this.m_hasStuck = true;
    }

    private void UnStick()
    {
      this.RB.isKinematic = false;
      this.m_hasStuck = false;
    }

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
      for (int index = 0; index < this.SpawnOnExplode.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnExplode[index], this.transform.position, Random.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public void Damage(FistVR.Damage d)
    {
      if (!this.m_hasStuck || d.Class == FistVR.Damage.DamageClass.Explosive)
        return;
      this.Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (this.m_hasStuck || (Object) collision.contacts[0].otherCollider.attachedRigidbody != (Object) null)
        return;
      this.Stick();
    }
  }
}
