// Decompiled with JetBrains decompiler
// Type: FistVR.PowerUp_Cyclops
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PowerUp_Cyclops : MonoBehaviour
  {
    public Transform Beam;
    public LayerMask CastMask;
    private RaycastHit m_hit;
    public ParticleSystem HitParticles;
    public float Thickness = 0.15f;

    private void Update()
    {
      if (Physics.Raycast(this.transform.position, this.transform.forward, out this.m_hit, 500f, (int) this.CastMask, QueryTriggerInteraction.Ignore))
      {
        Vector3 point = this.m_hit.point;
        this.HitParticles.transform.position = this.m_hit.point;
        this.HitParticles.Emit(1);
        this.Beam.transform.localScale = new Vector3(0.15f, 0.15f, this.m_hit.distance);
        if (!((Object) this.m_hit.collider.attachedRigidbody != (Object) null) || this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>() == null)
          return;
        if ((bool) (Object) this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>())
          FXM.Ignite(this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>(), 1f);
        Damage dam = new Damage();
        float cyclopsPower = GM.CurrentPlayerBody.GetCyclopsPower();
        dam.Class = Damage.DamageClass.Projectile;
        dam.Dam_Piercing = 1f * cyclopsPower;
        dam.Dam_TotalKinetic = 1f * cyclopsPower;
        dam.Dam_Thermal = 50f * cyclopsPower;
        dam.Dam_TotalEnergetic = 50f * cyclopsPower;
        dam.point = this.m_hit.point;
        dam.hitNormal = this.m_hit.normal;
        dam.strikeDir = this.transform.forward;
        this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>().Damage(dam);
      }
      else
      {
        this.HitParticles.transform.position = this.transform.position + this.transform.forward * 500f;
        this.Beam.transform.localScale = new Vector3(this.Thickness, this.Thickness, 501f);
      }
    }
  }
}
