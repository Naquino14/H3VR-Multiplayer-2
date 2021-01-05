// Decompiled with JetBrains decompiler
// Type: FistVR.AIMeleeWeapon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class AIMeleeWeapon : FVRDestroyableObject
  {
    [Header("Melee Weapon Params")]
    public Transform Spike;
    public Vector3 RetractedLocalPos;
    public Vector3 RetractedLocalScale;
    public Vector3 ExtendedLocalPos;
    public Vector3 ExtendedLocalScale;
    private Vector3 m_curLocalPos;
    private Vector3 m_tarLocalPos;
    private Vector3 m_curLocalScale;
    private Vector3 m_tarLocalScale;
    public AIMeleeWeapon.MeleeParticleEffectEvent[] Effects;
    public DamageDealt Dam;
    public Transform DamageCastPoint;
    public LayerMask DamageLM_world;
    public LayerMask DamageLM_player;
    private RaycastHit m_hit;
    public float DamageCastDistance;
    public Vector2 RefireRange;
    private float m_refireTick;
    private bool m_IsEnabled = true;
    private bool m_FireAtWill;
    public AudioSource MeleeAudSource;
    public AudioClip[] MeleeAudClips;
    public Vector2 MeleeVolumeRange;
    public Vector2 MeleePitchRange;

    public override void Awake()
    {
      base.Awake();
      this.m_curLocalPos = this.RetractedLocalPos;
      this.m_tarLocalPos = this.RetractedLocalPos;
      this.m_curLocalScale = this.RetractedLocalScale;
      this.m_tarLocalScale = this.RetractedLocalScale;
    }

    public override void Update()
    {
      base.Update();
      this.FiringSystem();
      this.m_curLocalPos = Vector3.Lerp(this.m_curLocalPos, this.m_tarLocalPos, Time.deltaTime * 32f);
      this.m_curLocalScale = Vector3.Lerp(this.m_curLocalScale, this.m_tarLocalScale, Time.deltaTime * 32f);
      this.Spike.localPosition = this.m_curLocalPos;
      this.Spike.localScale = this.m_curLocalScale;
    }

    private void FiringSystem()
    {
      if (!this.m_FireAtWill || !this.m_IsEnabled)
        return;
      if ((double) this.m_refireTick > 0.0)
      {
        this.m_refireTick -= Time.deltaTime;
      }
      else
      {
        this.Fire();
        this.m_refireTick = UnityEngine.Random.Range(this.RefireRange.x, this.RefireRange.y);
      }
    }

    public void SetFireAtWill(bool b) => this.m_FireAtWill = b;

    public override void DestroyEvent()
    {
      this.m_IsEnabled = false;
      this.Invoke("KillThis", 20f);
      base.DestroyEvent();
    }

    private void KillThis() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

    private void Fire()
    {
      this.MeleeAudSource.pitch = UnityEngine.Random.Range(this.MeleePitchRange.x, this.MeleePitchRange.y);
      this.MeleeAudSource.PlayOneShot(this.MeleeAudClips[UnityEngine.Random.Range(0, this.MeleeAudClips.Length)], UnityEngine.Random.Range(this.MeleeVolumeRange.x, this.MeleeVolumeRange.y));
      for (int index = 0; index < this.Effects.Length; ++index)
        this.Effects[index].System.Emit(UnityEngine.Random.Range((int) this.Effects[index].EmitAmount.x, (int) this.Effects[index].EmitAmount.y));
      float t = 1f;
      if (Physics.Raycast(this.DamageCastPoint.position, this.DamageCastPoint.forward, out this.m_hit, this.DamageCastDistance, (int) this.DamageLM_world, QueryTriggerInteraction.Ignore))
      {
        Damage dam = new Damage();
        dam.Class = Damage.DamageClass.Melee;
        dam.Dam_Piercing = 700f;
        dam.Dam_TotalKinetic = 700f;
        dam.point = this.m_hit.point;
        dam.hitNormal = this.m_hit.normal;
        dam.strikeDir = this.transform.forward;
        IFVRDamageable component = this.m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
        if (component != null)
          component.Damage(dam);
        else if (component == null && (UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null)
          this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>()?.Damage(dam);
        if ((UnityEngine.Object) this.Spike != (UnityEngine.Object) null)
        {
          t = (float) (1.0 - (double) this.m_hit.distance / (double) this.DamageCastDistance);
          this.m_tarLocalPos = Vector3.Lerp(this.RetractedLocalPos, this.ExtendedLocalPos, t);
          this.m_tarLocalScale = Vector3.Lerp(this.RetractedLocalScale, this.ExtendedLocalScale, t);
          this.Invoke("Retract", 0.2f);
        }
      }
      else if ((UnityEngine.Object) this.Spike != (UnityEngine.Object) null)
      {
        this.m_tarLocalPos = this.ExtendedLocalPos;
        this.m_tarLocalScale = this.ExtendedLocalScale;
        this.Invoke("Retract", 0.2f);
      }
      if (!Physics.Raycast(this.DamageCastPoint.position, this.DamageCastPoint.forward, out this.m_hit, t * this.DamageCastDistance, (int) this.DamageLM_player, QueryTriggerInteraction.Collide))
        return;
      this.Dam.force = this.DamageCastPoint.forward;
      this.Dam.hitNormal = this.m_hit.normal;
      this.Dam.IsInitialContact = true;
      this.Dam.IsInside = false;
      this.Dam.IsMelee = true;
      this.Dam.point = this.m_hit.point;
      this.Dam.ShotOrigin = (Transform) null;
      this.Dam.PointsDamage = 700f;
      this.Dam.strikeDir = this.Dam.force;
      IFVRReceiveDamageable component1 = this.m_hit.collider.transform.gameObject.GetComponent<IFVRReceiveDamageable>();
      if (component1 != null)
      {
        component1.Damage(this.Dam);
      }
      else
      {
        if (component1 != null || !((UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null))
          return;
        this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRReceiveDamageable>()?.Damage(this.Dam);
      }
    }

    private void Retract()
    {
      if (!((UnityEngine.Object) this.Spike != (UnityEngine.Object) null))
        return;
      this.m_tarLocalPos = this.RetractedLocalPos;
      this.m_tarLocalScale = this.RetractedLocalScale;
    }

    [Serializable]
    public class MeleeParticleEffectEvent
    {
      public ParticleSystem System;
      public Vector2 EmitAmount;
    }
  }
}
