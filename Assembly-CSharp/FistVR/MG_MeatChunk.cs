// Decompiled with JetBrains decompiler
// Type: FistVR.MG_MeatChunk
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class MG_MeatChunk : FVRPhysicalObject
  {
    public bool CanMeatBePickedUp;
    public int MeatID;
    public bool PlaysAcquireWhenGrabbed;
    private bool m_hasPlayedAcquire;

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (this.PlaysAcquireWhenGrabbed && !this.m_hasPlayedAcquire)
      {
        this.m_hasPlayedAcquire = true;
        GM.MGMaster.Narrator.PlayMeatAcquire(this.MeatID);
      }
      base.BeginInteraction(hand);
    }

    public override bool IsInteractable() => this.CanMeatBePickedUp;

    public override bool IsDistantGrabbable() => this.m_hasPlayedAcquire && base.IsDistantGrabbable();
  }
}
