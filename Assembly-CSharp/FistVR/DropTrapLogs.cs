// Decompiled with JetBrains decompiler
// Type: FistVR.DropTrapLogs
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DropTrapLogs : MonoBehaviour
  {
    public Transform ColRef;
    public Rigidbody RB;
    private Vector3 m_vel = Vector3.zero;

    private void Start()
    {
    }

    private void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (this.RB.isKinematic || (Object) collision.collider.attachedRigidbody == (Object) null)
        return;
      float y = this.ColRef.transform.position.y;
      ContactPoint contact = collision.contacts[0];
      if ((double) Mathf.Abs(contact.point.y - y) > 0.400000005960464)
        return;
      IFVRDamageable component = collision.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component == null)
        return;
      component.Damage(new Damage()
      {
        Class = Damage.DamageClass.Environment,
        damageSize = 0.1f,
        Dam_Blunt = 20000f,
        Dam_TotalKinetic = 20000f,
        hitNormal = contact.normal,
        point = contact.point,
        Source_IFF = 0,
        strikeDir = -contact.normal,
        Source_Point = contact.point
      });
    }
  }
}
