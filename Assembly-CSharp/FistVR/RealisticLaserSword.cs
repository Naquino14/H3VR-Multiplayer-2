// Decompiled with JetBrains decompiler
// Type: FistVR.RealisticLaserSword
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RealisticLaserSword : FVRPhysicalObject
  {
    public Transform OuterBeam;
    public Transform InnerBeam;
    public GameObject ImpactPointPrefab;
    private GameObject ImpactPoint;
    private TrailRenderer BurnTrail;
    public ParticleSystem ImpactSparks;
    public ParticleSystem GlowSprites;
    public LayerMask LaserMask;
    public LayerMask TargetMask;
    private RaycastHit m_hit;
    public Transform Aperture;
    private Vector3 LastPoint1 = Vector3.zero;
    private Vector3 LastPoint2 = Vector3.zero;
    private Vector3 LastNormal = Vector3.zero;
    public int maxCasts = 3;
    private bool m_isBeamActive;
    private bool m_wasBeamActive;
    public AudioSource humm;

    protected override void Awake()
    {
      base.Awake();
      this.ImpactPoint = Object.Instantiate<GameObject>(this.ImpactPointPrefab, this.transform.position, Quaternion.identity);
      this.ImpactPoint.SetActive(false);
    }

    public override void BeginInteraction(FVRViveHand hand) => base.BeginInteraction(hand);

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (hand.Input.TriggerPressed && this.m_hasTriggeredUpSinceBegin)
      {
        this.m_isBeamActive = true;
        this.OuterBeam.gameObject.SetActive(true);
        if (this.humm.isPlaying)
          return;
        this.humm.Play();
      }
      else
      {
        this.m_isBeamActive = false;
        this.OuterBeam.gameObject.SetActive(false);
        this.ImpactPoint.SetActive(false);
        this.humm.Stop();
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_isBeamActive = false;
      this.OuterBeam.gameObject.SetActive(false);
      this.ImpactPoint.SetActive(false);
      this.humm.Stop();
      base.EndInteraction(hand);
    }

    private void GenerateNewTrail(Vector3 point)
    {
      this.ImpactPoint = Object.Instantiate<GameObject>(this.ImpactPointPrefab, point, Quaternion.identity);
      this.BurnTrail = this.ImpactPoint.GetComponent<TrailRenderer>();
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.m_isBeamActive)
      {
        float z = 100f;
        Vector3 vector3_1 = this.Aperture.position + this.Aperture.forward * 100f;
        if (Physics.Raycast(this.Aperture.position, this.Aperture.forward, out this.m_hit, 100f, (int) this.LaserMask, QueryTriggerInteraction.Collide))
        {
          z = this.m_hit.distance;
          vector3_1 = this.m_hit.point;
          this.ImpactPoint.SetActive(true);
          this.ImpactPoint.transform.position = this.m_hit.point;
          if (this.m_hit.collider.gameObject.layer == LayerMask.NameToLayer("Flammable"))
            this.m_hit.collider.gameObject.BroadcastMessage("Ignite", SendMessageOptions.DontRequireReceiver);
        }
        else
          this.ImpactPoint.SetActive(false);
        this.OuterBeam.localScale = new Vector3(0.012f, 0.012f, z);
        if (!this.m_wasBeamActive)
        {
          this.LastPoint1 = this.Aperture.position;
          this.LastPoint2 = vector3_1;
        }
        else
        {
          for (int index = 0; index < this.maxCasts; ++index)
          {
            float t = (float) index / ((float) this.maxCasts - 1f);
            Vector3 origin = Vector3.Lerp(this.LastPoint1, this.Aperture.position, t);
            Vector3 vector3_2 = Vector3.Lerp(this.LastPoint2, this.Aperture.position, t) - origin;
            if (Physics.Raycast(origin, vector3_2.normalized, out this.m_hit, 100f, (int) this.TargetMask, QueryTriggerInteraction.Collide))
            {
              this.ImpactSparks.transform.position = this.m_hit.point + this.m_hit.normal * 0.05f;
              this.ImpactSparks.Emit(1);
              if ((Object) this.m_hit.collider.attachedRigidbody != (Object) null && this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>() != null)
              {
                if ((bool) (Object) this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>())
                  FXM.Ignite(this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>(), 1f);
                this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>().Damage(new Damage()
                {
                  Class = Damage.DamageClass.Projectile,
                  Dam_Piercing = 1f,
                  Dam_TotalKinetic = 1f,
                  Dam_Thermal = 10f,
                  Dam_TotalEnergetic = 10f,
                  point = this.m_hit.point,
                  hitNormal = this.m_hit.normal,
                  strikeDir = this.transform.forward
                });
              }
            }
          }
        }
        this.LastPoint1 = this.Aperture.position;
        this.LastPoint2 = vector3_1;
      }
      this.m_wasBeamActive = this.m_isBeamActive;
    }
  }
}
