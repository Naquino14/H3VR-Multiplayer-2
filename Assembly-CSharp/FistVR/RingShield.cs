// Decompiled with JetBrains decompiler
// Type: FistVR.RingShield
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RingShield : MonoBehaviour
  {
    public Renderer ShieldGeo;
    private float weight;
    private bool isOn;
    public Cubegame Game;
    private AudioSource aud;

    private void Awake() => this.aud = this.GetComponent<AudioSource>();

    private void OnCollisionEnter(Collision col)
    {
      if (!(col.collider.gameObject.tag != "Harmless"))
        return;
      if (!this.isOn)
      {
        this.isOn = true;
        this.ShieldGeo.gameObject.SetActive(true);
      }
      this.weight = 1f;
      this.Game.DamagePlayer();
      this.aud.PlayOneShot(this.aud.clip);
    }

    private void Update()
    {
      this.ShieldGeo.material.SetFloat("_TintWeight", this.weight);
      if ((double) this.weight > 0.00999999977648258)
      {
        this.weight = Mathf.Lerp(this.weight, 0.0f, Time.deltaTime * 6f);
      }
      else
      {
        this.weight = 0.0f;
        this.isOn = false;
        this.ShieldGeo.gameObject.SetActive(false);
      }
    }
  }
}
