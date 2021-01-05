// Decompiled with JetBrains decompiler
// Type: FistVR.RessemblerCoreTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RessemblerCoreTrigger : MonoBehaviour
  {
    public int SlotIndex;
    public Translocator T;
    private bool m_hasInserted;

    private void FixedUpdate() => this.m_hasInserted = false;

    private void OnTriggerEnter(Collider col)
    {
      if (!((Object) col.attachedRigidbody != (Object) null) || !((Object) col.attachedRigidbody.gameObject.GetComponent<RessemblerCore>() != (Object) null) || (this.m_hasInserted || !this.T.InsertCoreToSlot(this.SlotIndex)))
        return;
      this.m_hasInserted = true;
      Object.Destroy((Object) col.attachedRigidbody.gameObject);
      this.gameObject.SetActive(false);
    }
  }
}
