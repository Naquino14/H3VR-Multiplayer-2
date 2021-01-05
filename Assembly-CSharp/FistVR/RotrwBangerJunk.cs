// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwBangerJunk
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RotrwBangerJunk : MonoBehaviour
  {
    public FVRPhysicalObject O;
    public AudioEvent AudEvent_Eat;
    public RotrwBangerJunk.BangerJunkType Type;
    public int MatIndex;
    public int ContainerSize = 1;

    public void Update()
    {
      if (!((Object) this.O != (Object) null) || !this.O.IsHeld)
        return;
      FVRViveHand hand = this.O.m_hand;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f) >= 0.150000005960464)
        return;
      this.O.EndInteraction(this.O.m_hand);
      hand.ForceSetInteractable((FVRInteractiveObject) null);
      SM.PlayGenericSound(this.AudEvent_Eat, this.transform.position);
      if ((Object) GM.ZMaster != (Object) null)
        GM.ZMaster.EatBangerJunk(this.Type, this.MatIndex);
      Object.Destroy((Object) this.gameObject);
    }

    public enum BangerJunkType
    {
      TinCan,
      CoffeeCan,
      Bucket,
      Radio,
      FishFinder,
      BangSnaps,
      EggTimer,
    }
  }
}
