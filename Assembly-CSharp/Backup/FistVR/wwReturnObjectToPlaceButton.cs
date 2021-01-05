// Decompiled with JetBrains decompiler
// Type: FistVR.wwReturnObjectToPlaceButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwReturnObjectToPlaceButton : FVRInteractiveObject
  {
    public FVRPhysicalObject Obj;
    public Transform ReturnPoint;
    public AudioSource Aud;

    public override void Poke(FVRViveHand hand)
    {
      base.Poke(hand);
      if (this.Obj.IsHeld || !((Object) this.Obj.QuickbeltSlot == (Object) null))
        return;
      this.Aud.Play();
      this.Obj.transform.position = this.ReturnPoint.position;
      this.Obj.transform.rotation = this.ReturnPoint.rotation;
      this.Obj.RootRigidbody.velocity = Vector3.zero;
      this.Obj.RootRigidbody.angularVelocity = Vector3.zero;
    }
  }
}
