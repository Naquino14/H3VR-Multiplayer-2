// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwMeatCore
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RotrwMeatCore : FVRPhysicalObject
  {
    public RotrwMeatCore.CoreType Type;
    public AudioEvent AudEvent_Eat;
    public List<GameObject> BangerSplosions;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      FVRViveHand fvrViveHand = hand;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f) >= 0.150000005960464)
        return;
      this.EndInteraction(hand);
      fvrViveHand.ForceSetInteractable((FVRInteractiveObject) null);
      SM.PlayGenericSound(this.AudEvent_Eat, this.transform.position);
      if ((Object) GM.ZMaster != (Object) null)
        GM.ZMaster.EatMeatCore(this.Type);
      Object.Destroy((Object) this.gameObject);
    }

    public enum CoreType
    {
      Tasty,
      Moldy,
      Spikey,
      Zippy,
      Weighty,
      Juicy,
      Shiny,
      Burny,
    }
  }
}
