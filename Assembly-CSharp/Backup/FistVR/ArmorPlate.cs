// Decompiled with JetBrains decompiler
// Type: FistVR.ArmorPlate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ArmorPlate : MonoBehaviour, IFVRDamageable
  {
    public IcoSphereTarget Core;
    public int Life = 1;
    private Rigidbody rb;
    public float Points;
    private bool m_isDetached;
    private bool m_isDestroyed;
    private bool isDieTicking;
    private float dieTick = 5f;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
        this.Life -= (int) d.Dam_TotalKinetic;
      if (this.Life > 0)
        return;
      this.Boom(true);
    }

    public void Update()
    {
      if (this.isDieTicking)
        this.dieTick -= Time.deltaTime;
      if ((double) this.dieTick > 0.0)
        return;
      this.Boom(false);
    }

    public void Boom(bool getPoints)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      if ((Object) this.Core != (Object) null)
        this.Core.DamageParticle(this.transform.position, 3);
      if (getPoints && (Object) this.Core != (Object) null)
        this.Core.Game.ScorePoints(this.Points);
      Object.Destroy((Object) this.gameObject);
    }

    public void Detach(Vector3 vel)
    {
      if (this.m_isDetached)
        return;
      this.m_isDetached = true;
      this.transform.SetParent((Transform) null);
      this.rb = this.gameObject.AddComponent<Rigidbody>();
      this.rb.velocity = vel * Random.Range(1f, 20f);
      this.rb.angularVelocity = Random.onUnitSphere * 3f;
      this.rb.useGravity = false;
      this.gameObject.tag = "Harmless";
      this.isDieTicking = true;
      this.dieTick = Random.Range(3f, 5f);
    }
  }
}
