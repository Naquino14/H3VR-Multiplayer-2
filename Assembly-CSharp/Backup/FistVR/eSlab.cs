// Decompiled with JetBrains decompiler
// Type: FistVR.eSlab
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class eSlab : FVRPhysicalObject
  {
    [Header("eSlab Stuff")]
    public wwParkManager ParkManager;
    public GameObject ProxyDisc;
    private bool m_isDiscLoaded;
    private bool m_wasPlaying;
    public Image[] TargetSprites;
    public AudioSource Aud;
    private FVRObject m_discObject;
    public Transform DiscEjectPoint;
    public float insertCooldown;

    protected override void Awake()
    {
      base.Awake();
      this.ProxyDisc.SetActive(false);
    }

    public bool LoadDisc(eSlabDisc disc)
    {
      if (this.m_isDiscLoaded)
        return false;
      this.m_isDiscLoaded = true;
      this.ProxyDisc.SetActive(true);
      this.Aud.clip = disc.Clip;
      this.m_discObject = disc.ObjectWrapper;
      this.PlayDisc();
      return true;
    }

    private void PlayDisc()
    {
      this.ParkManager.PASystem.EngageSuppressedMode();
      this.Aud.Play();
    }

    private void StopDisc()
    {
      if (!this.Aud.isPlaying)
        return;
      this.Aud.Stop();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.insertCooldown > 0.0)
        this.insertCooldown -= Time.deltaTime;
      if (!this.Aud.isPlaying && this.m_wasPlaying)
        this.ParkManager.PASystem.DisEngageSuppressedMode();
      this.m_wasPlaying = this.Aud.isPlaying;
    }

    private void EjectDisc()
    {
      if (!this.m_isDiscLoaded)
        return;
      this.StopDisc();
      this.m_isDiscLoaded = false;
      this.insertCooldown = 1f;
      this.ProxyDisc.SetActive(false);
      Object.Instantiate<GameObject>(this.m_discObject.GetGameObject(), this.DiscEjectPoint.position, this.DiscEjectPoint.rotation).GetComponent<Rigidbody>().velocity = -this.DiscEjectPoint.right * 3f;
      this.m_discObject = (FVRObject) null;
      this.Aud.clip = (AudioClip) null;
    }

    public void UpdateSprites(Sprite sa, Sprite sb, Sprite sc, Sprite sd)
    {
      if ((Object) sa != (Object) null)
      {
        this.TargetSprites[0].sprite = sa;
        this.TargetSprites[0].color = Color.white;
      }
      else
      {
        this.TargetSprites[0].sprite = (Sprite) null;
        this.TargetSprites[0].color = Color.clear;
      }
      if ((Object) sb != (Object) null)
      {
        this.TargetSprites[1].sprite = sb;
        this.TargetSprites[1].color = Color.white;
      }
      else
      {
        this.TargetSprites[1].sprite = (Sprite) null;
        this.TargetSprites[1].color = Color.clear;
      }
      if ((Object) sc != (Object) null)
      {
        this.TargetSprites[2].sprite = sc;
        this.TargetSprites[2].color = Color.white;
      }
      else
      {
        this.TargetSprites[2].sprite = (Sprite) null;
        this.TargetSprites[2].color = Color.clear;
      }
      if ((Object) sd != (Object) null)
      {
        this.TargetSprites[3].sprite = sd;
        this.TargetSprites[3].color = Color.white;
      }
      else
      {
        this.TargetSprites[3].sprite = (Sprite) null;
        this.TargetSprites[3].color = Color.clear;
      }
    }

    private new void OnCollisionEnter(Collision col)
    {
      if ((double) col.relativeVelocity.magnitude <= 2.0)
        return;
      this.EjectDisc();
    }
  }
}
