// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPlayerHitbox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRPlayerHitbox : MonoBehaviour, IFVRDamageable
  {
    public bool IsActivated;
    private AudioSource m_aud;
    private Rigidbody m_rb;
    public AudioClip AudClip_Reset;
    public AudioClip AudClip_Hit;
    public FVRPlayerBody Body;
    public FVRViveHand Hand;
    public float DamageResist;
    public float DamageMultiplier = 1f;
    public AIEntity MyE;
    public Transform TransformTarget;
    public FVRPlayerHitbox.PlayerHitBoxType Type;

    private void Awake()
    {
      this.m_aud = this.GetComponent<AudioSource>();
      this.m_rb = this.GetComponent<Rigidbody>();
    }

    public void UpdatePositions()
    {
      if (!((Object) this.TransformTarget != (Object) null))
        return;
      this.m_rb.MovePosition(this.TransformTarget.position);
      this.m_rb.MoveRotation(this.TransformTarget.rotation);
    }

    public virtual void Damage(float i)
    {
      if (!GM.CurrentSceneSettings.DoesDamageGetRegistered)
        return;
      float DamagePoints = Mathf.Clamp(i * this.DamageMultiplier - this.DamageResist, 0.0f, 10000f);
      if ((double) DamagePoints <= 0.100000001490116 || !this.IsActivated)
        return;
      if (this.Body.RegisterPlayerHit(DamagePoints, false))
      {
        this.m_aud.PlayOneShot(this.AudClip_Reset, 1f);
      }
      else
      {
        if (GM.IsDead())
          return;
        this.m_aud.PlayOneShot(this.AudClip_Hit, 1f);
      }
    }

    public void Damage(FistVR.Damage d)
    {
      if (!GM.CurrentSceneSettings.DoesDamageGetRegistered)
        return;
      if ((double) d.Dam_Blinding > 0.0 && this.Type == FVRPlayerHitbox.PlayerHitBoxType.Head && (double) Vector3.Angle(d.strikeDir, GM.CurrentPlayerBody.Head.forward) > 90.0)
        GM.CurrentPlayerBody.BlindPlayer(d.Dam_Blinding);
      if (GM.CurrentPlayerBody.IsBlort)
        d.Dam_TotalEnergetic = 0.0f;
      else if (GM.CurrentPlayerBody.IsDlort)
        d.Dam_TotalEnergetic *= 3f;
      float DamagePoints = d.Dam_TotalKinetic + d.Dam_TotalEnergetic * 1f;
      if (GM.CurrentPlayerBody.IsDamResist || GM.CurrentPlayerBody.IsDamMult)
      {
        float damageResist = GM.CurrentPlayerBody.GetDamageResist();
        if ((double) damageResist <= 0.00999999977648258)
          return;
        DamagePoints *= damageResist;
      }
      if ((double) DamagePoints <= 0.100000001490116 || !this.IsActivated)
        return;
      if (this.Body.RegisterPlayerHit(DamagePoints, false))
      {
        this.m_aud.PlayOneShot(this.AudClip_Reset, 0.4f);
      }
      else
      {
        if (GM.IsDead())
          return;
        this.m_aud.PlayOneShot(this.AudClip_Hit, 0.4f);
      }
    }

    public virtual void Damage(DamageDealt dam)
    {
      if (!GM.CurrentSceneSettings.DoesDamageGetRegistered || (Object) dam.SourceFirearm != (Object) null && this.Type == FVRPlayerHitbox.PlayerHitBoxType.Hand)
        return;
      float DamagePoints = Mathf.Clamp(dam.PointsDamage * this.DamageMultiplier - this.DamageResist, 0.0f, 10000f);
      if (!dam.IsInitialContact || (double) DamagePoints <= 0.100000001490116 || !this.IsActivated)
        return;
      if (this.Body.RegisterPlayerHit(DamagePoints, dam.IsPlayer))
      {
        this.m_aud.PlayOneShot(this.AudClip_Reset, 0.4f);
      }
      else
      {
        if (GM.IsDead())
          return;
        this.m_aud.PlayOneShot(this.AudClip_Hit, 0.4f);
      }
    }

    public enum PlayerHitBoxType
    {
      Head,
      Torso,
      Hand,
    }
  }
}
