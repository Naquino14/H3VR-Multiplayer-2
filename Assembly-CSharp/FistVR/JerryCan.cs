// Decompiled with JetBrains decompiler
// Type: FistVR.JerryCan
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class JerryCan : FVRPhysicalObject, IFVRDamageable
  {
    private bool m_hasExploded;
    public AudioEvent AudEvent_Ignite;
    public GameObject Prefab_FireSplosion;
    public GameObject Prefab_GroundFire;
    public float GroundFireRange = 5f;
    public LayerMask LM_Env;
    private RaycastHit m_hit;

    public override bool IsDistantGrabbable() => base.IsDistantGrabbable();

    public override bool IsInteractable() => base.IsInteractable();

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal <= 20.0 && (double) d.Dam_TotalKinetic <= 500.0)
        return;
      this.Explode();
    }

    public override void BeginInteraction(FVRViveHand hand) => base.BeginInteraction(hand);

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
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
      Object.Destroy((Object) this.gameObject);
    }
  }
}
