// Decompiled with JetBrains decompiler
// Type: FistVR.AIFireArm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIFireArm : FVRDestroyableObject
  {
    [Header("AIFireArm Config")]
    public Transform Muzzle;
    public ParticleSystem PSystem_Smoke;
    public int SmokeAmount = 1;
    public float TrajectoryMuzzleVelocity;
    public float TrajectoryGravityMultiplier = 1f;
    public float ProjectileSpread;
    public int BurstLengthMin = 2;
    public int BurstLengthMax = 2;
    private int m_curBurstLength = 2;
    private int m_burstShotIndex;
    public float BoltMinRefireRate = 0.35f;
    public float BoltMaxRefireRate = 0.42f;
    private float m_boltRefireTick;
    public float BurstMinRefireRate = 1.2f;
    public float BurstMaxRefireRate = 1.6f;
    private float m_burstRefireTick;
    public GameObject ProjectilePrefab;
    private bool shouldFire;
    private AudioSource m_gunAudio;
    public AudioClip[] Aud_GunFire;
    public AudioClip Aud_Reload;
    public float FiringAngleThreshold = 5f;

    private new void Awake()
    {
      base.Awake();
      this.m_gunAudio = this.GetComponent<AudioSource>();
      this.m_curBurstLength = this.BurstLengthMin;
    }

    public void SetShouldFire(bool b) => this.shouldFire = b;

    public void UpdateWeaponSystem()
    {
      if ((double) this.m_burstRefireTick <= 0.0)
      {
        if ((double) this.m_boltRefireTick <= 0.0)
        {
          if (!this.shouldFire)
            return;
          if (this.m_burstShotIndex < this.m_curBurstLength)
          {
            this.m_boltRefireTick = Random.Range(this.BoltMinRefireRate, this.BoltMaxRefireRate);
            ++this.m_burstShotIndex;
            this.FireBullet();
          }
          else
          {
            this.m_gunAudio.PlayOneShot(this.Aud_Reload, 1f);
            this.m_burstShotIndex = 0;
            this.m_curBurstLength = Random.Range(this.BurstLengthMin, this.BurstLengthMax + 1);
            this.m_burstRefireTick = Random.Range(this.BurstMinRefireRate, this.BurstMaxRefireRate);
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
      this.PSystem_Smoke.Emit(this.SmokeAmount);
      GameObject gameObject = Object.Instantiate<GameObject>(this.ProjectilePrefab, this.Muzzle.position, this.Muzzle.rotation);
      gameObject.transform.Rotate(new Vector3(Random.Range(-this.ProjectileSpread, this.ProjectileSpread), Random.Range(-this.ProjectileSpread, this.ProjectileSpread), 0.0f));
      gameObject.GetComponent<BallisticProjectile>().Fire(gameObject.transform.forward, (FVRFireArm) null);
    }
  }
}
