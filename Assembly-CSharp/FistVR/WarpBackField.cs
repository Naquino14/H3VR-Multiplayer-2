// Decompiled with JetBrains decompiler
// Type: FistVR.WarpBackField
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class WarpBackField : MonoBehaviour
  {
    public Transform WarpPoint;
    public AudioEvent AudEvent_TP;

    private void OnTriggerEnter(Collider other)
    {
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.WarpPoint.position, true, this.WarpPoint.forward);
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_TP, this.WarpPoint.position);
    }
  }
}
