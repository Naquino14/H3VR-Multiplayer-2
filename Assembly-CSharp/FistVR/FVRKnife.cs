// Decompiled with JetBrains decompiler
// Type: FistVR.FVRKnife
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRKnife : FVRPhysicalObject
  {
    public Collider Blade;
    public Collider Handle;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.IsHeld || (double) this.RootRigidbody.velocity.magnitude <= 0.0)
        ;
    }

    private new void OnCollisionEnter(Collision col)
    {
      if (this.IsHeld || !((Object) col.contacts[0].thisCollider == (Object) this.Blade) || (double) Vector3.Angle(col.contacts[0].normal, this.transform.forward) <= 95.0 || (double) col.impulse.magnitude <= 0.0500000007450581)
        return;
      this.RootRigidbody.isKinematic = true;
      this.transform.SetParent(col.contacts[0].otherCollider.transform);
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.transform.SetParent((Transform) null);
    }
  }
}
