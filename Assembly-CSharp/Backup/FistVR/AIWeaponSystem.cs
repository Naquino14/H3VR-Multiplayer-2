// Decompiled with JetBrains decompiler
// Type: FistVR.AIWeaponSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIWeaponSystem : MonoBehaviour
  {
    public Transform Muzzle;
    public float FiringAngleThreshold = 5f;
    private int m_burstLength = 3;
    private int m_burstShotIndex;
    private float m_boltRefireRate = 0.42f;
    private float m_boltRefireTick;
    private float m_burstRefireRate = 1.2f;
    private float m_burstRefireTick;
    public GameObject BulletPrefab;
    public ParticleSystem SmokePSystem;
    private bool shouldFire;
    private AudioSource m_gunAudio;
    public AudioClip[] Aud_GunFire;
    public AudioClip Aud_Reload;

    private void Awake() => this.m_gunAudio = this.GetComponent<AudioSource>();

    public void SetShouldFire(bool b) => this.shouldFire = b;

    public void UpdateWeaponSystem()
    {
      if ((double) this.m_burstRefireTick <= 0.0)
      {
        if ((double) this.m_boltRefireTick <= 0.0)
        {
          if (!this.shouldFire)
            return;
          if (this.m_burstShotIndex < this.m_burstLength)
          {
            this.m_boltRefireTick = this.m_boltRefireRate;
            ++this.m_burstShotIndex;
            this.FireBullet();
          }
          else
          {
            this.m_gunAudio.PlayOneShot(this.Aud_Reload, 1f);
            this.m_burstShotIndex = 0;
            this.m_burstRefireTick = this.m_burstRefireRate;
          }
        }
        else
          this.m_boltRefireTick -= Time.deltaTime;
      }
      else
        this.m_burstRefireTick -= Time.deltaTime;
    }

    private void FireBullet()
    {
      this.m_gunAudio.PlayOneShot(this.Aud_GunFire[Random.Range(0, this.Aud_GunFire.Length)], 1f);
      this.SmokePSystem.Emit(1);
      Object.Instantiate<GameObject>(this.BulletPrefab, this.Muzzle.position, this.Muzzle.rotation);
    }
  }
}
