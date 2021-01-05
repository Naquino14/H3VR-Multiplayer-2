// Decompiled with JetBrains decompiler
// Type: FistVR.HairsprayCan
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HairsprayCan : FVRPhysicalObject, IFVRDamageable
  {
    public Transform Tip;
    public Vector2 TipRange;
    private bool m_isSpraying;
    private bool m_wasSpraying;
    private bool m_isIgnited;
    private bool m_wasIgnited;
    public LayerMask LM_sprayCast;
    private RaycastHit m_hit;
    public string Filltag;
    public float SprayDistance;
    public float SprayAngle;
    public AudioEvent AudEvent_Head;
    public AudioEvent AudEvent_Tail;
    public AudioSource AudSource_Loop;
    public AudioClip AudClip_Spray;
    public AudioClip AudClip_Fire;
    public Transform Muzzle;
    public ParticleSystem PSystem_Spray;
    public GameObject Proj_Fire;
    public GameObject IgnitorTrigger;
    public AudioEvent AudEvent_Rattle;
    public float RattleRadius;
    public float RattleHeight;
    private Vector3 m_rattlePos = Vector3.zero;
    private Vector3 m_rattleLastPos = Vector3.zero;
    private Vector3 m_rattleVel = Vector3.zero;
    private bool m_israttleSide;
    private bool m_wasrattleSide;
    public Transform ballviz;
    private bool m_hasExploed;
    public GameObject Splode;
    private float m_timeTilCast = 0.03f;

    public void Damage(FistVR.Damage d)
    {
      if (d.Class != FistVR.Damage.DamageClass.Projectile && ((double) d.Dam_Thermal <= 0.0 || !this.m_isSpraying))
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_hasExploed)
        return;
      this.m_hasExploed = true;
      Object.Instantiate<GameObject>(this.Splode, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    private void RattleUpdate()
    {
      this.m_wasrattleSide = this.m_israttleSide;
      if (this.m_wasrattleSide)
        this.m_rattleVel = this.RootRigidbody.velocity;
      else
        this.m_rattleVel -= this.RootRigidbody.GetPointVelocity(this.transform.TransformPoint(this.m_rattlePos)) * Time.deltaTime;
      this.m_rattleVel += Vector3.up * -0.5f * Time.deltaTime;
      Vector3 vector3 = this.transform.InverseTransformDirection(this.m_rattleVel);
      this.m_rattlePos += vector3 * Time.deltaTime;
      float num1 = this.m_rattlePos.y;
      Vector2 vector2 = new Vector2(this.m_rattlePos.x, this.m_rattlePos.z);
      this.m_israttleSide = false;
      float magnitude = vector2.magnitude;
      if ((double) magnitude > (double) this.RattleRadius)
      {
        float num2 = this.RattleRadius - magnitude;
        vector2 = (Vector2) Vector3.ClampMagnitude((Vector3) vector2, this.RattleRadius);
        num1 += num2 * Mathf.Sign(num1);
        vector3 = Vector3.ProjectOnPlane(vector3, new Vector3(vector2.x, 0.0f, vector2.y));
        this.m_israttleSide = true;
      }
      if ((double) Mathf.Abs(num1) > (double) this.RattleHeight)
      {
        num1 = this.RattleHeight * Mathf.Sign(num1);
        vector3.y = 0.0f;
        this.m_israttleSide = true;
      }
      this.m_rattlePos = new Vector3(vector2.x, num1, vector2.y);
      this.m_rattleVel = this.transform.TransformDirection(vector3);
      this.ballviz.localPosition = this.m_rattlePos;
      if (this.m_israttleSide && !this.m_wasrattleSide)
      {
        float num2 = Mathf.Clamp((float) (4.0 * ((double) Vector3.Distance(this.m_rattlePos, this.m_rattleLastPos) / (double) this.RattleRadius)), 0.0f, 1f);
        SM.PlayCoreSoundOverrides(FVRPooledAudioType.Casings, this.AudEvent_Rattle, this.ballviz.position, new Vector2(num2 * 0.4f, num2 * 0.4f), new Vector2(1f, 1f));
      }
      this.m_rattleLastPos = this.m_rattlePos;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.m_isSpraying = false;
      if (!this.m_hasTriggeredUpSinceBegin)
        return;
      this.SetAnimatedComponent(this.Tip, Mathf.Lerp(this.TipRange.x, this.TipRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Y);
      if ((double) hand.Input.TriggerFloat <= 0.800000011920929)
        return;
      this.m_isSpraying = true;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.SetAnimatedComponent(this.Tip, this.TipRange.x, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Y);
      this.m_isSpraying = false;
    }

    public void Ignite()
    {
      if (!this.m_isSpraying)
        return;
      this.m_isIgnited = true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.RattleUpdate();
      if (this.m_isSpraying)
      {
        this.m_timeTilCast -= Time.deltaTime;
        if (this.m_isIgnited)
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.Proj_Fire, this.Muzzle.position, this.Muzzle.rotation);
          gameObject.transform.Rotate(new Vector3(Random.Range(-this.SprayAngle, this.SprayAngle), Random.Range(-this.SprayAngle, this.SprayAngle), 0.0f));
          BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
          component.Fire(component.MuzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) null);
        }
        else
        {
          this.PSystem_Spray.Emit(10);
          if ((double) this.m_timeTilCast < 0.0)
          {
            this.m_timeTilCast = 0.03f;
            Vector3 forward = this.Muzzle.forward;
            Vector3 vector3_1 = Quaternion.AngleAxis(Random.Range((float) (-(double) this.SprayAngle * 3.0), this.SprayAngle * 3f), this.Muzzle.up) * forward;
            Vector3 vector3_2 = Quaternion.AngleAxis(Random.Range((float) (-(double) this.SprayAngle * 3.0), this.SprayAngle * 3f), this.Muzzle.right) * vector3_1;
            if (Physics.Raycast(this.Muzzle.position, this.Muzzle.forward, out this.m_hit, this.SprayDistance, (int) this.LM_sprayCast, QueryTriggerInteraction.Ignore) && this.m_hit.collider.gameObject.CompareTag(this.Filltag))
              this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<PotatoGun>().InsertGas(0.04f);
          }
        }
      }
      if (this.m_isSpraying && !this.m_wasSpraying)
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Head, this.Muzzle.position);
        this.AudSource_Loop.clip = this.AudClip_Spray;
        this.AudSource_Loop.Play();
      }
      else if (this.m_wasSpraying && !this.m_isSpraying)
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Tail, this.Muzzle.position);
        this.AudSource_Loop.Stop();
      }
      if (this.m_isSpraying && !this.m_isIgnited)
      {
        if (!this.IgnitorTrigger.activeSelf)
          this.IgnitorTrigger.SetActive(true);
      }
      else if (this.IgnitorTrigger.activeSelf)
        this.IgnitorTrigger.SetActive(false);
      if (!this.m_isSpraying)
        this.m_isIgnited = false;
      if (this.m_isIgnited && !this.m_wasIgnited)
      {
        this.AudSource_Loop.clip = this.AudClip_Fire;
        this.AudSource_Loop.Play();
      }
      this.m_wasSpraying = this.m_isSpraying;
      this.m_wasIgnited = this.m_isIgnited;
    }
  }
}
