// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockDamageableSensor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockDamageableSensor : MonoBehaviour, IFVRDamageable
  {
    public FlintlockFlashPan Pan;
    public FlintlockBarrel Barrel;

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal <= 1.0)
        return;
      if ((Object) this.Pan != (Object) null && this.Pan.FrizenState == FlintlockFlashPan.FState.Up)
        this.Pan.Ignite();
      if (!((Object) this.Barrel != (Object) null))
        return;
      this.Barrel.BurnOffOuter();
    }
  }
}
