// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019ThermalClip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019ThermalClip : FVRPhysicalObject
  {
    public float StoredHeat;
    public ParticleSystem PSystem;
    private ParticleSystem.EmissionModule emission;
    public LAPD2019ThermalClipTrigger Trig;
    public GameObject splode;
    public Renderer rend;

    protected override void Awake()
    {
      base.Awake();
      this.emission = this.PSystem.emission;
      this.emission.rateOverTimeMultiplier = 0.0f;
    }

    public void SetHeat(float h)
    {
      h = Mathf.Clamp(h, 0.0f, 1f);
      this.StoredHeat = h;
      this.emission.rateOverTimeMultiplier = this.StoredHeat;
    }

    private void OnTriggerEnter(Collider col)
    {
      if (!((Object) this.QuickbeltSlot == (Object) null) || !col.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger"))
        return;
      LAPD2019ThermalClipTrigger component = col.gameObject.GetComponent<LAPD2019ThermalClipTrigger>();
      if (!((Object) component != (Object) null))
        return;
      this.Trig = component;
    }

    private void OnTriggerExit(Collider col)
    {
      if (!((Object) this.Trig != (Object) null) || !col.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger") || !((Object) col.gameObject.GetComponent<LAPD2019ThermalClipTrigger>() == (Object) this.Trig))
        return;
      this.Trig = (LAPD2019ThermalClipTrigger) null;
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if (this.IsHeld || !((Object) this.QuickbeltSlot == (Object) null) || ((double) this.StoredHeat <= 0.200000002980232 || !((Object) col.collider.attachedRigidbody == (Object) null)) || (double) col.relativeVelocity.magnitude <= 1.0)
        return;
      if ((Object) this.splode != (Object) null)
        Object.Instantiate<GameObject>(this.splode, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.rend.material.SetFloat("_EmissionWeight", Mathf.Clamp(Mathf.Pow(this.StoredHeat, 1.5f), 0.0f, 1f));
      if (!((Object) this.Trig != (Object) null) || !this.Trig.Gun.LoadThermalClip(this.StoredHeat))
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
