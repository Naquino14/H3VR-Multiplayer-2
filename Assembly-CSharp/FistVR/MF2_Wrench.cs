// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Wrench
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF2_Wrench : MonoBehaviour
  {
    private void OnCollisionEnter(Collision collision)
    {
      if ((Object) collision.collider.attachedRigidbody == (Object) null || (double) collision.relativeVelocity.magnitude < 2.0)
        return;
      MF2_Dispenser component = collision.collider.attachedRigidbody.gameObject.GetComponent<MF2_Dispenser>();
      if (!((Object) component != (Object) null))
        return;
      component.HitCharge();
    }
  }
}
