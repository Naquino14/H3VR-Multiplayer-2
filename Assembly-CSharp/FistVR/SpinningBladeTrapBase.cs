// Decompiled with JetBrains decompiler
// Type: FistVR.SpinningBladeTrapBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SpinningBladeTrapBase : MonoBehaviour
  {
    public Rigidbody rb;

    private void OnCollisionEnter(Collision col)
    {
      if ((double) this.rb.angularVelocity.magnitude <= 20.0)
        return;
      IFVRDamageable component = col.collider.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component == null && (Object) col.collider.attachedRigidbody != (Object) null)
        component = col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component == null)
        return;
      Damage dam = new Damage()
      {
        Dam_Cutting = 20000f,
        Dam_TotalKinetic = 20000f,
        hitNormal = col.contacts[0].normal
      };
      dam.strikeDir = -dam.hitNormal;
      dam.point = col.contacts[0].point;
      dam.Class = Damage.DamageClass.Environment;
      component.Damage(dam);
    }
  }
}
