// Decompiled with JetBrains decompiler
// Type: FistVR.AIStrikePlate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIStrikePlate : MonoBehaviour, IFVRDamageable
  {
    public int NumStrikesLeft = 1;
    protected int m_originalNumStrikesLeft = 1;
    private AudioSource m_aud;

    public void Awake()
    {
      this.m_originalNumStrikesLeft = this.NumStrikesLeft;
      this.m_aud = this.GetComponent<AudioSource>();
    }

    public virtual void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      this.m_aud.PlayOneShot(this.m_aud.clip, 1.2f);
      if (this.NumStrikesLeft > 0)
        --this.NumStrikesLeft;
      if (this.NumStrikesLeft != 0)
        return;
      this.PlateFelled();
    }

    public virtual void Reset() => this.NumStrikesLeft = this.m_originalNumStrikesLeft;

    public virtual void PlateFelled()
    {
    }
  }
}
