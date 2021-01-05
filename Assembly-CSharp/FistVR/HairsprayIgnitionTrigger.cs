// Decompiled with JetBrains decompiler
// Type: FistVR.HairsprayIgnitionTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HairsprayIgnitionTrigger : MonoBehaviour, IFVRDamageable
  {
    public HairsprayCan can;

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal <= 0.0)
        return;
      this.can.Ignite();
    }

    public void OnParticleCollision(GameObject other)
    {
      if (!other.CompareTag("IgnitorSystem"))
        return;
      this.can.Ignite();
    }
  }
}
