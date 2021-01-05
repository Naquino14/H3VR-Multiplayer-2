// Decompiled with JetBrains decompiler
// Type: FistVR.wwOldTimeySpectacles
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwOldTimeySpectacles : FVRPhysicalObject
  {
    private bool m_hasTurnedOn;
    public Texture2D tex;
    public GameObject EffectPrefab;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.m_hasTurnedOn || (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.EyeCam.transform.position) >= 0.200000002980232)
        return;
      this.m_hasTurnedOn = true;
      Object.Instantiate<GameObject>(this.EffectPrefab, Vector3.zero, Quaternion.identity);
      hand.ForceSetInteractable((FVRInteractiveObject) null);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
