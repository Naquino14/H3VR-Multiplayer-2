// Decompiled with JetBrains decompiler
// Type: FistVR.WW_Keycard
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class WW_Keycard : FVRPhysicalObject
  {
    public int TierType;
    private float m_TimeToExpire = 300f;
    public Transform TickDownBar;
    public Vector3 BarFull;
    public Vector3 BarEmpty;
    public float HealPercent = 0.2f;
    public AudioEvent AudEvent_Eat;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.transform.position.y < -50.0)
        Object.Destroy((Object) this.gameObject);
      this.m_TimeToExpire -= Time.deltaTime;
      this.TickDownBar.transform.localScale = Vector3.Lerp(this.BarEmpty, this.BarFull, this.m_TimeToExpire / 300f);
      if ((double) this.m_TimeToExpire > 0.0)
        return;
      Object.Destroy((Object) this.gameObject);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      FVRViveHand fvrViveHand = hand;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f) >= 0.150000005960464)
        return;
      this.EndInteraction(hand);
      fvrViveHand.ForceSetInteractable((FVRInteractiveObject) null);
      SM.PlayGenericSound(this.AudEvent_Eat, this.transform.position);
      GM.CurrentPlayerBody.HealPercent(this.HealPercent);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
