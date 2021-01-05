// Decompiled with JetBrains decompiler
// Type: FistVR.FVRSparkler
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRSparkler : FVRPhysicalObject
  {
    [Header("Match Config")]
    public Renderer MatchRenderer;
    public Transform Point_Top;
    public Transform Point_Bottom;
    private bool m_hasBeenLit;
    private bool m_isBurning;
    private float m_burntState;
    private float m_burnSpeed = 0.05f;
    private Collider m_curStrikeCol;
    private int m_strikeFrames;
    public GameObject FireGO;
    public ParticleSystem Fire1;
    public ParticleSystem Fire2;
    public ParticleSystem Fire3;
    public ParticleSystem Fire4;
    public ParticleSystem Fire5;
    private float m_currentBurnPoint;
    private float m_currentBreakoff;
    private bool m_isTickingDownToDeath;
    private float m_deathTimer = 5f;
    private AudioSource fireAud;

    protected override void Awake()
    {
      base.Awake();
      this.fireAud = this.FireGO.GetComponent<AudioSource>();
    }

    protected override void FVRUpdate()
    {
      if (this.m_isTickingDownToDeath && !this.IsHeld)
      {
        this.m_deathTimer -= Time.deltaTime;
        if ((double) this.m_deathTimer <= 0.0)
          Object.Destroy((Object) this.gameObject);
      }
      if (this.m_isTickingDownToDeath)
        this.fireAud.volume = Mathf.Lerp(this.fireAud.volume, 0.0f, Time.deltaTime * 2f);
      if (!this.m_isBurning)
        return;
      this.m_burntState += Time.deltaTime * this.m_burnSpeed;
      if ((double) this.m_burntState >= 1.0)
      {
        this.m_burntState = 1f;
        this.m_isBurning = false;
        this.Fire1.emission.enabled = false;
        this.Fire2.emission.enabled = false;
        this.Fire3.emission.enabled = false;
        this.Fire4.emission.enabled = false;
        this.Fire5.emission.enabled = false;
        this.m_isTickingDownToDeath = true;
        this.FireGO.layer = LayerMask.NameToLayer("NoCol");
      }
      this.m_currentBurnPoint = Mathf.Lerp(0.12f, 0.78f, this.m_burntState);
      this.MatchRenderer.material.SetFloat("_TransitionCutoff", this.m_currentBurnPoint);
      this.m_currentBreakoff = Mathf.Max(this.m_currentBreakoff, this.m_currentBurnPoint - 0.15f);
      this.MatchRenderer.material.SetFloat("_DissolveCutoff", this.m_currentBreakoff);
      this.FireGO.transform.position = Vector3.Lerp(this.Point_Top.position, this.Point_Bottom.position, this.m_burntState);
    }

    public void Ignite()
    {
      if (this.m_hasBeenLit)
        return;
      this.m_hasBeenLit = true;
      this.m_isBurning = true;
      this.FireGO.SetActive(true);
      this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);
      this.m_strikeFrames = 0;
    }

    private new void OnCollisionEnter(Collision col)
    {
      if (!this.m_hasBeenLit || !this.m_isBurning || !((Object) col.collider.attachedRigidbody != (Object) null))
        return;
      IFVRDamageable fvrDamageable = col.collider.transform.gameObject.GetComponent<IFVRDamageable>() ?? col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (fvrDamageable == null)
        return;
      fvrDamageable.Damage(new Damage()
      {
        Dam_Thermal = 50f,
        Dam_TotalEnergetic = 50f,
        point = col.contacts[0].point,
        hitNormal = col.contacts[0].normal,
        strikeDir = this.transform.forward
      });
    }
  }
}
