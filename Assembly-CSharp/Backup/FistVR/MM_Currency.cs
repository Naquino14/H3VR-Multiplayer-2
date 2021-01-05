// Decompiled with JetBrains decompiler
// Type: FistVR.MM_Currency
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_Currency : FVRPhysicalObject
  {
    [Header("Currency Stuff")]
    public MMCurrency Type;
    private bool hasBeenCollected;
    public GameObject OnCollectSpawn;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      GM.MMFlags.LearnCurrency(this.Type);
      GM.MMFlags.SaveToFile();
    }

    public void Update()
    {
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + Vector3.up * -0.1f) >= 0.150000005960464)
        return;
      this.Collect(this.m_hand);
    }

    private void Collect(FVRViveHand hand)
    {
      if (this.hasBeenCollected)
        return;
      this.hasBeenCollected = true;
      GM.MMFlags.CollectCurrency(this.Type, 1);
      if ((Object) this.OnCollectSpawn != (Object) null)
        Object.Instantiate<GameObject>(this.OnCollectSpawn, this.transform.position, this.transform.rotation);
      if ((Object) hand != (Object) null)
      {
        FVRViveHand fvrViveHand = hand;
        this.EndInteraction(hand);
        fvrViveHand.ForceSetInteractable((FVRInteractiveObject) null);
      }
      GM.MMFlags.SaveToFile();
      Object.Destroy((Object) this.gameObject);
    }
  }
}
