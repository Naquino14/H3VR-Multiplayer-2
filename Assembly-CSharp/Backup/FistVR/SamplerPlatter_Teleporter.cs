// Decompiled with JetBrains decompiler
// Type: FistVR.SamplerPlatter_Teleporter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SamplerPlatter_Teleporter : MonoBehaviour
  {
    public int FromIndex;
    public int NextIndex;
    public Transform[] TeleportPointArray;
    public AudioEvent ClickEvent;

    public void TeleportToFromPoint() => this.TeleportToIndex(this.FromIndex);

    public void TeleportToNextPoint() => this.TeleportToIndex(this.NextIndex);

    public void TeleportToIndex(int i)
    {
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.TeleportPointArray[i].position, true, this.TeleportPointArray[i].forward);
      SM.PlayGenericSound(this.ClickEvent, this.transform.position);
    }
  }
}
