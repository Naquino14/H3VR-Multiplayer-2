// Decompiled with JetBrains decompiler
// Type: FistVR.AnimalNoiseMaker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AnimalNoiseMaker : FVRPhysicalObject
  {
    private AudioSource m_aud;
    private float soundChargedUp;
    private float m_curVolume;
    private float m_tarVolume;
    public bool UsesMultipleClips;
    public AudioClip[] clips;
    private int m_numClip;
    private bool m_hasSoundPlayed;
    public float SoundDrainSpeed = 1f;
    public string BangerText;
    public GameObject BangerSplosion;
    [Header("SpawnOnHit")]
    public bool SpawnOnHit;
    public FVRObject SpawnOnHitObject;
    private bool m_isPrimed;
    private bool m_hasSpawned;
    public GameObject SpawnCloud;

    protected override void Awake()
    {
      base.Awake();
      this.m_aud = this.GetComponent<AudioSource>();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((Object) this.QuickbeltSlot != (Object) null)
        this.m_isPrimed = false;
      this.m_curVolume = Mathf.Lerp(this.m_curVolume, this.m_tarVolume, Time.deltaTime * 3f);
      this.m_aud.volume = this.m_curVolume;
      if ((double) Vector3.Dot(this.transform.up, Vector3.up) < -0.100000001490116)
      {
        this.soundChargedUp += Time.deltaTime * 2f;
        this.soundChargedUp = Mathf.Clamp(this.soundChargedUp, 0.0f, 1.7f);
        this.m_tarVolume = 0.0f;
        this.m_hasSoundPlayed = false;
      }
      else
      {
        if ((double) Vector3.Dot(this.transform.up, Vector3.up) <= 0.300000011920929)
          return;
        if ((double) this.soundChargedUp > 0.0)
        {
          this.soundChargedUp -= Time.deltaTime * this.SoundDrainSpeed;
          if (!this.m_aud.isPlaying && !this.m_hasSoundPlayed)
          {
            this.m_hasSoundPlayed = true;
            if (this.UsesMultipleClips)
            {
              ++this.m_numClip;
              if (this.m_numClip >= this.clips.Length)
                this.m_numClip = 0;
              this.m_aud.clip = this.clips[this.m_numClip];
            }
            this.m_isPrimed = true;
            this.m_aud.Play();
          }
          this.m_tarVolume = 1f;
        }
        else
          this.m_tarVolume = 0.0f;
        this.m_aud.pitch = Mathf.Clamp(Vector3.Dot(this.transform.up, Vector3.down) * -2f, 0.9f, 1f);
      }
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if (!this.SpawnOnHit || !this.m_isPrimed || (this.m_hasSpawned || (Object) col.collider.attachedRigidbody != (Object) null) || (double) Vector3.Angle(col.contacts[0].normal, Vector3.up) > 15.0)
        return;
      this.m_hasSpawned = true;
      Vector3 point = col.contacts[0].point;
      Vector3 forward = GM.CurrentPlayerBody.Head.forward;
      forward.y = 0.0f;
      forward.Normalize();
      Object.Instantiate<GameObject>(this.SpawnOnHitObject.GetGameObject(), point, Quaternion.LookRotation(forward, Vector3.up));
      Object.Instantiate<GameObject>(this.SpawnCloud, point, Quaternion.LookRotation(forward, Vector3.up));
      Object.Destroy((Object) this.gameObject);
    }
  }
}
