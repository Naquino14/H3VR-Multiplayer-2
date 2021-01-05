// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigObjectCatcher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigObjectCatcher : MonoBehaviour
  {
    public LayerMask CastLM;
    private RaycastHit m_hit;

    private void OnTriggerEnter(Collider col)
    {
      if (!((Object) col.attachedRigidbody != (Object) null))
        return;
      if ((Object) col.attachedRigidbody.gameObject.GetComponent<FVRFireArmRound>() != (Object) null)
      {
        Object.Destroy((Object) col.attachedRigidbody.gameObject);
      }
      else
      {
        if (!Physics.Raycast(col.attachedRigidbody.transform.position + Vector3.up * 200f, -Vector3.up, out this.m_hit, 200f, (int) this.CastLM, QueryTriggerInteraction.Ignore))
          return;
        col.attachedRigidbody.velocity = Vector3.zero;
        col.attachedRigidbody.angularVelocity = Vector3.zero;
        col.attachedRigidbody.transform.position = this.m_hit.point + Vector3.up * 0.6f;
        col.attachedRigidbody.velocity = Vector3.zero;
        col.attachedRigidbody.angularVelocity = Vector3.zero;
      }
    }
  }
}
