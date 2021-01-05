// Decompiled with JetBrains decompiler
// Type: FistVR.MG_SmashyStool
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_SmashyStool : MonoBehaviour
  {
    public Transform TopStool;
    public LayerMask PlayerLM;
    private RaycastHit m_hit;
    public AudioEvent AudEvent_Smash;
    private float m_tick = 1f;
    private float m_MaxTick = 10f;
    private Vector3 retractedPos = new Vector3(0.0f, 2.1f, 0.0f);
    private bool m_isRetracting;
    public ParticleSystem Sparks;
    private bool m_isPlayingSound;

    private void Awake()
    {
      this.m_tick = Random.Range(8f, 20f);
      this.transform.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360f), 0.0f);
    }

    private void Update()
    {
      this.m_tick -= Time.deltaTime;
      if ((double) this.m_tick > 1.0 / 1000.0 && (double) this.m_tick < 0.699999988079071 && (!this.m_isPlayingSound && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < 15.0))
      {
        this.m_isPlayingSound = true;
        SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Smash, this.transform.position);
      }
      if ((double) this.m_tick <= 0.0)
      {
        this.m_isPlayingSound = false;
        this.m_tick = Random.Range(8f, 20f);
        this.TopStool.transform.localPosition = Vector3.zero;
        this.Sparks.Emit(Random.Range(15, 30));
        FXM.InitiateMuzzleFlashLowPriority(this.transform.position, Vector3.up, Random.Range(0.5f, 2f), Color.white, Random.Range(0.25f, 0.6f));
        if (Physics.Raycast(this.TopStool.position, -Vector3.down, out this.m_hit, 2.1f, (int) this.PlayerLM, QueryTriggerInteraction.Collide) && (Object) this.m_hit.collider.attachedRigidbody != (Object) null)
        {
          FVRPlayerHitbox component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
          if ((Object) component != (Object) null)
            component.Damage(new DamageDealt()
            {
              force = Vector3.zero,
              hitNormal = Vector3.zero,
              IsInside = false,
              MPa = 1f,
              MPaRootMeter = 1f,
              point = this.transform.position,
              PointsDamage = 1000f,
              ShotOrigin = (Transform) null,
              strikeDir = Vector3.zero,
              uvCoords = Vector2.zero,
              IsInitialContact = true
            });
        }
        this.Invoke("StartRetracting", 1f);
      }
      if (!this.m_isRetracting)
        return;
      float y = this.TopStool.transform.localPosition.y + Time.deltaTime;
      if ((double) y >= (double) this.retractedPos.y)
      {
        y = this.retractedPos.y;
        this.m_isRetracting = false;
      }
      this.TopStool.transform.localPosition = new Vector3(0.0f, y, 0.0f);
    }

    private void StartRetracting() => this.m_isRetracting = true;
  }
}
