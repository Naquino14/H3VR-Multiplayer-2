// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.BalloonHapticBump
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class BalloonHapticBump : MonoBehaviour
  {
    public GameObject physParent;

    private void OnCollisionEnter(Collision other)
    {
      if (!((Object) other.collider.GetComponentInParent<Balloon>() != (Object) null))
        return;
      Hand componentInParent = this.physParent.GetComponentInParent<Hand>();
      if (!((Object) componentInParent != (Object) null))
        return;
      componentInParent.TriggerHapticPulse((ushort) 500);
    }
  }
}
