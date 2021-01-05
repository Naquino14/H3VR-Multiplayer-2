// Decompiled with JetBrains decompiler
// Type: FistVR.wwTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwTarget : MonoBehaviour, IFVRDamageable
  {
    public string Ident;
    public Sprite TargetSprite;
    [Header("Hit Details")]
    public float TargetStruckRefire = 1f;
    private float m_targetStruckRefire = 1f;
    [Header("Sound Settings")]
    public float SoundRefire = 1f;
    private float m_soundRefireTick = 1f;
    public bool HasAudioHitSound = true;
    public AudioEvent AudioEvent;
    public FVRPooledAudioType AudioType;
    public wwTargetManager Manager;
    protected Vector3 m_originalPos;
    protected Quaternion m_originalRot;
    protected float m_originalScale;
    public float RespawnTime = 10f;
    public bool DoesRescale;
    protected bool hasManager;

    public void Awake()
    {
      this.m_targetStruckRefire = this.TargetStruckRefire;
      this.m_soundRefireTick = this.SoundRefire;
    }

    public void Start()
    {
      this.m_originalPos = this.transform.position;
      this.m_originalRot = this.transform.rotation;
      this.m_originalScale = this.transform.localScale.y;
    }

    public void SetManager(wwTargetManager m)
    {
      this.Manager = m;
      this.hasManager = true;
    }

    public void SetupAfterSpawn(
      wwTargetManager m,
      Vector3 pos,
      Quaternion rot,
      float scale,
      bool doesScale)
    {
      this.hasManager = true;
      this.Manager = m;
      this.m_originalPos = pos;
      this.m_originalRot = rot;
      this.m_originalScale = scale;
      this.DoesRescale = doesScale;
    }

    public virtual void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
        this.TargetStruck(d, true);
      else
        this.TargetStruck(d, false);
    }

    public virtual void Update()
    {
      if ((double) this.m_targetStruckRefire > 0.0)
        this.m_targetStruckRefire -= Time.deltaTime;
      if ((double) this.m_soundRefireTick <= 0.0)
        return;
      this.m_soundRefireTick -= Time.deltaTime;
    }

    public virtual void TargetStruck(FistVR.Damage dam, bool sendStruckEvent)
    {
      if ((double) this.m_soundRefireTick <= 0.0 && this.PlaySoundEvent())
        this.m_soundRefireTick = this.SoundRefire;
      if (!sendStruckEvent || (double) this.m_targetStruckRefire > 0.0)
        return;
      this.m_targetStruckRefire = this.TargetStruckRefire;
      this.SendStruckEvent();
    }

    public virtual bool PlaySoundEvent()
    {
      if (!this.HasAudioHitSound)
        return false;
      SM.PlayCoreSoundDelayed(this.AudioType, this.AudioEvent, this.transform.position, Vector3.Distance(GM.CurrentPlayerRoot.position, this.transform.position) / 343f);
      return true;
    }

    public void SendStruckEvent()
    {
      if (!this.hasManager)
        return;
      this.Manager.StruckEvent(this);
    }
  }
}
