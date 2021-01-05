// Decompiled with JetBrains decompiler
// Type: FistVR.BangSnap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BangSnap : FVRPhysicalObject
  {
    private bool m_hasSploded;
    private AudioSource m_aud;
    public AudioClip Audclip_Splode;
    public AudioClip Audclip_Fizzle;
    public BangSnapFlameTrigger FlameTrigger;
    public Renderer Unsploded;
    public Renderer Sploded;
    public GameObject SplodePrefab;
    public GameObject FizzlePrefab;
    public GameObject SecondarySplodePrefab;
    public float HitThreshold = 4f;

    protected override void Awake()
    {
      base.Awake();
      this.m_aud = this.GetComponent<AudioSource>();
    }

    public override void OnCollisionEnter(Collision col)
    {
      if ((double) col.relativeVelocity.magnitude <= (double) this.HitThreshold || this.m_hasSploded)
        return;
      this.m_hasSploded = true;
      this.Splode();
      if (!((Object) col.collider.attachedRigidbody != (Object) null))
        return;
      IFVRDamageable component = col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component == null)
        return;
      component.Damage(new Damage()
      {
        Dam_Thermal = 50f,
        Dam_TotalEnergetic = 50f,
        point = col.contacts[0].point,
        hitNormal = col.contacts[0].normal,
        strikeDir = this.transform.forward
      });
    }

    private void Splode()
    {
      this.m_aud.pitch = Random.Range(0.85f, 1.05f);
      this.m_aud.PlayOneShot(this.Audclip_Splode, Random.Range(0.9f, 1f));
      Object.Destroy((Object) this.FlameTrigger.gameObject);
      this.Unsploded.enabled = false;
      this.Sploded.enabled = true;
      Object.Instantiate<GameObject>(this.SplodePrefab, this.transform.position, Quaternion.identity);
      if ((Object) this.SecondarySplodePrefab != (Object) null)
        Object.Instantiate<GameObject>(this.SecondarySplodePrefab, this.transform.position, Quaternion.identity);
      this.Invoke("KillMe", 5f);
    }

    public void Fizzle()
    {
      if (this.m_hasSploded)
        return;
      this.m_hasSploded = true;
      this.m_aud.PlayOneShot(this.Audclip_Fizzle, 1f);
      this.Unsploded.enabled = false;
      this.Sploded.enabled = true;
      Object.Instantiate<GameObject>(this.FizzlePrefab, this.transform.position, this.transform.rotation).transform.SetParent(this.transform);
      this.Invoke("KillMe", 5f);
    }

    private void KillMe() => Object.Destroy((Object) this.gameObject);
  }
}
