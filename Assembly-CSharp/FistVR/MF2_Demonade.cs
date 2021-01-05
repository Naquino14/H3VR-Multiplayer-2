// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Demonade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF2_Demonade : MonoBehaviour, IFVRDamageable
  {
    private bool m_hasExploded;
    public Vector2 FuseTimeRange = new Vector2(2.3f, 2.3f);
    private float m_fuseTime = 2.3f;
    public bool ExplodesOnContact = true;
    public string ExplodeOnContactLayerName;
    public List<GameObject> SpawnOnExplode;
    public bool MatSwitches = true;
    public int IFF;
    public List<Material> Mats;
    public Renderer Rend;

    public void SetIFF(int i)
    {
      this.IFF = i;
      i = Mathf.Clamp(i, 0, 2);
      if (!this.MatSwitches)
        return;
      this.Rend.material = this.Mats[i];
    }

    private void Start() => this.m_fuseTime = Random.Range(this.FuseTimeRange.x, this.FuseTimeRange.y);

    private void Update()
    {
      this.m_fuseTime -= Time.deltaTime;
      if ((double) this.m_fuseTime > 0.0)
        return;
      this.Explode();
    }

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
      for (int index = 0; index < this.SpawnOnExplode.Count; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SpawnOnExplode[index], this.transform.position, Random.rotation);
        Explosion component1 = gameObject.GetComponent<Explosion>();
        if ((Object) component1 != (Object) null)
          component1.IFF = this.IFF;
        ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
        if ((Object) component2 != (Object) null)
          component2.IFF = this.IFF;
      }
      Object.Destroy((Object) this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (!this.ExplodesOnContact || (Object) collision.contacts[0].otherCollider.attachedRigidbody == (Object) null || !(this.ExplodeOnContactLayerName == LayerMask.LayerToName(collision.contacts[0].otherCollider.gameObject.layer)))
        return;
      this.Explode();
    }
  }
}
