// Decompiled with JetBrains decompiler
// Type: FistVR.Molotov
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Molotov : FVRPhysicalObject, IFVRDamageable
  {
    [Header("Molotov Params")]
    public float ShatterThreshold = 3f;
    public FVRIgnitable Igniteable;
    public AudioEvent AudEvent_Ignite;
    public GameObject Prefab_ShatterFX;
    public GameObject Prefab_FireSplosion;
    public GameObject Prefab_GroundFire;
    public float GroundFireRange = 5f;
    public LayerMask LM_Env;
    private RaycastHit m_hit;
    private float TickDownToShatter = 28f;
    private bool m_hasShattered;

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal > 30.0)
        FXM.Ignite(this.Igniteable, 1f);
      if ((double) d.Dam_TotalKinetic <= 100.0)
        return;
      this.Shatter();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.Igniteable.IsOnFire())
        return;
      this.TickDownToShatter -= Time.deltaTime;
      if ((double) this.TickDownToShatter > 0.0)
        return;
      this.Shatter();
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if ((double) col.relativeVelocity.magnitude <= (double) this.ShatterThreshold)
        return;
      this.Shatter();
    }

    private void Shatter()
    {
      if (this.m_hasShattered)
        return;
      this.m_hasShattered = true;
      if (this.Igniteable.IsOnFire())
      {
        Object.Instantiate<GameObject>(this.Prefab_FireSplosion, this.transform.position, this.transform.rotation);
        SM.PlayGenericSound(this.AudEvent_Ignite, this.transform.position);
        int num = 0;
        for (int index = 0; index < 5 && num <= 2; ++index)
        {
          Vector3 direction = -Vector3.up;
          if (index > 0)
          {
            direction = Random.onUnitSphere;
            if ((double) direction.y > 0.0)
              direction.y = -direction.y;
          }
          if (Physics.Raycast(this.transform.position + Vector3.up, direction, out this.m_hit, this.GroundFireRange, (int) this.LM_Env, QueryTriggerInteraction.Ignore))
          {
            Object.Instantiate<GameObject>(this.Prefab_GroundFire, this.m_hit.point, Quaternion.LookRotation(Vector3.up));
            ++num;
          }
        }
      }
      Object.Instantiate<GameObject>(this.Prefab_ShatterFX, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    private void OnParticleCollision(GameObject other) => this.Igniteable.OnParticleCollision(other);
  }
}
