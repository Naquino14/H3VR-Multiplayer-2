// Decompiled with JetBrains decompiler
// Type: FistVR.MG_FlameSpout
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_FlameSpout : MonoBehaviour
  {
    public ParticleSystem PSystem_Ambient;
    public ParticleSystem PSystem_Spout;
    public Transform CastPoint;
    public Transform FlashPoint;
    public LayerMask CastLM;
    private RaycastHit m_hit;
    private bool m_isSpouting;
    private float m_TickDownTilSpout;
    private float m_TickDownTilIdle;
    private float m_DamageTick;
    public AudioEvent AudEvent_Spout;

    private void Awake()
    {
      this.m_isSpouting = false;
      this.m_TickDownTilSpout = Random.Range(5f, 20f);
      this.m_TickDownTilIdle = Random.Range(1.2f, 1.3f);
    }

    private void Update()
    {
      if (this.m_isSpouting)
      {
        this.m_TickDownTilIdle -= Time.deltaTime;
        if ((double) this.m_TickDownTilIdle <= 0.0)
        {
          this.m_TickDownTilIdle = Random.Range(1.2f, 1.3f);
          this.m_isSpouting = false;
          this.PSystem_Ambient.gameObject.SetActive(true);
          ParticleSystem.EmissionModule emission = this.PSystem_Spout.emission;
          ParticleSystem.MinMaxCurve rate = emission.rate;
          rate.mode = ParticleSystemCurveMode.Constant;
          rate.constantMax = 0.0f;
          rate.constantMin = 0.0f;
          emission.rate = rate;
        }
        if ((double) this.m_DamageTick >= 0.0)
        {
          this.m_DamageTick -= Time.deltaTime;
        }
        else
        {
          this.m_DamageTick = Random.Range(0.05f, 0.2f);
          FXM.InitiateMuzzleFlashLowPriority(this.FlashPoint.position + Random.onUnitSphere * 0.2f, this.FlashPoint.forward, Random.Range(0.5f, 3f), new Color(1f, 0.8f, 0.6f), Random.Range(0.5f, 1.5f));
          if (!Physics.Raycast(this.CastPoint.position, this.CastPoint.forward, out this.m_hit, 2f, (int) this.CastLM, QueryTriggerInteraction.Collide) || !((Object) this.m_hit.collider.attachedRigidbody != (Object) null))
            return;
          FVRPlayerHitbox component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
          if (!((Object) component != (Object) null))
            return;
          component.Damage(new DamageDealt()
          {
            force = Vector3.zero,
            hitNormal = Vector3.zero,
            IsInside = false,
            MPa = 1f,
            MPaRootMeter = 1f,
            point = this.transform.position,
            PointsDamage = 250f,
            ShotOrigin = (Transform) null,
            strikeDir = Vector3.zero,
            uvCoords = Vector2.zero,
            IsInitialContact = true
          });
        }
      }
      else
      {
        this.m_TickDownTilSpout -= Time.deltaTime;
        if ((double) this.m_TickDownTilSpout > 0.0)
          return;
        float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position);
        if ((double) num < 15.0)
          SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_Spout, this.transform.position, num / 343f);
        this.m_TickDownTilSpout = Random.Range(8f, 25f);
        this.m_isSpouting = true;
        this.PSystem_Ambient.gameObject.SetActive(false);
        ParticleSystem.EmissionModule emission = this.PSystem_Spout.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 20f;
        rate.constantMin = 20f;
        emission.rate = rate;
      }
    }
  }
}
