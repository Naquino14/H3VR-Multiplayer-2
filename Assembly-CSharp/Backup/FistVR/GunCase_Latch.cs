// Decompiled with JetBrains decompiler
// Type: FistVR.GunCase_Latch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GunCase_Latch : FVRInteractiveObject, IFVRDamageable
  {
    private bool m_isOpen;
    public AudioEvent LatchOpen;

    public bool IsOpen() => this.m_isOpen;

    protected override void Awake() => base.Awake();

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      if (this.m_isOpen)
        return;
      this.Open();
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isOpen)
        return;
      this.Open();
    }

    private void Open()
    {
      this.m_isOpen = true;
      SM.PlayGenericSound(this.LatchOpen, this.transform.position);
      this.transform.localEulerAngles = new Vector3(-45f, 0.0f, 0.0f);
    }

    public void Reset()
    {
      this.transform.localEulerAngles = Vector3.zero;
      this.m_isOpen = false;
    }
  }
}
