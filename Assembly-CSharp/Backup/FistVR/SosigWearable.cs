// Decompiled with JetBrains decompiler
// Type: FistVR.SosigWearable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SosigWearable : MonoBehaviour, IFVRDamageable
  {
    public Sosig S;
    public SosigLink L;
    public List<Collider> Cols;
    public float BluntDamageTransmission = 1f;
    public float MeleeDamMult_Blunt = 1f;
    public float MeleeDamMult_Cutting = 1f;
    public float MeleeDamMult_Piercing = 1f;
    public float ColBluntDamageMult = 1f;
    public bool IsLodgeable;
    public bool IsStabbable;
    private Renderer m_rendMain;
    private bool hasRendMain;
    public Renderer OverrideRendMain;
    public List<SosigWearable.SosigWearableSlot> SlotsTaken;

    public virtual void Start()
    {
      if ((Object) this.L != (Object) null)
        this.L.SetColDamMult(this.ColBluntDamageMult);
      this.m_rendMain = this.GetComponent<Renderer>();
      if ((Object) this.m_rendMain != (Object) null)
      {
        this.hasRendMain = true;
      }
      else
      {
        if (!((Object) this.OverrideRendMain != (Object) null))
          return;
        this.m_rendMain = this.OverrideRendMain;
        this.hasRendMain = true;
      }
    }

    public void RegisterWearable(SosigLink l)
    {
      this.L = l;
      this.S = l.S;
      this.L.RegisterWearable(this);
    }

    public void Show()
    {
      if (!this.hasRendMain)
        return;
      this.m_rendMain.enabled = true;
    }

    public void Hide()
    {
      if (!this.hasRendMain)
        return;
      this.m_rendMain.enabled = false;
    }

    public virtual void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
      {
        if (!((Object) this.S != (Object) null) || (double) this.BluntDamageTransmission <= 0.00999999977648258)
          return;
        float num = 1f;
        if (this.S.IsDamResist || this.S.IsDamMult)
          num = this.S.BuffIntensity_DamResistHarm;
        if (this.S.IsFragile)
          num *= 100f;
        this.S.ProcessDamage(0.0f, 0.0f, d.Dam_Blunt * this.BluntDamageTransmission * this.S.DamMult_Blunt * num, 0.0f, d.point, this.L);
      }
      else
      {
        if (d.Class != FistVR.Damage.DamageClass.Melee || !((Object) this.L != (Object) null))
          return;
        this.L.Damage(d);
      }
    }

    public enum SosigWearableSlot
    {
      Helmet = 0,
      HeadTop = 1,
      Face = 2,
      HeadWrap = 3,
      Torso = 10, // 0x0000000A
      UpperLink = 11, // 0x0000000B
      LowerLink = 12, // 0x0000000C
      Backpack = 20, // 0x00000014
    }
  }
}
