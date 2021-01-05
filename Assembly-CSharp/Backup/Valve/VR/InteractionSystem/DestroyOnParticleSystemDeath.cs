// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.DestroyOnParticleSystemDeath
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (ParticleSystem))]
  public class DestroyOnParticleSystemDeath : MonoBehaviour
  {
    private ParticleSystem particles;

    private void Awake()
    {
      this.particles = this.GetComponent<ParticleSystem>();
      this.InvokeRepeating("CheckParticleSystem", 0.1f, 0.1f);
    }

    private void CheckParticleSystem()
    {
      if (this.particles.IsAlive())
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
