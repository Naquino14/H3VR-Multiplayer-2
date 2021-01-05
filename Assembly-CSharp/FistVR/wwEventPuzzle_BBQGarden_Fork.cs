// Decompiled with JetBrains decompiler
// Type: FistVR.wwEventPuzzle_BBQGarden_Fork
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwEventPuzzle_BBQGarden_Fork : MonoBehaviour, IFVRDamageable
  {
    public wwEventPuzzle_BBQGarden Garden;
    public bool isFork;
    public int ForkIndex;
    public AudioSource Aud;
    private float timeSincePlay;

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile || (double) this.timeSincePlay <= 0.200000002980232)
        return;
      this.Aud.PlayOneShot(this.Aud.clip, 0.4f);
      if (this.isFork)
        this.Garden.ForkHit(this.ForkIndex);
      else
        this.Aud.pitch = Random.Range(0.97f, 1.03f);
    }

    private void Update()
    {
      if ((double) this.timeSincePlay >= 2.0)
        return;
      this.timeSincePlay += Time.deltaTime;
    }
  }
}
