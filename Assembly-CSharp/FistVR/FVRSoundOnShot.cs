// Decompiled with JetBrains decompiler
// Type: FistVR.FVRSoundOnShot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRSoundOnShot : MonoBehaviour, IFVRDamageable
  {
    private AudioSource m_aud;
    private float timeSinceShot;

    private void Awake() => this.m_aud = this.GetComponent<AudioSource>();

    private void Update()
    {
      if ((double) this.timeSinceShot >= 1.0)
        return;
      this.timeSinceShot += Time.deltaTime;
    }

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile || (double) this.timeSinceShot < 0.200000002980232)
        return;
      this.timeSinceShot = 0.0f;
      this.m_aud.pitch = Random.Range(0.95f, 1.05f);
      this.m_aud.PlayOneShot(this.m_aud.clip, Random.Range(0.2f, 0.3f));
    }
  }
}
