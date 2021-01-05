// Decompiled with JetBrains decompiler
// Type: FistVR.Destructobin
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Destructobin : MonoBehaviour
  {
    private void testCol(Collider col)
    {
      if (!((Object) col.attachedRigidbody != (Object) null) || !((Object) col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != (Object) null))
        return;
      FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if ((Object) component.QuickbeltSlot != (Object) null || component.m_isHardnessed)
        return;
      if (!component.IsHeld)
      {
        Object.Destroy((Object) component.gameObject);
      }
      else
      {
        component.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
        component.EndInteraction(component.m_hand);
        Object.Destroy((Object) component.gameObject);
      }
    }

    private void OnTriggerStay(Collider other) => this.testCol(other);
  }
}
