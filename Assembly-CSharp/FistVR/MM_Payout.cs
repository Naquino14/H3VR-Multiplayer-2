// Decompiled with JetBrains decompiler
// Type: FistVR.MM_Payout
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_Payout : MonoBehaviour
  {
    public float Tick = 3f;
    public int Reward;

    private void Start() => this.Invoke("Dead", this.Tick);

    private void Dead()
    {
      GM.Omni.OmniUnlocks.GainCurrency(this.Reward);
      GM.Omni.SaveUnlocksToFile();
      Object.Destroy((Object) this.gameObject);
    }
  }
}
