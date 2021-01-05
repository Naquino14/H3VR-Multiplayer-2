// Decompiled with JetBrains decompiler
// Type: FistVR.Microtorch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Microtorch : MonoBehaviour
  {
    public FVRFireArmAttachment Attachment;
    public Transform FlamePoint;
    public GameObject FX;
    public bool m_isFireActive;
    public LayerMask LM_FireDamage;
    private RaycastHit m_hit;
    public float PointsDamage = 5f;

    private void Update()
    {
      if ((Object) this.Attachment != (Object) null)
      {
        if (!this.m_isFireActive && (Object) this.Attachment.curMount != (Object) null)
        {
          this.m_isFireActive = true;
          this.FX.SetActive(true);
        }
        if (this.m_isFireActive && (Object) this.Attachment.curMount == (Object) null)
        {
          this.m_isFireActive = false;
          this.FX.SetActive(false);
        }
      }
      if (!this.m_isFireActive || !Physics.Raycast(this.FlamePoint.position, this.FlamePoint.forward, out this.m_hit, 0.08f, (int) this.LM_FireDamage, QueryTriggerInteraction.Collide))
        return;
      IFVRDamageable component1 = this.m_hit.collider.gameObject.GetComponent<IFVRDamageable>();
      if (component1 == null && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
        component1 = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component1 != null)
        component1.Damage(new Damage()
        {
          Class = Damage.DamageClass.Explosive,
          Dam_Thermal = 50f,
          Dam_TotalEnergetic = 50f,
          point = this.m_hit.point,
          hitNormal = this.m_hit.normal,
          strikeDir = this.transform.forward
        });
      FVRIgnitable component2 = this.m_hit.collider.transform.gameObject.GetComponent<FVRIgnitable>();
      if ((Object) component2 == (Object) null && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
        this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
      if (!((Object) component2 != (Object) null))
        return;
      FXM.Ignite(component2, 1f);
    }
  }
}
