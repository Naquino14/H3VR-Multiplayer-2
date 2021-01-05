// Decompiled with JetBrains decompiler
// Type: FistVR.RonchRayDome
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RonchRayDome : MonoBehaviour, IFVRDamageable
  {
    public float LifeRemaining = 50000f;
    private float m_startingLife;
    public List<GameObject> SpawnOnDestruction;
    private bool m_isDestroyed;
    public Transform RayDomeSightPose;

    public void Start() => this.m_startingLife = this.LifeRemaining;

    public float GetLifeRatio() => Mathf.Clamp(this.LifeRemaining / this.m_startingLife, 0.0f, 1f);

    public void Damage(FistVR.Damage d)
    {
      float num1 = d.Dam_Blunt + d.Dam_Cutting + d.Dam_Piercing + d.Dam_Thermal;
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        num1 *= 3.25f;
      float num2;
      if ((double) num1 > 10000.0)
      {
        num2 = num1 - 10000f;
        if (d.Class == FistVR.Damage.DamageClass.Projectile)
          num2 *= 0.07f;
      }
      else
        num2 = num1 * 0.025f;
      this.LifeRemaining -= num2;
      if ((double) this.LifeRemaining > 0.0)
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.SpawnOnDestruction.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnDestruction[index], this.transform.position, Random.rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
