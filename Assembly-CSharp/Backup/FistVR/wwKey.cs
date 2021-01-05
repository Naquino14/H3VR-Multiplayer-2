// Decompiled with JetBrains decompiler
// Type: FistVR.wwKey
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwKey : FVRPhysicalObject
  {
    public wwParkManager Manager;
    public int KeyIndex;
    public int State;

    public void OnTriggerEnter(Collider col)
    {
      if (!col.gameObject.CompareTag("KeyDetectTrigger"))
        return;
      wwFinaleDoorTrigger component = col.gameObject.GetComponent<wwFinaleDoorTrigger>();
      this.Manager.FinaleManager.OpenDoor(component.Door.Index);
      this.Manager.RegisterDoorStateChange(component.Door.Index, 1);
      if (this.IsHeld)
      {
        FVRViveHand hand = this.m_hand;
        this.EndInteraction(hand);
        hand.ForceSetInteractable((FVRInteractiveObject) null);
      }
      this.Manager.RegisterKeyStateChange(this.KeyIndex, 2);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
