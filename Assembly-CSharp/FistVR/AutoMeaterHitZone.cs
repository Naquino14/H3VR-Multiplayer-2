// Decompiled with JetBrains decompiler
// Type: FistVR.AutoMeaterHitZone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AutoMeaterHitZone : MonoBehaviour, IFVRDamageable
  {
    public AutoMeater M;
    public AutoMeater.AMHitZoneType Type;
    public float ArmorThreshold = 1500f;
    public float ArmorThresholdReductionRate = 0.1f;
    public float LifeUntilFailure = 5000f;
    public GameObject SpawnOnDestruction1;
    public GameObject SpawnOnDestruction2;
    private bool m_isDestroyed;
    public List<GameObject> DisableOnDestroy = new List<GameObject>();
    public bool DestroysGunOnDestruction;
    [Header("MagazineConnection")]
    public bool IsSpecificMagazine;
    public int FirearmIndex;

    public void Damage(FistVR.Damage d)
    {
      float damTotalKinetic = d.Dam_TotalKinetic;
      float num1;
      if ((double) damTotalKinetic > (double) this.ArmorThreshold)
      {
        float num2 = damTotalKinetic - this.ArmorThreshold;
        this.ArmorThreshold = 0.0f;
        this.LifeUntilFailure -= num2;
        num1 = 0.5f;
      }
      else
      {
        this.ArmorThreshold -= damTotalKinetic * this.ArmorThresholdReductionRate;
        num1 = 1f;
      }
      if ((double) this.LifeUntilFailure < 0.0 && !this.m_isDestroyed)
      {
        this.m_isDestroyed = true;
        this.M.DestroyComponent(this.Type, this.SpawnOnDestruction1, this.SpawnOnDestruction2, this.transform, this.DestroysGunOnDestruction);
        for (int index = 0; index < this.DisableOnDestroy.Count; ++index)
        {
          if ((Object) this.DisableOnDestroy[index] != (Object) null)
            this.DisableOnDestroy[index].SetActive(false);
        }
      }
      else
        this.M.DamageEvent(this.transform.position - d.strikeDir, num1, this.Type);
    }

    public void BlowUp()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.M.DestroyComponent(this.Type, this.SpawnOnDestruction1, this.SpawnOnDestruction2, this.transform, this.DestroysGunOnDestruction);
      for (int index = 0; index < this.DisableOnDestroy.Count; ++index)
      {
        if ((Object) this.DisableOnDestroy[index] != (Object) null)
          this.DisableOnDestroy[index].SetActive(false);
      }
    }
  }
}
