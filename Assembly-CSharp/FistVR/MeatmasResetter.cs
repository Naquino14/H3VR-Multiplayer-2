// Decompiled with JetBrains decompiler
// Type: FistVR.MeatmasResetter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MeatmasResetter : MonoBehaviour
  {
    public FVRPhysicalObject PO;
    private bool isLoading;

    private void Start()
    {
    }

    private void Update()
    {
      if (!this.PO.IsHeld || (double) Vector3.Distance(GM.CurrentPlayerBody.Head.position, this.transform.position) >= 0.150000005960464)
        return;
      this.ResetMeatmas();
    }

    private void ResetMeatmas()
    {
      if (this.isLoading)
        return;
      this.GetComponent<AudioSource>().Play();
      this.isLoading = true;
      SteamVR_LoadLevel.Begin("Xmas");
    }
  }
}
