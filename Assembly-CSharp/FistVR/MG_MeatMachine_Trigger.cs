// Decompiled with JetBrains decompiler
// Type: FistVR.MG_MeatMachine_Trigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_MeatMachine_Trigger : MonoBehaviour
  {
    public MG_MeatMachine Machine;

    private void OnTriggerEnter(Collider col) => this.CheckCol(col);

    private void CheckCol(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      MG_MeatChunk component1 = col.attachedRigidbody.gameObject.GetComponent<MG_MeatChunk>();
      if ((Object) component1 != (Object) null)
      {
        int meatId = component1.MeatID;
        if (component1.IsHeld)
        {
          component1.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
          component1.EndInteraction(component1.m_hand);
        }
        Object.Destroy((Object) component1.gameObject);
        this.Machine.FedMeatIn(meatId);
      }
      FVRPhysicalObject component2 = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if (!((Object) component2 != (Object) null))
        return;
      if (component2.IsHeld)
      {
        component2.m_hand.ForceSetInteractable((FVRInteractiveObject) null);
        component2.EndInteraction(component2.m_hand);
      }
      Object.Destroy((Object) component2.gameObject);
      this.Machine.FedObjIn();
    }
  }
}
