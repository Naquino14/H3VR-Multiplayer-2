// Decompiled with JetBrains decompiler
// Type: FistVR.FVRGrenadePin
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRGrenadePin : FVRPhysicalObject
  {
    public FVRGrenade Grenade;
    public GameObject PinPiece;
    public bool IsSecondaryPin;
    private bool m_hasBeenPulled;
    public bool DieAfterTick = true;
    private bool m_isDying;
    private float m_dieTick = 10f;

    public override bool IsInteractable()
    {
      if (!this.Grenade.IsHeld)
        return false;
      if (this.m_hasBeenPulled)
        return base.IsInteractable();
      return !((Object) this.Grenade.QuickbeltSlot != (Object) null);
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (!this.m_hasBeenPulled)
      {
        this.m_hasBeenPulled = true;
        this.transform.SetParent((Transform) null);
        this.PinPiece.transform.SetParent(this.transform);
        Rigidbody rigidbody = this.PinPiece.AddComponent<Rigidbody>();
        rigidbody.mass = 0.01f;
        this.GetComponent<HingeJoint>().connectedBody = rigidbody;
        if (this.IsSecondaryPin)
          this.Grenade.PullPin2();
        else
          this.Grenade.PullPin();
        this.m_isDying = true;
        if ((Object) this.UXGeo_Held != (Object) null)
          Object.Destroy((Object) this.UXGeo_Held);
      }
      base.BeginInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.DieAfterTick || !this.m_isDying)
        return;
      this.m_dieTick -= Time.deltaTime;
      if ((double) this.m_dieTick > 0.0)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
