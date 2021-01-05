// Decompiled with JetBrains decompiler
// Type: FistVR.wwGatlingGunBaseHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwGatlingGunBaseHandle : FVRInteractiveObject
  {
    public Transform GunBase;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.GunBase.transform.rotation = Quaternion.Slerp(this.GunBase.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(-(hand.transform.position - this.GunBase.position), Vector3.up), Vector3.up), Time.deltaTime * 4f);
    }
  }
}
