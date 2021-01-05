// Decompiled with JetBrains decompiler
// Type: FistVR.Flipzo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Flipzo : FVRPhysicalObject
  {
    private bool m_isOpen;
    private bool m_isLit;
    private bool isTouching;
    private Vector2 initTouch = Vector2.zero;
    private Vector2 LastTouchPoint = Vector2.zero;
    public Transform Lid;
    private float m_lid_startRot;
    private float m_lid_endRot = -150f;
    private float m_lid_curRot;
    public Transform Spring;
    private float m_spring_startRot = 248f;
    private float m_spring_end_rot = 380f;
    private float m_spring_curRot = 248f;
    public Transform Flame;
    private float m_flame_min = 0.1f;
    private float m_flame_max = 0.85f;
    private float m_flame_cur = 0.1f;
    public AudioSource Audio_Lighter;
    public AudioClip AudioClip_Open;
    public AudioClip AudioClip_Close;
    public AudioClip AudioClip_Strike;
    public Transform[] FlameJoints;
    public float[] FlameWeights;
    public ParticleSystem Sparks;
    public AlloyAreaLight AlloyLight;
    public LayerMask LM_FireDamage;
    private RaycastHit m_hit;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
        {
          if (this.m_isOpen)
            this.Close();
          else
            this.Open();
        }
        if (hand.Input.AXButtonDown && this.m_isOpen)
          this.Light();
      }
      else
      {
        if (hand.Input.TouchpadTouched && !this.isTouching && hand.Input.TouchpadAxes != Vector2.zero)
        {
          this.isTouching = true;
          this.initTouch = hand.Input.TouchpadAxes;
        }
        if (hand.Input.TouchpadTouchUp && this.isTouching)
        {
          this.isTouching = false;
          float y = (this.LastTouchPoint - this.initTouch).y;
          if ((double) y > 0.5)
            this.Open();
          else if ((double) y < -0.5 && this.m_isOpen)
            this.Light();
          this.initTouch = Vector2.zero;
          Vector2 zero = Vector2.zero;
        }
        this.LastTouchPoint = hand.Input.TouchpadAxes;
      }
      if (hand.Input.TriggerDown)
        this.Close();
      float x = this.transform.InverseTransformDirection(hand.Input.VelAngularWorld).x;
      if ((double) x > 15.0)
        this.Open();
      else if ((double) x < -15.0)
        this.Close();
      if (!this.m_isOpen || this.m_isLit || !((Object) this.m_hand.OtherHand != (Object) null))
        return;
      Vector3 velLinearWorld = this.m_hand.OtherHand.Input.VelLinearWorld;
      if ((double) Vector3.Distance(this.m_hand.OtherHand.PalmTransform.position, this.transform.position) >= 0.200000002980232 || (double) Vector3.Angle(velLinearWorld, this.transform.up) <= 110.0 || (double) velLinearWorld.magnitude <= 2.0)
        return;
      this.Light();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_isOpen)
      {
        this.m_lid_curRot = Mathf.Lerp(this.m_lid_curRot, this.m_lid_endRot, Time.deltaTime * 9f);
        this.m_spring_curRot = Mathf.Lerp(this.m_spring_curRot, this.m_spring_end_rot, Time.deltaTime * 9f);
      }
      else
      {
        this.m_lid_curRot = Mathf.Lerp(this.m_lid_curRot, this.m_lid_startRot, Time.deltaTime * 15f);
        this.m_spring_curRot = Mathf.Lerp(this.m_spring_curRot, this.m_spring_startRot, Time.deltaTime * 9f);
        if ((double) this.m_lid_curRot < 1.0)
          this.Flame.gameObject.SetActive(false);
      }
      if (this.m_isLit)
      {
        this.m_flame_cur = Mathf.Lerp(this.m_flame_cur, this.m_flame_max, Time.deltaTime * 2f);
        this.AlloyLight.Intensity = this.m_flame_cur * (float) ((double) Mathf.PerlinNoise(Time.time * 10f, this.AlloyLight.transform.position.y) * 0.0500000007450581 + 0.5);
      }
      else
        this.m_flame_cur = Mathf.Lerp(this.m_flame_cur, this.m_flame_min, Time.deltaTime * 7f);
      this.Lid.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_lid_curRot);
      this.Spring.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_spring_curRot);
      this.Flame.localScale = new Vector3(this.m_flame_cur, this.m_flame_cur, this.m_flame_cur);
      Quaternion b1 = Quaternion.Slerp(Quaternion.Inverse(this.transform.rotation), Random.rotation, Mathf.PerlinNoise(Time.time * 5f, 0.0f) * 0.3f);
      for (int index = 0; index < this.FlameJoints.Length; ++index)
      {
        Quaternion b2 = Quaternion.Slerp(Quaternion.identity, b1, this.FlameWeights[index] + Random.Range(-0.05f, 0.05f));
        this.FlameJoints[index].localScale = new Vector3(Random.Range(0.95f, 1.05f), Random.Range(0.98f, 1.02f), Random.Range(0.95f, 1.05f));
        this.FlameJoints[index].localRotation = Quaternion.Slerp(this.FlameJoints[index].localRotation, b2, Time.deltaTime * 6f);
      }
      if (!this.m_isLit)
        return;
      Vector3 position = this.FlameJoints[0].position;
      Vector3 vector3 = this.FlameJoints[this.FlameJoints.Length - 1].position - position;
      if (!Physics.Raycast(position, vector3.normalized, out this.m_hit, vector3.magnitude, (int) this.LM_FireDamage, QueryTriggerInteraction.Collide))
        return;
      IFVRDamageable component1 = this.m_hit.collider.gameObject.GetComponent<IFVRDamageable>();
      if (component1 == null && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
        component1 = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component1 != null)
        component1.Damage(new Damage()
        {
          Class = Damage.DamageClass.Explosive,
          Dam_Thermal = 50f,
          Dam_TotalEnergetic = 50f,
          point = this.m_hit.point,
          hitNormal = this.m_hit.normal,
          strikeDir = this.transform.forward
        });
      FVRIgnitable component2 = this.m_hit.collider.transform.gameObject.GetComponent<FVRIgnitable>();
      if ((Object) component2 == (Object) null && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
        this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
      if (!((Object) component2 != (Object) null))
        return;
      FXM.Ignite(component2, 0.1f);
    }

    private void Open()
    {
      if (this.m_isOpen)
        return;
      this.m_isOpen = true;
      this.Audio_Lighter.PlayOneShot(this.AudioClip_Open, 0.3f);
    }

    private void Close()
    {
      if (!this.m_isOpen)
        return;
      this.m_isLit = false;
      this.m_isOpen = false;
      this.Audio_Lighter.PlayOneShot(this.AudioClip_Close, 0.3f);
    }

    private void Light()
    {
      this.Sparks.Emit(Random.Range(2, 3));
      if (this.m_isLit)
        return;
      this.m_isLit = true;
      this.Audio_Lighter.PlayOneShot(this.AudioClip_Strike, 0.3f);
      this.Flame.gameObject.SetActive(true);
    }
  }
}
